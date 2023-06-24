using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DingDingSync.Application.WorkWeixinUtils;

public interface IWorkWeixinAppService: IApplicationService
{
    /// <summary>
    /// 获取AccessToken
    /// </summary>
    /// <returns></returns>
    Task<string> GetAccessToken();

    Task<string> GetUserId(string code, string state);

    /// <summary>
    /// 获取企业部门列表（包含子部门）
    /// </summary>
    /// <returns></returns>
    Task<List<WorkWeixinDeptListDto>> GetDepartmentList();

    Task<List<WorkWeixinDeptUserListDto>> GetUserList(long deptId);

}