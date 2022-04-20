using Newtonsoft.Json;
using System.Collections.Generic;

namespace DingDingSync.Application.Jobs.EventInfo
{
    /// <summary>
    /// 通讯录企业部门创建
    /// </summary>
    public class org_dept_create_event : EventBaseInfo<long>
    {
        [JsonProperty(PropertyName = "DeptId")]
        public override List<long> ID { get => base.ID; set => base.ID = value; }
    }

    /// <summary>
    /// 通讯录企业部门修改
    /// </summary>
    public class org_dept_modify_event : EventBaseInfo<long>
    {
        [JsonProperty(PropertyName = "DeptId")]
        public override List<long> ID { get => base.ID; set => base.ID = value; }
    }

    /// <summary>
    /// 通讯录企业部门删除
    /// </summary>
    public class org_dept_remove_event : EventBaseInfo<long>
    {
        [JsonProperty(PropertyName = "DeptId")]
        public override List<long> ID { get => base.ID; set => base.ID = value; }
    }

    /// <summary>
    /// 企业解散
    /// </summary>
    public class org_remove_event : EventBaseInfo<long>
    {

    }

}
