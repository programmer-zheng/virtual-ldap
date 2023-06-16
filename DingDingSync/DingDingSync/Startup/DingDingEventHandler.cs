using System;
using DingDingSync.Application.Jobs;
using DingDingSync.Application.Jobs.EventHandler.DingDing;
using Microsoft.Extensions.DependencyInjection;

namespace DingDingSync.Web.Startup;

public static class DingDingEventHandler
{
    /// <summary>
    /// 注册钉钉事件处理程序
    /// </summary>
    /// <param name="services"></param>
    public static void RegisterDingDingEventHandler(this IServiceCollection services)
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
}