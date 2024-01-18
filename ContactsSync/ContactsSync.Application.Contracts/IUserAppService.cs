using ContactsSync.Application.Contracts.Dtos;
using ContactsSync.Domain.Shared;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace ContactsSync.Application.Contracts;

public interface IUserAppService : IApplicationService
{
    Task BatchAddUserAsync(params CreateUserDto[] users);

    Task BatchAddDeptUserRelaAsync(params CreateUserDeptRelaDto[] relas);

    Task<List<DeptUserDto>> GetAllUsersAsync();

    Task<List<DeptUserRelaDto>> GetAllDeptUserRelaAsync();

    /// <summary>
    ///     获取部门下属用户信息
    /// </summary>
    /// <param name="deptId"></param>
    /// <returns></returns>
    Task<List<LdapDeptUserDto>> GetDeptUsersAsync(Guid deptId);

    /// <summary>
    ///     根据用户名获取用户信息
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    [RemoteService(false)]
    Task<LdapUserValidateDto> GetLdapUserAsync(string username);

    Task<UserSimpleDto> GetSimpleUserByUserIdAsync(string userId);

    Task CreateUserApproval(Guid uid, string applyData);
}