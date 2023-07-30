using Newtonsoft.Json;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.DingDingUtils;
using VirtualLdap.Application.Jobs.EventInfo;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 员工角色信息发生变更
    /// </summary>
    public class LabelUserChangeEventHandler : DingdingBaseEventHandler
    {
        private readonly IDingTalkAppService _dingDingAppService;
        private readonly IUserAppService _userAppService;

        public LabelUserChangeEventHandler(
            IDingTalkAppService dingDingAppService, IUserAppService userAppService) 
        {
            _dingDingAppService = dingDingAppService;
            _userAppService = userAppService;
        }

        public override async Task Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<LabelUserChangeEvent>(msg);
            if (eventInfo != null)
            {
                foreach (var userid in eventInfo.ID)
                {
                    var dingDingUser = _dingDingAppService.GetUserDetail(userid);

                    var dbUser = await _userAppService.GetByIdAsync(userid);
                    if (dbUser != null)
                    {
                        dbUser.IsAdmin = IsAdmin(dingDingUser);
                        if (!dbUser.AccountEnabled && dbUser.IsAdmin)
                        {
                            dbUser.AccountEnabled = true;
                        }

                        await _userAppService.UpdateUserAsync(dbUser);
                    }
                }
            }
        }
    }
}