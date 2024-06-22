using ContactsSync.Application.Contracts.OpenPlatformProvider;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Senparc.Weixin;
using Senparc.Weixin.CommonAPIs;
using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.AdvancedAPIs.MailList;
using Senparc.Weixin.Work.AdvancedAPIs.OA;
using Senparc.Weixin.Work.AdvancedAPIs.OA.OAJson;
using Senparc.Weixin.Work.Containers;
using Volo.Abp;
using Volo.Abp.Application.Services;

namespace ContactsSync.Application.WeWork;

public class WeWorkProvider : ApplicationService, IOpenPlatformProviderApplicationService
{
    private readonly WeWorkConfigOptions _weWorkConfigOptions;


    public WeWorkProvider(IOptionsMonitor<WeWorkConfigOptions> options)
    {
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
        try
        {
            var departmentListResult = await MailListApi.GetDepartmentListAsync(accessToken, parentDeptId);
            if (departmentListResult.ErrorCodeValue != 0)
            {
                Logger.LogError($"{departmentListResult.errmsg}");
                throw new UserFriendlyException(departmentListResult.errmsg);
            }

            var departmentList = departmentListResult.department;
            var result = ObjectMapper.Map<List<DepartmentList>, List<PlatformDepartmentDto>>(departmentList);
            return result;
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }

    public async Task<List<PlatformDeptUserDto>> GetDeptUserListAsync(long deptId)
    {
        var accessToken = await GetAccessTokenAsync();
        var deptUserListResult = await MailListApi.GetDepartmentMemberInfoAsync(accessToken, deptId, 0);

        if (deptUserListResult.ErrorCodeValue != 0)
        {
            Logger.LogError($"{deptUserListResult.errmsg}");
            throw new UserFriendlyException(deptUserListResult.errmsg);
        }

        var deptUserList = deptUserListResult.userlist;
        var result = ObjectMapper.Map<List<GetMemberResult>, List<PlatformDeptUserDto>>(deptUserList);
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
            Logger.LogError($"{userInfoResult.errmsg}");
            throw new UserFriendlyException(userInfoResult.errmsg);
        }

        return userInfoResult.UserId;
    }

    public async Task<string?> CreateApprovalTemplate()
    {
        var accessToken = await GetAccessTokenAsync();
        var request = new ApprovalCreateTemplateRequestExt()
        {
            template_name = new List<ApprovalCreateTemplateRequest_TextAndLang>
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

        var urlFormat = Config.ApiWorkHost + "/cgi-bin/oa/approval/create_template?access_token={0}";
        try
        {
            var approvalCreateResult = await CommonJsonSend.SendAsync<ApprovalCreateTemplateResult>(accessToken, urlFormat, request);
            if (approvalCreateResult.ErrorCodeValue != 0)
            {
                Logger.LogError($"{approvalCreateResult.errmsg}");
                throw new UserFriendlyException(approvalCreateResult.errmsg);
            }

            return approvalCreateResult.template_id;
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }

    public Task<bool> DeleteApprovalTemplate(string templateNo)
    {
        throw new NotImplementedException();
    }

    public async Task<string> CreateApprovalInstance(string userId, List<string> approvers, string applyData)
    {
        var accessToken = await GetAccessTokenAsync();
        var request = new ApplyEventRequest
        {
            creator_userid = userId,
            template_id = _weWorkConfigOptions.TemplateId,
            use_template_approver = 0,
            notify_type = 1,
            approver = new List<ApplyEventRequest_Approver> { new() { attr = 1, userid = approvers } },
            apply_data = new ApplyEventRequest_ApplyData()
            {
                contents = new List<ApplyEventRequest_ApplyData_Contents>()
                {
                    new()
                    {
                        control = "Textarea",
                        id = "Textarea-01",
                        value = new ApplyEventRequest_ApplyData_Contents_Value() { text = applyData }
                    }
                }
            },
            summary_list = new List<ApplyEventRequest_SummaryList>
            {
                new()
                {
                    summary_info = new List<ApplyEventRequest_TextLang>()
                    {
                        new()
                        {
                            text = "域账号开通申请", lang = "zh_CN"
                        }
                    }
                }
            }
        };
        try
        {
            var applyEventResult = await OaApi.ApplyEventAsync(accessToken, request);
            if (applyEventResult.ErrorCodeValue != 0)
            {
                Logger.LogError($"{applyEventResult.errmsg}");
                throw new UserFriendlyException(applyEventResult.errmsg);
            }

            return applyEventResult.sp_no;
        }
        catch (Exception e)
        {
            throw new UserFriendlyException(e.Message);
        }
    }
}