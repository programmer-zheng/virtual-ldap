using Volo.Abp.Application;
using Volo.Abp.Modularity;

namespace ContactsSync.Application.Contracts;

[DependsOn(
    typeof(AbpDddApplicationContractsModule)
)]
public class ContactsSyncApplicationContractsModule : AbpModule
{
}