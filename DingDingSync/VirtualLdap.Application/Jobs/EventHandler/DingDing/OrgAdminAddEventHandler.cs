using Abp.Domain.Repositories;
using Castle.Core.Logging;
using VirtualLdap.Application.Jobs.EventInfo;
using VirtualLdap.Core.Entities;
using Newtonsoft.Json;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录用户被设为管理员
    /// </summary>
    public class OrgAdminAddEventHandler : DingdingBaseEventHandler
    {
        private readonly IRepository<UserEntity, string> _userRepository;

        public OrgAdminAddEventHandler(IRepository<UserEntity, string> userRepository, ILogger logger) : base(logger)
        {
            _userRepository = userRepository;
        }

        public override void Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<OrgAdminAddEvent>(msg);
            if (eventInfo != null)
            {
                foreach (var userid in eventInfo.ID)
                {
                    var dbUser = _userRepository.FirstOrDefault(userid);
                    if (dbUser != null)
                    {
                        dbUser.IsAdmin = true;
                        _userRepository.Update(dbUser);
                    }
                }
            }
        }
    }
}