using ContactsSync.Application.DingDing;
using ContactsSync.Application.WeWork;
using ContactsSync.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.Work.Tencent;
using Volo.Abp.AspNetCore.Mvc;

namespace ContactsSync.Web.Controllers;

public class CallbackController : AbpController
{
    private readonly DingDingConfigOptions _dingDingConfigOptions;
    private readonly WeWorkConfigOptions _weWorkConfigOptions;

    public CallbackController(IOptionsSnapshot<DingDingConfigOptions> dingDingConfigOptions, IOptionsSnapshot<WeWorkConfigOptions> weWorkConfigOptions)
    {
        _weWorkConfigOptions = weWorkConfigOptions.Value;
        _dingDingConfigOptions = dingDingConfigOptions.Value;
    }

    // 企业微信回调验证URL
    [HttpGet]
    [Route("/WeWorkCallback")]
    public IActionResult WeWorkCallbackGet(string msg_signature, string timestamp, string nonce, string echostr)
    {
        var token = _weWorkConfigOptions.Token;
        var encodingAesKey = _weWorkConfigOptions.EncodingAESKey;                                                       
        var corpId = _weWorkConfigOptions.CorpId;
        var wxBizMsgCrypt = new WXBizMsgCrypt(token, encodingAesKey, corpId);
        var returnStr = string.Empty;
        var resultCode = wxBizMsgCrypt.VerifyURL(msg_signature, timestamp, nonce, echostr, ref returnStr);
        if (resultCode == 0)
        {
            return Content(returnStr);
        }
        else
        {
            return Content($"参数错误，错误代码为：{resultCode}");
        }
    }

    [HttpPost]
    [Route("/DingDingCallback")]
    public dynamic DingDingCallback(string signature, string timestamp, string nonce, [FromBody] DingDingCallbackViewModel msgInput)
    {
        var aesKey = _dingDingConfigOptions.Aes_Key;
        var token = _dingDingConfigOptions.Token;
        var corpId = _dingDingConfigOptions.AppKey;

        var dingTalkEncryptor = new DingTalkEncryptor(token, aesKey, corpId);
        var message = dingTalkEncryptor.getDecryptMsg(signature, timestamp, nonce, msgInput.Encrypt);
        var messageObj = JObject.Parse(message);
        var eventType = messageObj.Value<string>("EventType");
        var _corpid = messageObj.Value<string>("CorpId");

        var longTimeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        var dic = dingTalkEncryptor.getEncryptedMap("success", longTimeStamp);
        return dic;
    }
}