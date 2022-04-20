using Newtonsoft.Json;
using System.Collections.Generic;

namespace DingDingSync.Application.Jobs.EventInfo
{
    /// <summary>
    /// 通讯录用户增加
    /// </summary>
    public class user_add_org_event : EventBaseInfo<string>
    {
        [JsonProperty(PropertyName = "UserId")]
        public override List<string> ID
        {
            get => base.ID;
            set => base.ID = value;
        }
    }

    /// <summary>
    /// 通讯录用户更改
    /// </summary>
    public class user_modify_org_event : EventBaseInfo<string>
    {
        [JsonProperty(PropertyName = "UserId")]
        public override List<string> ID
        {
            get => base.ID;
            set => base.ID = value;
        }
    }

    /// <summary>
    /// 通讯录用户离职
    /// </summary>
    public class user_leave_org_event : EventBaseInfo<string>
    {
        [JsonProperty(PropertyName = "UserId")]
        public override List<string> ID
        {
            get => base.ID;
            set => base.ID = value;
        }
    }

    /// <summary>
    /// 加入企业后用户激活
    /// </summary>
    public class user_active_org_event : EventBaseInfo<string>
    {
        [JsonProperty(PropertyName = "UserId")]
        public override List<string> ID
        {
            get => base.ID;
            set => base.ID = value;
        }
    }

    /// <summary>
    /// 通讯录用户被设为管理员
    /// </summary>
    public class org_admin_add_event : EventBaseInfo<string>
    {
        [JsonProperty(PropertyName = "UserId")]
        public override List<string> ID
        {
            get => base.ID;
            set => base.ID = value;
        }
    }


    /// <summary>
    /// 通讯录用户被取消设置管理员
    /// </summary>
    public class org_admin_remove_event : EventBaseInfo<string>
    {
        [JsonProperty(PropertyName = "UserId")]
        public override List<string> ID
        {
            get => base.ID;
            set => base.ID = value;
        }
    }


    /// <summary>
    /// 员工角色信息发生变更
    /// </summary>
    public class label_user_change_event : EventBaseInfo<string>
    {
        [JsonProperty(PropertyName = "UserIdList")]
        public override List<string> ID
        {
            get => base.ID;
            set => base.ID = value;
        }

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; }
    }
}