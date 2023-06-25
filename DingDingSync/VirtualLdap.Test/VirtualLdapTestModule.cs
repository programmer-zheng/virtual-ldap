using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using VirtualLdap.Application;
using VirtualLdap.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace VirtualLdap.Test;

[DependsOn(
    typeof(VirtualLdapApplicationModule),
    typeof(VirtualLdapEntityFramworkModule),
    typeof(AbpTestBaseModule))]
public class VirtualLdapTestModule : AbpModule
{

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        SetupInMemoryDb();
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(VirtualLdapTestModule).GetAssembly());
    }

    private void SetupInMemoryDb()
    {
        var services = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase();

        var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
            IocManager.IocContainer,
            services
        );

        var builder = new DbContextOptionsBuilder<VirtualLdapDbContext>();
        builder.UseInMemoryDatabase("Test").UseInternalServiceProvider(serviceProvider);

        IocManager.IocContainer.Register(
            Component
                .For<DbContextOptions<VirtualLdapDbContext>>()
                .Instance(builder.Options)
                .LifestyleSingleton()
        );
    }


}