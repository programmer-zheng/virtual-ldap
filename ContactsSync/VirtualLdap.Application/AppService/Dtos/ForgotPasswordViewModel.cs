using System.ComponentModel.DataAnnotations;

namespace VirtualLdap.Application.AppService.Dtos;

public class ForgotPasswordViewModel
{
    [MaxLength(50, ErrorMessage = "{0}长度超过限制")]
    [Required(ErrorMessage = "UserId不能为空")]
    public string UserId { get; set; }

    [MaxLength(6, ErrorMessage = "{0}长度超过限制")]
    [Required(ErrorMessage = "请输入验证码")]
    public string VerificationCode { get; set; }

    [Required(ErrorMessage = "新密码不能为空")]
    [MinLength(8, ErrorMessage = "密码不得少于8位")]
    [MaxLength(16, ErrorMessage = "{0}长度超过限制，最长16位")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#^*()\-_\.\[\]:])[A-Za-z\d!@#^*()\-_\.\[\]:]{8,16}$",
        ErrorMessage = "密码必须同时包含大小写英文字母、数字、特殊字符（!@#^*()-_.[]:）")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "确认密码不能为空")]
    [Compare("NewPassword", ErrorMessage = "两次密码不一致")]
    public string ConfirmPassword { get; set; }
}