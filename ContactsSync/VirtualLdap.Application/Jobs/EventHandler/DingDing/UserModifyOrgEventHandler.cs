using Newtonsoft.Json;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.DingDingUtils;
using VirtualLdap.Application.Jobs.EventInfo;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录用户更改
    /// </summary>
    public class UserModifyOrgEventHandler : DingdingBaseEventHandler
    {
        private readonly IDingTalkAppService _dingdingAppService;
        private readonly IUserAppService _userAppService;

        public UserModifyOrgEventHandler(
            IDingTalkAppService dingdingAppService, IUserAppService userAppService)
        {
            _dingdingAppService = dingdingAppService;
            _userAppService = userAppService;
        }

        public override async Task Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<UserModifyOrgEvent>(msg);
            if (eventInfo != null)
            {
                foreach (var userid in eventInfo.ID)
                {
                    try
                    {
                        var dingDingUser = _dingdingAppService.GetUserDetail(userid);
                        var dbUserEntity = _userAppService.GetByIdAsync(userid).Result;
                        if (dbUserEntity != null)
                        {
                            ObjectMapper.Map(dingDingUser, dbUserEntity);
                            dbUserEntity.IsAdmin = IsAdmin(dingDingUser);

                            if (!dbUserEntity.AccountEnabled && dbUserEntity.IsAdmin)
                            {
                                dbUserEntity.AccountEnabled = true;
                            }

                            _userAppService.UpdateUserAsync(dbUserEntity);
                            _userAppService.UpdateUserDepartmentRelationsAsync(userid, dingDingUser.DeptIdList);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("人员信息修改回调事件发生异常", e);
                    }
                }
            }
        }
    }
}