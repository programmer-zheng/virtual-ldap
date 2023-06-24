using System.Threading.Tasks;

namespace DingDingSync.Application;

public interface ICommonProvider
{
    /// <summary>
    /// 发送文本消息
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="msgContent"></param>
    /// <returns></returns>
    Task SendTextMessage(string userid, string msgContent);
    
    
}