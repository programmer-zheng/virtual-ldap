using ContactsSync.Application.AppServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace ContactsSync.Application.Background;

public class ContactsSyncWorker : AsyncPeriodicBackgroundWorkerBase
{
    public ContactsSyncWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory, IOptions<ContactsSyncConfigOptions> options) : base(timer, serviceScopeFactory)
    {
        // 通讯录同步工人定时执行周期，单位（秒），15分钟一次

        timer.Period = 1000 * 60 * options.Value.SyncPeriod;
#if DEBUG
        timer.Period = 1000 * 15;
#endif
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var contactsSync = ServiceProvider.GetRequiredService<IContactsSyncAppService>();
         await contactsSync.SyncDepartmentAndUser();
    }
}