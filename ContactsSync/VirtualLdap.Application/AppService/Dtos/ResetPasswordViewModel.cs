using System.ComponentModel.DataAnnotations;

namespace VirtualLdap.Application.AppService.Dtos
{
    public class ResetPasswordViewModel
    {
        [MaxLength(50, ErrorMessage = "{0}长度超过限制")]
        [Required(ErrorMessage = "UserId不能为空")]
        public string UserId { get; set; }

        [MaxLength(16, ErrorMessage = "{0}长度超过限制")]
        [Required(ErrorMessage = "当前密码不能为空")]
        public string OldPassword { get; set; }

        [MaxLength(16, ErrorMessage = "{0}长度超过限制")]
        [Required(ErrorMessage = "新密码不能为空")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#^*()\-_\.\[\]:])[A-Za-z\d!@#^*()\-_\.\[\]:]{8,16}$",
            ErrorMessage = "密码必须同时包含大小写英文字母、数字、特殊字符（!@#^*()-_.[]:）")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "确认密码不能为空")]
        [MaxLength(16, ErrorMessage = "{0}长度超过限制")]
        [Compare("NewPassword", ErrorMessage = "两次密码不一致")]
        public string ConfirmPassword { get; set; }
    }
}
