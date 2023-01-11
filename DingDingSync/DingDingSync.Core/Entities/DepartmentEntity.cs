using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace DingDingSync.Core.Entities
{
    public class DepartmentEntity : Entity<long>, ISoftDelete
    {

        /// <summary>
        /// 钉钉 部门ID
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public override long Id { get; set; }

        /// <summary>
        /// 部门名称。 
        /// </summary>
        [MaxLength(100)]
        public string DeptName { get; set; } = "";

        /// <summary>
        /// 父部门ID，1为根部门。
        /// </summary>
        public long ParentId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
