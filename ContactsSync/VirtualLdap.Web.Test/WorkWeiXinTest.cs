using Abp.Json;
using Shouldly;
using VirtualLdap.Application.WorkWeixinUtils;
using Xunit.Abstractions;

namespace VirtualLdap.Web.Test;

public class WorkWeiXinTest : VirtualLdapWebTestBase
{
    private readonly IWorkWeixinAppService _workWeixinAppService;

    private readonly ITestOutputHelper _output;

    public WorkWeiXinTest(ITestOutputHelper output)
    {
        _output = output;
        _workWeixinAppService = Resolve<IWorkWeixinAppService>();
    }

    [Fact]
    public async Task GetDepartments()
    {
        var departmentList = await _workWeixinAppService.GetDepartmentList();
        departmentList.ShouldNotBeNull();
        departmentList.ShouldNotBeEmpty();
        _output.WriteLine(departmentList.ToJsonString(indented: true));
    }

    [Fact]
    public async Task GetDepartmentUsers()
    {
        var users = await _workWeixinAppService.GetUserList(1);
        users.ShouldNotBeNull();
        users.ShouldNotBeEmpty();
        _output.WriteLine(users.ToJsonString(indented: true));
    }
}