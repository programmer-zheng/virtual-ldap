using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Web;
using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Dingtalkoauth2_1_0;
using AlibabaCloud.SDK.Dingtalkoauth2_1_0.Models;
using AlibabaCloud.SDK.Dingtalkworkflow_1_0.Models;
using AlibabaCloud.TeaUtil;
using AlibabaCloud.TeaUtil.Models;
using ContactsSync.Application.Contracts.OpenPlatformProvider;
using ContactsSync.Application.Contracts.SyncConfig;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tea;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace ContactsSync.Application.DingDing;

public class DingDingProvider : ApplicationService, IOpenPlatformProviderApplicationService
{
    private readonly DingDingConfigOptions _dingDingConfigOptions;

    private readonly ISyncConfigAppService _configAppService;

    private readonly IHttpClientFactory _httpClientFactory;

    public DingDingProvider(IOptionsMonitor<DingDingConfigOptions> options, ISyncConfigAppService configAppService, IHttpClientFactory httpClientFactory)
    {
        _configAppService = configAppService;
        _dingDingConfigOptions = options.CurrentValue;
        _httpClientFactory = httpClientFactory;
    }

    private Config CreateClientConfig()
    {
        Config config = new Config();
        config.Protocol = "https";
        config.RegionId = "central";
        return config;
    }

    private async Task<DingTalkConfigDto> GetDingDingConfig()
    {
        var config = await _configAppService.GetConfigDetail();

        if (config is DingTalkConfigDto)
        {
            return config as DingTalkConfigDto;
        }

        throw new UserFriendlyException("未能获取配置");
    }

    public string Source => "DingDing";

    public string ApprovalTemplateKey => $"{DingDingConfigOptions.DingDing}:ProcessCode";

    public async Task<string> GetAuthorizeUrl(string redirectUrl)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取AccesToken
    /// <para>文档地址：https://open.dingtalk.com/document/orgapp/obtain-the-access_token-of-an-internal-app</para>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    /// <exception cref="TeaException"></exception>
    /// <exception cref="Exception"></exception>
    public async Task<string> GetAccessTokenAsync()
    {
        if (_dingDingConfigOptions.AppKey.IsNullOrWhiteSpace() || _dingDingConfigOptions.AppSecret.IsNullOrWhiteSpace())
        {
            throw new UserFriendlyException("请检查是否正确配置了钉钉的AppKey和AppSecret");
        }

        var config = CreateClientConfig();
        Client client = new Client(config);
        // var dingdingConfig = await GetDingDingConfig();
        GetAccessTokenRequest getAccessTokenRequest = new GetAccessTokenRequest
        {
            AppKey = _dingDingConfigOptions.AppKey,
            AppSecret = _dingDingConfigOptions.AppSecret,
            // AppKey = dingdingConfig.AppKey,
            // AppSecret = dingdingConfig.AppSecret,
        };
        try
        {
            var getAccessTokenResponse = await client.GetAccessTokenAsync(getAccessTokenRequest);
            return getAccessTokenResponse.Body.AccessToken;
        }
        catch (TeaException err)
        {
            if (!Common.Empty(err.Code) && !Common.Empty(err.Message))
            {
                // err 中含有 code 和 message 属性，可帮助开发定位问题
                Logger.LogError($"获取钉钉AccessToken出错，{err.Message}");
            }

            throw err;
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
                Logger.LogError($"获取钉钉AccessToken出错，{err.Message}");
            }

            throw _err;
        }
    }

    public async Task<List<PlatformDepartmentDto>> GetDepartmentListAsync(long? parentDeptId = null)
    {
        return await GetDepartmentListAsyncV1(parentDeptId);
    }

    /// <summary>
    /// 获取部门列表，不返回根部门信息【V1版本，已不再更新】
    /// https://open.dingtalk.com/document/isvapp/get-department-list
    /// </summary>
    /// <param name="parentDeptId"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    private async Task<List<PlatformDepartmentDto>> GetDepartmentListAsyncV2(long? parentDeptId = null)
    {
        var accessToken = await GetAccessTokenAsync();
        var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/department/list");
        var req = new OapiV2DepartmentListsubRequest();
        req.DeptId = parentDeptId;
        req.Language = "zh_CN";
        var rsp = client.Execute<OapiV2DepartmentListsubResponse>(req, accessToken);
        Logger.LogDebug($"钉钉返回部门数据：{JsonConvert.SerializeObject(rsp)}");
        if (rsp.Errcode != 0)
        {
            Logger.LogError(rsp.Errmsg);
            throw new UserFriendlyException(rsp.SubErrMsg.IsNullOrWhiteSpace() ? rsp.Errmsg : rsp.SubErrMsg);
        }

        var departmentList = rsp.Result;
        var result = ObjectMapper.Map<List<OapiV2DepartmentListsubResponse.DeptBaseResponseDomain>, List<PlatformDepartmentDto>>(departmentList);
        return result;
    }

    /// <summary>
    /// 获取部门列表 默认不传父级部门ID时，可返回根部门信息【V1版本，已不再更新，且不推荐】
    /// <para>文档地址：https://open.dingtalk.com/document/orgapp/obtain-the-department-list</para>
    /// </summary>
    /// <param name="parentDeptId"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    private async Task<List<PlatformDepartmentDto>> GetDepartmentListAsyncV1(long? parentDeptId = null)
    {
        var accessToken = await GetAccessTokenAsync();
        var httpClient = _httpClientFactory.CreateClient();
        var apiUrl = "https://oapi.dingtalk.com/department/list";
        var paramDic = new Dictionary<string, string>()
        {
            { "access_token", accessToken },
            { "fetch_child", "true" }
        };
        if (parentDeptId != null && parentDeptId > 0)
        {
            paramDic.Add("id", parentDeptId.ToString());
        }

        var response = await httpClient.GetAsync(BuildRequestUrl(apiUrl, paramDic));

        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var rsp = JsonConvert.DeserializeObject<OapiDepartmentListResponse>(responseString);
            Logger.LogDebug($"钉钉返回部门数据：{JsonConvert.SerializeObject(rsp)}");
            if (rsp.Errcode != 0)
            {
                Logger.LogError(rsp.Errmsg);
                throw new UserFriendlyException(rsp.SubErrMsg.IsNullOrWhiteSpace() ? rsp.Errmsg : rsp.SubErrMsg);
            }

            var departmentList = rsp.Department;
            var result = ObjectMapper.Map<List<OapiDepartmentListResponse.DepartmentDomain>, List<PlatformDepartmentDto>>(departmentList);
            return result;
        }

        return null;
    }

    public async Task<List<PlatformDeptUserDto>> GetDeptUserListAsync(long deptId)
    {
        return await GetDeptUsers(deptId);
    }

    /// <summary>
    /// 获取部门成员列表
    /// <para>文档地址：https://open.dingtalk.com/document/orgapp/queries-the-complete-information-of-a-department-user</para>
    /// </summary>
    /// <param name="deptId">部门ID</param>
    /// <param name="cursor"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    private async Task<List<PlatformDeptUserDto>> GetDeptUsers(long deptId, long cursor = 0)
    {
        var result = new List<PlatformDeptUserDto>();
        var accessToken = await GetAccessTokenAsync();
        var httpClient = _httpClientFactory.CreateClient();
        var apiUrl = "https://oapi.dingtalk.com/topapi/v2/user/list";
        var paramDic = new Dictionary<string, string>()
        {
            { "access_token", accessToken },
            { "dept_id", deptId.ToString() },
            { "cursor", cursor.ToString() },
            { "size", "100" },
        };
        var response = await httpClient.GetAsync(BuildRequestUrl(apiUrl, paramDic));
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var rsp = JsonConvert.DeserializeObject<OapiV2UserListResponse>(responseString);
            if (rsp.Errcode != 0)
            {
                Logger.LogError(rsp.Errmsg);
                throw new UserFriendlyException(rsp.SubErrMsg.IsNullOrWhiteSpace() ? rsp.Errmsg : rsp.SubErrMsg);
            }

            var users = rsp.Result.List;
            result = ObjectMapper.Map<List<OapiV2UserListResponse.ListUserResponseDomain>, List<PlatformDeptUserDto>>(users);
            foreach (var platformDeptUserDto in result)
            {
                var user = users.First(t => t.Userid == platformDeptUserDto.UserId);
                platformDeptUserDto.IsDeptLeader = user.Leader;
            }

            if (rsp.Result.HasMore)
            {
                result.AddRange(await GetDeptUsers(deptId, rsp.Result.NextCursor));
            }
        }

        return result;
    }

    /// <summary>
    /// 通过免登码获取用户信息
    /// <para>文档地址：https://open.dingtalk.com/document/isvapp/obtain-the-userid-of-a-user-by-using-the-log-free</para>
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<string> GetUserIdByCode(string code)
    {
        var accessToken = await GetAccessTokenAsync();
        var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/user/getuserinfo");
        var req = new OapiV2UserGetuserinfoRequest();
        req.Code = code;
        var rsp = client.Execute<OapiV2UserGetuserinfoResponse>(req, accessToken);

        Logger.LogDebug($"授权用户信息：{JsonConvert.SerializeObject(rsp)}");
        if (rsp.Errcode != 0)
        {
            Logger.LogError(rsp.Errmsg);
            throw new UserFriendlyException(rsp.SubErrMsg.IsNullOrWhiteSpace() ? rsp.Errmsg : rsp.SubErrMsg);
        }

        return rsp.Result.Userid;
    }

    /// <summary>
    /// 创建审批表单模板
    /// <para>文档地址：https://open.dingtalk.com/document/isvapp/create-or-modify-an-approval-form-template</para>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<string?> CreateApprovalTemplate()
    {
        var accessToken = await GetAccessTokenAsync();
        var config = CreateClientConfig();
        var client = new AlibabaCloud.SDK.Dingtalkworkflow_1_0.Client(config);
        FormCreateHeaders formCreateHeaders = new FormCreateHeaders { XAcsDingtalkAccessToken = accessToken };
        FormComponent formComponent = new FormComponent
        {
            ComponentType = "TextareaField",
            Props = new FormComponentProps
            {
                Label = "申请开通原因",
                Placeholder = "请输入申请开通域账号原因",
                ComponentId = "Reason",
                Required = true,
            },
        };
        FormCreateRequest formCreateRequest = new FormCreateRequest
        {
            Name = "域账号开通申请",
            FormComponents = new List<FormComponent> { formComponent },
        };
        try
        {
            var rsp = await client.FormCreateWithOptionsAsync(formCreateRequest, formCreateHeaders, new RuntimeOptions());
            return rsp.Body.Result.ProcessCode;
        }
        catch (TeaException err)
        {
            if (!Common.Empty(err.Code) && !Common.Empty(err.Message))
            {
                throw new UserFriendlyException($"创建钉钉审批模板出错 {err.Message}");
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
                Logger.LogError($"创建钉钉审批模板出错 {err.Code} {err.Message}");
            }
        }

        return null;
    }

    /// <summary>
    /// 删除审批模板
    /// <para>文档地址：https://open.dingtalk.com/document/orgapp/delete-a-template</para>
    /// </summary>
    /// <param name="templateNo"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<bool> DeleteApprovalTemplate(string templateNo)
    {
        if (templateNo.IsNullOrWhiteSpace())
        {
            throw new ArgumentNullException(nameof(templateNo), "删除钉钉审批模板需要提供模板编码");
        }

        var result = new List<PlatformDeptUserDto>();
        var accessToken = await GetAccessTokenAsync();
        var httpClient = _httpClientFactory.CreateClient();
        var apiUrl = "https://oapi.dingtalk.com/topapi/process/delete";
        var paramDic = new Dictionary<string, string>()
        {
            { "access_token", accessToken }
        };
        var param = new
        {
            request = new
            {
                agentid = _dingDingConfigOptions.AgentId,
                process_code = templateNo
            }
        };
        var jsonContent = JsonContent.Create(param);
        var response = await httpClient.PostAsync(BuildRequestUrl(apiUrl, paramDic), jsonContent);
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var returnStr = await response.Content.ReadAsStringAsync();
            var rsp = JsonConvert.DeserializeObject<OapiProcessDeleteResponse>(returnStr);
            if (!rsp.Success)
            {
                throw new UserFriendlyException($"删除钉钉审批模板失败,错误码：{rsp.Errcode},错误信息请参考：https://open.dingtalk.com/search?keyword={rsp.Errcode}");
            }
        }

        return true;
    }

    /// <summary>
    /// 发起审批实例
    /// <para>文档地址：https://open.dingtalk.com/document/isvapp/initiate-approval-new</para>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="approvers"></param>
    /// <param name="applyData"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<string> CreateApprovalInstance(string userId, List<string> approvers, string applyData)
    {
        var accessToken = await GetAccessTokenAsync();
        var config = CreateClientConfig();
        var client = new AlibabaCloud.SDK.Dingtalkworkflow_1_0.Client(config);
        var startProcessInstanceHeaders = new StartProcessInstanceHeaders() { XAcsDingtalkAccessToken = accessToken };
        // var dingdingConfig = await GetDingDingConfig();
        var startProcessInstanceRequest = new StartProcessInstanceRequest
        {
            OriginatorUserId = userId,
            ProcessCode = _dingDingConfigOptions.ProcessCode,
            MicroappAgentId = _dingDingConfigOptions.AgentId,
            // ProcessCode = dingdingConfig.ProcessCode,
            // MicroappAgentId = Convert.ToInt64(dingdingConfig.AgentId),
            Approvers = new List<StartProcessInstanceRequest.StartProcessInstanceRequestApprovers>
            {
                // 多个人时使用或签，否则使用单人审批
                new() { ActionType = approvers.Count > 1 ? "OR" : "NONE", UserIds = approvers }
            },
            TargetSelectActioners = new List<StartProcessInstanceRequest.StartProcessInstanceRequestTargetSelectActioners>
            {
                new() { ActionerKey = "approver", ActionerUserIds = approvers }
            },
            FormComponentValues = new List<StartProcessInstanceRequest.StartProcessInstanceRequestFormComponentValues>
            {
                new()
                {
                    Id = "Reason", Name = "申请开通原因", Value = applyData
                }
            },
        };
        try
        {
            var rsp = await client.StartProcessInstanceWithOptionsAsync(startProcessInstanceRequest, startProcessInstanceHeaders, new RuntimeOptions());
            return rsp.Body.InstanceId;
        }
        catch (TeaException err)
        {
            if (!Common.Empty(err.Code) && !Common.Empty(err.Message))
            {
                Logger.LogError($"创建审批实例出错 {err.Code} {err.Message}");
                throw new UserFriendlyException($"创建审批实例出错 {err.Message}");
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
                Logger.LogError($"创建审批实例出错 {err.Code} {err.Message}");
            }
        }

        return null;
    }


    /// <summary>
    /// 构建请求地址
    /// </summary>
    /// <param name="url">url地址</param>
    /// <param name="paramDic">需要追加到url中的QueryString参数字典</param>
    /// <returns></returns>
    private string BuildRequestUrl(string url, Dictionary<string, string> paramDic)
    {
        StringBuilder stringBuilder = new StringBuilder(url);

        bool flag1 = url.Contains("?");
        bool flag2 = url.EndsWith("?") || url.EndsWith("&");
        if (!flag2)
        {
            if (flag1)
            {
                stringBuilder.Append("&");
            }
            else
            {
                stringBuilder.Append("?");
            }
        }

        var paramList = paramDic.Where(t => !string.IsNullOrWhiteSpace(t.Key) && !string.IsNullOrWhiteSpace(t.Value))
            .Select(t => $"{t.Key}={HttpUtility.UrlEncode(t.Value, Encoding.UTF8)}")
            .ToList();
        stringBuilder.Append(string.Join("&", paramList));
        return stringBuilder.ToString();
    }
}