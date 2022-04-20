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
    /// 企业被解散
    /// </summary>
    public class org_remove_event_handler : DingdingBaseEventHandler
    {
        public org_remove_event_handler(IRepository<DepartmentEntity, long> departmentRepository,
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
            var eventinfo = JsonConvert.DeserializeObject<org_remove_event>(msg);
            Console.WriteLine("企业解散...");
            Console.WriteLine($"{classname}:{string.Join(",", eventinfo.ID)}");
        }
    }
}