using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.ObjectMapping;
using Castle.Core.Logging;
using DingDingSync.Application.AppService;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Core.Entities;
using Microsoft.Extensions.Configuration;


namespace DingDingSync.Application.Jobs
{
    public class DingDingCallbackBackgroundJob : BackgroundJob<DingDingCallbackBackgroundJobArgs>, ITransientDependency
    {
        private readonly List<string> _listenEvent;

        public ILogger _logger { get; set; }
        public IIocManager _iocManager { get; set; }

        public DingDingCallbackBackgroundJob()
        {
            _listenEvent = new List<string>();
            _listenEvent.Add("user_add_org"); //通讯录用户增加
            _listenEvent.Add("user_modify_org"); //通讯录用户更改
            _listenEvent.Add("user_leave_org"); //通讯录用户离职
            _listenEvent.Add("user_active_org"); //加入企业后用户激活
            _listenEvent.Add("org_admin_add"); //通讯录用户被设为管理员
            _listenEvent.Add("org_admin_remove"); //通讯录用户被取消设置管理员
            _listenEvent.Add("org_dept_create"); //通讯录企业部门创建
            _listenEvent.Add("org_dept_modify"); //通讯录企业部门修改
            _listenEvent.Add("org_dept_remove"); //通讯录企业部门删除
            _listenEvent.Add("org_remove"); //企业被解散
            _listenEvent.Add("label_user_change"); //员工角色信息发生变更
        }

        // [UnitOfWork]
        // public override void Execute(DingDingCallbackBackgroundJobArgs args)
        // {
        //
        //
        // }

        [UnitOfWork]
        public override void Execute(DingDingCallbackBackgroundJobArgs args)
        {
            var className = $"DingDingSync.Application.Jobs.EventHandler.{args.EventType}_event_handler";
            Type classType = Type.GetType(className);
            if (classType != null)
            {
                //从容器中获取依赖项
                var x1 = _iocManager.Resolve<IRepository<DepartmentEntity, long>>();
                var x2 = _iocManager.Resolve<IRepository<UserEntity, string>>();
                var x3 = _iocManager.Resolve<IRepository<UserDepartmentsRelationEntity, string>>();
                var x4 = _iocManager.Resolve<IUserAppService>();
                var x5 = _iocManager.Resolve<IDepartmentAppService>();
                var x6 = _iocManager.Resolve<IDingdingAppService>();
                var x7 = _iocManager.Resolve<IObjectMapper>();
                var x8 = _iocManager.Resolve<IConfiguration>();
                var x9 = _iocManager.Resolve<IIkuaiAppService>();
                var x10 = _iocManager.Resolve<ILogger>();

                var handler = Activator.CreateInstance(classType, x1, x2, x3, x4, x5, x6, x7,x8,x9,x10);
                var method = classType.GetMethod("Do", new Type[] {typeof(string)});
                method.Invoke(handler, new object[] {args.Msg});
            }
            else
            {
                _logger.Error($"未能找到钉钉回调事件：{args.EventType} 的处理类，无法处理相关回调...");
            }
        }
    }
}