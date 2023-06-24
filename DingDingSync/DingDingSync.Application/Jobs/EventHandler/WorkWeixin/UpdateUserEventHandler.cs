using System.Threading.Tasks;

namespace DingDingSync.Application.Jobs.EventHandler.WorkWeixin;

public class UpdateUserEventHandler : WorkWeixinBaseEventHandler, IWorkWeixinEventHandler
{
    public string EventType { get; set; } = "update_user";

    public Task Handle(string msgContent)
    {
        return Task.CompletedTask;
    }

    public UpdateUserEventHandler(string msgContent) : base(msgContent)
    {
    }
}