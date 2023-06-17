namespace DingDingSync.Application.Jobs.EventHandler.WorkWeixin;

public interface IWorkWeixinEventHandler
{
    Task Handle(string msgContent);
}