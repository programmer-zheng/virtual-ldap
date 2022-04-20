using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Castle.Core.Logging;
using DingDingSync.Application.AppService;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Application.Jobs.EventInfo;
using DingDingSync.Core.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DingDingSync.Application.Jobs.EventHandler
{
    /// <summary>
    /// 通讯录用户离职
    /// </summary>
    public class user_leave_org_event_handler : DingdingBaseEventHandler
    {
        public user_leave_org_event_handler(IRepository<DepartmentEntity, long> departmentRepository,
            IRepository<UserEntity, string> userRepository,
            IRepository<UserDepartmentsRelationEntity, string> deptUserRelaRepository,
            IUserAppService userAppService,
            IDepartmentAppService departmentAppService,
            IDingdingAppService dingdingAppService,
            IObjectMapper objectMapper,
            IConfiguration configuration,
            IIkuaiAppService iKuaiAppService,
            ILogger logger) : base(departmentRepository, userRepository,
            deptUserRelaRepository, userAppService, departmentAppService, dingdingAppService, objectMapper,
            configuration, iKuaiAppService, logger)
        {
        }

        public override void Do(string msg)
        {
            var eventinfo = JsonConvert.DeserializeObject<user_leave_org_event>(msg);
            if (eventinfo != null && eventinfo.ID != null)
            {
                //先查询启用vpn账号的
                var users = _userRepository.GetAll().Where(t => eventinfo.ID.Contains(t.Id) && t.VpnAccountEnabled)
                    .Select(t => t.UserName).ToList();

                //批量删除用户
                _userRepository.Delete(t => eventinfo.ID.Contains(t.Id));

                //批量删除部门关系
                _deptUserRelaRepository.Delete(t => eventinfo.ID.Contains(t.UserId));

                foreach (var username in users)
                {
                    //移除爱快vpn账号
                    var ikuaiAccount = _iKuaiAppService.GetAccountIdByUsername(username);
                    _iKuaiAppService.RemoveAccount(ikuaiAccount.id);
                }
            }
        }
    }
}