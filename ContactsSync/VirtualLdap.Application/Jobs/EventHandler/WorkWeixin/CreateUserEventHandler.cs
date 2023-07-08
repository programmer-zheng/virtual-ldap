using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using VirtualLdap.Application.AppService;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.Jobs.EventHandler.WorkWeixin;

public class CreateUserEventHandler : WorkWeixinBaseEventHandler
{
    private readonly IUserAppService _userAppService;
    private readonly IConfiguration _configuration;

    public CreateUserEventHandler(IUserAppService userAppService, IConfiguration configuration)
    {
        _userAppService = userAppService;
        _configuration = configuration;
    }

    public override async Task Do(string msg)
    {
        var defaultPassword = _configuration.GetValue<string>("DefaultPassword");
        defaultPassword = string.IsNullOrWhiteSpace(defaultPassword) ? "123456" : defaultPassword;
        
        XElement xml = XElement.Parse(msg);
        var userId = xml.Element("UserID").Value;
        var deptIds = xml.Element("Department").Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
            .Select(t => Convert.ToInt64(t)).ToList();
        var userDetail = await WorkWeixinAppService.GetUserDetail(userId);
        var userEntity = ObjectMapper.Map<UserEntity>(userDetail);
        userEntity.IsAdmin = userDetail.Isleader;
        userEntity.AccountEnabled = userDetail.Isleader;
        userEntity.Password = defaultPassword.DesEncrypt();
        var username = await _userAppService.GetUserName(userEntity.Name);

        userEntity.UserName = username;
        await _userAppService.AddUser(userEntity);
        await _userAppService.UpdateUserDepartmentRelations(userEntity.Id, deptIds);
    }
}