using Abp.Application.Services;
using DingTalk.Api.Response;

namespace VirtualLdap.Application.DingDingUtils
{
    public interface IDingdingAppService : IApplicationService
    {
        string GetAccessToken();

        string GetJsapiTicket();

        /// <summary>
        /// 获取部门详情
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <returns></returns>
        OapiV2DepartmentGetResponse.DeptGetResponseDomain GetDepartmentDetail(long deptId);

        /// <summary>
        /// 获取下级部门基础信息
        /// </summary>
        /// <param name="parentDeptId">父级部门ID，默认公司层级的ID为1，父级ID为0</param>
        /// <returns></returns>
        List<OapiDepartmentListResponse.DepartmentDomain> GetDepartmentList(long parentDeptId = 0);

        OapiV2UserGetResponse.UserGetResponseDomain GetUserDetail(string userid);

        /// <summary>
        /// 获取部门人员列表 
        /// </summary>
        /// <param name="deptId">部门ID</param>
        /// <param name="cursor">分页游标，默认为0</param>
        /// <returns></returns>
        List<OapiV2UserListResponse.ListUserResponseDomain> GetUserList(long deptId, long cursor = 0);

        long SendMessage(long userid, string message);

        OapiV2UserGetuserinfoResponse.UserGetByCodeResponseDomain GetUserinfoByCode(string code);


        /// <summary>
        /// 获取推送失败的事件列表
        /// <para>参考文档：https://open.dingtalk.com/document/org/obtain-the-event-list-of-failed-push-messages</para>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        List<OapiCallBackGetCallBackFailedResultResponse.FailedDomain> GetCallbackFailEvents();

    }
}