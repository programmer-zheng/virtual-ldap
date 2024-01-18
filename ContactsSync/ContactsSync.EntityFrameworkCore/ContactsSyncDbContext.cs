using ContactsSync.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace ContactsSync.EntityFrameworkCore
{
    [ConnectionStringName("Default")]
    public class ContactsSyncDbContext : AbpDbContext<ContactsSyncDbContext>
    {
        public DbSet<DepartmentEntity> Departments { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<UserDepartmentsRelationEntity> DeptUserRelations { get; set; }
        
        public DbSet<UserApprovalEntity> UserAppovals { get; set; }

        public ContactsSyncDbContext(DbContextOptions<ContactsSyncDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ConfigureSettingManagement();
            modelBuilder.Entity<UserEntity>()
                .HasIndex(t => t.UserName).IsUnique().HasDatabaseName("IX_UserName");

            modelBuilder.Entity<UserDepartmentsRelationEntity>()
                .HasIndex(t => new { t.OriginDeptId, t.UserId, }).HasDatabaseName("IX_OriginDeptId_UserId");
        }
    }
}