using System.Threading.Tasks;

namespace DingDingSync.Application.Jobs.EventHandler.WorkWeixin;

public class UpdatePartyEventHandler : WorkWeixinBaseEventHandler, IWorkWeixinEventHandler
{
    public string EventType { get; set; } = "update_party";

    public Task Handle(string msgContent)
    {
        return Task.CompletedTask;
    }

    public UpdatePartyEventHandler(string msgContent) : base(msgContent)
    {
    }
}