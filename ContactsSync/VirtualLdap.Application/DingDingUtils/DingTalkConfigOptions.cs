using System.ComponentModel.DataAnnotations;

namespace VirtualLdap.Application.DingDingUtils
{
    public class DingTalkConfigOptions
    {
        public const string DingDing = "DingDing";

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
        /// AppKey
        /// </summary>
        [Required]
        public string AppKey { get; set; }

        /// <summary>
        /// AppSecret
        /// </summary>
        [Required]
        public string AppSecret { get; set; }

        /// <summary>
        /// 加密 aes_key
        /// </summary>
        [Required]
        public string Aes_Key { get; set; }

        /// <summary>
        /// 签名 token
        /// </summary>
        [Required]
        public string Token { get; set; }
    }
}