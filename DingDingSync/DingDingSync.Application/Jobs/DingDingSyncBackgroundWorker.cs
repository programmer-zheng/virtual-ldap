using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using Castle.Core.Logging;
using DingDingSync.Application.AppService;

namespace DingDingSync.Application.Jobs
{
    public class DingDingSyncBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        public ILogger Logger { get; set; }

        public IUserAppService UserAppService { get; set; }

        public DingDingSyncBackgroundWorker(AbpTimer timer) : base(timer)
        {
            timer.Period = 1000 * 30;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            Timer.Stop();
            Logger.Debug("同步工作开始……");
            try
            {
                UserAppService.SyncDepartmentAndUser();
            }
            catch (Exception e)
            {
                Logger.Error("同步发生异常", e);
            }

            Logger.Debug("同步工作结束……");
        }
    }
}