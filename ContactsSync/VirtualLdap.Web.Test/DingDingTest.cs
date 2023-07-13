using Abp.Json;
using Shouldly;
using VirtualLdap.Application.DingDingUtils;
using Xunit.Abstractions;

namespace VirtualLdap.Web.Test;

public class DingDingTest : VirtualLdapWebTestBase
{
    private readonly IDingTalkAppService _dingTalkAppService;
    private readonly ITestOutputHelper _output;

    public DingDingTest(ITestOutputHelper output)
    {
        _output = output;
        _dingTalkAppService = Resolve<IDingTalkAppService>();
    }

    [Fact]
    public async Task GetDepartments()
    {
        var departmentList = _dingTalkAppService.GetDepartmentList();
        departmentList.ShouldNotBeNull();
        departmentList.ShouldNotBeEmpty();
        _output.WriteLine(departmentList.ToJsonString(indented: true));
    }

    [Fact]
    public async Task GetDepartmentUsers()
    {
        var users = _dingTalkAppService.GetUserList(1);
        users.ShouldNotBeNull();
        users.ShouldNotBeEmpty();
        _output.WriteLine(users.ToJsonString(indented: true));
    }
}