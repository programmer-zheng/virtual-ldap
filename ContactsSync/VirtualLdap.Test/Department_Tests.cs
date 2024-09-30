using Abp.Json;
using Shouldly;
using VirtualLdap.Application.AppService;
using VirtualLdap.Core.Entities;
using Xunit;
using Xunit.Abstractions;

namespace VirtualLdap.Test;

public class Department_Tests : VirtualLdapTestBase
{
    private readonly IDepartmentAppService _departmentAppService;
    private readonly ITestOutputHelper _output;

    public Department_Tests(ITestOutputHelper output)
    {
        _output = output;
        _departmentAppService = Resolve<IDepartmentAppService>();
    }

    [Fact]
    public void Departments_Test()
    {
        var allDepartments = _departmentAppService.GetAllDepartments();
        allDepartments.ShouldNotBeNull();
        allDepartments.ShouldNotBeEmpty();
        allDepartments.First().Name.ShouldBeEquivalentTo(TestDataBuilder.DefaultDepartName);
        _output.WriteLine(allDepartments.ToJsonString());
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
        await _departmentAppService.AddDepartmentAsync(entity);

        entity.DeptName = "修改名称";
        entity.ParentId = 1;
        await _departmentAppService.UpdateDepartmentAsync(entity);

        var allDepartments = _departmentAppService.GetAllDepartments();

        allDepartments.ShouldNotBeNull();
        allDepartments.Count.ShouldBeEquivalentTo(2);
        _output.WriteLine(allDepartments.ToJsonString());

        var depart = allDepartments.FirstOrDefault(t => t.Id == int.MaxValue);
        depart.ShouldNotBeNull();
        depart.Name.ShouldBeEquivalentTo(entity.DeptName);
    }
}