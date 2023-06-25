namespace VirtualLdap.Application.IKuai
{
    public class IKuaiConfigOptions
    {
        public const string IKuai = "Ikuai";
        
        /// <summary>
        /// 是否启用爱快VPN功能
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// Gwid
        /// </summary>
        public string Gwid { get; set; }

        /// <summary>
        /// Open_Rsa_Pubkey
        /// </summary>
        public string Open_Rsa_Pubkey { get; set; }


        /// <summary>
        /// 爱快开放平台基础路径
        /// </summary>
        public string BasePath { get; set; } = "https://open.ikuai8.com/api/v3";
    }
}