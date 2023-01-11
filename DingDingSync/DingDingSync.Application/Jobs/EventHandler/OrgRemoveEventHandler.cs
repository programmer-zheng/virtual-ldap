using Castle.Core.Logging;

namespace DingDingSync.Application.Jobs.EventHandler
{
    /// <summary>
    /// 企业被解散
    /// </summary>
    public class OrgRemoveEventHandler : DingdingBaseEventHandler
    {
        public OrgRemoveEventHandler(ILogger logger) : base(logger)
        {
        }

        public override void Do(string msg)
        {
            Logger.Info("企业解散...");
        }
    }
}