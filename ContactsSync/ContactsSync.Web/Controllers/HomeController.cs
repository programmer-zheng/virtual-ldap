using System.Diagnostics;
using System.Web;
using ContactsSync.Application.AppServices;
using ContactsSync.Application.Background;
using ContactsSync.Application.Contracts;
using ContactsSync.Application.OpenPlatformProvider;
using ContactsSync.Domain.Shared;
using ContactsSync.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace ContactsSync.Web.Controllers
{
    public class HomeController : AbpController
    {
        private readonly ContactsSyncConfigOptions _contactsSyncConfigOptions;

        public HomeController(IOptionsSnapshot<ContactsSyncConfigOptions> syncConfig)
        {
            _contactsSyncConfigOptions = syncConfig.Value;
            // Provider = ServiceProvider.GetKeyedService<IOpenPlatformProvider>(_contactsSyncConfigOptions.OpenPlatformProvider.ToString());
        }

        public IConfiguration Configuration { get; set; }

        public IUserAppService UserAppService { get; set; }

        public async Task<IActionResult> Index()
        {
            // todo 待abp 8.1发布后，使用LazyServiceProvider替换
            var provider = ServiceProvider.GetKeyedService<IOpenPlatformProvider>(_contactsSyncConfigOptions.OpenPlatformProvider.ToString());
            var templateId = await provider.GetConfigedApprovalTemplateId();
            if (templateId.IsNullOrWhiteSpace())
            {
                // 创建模板
                templateId = await provider.CreateApprovalTemplate();
                var key = provider.ApprovalTemplateKey;
                // 保存模板编号到配置文件中
                SettingsHelper.AddOrUpdateAppSetting<string>(key, templateId);
            }

            string userid = Request.Cookies[ContactsSyncWebConsts.CookieName];
            if (!string.IsNullOrWhiteSpace(userid))
            {
                return RedirectToAction("Manage");
            }

            if (_contactsSyncConfigOptions.OpenPlatformProvider == OpenPlatformProviderEnum.DingDing)
            {
                ViewBag.CorpId = Configuration.GetValue<string>("DingDing:CorpId");
                return View("DingDingOauth");
            }

            if (_contactsSyncConfigOptions.OpenPlatformProvider == OpenPlatformProviderEnum.WeWork)
            {
                var host = Request.Host.ToString();
                var redirectUrl = HttpUtility.UrlEncode($"{Request.Scheme}://{host}/AuthorizeCallback");

                var authorizeUrl = await provider.GetAuthorizeUrl(redirectUrl);
                return Redirect(authorizeUrl);
            }

            return NotFound();
        }

        [HttpGet]
        [Route("/AuthorizeCallback")]
        public async Task<IActionResult> AuthorizeCallback(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Content("请先授权");
            }

            // todo 待abp 8.1发布后，使用LazyServiceProvider替换
            var provider = ServiceProvider.GetKeyedService<IOpenPlatformProvider>(_contactsSyncConfigOptions.OpenPlatformProvider.ToString());
            var userId = await provider.GetUserIdByCode(code);
            var cookieOptions = new CookieOptions { HttpOnly = true, Expires = DateTimeOffset.Now.AddHours(2) };
            Response.Cookies.Append(ContactsSyncWebConsts.CookieName, userId, cookieOptions);
            return RedirectToAction("Manage");
        }

        public async Task<IActionResult> Manage()
        {
            string userid = Request.Cookies[ContactsSyncWebConsts.CookieName];
            if (string.IsNullOrWhiteSpace(userid))
            {
                return RedirectToAction("Index");
            }

            var userSimpleDto = await UserAppService.GetSimpleUserByUserIdAsync(userid);

            return View(userSimpleDto);
        }

        [Route("/CreateApproval")]
        public async Task CreateApproval(CreateUserApprovalViewModel input)
        {
            if (!ModelState.IsValid)
            {
                throw new UserFriendlyException("提交的数据有误");
            }

            try
            {
                await UserAppService.CreateUserApproval(input.Uid, input.ApprovalData);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "提交审批申请发生错误");
                throw new UserFriendlyException("提交审批申请失败");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}