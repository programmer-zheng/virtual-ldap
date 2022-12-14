using System;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using DingDingSync.Application;
using DingDingSync.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;

namespace DingDingSync.Web.Startup
{
    [DependsOn(
        typeof(DingDingSyncApplicationModule),
        typeof(DingDingSyncEntityFramworkModule),
        typeof(AbpAspNetCoreModule))]
    public class WebModule : AbpModule
    {

        private readonly IConfiguration _configuration;
        public WebModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabled = false;
            Configuration.DefaultNameOrConnectionString = _configuration.GetConnectionString("Default");

            Configuration.Modules.AbpAspNetCore().CreateControllersForAppServices(typeof(WebModule).Assembly);
            Configuration.Modules.AbpAspNetCore().DefaultWrapResultAttribute.WrapOnSuccess = false;

            Configuration.Caching.Configure("DingDing", cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromHours(1);
            });
            Configuration.Caching.Configure("IKuai", cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromHours(1);
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(WebModule).GetAssembly());
        }

        public override void PostInitialize()
        {

            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(WebModule).Assembly);
        }
    }
}
