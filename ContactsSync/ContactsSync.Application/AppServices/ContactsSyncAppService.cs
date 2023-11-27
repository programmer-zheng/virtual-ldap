using System.Text;
using System.Text.RegularExpressions;
using ContactsSync.Application.AppServices.Dtos;
using ContactsSync.Application.OpenPlatformProvider;
using ContactsSync.Domain.Shared;
using TinyPinyin;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace ContactsSync.Application.AppServices;

public class ContactsSyncAppService : IContactsSyncAppService
{
    private readonly IOpenPlatformProvider _openPlatformProvider;
    private readonly IDepartmentAppService _departmentAppService;
    private readonly IUserAppService _userAppService;
    private readonly IObjectMapper _objectMapper;

    public ContactsSyncAppService(IOpenPlatformProvider openPlatformProvider, IDepartmentAppService departmentAppService, IUserAppService userAppService,
        IObjectMapper objectMapper)
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

                // 部门下的人员数据
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

                    // 部门人员关联
                    if (!existsRelaList.Any(t => t.UserId == deptUserDto.UserId && t.OriginDeptId == departmentDto.DepartmentId))
                    {
                        relaList.Add(new UserDepartmentsRelationEntity
                        {
                            UserId = deptUserDto.UserId,
                            OriginDeptId = departmentDto.DepartmentId,
                            Source = _openPlatformProvider.Source,
                            IsLeader = deptUserDto.IsDeptLeader
                        });
                    }
                }
            }

            foreach (var user in userList)
            {
                var userName = await GetUserNameAsync(user.Name, userList, existsUserList);
                user.UserName = userName;
            }

            // 批量添加部门
            await _departmentAppService.BatchAddDepartmentAsync(deptList.ToArray());
            // 批量添加人员
            await _userAppService.BatchAddUserAsync(userList.ToArray());
            // 批量添加部门人员关系
            await _userAppService.BatchAddDeptUserRelaAsync(relaList.ToArray());
        }
    }

    private async Task<string> GetUserNameAsync(string name, List<UserEntity> newUserList, List<DeptUserDto> dbUserList)
    {
        var username = new StringBuilder();

        var regex = new Regex(@"^[\u4e00-\u9fa5]+$", RegexOptions.IgnoreCase);

        var match = regex.Match(name);
        if (match.Success)
        {
            //对重名人员，去掉人名以外字符
            var pinyin = PinyinHelper.GetPinyin(match.Groups[0].Value, "").ToLower();
            username.Append(pinyin);

            var sameCount = dbUserList.Count(t => t.UserName.Contains(pinyin));
            var newUserSameNameCount = 0;
            newUserSameNameCount = newUserList.Count(t => !string.IsNullOrWhiteSpace(t.UserName)
                                                          && t.UserName.StartsWith(username.ToString(),
                                                              StringComparison.OrdinalIgnoreCase));

            if (sameCount > 0 || newUserSameNameCount > 0)
            {
                username.Append(sameCount + newUserSameNameCount + 1);
            }
        }
        else
        {
            username.Append(name);
        }

        return username.ToString().ToLower();
    }
}