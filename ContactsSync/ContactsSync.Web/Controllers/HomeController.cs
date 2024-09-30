using System.Diagnostics;
using System.Web;
using ContactsSync.Application.Background;
using ContactsSync.Application.Contracts;
using ContactsSync.Application.Contracts.OpenPlatformProvider;
using ContactsSync.Application.Contracts.SyncConfig;
using ContactsSync.Domain.Settings;
using ContactsSync.Domain.Shared;
using ContactsSync.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Settings;

namespace ContactsSync.Web.Controllers
{
    public class HomeController : AbpController
    {
        #region 构造函数及注入

        private readonly ContactsSyncConfigOptions _contactsSyncConfigOptions;

        public HomeController(IOptionsSnapshot<ContactsSyncConfigOptions> syncConfig)
        {
            _contactsSyncConfigOptions = syncConfig.Value;
        }

        public IConfiguration Configuration { get; set; }

        public IUserAppService UserAppService { get; set; }

        public ISettingProvider SettingManagementProvider { get; set; }

        public ISyncConfigAppService SyncConfigApplicationService { get; set; }

        #endregion

        public async Task<IActionResult> Settings([FromServices] ISyncConfigAppService syncConfigAppService)
        {
            var viewModel = new SyncConfigSettingViewModel();
            var syncConfigDto = await syncConfigAppService.GetSyncConfig();
            if (syncConfigDto != null)
            {
                viewModel.ProviderName = syncConfigDto.ProviderName;
                viewModel.SyncPeriod = syncConfigDto.SyncPeriod;
                if (syncConfigDto.ProviderName == OpenPlatformProviderEnum.DingDing)
                {
                    viewModel.DingTalk = syncConfigDto.ProviderConfig as DingTalkConfigDto;
                }
                else if (syncConfigDto.ProviderName == OpenPlatformProviderEnum.WeWork)
                {
                    viewModel.WeWork = syncConfigDto.ProviderConfig as WeWorkConfigDto;
                }
            }

            return View(viewModel);
        }

        [Route("/SaveSettings")]
        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<IActionResult> SaveSettings([FromBody] UpdateContactsSyncConfigDto dto)
        {
            await SyncConfigApplicationService.SaveSyncConfig(dto);
            return Json(null);
        }

        public async Task<IActionResult> Index()
        {
            var providerName = await SettingManagementProvider.GetOrNullAsync(ContactsSyncSettings.ProviderName);
            if (providerName.IsNullOrWhiteSpace())
            {
                return RedirectToAction("Settings");
            }

            var provider = LazyServiceProvider.GetKeyedService<IOpenPlatformProviderApplicationService>(_contactsSyncConfigOptions.OpenPlatformProvider.ToString());

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

            var provider = LazyServiceProvider.GetKeyedService<IOpenPlatformProviderApplicationService>(_contactsSyncConfigOptions.OpenPlatformProvider.ToString());
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

        [HttpPost]
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