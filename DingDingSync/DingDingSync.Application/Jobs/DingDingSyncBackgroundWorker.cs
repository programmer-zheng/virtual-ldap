using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Castle.Core.Logging;
using DingDingSync.Application.AppService;

namespace DingDingSync.Application.Jobs
{
    public class DingDingSyncBackgroundWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        public IUserAppService UserAppService { get; set; }

        public DingDingSyncBackgroundWorker(AbpAsyncTimer timer) : base(timer)
        {
            timer.Period = 1000 * 10;
        }

        [UnitOfWork]
        protected override async Task DoWorkAsync()
        {
            Timer.Stop();
            Logger.Debug("同步工作开始……");
            try
            {
                await UserAppService.SyncDepartMentAndUserFromWorkWeixin();
            }
            catch (Exception e)
            {
                Logger.Error("同步发生异常", e);
            }

            Logger.Debug("同步工作结束……");
        }
    }
}