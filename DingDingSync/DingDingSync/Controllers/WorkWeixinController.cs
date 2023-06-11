using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Castle.Core.Logging;
using DingDingSync.Application.WorkWeixinUtils;
using DingDingSync.Core;
using DingDingSync.Web.Startup;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DingDingSync.Web.Controllers;

public class WorkWeixinController : AbpController
{
    public ILogger Logger { get; set; }
    public IConfiguration Configuration { get; set; }

    private readonly WorkWeixinConfigOptions _weixinConfigOptions;

    public WorkWeixinController(IOptions<WorkWeixinConfigOptions> options)
    {
        _weixinConfigOptions = options.Value;
    }


    public async Task<IActionResult> Index()
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

        var ticket = jObject.Value<string>("user_ticket");
        url = $"https://qyapi.weixin.qq.com/cgi-bin/auth/getuserdetail?access_token={accessToken}";
        var json = JsonConvert.SerializeObject(new { user_ticket = ticket });
        var content = new StringContent(json, Encoding.UTF8);
        response = await client.PostAsync(url, content);
        result = await response.Content.ReadAsStringAsync();
        /*
            {
              "errcode": 0,
              "errmsg": "ok",
              "userid": "ZhengWei",
              "mobile": "17368463372",
              "gender": "1",
              "email": "",
              "avatar": "https://wx.qlogo.cn/mmhead/UzdMZnQBfw0msjbVww7X0neTbGVJJNoes1ABdXShOb0/0",
              "qr_code": "https://open.work.weixin.qq.com/wwopen/userQRCode?vcode\u003dvc9d9ab2880b16eeec",
              "biz_mail": "zhengwei@qwcs97.wecom.work",
              "address": ""
            }
         */
        return Content(result);
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
    public async Task<IActionResult> Callback_Get(string msg_signature, string timestamp, string nonce, string echostr)
    {
        var token = Configuration["WorkWeixin:Token"];
        var encodingAESKey = Configuration["WorkWeixin:EncodingAESKey"];
        var corpId = Configuration["WorkWeixin:CorpId"];
        var wxBizMsgCrypt = new WXBizMsgCrypt(token, encodingAESKey, corpId);
        var returnStr = string.Empty;
        var resultCode = wxBizMsgCrypt.VerifyURL(msg_signature, timestamp, nonce, echostr, ref returnStr);
        if (resultCode == 0)
        {
            return Content(returnStr);
        }
        else
        {
            return Json(new { Msg = $"参数错误，错误代码为：{resultCode}" });
        }
    }
}