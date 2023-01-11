using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace DingDingSync.Core.Entities
{
    public class UserDepartmentsRelationEntity : Entity<string>, ISoftDelete
    {

        [MaxLength(50)]
        public override string Id { get; set; } = "";

        [MaxLength(50)]
        public string UserId { get; set; } = "";

        /// <summary>
        /// 部门ID
        /// </summary>
        public long DeptId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
