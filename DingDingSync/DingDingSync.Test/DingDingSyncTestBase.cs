using Abp.TestBase;
using DingDingSync.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DingDingSync.Test;

public class DingDingSyncTestBase : AbpIntegratedTestBase<DingDingSyncTestModule>
{
    protected DingDingSyncTestBase()
    {
        UsingDbContext(context =>
        {
            new SeedDataInit(context).Init();
        });
    }

    protected virtual void UsingDbContext(Action<DingDingSyncDbContext> action)
    {
        using (var context = LocalIocManager.Resolve<DingDingSyncDbContext>())
        {
            action(context);
            context.SaveChanges();
        }
    }

    protected virtual T UsingDbContext<T>(Func<DingDingSyncDbContext, T> func)
    {
        T result;

        using (var context = LocalIocManager.Resolve<DingDingSyncDbContext>())
        {
            result = func(context);
            context.SaveChanges();
        }

        return result;
    }

    protected virtual async Task UsingDbContextAsync(Func<DingDingSyncDbContext, Task> action)
    {
        using (var context = LocalIocManager.Resolve<DingDingSyncDbContext>())
        {
            await action(context);
            await context.SaveChangesAsync(true);
        }
    }

    protected virtual async Task<T> UsingDbContextAsync<T>(Func<DingDingSyncDbContext, Task<T>> func)
    {
        T result;

        using (var context = LocalIocManager.Resolve<DingDingSyncDbContext>())
        {
            result = await func(context);
            context.SaveChanges();
        }

        return result;
    }
}