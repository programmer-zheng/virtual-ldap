using Abp.Domain.Repositories;
using Castle.Core.Logging;
using DingDingSync.Application.Jobs.EventInfo;
using DingDingSync.Core.Entities;
using Newtonsoft.Json;

namespace DingDingSync.Application.Jobs.EventHandler
{
    /// <summary>
    /// 通讯录企业部门删除
    /// </summary>
    public class OrgDeptRemoveEventHandler : DingdingBaseEventHandler
    {
        private readonly IRepository<DepartmentEntity, long> _departmentRepository;
        private readonly IRepository<UserDepartmentsRelationEntity, string> _userDepartmentRelationRepository;

        public OrgDeptRemoveEventHandler(IRepository<DepartmentEntity, long> departmentRepository, ILogger logger,
            IRepository<UserDepartmentsRelationEntity, string> userDepartmentRelationRepository) : base(logger)
        {
            _departmentRepository = departmentRepository;
            _userDepartmentRelationRepository = userDepartmentRelationRepository;
        }

        public override void Do(string msg)
        {
            var eventInfo = JsonConvert.DeserializeObject<OrgDeptRemoveEvent>(msg);
            if (eventInfo != null && eventInfo.ID.Count > 0)
            {
                try
                {
                    //部门删除时，钉钉会提醒删除部门下的人员，有人员时无法删除部门，这里只需要删除部门表的数据及部门人员关系表数据
                    _departmentRepository.Delete(t => eventInfo.ID.Contains(t.Id));
                    _userDepartmentRelationRepository.Delete(t => eventInfo.ID.Contains(t.DeptId));
                }
                catch (Exception e)
                {
                    Logger.Error("删除部门时发生异常", e);
                }
            }
        }
    }
}