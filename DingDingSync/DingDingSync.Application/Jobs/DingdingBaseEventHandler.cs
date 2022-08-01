using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Core.Entities;
using DingTalk.Api.Response;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Castle.Core.Logging;
using DingDingSync.Application.AppService;
using TinyPinyin;

namespace DingDingSync.Application.Jobs
{
    public abstract class DingdingBaseEventHandler
    {
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