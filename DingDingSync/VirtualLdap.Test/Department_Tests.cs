using System.Linq;
using System.Threading.Tasks;
using Abp.Json;
using VirtualLdap.Application.AppService;
using VirtualLdap.Core.Entities;
using Shouldly;
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
    public async Task Departments_Test()
    {
        var allDepartments = await _departmentAppService.GetAllDepartments();
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
        await _departmentAppService.AddDepartment(entity);
        var allDepartments = await _departmentAppService.GetAllDepartments();
        allDepartments.ShouldNotBeNull();
        allDepartments.Count.ShouldBeEquivalentTo(2);
        _output.WriteLine(allDepartments.ToJsonString());
    }
}