using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Abp.UI;
using Castle.Core.Logging;
using DingDingSync.Application;
using DingDingSync.Application.AppService;
using DingDingSync.Application.AppService.Dtos;
using DingDingSync.Application.Jobs;
using DingDingSync.Core;
using DingDingSync.Core.Entities;
using DingDingSync.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DingDingSync.Web.Controllers
{
    public class HomeController : AbpController
    {
        public IConfiguration Configuration { get; set; }

        public IUserAppService UserAppService { get; set; }

        public IBackgroundJobManager BackgroundJobManager { get; set; }
        
        public ICommonProvider CommonProvider { get; set; }

        private readonly IRepository<UserEntity, string> _userRepository;

        public HomeController(IRepository<UserEntity, string> userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpGet]
        public IActionResult Index()
        {
            // 获取配置文件中当前工作环境（钉钉/企业微信）
            var workEnv = Configuration["WorkEnv"];
            if (workEnv.Equals("DingDing", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "DingDing");
            }
            else if (workEnv.Equals("WorkWeixin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "WorkWeixin");
            }

            return NotFound();
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> Test()
        {
            var userid = _userRepository.GetAll().FirstOrDefault().Id;
            await CommonProvider.SendTextMessage(userid, "测试消息发送~");
            return Json(null);
        }


        [Route("/UserDetail")]
        public async Task<IActionResult> UserDetail(string userid)
        {
            var user = await UserAppService.GetDeptUserDetail(userid);
            return Json(user);
        }

        [Route("/ManageUsers")]
        public async Task<IActionResult> ManageUsers(string userid)
        {
            var users = await UserAppService.GetAdminDeptUsers(userid);
            return PartialView(users);
        }


        [HttpGet]
        [Route("/manage")]
        public async Task<IActionResult> Manage(string code)
        {
            // 从cookie中获取userid，若userid获取不到，跳转至授权页面，获取授权码，根据授权码获取用户id，再跳转回来
            string userid = Request.Cookies[LdapConsts.CookieName];
            if (string.IsNullOrWhiteSpace(userid))
            {
                return RedirectToAction("Index");
            }

            UserSimpleDto user = null;
            // 根据用户id，查询数据库，获取人员信息，返回ldap用户名、姓名等信息
            var userinfo = await UserAppService.GetByIdAsync(userid);
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
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Route("/EnableAccount")]
        public async Task<IActionResult> EnableAccount(string userid, string username)
        {
            var userinfo = await UserAppService.GetByIdAsync(userid);
            if (userinfo != null && userinfo.AccountEnabled)
            {
                return Json(new { success = false, Msg = $"{userinfo.Name} 账号已启用，无须重复操作！" });
            }

            var result = await UserAppService.EnableAccount(userid, username);
            return Json(new { success = result, Msg = result ? "启用账号成功！" : "启用账号失败！" });
        }


        [Route("/EnableVpnAccount")]
        public async Task<IActionResult> EnableVpnAccount(string userid)
        {
            var result = false;
            try
            {
                var userinfo = await UserAppService.GetByIdAsync(userid);
                if (userinfo != null)
                {
                    if (!userinfo.AccountEnabled)
                    {
                        throw new UserFriendlyException($"启用VPN账号失败，请先为 {userinfo.Name} 开通域账号！");
                    }

                    if (!userinfo.PasswordInited)
                    {
                        throw new UserFriendlyException($"{userinfo.Name} 还未修改初始密码，无法启用VPN账号，请先修改默认密码！");
                    }

                    if (userinfo.VpnAccountEnabled)
                    {
                        throw new UserFriendlyException($"{userinfo.Name} 的VPN账号已启用，无须重复操作；若无法使用VPN账号，请联系管理员！");
                    }

                    result = await UserAppService.EnableVpnAccount(userid);
                    if (result)
                    {
                        await BackgroundJobManager.EnqueueAsync<IKuaiSyncAccountBackgroundJob, string>(userid);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"启用VPN账号{userid}失败，发生异常", e);
                return Json(new { success = false, Msg = e.Message });
            }

            return Json(new { success = result, Msg = result ? "启用VPN账号成功" : "启用VPN账号失败" });
        }

        //管理员为员工重置密码
        [Route("/ResetAccountPassword")]
        public async Task<IActionResult> ResetAccountPassword(string userid)
        {
            var userinfo = await UserAppService.GetByIdAsync(userid);
            if (userinfo == null)
            {
                return Json(new { success = false, Msg = "重置密码失败，不存在该员工！" });
            }

            var result = await UserAppService.ResetAccountPassword(userid);
            if (result && userinfo.VpnAccountEnabled)
            {
                await BackgroundJobManager.EnqueueAsync<IKuaiSyncAccountBackgroundJob, string>(userid);
            }

            return Json(new { success = result, Msg = result ? $"为 {userinfo.Name} 重置密码成功！" : "重置密码失败！" });
        }
    }
}