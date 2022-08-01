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
using DingDingSync.Application.Jobs;
using DingDingSync.Application.Jobs.EventHandler;

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
            
            services.AddTransient<user_add_org_event_handler>(); //通讯录用户增加
            services.AddTransient<user_modify_org_event_handler>(); //通讯录用户更改
            services.AddTransient<user_leave_org_event_handler>(); //通讯录用户离职
            services.AddTransient<user_active_org_event_handler>(); //加入企业后用户激活
            services.AddTransient<org_admin_add_event_handler>(); //通讯录用户被设为管理员
            services.AddTransient<org_admin_remove_event_handler>(); //通讯录用户被取消设置管理员
            services.AddTransient<org_dept_create_event_handler>(); //通讯录企业部门创建
            services.AddTransient<org_dept_modify_event_handler>(); //通讯录企业部门修改
            services.AddTransient<org_dept_remove_event_handler>(); //通讯录企业部门删除
            services.AddTransient<org_remove_event_handler>(); //企业被解散
            services.AddTransient<label_user_change_event_handler>(); //员工角色信息发生变更
            services.AddTransient(serviceProvider =>
            {
                Func<string, DingdingBaseEventHandler> func = (eventType) =>
                {
                    switch (eventType)
                    {
                        case "user_add_org":  //通讯录用户增加
                            return serviceProvider.GetService<user_add_org_event_handler>();
                        case "user_modify_org":  //通讯录用户更改
                            return serviceProvider.GetService<user_modify_org_event_handler>();
                        case "user_leave_org":  //通讯录用户离职
                            return serviceProvider.GetService<user_leave_org_event_handler>();
                        case "user_active_org":  //加入企业后用户激活
                            return serviceProvider.GetService<user_active_org_event_handler>();
                        case "org_admin_add":  //通讯录用户被设为管理员
                            return serviceProvider.GetService<org_admin_add_event_handler>();
                        case "org_admin_remove":  //通讯录用户被取消设置管理员
                            return serviceProvider.GetService<org_admin_remove_event_handler>();
                        case "org_dept_create":  //通讯录企业部门创建
                            return serviceProvider.GetService<org_dept_create_event_handler>();
                        case "org_dept_modify":  //通讯录企业部门修改
                            return serviceProvider.GetService<org_dept_modify_event_handler>();
                        case "org_dept_remove":  //通讯录企业部门删除
                            return serviceProvider.GetService<org_dept_remove_event_handler>();
                        case "org_remove":  //企业被解散
                            return serviceProvider.GetService<org_remove_event_handler>();
                        case "label_user_change":  //员工角色信息发生变更
                            return serviceProvider.GetService<label_user_change_event_handler>();
                        default:
                            return null;
                    }
                };
                return func;
            });
            
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