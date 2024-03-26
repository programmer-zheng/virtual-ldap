namespace ContactsSync.Domain.Shared;

public class UserDepartmentsRelationEntity : BaseEntity
{
    /// <summary>
    /// 平台用户ID
    /// </summary>
    public string UserId { get; set; }= "";

    /// <summary>
    /// 部门ID
    /// </summary>
    public long OriginDeptId { get; set; }

    /// <summary>
    /// 是否部门负责人
    /// </summary>
    public bool IsLeader { get; set; }
}