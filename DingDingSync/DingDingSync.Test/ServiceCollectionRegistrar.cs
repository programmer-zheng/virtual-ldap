using System.Reflection;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Abp.Dependency;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DingDingSync.EntityFrameworkCore;

namespace DingDingSync.Test;

public static class ServiceCollectionRegistrar
{
    //public static void Register(IIocManager iocManager)
    //{
    //    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
    //        .AddJsonFile("appsettings.json", optional: false);

    //    var configuration = configurationBuilder.Build();
    //    var connectionString = configuration.GetConnectionString("Default");
    //    var services = new ServiceCollection();
    //    // services.AddEntityFrameworkInMemoryDatabase();
    //    services.AddAbpDbContext<DingDingSyncDbContext>(options =>
    //    {
    //        DingDingSyncDbContextConfigurer.Configure(options.DbContextOptions, connectionString);
    //    });
    //    var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);

    //    var builder = new DbContextOptionsBuilder<DingDingSyncDbContext>();
    //    //DingDingSyncDbContextConfigurer.Configure(builder, connectionString);
    //    builder.UseInternalServiceProvider(serviceProvider);
    //    services.AddSingleton<DbContextOptions<DingDingSyncDbContext>>(builder.Options);

    //    // builder.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(serviceProvider);

    //    iocManager.IocContainer.Register(
    //        Component
    //            .For<DbContextOptions<DingDingSyncDbContext>>()
    //            .Instance(builder.Options)
    //            .LifestyleSingleton()
    //    );
    //}

    public static void SetupInMemoryDb(IIocManager iocManager)
    {
        var services = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase();

        var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(
            iocManager.IocContainer,
            services
        );

        var builder = new DbContextOptionsBuilder<DingDingSyncDbContext>();
        builder.UseInMemoryDatabase("Test").UseInternalServiceProvider(serviceProvider);

        iocManager.IocContainer.Register(
            Component
                .For<DbContextOptions<DingDingSyncDbContext>>()
                .Instance(builder.Options)
                .LifestyleSingleton()
        );
    }
}