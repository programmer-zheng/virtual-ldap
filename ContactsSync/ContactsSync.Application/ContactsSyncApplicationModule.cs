﻿using System.Net;
using System.Transactions;
using ContactsSync.Application.AppServices;
using ContactsSync.Application.Background;
using ContactsSync.Application.DingDing;
using ContactsSync.Application.OpenPlatformProvider;
using ContactsSync.Application.WeWork;
using ContactsSync.EntityFrameworkCore;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.RegisterServices;
using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.BackgroundWorkers.Hangfire;
using Volo.Abp.Modularity;

namespace ContactsSync.Application;

[DependsOn(
    typeof(AbpAutoMapperModule),
    typeof(AbpBackgroundJobsModule),
    typeof(AbpBackgroundWorkersModule),
    // typeof(AbpBackgroundWorkersHangfireModule), //hangfire
    typeof(ContactsSyncEntityFrameworkCoreModule)
)]
public class ContactsSyncApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        var configuration = context.Services.GetConfiguration();
        var services = context.Services;

        Configure<AbpAutoMapperOptions>(options =>
        {
            // 添加当前程序集中所有的映射规则
            options.AddMaps<ContactsSyncApplicationModule>();
        });

        services.AddOptions<ContactsSyncConfigOptions>()
            .Bind(configuration.GetSection(ContactsSyncConfigOptions.Sync))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        ConfigIKuaiService(services, configuration);
        ConfigWeWorkService(services, configuration);
        ConfigDingTalkService(services, configuration);
        // // hangfire配置
        // var connectionString = configuration.GetConnectionString("Default");
        // context.Services.AddHangfire(globalConfiguration =>
        // {
        //     globalConfiguration.UseStorage(new MySqlStorage(connectionString, new MySqlStorageOptions()
        //     {
        //         TransactionIsolationLevel = IsolationLevel.ReadCommitted,
        //         QueuePollInterval = TimeSpan.FromSeconds(15),
        //         JobExpirationCheckInterval = TimeSpan.FromHours(1),
        //         CountersAggregateInterval = TimeSpan.FromMinutes(5),
        //         PrepareSchemaIfNecessary = true,
        //         DashboardJobListLimit = 50000,
        //         TransactionTimeout = TimeSpan.FromMinutes(1),
        //         TablesPrefix = "Hangfire"
        //     }));
        // });
    }


    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var configuration = context.GetConfiguration();
        // // 配置hangfire
        // app.UseHangfireDashboard(options: new DashboardOptions
        // {
        //     IsReadOnlyFunc = (_ => true),
        // });
        // // hangfire的定时任务
        // context.AddBackgroundWorkerAsync<HangfireWorker>();

        // 在异步方法 OnApplicationInitializationAsync 中注册 Worker不生效 
        var syncConfig = context.ServiceProvider.GetRequiredService<IOptions<ContactsSyncConfigOptions>>();
        if (syncConfig.Value.Enabled)
        {
            context.AddBackgroundWorkerAsync<ContactsSyncWorker>();
        }
    }


    private void ConfigIKuaiService(IServiceCollection services, IConfiguration configuration)
    {
    }

    private void ConfigWeWorkService(IServiceCollection services, IConfiguration configuration)
    {
        if ("WeWork".Equals(configuration["WorkEnv"], StringComparison.OrdinalIgnoreCase))
        {
            services.AddOptions<WeWorkConfigOptions>()
                .Bind(configuration.GetSection(WeWorkConfigOptions.WeWork))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddScoped<IOpenPlatformProvider, WeWorkProvider>();
            // // 添加盛派企业微信相关
            services.AddSenparcGlobalServices(configuration).AddSenparcWeixinServices(configuration).AddMemoryCache();

            var senparcSetting = new SenparcSetting() { };
            IRegisterService registerService = RegisterService.Start(senparcSetting).UseSenparcGlobal();
            var senparcWeixinSetting = new SenparcWeixinSetting() { };
            registerService.UseSenparcWeixin(senparcWeixinSetting, null);
            // var 
        }
    }

    private void ConfigDingTalkService(IServiceCollection services, IConfiguration configuration)
    {
        if ("DingDing".Equals(configuration["WorkEnv"], StringComparison.OrdinalIgnoreCase))
        {
            services.AddOptions<DingDingConfigOptions>()
                .Bind(configuration.GetSection(DingDingConfigOptions.DingDing))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            services.AddScoped<IOpenPlatformProvider, DingDingProvider>();
        }
    }
}