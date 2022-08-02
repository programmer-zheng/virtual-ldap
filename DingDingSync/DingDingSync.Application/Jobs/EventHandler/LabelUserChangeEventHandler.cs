using System;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Castle.Core.Logging;
using DingDingSync.Application.AppService;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Application.Jobs.EventInfo;
using DingDingSync.Core.Entities;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace DingDingSync.Application.Jobs.EventHandler
{
    /// <summary>
    /// 员工角色信息发生变更
    /// </summary>
    public class LabelUserChangeEventHandler : DingdingBaseEventHandler
    {
        private readonly IRepository<UserEntity, string> _userRepository;
        private readonly IDingdingAppService _dingdingAppService;


        public LabelUserChangeEventHandler(IRepository<UserEntity, string> userRepository,
            IDingdingAppService dingdingAppService)
        {
            _userRepository = userRepository;
            _dingdingAppService = dingdingAppService;
        }

        public override void Do(string msg)
        {
            var eventinfo = JsonConvert.DeserializeObject<LabelUserChangeEvent>(msg);
            if (eventinfo != null && eventinfo.ID != null)
            {
                foreach (var userid in eventinfo.ID)
                {
                    var dingdingUser = _dingdingAppService.GetUserDetail(userid);
                    Console.WriteLine($"用户信息变更：{eventinfo.Action} ");
                    Console.WriteLine(JsonConvert.SerializeObject(dingdingUser));

                    var dbUser = _userRepository.FirstOrDefault(userid);
                    if (dbUser != null)
                    {
                        dbUser.IsAdmin = IsAdmin(dingdingUser);
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