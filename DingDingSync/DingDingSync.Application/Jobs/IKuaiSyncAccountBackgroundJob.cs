using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Castle.Core.Logging;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Application.IKuai.Dtos;
using DingDingSync.Core.Entities;

namespace DingDingSync.Application.Jobs;

public class IKuaiSyncAccountBackgroundJob : BackgroundJob<string>, ITransientDependency
{
    private readonly IRepository<UserEntity, string> _userRepository;
    private readonly IkuaiAppService _ikuaiAppService;
    private readonly ILogger _logger;
    private readonly IDingdingAppService _dingdingAppService;

    public IKuaiSyncAccountBackgroundJob(IRepository<UserEntity, string> userRepository,
        IkuaiAppService ikuaiAppService, ILogger logger, IDingdingAppService dingdingAppService)
    {
        _userRepository = userRepository;
        _ikuaiAppService = ikuaiAppService;
        _logger = logger;
        _dingdingAppService = dingdingAppService;
    }

    
    [UnitOfWork]
    public override void Execute(string userId)
    {
        try
        {
            var message = string.Empty;
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
                        message = "已将你的VPN账号删除，如有疑问，请联系管理员";
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
                        
                        message = $"已为您开通VPN，账号密码与域账号相同；账号为：{user.UserName} ，密码为：{password} ，请勿泄露！";
                    }
                    else
                    {
                        ikuaiAccount.share = 2;
                        ikuaiAccount.passwd = password;
                        ikuaiAccount.enabled = "yes";
                        _ikuaiAppService.UpdateAccount(ikuaiAccount);
                        
                        message = $"域账号信息与VPN已同步；账号为：{user.UserName}，密码为：{password} ，请勿泄露！";
                    }
                }
                                        
                _dingdingAppService.SendTextMessage(userId, message);
            }
        }
        catch (IKuaiException e)
        {
            _logger.Error("调用iKuai接口时发生异常", e);
        }
        catch (Exception e)
        {
            _logger.Error("处理VPN账号时发生异常", e);
        }
    }
}