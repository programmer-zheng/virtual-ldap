namespace ContactsSync.Application.OpenPlatformProvider;

public class PlatformDeptUserDto
{
    
    /// <summary>
    /// 平台用户的userId
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string Avatar { get; set; }
    
    /// <summary>
    /// 手机号
    /// </summary>
    public string Mobile { get; set; }

    /// <summary>
    /// 职位
    /// </summary>
    public string Position { get; set; }

    /// <summary>
    /// 个人邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 企业邮箱
    /// </summary>
    public string BizEmail { get; set; }

    /// <summary>
    /// 是否部门主管
    /// </summary>
    public bool IsDeptLeader { get; set; }

    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActivated { get; set; }
}