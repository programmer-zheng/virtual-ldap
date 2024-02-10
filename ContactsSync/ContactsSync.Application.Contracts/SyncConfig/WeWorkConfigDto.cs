using System.ComponentModel.DataAnnotations;

namespace ContactsSync.Application.Contracts.SyncConfig;

public class WeWorkConfigDto : SyncConfigBase
{
    [Required]
    public required string CorpId { get; set; }

    [RegularExpression(@"^[1-9]\d+$")]
    [Required]
    public required string AgentId { get; set; }

    [Required]
    public required string AppSecret { get; set; }

    [Display(Description = "用于配置回调接口验证")]
    public string Token { get; set; }

    [Display(Description = "用于配置回调接口验证")]
    public string EncodingAesKey { get; set; }

    public string TemplateId { get; set; }
}