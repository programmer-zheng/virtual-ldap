using System.ComponentModel.DataAnnotations;
using Volo.Abp.Auditing;

namespace ContactsSync.Domain.Contacts;

/// <summary>
/// 用户
/// </summary>
public class UserEntity : ContactBaseEntity, IHasDeletionTime
{
    /// <summary>
    /// 第三方平台UserId
    /// </summary>
    [MaxLength(64)]
    public string UserId { get; set; } = "";

    /// <summary>
    /// 姓名
    /// </summary>
    [MaxLength(200)]
    public string Name { get; set; } = "";

    /// <summary>
    /// 头像
    /// </summary>
    [MaxLength(200)]
    public string? Avatar { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [MaxLength(20)]
    public string? Mobile { get; set; }

    /// <summary>
    /// 个人邮箱
    /// </summary>
    [MaxLength(100)]
    public string? Email { get; set; }

    /// <summary>
    /// 企业邮箱
    /// </summary>
    [MaxLength(100)]
    public string? BizEmail { get; set; }

    /// <summary>
    /// 职务
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 是否启用VPN
    /// </summary>
    public bool IsVpnEnabled { get; set; }

    /// <summary>
    /// 用户名（系统生成，姓名全拼，同名加上序号）
    /// </summary>
    [MaxLength(50)]
    public string UserName { get; set; } = "";

    /// <summary>
    /// 密码
    /// </summary>
    [MaxLength(100)]
    public string? Password { get; set; }

    /// <summary>
    /// 密码是否初始化 false：默认密码 true：自行修改了密码
    /// </summary>
    public bool IsPasswordInited { get; set; }

    public DateTime? DeletionTime { get; set; }
}