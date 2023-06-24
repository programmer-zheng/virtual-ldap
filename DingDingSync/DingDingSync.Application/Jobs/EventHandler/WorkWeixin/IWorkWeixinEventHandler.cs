using System.Threading.Tasks;

namespace DingDingSync.Application.Jobs.EventHandler.WorkWeixin;

public interface IWorkWeixinEventHandler
{
    public string EventType { get; set; }
    
    Task Handle(string msgContent);
}