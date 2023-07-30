using System.Xml.Linq;
using VirtualLdap.Application.AppService;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.Jobs.EventHandler.WorkWeixin;

public class UpdatePartyEventHandler : WorkWeixinBaseEventHandler
{
    private readonly IDepartmentAppService _departmentAppService;

    public UpdatePartyEventHandler(IDepartmentAppService departmentAppService)
    {
        _departmentAppService = departmentAppService;
    }

    public override async Task Do(string msg)
    {
        XElement xml = XElement.Parse(msg);
        var id = xml.Element("Id").Value;
        var departmentDetail = await WorkWeixinAppService.GetDepartmentDetail(id);
        var departmentEntity = ObjectMapper.Map<DepartmentEntity>(departmentDetail);
        await _departmentAppService.UpdateDepartmentAsync(departmentEntity);
    }
}