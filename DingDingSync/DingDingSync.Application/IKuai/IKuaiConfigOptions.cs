namespace DingDingSync.Application.IKuai
{
    public class IKuaiConfigOptions
    {
        public const string IKuai = "Ikuai";

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


        public string BasePath { get; set; }
    }
}