using ContactsSync.Application.AppServices;
using ContactsSync.Application.OpenPlatformProvider;
using ContactsSync.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Threading;
using Volo.Abp.Uow;

namespace ContactsSync.Application.Background;

public class ContactsSyncWorker : AsyncPeriodicBackgroundWorkerBase
{
    public ContactsSyncWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory, IOptions<ContactsSyncConfigOptions> options) : base(timer, serviceScopeFactory)
    {
        // 通讯录同步工人定时执行周期，单位（秒），15分钟一次

        timer.Period = 1000 * 60 * options.Value.SyncPeriod;
#if DEBUG
        timer.Period = 1000 * 15;
#endif
    }

    [UnitOfWork]
    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        var openPlatformProvider = ServiceProvider.GetRequiredService<IOpenPlatformProvider>();
        var departmentAppService = ServiceProvider.GetRequiredService<IDepartmentAppService>();
        var userAppService = ServiceProvider.GetRequiredService<IUserAppService>();
        var objectMapper = ServiceProvider.GetRequiredService<IObjectMapper>();
        var platformDepartments = await openPlatformProvider.GetDepartmentListAsync();
        if (platformDepartments?.Count > 0)
        {
            // 数据库中的数据
            var existsDepartmentList = await departmentAppService.GetAllDepartments();
            var existsUserList = await userAppService.GetAllUsersAsync();
            var existsRelaList = await userAppService.GetAllDeptUserRelaAsync();

            // 需要新增的数据
            var deptList = new List<DepartmentEntity>();
            var userList = new List<UserEntity>();
            var relaList = new List<UserDepartmentsRelationEntity>();
            foreach (var departmentDto in platformDepartments)
            {
                // 映射部门
                var departmentEntity = objectMapper.Map<PlatformDepartmentDto, DepartmentEntity>(departmentDto);
                if (existsDepartmentList.All(t => t.OriginId != departmentEntity.OriginId))
                {
                    departmentEntity.Source = openPlatformProvider.Source;
                    deptList.Add(departmentEntity);
                }

                var platformDeptUsers = await openPlatformProvider.GetDeptUserListAsync(departmentDto.DepartmentId);
                foreach (var deptUserDto in platformDeptUsers)
                {
                    // 映射人员
                    var userEntity = objectMapper.Map<PlatformDeptUserDto, UserEntity>(deptUserDto);
                    if (existsUserList.All(t => t.UserId != deptUserDto.UserId) && userList.All(t => t.UserId != deptUserDto.UserId))
                    {
                        userEntity.Source = openPlatformProvider.Source;
                        userList.Add(userEntity);
                    }

                    if (!existsRelaList.Any(t => t.UserId == deptUserDto.UserId && t.OriginDeptId == departmentDto.DepartmentId))
                    {
                        relaList.Add(new UserDepartmentsRelationEntity
                        {
                            UserId = deptUserDto.UserId,
                            OriginDeptId = departmentDto.DepartmentId,
                            Source = openPlatformProvider.Source
                        });
                    }
                }
            }

            // 批量添加部门
            await departmentAppService.BatchAddDepartmentAsync(deptList.ToArray());
            // 批量添加人员
            await userAppService.BatchAddUserAsync(userList.ToArray());
            // 批量添加部门人员关系
            await userAppService.BatchAddDeptUserRelaAsync(relaList.ToArray());
        }
    }
}