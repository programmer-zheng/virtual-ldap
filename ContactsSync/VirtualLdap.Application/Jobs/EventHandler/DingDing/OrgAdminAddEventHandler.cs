using Newtonsoft.Json;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.Jobs.EventInfo;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录用户被设为管理员
    /// </summary>
    public class OrgAdminAddEventHandler : DingdingBaseEventHandler
    {
        private readonly IUserAppService _userAppService;

        public OrgAdminAddEventHandler(IUserAppService userAppService) 
        {
            _userAppService = userAppService;
        }

        public override async Task Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<OrgAdminAddEvent>(msg);
            if (eventInfo != null)
            {
                foreach (var userid in eventInfo.ID)
                {
                    var dbUser = await _userAppService.GetByIdAsync(userid);
                    if (dbUser != null)
                    {
                        dbUser.IsAdmin = true;
                        await _userAppService.UpdateUser(dbUser);
                    }
                }
            }
        }
    }
}