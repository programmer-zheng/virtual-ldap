using ContactsSync.Application.OpenPlatformProvider;
using ContactsSync.Domain.Shared;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace ContactsSync.Application.AppServices;

public class ContactsSync : IContactsSync
{
    private readonly IOpenPlatformProvider _openPlatformProvider;
    private readonly IDepartmentAppService _departmentAppService;
    private readonly IUserAppService _userAppService;
    private readonly IObjectMapper _objectMapper;

    public ContactsSync(IOpenPlatformProvider openPlatformProvider, IDepartmentAppService departmentAppService, IUserAppService userAppService, IObjectMapper objectMapper)
    {
        _openPlatformProvider = openPlatformProvider;
        _departmentAppService = departmentAppService;
        _userAppService = userAppService;
        _objectMapper = objectMapper;
    }

    [UnitOfWork]
    public async Task SyncDepartmentAndUser()
    {
        var platformDepartments = await _openPlatformProvider.GetDepartmentListAsync();
        if (platformDepartments?.Count > 0)
        {
            // 数据库中的数据
            var existsDepartmentList = await _departmentAppService.GetAllDepartments();
            var existsUserList = await _userAppService.GetAllUsersAsync();
            var existsRelaList = await _userAppService.GetAllDeptUserRelaAsync();

            // 需要新增的数据
            var deptList = new List<DepartmentEntity>();
            var userList = new List<UserEntity>();
            var relaList = new List<UserDepartmentsRelationEntity>();
            foreach (var departmentDto in platformDepartments)
            {
                // 映射部门
                var departmentEntity = _objectMapper.Map<PlatformDepartmentDto, DepartmentEntity>(departmentDto);
                if (existsDepartmentList.All(t => t.OriginId != departmentEntity.OriginId))
                {
                    departmentEntity.Source = _openPlatformProvider.Source;
                    deptList.Add(departmentEntity);
                }

                var platformDeptUsers = await _openPlatformProvider.GetDeptUserListAsync(departmentDto.DepartmentId);
                foreach (var deptUserDto in platformDeptUsers)
                {
                    // 映射人员
                    var userEntity = _objectMapper.Map<PlatformDeptUserDto, UserEntity>(deptUserDto);
                    if (existsUserList.All(t => t.UserId != deptUserDto.UserId) && userList.All(t => t.UserId != deptUserDto.UserId))
                    {
                        userEntity.Source = _openPlatformProvider.Source;
                        userList.Add(userEntity);
                    }

                    if (!existsRelaList.Any(t => t.UserId == deptUserDto.UserId && t.OriginDeptId == departmentDto.DepartmentId))
                    {
                        relaList.Add(new UserDepartmentsRelationEntity
                        {
                            UserId = deptUserDto.UserId,
                            OriginDeptId = departmentDto.DepartmentId,
                            Source = _openPlatformProvider.Source
                        });
                    }
                }
            }

            // 批量添加部门
            await _departmentAppService.BatchAddDepartmentAsync(deptList.ToArray());
            // 批量添加人员
            await _userAppService.BatchAddUserAsync(userList.ToArray());
            // 批量添加部门人员关系
            await _userAppService.BatchAddDeptUserRelaAsync(relaList.ToArray());
        }
    }
}