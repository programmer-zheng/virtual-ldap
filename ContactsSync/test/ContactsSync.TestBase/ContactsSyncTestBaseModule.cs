using ContactsSync.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace ContactsSync.TestBase;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(ContactsSyncDomainModule)
)]
public class ContactsSyncTestBaseModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        base.PreConfigureServices(context);
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile("appsettings.Development.json", true, true)
            .Build();
        context.Services.ReplaceConfiguration(configuration);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false; // 禁用后台任务
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        SeedTestData(context);
    }


    private static void SeedTestData(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(async () =>
        {
            using (var scope = context.ServiceProvider.CreateScope())
            {
                await scope.ServiceProvider
                    .GetRequiredService<IDataSeeder>()
                    .SeedAsync();
            }
        });
    }
}