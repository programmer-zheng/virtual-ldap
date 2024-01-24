using Volo.Abp.Settings;

namespace ContactsSync.Domain.Settings;

public class ContactsSyncSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
            new SettingDefinition(ContactsSyncSettings.ProviderName),
            new SettingDefinition(ContactsSyncSettings.SyncPeriod, "30"),
            new SettingDefinition(ContactsSyncSettings.PlatformConfig)
        );
    }
}