using System.ComponentModel.DataAnnotations;

namespace ContactsSync.Application.DingDing;

public class DingDingConfigOptions
{
    public const string DingDing = "DingDing";

    /// <summary>
    /// CorpId
    /// </summary>
    [Required(ErrorMessage = "钉钉配置缺失，未在配置中发现{0}")]
    public string CorpId { get; set; }

    /// <summary>
    /// AgentId
    /// </summary>
    [Required(ErrorMessage = "钉钉配置缺失，未在配置中发现{0}")]
    [RegularExpression(@"[1-9]\d+", ErrorMessage = "请正确填写AgentId")]
    public long AgentId { get; set; }

    /// <summary>
    /// AppKey
    /// </summary>
    [Required(ErrorMessage = "钉钉配置缺失，未在配置中发现{0}")]
    public string AppKey { get; set; }

    /// <summary>
    /// AppSecret
    /// </summary>
    [Required(ErrorMessage = "钉钉配置缺失，未在配置中发现{0}")]
    public string AppSecret { get; set; }

    /// <summary>
    /// 加密 aes_key
    /// </summary>
    [Required(ErrorMessage = "钉钉配置缺失，未在配置中发现{0}")]
    public string Aes_Key { get; set; }

    /// <summary>
    /// 签名 token
    /// </summary>
    [Required(ErrorMessage = "钉钉配置缺失，未在配置中发现{0}")]
    public string Token { get; set; }
        
    /// <summary>
    /// 审批流的唯一码
    /// </summary>
    public string ProcessCode { get; set; }
}