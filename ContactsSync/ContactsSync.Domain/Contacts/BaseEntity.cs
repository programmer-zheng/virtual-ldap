using System.ComponentModel.DataAnnotations;
using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace ContactsSync.Domain.Shared;

public class BaseEntity : Entity<Guid>, ISoftDelete
{
    public BaseEntity()
    {
        Id = Guid.NewGuid();
    }

    public bool IsDeleted { get; set; }

    /// <summary>
    /// 来源（钉钉、企微）
    /// </summary>
    [MaxLength(20)]
    public string Source { get; set; }= "";
}