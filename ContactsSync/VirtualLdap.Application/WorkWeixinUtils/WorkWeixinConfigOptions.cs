using System.ComponentModel.DataAnnotations;

namespace VirtualLdap.Application.WorkWeixinUtils;

public class WorkWeixinConfigOptions
{
    public const string WorkWeixin = "WorkWeiXin";

    /// <summary>
    /// CorpId
    /// </summary>
    [Required]
    public string CorpId { get; set; }

    /// <summary>
    /// AgentId
    /// </summary>
    [Required]
    [RegularExpression(@"[1-9]\d+", ErrorMessage = "请正确填写AgentId")]
    public long AgentId { get; set; }

    /// <summary>
    /// AppSecret
    /// </summary>
    [Required]
    public string AppSecret { get; set; }

    /// <summary>
    /// EncodingAESKey
    /// </summary>
    [Required]
    public string EncodingAESKey { get; set; }

    /// <summary>
    /// token
    /// </summary>
    [Required]
    public string Token { get; set; }
}