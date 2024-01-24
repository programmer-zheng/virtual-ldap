using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ContactsSync.Application.Contracts.SyncConfig;
using ContactsSync.Domain.Shared;

namespace ContactsSync.Web.Models;

public class SyncConfigSettingViewModel
{
    /// <summary>
    /// 提供程序
    /// </summary>
    [DisplayName("开放平台")]
    public OpenPlatformProviderEnum ProviderName { get; set; }

    /// <summary>
    /// 同步周期
    /// </summary>
    [DisplayName("同步周期（单位分钟）")]
    [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "同步周期必须是正整数")]
    public int SyncPeriod { get; set; }

    public DingTalkConfigDto DingTalk { get; set; }

    public WeWorkConfigDto WeWork { get; set; }
}