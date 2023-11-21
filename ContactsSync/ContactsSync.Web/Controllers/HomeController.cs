using System.Diagnostics;
using System.Web;
using ContactsSync.Application.AppServices;
using ContactsSync.Application.OpenPlatformProvider;
using ContactsSync.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Volo.Abp.AspNetCore.Mvc;

namespace ContactsSync.Web.Controllers
{
    public class HomeController : AbpController
    {
        public IConfiguration Configuration { get; set; }

        public IOpenPlatformProvider Provider { get; set; }

        public IUserAppService UserAppService { get; set; }

        public async Task<IActionResult> Index()
        {
            string userid = Request.Cookies[ContactsSyncWebConsts.CookieName];
            if (!string.IsNullOrWhiteSpace(userid))
            {
                return RedirectToAction("Manage");
            }

            var workEnv = Configuration["WorkEnv"];
            if ("DingDing".Equals(workEnv, StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.CorpId = Configuration.GetValue<string>("DingDing:CorpId");
                return View("DingDingOauth");
            }

            if ("WeWork".Equals(workEnv, StringComparison.OrdinalIgnoreCase))
            {
                var host = Request.Host.ToString();
                var redirectUrl = HttpUtility.UrlEncode($"{Request.Scheme}://{host}/AuthorizeCallback");

                var authorizeUrl = await Provider.GetAuthorizeUrl(redirectUrl);
                return Redirect(authorizeUrl);
            }

            return NotFound();
        }

        [Route("/SaveConfig")]
        [HttpGet]
        public async Task<IActionResult> SaveConfig()
        {
            string contentPath = AppContext.BaseDirectory + @"\"; //项目根目录
            Logger.LogInformation($"目录：{contentPath}");
            var filePath = contentPath + "appsettings.json";
            JObject jsonObject;
            using (StreamReader file = new StreamReader(filePath))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                jsonObject = (JObject)JToken.ReadFrom(reader);
                jsonObject["TemplateId"] = "asdasdasdasd";
            }

            using (var writer = new StreamWriter(filePath))
            using (JsonTextWriter jsonwriter = new JsonTextWriter(writer))
            {
                jsonObject.WriteTo(jsonwriter);
            }

            return Ok();
        }

        [Route("/GetConfig")]
        [HttpGet]
        public async Task<IActionResult> GetConfig()
        {
            var templateId = await Provider.GetConfigedApprovalTemplateId();
            return Content(templateId);
        }

        [HttpGet]
        [Route("/AuthorizeCallback")]
        public async Task<IActionResult> AuthorizeCallback(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Content("请先授权");
            }

            var userId = await Provider.GetUserIdByCode(code);
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


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}