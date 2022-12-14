using Abp.Application.Services;
using DingDingSync.Application.AppService.Dtos;
using DingDingSync.Core.Entities;

namespace DingDingSync.Application.AppService
{
    public interface IUserAppService : IApplicationService
    {
        Task<UserEntity> GetByIdAsync(string userId);

        Task<UserEntity> GetByUserNameAsync(string username);

        Task<List<DeptUserDto>> DeptUsers(long deptId);

        Task<List<DeptUserDto>> GetAdminDeptUsers(string adminUserId);

        Task SyncDepartmentAndUser();

        Task<bool> ResetPassword(ResetPasswordViewModel model);

        Task<bool> EnableAccount(string userId, string username);

        Task<bool> EnableVpnAccount(string userId);

        Task<bool> ResetAccountPassword(string userId);

        Task<DeptUserDto> GetDeptUserDetail(string userid);

        Task<string> GetUserName(string name, List<UserEntity>? newUserList = null);

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task SendVerificationCode(string userid);

        Task<bool> ForgotPassword(ForgotPasswordViewModel model);
    }
}