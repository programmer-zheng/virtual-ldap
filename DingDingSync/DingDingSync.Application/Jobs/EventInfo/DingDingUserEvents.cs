using Newtonsoft.Json;

namespace DingDingSync.Application.Jobs.EventInfo
{
    public class DingDingUserEventBase : DingDingEventBaseInfo<string>
    {
        [JsonProperty(PropertyName = "UserId")]
        public override List<string> ID { get; set; }
    }

    /// <summary>
    /// 通讯录用户增加
    /// </summary>
    public class UserAddOrgEvent : DingDingUserEventBase
    {
    }

    /// <summary>
    /// 通讯录用户更改
    /// </summary>
    public class UserModifyOrgEvent : DingDingUserEventBase
    {
    }

    /// <summary>
    /// 通讯录用户离职
    /// </summary>
    public class UserLeaveOrgEvent : DingDingUserEventBase
    {
    }

    /// <summary>
    /// 加入企业后用户激活
    /// </summary>
    public class UserActiveOrgEvent : DingDingUserEventBase
    {
    }

    /// <summary>
    /// 通讯录用户被设为管理员
    /// </summary>
    public class OrgAdminAddEvent : DingDingUserEventBase
    {
    }


    /// <summary>
    /// 通讯录用户被取消设置管理员
    /// </summary>
    public class OrgAdminRemoveEvent : DingDingUserEventBase
    {
    }


    /// <summary>
    /// 员工角色信息发生变更
    /// </summary>
    public class LabelUserChangeEvent : DingDingUserEventBase
    {
        [JsonProperty(PropertyName = "UserIdList")]
        public override List<string> ID { get; set; }

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }
    }
}