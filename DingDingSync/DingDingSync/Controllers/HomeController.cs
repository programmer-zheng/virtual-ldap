using Abp.AspNetCore.Mvc.Controllers;
using Abp.Extensions;
using Abp.Runtime.Validation;
using Abp.UI;
using DingDingSync.Application;
using DingDingSync.Application.AppService;
using DingDingSync.Application.AppService.Dtos;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Application.IKuai.Dtos;
using DingDingSync.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;

namespace DingDingSync.Web.Controllers
{
    public class HomeController : AbpController
    {
        public ILogger _logger { get; set; }


        public IDingdingAppService _dingdingAppService { get; set; }

        public IUserAppService _userAppService { get; set; }

        public IIkuaiAppService _ikuaiAppService { get; set; }

        private readonly DingDingConfigOptions _dingDingConfigOptions;

        public HomeController(IOptions<DingDingConfigOptions> options)
        {
            _dingDingConfigOptions = options.Value;
        }


        [HttpGet]
        public IActionResult Index()
        {
            string userid = Request.Cookies[$"{_dingDingConfigOptions.CorpId}_UserId"];
            if (!string.IsNullOrWhiteSpace(userid))
            {
                return RedirectToAction("Manage");
            }

            ViewBag.CorpId = _dingDingConfigOptions.CorpId;
            return View();
        }


        [Route("/UserDetail")]
        public async Task<IActionResult> UserDetail(string userid)
        {
            var user = await _userAppService.GetDeptUserDetail(userid);
            return Json(user);
        }

        [Route("/ManageUsers")]
        public async Task<IActionResult> ManageUsers(string userid)
        {
            var users = await _userAppService.GetAdminDeptUsers(userid);
            return PartialView(users);
        }


        [HttpGet]
        [Route("/manage")]
        public async Task<IActionResult> Manage(string code)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                var dingdingUser = _dingdingAppService.GetUserinfoByCode(code);
                if (dingdingUser == null)
                {
                    return Content("获取钉钉人员信息失败，请关闭应用重新打开!");
                }

                Response.Cookies.Append($"{_dingDingConfigOptions.CorpId}_UserId", dingdingUser.Userid,
                    new Microsoft.AspNetCore.Http.CookieOptions
                        {HttpOnly = true, Expires = DateTimeOffset.Now.AddDays(7)});
            }

            //todo 从cookie中获取userid，若userid获取不到，跳转至授权页面，获取授权码，根据授权码获取用户id，再跳转回来
            string userid = Request.Cookies[$"{_dingDingConfigOptions.CorpId}_UserId"];
            if (string.IsNullOrWhiteSpace(userid))
            {
                return RedirectToAction("Index");
            }

            UserSimpleDto user = null;
            //根据用户id，查询数据库，获取人员信息，返回ldap用户名、姓名等信息
            var userinfo = await _userAppService.GetByIdAsync(userid);
            if (userinfo == null)
            {
                ViewBag.Msg = "域账号未开通，请联系管理员开通！";
            }
            else
            {
                user = new UserSimpleDto
                {
                    UserId = userinfo.Id,
                    Name = userinfo.Name,
                    UserName = userinfo.UserName,
                    IsAdmin = userinfo.IsAdmin,
                    EnableAccount = userinfo.AccountEnabled,
                    EnableVpnAccount = userinfo.VpnAccountEnabled
                };
            }

            return View(user);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [Route("/EnableAccount")]
        public async Task<IActionResult> EnableAccount(string userid, string username)
        {
            var userinfo = await _userAppService.GetByIdAsync(userid);
            if (userinfo != null && userinfo.AccountEnabled)
            {
                return Json(new {success = false, Msg = $"{userinfo.Name} 账号已启用，无须重复操作！"});
            }

            var result = await _userAppService.EnableAccount(userid, username);
            return Json(new {success = result, Msg = result ? "启用账号成功！" : "启用账号失败！"});
        }


        [Route("/EnableVpnAccount")]
        public async Task<IActionResult> EnableVpnAccount(string userid)
        {
            var result = false;
            try
            {
                var userinfo = await _userAppService.GetByIdAsync(userid);
                if (userinfo != null)
                {
                    if (!userinfo.AccountEnabled)
                    {
                        throw new UserFriendlyException($"启用VPN账号失败，请先为 {userinfo.Name} 开通域账号！");
                    }

                    if (userinfo.VpnAccountEnabled)
                    {
                        throw new UserFriendlyException($"{userinfo.Name} 的VPN账号已启用，无须重复操作；若无法使用VPN账号，请联系管理员！");
                    }

                    result = await _userAppService.EnableVpnAccount(userid);
                    if (result)
                    {
                        //先去爱快路由器中操作，若操作失败，则不会更新数据库中字段
                        var ikuaiAccount = _ikuaiAppService.GetAccountIdByUsername(userinfo.UserName);
                        if (ikuaiAccount == null)
                        {
                            var pwd = userinfo.Password.DesDecrypt();
                            _ikuaiAppService.CreateAccount(new AccountCommon(userinfo.UserName,
                                pwd, userinfo.Name));
                        }
                    }
                }
            }
            catch (IKuaiException e)
            {
                _logger.Error($"启用VPN账号{userid}时，调用爱快接口发生异常", e);
                return Json(new {success = false, Msg = e.Message});
            }
            catch (Exception e)
            {
                _logger.Error($"启用VPN账号{userid}失败，发生异常", e);
                return Json(new {success = false, Msg = e.Message});
            }

            return Json(new {success = result, Msg = result ? "启用VPN账号成功" : "启用VPN账号失败"});
        }

        //管理员为员工重置密码
        [Route("/ResetAccountPassword")]
        public async Task<IActionResult> ResetAccountPassword(string userid)
        {
            var userinfo = await _userAppService.GetByIdAsync(userid);
            if (userinfo == null)
            {
                return Json(new {success = false, Msg = "重置密码失败，不存在该员工！"});
            }

            var result = await _userAppService.ResetAccountPassword(userid);
            if (result && userinfo.VpnAccountEnabled)
            {
                var ikuaiAccount = _ikuaiAppService.GetAccountIdByUsername(userinfo.UserName);
                if (ikuaiAccount != null)
                {
                    _ikuaiAppService.RemoveAccount(ikuaiAccount.id);
                }
            }

            return Json(new {success = result, Msg = result ? $"为 {userinfo.Name} 重置密码成功！" : "重置密码失败！"});
        }
    }
}