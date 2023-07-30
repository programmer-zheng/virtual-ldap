using Abp.ObjectMapping;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.DingDingUtils;
using VirtualLdap.Application.Jobs.EventInfo;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录用户增加
    /// </summary>
    public class UserAddOrgEventHandler : DingdingBaseEventHandler
    {
        private readonly IDingTalkAppService _dingdingAppService;
        private readonly IObjectMapper _objectMapper;
        private readonly IConfiguration _configuration;
        private readonly IUserAppService _userAppService;

        public UserAddOrgEventHandler(IDingTalkAppService dingdingAppService,
            IObjectMapper objectMapper,
            IConfiguration configuration,
            IUserAppService userAppService)
        {
            _dingdingAppService = dingdingAppService;
            _objectMapper = objectMapper;
            _configuration = configuration;
            _userAppService = userAppService;
        }

        public override async Task Do(string msg)
        {
            var defaultPassword = _configuration.GetValue<string>("DefaultPassword");
            defaultPassword = string.IsNullOrWhiteSpace(defaultPassword) ? "123456" : defaultPassword;
            var eventInfo = JsonConvert.DeserializeObject<UserAddOrgEvent>(msg);
            if (eventInfo != null && eventInfo.ID.Count > 0)
            {
                foreach (var userid in eventInfo.ID)
                {
                    try
                    {
                        //调用钉钉API获取人员详情
                        var dingdingUser = _dingdingAppService.GetUserDetail(userid);

                        //判断是否管理人员
                        var isAdmin = IsAdmin(dingdingUser);

                        // 映射至数据实体
                        var userEntity = _objectMapper.Map<UserEntity>(dingdingUser);
                        userEntity.IsAdmin = isAdmin;
                        userEntity.AccountEnabled = isAdmin;
                        userEntity.Password = defaultPassword.DesEncrypt();
                        var username = await _userAppService.GetUserNameAsync(userEntity.Name);

                        userEntity.UserName = username;
                        if (!userEntity.HiredDate.HasValue)
                        {
                            userEntity.HiredDate = DateTime.Today;
                        }

                        // 插入人员数据
                        await _userAppService.AddUserAsync(userEntity);
                        // 插入部门关联数据
                        await _userAppService.UpdateUserDepartmentRelationsAsync(dingdingUser.Userid, dingdingUser.DeptIdList);
                    }
                    catch (Exception e)
                    {
                        Logger.Error("处理员工入职发生异常", e);
                    }
                }
            }
        }
    }
}