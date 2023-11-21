using Hangfire;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers.Hangfire;

namespace ContactsSync.Application.Background;

public class HangfireWorker : HangfireBackgroundWorkerBase
{
    public HangfireWorker()
    {
        RecurringJobId = nameof(HangfireWorker);
        CronExpression = Cron.Minutely();
    }

    [JobDisplayName("hangfire定时任务")]
    public override async Task DoWorkAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        Logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}hangfire worker");
    }
}