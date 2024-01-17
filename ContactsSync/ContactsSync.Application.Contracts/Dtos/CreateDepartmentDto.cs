using System.ComponentModel.DataAnnotations;

namespace ContactsSync.Application.Contracts.Dtos;

public class CreateDepartmentDto
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
    
    /// <summary>
    /// 来源（钉钉、企微）
    /// </summary>
    [MaxLength(20)]
    public string Source { get; set; }
}