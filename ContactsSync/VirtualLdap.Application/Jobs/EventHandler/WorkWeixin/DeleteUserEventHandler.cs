using System.Xml.Linq;
using VirtualLdap.Application.AppService;

namespace VirtualLdap.Application.Jobs.EventHandler.WorkWeixin;

public class DeleteUserEventHandler : WorkWeixinBaseEventHandler
{
    private readonly IUserAppService _userAppService;

    public DeleteUserEventHandler(IUserAppService userAppService)
    {
        _userAppService = userAppService;
    }

    public override async Task Do(string msg)
    {
        XElement xml = XElement.Parse(msg);
        var userId = xml.Element("UserID").Value;
        await _userAppService.RemoveUserAsync(userId);
    }
}