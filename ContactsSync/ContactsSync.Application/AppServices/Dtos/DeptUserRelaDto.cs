namespace ContactsSync.Application.AppServices.Dtos;

public class DeptUserRelaDto
{
    /// <summary>
    /// 平台用户ID
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 部门ID
    /// </summary>
    public long OriginDeptId { get; set; }
}