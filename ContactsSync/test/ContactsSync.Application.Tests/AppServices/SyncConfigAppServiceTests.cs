using ContactsSync.Application.Contracts.SyncConfig;
using ContactsSync.Domain.Shared;
using Volo.Abp;
using Volo.Abp.Validation;
using Xunit;

namespace ContactsSync.Application.Tests.AppServices;

public class SyncConfigAppServiceTests : ContactsSyncApplicationTestBase
{
    private readonly ISyncConfigAppService _syncConfigApplicationService;

    public SyncConfigAppServiceTests()
    {
        _syncConfigApplicationService = GetRequiredService<ISyncConfigAppService>();
    }

    [Fact]
    public async Task SaveConfig()
    {
        try
        {
            var dto = new UpdateContactsSyncConfigDto { ProviderName = OpenPlatformProviderEnum.DingDing, SyncPeriod = 10 };
            await _syncConfigApplicationService.SaveSyncConfig(dto);
        }
        catch (AbpValidationException e)
        {
            throw new UserFriendlyException(e.ValidationErrors.First().ErrorMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}