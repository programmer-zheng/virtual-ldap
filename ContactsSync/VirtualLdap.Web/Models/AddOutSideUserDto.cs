using System.ComponentModel.DataAnnotations;

namespace VirtualLdap.Web.Models
{
    public class AddOutSideUserDto
    {
        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }


        /// <summary>
        /// 账号状态 ，true 启用
        /// </summary>
        public bool AccountStatus { get; set; }

        /// <summary>
        /// VPN 账号状态，true 启用
        /// </summary>
        public bool VpnStatus { get; set; }

    }
}
