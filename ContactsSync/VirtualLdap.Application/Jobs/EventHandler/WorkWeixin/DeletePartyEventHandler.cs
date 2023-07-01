using System.Xml.Linq;
using VirtualLdap.Application.AppService;

namespace VirtualLdap.Application.Jobs.EventHandler.WorkWeixin;

public class DeletePartyEventHandler : WorkWeixinBaseEventHandler
{
    private readonly IDepartmentAppService _departmentAppService;

    public DeletePartyEventHandler(IDepartmentAppService departmentAppService)
    {
        _departmentAppService = departmentAppService;
    }

    public override async Task Do(string msg)
    {
        XElement xml = XElement.Parse(msg);
        var id = xml.Element("Id").Value;
        await _departmentAppService.RemoveDepartment(Convert.ToInt64(id));
    }
}