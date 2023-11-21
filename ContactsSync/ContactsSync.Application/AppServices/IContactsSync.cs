using Volo.Abp;
using Volo.Abp.Application.Services;

namespace ContactsSync.Application.AppServices;

public interface IContactsSync : IApplicationService
{
    /// <summary>
    /// 同步部门和成员
    /// </summary>
    /// <returns></returns>
    [RemoteService(isEnabled: false)]
    Task SyncDepartmentAndUser();
}