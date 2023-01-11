using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Castle.Core.Logging;

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