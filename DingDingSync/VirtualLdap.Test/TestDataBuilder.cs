using System.Linq;
using VirtualLdap.Application;
using VirtualLdap.Core.Entities;
using VirtualLdap.EntityFrameworkCore;

namespace VirtualLdap.Test;

public class TestDataBuilder
{
    public const string DefaultUserName = "测试账号";
    public const string DefaultDepartName = "测试部门";
    public const string DefaultUserId = "8A1FF092EECD4CF68C1508DC353467A9";
    public const long DefaultDeptId = 1L;

    private readonly VirtualLdapDbContext _context;

    public TestDataBuilder(VirtualLdapDbContext context)
    {
        _context = context;
    }

    public void Build()
    {
        var defaultDepart = _context.Departments.FirstOrDefault(t => t.DeptName == DefaultDepartName);
        if (defaultDepart == null)
        {
            defaultDepart = new DepartmentEntity()
            {
                Id = DefaultDeptId, DeptName = DefaultDepartName, ParentId = 0
            };
            _context.Departments.Add(defaultDepart);
            _context.SaveChanges();
        }

        var defaultUser = _context.Users.FirstOrDefault(t => t.Name == DefaultUserName);
        if (defaultUser == null)
        {
            defaultUser = new UserEntity()
            {
                Id = DefaultUserId,
                Name = DefaultUserName,
                IsAdmin = true,
                Active = true,
                Password = "123456".DesEncrypt()
            };
            _context.Users.Add(defaultUser);
            _context.SaveChanges();
        }

        var deptUserRela =
            _context.UserDepartmentsRelations.FirstOrDefault(
                t => t.DeptId == DefaultDeptId && t.UserId == DefaultUserId);
        if (deptUserRela == null)
        {
            deptUserRela = new UserDepartmentsRelationEntity()
            {
                Id = Guid.NewGuid().ToString("N"), DeptId = DefaultDeptId, UserId = DefaultUserId
            };
            _context.UserDepartmentsRelations.Add(deptUserRela);
            _context.SaveChanges();
        }

        var regularUser = _context.Users.FirstOrDefault(t => t.Name == "普通用户");
        if (regularUser == null)
        {
            var userId = Guid.NewGuid().ToString("N");
            _context.Users.Add(new UserEntity() { Name = "普通用户", Id = userId });
            _context.UserDepartmentsRelations.Add(new UserDepartmentsRelationEntity()
                { Id = Guid.NewGuid().ToString("N"), DeptId = DefaultDeptId, UserId = userId });
            _context.SaveChanges();
        }
    }
}