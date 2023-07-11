using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.EntityFrameworkCore;
using Abp.Extensions;
using Castle.Facilities.Logging;
using VirtualLdap.Application;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.DingDingUtils;
using VirtualLdap.Application.IKuai;
using VirtualLdap.Application.WorkWeixinUtils;
using VirtualLdap.EntityFrameworkCore;

namespace VirtualLdap.Web.Startup
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IHostEnvironment Environment { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAbpDbContext<VirtualLdapDbContext>(options =>
            {
                VirtualLdapDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
            });

            services.AddSingleton<CheckTokenFilterAttribute>();

            ConfigureExternalServices(services);

            services.AddControllersWithViews().AddXmlSerializerFormatters(); //.AddRazorRuntimeCompilation();
            return services.AddAbp<WebModule>(options =>
            {
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(facility =>
                    facility.UseAbpLog4Net()
                        .WithConfig(Environment.IsDevelopment() ? "log4net.Development.config" : "log4net.config"));
            });
        }

        /// <summary>
        /// 配置外部服务（爱快、钉钉、企业微信）
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="Exception">配置不正确将引发异常，并终止应用程序</exception>
        private void ConfigureExternalServices(IServiceCollection services)
        {
            // 添加爱快配置并验证
            services.AddOptions<IKuaiConfigOptions>().Bind(Configuration.GetSection(IKuaiConfigOptions.IKuai))
                .ValidateDataAnnotations()
                .Validate(config =>
                {
                    if (config.IsEnabled)
                    {
                        if (config.OpenId.IsNullOrWhiteSpace() || config.Gwid.IsNullOrWhiteSpace() || config.Open_Rsa_Pubkey.IsNullOrWhiteSpace())
                        {
                            return false;
                        }

                        return true;
                    }

                    return true;
                }, "爱快路由器账号同步功能已启用，但未正确配置爱快开放平台相关参数").ValidateOnStart();

            var workEnv = Configuration["WorkEnv"];
            if (workEnv.Equals("DingDing", StringComparison.OrdinalIgnoreCase))
            {
                // 钉钉相关配置
                services.AddOptions<DingTalkConfigOptions>().Bind(Configuration.GetSection(DingTalkConfigOptions.DingDing))
                    .ValidateDataAnnotations().ValidateOnStart();
                services.AddScoped<IMessageProvider, DingTalkAppService>();
                services.AddScoped<ISyncContacts, DingDingSyncContactsService>();
                // 注册钉钉回调
                services.RegisterDingDingEventHandler();
            }
            else if (workEnv.Equals("WorkWeixin", StringComparison.OrdinalIgnoreCase))
            {
                // 企业微信相关配置
                services.AddOptions<WorkWeixinConfigOptions>().Bind(Configuration.GetSection(WorkWeixinConfigOptions.WorkWeixin))
                    .ValidateDataAnnotations().ValidateOnStart();
                services.AddScoped<IMessageProvider, WorkWeixinAppService>();
                services.AddScoped<ISyncContacts, WorkWeixinSyncContactsService>();
                // 注册企业微信回调
                services.RegisterWorkWeiXinEventHandler();
            }
            else
            {
                throw new Exception("目前只支持钉钉(DingDing)、企业微信(WorkWeixin)，请正确填写 WorkEnv");
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            app.UseAbp();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days.
                // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            //app.UseMiddleware<CheckTokenMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}