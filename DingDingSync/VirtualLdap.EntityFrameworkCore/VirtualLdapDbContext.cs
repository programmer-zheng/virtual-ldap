using Abp.EntityFrameworkCore;
using VirtualLdap.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace VirtualLdap.EntityFrameworkCore
{
    public class VirtualLdapDbContext : AbpDbContext
    {

        public DbSet<DepartmentEntity> Departments { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<UserDepartmentsRelationEntity> UserDepartmentsRelations { get; set; }

        public VirtualLdapDbContext(DbContextOptions<VirtualLdapDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDepartmentsRelationEntity>().HasIndex(t => new { t.Id, t.DeptId });
            modelBuilder.Entity<UserEntity>().HasIndex(t => t.UserName).IsUnique().HasName("IX_UserName");


            //如果不调用基类中的 OnModelCreating 则无法使用abp自带的伪删除过滤
            base.OnModelCreating(modelBuilder);
            /* 全局查询筛选器 
             * https://docs.microsoft.com/zh-cn/ef/core/querying/filters 
             */
            //modelBuilder.Entity<DepartmentEntity>().HasQueryFilter(t => !t.IsDeleted);
            //modelBuilder.Entity<UserEntity>().HasQueryFilter(t => !t.IsDeleted);
            //modelBuilder.Entity<UserDepartmentsRelationEntity>().HasQueryFilter(t => !t.IsDeleted);

        }
    }
}
