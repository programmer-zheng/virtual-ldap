namespace ContactsSync.Application.OpenPlatformProvider;

/// <summary>
/// 开放平台
/// </summary>
public interface IOpenPlatformProvider
{
    /// <summary>
    /// 数据来源
    /// </summary>
    public string Source { get; }


    /// <summary>
    /// 审批模板在配置文件中的Key
    /// </summary>
    public string ApprovalTemplateKey { get; }

    /// <summary>
    /// 获取授权跳转Url
    /// </summary>
    /// <param name="redirectUrl"></param>
    /// <returns></returns>
    Task<string> GetAuthorizeUrl(string redirectUrl);

    /// <summary>
    /// 获取AccessToken
    /// </summary>
    /// <returns></returns>
    Task<string> GetAccessTokenAsync();

    /// <summary>
    /// 获取部门列表
    /// </summary>
    /// <param name="parentDeptId">父级部门ID</param>
    /// <returns></returns>
    Task<List<PlatformDepartmentDto>> GetDepartmentListAsync(long? parentDeptId = null);

    /// <summary>
    /// 获取部门成员列表
    /// </summary>
    /// <param name="deptId"></param>
    /// <param name="cursor"></param>
    /// <returns></returns>
    Task<List<PlatformDeptUserDto>> GetDeptUserListAsync(long deptId, long cursor = 0);

    /// <summary>
    /// 根据授权码获取用户ID
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    Task<string> GetUserIdByCode(string code);

    /// <summary>
    /// 创建审批模板
    /// </summary>
    /// <returns>模板ID</returns>
    Task<string?> CreateApprovalTemplate();

    /// <summary>
    /// 获取配置文件中审批模板Id
    /// </summary>
    /// <returns></returns>
    Task<string?> GetConfigedApprovalTemplateId();
}