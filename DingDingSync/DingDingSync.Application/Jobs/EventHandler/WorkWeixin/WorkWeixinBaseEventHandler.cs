using System.Xml.Linq;

namespace DingDingSync.Application.Jobs.EventHandler.WorkWeixin;

public abstract class WorkWeixinBaseEventHandler
{
    private string _msgContent;

    public WorkWeixinBaseEventHandler(string msgContent)
    {
        _msgContent = msgContent;
        XElement root = XElement.Parse(_msgContent);
        ChangeType = root.Element("ChangeType")?.Value;
        Id = root.Element("Id")?.Value;
    }

    public string ChangeType { get; private set; }
    
    public string Id { get; private set; }
}