using Abp.Application.Services;
using VirtualLdap.Application.AppService.Dtos;
using VirtualLdap.Core.Entities;

namespace VirtualLdap.Application.AppService
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

        Task<DeptUserDto> GetDeptUserDetail(string userid);

        /// <summary>
        /// 获取用户的用户名
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="newUserList">同批次新增人员列表</param>
        /// <returns>用户的用户名</returns>
        Task<string> GetUserName(string name, List<UserEntity>? newUserList = null);

        Task SyncDepartmentAndUser();

        Task SyncDepartMentAndUserFromWorkWeixin();

        #region 账号启用

        Task<bool> EnableAccount(string userId, string username);

        Task<bool> EnableVpnAccount(string userId);

        #endregion

        #region 密码相关

        /// <summary>
        /// 重新修改密码 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> ResetPassword(ResetPasswordViewModel model);

        /// <summary>
        /// 重置用户账号密码为默认密码
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        Task<bool> ResetAccountPassword(string userId);

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> ForgotPassword(ForgotPasswordViewModel model);

        #endregion

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task SendVerificationCode(string userid);
    }
}