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
    /// 通讯录企业部门创建
    /// </summary>
    public class org_dept_create_event_handler : DingdingBaseEventHandler
    {
        
        protected readonly IDingdingAppService _dingdingAppService;
        protected readonly IObjectMapper _objectMapper;
        protected readonly IRepository<DepartmentEntity, long> _departmentRepository;

        public org_dept_create_event_handler(IDingdingAppService dingdingAppService, 
            IObjectMapper objectMapper, 
            IRepository<DepartmentEntity, long> departmentRepository)
        {
            _dingdingAppService = dingdingAppService;
            _objectMapper = objectMapper;
            _departmentRepository = departmentRepository;
        }

        public override void Do(string msg)
        {
            var classname = GetType().Name;
            var eventinfo = JsonConvert.DeserializeObject<org_dept_create_event>(msg);
            if (eventinfo != null && eventinfo.ID != null && eventinfo.ID.Count > 0)
            {
                foreach (var deptid in eventinfo.ID)
                {
                    var dept = _dingdingAppService.GetDepartmentDetail(deptid);

                    var deptEntity = _objectMapper.Map<DepartmentEntity>(dept);

                    _departmentRepository.Insert(deptEntity);
                }
            }
        }
    }
}