using ContactsSync.Application.OpenPlatformProvider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.AdvancedAPIs.MailList;
using Senparc.Weixin.Work.AdvancedAPIs.OA;
using Senparc.Weixin.Work.AdvancedAPIs.OA.OAJson;
using Senparc.Weixin.Work.Containers;
using Volo.Abp;
using Volo.Abp.ObjectMapping;

namespace ContactsSync.Application.WeWork;

public class WeWorkProvider : IOpenPlatformProvider
{
    private readonly WeWorkConfigOptions _weWorkConfigOptions;

    private readonly IObjectMapper _objectMapper;
    private readonly ILogger<WeWorkProvider> _logger;

    public WeWorkProvider(IOptionsMonitor<WeWorkConfigOptions> options, IObjectMapper objectMapper, ILogger<WeWorkProvider> logger)
    {
        _objectMapper = objectMapper;
        _logger = logger;
        _weWorkConfigOptions = options.CurrentValue;
    }

    public string Source => "WeWork";

    public string ApprovalTemplateKey => $"{WeWorkConfigOptions.WeWork}:TemplateId";

    public async Task<string> GetAuthorizeUrl(string redirectUrl)
    {
        var state = Random.Shared.Next(10000, 99999).ToString();
        return OAuth2Api.GetCode(_weWorkConfigOptions.CorpId, redirectUrl, state, _weWorkConfigOptions.AgentId.ToString());
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var accessToken = await AccessTokenContainer.GetTokenAsync(_weWorkConfigOptions.CorpId, _weWorkConfigOptions.AppSecret);
        return accessToken;
    }

    public async Task<List<PlatformDepartmentDto>> GetDepartmentListAsync(long? parentDeptId = null)
    {
        var accessToken = await GetAccessTokenAsync();
        var departmentListResult = await MailListApi.GetDepartmentListAsync(accessToken, parentDeptId);
        if (departmentListResult.ErrorCodeValue != 0)
        {
            _logger.LogError($"{departmentListResult.errmsg}");
            throw new UserFriendlyException(departmentListResult.errmsg);
        }

        var departmentList = departmentListResult.department;
        var result = _objectMapper.Map<List<DepartmentList>, List<PlatformDepartmentDto>>(departmentList);
        return result;
    }

    public async Task<List<PlatformDeptUserDto>> GetDeptUserListAsync(long deptId, long cursor = 0)
    {
        var accessToken = await GetAccessTokenAsync();
        var deptUserListResult = await MailListApi.GetDepartmentMemberInfoAsync(accessToken, deptId, 0);

        if (deptUserListResult.ErrorCodeValue != 0)
        {
            _logger.LogError($"{deptUserListResult.errmsg}");
            throw new UserFriendlyException(deptUserListResult.errmsg);
        }

        var deptUserList = deptUserListResult.userlist;
        var result = _objectMapper.Map<List<GetMemberResult>, List<PlatformDeptUserDto>>(deptUserList);
        foreach (var platformDeptUserDto in result)
        {
            var user = deptUserList.First(t => t.userid == platformDeptUserDto.UserId);
            var isLeader = user!.is_leader_in_dept.Any(t => t == 1);
            platformDeptUserDto.IsDeptLeader = isLeader;
        }

        return result;
    }

    public async Task<string> GetUserIdByCode(string code)
    {
        var accessToken = await GetAccessTokenAsync();
        var userInfoResult = await OAuth2Api.GetUserIdAsync(accessToken, code);
        if (userInfoResult.ErrorCodeValue != 0)
        {
            _logger.LogError($"{userInfoResult.errmsg}");
            throw new UserFriendlyException(userInfoResult.errmsg);
        }

        return userInfoResult.UserId;
    }

    public async Task<string?> CreateApprovalTemplate()
    {
        var accessToken = await GetAccessTokenAsync();
        var request = new ApprovalCreateTemplateRequest
        {
            template_names = new List<ApprovalCreateTemplateRequest_TextAndLang>
            {
                new() { text = "域账号开通申请", lang = "zh_CN" }
            },
            template_content = new ApprovalCreateTemplateRequest_TemplateContent
            {
                controls = new List<ApprovalCreateTemplateRequest_TemplateContent_Controls>()
                {
                    new()
                    {
                        property = new ApprovalCreateTemplateRequest_TemplateContent_Controls_Property()
                        {
                            control = "Textarea", id = "Textarea-01",
                            title = new List<ApprovalCreateTemplateRequest_TextAndLang>() { new() { text = "申请开通原因", lang = "zh_CN" } },
                            require = 1,
                            placeholder = new List<ApprovalCreateTemplateRequest_TextAndLang>()
                                { new() { text = "请输入申请开通域账号原因", lang = "zh_CN" } }
                        },
                    }
                }
            }
        };
        var approvalCreateResult = await OaApi.ApprovalCreateTemplateAsync(accessToken, request);
        if (approvalCreateResult.ErrorCodeValue != 0)
        {
            _logger.LogError($"{approvalCreateResult.errmsg}");
            throw new UserFriendlyException(approvalCreateResult.errmsg);
        }

        return approvalCreateResult.P2PData.ToString()!;
    }

    public async Task<string?> GetConfigedApprovalTemplateId()
    {
        return _weWorkConfigOptions.TemplateId;
    }
}