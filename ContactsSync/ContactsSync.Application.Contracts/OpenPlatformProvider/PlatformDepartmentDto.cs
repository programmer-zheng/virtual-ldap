namespace ContactsSync.Application.Contracts.OpenPlatformProvider;

public class PlatformDepartmentDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public long DepartmentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DeptName { get; set; }

    /// <summary>
    /// 父级部门ID
    /// </summary>
    public long ParentId { get; set; }
}