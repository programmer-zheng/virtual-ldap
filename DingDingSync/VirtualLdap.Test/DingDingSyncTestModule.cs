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
    typeof(DingDingSyncApplicationModule),
    typeof(DingDingSyncEntityFramworkModule),
    typeof(AbpTestBaseModule))]
public class DingDingSyncTestModule : AbpModule
{

    public override void PreInitialize()
    {
        Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        SetupInMemoryDb();
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(DingDingSyncTestModule).GetAssembly());
    }

    private void SetupInMemoryDb()
    {
        var services = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase();

        var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
            IocManager.IocContainer,
            services
        );

        var builder = new DbContextOptionsBuilder<DingDingSyncDbContext>();
        builder.UseInMemoryDatabase("Test").UseInternalServiceProvider(serviceProvider);

        IocManager.IocContainer.Register(
            Component
                .For<DbContextOptions<DingDingSyncDbContext>>()
                .Instance(builder.Options)
                .LifestyleSingleton()
        );
    }


}