using System.ComponentModel.DataAnnotations;

namespace ContactsSync.Application.Contracts.SyncConfig;

public class ProviderConfig
{
    [Required]
    public required string CorpId { get; set; }

    [RegularExpression(@"^[1-9]\d+$")]
    [Required]
    public required string AgentId { get; set; }

    [Required]
    public required string AppSecret { get; set; }

    #region DingTalk

    public string AesKey { get; set; }

    public string AppKey { get; set; }

    public string? ProcessCode { get; set; }

    #endregion

    #region WeWork

    public string Token { get; set; }

    public string EncodingAesKey { get; set; }

    public string? TemplateId { get; set; }

    #endregion
}