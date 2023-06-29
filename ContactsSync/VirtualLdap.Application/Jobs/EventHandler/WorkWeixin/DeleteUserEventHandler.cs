namespace VirtualLdap.Application.Jobs.EventHandler.WorkWeixin;

public class DeleteUserEventHandler : WorkWeixinBaseEventHandler, IWorkWeixinEventHandler
{
    public string EventType { get; set; } = "delete_user";

    public Task Handle(string msgContent)
    {
        return Task.CompletedTask;
    }

    public DeleteUserEventHandler(string msgContent) : base(msgContent)
    {
    }
}