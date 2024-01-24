using ContactsSync.Domain.Shared;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement;

namespace ContactsSync.Domain;

[DependsOn(
    typeof(AbpSettingManagementDomainModule),
    typeof(ContactsSyncDomainSharedModule)
)]
public class ContactsSyncDomainModule : AbpModule
{
}