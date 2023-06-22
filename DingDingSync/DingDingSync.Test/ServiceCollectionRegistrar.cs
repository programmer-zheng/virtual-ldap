using Abp.Dependency;
using Abp.EntityFrameworkCore;
using Castle.MicroKernel.Registration;
using Castle.Windsor.MsDependencyInjection;
using DingDingSync.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DingDingSync.Test;

public static class ServiceCollectionRegistrar
{
    public static void Register(IIocManager iocManager)
    {
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false);

        var configuration = configurationBuilder.Build();
        var connectionString = configuration.GetConnectionString("Default");
        var services = new ServiceCollection();
        // services.AddEntityFrameworkInMemoryDatabase();
        
        services.AddAbpDbContext<DingDingSyncDbContext>(options =>
        {
            DingDingSyncDbContextConfigurer.Configure(options.DbContextOptions, connectionString);
        });
        var serviceProvider = WindsorRegistrationHelper.CreateServiceProvider(iocManager.IocContainer, services);

        var builder = new DbContextOptionsBuilder<DingDingSyncDbContext>();
        DingDingSyncDbContextConfigurer.Configure(builder, connectionString);
        builder.UseInternalServiceProvider(serviceProvider);

        // builder.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(serviceProvider);

        iocManager.IocContainer.Register(
            Component
                .For<DbContextOptions<DingDingSyncDbContext>>()
                .Instance(builder.Options)
                .LifestyleSingleton()
        );
    }
}