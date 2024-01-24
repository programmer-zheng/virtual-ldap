using ContactsSync.EntityFrameworkCore.Tests;
using Volo.Abp.Modularity;

namespace ContactsSync.Application.Tests;

[DependsOn(
    typeof(ContactsSyncApplicationModule),
    typeof(ContactsSyncEntityFrameworkCoreTestModule)
)]
public class ContactsSyncApplicationTestModule : AbpModule
{
}