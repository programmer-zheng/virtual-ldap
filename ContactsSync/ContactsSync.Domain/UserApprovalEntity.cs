using System.ComponentModel.DataAnnotations;

namespace ContactsSync.Domain.Shared;

/// <summary>
/// 用户审批
/// </summary>
public class UserApprovalEntity : BaseEntity
{
    /// <summary>
    /// 用户表主键
    /// </summary>
    public Guid Uid { get; set; }

    /// <summary>
    /// 第三方平台UserId
    /// </summary>
    [MaxLength(64)]
    public string UserId { get; set; }

    /// <summary>
    /// 审批实例ID                                                                                                    
    /// </summary>
    [MaxLength(50)]
    public string InstanceId { get; set; }

    /// <summary>
    /// 审批结果
    /// </summary>
    public bool? Result { get; set; }
}