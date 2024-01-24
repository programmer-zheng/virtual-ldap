using ContactsSync.Application.Contracts.SyncConfig;
using ContactsSync.Domain.Settings;
using ContactsSync.Domain.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Volo.Abp.Application.Services;
using Volo.Abp.SettingManagement;

namespace ContactsSync.Application.AppServices;

[IgnoreAntiforgeryToken]
public class SyncConfigAppService : ApplicationService, ISyncConfigAppService
{
    private readonly ISettingManager _settingManager;

    public SyncConfigAppService(ISettingManager settingManager)
    {
        _settingManager = settingManager;
    }

    public async Task SaveSyncConfig(UpdateContactsSyncConfigDto dto)
    {
        Logger.LogInformation(JsonConvert.SerializeObject(dto));
        await _settingManager.SetGlobalAsync(ContactsSyncSettings.ProviderName, dto.ProviderName.ToString());
        await _settingManager.SetGlobalAsync(ContactsSyncSettings.SyncPeriod, dto.SyncPeriod.ToString());
        await _settingManager.SetGlobalAsync(ContactsSyncSettings.PlatformConfig, JsonConvert.SerializeObject(dto.ProviderConfig));
    }

    public async Task<GetSyncConfigDto> GetSyncConfig()
    {
        var configs = await _settingManager.GetAllGlobalAsync();
        var providerNameValue = configs.FirstOrDefault(t => t.Name == ContactsSyncSettings.ProviderName)?.Value;
        var syncPeriodValue = configs.FirstOrDefault(t => t.Name == ContactsSyncSettings.SyncPeriod)?.Value;
        var providerConfigValue = configs.FirstOrDefault(t => t.Name == ContactsSyncSettings.PlatformConfig)?.Value;
        if (providerNameValue.IsNullOrWhiteSpace())
        {
            return null;
        }


        var providerNameEnum = Enum.Parse<OpenPlatformProviderEnum>(providerNameValue);

        var result = new GetSyncConfigDto() { ProviderName = providerNameEnum, SyncPeriod = Convert.ToInt32(syncPeriodValue) };

        SyncConfigBase config = null;
        if (providerNameEnum == OpenPlatformProviderEnum.DingDing)
        {
            config = JsonConvert.DeserializeObject<DingTalkConfigDto>(providerConfigValue);
        }
        else if (providerNameEnum == OpenPlatformProviderEnum.WeWork)
        {
            config = JsonConvert.DeserializeObject<WeWorkConfigDto>(providerConfigValue);
        }

        result.ProviderConfig = config;
        return result;
    }
}
