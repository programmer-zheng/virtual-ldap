using ContactsSync.Application.Background;
using ContactsSync.Application.Contracts;
using ContactsSync.Application.Contracts.Dtos;
using ContactsSync.Application.Contracts.OpenPlatformProvider;
using ContactsSync.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ContactsSync.Application.AppServices;

public class UserAppService : ApplicationService, IUserAppService
{
    private readonly IRepository<DepartmentEntity> _deptRepository;
    private readonly IRepository<UserEntity> _userRepository;
    private readonly IRepository<UserDepartmentsRelationEntity> _deptUserRelaRepository;
    private readonly IRepository<UserApprovalEntity> _userApprovalRepository;
    private readonly ContactsSyncConfigOptions _contactsSyncConfigOptions;

    public UserAppService(IRepository<DepartmentEntity> deptRepository,
        IRepository<UserEntity> userRepository, IRepository<UserDepartmentsRelationEntity> deptUserRelaRepository,
        IRepository<UserApprovalEntity> userApprovalRepository, IOptionsSnapshot<ContactsSyncConfigOptions> syncConfig)
    {
        _deptRepository = deptRepository;
        _userRepository = userRepository;
        _deptUserRelaRepository = deptUserRelaRepository;
        _userApprovalRepository = userApprovalRepository;
        _contactsSyncConfigOptions = syncConfig.Value;
    }

    public async Task BatchAddUserAsync(params CreateUserDto[] users)
    {
        var entities = ObjectMapper.Map<CreateUserDto[], UserEntity[]>(users);
        await _userRepository.InsertManyAsync(entities);
    }

    public async Task BatchAddDeptUserRelaAsync(params CreateUserDeptRelaDto[] relas)
    {
        var entities = ObjectMapper.Map<CreateUserDeptRelaDto[], UserDepartmentsRelationEntity[]>(relas);
        await _deptUserRelaRepository.InsertManyAsync(entities);
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

    public async Task<List<LdapDeptUserDto>> GetDeptUsersAsync(Guid deptId)
    {
        var userQuery = await _userRepository.GetQueryableAsync();
        var relaQuery = await _deptUserRelaRepository.GetQueryableAsync();
        var deptQuery = await _deptRepository.GetQueryableAsync();
        var query = from x in userQuery
            join rela in relaQuery on x.UserId equals rela.UserId
            join dept in deptQuery on rela.OriginDeptId equals dept.OriginId
            where dept.Id == deptId && x.IsEnabled == true
            select x;
        var userList = await query.ToListAsync();

        var relas = await (from rel in relaQuery
            join dept in deptQuery on rel.OriginDeptId equals dept.OriginId
            select new { rel.UserId, dept.Id, dept.OriginId }).ToListAsync();

        var result = ObjectMapper.Map<List<UserEntity>, List<LdapDeptUserDto>>(userList);
        foreach (var user in result)
        {
            user.Departments = relas.Where(t => t.UserId == user.UserId).Select(t => t.Id);
        }

        return result;
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

    public async Task CreateUserApproval(Guid uid, string applyData)
    {
        // 检查是否提交过审批
        var userApproval = await _userApprovalRepository.FirstOrDefaultAsync(t => t.Uid == uid);
        if (userApproval is not null && userApproval.Result is null)
        {
            throw new UserFriendlyException("已提交过审批请求");
        }

        // 获取用户是否存在
        var user = await _userRepository.FirstOrDefaultAsync(t => t.Id == uid);
        if (user is null)
        {
            throw new UserFriendlyException("不存在的用户，无法提交审批");
        }

        var deptUserRelaQuable = await _deptUserRelaRepository.GetQueryableAsync();
        // 获取用户的部门领导
        var leadersQueryable = from rela in deptUserRelaQuable
            join userDept in deptUserRelaQuable on rela.OriginDeptId equals userDept.OriginDeptId
            where userDept.UserId == user.UserId && rela.IsLeader == true
            select rela.UserId;
        var leaders = await leadersQueryable.ToListAsync();

        // todo 待abp 8.1发布后，使用LazyServiceProvider替换
        var _openPlatformProvider = ServiceProvider.GetKeyedService<IOpenPlatformProviderApplicationService>(_contactsSyncConfigOptions.OpenPlatformProvider.ToString());
        // 创建审批实例
        var approvalInstance = await _openPlatformProvider.CreateApprovalInstance(user.UserId, leaders, applyData);

        // 保存用户审批实例
        userApproval = new UserApprovalEntity() { Uid = uid, UserId = user.UserId, InstanceId = approvalInstance, Source = _openPlatformProvider.Source };
        await _userApprovalRepository.InsertAsync(userApproval);
    }
}