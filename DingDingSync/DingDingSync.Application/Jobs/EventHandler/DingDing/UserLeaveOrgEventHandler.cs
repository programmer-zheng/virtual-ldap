using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Castle.Core.Logging;
using DingDingSync.Application.Jobs.EventInfo;
using DingDingSync.Core.Entities;
using Newtonsoft.Json;

namespace DingDingSync.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录用户离职
    /// </summary>
    public class UserLeaveOrgEventHandler : DingdingBaseEventHandler
    {
        private readonly IRepository<UserEntity, string> _userRepository;
        private readonly IRepository<UserDepartmentsRelationEntity, string> _deptUserRelaRepository;

        private readonly IBackgroundJobManager _backgroundJobManager;

        public UserLeaveOrgEventHandler(IRepository<UserEntity, string> userRepository,
            IRepository<UserDepartmentsRelationEntity, string> deptUserRelaRepository,
            ILogger logger, IBackgroundJobManager backgroundJob) : base(logger)
        {
            _userRepository = userRepository;
            _deptUserRelaRepository = deptUserRelaRepository;
            _backgroundJobManager = backgroundJob;
        }

        public override void Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<UserLeaveOrgEvent>(msg);
            if (eventInfo != null)
            {
                try
                {
                    //先查询启用vpn账号的
                    var users = _userRepository.GetAll().Where(t => eventInfo.ID.Contains(t.Id) && t.VpnAccountEnabled)
                        .Select(t => t.UserName).ToList();

                    //批量删除用户
                    _userRepository.Delete(t => eventInfo.ID.Contains(t.Id));

                    //批量删除部门关系
                    _deptUserRelaRepository.Delete(t => eventInfo.ID.Contains(t.UserId));

                    foreach (var userId in eventInfo.ID)
                    {
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