using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Dependency;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VirtualLdap.Application.DingDingUtils;
using VirtualLdap.Application.IKuai;
using VirtualLdap.Application.WorkWeixinUtils;
using VirtualLdap.EntityFrameworkCore;

namespace VirtualLdap.Web.Test;

public class Startup
{
    private readonly IConfigurationRoot _appConfiguration;

    public Startup(IWebHostEnvironment env)
    {
        _appConfiguration = env.GetAppConfiguration();
    }

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.AddEntityFrameworkInMemoryDatabase();
        services.AddMvc();
        services.AddLogging();
        services.AddOptions<IKuaiConfigOptions>().Bind(_appConfiguration.GetSection(IKuaiConfigOptions.IKuai))
            .ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<DingTalkConfigOptions>().Bind(_appConfiguration.GetSection(DingTalkConfigOptions.DingDing))
            .ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<WorkWeixinConfigOptions>().Bind(_appConfiguration.GetSection(WorkWeixinConfigOptions.WorkWeixin))
            .ValidateDataAnnotations().ValidateOnStart();
        // IdentityRegistrar.Register(services);
        // AuthConfigurer.Configure(services, _appConfiguration);
        //
        // services.AddScoped<IWebResourceManager, WebResourceManager>();

        //Configure Abp and Dependency Injection
        return services.AddAbp<VirtualLdapWebTestModule>(options => { options.SetupTest(); });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        UseInMemoryDb(app.ApplicationServices);

        app.UseAbp(); //Initializes ABP framework.

        app.UseExceptionHandler("/Error");

        app.UseStaticFiles();
        app.UseRouting();

        app.UseAuthentication();

        // app.UseJwtTokenMiddleware();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}"); });
    }

    private void UseInMemoryDb(IServiceProvider serviceProvider)
    {
        var builder = new DbContextOptionsBuilder<VirtualLdapDbContext>();
        builder.UseInMemoryDatabase(Guid.NewGuid().ToString()).UseInternalServiceProvider(serviceProvider);
        var options = builder.Options;

        var iocManager = serviceProvider.GetRequiredService<IIocManager>();
        iocManager.IocContainer
            .Register(
                Component.For<DbContextOptions<VirtualLdapDbContext>>()
                    .Instance(options)
                    .LifestyleSingleton()
            );
    }
}