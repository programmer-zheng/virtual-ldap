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
    public class user_modify_org_event_handler : DingdingBaseEventHandler
    {
        public user_modify_org_event_handler(IRepository<DepartmentEntity, long> departmentRepository,
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
            var eventinfo = JsonConvert.DeserializeObject<user_modify_org_event>(msg);
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