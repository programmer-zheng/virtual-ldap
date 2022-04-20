using Abp.Application.Services;
using DingDingSync.Application.IKuai.Dtos;
using DingTalk.Api.Response;
using System.Collections.Generic;

namespace DingDingSync.Application.IKuai
{
    public interface IIkuaiAppService : IApplicationService
    {
        IKuaiApiResponse<RouterBasicDto> GetDeviceBaseInfo();

        AccountDto GetAccountIdByUsername(string username);

        bool CreateAccount(AccountCommon account);

        bool UpdateAccount(AccountDto account);

        bool RemoveAccount(int accountId);
    }
}