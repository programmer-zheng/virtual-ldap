using Abp.AspNetCore.Mvc.Controllers;
using Abp.BackgroundJobs;
using Abp.Extensions;
using Abp.Runtime.Validation;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.AppService.Dtos;
using VirtualLdap.Application.DingDingUtils;
using VirtualLdap.Application.Jobs;
using VirtualLdap.Core;

namespace VirtualLdap.Web.Controllers;

public class StaffController : AbpController
{
    public IUserAppService UserAppService { get; set; }

    public IBackgroundJobManager BackgroundJobManager { get; set; }

    public IDingTalkAppService DingTalkAppService { get; set; }

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

                string userid = Request.Cookies[LdapConsts.CookieName];
                if (userid != input.UserId)
                {
                    throw new UserFriendlyException("不能对他人账号进行忘记密码操作！");
                }

                var flag = await UserAppService.ResetPasswordAsync(input);
                if (flag)
                {
                    await BackgroundJobManager.EnqueueAsync<IKuaiSyncAccountBackgroundJob, string>(input.UserId);
                }

                return Json(new { Success = flag, Msg = $"密码修改{(flag ? "成功" : "失败")}！" });
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Msg = e.Message });
            }
        }

        var errorReason = ModelState.Values.SelectMany(t => t.Errors).Select(t => t.ErrorMessage).FirstOrDefault();
        return Json(new { Success = false, Msg = $"密码修改失败,{errorReason}" });
    }

    [Route("/ForgotPassword")]
    [HttpGet]
    public async Task<IActionResult> ForgotPassword()
    {
        string userid = Request.Cookies[LdapConsts.CookieName];

        return View(new ForgotPasswordViewModel() { UserId = userid });
    }

    [Route("/ForgotPassword")]
    [HttpPost]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errorReason = ModelState.Values.SelectMany(t => t.Errors).Select(t => t.ErrorMessage).FirstOrDefault();
            return Json(new { Success = false, Msg = $"密码修改失败,{errorReason}" });
        }

        string userid = Request.Cookies[LdapConsts.CookieName];
        if (userid != model.UserId)
        {
            throw new UserFriendlyException("不能对他人账号进行忘记密码操作！");
        }

        try
        {
            var userinfo = await UserAppService.GetByIdAsync(model.UserId);
            if (userinfo == null)
            {
                throw new UserFriendlyException("用户不存在");
            }

            var flag = await UserAppService.ForgotPasswordAsync(model);
            if (flag && userinfo.VpnAccountEnabled)
            {
                await BackgroundJobManager.EnqueueAsync<IKuaiSyncAccountBackgroundJob, string>(model.UserId);
            }

            return Json(new { Success = flag, Msg = $"密码修改{(flag ? "成功" : "失败")}！" });
        }
        catch (Exception e)
        {
            return Json(new { Success = false, Msg = e.Message });
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
        string userid = Request.Cookies[LdapConsts.CookieName];

        if (userid.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("请从工作台中点击应用操作！");
        }

        await UserAppService.SendVerificationCodeAsync(userid);
        return Json(new { Msg = "验证码已发送，请注意查收。" });
    }

    [Route("/apply")]
    [HttpGet]
    public async Task<IActionResult> ApplyAccount(string userId, long deptId)
    {
        await DingTalkAppService.CreateApproval(userId, deptId, "");
        return Json(null);
    }
}