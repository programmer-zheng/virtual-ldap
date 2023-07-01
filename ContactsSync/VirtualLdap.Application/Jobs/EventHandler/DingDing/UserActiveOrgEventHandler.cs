namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 加入企业后用户激活
    /// </summary>
    public class UserActiveOrgEventHandler : DingdingBaseEventHandler
    {
        public override async Task Do(string msg)
        {
            Logger.Info($"用户激活");
        }
    }
}