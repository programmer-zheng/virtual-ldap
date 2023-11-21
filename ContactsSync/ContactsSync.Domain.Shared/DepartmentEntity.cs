using System.ComponentModel.DataAnnotations;

namespace ContactsSync.Domain.Shared;

/// <summary>
/// 部门
/// </summary>
public class DepartmentEntity : BaseEntity
{
    /// <summary>
    /// 原始ID
    /// </summary>
    public long OriginId { get; set; }

    /// <summary>
    /// 部门名称。 
    /// </summary>
    [MaxLength(200)]
    public string DeptName { get; set; } = "";


    /// <summary>
    /// 父部门ID，1为根部门。
    /// </summary>
    public long ParentId { get; set; }
}