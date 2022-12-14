using System.Text;
using System.Text.RegularExpressions;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Caching;
using Abp.UI;
using Castle.Core.Logging;
using DingDingSync.Application.AppService.Dtos;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TinyPinyin;

namespace DingDingSync.Application.AppService
{
    public class UserAppService : IUserAppService
    {
        public IDingdingAppService DingdingAppService { get; set; }

        public IRepository<UserEntity, string> UserRepository { get; set; }

        public IRepository<DepartmentEntity, long> DeptRepository { get; set; }

        public IRepository<UserDepartmentsRelationEntity, string> UserDeptRelaRepository { get; set; }

        public ILogger Logger { get; set; }

        public IConfiguration Configuration { get; set; }

        public IObjectMapper ObjectMapper { get; set; }

        public ICacheManager CacheManager { get; set; }

        public async Task<List<DeptUserDto>> DeptUsers(long deptId)
        {
            var emailSuffix = Configuration.GetValue<string>("EmailSuffix");
            var users = await (from user in UserRepository.GetAll()
                join rela in UserDeptRelaRepository.GetAll() on user.Id equals rela.UserId
                where rela.DeptId == deptId && user.AccountEnabled == true
                select new DeptUserDto
                {
                    Userid = user.Id,
                    Name = user.Name,
                    UserName = user.UserName,
                    Mobile = user.Mobile,
                    UnionId = user.UnionId,
                    Password = user.Password,
                    Email = user.Email == "" || user.Email == null ? $"{user.UserName}@{emailSuffix}" : user.Email,
                    Avatar = user.Avatar,
                    WorkPlace = user.WorkPlace,
                    Active = user.Active,
                    Position = user.Position,
                    JobNumber = user.JobNumber,
                    Department = new List<long> {rela.DeptId}
                }).ToListAsync();
            return users;
        }

        /// <summary>
        /// ??????????????????????????????ID??????????????????????????????
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="depts"></param>
        /// <returns></returns>
        private List<long> GetDeptIds(List<long> ids, List<DepartmentEntity> depts)
        {
            var result = new List<long>();
            var existsDepartments = depts.Where(t => ids.Contains(t.Id)).Select(t => t.Id).ToList();
            result.AddRange(existsDepartments);
            var children = depts.Where(t => ids.Contains(t.ParentId)).Select(t => t.Id).ToList();
            if (children.Count > 0)
            {
                result.AddRange(GetDeptIds(children, depts));
            }

            return result;
        }

        public async Task<List<DeptUserDto>> GetAdminDeptUsers(string adminUserId)
        {
            var depts = await DeptRepository.GetAll().ToListAsync();

            var adminDepts = await UserDeptRelaRepository.GetAll().Where(t => t.UserId == adminUserId)
                .Select(t => t.DeptId).ToListAsync();
            var deptIds = GetDeptIds(adminDepts, depts);
            var users = await (from user in UserRepository.GetAll()
                join rela in UserDeptRelaRepository.GetAll() on user.Id equals rela.UserId
                where deptIds.Contains(rela.DeptId) && user.Id != adminUserId
                orderby rela.DeptId, user.UserName
                select new DeptUserDto
                {
                    Userid = user.Id,
                    Name = user.Name,
                    UserName = user.UserName,
                    Mobile = user.Mobile,
                    UnionId = user.UnionId,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    WorkPlace = user.WorkPlace,
                    Active = user.Active,
                    Position = user.Position,
                    JobNumber = user.JobNumber,
                    HiredDate = user.HiredDate,
                    AccountEnabled = user.AccountEnabled,
                    VpnAccountEnabled = user.VpnAccountEnabled,
                }).ToListAsync();
            return users;
        }

        public async Task<UserEntity> GetByIdAsync(string userId)
        {
            return await UserRepository.GetAll().FirstOrDefaultAsync(t => t.Id == userId);
        }

        public async Task<UserEntity> GetByUserNameAsync(string username)
        {
            return await UserRepository.GetAll().FirstOrDefaultAsync(t => t.UserName == username);
        }

        public async Task SyncDepartmentAndUser()
        {
            var defaultPassword = Configuration.GetValue<string>("DefaultPassword");
            defaultPassword = string.IsNullOrWhiteSpace(defaultPassword) ? "123456" : defaultPassword;
            //????????????????????????
            var depts = DingdingAppService.GetDepartmentList();
            Logger.Debug($"???????????????????????????{JsonConvert.SerializeObject(depts)}");

            //?????????????????????????????????
            var deptList = DeptRepository.GetAllList();
            //???????????????????????????????????????
            var userList = UserRepository.GetAllList();
            var relaList = UserDeptRelaRepository.GetAllList();

            var newUserList = new List<UserEntity>();
            var newDeptList = new List<DepartmentEntity>();
            var newRelaList = new List<UserDepartmentsRelationEntity>();
            foreach (var item in depts)
            {
                if (!deptList.Any(t => t.Id == item.Id))
                {
                    var deptEntity = ObjectMapper.Map<DepartmentEntity>(item);
                    newDeptList.Add(deptEntity);
                }

                //??????????????????
                var deptDetail = DingdingAppService.GetDepartmentDetail(item.Id);
                //??????????????????????????????
                var managerId = deptDetail.DeptManagerUseridList;
                //????????????????????????
                var users = DingdingAppService.GetUserList(item.Id);
                Logger.Debug($"?????????????????????{item.Name}????????????????????????{string.Join("???", users.Select(t => t.Name).ToList())}");
                foreach (var user in users)
                {
                    if (!userList.Any(t => t.Id == user.Userid) && !newUserList.Any(t => t.Id == user.Userid))
                    {
                        var isAdmin = user.Admin || managerId != null && managerId.Contains(user.Userid);

                        var userEntity = ObjectMapper.Map<UserEntity>(user);
                        userEntity.IsAdmin = isAdmin;
                        userEntity.AccountEnabled = isAdmin;
                        userEntity.Password = defaultPassword.DesEncrypt();

                        newUserList.Add(userEntity);
                    }

                    //????????????????????????
                    if (!relaList.Any(t => t.UserId == user.Userid && t.DeptId == item.Id))
                    {
                        newRelaList.Add(new UserDepartmentsRelationEntity
                            {Id = Guid.NewGuid().ToString(), UserId = user.Userid, DeptId = item.Id});
                    }
                }
            }

            var regex = new Regex(@"^[\u4e00-\u9fa5]+", RegexOptions.IgnoreCase);
            foreach (var item in newUserList.OrderBy(t => t.HiredDate))
            {
                var username = await GetUserName(item.Name, newUserList);

                item.UserName = username.ToString().ToLower();
            }


            try
            {
                foreach (var item in newDeptList)
                {
                    DeptRepository.Insert(item);
                }

                foreach (var item in newUserList)
                {
                    UserRepository.Insert(item);
                }

                foreach (var item in newRelaList)
                {
                    UserDeptRelaRepository.Insert(item);
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException("????????????????????????", e);
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                var pwd = model.NewPassword.DesEncrypt();
                UserRepository.Update(model.UserId, t =>
                {
                    t.PasswordInited = true;
                    t.Password = pwd;
                });
                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"{model.UserId} ????????????????????????", e);
                throw new UserFriendlyException("??????????????????????????????");
            }
        }

        public async Task<bool> EnableAccount(string userId, string username)
        {
            try
            {
                UserRepository.Update(userId, t =>
                {
                    t.AccountEnabled = true;
                    t.UserName = username;
                });

                var msgContent = $"??????????????????????????????????????????????????????{username}???";
                DingdingAppService.SendTextMessage(userId, msgContent);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"???{userId}????????????????????????", e);
                throw new UserFriendlyException($"???{userId}????????????????????????", e);
            }
        }

        public async Task<bool> EnableVpnAccount(string userId)
        {
            try
            {
                UserRepository.Update(userId, t => t.VpnAccountEnabled = true);

                var msgContent = $"???????????????VPN?????????VPN???????????????????????????????????????";
                DingdingAppService.SendTextMessage(userId, msgContent);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"???{userId}??????VPN??????????????????", e);
                throw new UserFriendlyException($"???{userId}??????VPN??????????????????", e);
            }
        }

        public async Task<bool> ResetAccountPassword(string userId)
        {
            var defaultPassword = Configuration.GetValue<string>("DefaultPassword");
            defaultPassword = string.IsNullOrWhiteSpace(defaultPassword) ? "123456" : defaultPassword;
            var user = await UserRepository.GetAll().FirstOrDefaultAsync(t => t.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("???????????????");
            }

            try
            {
                user.PasswordInited = false;
                user.Password = defaultPassword.DesEncrypt();
                UserRepository.Update(user);
                var msgContent = $"??????????????????{user.UserName}???????????????????????????????????????{defaultPassword}???";
                DingdingAppService.SendTextMessage(user.Id, msgContent);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"???{user.Name}????????????????????????", e);
                throw new UserFriendlyException($"???{user.Name}????????????????????????", e);
            }
        }

        public async Task<DeptUserDto> GetDeptUserDetail(string userid)
        {
            var userdto = await (from user in UserRepository.GetAll()
                where user.Id == userid
                select new DeptUserDto
                {
                    AccountEnabled = user.AccountEnabled,
                    VpnAccountEnabled = user.VpnAccountEnabled,
                    HiredDate = user.HiredDate,
                    Userid = user.Id,
                    Name = user.Name,
                    UserName = user.UserName,
                    Mobile = user.Mobile,
                    UnionId = user.UnionId,
                    Email = user.Email,
                    Avatar = user.Avatar,
                    WorkPlace = user.WorkPlace,
                    Active = user.Active,
                    Position = user.Position,
                    JobNumber = user.JobNumber,
                }).FirstOrDefaultAsync();
            return userdto;
        }

        public async Task<string> GetUserName(string name, List<UserEntity>? newUserList = null)
        {
            var username = new StringBuilder();

            var regex = new Regex(@"^[\u4e00-\u9fa5]+", RegexOptions.IgnoreCase);

            if (regex.IsMatch(name))
            {
                //??????????????????????????????????????????
                var match = regex.Match(name);
                var pinyin = PinyinHelper.GetPinyin(match.Groups[0].Value, "").ToLower();
                username.Append(pinyin);

                var sameCount = UserRepository.Count(t => t.UserName.Contains(username.ToString()));
                var newUserSameNameCount = 0;
                if (newUserList != null)
                {
                    newUserSameNameCount = newUserList.Count(t => !string.IsNullOrWhiteSpace(t.UserName)
                                                                  && t.UserName.StartsWith(username.ToString(),
                                                                      StringComparison.OrdinalIgnoreCase));
                }

                if (sameCount > 0 || newUserSameNameCount > 0)
                {
                    username.Append(sameCount + newUserSameNameCount + 1);
                }
            }
            else
            {
                username.Append(name);
            }

            return username.ToString().ToLower();
        }

        public async Task SendVerificationCode(string userid)
        {
            var random = new Random().Next(100000, 999999);
            await CacheManager.GetCache("DingDing").AsTyped<string, string>().SetAsync($"ForgotPassword-{userid}", random.ToString());
            var msgContent = $"???????????????????????????????????????????????????{random}???";
            DingdingAppService.SendTextMessage(userid, msgContent);
        }

        public async Task<bool> ForgotPassword(ForgotPasswordViewModel model)
        {
            var cache = CacheManager.GetCache("DingDing").AsTyped<string, string>();
            var cachedRandomCode = cache.GetOrDefault($"ForgotPassword-{model.UserId}");
            if (cachedRandomCode != model.VerificationCode)
            {
                throw new UserFriendlyException("????????????????????????????????????");
            }

            await UserRepository.UpdateAsync(model.UserId, async t => t.Password = model.NewPassword.DesEncrypt());

            return true;
        }
    }
}