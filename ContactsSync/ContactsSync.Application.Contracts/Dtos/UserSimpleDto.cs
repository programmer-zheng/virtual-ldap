namespace ContactsSync.Application.Contracts.Dtos;

public class UserSimpleDto
{
    /// <summary>
    ///     系统UID
    /// </summary>
    public Guid Uid { get; set; }

    /// <summary>
    ///     平台UserId
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    ///     姓名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    ///     用户名
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    ///     密码是否已初始化
    /// </summary>
    public bool IsPasswordInited { get; set; }

    /// <summary>
    ///     账号是否启用
    /// </summary>
    public bool IsEnabled { get; set; }

    /// <summary>
    ///     VPN账号是否启用
    /// </summary>
    public bool IsVpnEnabled { get; set; }
}