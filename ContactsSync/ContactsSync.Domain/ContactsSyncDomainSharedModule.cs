using ContactsSync.Domain.Shared;
using Volo.Abp.Modularity;

namespace ContactsSync.Domain;

[DependsOn(
    typeof(ContactsSyncDomainSharedModule),
    typeof(ContactsSyncDomainSharedModule)
)]
public class ContactsSyncDomainModule : AbpModule
{
}