using Newtonsoft.Json;

namespace DingDingSync.Application.Jobs.EventInfo
{
    public class OrgDeptEventBase : DingDingEventBaseInfo<long>
    {
        [JsonProperty(PropertyName = "DeptId")]
        public override List<long> ID { get; set; }
    }

    /// <summary>
    /// 通讯录企业部门创建
    /// </summary>
    public class OrgDeptCreateEvent : OrgDeptEventBase
    {
    }

    /// <summary>
    /// 通讯录企业部门修改
    /// </summary>
    public class OrgDeptModifyEvent : OrgDeptEventBase
    {
    }

    /// <summary>
    /// 通讯录企业部门删除
    /// </summary>
    public class OrgDeptRemoveEvent : OrgDeptEventBase
    {
    }

    /// <summary>
    /// 企业解散
    /// </summary>
    public class OrgRemoveEvent : OrgDeptEventBase
    {
    }
}