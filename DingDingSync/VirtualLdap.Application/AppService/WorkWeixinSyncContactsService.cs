using Abp.Application.Services;
using Abp.Domain.Uow;
using Abp.UI;
using Microsoft.Extensions.Configuration;
using VirtualLdap.Application.WorkWeixinUtils;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.AppService;

public class WorkWeixinSyncContactsService : SyncContactsBase, ISyncContacts, IApplicationService
{
    public IWorkWeixinAppService WorkWeixinAppService { get; set; }

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
            var departmentList = await WorkWeixinAppService.GetDepartmentList();
            foreach (var department in departmentList)
            {
                if (!deptList.Any(t => t.Id == department.Id))
                {
                    newDeptList.Add(ObjectMapper.Map<DepartmentEntity>(department));
                }

                var users = await WorkWeixinAppService.GetUserList(department.Id);

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