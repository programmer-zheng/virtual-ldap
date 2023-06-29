using Abp.Domain.Repositories;
using Castle.Core.Logging;
using VirtualLdap.Application.DingDingUtils;
using VirtualLdap.Application.Jobs.EventInfo;
using VirtualLdap.Core.Entities;
using Newtonsoft.Json;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录用户被取消设置管理员
    /// </summary>
    public class OrgAdminRemoveEventHandler : DingdingBaseEventHandler
    {
        private readonly IDingTalkAppService _dingDingAppService;
        private readonly IRepository<UserEntity, string> _userRepository;

        public OrgAdminRemoveEventHandler(IDingTalkAppService dingDingAppService,
            IRepository<UserEntity, string> userRepository, ILogger logger) : base(logger)
        {
            _dingDingAppService = dingDingAppService;
            _userRepository = userRepository;
        }

        public override void Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<OrgAdminRemoveEvent>(msg);
            if (eventInfo != null)
            {
                foreach (var userid in eventInfo.ID)
                {
                    try
                    {
                        var dingDingUser = _dingDingAppService.GetUserDetail(userid);

                        //被取消设置为管理员，如果是部门领导，仍是管理员身份，可查询下属人员
                        var isAdmin = (dingDingUser.LeaderInDept != null &&
                                       dingDingUser.LeaderInDept.Count(t => t.Leader) > 0);

                        var dbUser = _userRepository.FirstOrDefault(userid);
                        if (dbUser != null)
                        {
                            dbUser.IsAdmin = isAdmin;
                            _userRepository.Update(dbUser);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("取消设置用户管理员时发生异常", e);
                    }
                }
            }
        }
    }
}