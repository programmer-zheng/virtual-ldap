using System.Reflection;
using System.Xml.Linq;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Castle.Core.Logging;
using VirtualLdap.Application.Jobs.EventHandler.WorkWeixin;

namespace VirtualLdap.Application.Jobs;

public class WorkWeixinCallbackBackgroundJob : AsyncBackgroundJob<string>, ITransientDependency
{
    private readonly ILogger _logger;
    private readonly Func<string, WorkWeixinBaseEventHandler> _eventHandlerFactory;

    public WorkWeixinCallbackBackgroundJob(ILogger logger,
        Func<string, WorkWeixinBaseEventHandler> eventHandlerFactory)
    {
        _logger = logger;
        _eventHandlerFactory = eventHandlerFactory;
    }

    [UnitOfWork]
    public override async Task ExecuteAsync(string args)
    {
        _logger.Debug($"企业微信消息内容：{args}");

        // 企业微信回调数据，具体可参考
        // https://developer.work.weixin.qq.com/document/path/90967
        var xml = XElement.Parse(args);
        var changeType = xml.Element("ChangeType")?.Value;
        var handler = _eventHandlerFactory(changeType);
        if (handler != null)
        {
            try
            {
                await handler.Do(args);
            }
            catch (Exception e)
            {
                _logger.Error("处理企业微信回调发生异常", e);
                throw;
            }
        }
        else
        {
            _logger.Error($"未能找到企业微信回调事件：{changeType} 的处理类，无法处理相关回调...");
        }
    }
}