using Abp.Domain.Repositories;
using Castle.Core.Logging;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.Jobs.EventInfo;
using DingDingSync.Core.Entities;
using Newtonsoft.Json;
using System;

namespace DingDingSync.Application.Jobs.EventHandler.DingDing
{
    /// <summary>
    /// 通讯录企业部门修改
    /// </summary>
    public class OrgDeptModifyEventHandler : DingdingBaseEventHandler
    {
        private readonly IRepository<DepartmentEntity, long> _departmentRepository;
        private readonly IDingdingAppService _dingDingAppService;

        public OrgDeptModifyEventHandler(IRepository<DepartmentEntity, long> departmentRepository,
            IDingdingAppService dingDingAppService, ILogger logger) : base(logger)
        {
            _departmentRepository = departmentRepository;
            _dingDingAppService = dingDingAppService;
        }

        public override void Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<OrgDeptModifyEvent>(msg);
            if (eventInfo != null)
            {
                foreach (var deptId in eventInfo.ID)
                {
                    try
                    {
                        var dingDingUser = _dingDingAppService.GetDepartmentDetail(deptId);
                        var dbDept = _departmentRepository.FirstOrDefault(deptId);
                        if (dbDept != null)
                        {
                            dbDept.DeptName = dingDingUser.Name;
                            dbDept.ParentId = dingDingUser.ParentId;
                            _departmentRepository.Update(dbDept);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("修改部门信息时发生异常", e);
                    }
                }
            }
        }
    }
}