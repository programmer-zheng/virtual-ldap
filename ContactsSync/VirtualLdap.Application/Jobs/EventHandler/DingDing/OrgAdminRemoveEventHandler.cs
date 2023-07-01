using Newtonsoft.Json;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.DingDingUtils;
using VirtualLdap.Application.Jobs.EventInfo;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录用户被取消设置管理员
    /// </summary>
    public class OrgAdminRemoveEventHandler : DingdingBaseEventHandler
    {
        private readonly IDingTalkAppService _dingDingAppService;
        private readonly IUserAppService _userAppService;

        public OrgAdminRemoveEventHandler(IDingTalkAppService dingDingAppService, IUserAppService userAppService)
        {
            _dingDingAppService = dingDingAppService;
            _userAppService = userAppService;
        }

        public override async Task Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<OrgAdminRemoveEvent>(msg);
            if (eventInfo != null)
            {
                foreach (var userid in eventInfo.ID)
                {
                    try
                    {
                        var dingDingUser = _dingDingAppService.GetUserDetail(userid);

                        //被取消设置为管理员，如果是部门领导，仍是管理员身份，可查询下属人员
                        var isAdmin = (dingDingUser.LeaderInDept != null &&
                                       dingDingUser.LeaderInDept.Count(t => t.Leader) > 0);

                        var dbUser = await _userAppService.GetByIdAsync(userid);
                        if (dbUser != null)
                        {
                            dbUser.IsAdmin = isAdmin;
                            await _userAppService.UpdateUser(dbUser);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("取消设置用户管理员时发生异常", e);
                    }
                }
            }
        }
    }
}