﻿using System.Diagnostics;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.BackgroundJobs;
using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using VirtualLdap.Application;
using VirtualLdap.Application.AppService;
using VirtualLdap.Application.AppService.Dtos;
using VirtualLdap.Application.Jobs;
using VirtualLdap.Core;
using VirtualLdap.Core.Entities;
using VirtualLdap.Web.Models;

namespace VirtualLdap.Web.Controllers
{
    public class HomeController : AbpController
    {
        public IConfiguration Configuration { get; set; }

        public IUserAppService UserAppService { get; set; }

        public IBackgroundJobManager BackgroundJobManager { get; set; }

        public IMessageProvider MessageProvider { get; set; }

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
            await MessageProvider.SendTextMessageAsync(userid, "测试消息发送~");
            return Json(null);
        }


        [Route("/UserDetail")]
        public async Task<IActionResult> UserDetail(string userid)
        {
            var user = await UserAppService.GetDeptUserDetailAsync(userid);
            return Json(user);
        }

        [Route("/ManageUsers")]
        public IActionResult ManageUsers(string userid)
        {
            var users = UserAppService.GetAdminDeptUsers(userid);
            return PartialView(users);
        }


        [HttpGet]
        [Route("/manage")]
        public async Task<IActionResult> Manage(string code)
        {
            // 从cookie中获取userid，若userid获取不到，跳转至授权页面，获取授权码，根据授权码获取用户id，再跳转回来
            string userid = Request.Cookies[LdapConsts.CookieName];
#if DEBUG
            userid = "manager4723";
#endif
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

            var result = await UserAppService.EnableAccountAsync(userid, username);
            return Json(new { success = result, Msg = result ? "启用账号成功！" : "启用账号失败！" });
        }


        [Route("/EnableVpnAccount")]
        public async Task<IActionResult> EnableVpnAccount(string userid)
        {
            bool result;
            try
            {
                result = await UserAppService.EnableVpnAccountAsync(userid);
                if (result)
                {
                    await BackgroundJobManager.EnqueueAsync<IKuaiSyncAccountBackgroundJob, string>(userid);
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

            var result = await UserAppService.ResetAccountPasswordAsync(userid);
            if (result && userinfo.VpnAccountEnabled)
            {
                await BackgroundJobManager.EnqueueAsync<IKuaiSyncAccountBackgroundJob, string>(userid);
            }

            return Json(new { success = result, Msg = result ? $"为 {userinfo.Name} 重置密码成功！" : "重置密码失败！" });
        }
    }
}