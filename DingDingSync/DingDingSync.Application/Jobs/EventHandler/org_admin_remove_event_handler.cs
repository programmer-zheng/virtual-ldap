using System;
using System.Linq;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Application.Jobs;
using DingDingSync.Application.Jobs.EventInfo;
using DingDingSync.Core.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Castle.Core.Logging;
using DingDingSync.Application.AppService;

namespace DingDingSync.Application.Jobs.EventHandler
{
    /// <summary>
    /// 通讯录用户被取消设置管理员
    /// </summary>
    public class org_admin_remove_event_handler : DingdingBaseEventHandler
    {
        protected readonly IDingdingAppService _dingdingAppService;
        protected readonly IRepository<UserEntity, string> _userRepository;
        public org_admin_remove_event_handler(IDingdingAppService dingdingAppService, IRepository<UserEntity, string> userRepository)
        {
            _dingdingAppService = dingdingAppService;
            _userRepository = userRepository;
        }

        public override void Do(string msg)
        {
            var eventinfo = JsonConvert.DeserializeObject<org_admin_remove_event>(msg);
            if (eventinfo != null && eventinfo.ID != null)
            {
                foreach (var userid in eventinfo.ID)
                {
                    var dingdingUser = _dingdingAppService.GetUserDetail(userid);

                    //被取消设置为管理员，如果是部门领导，仍是管理员身份，可查询下属人员
                    var isadmin = (dingdingUser.LeaderInDept != null &&
                                   dingdingUser.LeaderInDept.Count(t => t.Leader) > 0);

                    var dbUser = _userRepository.FirstOrDefault(userid);
                    if (dbUser != null)
                    {
                        dbUser.IsAdmin = isadmin;
                        _userRepository.Update(dbUser);
                    }
                }
            }
        }
    }
}