using Newtonsoft.Json;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.DingDingUtils;
using VirtualLdap.Application.Jobs.EventInfo;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录企业部门修改
    /// </summary>
    public class OrgDeptModifyEventHandler : DingdingBaseEventHandler
    {
        private readonly IDingTalkAppService _dingDingAppService;
        private readonly IDepartmentAppService _departmentAppService;

        public OrgDeptModifyEventHandler(IDingTalkAppService dingDingAppService, 
            IDepartmentAppService departmentAppService) 
        {
            _dingDingAppService = dingDingAppService;
            _departmentAppService = departmentAppService;
        }

        public override async Task Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<OrgDeptModifyEvent>(msg);
            if (eventInfo != null)
            {
                foreach (var deptId in eventInfo.ID)
                {
                    try
                    {
                        var dept = _dingDingAppService.GetDepartmentDetail(deptId);
                        var deptEntity = ObjectMapper.Map<DepartmentEntity>(dept);
                        _departmentAppService.UpdateDepartmentAsync(deptEntity);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("修改部门信息时发生异常", e);
                    }
                }
            }
        }
    }
}