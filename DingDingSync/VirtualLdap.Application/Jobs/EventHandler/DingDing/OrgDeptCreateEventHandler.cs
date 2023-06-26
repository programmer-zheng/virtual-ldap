using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Castle.Core.Logging;
using VirtualLdap.Application.DingDingUtils;
using VirtualLdap.Application.Jobs.EventInfo;
using VirtualLdap.Core.Entities;
using Newtonsoft.Json;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录企业部门创建
    /// </summary>
    public class OrgDeptCreateEventHandler : DingdingBaseEventHandler
    {
        private readonly IDingTalkAppService _dingdingAppService;
        private readonly IObjectMapper _objectMapper;
        private readonly IRepository<DepartmentEntity, long> _departmentRepository;

        public OrgDeptCreateEventHandler(IDingTalkAppService dingdingAppService,
            IObjectMapper objectMapper,
            IRepository<DepartmentEntity, long> departmentRepository, ILogger logger) : base(logger)
        {
            _dingdingAppService = dingdingAppService;
            _objectMapper = objectMapper;
            _departmentRepository = departmentRepository;
        }

        public override void Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<OrgDeptCreateEvent>(msg);
            if (eventInfo != null && eventInfo.ID.Count > 0)
            {
                foreach (var deptId in eventInfo.ID)
                {
                    try
                    {
                        var dept = _dingdingAppService.GetDepartmentDetail(deptId);

                        var deptEntity = _objectMapper.Map<DepartmentEntity>(dept);

                        _departmentRepository.Insert(deptEntity);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("新增部门时发生异常", e);
                    }
                }
            }
        }
    }
}