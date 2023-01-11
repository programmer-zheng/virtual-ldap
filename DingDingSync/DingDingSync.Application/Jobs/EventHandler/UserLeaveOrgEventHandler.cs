using Abp.Domain.Repositories;
using Castle.Core.Logging;
using DingDingSync.Application.IKuai;
using DingDingSync.Application.Jobs.EventInfo;
using DingDingSync.Core.Entities;
using Newtonsoft.Json;

namespace DingDingSync.Application.Jobs.EventHandler
{
    /// <summary>
    /// 通讯录用户离职
    /// </summary>
    public class UserLeaveOrgEventHandler : DingdingBaseEventHandler
    {
        private readonly IRepository<UserEntity, string> _userRepository;
        private readonly IRepository<UserDepartmentsRelationEntity, string> _deptUserRelaRepository;
        private readonly IIkuaiAppService _iKuaiAppService;

        public UserLeaveOrgEventHandler(IRepository<UserEntity, string> userRepository,
            IRepository<UserDepartmentsRelationEntity, string> deptUserRelaRepository,
            IIkuaiAppService iKuaiAppService, ILogger logger) : base(logger)
        {
            _userRepository = userRepository;
            _deptUserRelaRepository = deptUserRelaRepository;
            _iKuaiAppService = iKuaiAppService;
        }

        public override void Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<UserLeaveOrgEvent>(msg);
            if (eventInfo != null)
            {
                try
                {
                    //先查询启用vpn账号的
                    var users = _userRepository.GetAll().Where(t => eventInfo.ID.Contains(t.Id) && t.VpnAccountEnabled)
                        .Select(t => t.UserName).ToList();

                    //批量删除用户
                    _userRepository.Delete(t => eventInfo.ID.Contains(t.Id));

                    //批量删除部门关系
                    _deptUserRelaRepository.Delete(t => eventInfo.ID.Contains(t.UserId));

                    foreach (var username in users)
                    {
                        //移除爱快vpn账号
                        var ikuaiAccount = _iKuaiAppService.GetAccountIdByUsername(username);
                        _iKuaiAppService.RemoveAccount(ikuaiAccount.id);
                    }
                }
                catch (Exception e)
                {
                    Logger.Error("处理用户离职时发生异常", e);
                }
            }
        }
    }
}