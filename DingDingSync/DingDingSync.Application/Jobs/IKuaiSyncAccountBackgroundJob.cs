using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using DingDingSync.Application.IKuai;
using DingDingSync.Application.IKuai.Dtos;
using DingDingSync.Core.Entities;

namespace DingDingSync.Application.Jobs;

public class IKuaiSyncAccountBackgroundJob : BackgroundJob<string>, ITransientDependency
{
    private readonly IRepository<UserEntity, string> _userRepository;
    private readonly IkuaiAppService _ikuaiAppService;

    public IKuaiSyncAccountBackgroundJob(IRepository<UserEntity, string> userRepository,
        IkuaiAppService ikuaiAppService)
    {
        _userRepository = userRepository;
        _ikuaiAppService = ikuaiAppService;
    }

    public override void Execute(string userId)
    {
        // 根据userId查询用户信息
        var user = _userRepository.FirstOrDefault(userId);
        // 若用户不为空 判断VPN是否启用
        if (user != null && user.VpnAccountEnabled)
        {
            var ikuaiAccount = _ikuaiAppService.GetAccountIdByUsername(user.UserName);
            // 域账号被删除
            if (user.IsDeleted)
            {
                // 爱快账号不为空
                if (ikuaiAccount != null)
                {
                    _ikuaiAppService.RemoveAccount(ikuaiAccount.id);
                }
            }
            else
            {
                // 解密用户密码                                                                                         
                var password = user.Password.DesDecrypt();
                // 同步用户名、密码至爱快开放平台（判断账号是否创建，未创建调用新建接口，已建，调用修改接口）
                if (ikuaiAccount == null)
                {
                    _ikuaiAppService.CreateAccount(new AccountCommon(user.UserName, password, user.Name));
                }
                else
                {
                    ikuaiAccount.share = 2;
                    ikuaiAccount.passwd = password;
                    ikuaiAccount.enabled = "yes";
                    _ikuaiAppService.UpdateAccount(ikuaiAccount);
                }
            }
        }
    }
}