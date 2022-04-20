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
    public class label_user_change_event_handler : DingdingBaseEventHandler
    {
        public label_user_change_event_handler(IRepository<DepartmentEntity, long> departmentRepository,
            IRepository<UserEntity, string> userRepository,
            IRepository<UserDepartmentsRelationEntity, string> deptUserRelaRepository,
            IUserAppService userAppService,
            IDepartmentAppService departmentAppService,
            IDingdingAppService dingdingAppService,
            IObjectMapper objectMapper,
            IConfiguration configuration,
            IIkuaiAppService iKuaiAppService,
            ILogger logger) : base(departmentRepository, userRepository,
            deptUserRelaRepository, userAppService, departmentAppService, dingdingAppService, objectMapper,
            configuration, iKuaiAppService, logger)
        {
        }

        public override void Do(string msg)
        {
            var eventinfo = JsonConvert.DeserializeObject<label_user_change_event>(msg);
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