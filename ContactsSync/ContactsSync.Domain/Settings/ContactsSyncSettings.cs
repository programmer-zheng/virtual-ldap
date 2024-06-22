namespace ContactsSync.Domain.Settings;

public static class ContactsSyncSettings
{
    private const string Prefix = "ContactsSync";

    /// <summary>
    /// 同步提供程序
    /// </summary>
    public const string ProviderName = Prefix + ".ProviderName";

    /// <summary>
    /// 同步周期（单位分钟）
    /// </summary>
    public const string SyncPeriod = Prefix + ".SyncPeriod";

    /// <summary>
    /// 平台配置
    /// </summary>
    public const string PlatformConfig = Prefix + ".PlatformConfig";
}