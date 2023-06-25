using System.Reflection;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Castle.Core.Logging;
using DingDingSync.Application.Jobs.EventHandler.WorkWeixin;

namespace DingDingSync.Application.Jobs;

public class WorkWeixinCallbackBackgroundJob : AsyncBackgroundJob<string>, ITransientDependency
{
    private readonly ILogger _logger;

    public WorkWeixinCallbackBackgroundJob(ILogger logger)
    {
        _logger = logger;
    }

    [UnitOfWork]
    public override async Task ExecuteAsync(string args)
    {
        _logger.Info($"企业微信消息内容：{args}");

        var types = Assembly.GetExecutingAssembly().GetTypes();
        var handlers = types.Where(t => t.IsClass && t.IsAssignableTo(typeof(IWorkWeixinEventHandler)));
        WorkWeixinCallbackHandler callbackHandler = new WorkWeixinCallbackHandler();

        foreach (var handler in handlers)
        {
            var handlerInstance = (IWorkWeixinEventHandler)Activator.CreateInstance(handler, args);
            callbackHandler.RegisterHandler(handlerInstance);
        }

        await callbackHandler.Notify(args);
    }
}