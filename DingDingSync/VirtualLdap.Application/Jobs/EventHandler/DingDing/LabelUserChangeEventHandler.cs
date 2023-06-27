﻿using Abp.Domain.Repositories;
using Castle.Core.Logging;
using VirtualLdap.Application.DingDingUtils;
using VirtualLdap.Application.Jobs.EventInfo;
using VirtualLdap.Core.Entities;
using Newtonsoft.Json;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 员工角色信息发生变更
    /// </summary>
    public class LabelUserChangeEventHandler : DingdingBaseEventHandler
    {
        private readonly IRepository<UserEntity, string> _userRepository;
        private readonly IDingTalkAppService _dingDingAppService;


        public LabelUserChangeEventHandler(IRepository<UserEntity, string> userRepository,
            IDingTalkAppService dingDingAppService, ILogger logger) : base(logger)
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