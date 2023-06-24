using System;
using System.Threading.Tasks;

namespace DingDingSync.Application.Jobs.EventHandler.WorkWeixin;

public class CreateUserEventHandler : WorkWeixinBaseEventHandler, IWorkWeixinEventHandler
{
    public string EventType { get; set; } = "create_user";

    public Task Handle(string msgContent)
    {
        Console.WriteLine("CreateUserEventHandler");
        return Task.CompletedTask;;
    }

    public CreateUserEventHandler(string msgContent) : base(msgContent)
    {
    }
}