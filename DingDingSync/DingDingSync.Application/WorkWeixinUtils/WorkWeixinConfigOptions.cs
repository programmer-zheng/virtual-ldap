namespace DingDingSync.Application.WorkWeixinUtils;

public class WorkWeixinConfigOptions
{
    public const string WorkWeixin = "WorkWeixin";

    /// <summary>
    /// CorpId
    /// </summary>
    public string CorpId { get; set; }

    /// <summary>
    /// AgentId
    /// </summary>
    public long AgentId { get; set; }

    /// <summary>
    /// AppSecret
    /// </summary>
    public string AppSecret { get; set; }

    /// <summary>
    /// EncodingAESKey
    /// </summary>
    public string EncodingAESKey { get; set; }

    /// <summary>
    /// token
    /// </summary>
    public string Token { get; set; }
}