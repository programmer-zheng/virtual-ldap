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
        private readonly ILogger _logger;

        private readonly Func<string, DingdingBaseEventHandler> _eventHandlerFactory;

        public DingDingCallbackBackgroundJob(Func<string, DingdingBaseEventHandler> eventHandlerFactory, ILogger logger)
        {
            _eventHandlerFactory = eventHandlerFactory;
            _logger = logger;
        }

        [UnitOfWork]
        public override void Execute(DingDingCallbackBackgroundJobArgs args)
        {
            var handler = _eventHandlerFactory(args.EventType);
            if (handler != null)
            {
                handler.Do(args.Msg);
            }
            else
            {
                _logger.Error($"未能找到钉钉回调事件：{args.EventType} 的处理类，无法处理相关回调...");
            }
        }
    }
}