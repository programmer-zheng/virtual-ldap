using System.ComponentModel.DataAnnotations;

namespace ContactsSync.Application.Background;

public class ContactsSyncConfigOptions
{
    public const string Sync = "Sync";

    /// <summary>
    /// 是否启用同步
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 同步周期，单位分钟，默认15
    /// </summary>
    [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "同步周期必须是大于0的整数")]
    public int SyncPeriod { get; set; } = 15;
}