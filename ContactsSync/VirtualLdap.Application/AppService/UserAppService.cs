using System.Text;
using System.Text.RegularExpressions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using Abp.UI;
using Castle.Core.Logging;
using VirtualLdap.Application.AppService.Dtos;
using VirtualLdap.Core.Entities;
using Microsoft.Extensions.Configuration;
using TinyPinyin;

namespace VirtualLdap.Application.AppService
{
    public class UserAppService : IUserAppService
    {
        public IRepository<UserEntity, string> UserRepository { get; set; }

        public IRepository<DepartmentEntity, long> DeptRepository { get; set; }

        public IMessageProvider MessageProvider { get; set; }

        public IRepository<UserDepartmentsRelationEntity, string> UserDeptRelaRepository { get; set; }

        public ILogger Logger { get; set; }

        public IConfiguration Configuration { get; set; }

        public ICacheManager CacheManager { get; set; }

        public List<DeptUserDto> DeptUsers(long deptId)
        {
            var emailSuffix = Configuration.GetValue<string>("EmailSuffix");
            var users = (from user in UserRepository.GetAll()
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
                    Department = new List<long> { rela.DeptId }
                }).ToList();
            return users;
        }

        /// <summary>
        /// 获取当前部门的子部门ID（包含二级、三级等）
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

        public List<DeptUserDto> GetAdminDeptUsers(string adminUserId)
        {
            var depts = DeptRepository.GetAll().ToList();

            var adminDepts = UserDeptRelaRepository.GetAll().Where(t => t.UserId == adminUserId)
                .Select(t => t.DeptId).ToList();
            var deptIds = GetDeptIds(adminDepts, depts);
            var users = (from user in UserRepository.GetAll()
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
                }).ToList();
            return users;
        }

        public async Task AddUser(UserEntity dto)
        {
            await UserRepository.InsertAsync(dto);
        }

        public async Task UpdateUserDepartmentRelations(string userId, List<long> depIdList)
        {
            await UserDeptRelaRepository.HardDeleteAsync(t => t.UserId == userId);
            if (depIdList != null)
            {
                foreach (var deptId in depIdList)
                {
                    await UserDeptRelaRepository.InsertAsync(new UserDepartmentsRelationEntity()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = userId, DeptId = deptId
                    });
                }
            }
        }

        public async Task RemoveUser(string id)
        {
            await UserRepository.DeleteAsync(id);
            await UserDeptRelaRepository.DeleteAsync(t => t.UserId == id);
        }

        public async Task UpdateUser(UserEntity dto)
        {
            await UserRepository.UpdateAsync(dto);
        }

        public async Task<UserEntity> GetByIdAsync(string userId)
        {
            return await UserRepository.FirstOrDefaultAsync(t => t.Id == userId);
        }

        public async Task<UserEntity> GetByUserNameAsync(string username)
        {
            return await UserRepository.FirstOrDefaultAsync(t =>
                t.UserName == username || t.Mobile == username || t.Email == username);
        }

        public async Task<bool> ResetPassword(ResetPasswordViewModel model)
        {
            try
            {
                var userinfo = await GetByIdAsync(model.UserId);
                if (userinfo == null)
                {
                    throw new UserFriendlyException("用户不存在");
                }

                if (userinfo.Password != model.OldPassword.DesEncrypt())
                {
                    throw new UserFriendlyException("当前密码不正确，无法修改密码！");
                }

                var pwd = model.NewPassword.DesEncrypt();
                UserRepository.Update(model.UserId, t =>
                {
                    t.PasswordInited = true;
                    t.Password = pwd;
                });
                return true;
            }
            catch (UserFriendlyException)
            {
                throw;
            }
            catch (Exception e)
            {
                Logger.Error($"{model.UserId} 修改密码发生错误", e);
                throw new UserFriendlyException("修改密码发生未知错误");
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

                var msgContent = $"已为您开通域账号，域账号的用户名为：{username}。";
                if (MessageProvider != null)
                {
                    await MessageProvider.SendTextMessage(userId, msgContent);
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"为{userId}启用账号发生异常", e);
                throw new UserFriendlyException($"为{userId}启用账号发生异常", e);
            }
        }

        public async Task<bool> EnableVpnAccount(string userId)
        {
            try
            {
                var userinfo = await GetByIdAsync(userId);
                if (userinfo == null)
                {
                    throw new UserFriendlyException("用户不存在");
                }

                if (!userinfo.AccountEnabled)
                {
                    throw new UserFriendlyException($"启用VPN账号失败，请先为 {userinfo.Name} 开通域账号！");
                }

                if (!userinfo.PasswordInited)
                {
                    throw new UserFriendlyException($"{userinfo.Name} 还未修改初始密码，无法启用VPN账号，请先修改默认密码！");
                }

                if (userinfo.VpnAccountEnabled)
                {
                    throw new UserFriendlyException($"{userinfo.Name} 的VPN账号已启用，无须重复操作；若无法使用VPN账号，请联系管理员！");
                }

                UserRepository.Update(userId, t => t.VpnAccountEnabled = true);

                var msgContent = $"已为您启用VPN账号，VPN的账号、密码与域账号相同。";
                await MessageProvider.SendTextMessage(userId, msgContent);
                return true;
            }
            catch (UserFriendlyException)
            {
                throw;
            }
            catch (Exception e)
            {
                Logger.Error($"为{userId}启用VPN账号发生异常", e);
                throw new UserFriendlyException($"为{userId}启用VPN账号发生异常", e);
            }
        }

        public async Task<bool> ResetAccountPassword(string userId)
        {
            var defaultPassword = string.Empty;
            if (Configuration != null)
            {
                defaultPassword = Configuration.GetValue<string>("DefaultPassword");
            }

            defaultPassword = string.IsNullOrWhiteSpace(defaultPassword) ? "123456" : defaultPassword;
            var user = UserRepository.GetAll().FirstOrDefault(t => t.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException("用户不存在");
            }

            try
            {
                user.PasswordInited = false;
                user.Password = defaultPassword.DesEncrypt();
                UserRepository.Update(user);
                var msgContent = $"您的域账号：{user.UserName}，密码已重置，默认密码为：{defaultPassword}。";
                if (MessageProvider != null)
                {
                    await MessageProvider.SendTextMessage(user.Id, msgContent);
                }

                return true;
            }
            catch (Exception e)
            {
                Logger.Error($"为{user.Name}重置密码发生异常", e);
                throw new UserFriendlyException($"为{user.Name}重置密码发生异常", e);
            }
        }

        public async Task<DeptUserDto> GetDeptUserDetail(string userid)
        {
            var userdto = (from user in UserRepository.GetAll()
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
                }).FirstOrDefault();
            return userdto;
        }

        [UnitOfWork]
        public async Task<string> GetUserName(string name, List<UserEntity>? newUserList = null)
        {
            var username = new StringBuilder();

            var regex = new Regex(@"^[\u4e00-\u9fa5]+", RegexOptions.IgnoreCase);

            if (regex.IsMatch(name))
            {
                //对重名人员，去掉人名以外字符
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
            var random = Random.Shared.Next(100000, 999999);
            await CacheManager.GetCache("DingDing").AsTyped<string, string>()
                .SetAsync($"ForgotPassword-{userid}", random.ToString());
            var msgContent = $"您正在进行忘记密码操作，验证码是：{random}。";
            await MessageProvider.SendTextMessage(userid, msgContent);
        }

        public async Task<bool> ForgotPassword(ForgotPasswordViewModel model)
        {
            var cache = CacheManager.GetCache("DingDing").AsTyped<string, string>();
            var cachedRandomCode = cache.GetOrDefault($"ForgotPassword-{model.UserId}");
            if (cachedRandomCode != model.VerificationCode)
            {
                throw new UserFriendlyException("验证码不正确，请重新输入");
            }

            await UserRepository.UpdateAsync(model.UserId, async t => t.Password = model.NewPassword.DesEncrypt());

            return true;
        }
    }
}