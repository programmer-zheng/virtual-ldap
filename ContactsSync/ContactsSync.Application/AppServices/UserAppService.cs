using ContactsSync.Application.AppServices.Dtos;
using ContactsSync.Domain.Shared;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ContactsSync.Application.AppServices;

public class UserAppService : ApplicationService, IUserAppService
{
    private readonly IRepository<DepartmentEntity> _deptRepository;
    private readonly IRepository<UserEntity> _userRepository;
    private readonly IRepository<UserDepartmentsRelationEntity> _deptUserRelaRepository;

    public UserAppService(IRepository<DepartmentEntity> deptRepository, IRepository<UserEntity> userRepository, IRepository<UserDepartmentsRelationEntity> deptUserRelaRepository)
    {
        _deptRepository = deptRepository;
        _userRepository = userRepository;
        _deptUserRelaRepository = deptUserRelaRepository;
    }

    public async Task BatchAddUserAsync(params UserEntity[] users)
    {
        await _userRepository.InsertManyAsync(users);
    }

    public async Task BatchAddDeptUserRelaAsync(params UserDepartmentsRelationEntity[] relas)
    {
        await _deptUserRelaRepository.InsertManyAsync(relas);
    }

    public async Task<List<DeptUserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetListAsync();
        var result = ObjectMapper.Map<List<UserEntity>, List<DeptUserDto>>(users);
        return result;
    }

    public async Task<List<DeptUserRelaDto>> GetAllDeptUserRelaAsync()
    {
        var relas = await _deptUserRelaRepository.GetListAsync();
        var result = ObjectMapper.Map<List<UserDepartmentsRelationEntity>, List<DeptUserRelaDto>>(relas);
        return result;
    }

    public async Task<List<DeptUserDto>> GetDeptUsersAsync(long deptId)
    {
        throw new NotImplementedException();
    }

    public async Task<LdapUserValidateDto?> GetLdapUserAsync(string username)
    {
        var userEntity = await _userRepository.FirstOrDefaultAsync(t => t.UserName == username || t.Email == username || t.BizEmail == username || t.Mobile == username);
        if (userEntity != null)
        {
            var result = ObjectMapper.Map<UserEntity, LdapUserValidateDto>(userEntity);
            return result;
        }

        return null;
    }

    public async Task<UserSimpleDto> GetSimpleUserByUserIdAsync(string userId)
    {
        var userEntity = await _userRepository.FirstOrDefaultAsync(t => t.UserId == userId);
        if (userEntity != null)
        {
            var result = ObjectMapper.Map<UserEntity, UserSimpleDto>(userEntity);
            return result;
        }

        return null;
    }
}