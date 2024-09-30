using Abp.ObjectMapping;
using VirtualLdap.Application.WorkWeixinUtils;

namespace VirtualLdap.Application.Jobs.EventHandler.WorkWeixin;

public abstract class WorkWeixinBaseEventHandler
{
    public IWorkWeixinAppService WorkWeixinAppService { get; set; }
    
    public IObjectMapper ObjectMapper { get; set; }
    
    public abstract Task Do(string msg);

}