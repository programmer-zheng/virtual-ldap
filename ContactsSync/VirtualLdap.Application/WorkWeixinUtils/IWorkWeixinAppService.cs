using Abp.Application.Services;

namespace VirtualLdap.Application.WorkWeixinUtils;

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

    /// <summary>
    /// 获取部门详情
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<WorkWeixinDeptListDto> GetDepartmentDetail(long id);
    Task<WorkWeixinDeptListDto> GetDepartmentDetail(string id);
    

    Task<List<WorkWeixinDeptUserListDto>> GetUserList(long deptId);

    /// <summary>
    /// 获取成员详情
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<WorkWeixinDeptUserListDto> GetUserDetail(string userId);

}