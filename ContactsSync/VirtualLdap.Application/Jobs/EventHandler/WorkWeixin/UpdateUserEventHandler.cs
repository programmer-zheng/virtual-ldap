using System.Xml.Linq;
using VirtualLdap.Application.AppService;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.Jobs.EventHandler.WorkWeixin;

public class UpdateUserEventHandler : WorkWeixinBaseEventHandler
{
    private readonly IUserAppService _userAppService;

    public UpdateUserEventHandler(IUserAppService userAppService)
    {
        _userAppService = userAppService;
    }

    public override async Task Do(string msg)
    {
        XElement xml = XElement.Parse(msg);
        var userId = xml.Element("UserID").Value;
        var deptIds = xml.Element("Department").Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => Convert.ToInt64(t)).ToList();
        var userDetail = await WorkWeixinAppService.GetUserDetail(userId);
        var userEntity = ObjectMapper.Map<UserEntity>(userDetail);
        userEntity.IsAdmin = userDetail.Isleader;
        await _userAppService.UpdateUserAsync(userEntity);
        await _userAppService.UpdateUserDepartmentRelationsAsync(userEntity.Id, deptIds);
    }
}