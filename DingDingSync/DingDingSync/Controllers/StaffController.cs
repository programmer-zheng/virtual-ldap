using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Extensions;
using Abp.Runtime.Validation;
using Abp.UI;
using Castle.Core.Logging;
using DingDingSync.Application;
using DingDingSync.Application.AppService;
using DingDingSync.Application.AppService.Dtos;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Application.IKuai.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DingDingSync.Web.Controllers;

public class StaffController : AbpController
{
    #region 构造函数及属性

    public ILogger _logger { get; set; }

    public IUserAppService _userAppService { get; set; }

    public IIkuaiAppService _ikuaiAppService { get; set; }

    private readonly DingDingConfigOptions _dingDingConfigOptions;

    public StaffController(IOptions<DingDingConfigOptions> options)
    {
        _dingDingConfigOptions = options.Value;
    }

    #endregion

    /// <summary>
    /// 用户修改密码
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="UserFriendlyException"></exception>
    [HttpPost]
    [Route("/ChangePassword")]
    [DisableValidation]
    public async Task<IActionResult> ChangePassword([FromBody] ResetPasswordViewModel input)
    {
        if (ModelState.IsValid)
        {
            try
            {
                if (input.OldPassword.Equals(input.NewPassword))
                {
                    throw new ArgumentException("密码修改失败，新密码不能与旧密码一致!");
                }

                var userinfo = await _userAppService.GetByIdAsync(input.UserId);
                if (userinfo == null)
                {
                    throw new UserFriendlyException("用户不存在");
                }

                if (userinfo.Password != input.OldPassword.DesEncrypt())
                {
                    throw new UserFriendlyException("当前密码不正确，无法修改密码！");
                }

                var flag = await _userAppService.ResetPassword(input);
                if (flag && userinfo.VpnAccountEnabled)
                {
                    //若密码修改成功且启用vpn账号，vpn账号的密码同步修改
                    var ikuaiAccount = _ikuaiAppService.GetAccountIdByUsername(userinfo.UserName);
                    if (ikuaiAccount == null)
                    {
                        //若账号为首次开通后修改初始密码，则新建vpn账号
                        _ikuaiAppService.CreateAccount(new AccountCommon(userinfo.UserName,
                            input.NewPassword, userinfo.Name));
                    }
                    else
                    {
                        if (ikuaiAccount.share <= 1)
                        {
                            ikuaiAccount.share = 2;
                        }

                        //反之直接修改密码，并启用
                        ikuaiAccount.enabled = "yes";
                        ikuaiAccount.passwd = input.NewPassword;
                        _ikuaiAppService.UpdateAccount(ikuaiAccount);
                    }
                }

                return Json(new {Success = flag, Msg = $"密码修改{(flag ? "成功" : "失败")}！"});
            }
            catch (Exception e)
            {
                return Json(new {Success = false, Msg = e.Message});
            }
        }

        var errorReason = ModelState.Values.SelectMany(t => t.Errors).Select(t => t.ErrorMessage).FirstOrDefault();
        return Json(new {Success = false, Msg = $"密码修改失败,{errorReason}"});
    }

    [Route("/ForgotPassword")]
    [HttpGet]
    public async Task<IActionResult> ForgotPassword()
    {
        string userid = Request.Cookies[$"{_dingDingConfigOptions.CorpId}_UserId"];

        return View(new ForgotPasswordViewModel() {UserId = userid});
    }

    [Route("/ForgotPassword")]
    [HttpPost]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errorReason = ModelState.Values.SelectMany(t => t.Errors).Select(t => t.ErrorMessage).FirstOrDefault();
            return Json(new {Success = false, Msg = $"密码修改失败,{errorReason}"});
        }

        string userid = Request.Cookies[$"{_dingDingConfigOptions.CorpId}_UserId"];
        if (userid != model.UserId)
        {
            throw new UserFriendlyException("不能对他人账号进行忘记密码操作！");
        }

        try
        {
            var userinfo = await _userAppService.GetByIdAsync(model.UserId);
            if (userinfo == null)
            {
                throw new UserFriendlyException("用户不存在");
            }

            var flag = await _userAppService.ForgotPassword(model);
            if (flag && userinfo.VpnAccountEnabled)
            {
                //若密码修改成功且启用vpn账号，vpn账号的密码同步修改
                var ikuaiAccount = _ikuaiAppService.GetAccountIdByUsername(userinfo.UserName);
                if (ikuaiAccount == null)
                {
                    //若账号为首次开通后修改初始密码，则新建vpn账号
                    _ikuaiAppService.CreateAccount(new AccountCommon(userinfo.UserName,
                        model.NewPassword, userinfo.Name));
                }
                else
                {
                    if (ikuaiAccount.share <= 1)
                    {
                        ikuaiAccount.share = 2;
                    }

                    //反之直接修改密码，并启用
                    ikuaiAccount.enabled = "yes";
                    ikuaiAccount.passwd = model.NewPassword;
                    _ikuaiAppService.UpdateAccount(ikuaiAccount);
                }
            }

            return Json(new {Success = flag, Msg = $"密码修改{(flag ? "成功" : "失败")}！"});
        }
        catch (Exception e)
        {
            return Json(new {Success = false, Msg = e.Message});
        }
    }


    /// <summary>
    /// 发送验证码
    /// </summary>
    /// <returns></returns>
    [Route("/SendVerificationCode")]
    [HttpGet]
    public async Task<IActionResult> SendVerificationCode()
    {
        string userid = Request.Cookies[$"{_dingDingConfigOptions.CorpId}_UserId"];

        if (userid.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("请从钉钉中进行操作！");
        }

        await _userAppService.SendVerificationCode(userid);
        return Json(new {Msg = "验证码已发送至钉钉，请注意查收。"});
    }
}