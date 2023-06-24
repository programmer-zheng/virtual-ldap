using System.Threading.Tasks;

namespace DingDingSync.Application.Jobs.EventHandler.WorkWeixin;

public class DeletePartyEventHandler : WorkWeixinBaseEventHandler, IWorkWeixinEventHandler
{
    public string EventType { get; set; } = "delete_party";

    public Task Handle(string msgContent)
    {
        return Task.CompletedTask;
    }

    public DeletePartyEventHandler(string msgContent) : base(msgContent)
    {
    }
}