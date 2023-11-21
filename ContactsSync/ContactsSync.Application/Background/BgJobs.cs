using ContactsSync.Domain.Shared;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace ContactsSync.Application.Background;

public class BgJobsArgs
{
    public string Name { get; set; }
}

public class BgJobs : AsyncBackgroundJob<BgJobsArgs>, ITransientDependency
{
    private readonly IRepository<UserEntity> _userRepository;

    public BgJobs(IRepository<UserEntity> userRepository)
    {
        _userRepository = userRepository;
    }

    public override async Task ExecuteAsync(BgJobsArgs args)
    {
        Logger.LogInformation("后台Job触发了");
        try
        {
            await _userRepository.InsertAsync(new UserEntity()
            {
                Name = $"{args.Name}{Random.Shared.Next(10000000, 99999999)}",
            });
        }
        catch (Exception e)
        {
            Logger.LogError("插入失败", e);
            throw;
        }

        Logger.LogInformation("后台Job触发了，检查数据是否成功插入");
    }
}