using Abp.Application.Services;
using DingTalk.Api.Response;

namespace DingDingSync.Application.DingDingUtils
{
    public interface IDingdingAppService : IApplicationService
    {
        string GetAccessToken();

        string GetJsapiTicket();

        OapiV2DepartmentGetResponse.DeptGetResponseDomain GetDepartmentDetail(long deptId);

        List<OapiDepartmentListResponse.DepartmentDomain> GetDepartmentList(long dept_id = 1);

        OapiV2UserGetResponse.UserGetResponseDomain GetUserDetail(string userid);

        List<OapiV2UserListResponse.ListUserResponseDomain> GetUserList(long dept_id, long cursor = 0);

        long SendMessage(long userid, string message);

        OapiV2UserGetuserinfoResponse.UserGetByCodeResponseDomain GetUserinfoByCode(string code);


        /// <summary>
        /// 获取推送失败的事件列表
        /// <para>参考文档：https://open.dingtalk.com/document/org/obtain-the-event-list-of-failed-push-messages</para>
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        List<OapiCallBackGetCallBackFailedResultResponse.FailedDomain> GetCallbackFailEvents();

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="msgContent"></param>
        /// <returns></returns>
        OapiMessageCorpconversationAsyncsendV2Response SendTextMessage(string userid, string msgContent);
    }
}