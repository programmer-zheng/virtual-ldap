using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace VirtualLdap.Core.Entities
{
    public class UserEntity : Entity<string>, ISoftDelete, IHasDeletionTime
    {
        /// <summary>
        /// 员工的userId。
        /// </summary>
        [Key]
        [MaxLength(50)]
        public override string Id { get; set; } = "";

        /// <summary>
        /// UnionId 员工在当前开发者企业账号范围内的唯一标识。
        /// </summary>
        [MaxLength(50)]
        public string UnionId { get; set; } = "";

        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; } = "";

        /// <summary>
        /// 工号
        /// </summary>
        [MaxLength(50)]
        public string? JobNumber { get; set; }

        /// <summary>
        /// 雇佣日期
        /// </summary>
        public DateTime? HiredDate { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [MaxLength(20)]
        public string? Tel { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(20)]
        public string? Mobile { get; set; }

        /// <summary>
        /// 工作地点
        /// </summary>
        [MaxLength(100)]
        public string? WorkPlace { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(100)]
        public string? Email { get; set; }

        /// <summary>
        /// 账号是否激活
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [MaxLength(200)]
        public string? Avatar { get; set; }

        /// <summary>
        /// 岗位
        /// </summary>
        [MaxLength(50)]
        public string? Position { get; set; }


        /// <summary>
        /// 用户名（系统生成，姓名全拼，同名加上序号）
        /// </summary>
        [MaxLength(50)]
        public string UserName { get; set; } = "";

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// 密码是否初始化 false：默认密码 true：自行修改了密码
        /// </summary>
        public bool PasswordInited { get; set; }

        public bool IsDeleted { get; set; }

        /// <summary>
        /// 账号是否启用
        /// </summary>
        public bool AccountEnabled { get; set; }

        /// <summary>
        /// 是否启用VPN账号
        /// </summary>
        public bool VpnAccountEnabled { get; set; }

        public DateTime? DeletionTime { get; set; }
    }
}