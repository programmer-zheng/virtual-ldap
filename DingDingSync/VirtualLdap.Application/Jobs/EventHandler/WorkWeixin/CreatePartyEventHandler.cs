namespace VirtualLdap.Application.Jobs.EventHandler.WorkWeixin;

public class CreatePartyEventHandler : WorkWeixinBaseEventHandler, IWorkWeixinEventHandler
{
    public string EventType { get; set; } = "create_party";

    public Task Handle(string msgContent)
    {
        if (EventType == ChangeType)
        {
            Console.WriteLine("CreatePartyEventHandler");
            
        }
        return Task.CompletedTask;
    }

    public CreatePartyEventHandler(string msgContent) : base(msgContent)
    {
    }
}