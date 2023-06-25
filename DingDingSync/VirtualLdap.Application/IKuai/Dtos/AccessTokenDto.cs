namespace VirtualLdap.Application.IKuai.Dtos
{
    public class AccessTokenDto
    {
        public string access_token { get; set; }

        public int token_create_time { get; set; }

        public int token_excess_time { get; set; }

        /// <summary>
        /// 友好格式的 AccessToken 过期时间（以服务器时间为准）
        /// </summary>
        public string token_expire_at_server_time { get; set; }
    }
}
