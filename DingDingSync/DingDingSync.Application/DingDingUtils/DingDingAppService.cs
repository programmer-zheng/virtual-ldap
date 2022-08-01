using Abp.Runtime.Caching;
using Abp.UI;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using Tea;

namespace DingDingSync.Application.DingDingUtils
{
    public class DingDingAppService : IDingdingAppService
    {
        private readonly DingDingConfigOptions _dingDingConfigOptions;

        public ILogger Logger { get; set; }

        public ICacheManager CacheManager { get; set; }

        public DingDingAppService(IOptions<DingDingConfigOptions> options
        )
        {
            _dingDingConfigOptions = options.Value;
        }


        public AlibabaCloud.SDK.Dingtalkoauth2_1_0.Client CreateClient()
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config();
            config.Protocol = "https";
            config.RegionId = "central";
            return new AlibabaCloud.SDK.Dingtalkoauth2_1_0.Client(config);
        }

        public string GetAccessToken()
        {
            var cache = CacheManager.GetCache("DingDing").AsTyped<string, string>();
            var result = cache.Get("AccessToken", () =>
            {
                AlibabaCloud.SDK.Dingtalkoauth2_1_0.Client client = CreateClient();
                AlibabaCloud.SDK.Dingtalkoauth2_1_0.Models.GetAccessTokenRequest getAccessTokenRequest =
                    new AlibabaCloud.SDK.Dingtalkoauth2_1_0.Models.GetAccessTokenRequest
                    {
                        AppKey = _dingDingConfigOptions.AppKey,
                        AppSecret = _dingDingConfigOptions.AppSecret,
                    };
                try
                {
                    var xx = client.GetAccessToken(getAccessTokenRequest);
                    return xx.Body.AccessToken;
                }
                catch (TeaException err)
                {
                    if (!AlibabaCloud.TeaUtil.Common.Empty(err.Code) && !AlibabaCloud.TeaUtil.Common.Empty(err.Message))
                    {
                        // err 中含有 code 和 message 属性，可帮助开发定位问题
                    }
                }
                catch (Exception _err)
                {
                    TeaException err = new TeaException(new Dictionary<string, object>
                    {
                        {"message", _err.Message}
                    });
                    if (!AlibabaCloud.TeaUtil.Common.Empty(err.Code) && !AlibabaCloud.TeaUtil.Common.Empty(err.Message))
                    {
                        // err 中含有 code 和 message 属性，可帮助开发定位问题
                    }
                }

                return null;
            });
            return result;
        }

        public string GetJsapiTicket()
        {
            var token = GetAccessToken();
            //https://oapi.dingtalk.com/get_jsapi_ticket
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/get_jsapi_ticket");
            OapiGetJsapiTicketRequest req = new OapiGetJsapiTicketRequest();
            req.SetHttpMethod("GET");
            try
            {
                OapiGetJsapiTicketResponse rsp = client.Execute(req, token);
                if (rsp.Errcode != 0)
                {
                    throw new UserFriendlyException(rsp.Errmsg);
                }

                return rsp.Ticket;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public List<OapiDepartmentListResponse.DepartmentDomain> GetDepartmentList(long dept_id = 1)
        {
            try
            {
                var token = GetAccessToken();
                IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/department/list");
                var req = new OapiDepartmentListRequest();
                req.FetchChild = true;
                req.Id = $"{dept_id}";
                req.SetHttpMethod("GET");
                var rsp = client.Execute(req, token);
                if (rsp.Errcode != 0)
                {
                    throw new UserFriendlyException(rsp.Errmsg);
                }

                return rsp.Department;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public OapiV2DepartmentGetResponse.DeptGetResponseDomain GetDepartmentDetail(long deptId)
        {
            try
            {
                var token = GetAccessToken();
                IDingTalkClient client =
                    new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/department/get");
                var req = new OapiV2DepartmentGetRequest();
                req.DeptId = deptId;
                var rsp = client.Execute(req, token);
                if (rsp.Errcode != 0)
                {
                    throw new UserFriendlyException(rsp.Errmsg);
                }

                return rsp.Result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public List<OapiV2UserListResponse.ListUserResponseDomain> GetUserList(long dept_id, long cursor = 0)
        {
            try
            {
                var token = GetAccessToken();
                IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/user/list");
                OapiV2UserListRequest req = new OapiV2UserListRequest();
                req.DeptId = dept_id;
                req.Cursor = cursor;
                req.Size = 100;
                //req.OrderField = "entry_asc";
                OapiV2UserListResponse rsp = client.Execute(req, token);
                if (rsp.IsError)
                {
                    throw new UserFriendlyException(rsp.SubErrMsg);
                }

                var list = new List<OapiV2UserListResponse.ListUserResponseDomain>();
                if (rsp.Result.List != null)
                {
                    list.AddRange(rsp.Result.List);
                }

                if (rsp.Result.HasMore)
                {
                    list.AddRange(GetUserList(dept_id, rsp.Result.NextCursor));
                }

                return list;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public OapiV2UserGetResponse.UserGetResponseDomain GetUserDetail(string userid)
        {
            //
            try
            {
                var token = GetAccessToken();
                IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/user/get");
                var req = new OapiV2UserGetRequest();
                req.Userid = userid;
                var rsp = client.Execute(req, token);
                if (rsp.Errcode != 0)
                {
                    throw new UserFriendlyException(rsp.Errmsg);
                }

                return rsp.Result;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public long SendMessage(long userid, string message)
        {
            try
            {
                var token = GetAccessToken();
                IDingTalkClient client =
                    new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2");
                OapiMessageCorpconversationAsyncsendV2Request req = new OapiMessageCorpconversationAsyncsendV2Request();
                req.AgentId = _dingDingConfigOptions.AgentId;
                req.UseridList = userid.ToString();
                req.Msg_ = new OapiMessageCorpconversationAsyncsendV2Request.MsgDomain
                {
                    Msgtype = "text",
                    Text = new OapiMessageCorpconversationAsyncsendV2Request.TextDomain
                    {
                        Content = message
                        //$"您的账号：xxxx，已于{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}开通，默认密码：123456。该账号可用于公司内部SVN、Git、VPN、Jenkins等基础服务登录，请尽快修改密码。"
                    }
                };
                OapiMessageCorpconversationAsyncsendV2Response rsp = client.Execute(req, token);
                if (rsp.IsError)
                {
                    throw new UserFriendlyException(rsp.SubErrMsg);
                }

                return rsp.TaskId;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public OapiV2UserGetuserinfoResponse.UserGetByCodeResponseDomain GetUserinfoByCode(string code)
        {
            var token = GetAccessToken();
            IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/user/getuserinfo");
            OapiV2UserGetuserinfoRequest req = new OapiV2UserGetuserinfoRequest();
            req.Code = code;
            OapiV2UserGetuserinfoResponse rsp = client.Execute(req, token);
            if (rsp.IsError)
            {
                throw new UserFriendlyException(rsp.SubErrMsg);
            }

            return rsp.Result;
        }

        public List<OapiCallBackGetCallBackFailedResultResponse.FailedDomain> GetCallbackFailEvents()
        {
            try
            {
                var token = GetAccessToken();
                IDingTalkClient client =
                    new DefaultDingTalkClient("https://oapi.dingtalk.com/call_back/get_call_back_failed_result");
                var req = new OapiCallBackGetCallBackFailedResultRequest();
                req.SetHttpMethod("GET");
                var rsp = client.Execute(req, token);
                if (rsp.Errcode != 0)
                {
                    throw new UserFriendlyException(rsp.Errmsg);
                }

                return rsp.FailedList;
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
    }
}