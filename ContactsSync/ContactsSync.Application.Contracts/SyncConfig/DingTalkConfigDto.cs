using System.ComponentModel.DataAnnotations;

namespace ContactsSync.Application.Contracts.SyncConfig;

public class DingTalkConfigDto : SyncConfigBase
{
    [Required]
    public required string CorpId { get; set; }

    [RegularExpression(@"^[1-9]\d+$")]
    [Required]
    public required string AgentId { get; set; }

    [Required]
    public required string AppKey { get; set; }

    [Required]
    public required string AppSecret { get; set; }

    [Required]
    public required string AesKey { get; set; }

    public string? ProcessCode { get; set; }
}