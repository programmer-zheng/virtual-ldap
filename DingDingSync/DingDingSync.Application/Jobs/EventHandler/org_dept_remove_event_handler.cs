using System;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Application.Jobs;
using DingDingSync.Application.Jobs.EventInfo;
using DingDingSync.Core.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Castle.Core.Logging;
using DingDingSync.Application.AppService;

namespace DingDingSync.Application.Jobs.EventHandler
{
    /// <summary>
    /// 通讯录企业部门删除
    /// </summary>
    public class org_dept_remove_event_handler : DingdingBaseEventHandler
    {
        public org_dept_remove_event_handler(IRepository<DepartmentEntity, long> departmentRepository,
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
            var classname = GetType().Name;
            var eventinfo = JsonConvert.DeserializeObject<org_dept_remove_event>(msg);
            if (eventinfo != null && eventinfo.ID != null && eventinfo.ID.Count > 0)
            {
                //部门删除时，钉钉会提醒删除部门下的人员，有人员时无法删除部门，这里只需要删除部门表的数据
                _departmentRepository.Delete(t => eventinfo.ID.Contains(t.Id));
            }
        }
    }
}