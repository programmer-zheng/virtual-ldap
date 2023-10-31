using Abp.Runtime.Caching;
using Abp.UI;
using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Dingtalkoauth2_1_0;
using AlibabaCloud.SDK.Dingtalkoauth2_1_0.Models;
using AlibabaCloud.TeaUtil;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Microsoft.Extensions.Options;
using Tea;

namespace VirtualLdap.Application.DingDingUtils
{
    public class DingTalkAppService : IDingTalkAppService, IMessageProvider
    {
        private readonly DingTalkConfigOptions _dingDingConfigOptions;

        public DingTalkAppService(IOptions<DingTalkConfigOptions> options)
        {
            _dingDingConfigOptions = options.Value;
        }

        public ICacheManager CacheManager { get; set; }


        private Client CreateClient()
        {
            Config config = new Config();
            config.Protocol = "https";
            config.RegionId = "central";
            return new Client(config);
        }

        public string GetAccessToken()
        {
            var cache = CacheManager.GetCache("DingDing").AsTyped<string, string>();
            var result = cache.Get("AccessToken", () =>
            {
                Client client = CreateClient();
                GetAccessTokenRequest getAccessTokenRequest =
                    new GetAccessTokenRequest
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
                    if (!Common.Empty(err.Code) && !Common.Empty(err.Message))
                    {
                        // err 中含有 code 和 message 属性，可帮助开发定位问题
                    }
                }
                catch (Exception _err)
                {
                    TeaException err = new TeaException(new Dictionary<string, object>
                    {
                        { "message", _err.Message }
                    });
                    if (!Common.Empty(err.Code) && !Common.Empty(err.Message))
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

        /// <summary>
        /// 获取下级部门基础信息
        /// </summary>
        /// <param name="parentDeptId">父级部门ID，默认公司层级的ID为1，父级ID为0</param>
        /// <returns></returns>
        /// <exception cref="UserFriendlyException">接口调用失败，抛出具体异常信息</exception>
        public List<OapiDepartmentListResponse.DepartmentDomain> GetDepartmentList(long parentDeptId = 0)
        {
            try
            {
                var token = GetAccessToken();
                IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/department/list");
                var req = new OapiDepartmentListRequest();
                req.FetchChild = true;
                if (parentDeptId > 0)
                {
                    req.Id = parentDeptId.ToString();
                }

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

        /// <summary>
        /// 获取部门详情
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取部门人员列表 
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <param name="cursor">分页游标，默认为0</param>
        /// <returns></returns>
        public List<OapiV2UserListResponse.ListUserResponseDomain> GetUserList(long deptId, long cursor = 0)
        {
            try
            {
                var token = GetAccessToken();
                IDingTalkClient client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/user/list");
                OapiV2UserListRequest req = new OapiV2UserListRequest();
                req.DeptId = deptId;
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
                    list.AddRange(GetUserList(deptId, rsp.Result.NextCursor));
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

        public async Task SendTextMessageAsync(string userid, string msgContent)
        {
            try
            {
                var token = GetAccessToken();
                IDingTalkClient client =
                    new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/message/corpconversation/asyncsend_v2");
                var req = new OapiMessageCorpconversationAsyncsendV2Request();
                req.AgentId = _dingDingConfigOptions.AgentId;
                req.UseridList = userid;
                req.ToAllUser = false;

                OapiMessageCorpconversationAsyncsendV2Request.MsgDomain msg =
                    new OapiMessageCorpconversationAsyncsendV2Request.MsgDomain();
                msg.Msgtype = "text";
                msg.Text = new OapiMessageCorpconversationAsyncsendV2Request.TextDomain() { Content = msgContent };
                req.Msg_ = msg;
                var rsp = client.Execute(req, token);
                if (rsp.Errcode != 0)
                {
                    throw new UserFriendlyException(rsp.Errmsg);
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }

        public async Task CreateApproval( string originator_user_id, long dept_id, string msg)
        {
            try
            {
                var token = GetAccessToken();
                IDingTalkClient client =
                    new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/processinstance/create");
                var req = new OapiProcessinstanceCreateRequest();
                req.ProcessCode = _dingDingConfigOptions.ProcessCode;
                req.OriginatorUserId = originator_user_id;
                req.DeptId = dept_id;

                // var formComponentValueVoList = new List<OapiProcessinstanceCreateRequest.FormComponentValueVoDomain>();
                // OapiProcessinstanceCreateRequest.FormComponentValueVoDomain formComponentValueVo1 = new OapiProcessinstanceCreateRequest.FormComponentValueVoDomain();
                // formComponentValueVoList.Add(formComponentValueVo1);
                // formComponentValueVo1.Name = ("申请理由");
                // formComponentValueVo1.Value = msg;
                // req.FormComponentValues_ = formComponentValueVoList;

                var rsp = client.Execute(req, token);
                if (rsp.Errcode != 0)
                {
                    throw new UserFriendlyException(rsp.Errmsg);
                }
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
    }
}