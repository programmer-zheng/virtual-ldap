using Abp.ObjectMapping;
using Castle.Core.Logging;
using DingTalk.Api.Response;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    public abstract class DingdingBaseEventHandler
    {
        public ILogger Logger { get; set; }

        public IObjectMapper ObjectMapper { get; set; }

        public abstract Task Do(string msg);

        /// <summary>
        /// 判断是否为管理人员
        /// </summary>
        /// <param name="dingdingUser"></param>
        /// <returns></returns>
        protected bool IsAdmin(OapiV2UserGetResponse.UserGetResponseDomain dingdingUser)
        {
            return dingdingUser.Admin ||
                   (dingdingUser.LeaderInDept != null && dingdingUser.LeaderInDept.Count(t => t.Leader) > 0);
        }
    }
}