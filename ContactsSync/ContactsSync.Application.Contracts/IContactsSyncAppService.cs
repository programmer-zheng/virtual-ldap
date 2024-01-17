using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace ContactsSync.Application.Contracts;

public interface IContactsSyncAppService : ITransientDependency
{
    /// <summary>
    ///     同步部门和成员
    /// </summary>
    /// <returns></returns>
    [RemoteService(false)]
    Task SyncDepartmentAndUser();
}