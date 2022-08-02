using System;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Application.Jobs;
using DingDingSync.Application.Jobs.EventInfo;
using DingDingSync.Core.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using DingDingSync.Application.AppService;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DingDingSync.Application.Jobs.EventHandler
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
            IDingdingAppService dingdingAppService, IObjectMapper objectMapper)
        {
            _userRepository = userRepository;
            _deptUserRelaRepository = deptUserRelaRepository;
            _dingdingAppService = dingdingAppService;
            _objectMapper = objectMapper;
        }

        public override void Do(string msg)
        {
            var eventinfo = JsonConvert.DeserializeObject<UserModifyOrgEvent>(msg);
            if (eventinfo != null && eventinfo.ID != null)
            {
                foreach (var userid in eventinfo.ID)
                {
                    var dingdingUser = _dingdingAppService.GetUserDetail(userid);
                    //将钉钉返回的映射到数据实体
                    var userEntity = _objectMapper.Map<UserEntity>(dingdingUser);

                    var dbUserEntity = _userRepository.FirstOrDefault(t => t.Id == userid);
                    if (dbUserEntity != null)
                    {
                        dbUserEntity.UnionId = userEntity.UnionId;
                        dbUserEntity.Name = userEntity.Name;
                        dbUserEntity.JobNumber = userEntity.JobNumber;
                        dbUserEntity.HiredDate = userEntity.HiredDate;
                        dbUserEntity.Tel = userEntity.Tel;
                        dbUserEntity.Mobile = userEntity.Mobile;
                        dbUserEntity.WorkPlace = userEntity.WorkPlace;
                        dbUserEntity.Email = userEntity.Email;
                        dbUserEntity.Active = userEntity.Active;
                        dbUserEntity.IsAdmin = IsAdmin(dingdingUser);
                        dbUserEntity.Avatar = userEntity.Avatar;
                        dbUserEntity.Position = userEntity.Position;

                        if (!dbUserEntity.AccountEnabled && dbUserEntity.IsAdmin)
                        {
                            dbUserEntity.AccountEnabled = true;
                        }

                        _userRepository.Update(dbUserEntity);

                        //删除原部门关联信息，重新插入
                        _deptUserRelaRepository.HardDelete(t => t.UserId == userid);
                        foreach (var deptid in dingdingUser.DeptIdList)
                        {
                            var rela = new UserDepartmentsRelationEntity()
                            {
                                Id = Guid.NewGuid().ToString(),
                                UserId = userid,
                                DeptId = deptid,
                            };
                            _deptUserRelaRepository.Insert(rela);
                        }
                    }
                }
            }
        }
    }
}