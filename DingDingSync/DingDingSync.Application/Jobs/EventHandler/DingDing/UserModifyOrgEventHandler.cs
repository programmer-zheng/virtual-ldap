using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Castle.Core.Logging;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.Jobs.EventInfo;
using DingDingSync.Core.Entities;
using Newtonsoft.Json;

namespace DingDingSync.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录用户更改
    /// </summary>
    public class UserModifyOrgEventHandler : DingdingBaseEventHandler
    {
        private readonly IRepository<UserEntity, string> _userRepository;
        private readonly IRepository<UserDepartmentsRelationEntity, string> _deptUserRelaRepository;
        private readonly IDingdingAppService _dingdingAppService;
        private readonly IObjectMapper _objectMapper;

        public UserModifyOrgEventHandler(IRepository<UserEntity, string> userRepository,
            IRepository<UserDepartmentsRelationEntity, string> deptUserRelaRepository,
            IDingdingAppService dingdingAppService, IObjectMapper objectMapper, ILogger logger) : base(logger)
        {
            _userRepository = userRepository;
            _deptUserRelaRepository = deptUserRelaRepository;
            _dingdingAppService = dingdingAppService;
            _objectMapper = objectMapper;
        }

        public override void Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<UserModifyOrgEvent>(msg);
            if (eventInfo != null)
            {
                foreach (var userid in eventInfo.ID)
                {
                    try
                    {
                        var dingDingUser = _dingdingAppService.GetUserDetail(userid);

                        var dbUserEntity = _userRepository.FirstOrDefault(t => t.Id == userid);
                        if (dbUserEntity != null)
                        {
                            _objectMapper.Map(dingDingUser, dbUserEntity);
                            dbUserEntity.IsAdmin = IsAdmin(dingDingUser);

                            if (!dbUserEntity.AccountEnabled && dbUserEntity.IsAdmin)
                            {
                                dbUserEntity.AccountEnabled = true;
                            }

                            _userRepository.Update(dbUserEntity);

                            //删除原部门关联信息，重新插入
                            _deptUserRelaRepository.HardDelete(t => t.UserId == userid);
                            foreach (var deptId in dingDingUser.DeptIdList)
                            {
                                var rela = new UserDepartmentsRelationEntity()
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    UserId = userid,
                                    DeptId = deptId,
                                };
                                _deptUserRelaRepository.Insert(rela);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("人员信息修改回调事件发生异常", e);
                    }
                }
            }
        }
    }
}