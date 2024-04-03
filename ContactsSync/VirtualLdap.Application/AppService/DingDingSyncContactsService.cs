using Abp.Application.Services;
using Abp.UI;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VirtualLdap.Application.DingDingUtils;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.AppService;

public class DingDingSyncContactsService : SyncContactsBase, ISyncContacts, IApplicationService
{
    public IDingTalkAppService DingdingAppService { get; set; }

    public IUserAppService UserAppService { get; set; }

    public async Task SyncContactsAsync()
    {
        var defaultPassword = Configuration.GetValue<string>("DefaultPassword");
        defaultPassword = string.IsNullOrWhiteSpace(defaultPassword) ? "123456" : defaultPassword;
        try
        {
            //获取钉钉所有部门
            var depts = DingdingAppService.GetDepartmentList();
            Logger.Debug($"钉钉返回部门信息：{JsonConvert.SerializeObject(depts)}");
            //部分企业或组织在使用钉钉某些功能后，将在顶级id=1的部门下生成id为负数的部门，理应筛选掉并做日志提醒
            var deptWithNegativeId = depts.FindAll(dept => dept.Id < 1);
            if (deptWithNegativeId != null)
            {
                foreach (var item in deptWithNegativeId)
                {

                    Logger.Debug($"找到了id为负数的部门，id: {item.Id}, name: {item.Name}，现在将其删除");
                    depts.Remove(item);
                }
            }

            //获取当前数据库中的部门
            var deptList = DeptRepository.GetAllList();
            //获取当前数据库中的人员信息
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

                //获取部门详情
                var deptDetail = DingdingAppService.GetDepartmentDetail(item.Id);
                //当前部门管理人员列表
                var managerId = deptDetail.DeptManagerUseridList;
                //当前部门人员列表
                var users = DingdingAppService.GetUserList(item.Id);
                Logger.Debug($"钉钉返回部门【{item.Name}】中的人员信息：{string.Join("、", users.Select(t => t.Name).ToList())}");
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

                    //部门人员关系数据
                    if (!relaList.Any(t => t.UserId == user.Userid && t.DeptId == item.Id))
                    {
                        newRelaList.Add(new UserDepartmentsRelationEntity
                        { Id = Guid.NewGuid().ToString(), UserId = user.Userid, DeptId = item.Id });
                    }
                }
            }

            foreach (var item in newUserList.OrderBy(t => t.HiredDate))
            {
                var username = await UserAppService.GetUserNameAsync(item.Name, newUserList);

                item.UserName = username.ToString().ToLower();
            }

            ProcessNewData(newUserList, newDeptList, newRelaList);
        }
        catch (UserFriendlyException e)
        {
            Logger.Error("调用钉钉接口同步组织架构时发生异常", e);
        }
        catch (Exception e)
        {
            throw new UserFriendlyException("同步数据发生未知异常", e);
        }
    }
}