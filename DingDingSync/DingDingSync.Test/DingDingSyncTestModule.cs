using Abp.Modules;
using Abp.TestBase;
using DingDingSync.Application;
using DingDingSync.EntityFrameworkCore;

namespace DingDingSync.Test;

[DependsOn(typeof(DingDingSyncApplicationModule),
    typeof(DingDingSyncEntityFramworkModule),
    typeof(AbpTestBaseModule))]
public class DingDingSyncTestModule : AbpModule
{
    public override void PreInitialize()
    {
        
        Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(30);
        Configuration.UnitOfWork.IsTransactional = false;
        base.PreInitialize();
    }

    public override void Initialize()
    {
        //IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        ServiceCollectionRegistrar.Register(IocManager);
    }
    
}
