﻿using System;
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

namespace DingDingSync.Application.Jobs.EventHandler
{
    /// <summary>
    /// 通讯录企业部门修改
    /// </summary>
    public class OrgDeptModifyEventHandler : DingdingBaseEventHandler
    {
        private readonly IRepository<DepartmentEntity, long> _departmentRepository;
        private readonly IDingdingAppService _dingdingAppService;

        public OrgDeptModifyEventHandler(IRepository<DepartmentEntity, long> departmentRepository,
            IDingdingAppService dingdingAppService)
        {
            _departmentRepository = departmentRepository;
            _dingdingAppService = dingdingAppService;
        }

        public override void Do(string msg)
        {
            var classname = GetType().Name;
            var eventinfo = JsonConvert.DeserializeObject<OrgDeptModifyEvent>(msg);
            if (eventinfo != null && eventinfo.ID != null)
            {
                foreach (var deptid in eventinfo.ID)
                {
                    var dingdingDept = _dingdingAppService.GetDepartmentDetail(deptid);
                    var dbDept = _departmentRepository.FirstOrDefault(deptid);
                    if (dbDept != null)
                    {
                        dbDept.DeptName = dingdingDept.Name;
                        dbDept.ParentId = dingdingDept.ParentId;
                        _departmentRepository.Update(dbDept);
                    }
                }
            }
        }
    }
}