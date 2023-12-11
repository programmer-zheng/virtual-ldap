using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Dingtalkoauth2_1_0;
using AlibabaCloud.SDK.Dingtalkoauth2_1_0.Models;
using AlibabaCloud.SDK.Dingtalkworkflow_1_0.Models;
using AlibabaCloud.TeaUtil;
using AlibabaCloud.TeaUtil.Models;
using ContactsSync.Application.OpenPlatformProvider;
using DingTalk.Api;
using DingTalk.Api.Request;
using DingTalk.Api.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Tea;
using Volo.Abp;
using Volo.Abp.ObjectMapping;

namespace ContactsSync.Application.DingDing;

public class DingDingProvider : IOpenPlatformProvider
{
    private readonly DingDingConfigOptions _dingDingConfigOptions;
    private readonly ILogger<DingDingProvider> _logger;
    private readonly IObjectMapper _objectMapper;

    public DingDingProvider(IOptionsMonitor<DingDingConfigOptions> options, ILogger<DingDingProvider> logger, IObjectMapper objectMapper)
    {
        _logger = logger;
        _objectMapper = objectMapper;
        _dingDingConfigOptions = options.CurrentValue;
    }

    private Config CreateClientConfig()
    {
        Config config = new Config();
        config.Protocol = "https";
        config.RegionId = "central";
        return config;
    }

    public string Source => "DingDing";

    public string ApprovalTemplateKey => $"{DingDingConfigOptions.DingDing}:ProcessCode";

    public async Task<string> GetAuthorizeUrl(string redirectUrl)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var config = CreateClientConfig();
        Client client = new Client(config);
        GetAccessTokenRequest getAccessTokenRequest = new GetAccessTokenRequest
        {
            AppKey = _dingDingConfigOptions.AppKey,
            AppSecret = _dingDingConfigOptions.AppSecret,
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
                _logger.LogError($"获取钉钉AccessToken出错，{err.Message}");
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
                _logger.LogError($"获取钉钉AccessToken出错，{err.Message}");
            }
        }

        return null;
    }

    public async Task<List<PlatformDepartmentDto>> GetDepartmentListAsync(long? parentDeptId = null)
    {
        var accessToken = await GetAccessTokenAsync();
        var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/department/listsub");
        var req = new OapiV2DepartmentListsubRequest();
        req.DeptId = parentDeptId;
        req.Language = "zh_CN";
        var rsp = client.Execute<OapiV2DepartmentListsubResponse>(req, accessToken);
        _logger.LogDebug($"钉钉返回部门数据：{JsonConvert.SerializeObject(rsp)}");
        if (rsp.Errcode != 0)
        {
            _logger.LogError(rsp.Errmsg);
            throw new UserFriendlyException(rsp.Errmsg);
        }

        var departmentList = rsp.Result;
        var result = _objectMapper.Map<List<OapiV2DepartmentListsubResponse.DeptBaseResponseDomain>, List<PlatformDepartmentDto>>(departmentList);
        return result;
    }

    public async Task<List<PlatformDeptUserDto>> GetDeptUserListAsync(long deptId, long cursor = 0)
    {
        var accessToken = await GetAccessTokenAsync();
        var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/user/list");
        var req = new OapiV2UserListRequest();
        req.DeptId = deptId;
        req.Cursor = cursor;
        req.Size = 100;
        var rsp = client.Execute<OapiV2UserListResponse>(req, accessToken);

        _logger.LogDebug($"钉钉返回人员数据：{JsonConvert.SerializeObject(rsp)}");
        if (rsp.Errcode != 0)
        {
            _logger.LogError(rsp.Errmsg);
            throw new UserFriendlyException(rsp.Errmsg);
        }

        var users = rsp.Result.List;
        var result = _objectMapper.Map<List<OapiV2UserListResponse.ListUserResponseDomain>, List<PlatformDeptUserDto>>(users);
        foreach (var platformDeptUserDto in result)
        {
            var user = users.First(t => t.Userid == platformDeptUserDto.UserId);
            platformDeptUserDto.IsDeptLeader = user.Leader;
        }

        if (rsp.Result.HasMore)
        {
            result.AddRange(await GetDeptUserListAsync(deptId, rsp.Result.NextCursor));
        }

        return result;
    }

    public async Task<string> GetUserIdByCode(string code)
    {
        var accessToken = await GetAccessTokenAsync();
        var client = new DefaultDingTalkClient("https://oapi.dingtalk.com/topapi/v2/user/getuserinfo");
        var req = new OapiV2UserGetuserinfoRequest();
        req.Code = code;
        var rsp = client.Execute<OapiV2UserGetuserinfoResponse>(req, accessToken);

        _logger.LogDebug($"授权用户信息：{JsonConvert.SerializeObject(rsp)}");
        if (rsp.Errcode != 0)
        {
            _logger.LogError(rsp.Errmsg);
            throw new UserFriendlyException(rsp.Errmsg);
        }

        return rsp.Result.Userid;
    }

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
                Label = "申请开通原因", Placeholder = "请输入申请开通域账号原因", ComponentId = "Reason", Required = true,
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
                _logger.LogError($"创建钉钉审批模板出错 {err.Code} {err.Message}");
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
                _logger.LogError($"创建钉钉审批模板出错 {err.Code} {err.Message}");
            }
        }

        return null;
    }

    public async Task<string?> GetConfigedApprovalTemplateId()
    {
        return _dingDingConfigOptions.ProcessCode;
    }

    public async Task<string> CreateApprovalInstance(string userId, List<string> approvers, string applyData)
    {
        var accessToken = await GetAccessTokenAsync();
        var config = CreateClientConfig();
        var client = new AlibabaCloud.SDK.Dingtalkworkflow_1_0.Client(config);
        var startProcessInstanceHeaders = new StartProcessInstanceHeaders() { XAcsDingtalkAccessToken = accessToken };
        var startProcessInstanceRequest = new StartProcessInstanceRequest
        {
            OriginatorUserId = userId,
            ProcessCode = _dingDingConfigOptions.ProcessCode,
            MicroappAgentId = _dingDingConfigOptions.AgentId,
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
                _logger.LogError($"创建审批实例出错 {err.Code} {err.Message}");
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
                _logger.LogError($"创建审批实例出错 {err.Code} {err.Message}");
            }
        }

        return null;
    }
}