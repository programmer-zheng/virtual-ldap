using ContactsSync.Application.Contracts.OpenPlatformProvider;
using Volo.Abp.Application.Services;

namespace ContactsSync.Application.Contracts.SyncConfig;

public interface ISyncConfigAppService : IApplicationService
{
    Task SaveSyncConfig(UpdateContactsSyncConfigDto dto);

    Task<GetSyncConfigDto> GetSyncConfig();

    Task<IOpenPlatformProviderApplicationService> GetServiceByConfigKey();

    Task<SyncConfigBase> GetConfigDetail();
}