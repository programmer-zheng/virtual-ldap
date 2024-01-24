using ContactsSync.Application.Contracts;
using Shouldly;
using Xunit;

namespace ContactsSync.Application.Tests.AppServices;

public class UserAppServiceTests : ContactsSyncApplicationTestBase
{
    private readonly IUserAppService _userAppService;

    public UserAppServiceTests()
    {
        _userAppService = GetRequiredService<IUserAppService>();
    }

    [Fact]
    public async Task TestAddUser()
    {
        var result = await _userAppService.GetAllUsersAsync();

        result.Count.ShouldBeEquivalentTo(1);
        result.FirstOrDefault()?.Name.ShouldBe("test");
    }
}