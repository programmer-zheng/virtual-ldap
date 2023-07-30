using Abp.Domain.Repositories;
using Newtonsoft.Json;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.Jobs.EventInfo;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录企业部门删除
    /// </summary>
    public class OrgDeptRemoveEventHandler : DingdingBaseEventHandler
    {
        private readonly IDepartmentAppService _departmentAppService;

        public OrgDeptRemoveEventHandler(IDepartmentAppService departmentAppService)
        {
            _departmentAppService = departmentAppService;
        }

        public override async Task Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<OrgDeptRemoveEvent>(msg);
            if (eventInfo != null && eventInfo.ID.Count > 0)
            {
                try
                {
                    foreach (var deptId in eventInfo.ID)
                    {
                        //部门删除时，钉钉会提醒删除部门下的人员，有人员时无法删除部门，这里只需要删除部门表的数据及部门人员关系表数据
                        await _departmentAppService.RemoveDepartmentAsync(deptId);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("删除部门时发生异常", e);
                }
            }
        }
    }
}