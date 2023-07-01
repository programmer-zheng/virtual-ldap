using Abp.BackgroundJobs;
using Newtonsoft.Json;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.Jobs.EventInfo;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录用户离职
    /// </summary>
    public class UserLeaveOrgEventHandler : DingdingBaseEventHandler
    {
        private readonly IUserAppService _userAppService;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public UserLeaveOrgEventHandler(IBackgroundJobManager backgroundJob, IUserAppService userAppService)
        {
            _backgroundJobManager = backgroundJob;
            _userAppService = userAppService;
        }

        public override async Task Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<UserLeaveOrgEvent>(msg);
            if (eventInfo != null)
            {
                try
                {
                    foreach (var userId in eventInfo.ID)
                    {
                        await _userAppService.RemoveUser(userId);
                        await _userAppService.UpdateUserDepartmentRelations(userId, null);
                        //移除爱快vpn账号
                        _backgroundJobManager.Enqueue<IKuaiSyncAccountBackgroundJob, string>(userId);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("处理用户离职时发生异常", e);
                }
            }
        }
    }
}