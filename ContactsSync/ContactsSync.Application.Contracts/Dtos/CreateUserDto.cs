using System.ComponentModel.DataAnnotations;

namespace ContactsSync.Application.Contracts.Dtos;

public class CreateUserDto
{
    /// <summary>
    /// 第三方平台UserId
    /// </summary>
    [MaxLength(64)]
    public string UserId { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [MaxLength(200)]
    public string Name { get; set; } = "";

    /// <summary>
    /// 头像
    /// </summary>
    [MaxLength(200)]
    public string Avatar { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [MaxLength(20)]
    public string Mobile { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    [MaxLength(100)]
    public string Position { get; set; }

    /// <summary>
    /// 个人邮箱
    /// </summary>
    [MaxLength(100)]
    public string Email { get; set; }

    /// <summary>
    /// 企业邮箱
    /// </summary>
    [MaxLength(100)]
    public string BizEmail { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }
    
    
    /// <summary>
    /// 用户名（系统生成，姓名全拼，同名加上序号）
    /// </summary>
    [MaxLength(50)]
    public string UserName { get; set; }
    
    /// <summary>
    /// 来源（钉钉、企微）
    /// </summary>
    [MaxLength(20)]
    public string Source { get; set; }
}

public class CreateUserDeptRelaDto
{
    
    /// <summary>
    /// 平台用户ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public long OriginDeptId { get; set; }

    /// <summary>
    /// 是否部门负责人
    /// </summary>
    public bool IsLeader { get; set; }
    /// <summary>
    /// 来源（钉钉、企微）
    /// </summary>
    [MaxLength(20)]
    public string Source { get; set; }
}