using System.ComponentModel.DataAnnotations;

namespace DingDingSync.Application.AppService.Dtos
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
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "确认密码不能为空")]
        [MaxLength(16, ErrorMessage = "{0}长度超过限制")]
        [Compare("NewPassword", ErrorMessage = "两次密码不一致")]
        public string ConfirmPassword { get; set; }
    }
}
