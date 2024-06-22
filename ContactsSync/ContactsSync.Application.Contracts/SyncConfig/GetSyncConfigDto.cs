using ContactsSync.Domain.Shared;

namespace ContactsSync.Application.Contracts.SyncConfig;

public class GetSyncConfigDto
{
    /// <summary>
    /// 提供程序
    /// </summary>
    public OpenPlatformProviderEnum ProviderName { get; set; }

    /// <summary>
    /// 同步周期
    /// </summary>
    public int SyncPeriod { get; set; }
    
    public SyncConfigBase ProviderConfig { get; set; }
}

public abstract class SyncConfigBase
{
    
}