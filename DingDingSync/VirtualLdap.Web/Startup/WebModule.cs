using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using VirtualLdap.Application;
using VirtualLdap.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace VirtualLdap.Web.Startup
{
    [DependsOn(
        typeof(VirtualLdapApplicationModule),
        typeof(VirtualLdapEntityFramworkModule),
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
