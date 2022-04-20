using System;
using System.Text;
using System.Text.RegularExpressions;
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
using TinyPinyin;

namespace DingDingSync.Application.Jobs.EventHandler
{
    /// <summary>
    /// 通讯录用户增加
    /// </summary>
    public class user_add_org_event_handler : DingdingBaseEventHandler
    {
        public user_add_org_event_handler(IRepository<DepartmentEntity, long> departmentRepository,
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
            var defaultPassword = _configuration.GetValue<string>("DefaultPassword");
            defaultPassword = string.IsNullOrWhiteSpace(defaultPassword) ? "123456" : defaultPassword;
            var eventinfo = JsonConvert.DeserializeObject<user_add_org_event>(msg);
            if (eventinfo != null && eventinfo.ID != null && eventinfo.ID.Count > 0)
            {
                foreach (var userid in eventinfo.ID)
                {
                    try
                    {
                        //调用钉钉API获取人员详情
                        var dingdingUser = _dingdingAppService.GetUserDetail(userid);

                        //判断是否管理人员
                        var isAdmin = IsAdmin(dingdingUser);

                        //映射至数据实体
                        var userEntity = _objectMapper.Map<UserEntity>(dingdingUser);
                        userEntity.IsAdmin = isAdmin;
                        userEntity.AccountEnabled = isAdmin;
                        userEntity.Password = defaultPassword.DesEncrypt();
                        var username = _userAppService.GetUserName(userEntity.Name).Result;

                        userEntity.UserName = username;

                        //插入人员数据
                        _userRepository.Insert(userEntity);
                        //循环插入部门关联数据
                        foreach (var deptid in dingdingUser.DeptIdList)
                        {
                            var rela = new UserDepartmentsRelationEntity
                            {
                                Id = Guid.NewGuid().ToString(),
                                DeptId = deptid,
                                UserId = dingdingUser.Userid
                            };
                            _deptUserRelaRepository.Insert(rela);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Error("处理员工入职发生异常", e);
                    }
                }
            }
        }
    }
}