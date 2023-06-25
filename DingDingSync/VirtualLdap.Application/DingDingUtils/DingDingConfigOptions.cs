namespace VirtualLdap.Application.DingDingUtils
{
    public class DingDingConfigOptions
    {
        public const string DingDing = "DingDing";

        /// <summary>
        /// CorpId
        /// </summary>
        public string CorpId { get; set; }

        /// <summary>
        /// AgentId
        /// </summary>
        public long AgentId { get; set; }

        /// <summary>
        /// AppKey
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// AppSecret
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 加密 aes_key
        /// </summary>
        public string Aes_Key { get; set; }

        /// <summary>
        /// 签名 token
        /// </summary>
        public string Token { get; set; }
    }
}