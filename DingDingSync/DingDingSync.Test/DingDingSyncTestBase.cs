using Abp.Dependency;
using Abp.TestBase;
using DingDingSync.EntityFrameworkCore;

namespace DingDingSync.Test;

public abstract class DingDingSyncTestBase : AbpIntegratedTestBase<DingDingSyncTestModule>
{
    protected DingDingSyncTestBase()
    {
        // UsingDbContext(t => { });
    }

    protected override void PreInitialize()
    {
        base.PreInitialize();
    }

    public void UsingDbContext(Action<DingDingSyncDbContext> action)
    {
        using (var context = LocalIocManager.Resolve<DingDingSyncDbContext>())
        {
            action(context);
            context.SaveChanges();
        }
    }
}