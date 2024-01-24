using System.ComponentModel.DataAnnotations;
using ContactsSync.Domain.Shared;

namespace ContactsSync.Application.Contracts.SyncConfig;

public class UpdateContactsSyncConfigDto : IValidatableObject
{
    /// <summary>
    /// 提供程序
    /// </summary>
    public OpenPlatformProviderEnum ProviderName { get; set; }

    /// <summary>
    /// 同步周期
    /// </summary>
    [RegularExpression(@"^[1-9]\d*$")]
    public int SyncPeriod { get; set; }

    public ProviderConfig ProviderConfig { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ProviderName == OpenPlatformProviderEnum.DingDing)
        {
            if (ProviderConfig.AesKey.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult($"{nameof(ProviderConfig.AesKey)}不能为空");
            }

            if (ProviderConfig.AppKey.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult($"{nameof(ProviderConfig.AppKey)}不能为空");
            }
        }
        else if (ProviderName == OpenPlatformProviderEnum.WeWork)
        {
            if (ProviderConfig.Token.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult($"{nameof(ProviderConfig.Token)}不能为空");
            }

            if (ProviderConfig.EncodingAesKey.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult($"{nameof(ProviderConfig.EncodingAesKey)}不能为空");
            }
        }
    }
}