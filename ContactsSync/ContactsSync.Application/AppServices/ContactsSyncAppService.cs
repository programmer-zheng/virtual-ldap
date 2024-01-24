using System.Text;
using System.Text.RegularExpressions;
using ContactsSync.Application.Background;
using ContactsSync.Application.Contracts;
using ContactsSync.Application.Contracts.Dtos;
using ContactsSync.Application.Contracts.OpenPlatformProvider;
using ContactsSync.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TinyPinyin;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;

namespace ContactsSync.Application.AppServices;

public class ContactsSyncAppService : ApplicationService, IContactsSyncAppService
{
    private readonly IDepartmentAppService _departmentAppService;
    private readonly IUserAppService _userAppService;

    private readonly ContactsSyncConfigOptions _syncConfig;

    public ContactsSyncAppService(IDepartmentAppService departmentAppService, IUserAppService userAppService,
        IOptionsSnapshot<ContactsSyncConfigOptions> syncConfig)
    {
        _departmentAppService = departmentAppService;
        _userAppService = userAppService;
        _syncConfig = syncConfig.Value;
    }

    [UnitOfWork]
    public async Task SyncDepartmentAndUser()
    {
        // todo 待abp 8.1发布后，使用LazyServiceProvider替换
        var openPlatformProvider = ServiceProvider.GetKeyedService<IOpenPlatformProviderApplicationService>(_syncConfig.OpenPlatformProvider.ToString());
        var platformDepartments = await openPlatformProvider!.GetDepartmentListAsync()!;
        if (platformDepartments?.Count > 0)
        {
            // 数据库中的数据
            var existsDepartmentList = await _departmentAppService.GetAllDepartments();
            var existsUserList = await _userAppService.GetAllUsersAsync();
            var existsRelaList = await _userAppService.GetAllDeptUserRelaAsync();

            // 需要新增的数据
            var deptList = new List<CreateDepartmentDto>();
            var userList = new List<CreateUserDto>();
            var relaList = new List<CreateUserDeptRelaDto>();
            foreach (var departmentDto in platformDepartments)
            {
                // 映射部门
                var departmentEntity = ObjectMapper.Map<PlatformDepartmentDto, CreateDepartmentDto>(departmentDto);
                if (existsDepartmentList.All(t => t.OriginId != departmentEntity.OriginId))
                {
                    departmentEntity.Source = openPlatformProvider.Source;
                    deptList.Add(departmentEntity);
                }

                // 部门下的人员数据
                var platformDeptUsers = await openPlatformProvider.GetDeptUserListAsync(departmentDto.DepartmentId);
                foreach (var deptUserDto in platformDeptUsers)
                {
                    // 映射人员
                    var userEntity = ObjectMapper.Map<PlatformDeptUserDto, CreateUserDto>(deptUserDto);
                    if (existsUserList.All(t => t.UserId != deptUserDto.UserId) && userList.All(t => t.UserId != deptUserDto.UserId))
                    {
                        userEntity.Source = openPlatformProvider.Source;
                        userList.Add(userEntity);
                    }

                    // 部门人员关联
                    if (!existsRelaList.Any(t => t.UserId == deptUserDto.UserId && t.OriginDeptId == departmentDto.DepartmentId))
                    {
                        relaList.Add(new CreateUserDeptRelaDto
                        {
                            UserId = deptUserDto.UserId,
                            OriginDeptId = departmentDto.DepartmentId,
                            Source = openPlatformProvider.Source,
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

    private async Task<string> GetUserNameAsync(string name, List<CreateUserDto> newUserList, List<DeptUserDto> dbUserList)
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