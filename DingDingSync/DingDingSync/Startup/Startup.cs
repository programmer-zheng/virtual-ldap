using Abp.AspNetCore;
using Abp.EntityFrameworkCore;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Abp.Castle.Logging.Log4Net;
using Castle.Facilities.Logging;

namespace DingDingSync.Web.Startup
{
    public class Startup
    {
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAbpDbContext<DingDingSyncDbContext>(options =>
            {
                Console.WriteLine($"database connection string: {options.ConnectionString}");
                DingDingSyncDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
            });

            services.Configure<DingDingConfigOptions>(Configuration.GetSection(DingDingConfigOptions.DingDing));
            services.Configure<IKuaiConfigOptions>(Configuration.GetSection(IKuaiConfigOptions.IKuai));

            services.AddScoped<IDingdingAppService, DingDingAppService>();
            services.AddControllersWithViews(); //.AddRazorRuntimeCompilation();
            return services.AddAbp<WebModule>(options =>
            {
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(facility =>
                    facility.UseAbpLog4Net().WithConfig("log4net.config"));
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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