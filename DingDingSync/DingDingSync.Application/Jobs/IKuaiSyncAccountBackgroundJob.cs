using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Repositories;
using DingDingSync.Core.Entities;

namespace DingDingSync.Application.Jobs;

public class IKuaiSyncAccountBackgroundJob : BackgroundJob<string>, ITransientDependency
{
    private readonly IRepository<UserEntity, string> _userRepository;

    public IKuaiSyncAccountBackgroundJob(IRepository<UserEntity, string> userRepository)
    {
        _userRepository = userRepository;
    }

    public override void Execute(string userId)
    {
        // 根据userId查询用户信息
        // 若用户不为空
        // 判断VPN是否启用
        // 解密用户密码                                                                                         
        // 同步用户名、密码至爱快开放平台（判断账号是否创建，未创建调用新建接口，已建，调用修改接口）
        throw new NotImplementedException();
    }
}