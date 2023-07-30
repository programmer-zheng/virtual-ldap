using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using VirtualLdap.Application.AppService;

namespace VirtualLdap.Application.Jobs
{
    public class VirtualLdapBackgroundWorker : AsyncPeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly ISyncContacts _syncContactsAppService;

        public VirtualLdapBackgroundWorker(AbpAsyncTimer timer, ISyncContacts syncContactsAppService) : base(timer)
        {
            timer.Period = 1000 * 60 * 15;
#if DEBUG
            timer.Period = 1000 * 10;
#endif
            _syncContactsAppService = syncContactsAppService;
        }

        [UnitOfWork]
        protected override async Task DoWorkAsync()
        {
#if DEBUG
            Timer.Stop();
#endif
            Logger.Debug("同步工作开始……");
            try
            {
                await _syncContactsAppService.SyncContactsAsync();
            }
            catch (Exception e)
            {
                Logger.Error("同步发生异常", e);
            }

            Logger.Debug("同步工作结束……");
        }
    }
}