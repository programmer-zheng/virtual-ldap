using System;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.BackgroundJobs;
using Castle.Core.Logging;
using DingDingSync.Application.AppService;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.Jobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace DingDingSync.Web.Controllers
{
    public class DingDingController : AbpController
    {
        
        private readonly DingDingConfigOptions _dingDingConfigOptions;
        
        public ILogger Logger { get; set; }
        
        public IDingdingAppService DingdingAppService { get; set; }

        public IBackgroundJobManager BackgroundJobManager { get; set; }

        public IUserAppService UserAppService { get; set; }


        public DingDingController(IOptions<DingDingConfigOptions> options)
        {
            _dingDingConfigOptions = options.Value;
        }


        [Route("/dingdingsync")]
        public async Task<IActionResult> Sync()
        {
            await UserAppService.SyncDepartmentAndUser();
            return Content("同步完成");
        }

        [Route("/testjob")]
        public async Task<IActionResult> testjob()
        {
            var msg = "{" +
                      "\"CorpId\":\"ding28716be3f4b986aebc961a6cb783455b\"," +
                      "\"EventType\":\"org_dept_remove\"," +
                      "\"DeptId\":[621798889]," +
                      "\"TimeStamp\":\"1648014773789\"}";
            var messageObj = JObject.Parse(msg);
            await BackgroundJobManager.EnqueueAsync<DingDingCallbackBackgroundJob, DingDingCallbackBackgroundJobArgs>(
                new DingDingCallbackBackgroundJobArgs
                {
                    Msg = msg,
                    EventType = messageObj.Value<string>("EventType"),
                });
            return Content("发送请求成功");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="signature">加密签名</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonce">随机数</param>
        /// <returns></returns>
        [HttpPost]
        [Route("/dingdingcallback")]
        public async Task<dynamic> Callback(string signature, string timestamp, string nonce,
            [FromBody] DingMessage dingMessage)
        {
            var aes_key = _dingDingConfigOptions.Aes_Key;
            var token = _dingDingConfigOptions.Token;
            var corpId = _dingDingConfigOptions.AppKey;

            var dingTalkEncryptor = new DingTalkEncryptor(token, aes_key, corpId);
            var message = dingTalkEncryptor.getDecryptMsg(signature, timestamp, nonce, dingMessage.Encrypt);
            var messageObj = JObject.Parse(message);
            var eventType = messageObj.Value<string>("EventType");
            var _corpid = messageObj.Value<string>("CorpId");


            Logger.Warn($"{DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")}事件类型：{eventType}");
            if (!string.IsNullOrWhiteSpace(_corpid) && !_corpid.Equals(corpId, StringComparison.OrdinalIgnoreCase) &&
                !eventType.Equals("check_url", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine();
                Logger.Warn(messageObj.ToString());
                Console.WriteLine();
                await BackgroundJobManager
                    .EnqueueAsync<DingDingCallbackBackgroundJob, DingDingCallbackBackgroundJobArgs>(
                        new DingDingCallbackBackgroundJobArgs
                        {
                            Msg = message,
                            EventType = eventType
                        });
            }

            var longTimeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            var dic = dingTalkEncryptor.getEncryptedMap("success", longTimeStamp);

            #region 注释

            /*
             人员离职消息内容：
            {"CorpId":"ding28716be3f4b986aebc961a6cb783455b","EventType":"user_leave_org","UserId":["013415163130664023"],"OptStaffId":"0113430928611169518","TimeStamp":"1645077203219"}

            部门新增
            {"CorpId":"ding28716be3f4b986aebc961a6cb783455b","EventType":"org_dept_create","DeptId":[600582447],"TimeStamp":"1645150739144"}

            部门修改
            {"CorpId":"ding28716be3f4b986aebc961a6cb783455b","EventType":"org_dept_modify","DeptId":[600582447],"TimeStamp":"1645151768877"}

            部门删除
            {"CorpId":"ding28716be3f4b986aebc961a6cb783455b","EventType":"org_dept_remove","DeptId":[600582447],"TimeStamp":"1645151821549"}
             */
            /*
             * https://open.dingtalk.com/document/orgapp-server/callback-overview
             * 消息加解密
                为了保证数据传输的安全，钉钉在向回调URL推送订阅事件时，会携带配置的token用来验证事件来源。
                同时使用该密钥对消息内容做对称加密。
                单击这里获取回调加解密类库和对应demo。
                钉钉服务器会把msg消息体明文编码成encrypt。
                encrypt = Base64_Encode(AES_Encrypt[random(16B) + msg_len(4B) + msg + $key])
                是对明文消息msg加密处理后的Base64编码。其中：
                random为16字节的随机字符串。
                msg_len为4字节的msg长度，网络字节序。
                msg为消息体明文。
                key为应用的suiteKey。
                取出返回的JSON中的encrypt字段：
                对密文BASE64解码：aes_msg=Base64_Decode(encrypt)；
                使用AESKey做AES解密：rand_msg=AES_Decrypt(aes_msg)；
             */
            /*
             * 返回钉钉的数据格式为:
                {
                  "msg_signature":"111108bb8e6dbce3c9671d6fdb69d1506xxxx",
                  "timeStamp":"1783610513",
                  "nonce":"123456",
                  "encrypt":"1ojQf0NSvw2WPvW7LijxS8UvISr8pdDP+rXpPbcLGOmIxxxx"
                 }             
            msg_signature为消息体签名。
            timeStamp为时间戳。
            nonce为随机字符串。
            encrypt为success加密字符串。
             */

            #endregion

            return dic;
        }

        [Route("/dingdinggetfailevents")]
        public async Task<IActionResult> GetFailEvents()
        {
            var result = DingdingAppService.GetCallbackFailEvents();
            return Json(result);
        }
    }


    public class DingMessage
    {
        public string Encrypt { get; set; }
    }
}