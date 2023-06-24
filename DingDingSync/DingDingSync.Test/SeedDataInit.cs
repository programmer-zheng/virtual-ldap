using DingDingSync.Core.Entities;
using DingDingSync.EntityFrameworkCore;
using System.Linq;

namespace DingDingSync.Test;

public class SeedDataInit
{
    public const string DefaultUserName = "测试账号";
    public const string DefaultDepartName = "测试部门";
    public const string DefaultUserId = "8A1FF092EECD4CF68C1508DC353467A9";
    public const long DefaultDeptId = 1L;

    private readonly DingDingSyncDbContext _context;

    public SeedDataInit(DingDingSyncDbContext context)
    {
        _context = context;
    }

    public void Init()
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
                DeptId = DefaultDeptId, UserId = DefaultUserId
            };
            _context.UserDepartmentsRelations.Add(deptUserRela);
            _context.SaveChanges();
        }
    }
}