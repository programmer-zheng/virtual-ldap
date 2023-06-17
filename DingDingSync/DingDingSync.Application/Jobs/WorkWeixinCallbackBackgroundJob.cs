using System.Xml.Linq;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Castle.Core.Logging;

namespace DingDingSync.Application.Jobs;

public class WorkWeixinCallbackBackgroundJob : BackgroundJob<string>, ITransientDependency
{
    private readonly ILogger _logger;

    public WorkWeixinCallbackBackgroundJob(ILogger logger)
    {
        _logger = logger;
    }

    [UnitOfWork]
    public override void Execute(string args)
    {
        _logger.Info($"企业微信消息内容：{args}");

        XElement root = XElement.Parse(args);
        var changeType = root.Element("ChangeType")?.Value;
        var id = root.Element("Id")?.Value;
        // 根据ID获取详情
        throw new NotImplementedException();
    }
}