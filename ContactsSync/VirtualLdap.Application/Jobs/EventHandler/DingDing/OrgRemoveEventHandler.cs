namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 企业被解散
    /// </summary>
    public class OrgRemoveEventHandler : DingdingBaseEventHandler
    {
        public override async Task Do(string msg)
        {
            Logger.Info("企业解散...");
        }
    }
}