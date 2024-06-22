using ContactsSync.Domain.Contacts;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace ContactsSync.TestBase;

public class ContactsSyncTestDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<UserEntity> _userRepository;

    public ContactsSyncTestDataSeedContributor(IRepository<UserEntity> userRepository)
    {
        _userRepository = userRepository;
    }

    [UnitOfWork]
    public async Task SeedAsync(DataSeedContext context)
    {
        // todo 在此添加种子数据
        await AddUsers();
    }

    private async Task AddUsers()
    {
        _userRepository.InsertAsync(new UserEntity() { Name = "test", UserName = "test", UserId = "test", Position = "Test Manager" });
    }
}