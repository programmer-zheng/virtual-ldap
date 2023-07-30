using Newtonsoft.Json;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.DingDingUtils;
using VirtualLdap.Application.Jobs.EventInfo;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录企业部门创建
    /// </summary>
    public class OrgDeptCreateEventHandler : DingdingBaseEventHandler
    {
        private readonly IDingTalkAppService _dingdingAppService;

        private readonly IDepartmentAppService _departmentAppService;

        public OrgDeptCreateEventHandler(IDingTalkAppService dingdingAppService,
            IDepartmentAppService departmentAppService)
        {
            _dingdingAppService = dingdingAppService;
            _departmentAppService = departmentAppService;
        }

        public override async Task Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<OrgDeptCreateEvent>(msg);
            if (eventInfo != null && eventInfo.ID.Count > 0)
            {
                foreach (var deptId in eventInfo.ID)
                {
                    try
                    {
                        var dept = _dingdingAppService.GetDepartmentDetail(deptId);

                        var deptEntity = ObjectMapper.Map<DepartmentEntity>(dept);

                        _departmentAppService.AddDepartmentAsync(deptEntity);
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