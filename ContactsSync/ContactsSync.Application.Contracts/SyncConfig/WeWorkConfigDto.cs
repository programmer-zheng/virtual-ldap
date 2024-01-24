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

    [Required]
    public required string Token { get; set; }

    [Required]
    public required string EncodingAesKey { get; set; }

    public string? TemplateId { get; set; }
}