using System;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.EntityFrameworkCore;
using Castle.Facilities.Logging;
using DingDingSync.Application;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Application.Jobs;
using DingDingSync.Application.Jobs.EventHandler;
using DingDingSync.Application.WorkWeixinUtils;
using DingDingSync.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DingDingSync.Web.Startup
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
            services.AddAbpDbContext<DingDingSyncDbContext>(options =>
            {
                DingDingSyncDbContextConfigurer.Configure(options.DbContextOptions, options.ConnectionString);
            });

            services.Configure<DingDingConfigOptions>(Configuration.GetSection(DingDingConfigOptions.DingDing));
            services.Configure<WorkWeixinConfigOptions>(Configuration.GetSection(WorkWeixinConfigOptions.WorkWeixin));

            services.Configure<IKuaiConfigOptions>(Configuration.GetSection(IKuaiConfigOptions.IKuai));

            // todo 注释掉，观察是否不需要手工注入
            // services.AddScoped<IDingdingAppService, DingDingAppService>();
            // services.AddScoped<IWorkWeixinAppService, WorkWeixinAppService>();

            RegisterDingDingEventHandler(services);

            var workEnv = Configuration["WorkEnv"];
            if (workEnv.Equals("DingDing", StringComparison.OrdinalIgnoreCase))
            {
                services.AddScoped<ICommonProvider, DingDingAppService>();
            }
            else if (workEnv.Equals("WorkWeixin", StringComparison.OrdinalIgnoreCase))
            {
                services.AddScoped<ICommonProvider, WorkWeixinAppService>();
            }
            else
            {
                throw new Exception("目前只支持钉钉(DingDing)、企业微信(WorkWeixin)，请正确填写 WorkEnv");
            }

            services.AddControllersWithViews(); //.AddRazorRuntimeCompilation();
            return services.AddAbp<WebModule>(options =>
            {
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(facility =>
                    facility.UseAbpLog4Net()
                        .WithConfig(Environment.IsDevelopment() ? "log4net.Development.config" : "log4net.config"));
            });
        }

        /// <summary>
        /// 注册钉钉事件处理程序
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterDingDingEventHandler(IServiceCollection services)
        {
            services.AddTransient<UserAddOrgEventHandler>(); //通讯录用户增加
            services.AddTransient<UserModifyOrgEventHandler>(); //通讯录用户更改
            services.AddTransient<UserLeaveOrgEventHandler>(); //通讯录用户离职
            services.AddTransient<UserActiveOrgEventHandler>(); //加入企业后用户激活
            services.AddTransient<OrgAdminAddEventHandler>(); //通讯录用户被设为管理员
            services.AddTransient<OrgAdminRemoveEventHandler>(); //通讯录用户被取消设置管理员
            services.AddTransient<OrgDeptCreateEventHandler>(); //通讯录企业部门创建
            services.AddTransient<OrgDeptModifyEventHandler>(); //通讯录企业部门修改
            services.AddTransient<OrgDeptRemoveEventHandler>(); //通讯录企业部门删除
            services.AddTransient<OrgRemoveEventHandler>(); //企业被解散
            services.AddTransient<LabelUserChangeEventHandler>(); //员工角色信息发生变更
            services.AddTransient(serviceProvider =>
            {
                Func<string, DingdingBaseEventHandler> func = (eventType) =>
                {
                    switch (eventType)
                    {
                        case "user_add_org": //通讯录用户增加
                            return serviceProvider.GetService<UserAddOrgEventHandler>();
                        case "user_modify_org": //通讯录用户更改
                            return serviceProvider.GetService<UserModifyOrgEventHandler>();
                        case "user_leave_org": //通讯录用户离职
                            return serviceProvider.GetService<UserLeaveOrgEventHandler>();
                        case "user_active_org": //加入企业后用户激活
                            return serviceProvider.GetService<UserActiveOrgEventHandler>();
                        case "org_admin_add": //通讯录用户被设为管理员
                            return serviceProvider.GetService<OrgAdminAddEventHandler>();
                        case "org_admin_remove": //通讯录用户被取消设置管理员
                            return serviceProvider.GetService<OrgAdminRemoveEventHandler>();
                        case "org_dept_create": //通讯录企业部门创建
                            return serviceProvider.GetService<OrgDeptCreateEventHandler>();
                        case "org_dept_modify": //通讯录企业部门修改
                            return serviceProvider.GetService<OrgDeptModifyEventHandler>();
                        case "org_dept_remove": //通讯录企业部门删除
                            return serviceProvider.GetService<OrgDeptRemoveEventHandler>();
                        case "org_remove": //企业被解散
                            return serviceProvider.GetService<OrgRemoveEventHandler>();
                        case "label_user_change": //员工角色信息发生变更
                            return serviceProvider.GetService<LabelUserChangeEventHandler>();
                        default:
                            return null;
                    }
                };
                return func;
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