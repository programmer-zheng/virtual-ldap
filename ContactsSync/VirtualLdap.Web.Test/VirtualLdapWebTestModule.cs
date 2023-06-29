using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using VirtualLdap.Web.Startup;

namespace VirtualLdap.Web.Test;

[DependsOn(
    typeof(WebModule),
    typeof(AbpAspNetCoreTestBaseModule)
)]
public class VirtualLdapWebTestModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(VirtualLdapWebTestModule).GetAssembly());
    }

    public override void PostInitialize()
    {
        IocManager.Resolve<ApplicationPartManager>()
            .AddApplicationPartsIfNotAddedBefore(typeof(WebModule).Assembly);
    }
}