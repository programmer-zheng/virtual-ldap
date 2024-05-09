using ContactsSync.Application.Contracts.OpenPlatformProvider;
using ContactsSync.Domain.Shared;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace ContactsSync.Application.Tests.Provider;

public class DingDingProviderTests : ContactsSyncApplicationTestBase
{
    private readonly IOpenPlatformProviderApplicationService _openPlatformProviderApplicationService;

    public readonly ITestOutputHelper Output;

    public DingDingProviderTests(ITestOutputHelper output)
    {
        _openPlatformProviderApplicationService = GetRequiredKeyedService<IOpenPlatformProviderApplicationService>(OpenPlatformProviderEnum.DingDing.ToString());
        Output = output;
    }

    [Fact]
    public async Task TestGetAccessTokenAsync()
    {
        var token = await _openPlatformProviderApplicationService.GetAccessTokenAsync();
        token.ShouldNotBeNullOrWhiteSpace();
        Output.WriteLine($"AccessToken：{token}");
    }

    [Fact]
    public async Task TestGetDepartmentListAndUsersAsync()
    {
        var departments = await _openPlatformProviderApplicationService.GetDepartmentListAsync();
        // var departments = new List<PlatformDepartmentDto> { new PlatformDepartmentDto() { DepartmentId = 1, DeptName = "test" } };
        departments.ShouldNotBeNull().ShouldNotBeEmpty();
        Output.WriteLine("部门列表：");
        Output.WriteLine(JsonConvert.SerializeObject(departments));
        foreach (var department in departments)
        {
            Output.WriteLine("");
            var users = await _openPlatformProviderApplicationService.GetDeptUserListAsync(department.DepartmentId);
            users.ShouldNotBeNull().ShouldNotBeEmpty();
            Output.WriteLine($"部门 ==> {department.DeptName} 下人员信息：");
            Output.WriteLine(JsonConvert.SerializeObject(users));
        }
    }

    [Fact]
    public async Task TestCreateApprovalTemplate()
    {
        var spNo = await _openPlatformProviderApplicationService.CreateApprovalTemplate();
        spNo.ShouldNotBeNullOrEmpty();
    }
}