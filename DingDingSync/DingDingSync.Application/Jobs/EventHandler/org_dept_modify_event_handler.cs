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
    /// 通讯录企业部门修改
    /// </summary>
    public class org_dept_modify_event_handler : DingdingBaseEventHandler
    {
        public org_dept_modify_event_handler(IRepository<DepartmentEntity, long> departmentRepository,
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
            var eventinfo = JsonConvert.DeserializeObject<org_dept_modify_event>(msg);
            if (eventinfo != null && eventinfo.ID != null)
            {
                foreach (var deptid in eventinfo.ID)
                {
                    var dingdingDept = _dingdingAppService.GetDepartmentDetail(deptid);
                    var dbDept = _departmentRepository.FirstOrDefault(deptid);
                    if (dbDept != null)
                    {
                        dbDept.DeptName = dingdingDept.Name;
                        dbDept.ParentId = dingdingDept.ParentId;
                        _departmentRepository.Update(dbDept);
                    }
                }
            }
        }
    }
}