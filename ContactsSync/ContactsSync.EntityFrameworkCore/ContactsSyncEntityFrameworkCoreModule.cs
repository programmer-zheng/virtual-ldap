using ContactsSync.Domain;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Modularity;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace ContactsSync.EntityFrameworkCore
{
    [DependsOn(
        typeof(ContactsSyncDomainModule),
        typeof(AbpEntityFrameworkCoreMySQLModule),
        typeof(AbpSettingManagementEntityFrameworkCoreModule)
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