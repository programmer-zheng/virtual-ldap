using Abp.Application.Services;
using DingDingSync.Application.AppService.Dtos;
using DingDingSync.Core.Entities;

namespace DingDingSync.Application.AppService
{
    public interface IUserAppService : IApplicationService
    {
        /// <summary>
        /// 根据主键获取用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserEntity> GetByIdAsync(string userId);

        /// <summary>
        /// 根据用户名获取用户信息
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<UserEntity> GetByUserNameAsync(string username);

        /// <summary>
        /// 获取部门下属用户信息
        /// </summary>
        /// <param name="deptId"></param>
        /// <returns></returns>
        Task<List<DeptUserDto>> DeptUsers(long deptId);

        /// <summary>
        /// 获取管理人员下属用户信息
        /// </summary>
        /// <param name="adminUserId"></param>
        /// <returns></returns>
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