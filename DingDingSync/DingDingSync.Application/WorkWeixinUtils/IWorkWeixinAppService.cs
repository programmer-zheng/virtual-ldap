using Abp.Application.Services;

namespace DingDingSync.Application.WorkWeixinUtils;

public interface IWorkWeixinAppService: IApplicationService
{
    /// <summary>
    /// 获取AccessToken
    /// </summary>
    /// <returns></returns>
    Task<string> GetAccessToken();

    /// <summary>
    /// 获取企业部门列表（包含子部门）
    /// </summary>
    /// <returns></returns>
    Task<List<WorkWeixinDeptListDto>> GetDepartmentList();

    Task<List<WorkWeixinDeptUserListDto>> GetUserList(long deptId);

}