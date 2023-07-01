using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.EntityFrameworkCore;
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
        public IConfiguration Configuration { get; }

        public IHostEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAbpDbContext<VirtualLdapDbContext>(options =>
            {
                VirtualLdapDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
            });

            services.AddSingleton<CheckTokenFilterAttribute>();

            services.Configure<IKuaiConfigOptions>(Configuration.GetSection(IKuaiConfigOptions.IKuai));
            services.Configure<DingTalkConfigOptions>(Configuration.GetSection(DingTalkConfigOptions.DingDing));
            services.Configure<WorkWeixinConfigOptions>(Configuration.GetSection(WorkWeixinConfigOptions.WorkWeixin));

            var workEnv = Configuration["WorkEnv"];
            if (workEnv.Equals("DingDing", StringComparison.OrdinalIgnoreCase))
            {
                services.AddScoped<IMessageProvider, DingTalkAppService>();            
                services.AddScoped<ISyncContacts, DingDingSyncContactsService>();
                services.RegisterDingDingEventHandler();

            }
            else if (workEnv.Equals("WorkWeixin", StringComparison.OrdinalIgnoreCase))
            {
                services.AddScoped<IMessageProvider, WorkWeixinAppService>();
                services.AddScoped<ISyncContacts, WorkWeixinSyncContactsService>();
                services.RegisterWorkWeiXinEventHandler();
            }
            else
            {
                throw new Exception("目前只支持钉钉(DingDing)、企业微信(WorkWeixin)，请正确填写 WorkEnv");
            }

            services.AddControllersWithViews().AddXmlSerializerFormatters(); //.AddRazorRuntimeCompilation();
            return services.AddAbp<WebModule>(options =>
            {
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(facility =>
                    facility.UseAbpLog4Net()
                        .WithConfig(Environment.IsDevelopment() ? "log4net.Development.config" : "log4net.config"));
            });
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