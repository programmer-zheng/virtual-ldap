using System.Text;
using Abp.Extensions;
using Abp.Runtime.Caching;
using Abp.UI;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DingDingSync.Application.WorkWeixinUtils;

public class WorkWeixinAppService : IWorkWeixinAppService, ICommonProvider
{
    public ICacheManager CacheManager { get; set; }

    private readonly WorkWeixinConfigOptions _weixinConfigOptions;

    public WorkWeixinAppService(IOptions<WorkWeixinConfigOptions> options)
    {
        _weixinConfigOptions = options.Value;
    }

    public async Task<string> GetAccessToken()
    {
        var cache = CacheManager.GetCache("WorkWeixin").AsTyped<string, string>();
        var result = await cache.GetAsync("AccessToken", async () =>
        {
            var corpId = _weixinConfigOptions.CorpId;
            var secret = _weixinConfigOptions.AppSecret;
            var url = $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpId}&corpsecret={secret}";
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            var apiReturnResult = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(apiReturnResult);
            var errorCode = jObject.Value<int>("errcode");
            if (errorCode == 0)
            {
                return jObject.Value<string>("access_token");
            }
            else
            {
                var msg = jObject.Value<string>("errmsg");
                throw new UserFriendlyException($"调用企业微信API获取AccessToken时返回异常，错误代码为：{errorCode}，错误信息为：{msg}");
            }

            return apiReturnResult;
        });
        return result;
    }

    public async Task<string> GetUserId(string code, string state)
    {
        var accessToken = await GetAccessToken();
        var client = new HttpClient();
        var url = $"https://qyapi.weixin.qq.com/cgi-bin/auth/getuserinfo?access_token={accessToken}&code={code}";
        var response = await client.GetAsync(url);
        var result = await response.Content.ReadAsStringAsync();
        /*
            {
              "userid": "xxxx",
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
            return jObject.Value<string>("userid");
        }
        else
        {
            throw new UserFriendlyException(jObject.Value<string>("errmsg"));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<List<WorkWeixinDeptListDto>> GetDepartmentList()
    {
        var accessToken = await GetAccessToken();
        var url = $"https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={accessToken}";
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        var apiReturnResult = await response.Content.ReadAsStringAsync();
        var jObject = JObject.Parse(apiReturnResult);
        var errorCode = jObject.Value<int>("errcode");
        if (errorCode == 0)
        {
            var array = (JArray)jObject["department"];
            var deparmentList = array.ToObject<List<WorkWeixinDeptListDto>>();
            return deparmentList;
        }
        else
        {
            var msg = jObject.Value<string>("errmsg");
            throw new UserFriendlyException($"调用企业微信API获取部门列表时返回异常，错误代码为：{errorCode}，错误信息为：{msg}");
        }
    }

    public async Task<List<WorkWeixinDeptUserListDto>> GetUserList(long deptId)
    {
        var accessToken = await GetAccessToken();
        var url = $"https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token={accessToken}&department_id={deptId}";
        var client = new HttpClient();
        var response = await client.GetAsync(url);
        var apiReturnResult = await response.Content.ReadAsStringAsync();
        var jObject = JObject.Parse(apiReturnResult);
        var errorCode = jObject.Value<int>("errcode");
        if (errorCode == 0)
        {
            var array = (JArray)jObject["userlist"];
            var userList = array.ToObject<List<WorkWeixinDeptUserListDto>>();
            return userList;
        }
        else
        {
            var msg = jObject.Value<string>("errmsg");
            throw new UserFriendlyException($"调用企业微信API获取部门下属人员时返回异常，错误代码为：{errorCode}，错误信息为：{msg}");
        }
    }

    public async Task SendTextMessage(string userid, string msgContent)
    {
        if (userid.IsNullOrWhiteSpace())
        {
            return;
        }

        var accessToken = await GetAccessToken();
        var url = $"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={accessToken}";
        var client = new HttpClient();
        var textMessage = new
        {
            touser = userid,
            msgtype = "text",
            agentid = _weixinConfigOptions.AgentId,
            text = new
            {
                content = msgContent
            }
        };
        var json = JsonConvert.SerializeObject(textMessage);
        var content = new StringContent(json, Encoding.UTF8);
        var response = await client.PostAsync(url, content);
        var apiReturnResult = await response.Content.ReadAsStringAsync();
        var jObject = JObject.Parse(apiReturnResult);
        var errorCode = jObject.Value<int>("errcode");
        if (errorCode != 0)
        {
            var msg = jObject.Value<string>("errmsg");
            throw new UserFriendlyException($"调用企业微信API发送文本消息时返回异常，错误代码为：{errorCode}，错误信息为：{msg}");
        }
    }
}