using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using VirtualLdap.Application.AppService;

namespace VirtualLdap.Application.Jobs
{
    public class VirtualLdapBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly ISyncContacts _syncContactsAppService;

        public VirtualLdapBackgroundWorker(AbpTimer timer
            , ISyncContacts syncContactsAppService
        ) : base(timer)
        {
            timer.Period = 1000 * 10;
            _syncContactsAppService = syncContactsAppService;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            Timer.Stop();
            Logger.Debug("同步工作开始……");
            try
            {
                // _syncContactsAppService.Sync();
            }
            catch (Exception e)
            {
                Logger.Error("同步发生异常", e);
            }

            Logger.Debug("同步工作结束……");
        }
    }
}