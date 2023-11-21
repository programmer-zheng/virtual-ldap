using System.ComponentModel.DataAnnotations;

namespace ContactsSync.Application.WeWork;

public class WeWorkConfigOptions
{
    public const string WeWork = "WeWork";

    /// <summary>
    /// CorpId
    /// </summary>
    [Required(ErrorMessage = "企业微信配置缺失，未在配置中发现{0}")]
    public string CorpId { get; set; }

    /// <summary>
    /// AgentId
    /// </summary>
    [Required(ErrorMessage = "企业微信配置缺失，未在配置中发现{0}")]
    [RegularExpression(@"[1-9]\d+", ErrorMessage = "请正确填写AgentId")]
    public long AgentId { get; set; }

    /// <summary>
    /// AppSecret
    /// </summary>
    [Required(ErrorMessage = "企业微信配置缺失，未在配置中发现{0}")]
    public string AppSecret { get; set; }

    /// <summary>
    /// EncodingAESKey
    /// </summary>
    [Required(ErrorMessage = "企业微信配置缺失，未在配置中发现{0}")]
    public string EncodingAESKey { get; set; }

    /// <summary>
    /// token
    /// </summary>
    [Required(ErrorMessage = "企业微信配置缺失，未在配置中发现{0}")]
    public string Token { get; set; }

    /// <summary>
    /// 企业微信审批模板ID
    /// </summary>
    public string TemplateId { get; set; }
}