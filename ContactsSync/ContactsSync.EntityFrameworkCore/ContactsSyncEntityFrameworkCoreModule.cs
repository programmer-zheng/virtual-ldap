using ContactsSync.Domain;
using ContactsSync.Domain.Shared;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Modularity;

namespace ContactsSync.EntityFrameworkCore
{
    [DependsOn(
        typeof(ContactsSyncDomainModule),
        typeof(AbpEntityFrameworkCoreMySQLModule)
        )]
    public class ContactsSyncEntityFrameworkCoreModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<ContactsSyncDbContext>(options =>
            {
                // 注入所有实体的仓储
                options.AddDefaultRepositories(includeAllEntities: true);
            });
            // 配置使用MySQL
            Configure<AbpDbContextOptions>(options => options.UseMySQL());
        }
    }
}