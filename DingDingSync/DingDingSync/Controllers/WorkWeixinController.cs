using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using DingDingSync.Application.AppService;
using DingDingSync.Application.WorkWeixinUtils;
using DingDingSync.Core;
using DingDingSync.Web.Startup;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace DingDingSync.Web.Controllers;

public class WorkWeixinController : AbpController
{
    public IConfiguration Configuration { get; set; }
    
    public IUserAppService UserAppService { get; set; }
    
    public IWorkWeixinAppService WorkWeixinAppService { get; set; }

    private readonly WorkWeixinConfigOptions _weixinConfigOptions;

    public WorkWeixinController(IOptions<WorkWeixinConfigOptions> options)
    {
        _weixinConfigOptions = options.Value;
    }


    public IActionResult Index()
    {
        var corpId = _weixinConfigOptions.CorpId;
        var host = Request.Host.ToString();
        var redirectUrl = WebUtility.UrlEncode($"{Request.Scheme}://{host}/WorkWeixin_Authorize");
        var randomNumber = Random.Shared.Next(10000, 99999);
        var appid = _weixinConfigOptions.AgentId;
        var url =
            $"https://open.weixin.qq.com/connect/oauth2/authorize?appid={corpId}&redirect_uri={redirectUrl}&response_type=code&scope=snsapi_privateinfo&state={randomNumber}&agentid={appid}#wechat_redirect";
        return Redirect(url);
    }

    [HttpGet]
    [Route("/WorkWeixin_Authorize")]
    public async Task<IActionResult> Authorize(string code, string state)
    {
        var accessToken = await GetWorkWeixinAccessToken();
        var client = new HttpClient();
        var url = $"https://qyapi.weixin.qq.com/cgi-bin/auth/getuserinfo?access_token={accessToken}&code={code}";
        var response = await client.GetAsync(url);
        var result = await response.Content.ReadAsStringAsync();
        /*
        {
          "userid": "ZhengWei",
          "errcode": 0,
          "errmsg": "ok",
          "user_ticket": "KNDoS9LEcUDLy_R3_yaZ-CwcEg6-Jtk-bxggzPi9qQQaaYNcdcFARPkkdrA4lVSoIHdjYiNZM_HNRNqDWiKDTvWvK_FUcig4fx7pJCR_FWM",
          "expires_in": 1800
        }
         */

        var jObject = JObject.Parse(result);
        var errorCode = jObject.Value<int>("errcode");
        if (errorCode == 0)
        {
            var userid = jObject.Value<string>("userid");

            Response.Cookies.Append(LdapConsts.CookieName, userid,
                new CookieOptions
                    { HttpOnly = true, Expires = DateTimeOffset.Now.AddDays(7) });
            return RedirectToAction("Manage", "Home");
        }
        else
        {
            return Content(jObject.Value<string>("errmsg"));
        }

    }

    private async Task<string> GetWorkWeixinAccessToken()
    {
        var corpId = _weixinConfigOptions.CorpId;
        var secret = _weixinConfigOptions.AppSecret;
        var url = $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpId}&corpsecret={secret}";
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        var result = await response.Content.ReadAsStringAsync();
        var jObject = JObject.Parse(result);
        var errorCode = jObject.Value<int>("errcode");
        if (errorCode == 0)
        {
            return jObject.Value<string>("access_token");
        }

        return result;
    }

    [HttpGet]
    [Route("/WorkWeixin_Callback")]
    public IActionResult Callback_Get(string msgSignature, string timestamp, string nonce, string echostr)
    {
        var token = Configuration["WorkWeixin:Token"];
        var encodingAesKey = Configuration["WorkWeixin:EncodingAESKey"];
        var corpId = Configuration["WorkWeixin:CorpId"];
        var wxBizMsgCrypt = new WXBizMsgCrypt(token, encodingAesKey, corpId);
        var returnStr = string.Empty;
        var resultCode = wxBizMsgCrypt.VerifyURL(msgSignature, timestamp, nonce, echostr, ref returnStr);
        if (resultCode == 0)
        {
            return Content(returnStr);
        }
        else
        {
            return Json(new { Msg = $"参数错误，错误代码为：{resultCode}" });
        }
    }
    
    [Route("/WorkWeixin_Sync")]
    public async Task<IActionResult> Sync()
    {
        await UserAppService.SyncDepartMentAndUserFromWorkWeixin();
        return Content("同步完成");
    }
}