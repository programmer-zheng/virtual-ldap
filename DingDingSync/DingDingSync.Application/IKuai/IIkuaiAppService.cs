using Abp.Application.Services;
using DingDingSync.Application.IKuai.Dtos;

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