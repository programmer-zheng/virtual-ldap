using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace ContactsSync.Application.AppServices;

public interface IContactsSyncAppService : ITransientDependency
{
    /// <summary>
    /// 同步部门和成员
    /// </summary>
    /// <returns></returns>
    [RemoteService(isEnabled: false)]
    Task SyncDepartmentAndUser();
}