namespace VirtualLdap.Application;

public interface IMessageProvider
{
    /// <summary>
    /// 发送文本消息
    /// </summary>
    /// <param name="userid"></param>
    /// <param name="msgContent"></param>
    /// <returns></returns>
    Task SendTextMessageAsync(string userid, string msgContent);
    
    
}