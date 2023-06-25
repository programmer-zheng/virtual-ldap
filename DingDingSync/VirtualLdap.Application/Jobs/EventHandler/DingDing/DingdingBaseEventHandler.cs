using Castle.Core.Logging;
using DingTalk.Api.Response;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    public abstract class DingdingBaseEventHandler
    {
        protected readonly ILogger Logger;

        public DingdingBaseEventHandler(ILogger logger)
        {
            Logger = logger;
        }

        public abstract void Do(string msg);

        /// <summary>
        /// 判断是否为管理人员
        /// </summary>
        /// <param name="dingdingUser"></param>
        /// <returns></returns>
        public virtual bool IsAdmin(OapiV2UserGetResponse.UserGetResponseDomain dingdingUser)
        {
            return dingdingUser.Admin ||
                   (dingdingUser.LeaderInDept != null && dingdingUser.LeaderInDept.Count(t => t.Leader) > 0);
        }
    }
}