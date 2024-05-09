using ContactsSync.Application.Contracts.OpenPlatformProvider;
using ContactsSync.Domain.Shared;
using Newtonsoft.Json;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace ContactsSync.Application.Tests.Provider;

public class WeWorkProviderTests : ContactsSyncApplicationTestBase
{
    private readonly IOpenPlatformProviderApplicationService _openPlatformProviderApplicationService;

    public readonly ITestOutputHelper Output;

    public WeWorkProviderTests(ITestOutputHelper output)
    {
        _openPlatformProviderApplicationService = GetRequiredKeyedService<IOpenPlatformProviderApplicationService>(OpenPlatformProviderEnum.WeWork.ToString());
        Output = output;
    }

    [Fact]
    public async Task TestGetAccessTokenAsync()
    {
        var token = await _openPlatformProviderApplicationService.GetAccessTokenAsync();
        Output.WriteLine($"AccessToken：{token}");
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task TestGetDepartmentListAndUsersAsync()
    {
        var departments = await _openPlatformProviderApplicationService.GetDepartmentListAsync();
        departments.ShouldNotBeNull().ShouldNotBeEmpty();
        Output.WriteLine("部门列表：");
        Output.WriteLine(JsonConvert.SerializeObject(departments));
        foreach (var department in departments)
        {
            var users = await _openPlatformProviderApplicationService.GetDeptUserListAsync(department.DepartmentId);
            users.ShouldNotBeNull();
            Output.WriteLine("");
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