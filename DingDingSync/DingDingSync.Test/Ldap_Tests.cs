using Abp.Json;
using DingDingSync.Application.AppService;
using DingDingSync.Core.Entities;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DingDingSync.Test;

public class Ldap_Tests : DingDingSyncTestBase
{
    private readonly IDepartmentAppService _departmentAppService;
    private readonly ITestOutputHelper _output;

    public Ldap_Tests(ITestOutputHelper output)
    {
        _output = output;
        _departmentAppService = Resolve<IDepartmentAppService>();
    }

    [Fact]
    public async Task Departments_Test()
    {
        var allDepartments = await _departmentAppService.GetAllDepartments();
        _output.WriteLine(allDepartments.ToJsonString());
        allDepartments.ShouldNotBeNull();
        allDepartments.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task AddDepartment()
    {
        var entity = new DepartmentEntity()
        {
            Id = int.MaxValue,
            DeptName = "部门名称_单元测试",
            ParentId = 0
        };
        await _departmentAppService.AddDepartment(entity);
        var allDepartments = await _departmentAppService.GetAllDepartments();
        allDepartments.ShouldNotBeNull();
        entity.DeptName.ShouldBeEquivalentTo(allDepartments.Last().Name);
        _output.WriteLine(allDepartments.ToJsonString());
    }
}