using Abp.Extensions;
using Abp.Runtime.Caching;
using Abp.UI;
using DingDingSync.Application.IKuai.Dtos;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Castle.Core.Logging;
using Tea;

namespace DingDingSync.Application.IKuai
{
    public class IkuaiAppService : IIkuaiAppService
    {
        private readonly IKuaiConfigOptions _ikuaiConfigOptions;

        private readonly string deviceId;

        private readonly string token;

        private readonly ILogger _logger;

        private readonly ICacheManager _cacheManager;

        public IkuaiAppService(IOptions<IKuaiConfigOptions> options, ILogger logger,
            ICacheManager cacheManager)
        {
            _ikuaiConfigOptions = options.Value;
            deviceId = _ikuaiConfigOptions.Gwid;
            _cacheManager = cacheManager;
            _logger = logger;
            token = GetAccessToken();
        }

        #region 辅助方法

        private string GetAccessToken()
        {
            var cache = _cacheManager.GetCache("IKuai").AsTyped<string, string>();
            var result = cache.Get("AccessToken", () =>
            {
                var url = "/open/access_token";
                try
                {
                    var tupe = GetSign();
                    var paramObj = new
                    {
                        client_id = _ikuaiConfigOptions.OpenId,
                        request_time = tupe.Item1,
                        sign = tupe.Item2,
                        client_type = "OPEN_PARTNER",
                        options = new
                        {
                            token_lifetime = 7200
                        }
                    };
                    var response = GetiKuaiApiRequest(url, paramObj);
                    if (!string.IsNullOrWhiteSpace(response))
                    {
                        var obj = JsonConvert.DeserializeObject<AccessTokenDto>(response);
                        return obj.access_token;
                    }
                }
                catch (Exception e)
                {
                    _logger.Error($"请求爱快授权发生异常", e);
                    return string.Empty;
                }

                return string.Empty;
            });
            return result;
        }

        private string GetiKuaiApiRequest(string url, object requestParameter, string token = "", string deviceId = "")
        {
            var client = new HttpClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Add("IK-Access-Token", token);
            }

            if (!string.IsNullOrWhiteSpace(deviceId))
            {
                client.DefaultRequestHeaders.Add("Path", deviceId);
            }

            var parameterJson = string.Empty;
            if (requestParameter != null)
            {
                parameterJson = JsonConvert.SerializeObject(requestParameter, Formatting.None,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }

            try
            {
                var content = new StringContent(parameterJson, Encoding.UTF8, "application/json");
                var response = client.PostAsync(_ikuaiConfigOptions.BasePath + url, content).Result;
                var responseContent = response.Content.ReadAsStringAsync().Result;
                return responseContent;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException($"请求爱快接口{url}时发生异常：", e);
            }
        }

        private Tuple<int, string> GetSign()
        {
            //UNIX_TIMESTAMP
            var timeSpan =
                Convert.ToInt32((DateTime.UtcNow - DateTime.SpecifyKind(new DateTime(1970, 1, 1), DateTimeKind.Utc))
                    .TotalSeconds);
            var sign = EncryptByPublicKey(timeSpan.ToString(), _ikuaiConfigOptions.Open_Rsa_Pubkey);
            return new Tuple<int, string>(timeSpan, sign);
        }

        private string EncryptByPublicKey(string palinData, string publickKey)
        {
            if (string.IsNullOrWhiteSpace(palinData)) return null;
            RSA rsa = CreateRsaFromPublicKey(publickKey);
            byte[] palinDataBytes = Encoding.UTF8.GetBytes(palinData);
            byte[] encryptDataBytes = rsa.Encrypt(palinDataBytes, RSAEncryptionPadding.Pkcs1);
            return Convert.ToBase64String(encryptDataBytes);
        }


        /// <summary>
        /// 创建公钥
        /// </summary>
        /// <param name="publicKeyString"></param>
        /// <returns></returns>
        private RSA CreateRsaFromPublicKey(string publicKeyString)
        {
            byte[] SeqOID = {0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00};
            byte[] x509key;
            byte[] seq = new byte[15];
            publicKeyString = publicKeyString.Replace("-----BEGIN PUBLIC KEY-----", "")
                .Replace("-----END PUBLIC KEY-----", "")
                .Replace("\n", "").Replace("\r", "");
            try
            {
                x509key = Convert.FromBase64String(publicKeyString);
            }
            catch (Exception e)
            {
                _logger.Error($"爱快公钥不正确，当前公钥为：{publicKeyString}", e);
                throw new ArgumentException("爱快公钥不正确");
            }

            using (var mem = new MemoryStream(x509key))
            {
                using (var binr = new BinaryReader(mem))
                {
                    byte bt = 0;
                    ushort twobytes = 0;
                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return null;
                    seq = binr.ReadBytes(15);
                    if (!CompareBytearrays(seq, SeqOID))
                        return null;
                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103)
                        binr.ReadByte();
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();
                    else
                        return null;
                    bt = binr.ReadByte();
                    if (bt != 0x00)
                        return null;
                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130)
                        binr.ReadByte();
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();
                    else
                        return null;
                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;
                    if (twobytes == 0x8102)
                        lowbyte = binr.ReadByte();
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte();
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;

                    byte[] modint = {lowbyte, highbyte, 0x00, 0x00};
                    int modsize = BitConverter.ToInt32(modint, 0);
                    int firstbyte = binr.PeekChar();
                    if (firstbyte == 0x00)
                    {
                        binr.ReadByte();
                        modsize -= 1;
                    }

                    byte[] modulus = binr.ReadBytes(modsize);
                    if (binr.ReadByte() != 0x02)
                        return null;
                    int expbytes = binr.ReadByte();
                    byte[] exponent = binr.ReadBytes(expbytes);
                    var rsa = RSA.Create();
                    var rsaKeyInfo = new RSAParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };
                    rsa.ImportParameters(rsaKeyInfo);
                    return rsa;
                }
            }
        }


        private bool CompareBytearrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;
            int i = 0;
            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }

            return true;
        }

        #endregion

        public IKuaiApiResponse<RouterBasicDto> GetDeviceBaseInfo()
        {
            var url = $"/open/9/{deviceId}";
            var response = GetiKuaiApiRequest(url, null, token, deviceId);
            var result = JsonConvert.DeserializeObject<IKuaiApiResponse<RouterBasicDto>>(response);
            return result;
        }


        public AccountDto GetAccountIdByUsername(string username)
        {
            var url = $"/open/11/{deviceId}";
            var reqParam = new {param = new {username}};
            var response = GetiKuaiApiRequest(url, reqParam, token, deviceId);
            var result = JsonConvert.DeserializeObject<IKuaiApiResponseList<AccountDto>>(response);
            if (result != null && result.Data != null && result.Data.Count > 0)
            {
                return result.Data.FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// 添加账号
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="token"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool CreateAccount(AccountCommon account)
        {
            var url = $"/open/5/{deviceId}";
            var reqParam = new {param = account};
            var response = GetiKuaiApiRequest(url, reqParam, token, deviceId);
            var code = JObject.Parse(response).Value<int>("ErrorCode");
            var flag = code % 10000 == 0;
            if (!flag)
            {
                var errorMsg = string.Empty;
                var jobject = JObject.Parse(response);
                JToken jtoken = null;
                jobject.TryGetValue("ErrorMsg", out jtoken);
                if (jtoken.Type == JTokenType.Array)
                {
                    var arr = from c in jtoken.Children()
                        select c.Value<string>();
                    errorMsg = string.Join(",", arr);
                }
                else if (jtoken.Type == JTokenType.String)
                {
                    errorMsg = jobject.Value<string>("ErrorMsg");
                }

                throw new IKuaiException(code, errorMsg, deviceId);
            }

            return flag;
        }

        /// <summary>
        /// 修改账号
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="token"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool UpdateAccount(AccountDto account)
        {
            var url = $"/open/6/{deviceId}";
            var reqParam = new {param = account};
            var response = GetiKuaiApiRequest(url, reqParam, token, deviceId);
            var code = JObject.Parse(response).Value<int>("ErrorCode");
            var flag = code % 10000 == 0;
            if (!flag)
            {
                var errorMsg = string.Empty;
                var jobject = JObject.Parse(response);
                JToken jtoken = null;
                jobject.TryGetValue("ErrorMsg", out jtoken);
                if (jtoken.Type == JTokenType.Array)
                {
                    var arr = from c in jtoken.Children()
                        select c.Value<string>();
                    errorMsg = string.Join(",", arr);
                }
                else if (jtoken.Type == JTokenType.String)
                {
                    errorMsg = jobject.Value<string>("ErrorMsg");
                }

                throw new IKuaiException(code, errorMsg, deviceId);
            }

            return flag;
        }

        /// <summary>
        /// 删除账号
        /// </summary>
        /// <param name="deviceId"></param>
        /// <param name="token"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public bool RemoveAccount(int accountId)
        {
            var url = $"/open/8/{deviceId}";
            var reqParam = new
            {
                param = new
                {
                    id = accountId
                }
            };
            var response = GetiKuaiApiRequest(url, reqParam, token, deviceId);
            var code = JObject.Parse(response).Value<int>("ErrorCode");
            var flag = code % 10000 == 0;
            if (!flag)
            {
                var errorMsg = JObject.Parse(response).Value<string>("ErrorMsg");
                throw new IKuaiException(code, errorMsg, deviceId);
            }

            return flag;
        }
    }
}