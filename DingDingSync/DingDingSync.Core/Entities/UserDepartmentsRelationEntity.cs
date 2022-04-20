using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
