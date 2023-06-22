using DingDingSync.Application.Jobs.EventHandler.WorkWeixin;

namespace DingDingSync.Application.Jobs;

public class WorkWeixinCallbackHandler
{
    private List<IWorkWeixinEventHandler> _handlers = new List<IWorkWeixinEventHandler>();

    public void RegisterHandler(IWorkWeixinEventHandler handler)
    {
        _handlers.Add(handler);
    }

    public void RemoveHandler(IWorkWeixinEventHandler handler)
    {
        _handlers.Remove(handler);
    }

    public async Task Notify(string msgContent)
    {
        foreach (var handler in _handlers)
        {
            await handler.Handle(msgContent);
        }
    }
}