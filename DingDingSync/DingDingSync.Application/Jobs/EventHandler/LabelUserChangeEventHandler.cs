using Abp.Domain.Repositories;
using Castle.Core.Logging;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.Jobs.EventInfo;
using DingDingSync.Core.Entities;
using Newtonsoft.Json;

namespace DingDingSync.Application.Jobs.EventHandler
{
    /// <summary>
    /// 员工角色信息发生变更
    /// </summary>
    public class LabelUserChangeEventHandler : DingdingBaseEventHandler
    {
        private readonly IRepository<UserEntity, string> _userRepository;
        private readonly IDingdingAppService _dingDingAppService;


        public LabelUserChangeEventHandler(IRepository<UserEntity, string> userRepository,
            IDingdingAppService dingDingAppService, ILogger logger) : base(logger)
        {
            _userRepository = userRepository;
            _dingDingAppService = dingDingAppService;
        }

        public override void Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<LabelUserChangeEvent>(msg);
            if (eventInfo != null)
            {
                foreach (var userid in eventInfo.ID)
                {
                    var dingDingUser = _dingDingAppService.GetUserDetail(userid);
                    Console.WriteLine($"用户信息变更：{eventInfo.Action} ");
                    Console.WriteLine(JsonConvert.SerializeObject(dingDingUser));

                    var dbUser = _userRepository.FirstOrDefault(userid);
                    if (dbUser != null)
                    {
                        dbUser.IsAdmin = IsAdmin(dingDingUser);
                        if (!dbUser.AccountEnabled && dbUser.IsAdmin)
                        {
                            dbUser.AccountEnabled = true;
                        }

                        _userRepository.Update(dbUser);
                    }
                }
            }
        }
    }
}