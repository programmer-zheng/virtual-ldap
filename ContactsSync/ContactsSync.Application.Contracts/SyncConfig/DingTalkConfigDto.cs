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
    public string AppKey { get; set; }

    [Required]
    public required string AppSecret { get; set; }

    public string AesKey { get; set; }
    
    public string Token { get; set; }

    public string ProcessCode { get; set; }
}