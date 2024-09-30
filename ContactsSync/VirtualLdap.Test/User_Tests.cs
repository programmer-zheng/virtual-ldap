using Abp.Extensions;
using Shouldly;
using VirtualLdap.Application;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.AppService.Dtos;
using Xunit;
using Xunit.Abstractions;

namespace VirtualLdap.Test;

public class User_Tests : VirtualLdapTestBase
{
    private readonly IUserAppService _userAppService;
    private readonly ITestOutputHelper _output;

    public User_Tests(ITestOutputHelper output)
    {
        _output = output;
        _userAppService = Resolve<IUserAppService>();
    }

    [Fact]
    public async Task EnableUserAccount_Test()
    {
        var userName = await _userAppService.GetUserNameAsync(TestDataBuilder.DefaultUserName);
        var user = await _userAppService.GetByIdAsync(TestDataBuilder.DefaultUserId);

        // 默认账号应该为未启用状态
        user.AccountEnabled.ShouldBeFalse();

        // 默认无用户名
        user.UserName.IsNullOrWhiteSpace().ShouldBeTrue();

        // 启用账户并设置用户名
        var enableAccount = await _userAppService.EnableAccountAsync(TestDataBuilder.DefaultUserId, userName);
        enableAccount.ShouldBeTrue();
    }

    [Fact]
    public async Task UserChangePassword_Test()
    {
        var dto = new ResetPasswordViewModel
        {
            UserId = TestDataBuilder.DefaultUserId,
            OldPassword = "123456",
            NewPassword = "Abc@123456",
            ConfirmPassword = "Abc@123456",
        };
        var result = await _userAppService.ResetPasswordAsync(dto);
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task AdminResetUserPassword_Test()
    {
        var users = _userAppService.GetAdminDeptUsers(TestDataBuilder.DefaultUserId);
        users.ShouldNotBeNull();
        users.ShouldNotBeEmpty();

        var userid = users.First().Userid;
        
        var result =await _userAppService.ResetAccountPasswordAsync(userid);
        result.ShouldBeTrue();

        var userDetail = await _userAppService.GetByIdAsync(userid);
        userDetail.Password.ShouldBeEquivalentTo("123456".DesEncrypt());

    }
}