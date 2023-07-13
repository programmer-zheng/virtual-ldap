using Abp.Application.Services;
using Abp.UI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VirtualLdap.Application.WorkWeixinUtils;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.AppService;

public class WorkWeixinSyncContactsService : SyncContactsBase, ISyncContacts, IApplicationService
{
    public IWorkWeixinAppService WorkWeixinAppService { get; set; }

    public IUserAppService UserAppService { get; set; }

    public async Task Sync()
    {
        var defaultPassword = Configuration.GetValue<string>("DefaultPassword");
        defaultPassword = string.IsNullOrWhiteSpace(defaultPassword) ? "123456" : defaultPassword;
        try
        {
            //获取当前数据库中的部门
            var deptList = DeptRepository.GetAllList();
            //获取当前数据库中的人员信息
            var userList = UserRepository.GetAllList();
            var relaList = UserDeptRelaRepository.GetAllList();

            var newUserList = new List<UserEntity>();
            var newDeptList = new List<DepartmentEntity>();
            var newRelaList = new List<UserDepartmentsRelationEntity>();
            var activatedUserIdList = new List<string>();
            var departmentList = await WorkWeixinAppService.GetDepartmentList();
            var unactivatedUserIdList = userList.Where(t => t.Active == false).Select(t => t.Id).ToList();

            Logger.Debug($"企业微信返回部门信息：{JsonConvert.SerializeObject(departmentList)}");

            foreach (var department in departmentList)
            {
                if (!deptList.Any(t => t.Id == department.Id))
                {
                    newDeptList.Add(ObjectMapper.Map<DepartmentEntity>(department));
                }

                var users = await WorkWeixinAppService.GetUserList(department.Id);
                Logger.Debug($"企业微信返回部门【{department.Name}】中的人员信息：{string.Join("、", users.Select(t => t.Name).ToList())}");


                var deptActivatedUserIdList = users.Where(t => t.Status == 1 && unactivatedUserIdList.Contains(t.Userid))
                    .Select(t => t.Userid).ToList();
                activatedUserIdList.AddRange(deptActivatedUserIdList);
                foreach (var user in users)
                {
                    if (!userList.Any(t => t.Id == user.Userid))
                    {
                        var userEntity = ObjectMapper.Map<UserEntity>(user);
                        var isAdmin = user.Isleader;
                        userEntity.IsAdmin = isAdmin;
                        userEntity.AccountEnabled = isAdmin;
                        userEntity.Password = defaultPassword.DesEncrypt();
                        newUserList.Add(userEntity);
                    }

                    //部门人员关系数据
                    if (!relaList.Any(t => t.UserId == user.Userid && t.DeptId == department.Id))
                    {
                        newRelaList.Add(new UserDepartmentsRelationEntity
                            { Id = Guid.NewGuid().ToString(), UserId = user.Userid, DeptId = department.Id });
                    }
                }
            }

            await UserAppService.SetUserActivted(activatedUserIdList);
            ProcessNewData(newUserList, newDeptList, newRelaList);
        }
        catch (UserFriendlyException e)
        {
            Logger.Error("调用企业微信接口同步组织架构时发生异常", e);
        }
        catch (Exception e)
        {
            throw new UserFriendlyException("同步数据发生未知异常", e);
        }
    }
}