using Abp.TestBase;
using VirtualLdap.EntityFrameworkCore;

namespace VirtualLdap.Test;

public class VirtualLdapTestBase : AbpIntegratedTestBase<VirtualLdapTestModule>
{
    public VirtualLdapTestBase()
    {
        UsingDbContext(context =>
        {
            new TestDataBuilder(context).Build();
        });
    }

    protected virtual void UsingDbContext(Action<VirtualLdapDbContext> action)
    {
        using (var context = LocalIocManager.Resolve<VirtualLdapDbContext>())
        {
            action(context);
            context.SaveChanges();
        }
    }

    protected virtual T UsingDbContext<T>(Func<VirtualLdapDbContext, T> func)
    {
        T result;

        using (var context = LocalIocManager.Resolve<VirtualLdapDbContext>())
        {
            result = func(context);
            context.SaveChanges();
        }

        return result;
    }

    protected virtual async Task UsingDbContextAsync(Func<VirtualLdapDbContext, Task> action)
    {
        using (var context = LocalIocManager.Resolve<VirtualLdapDbContext>())
        {
            await action(context);
            await context.SaveChangesAsync(true);
        }
    }

    protected virtual async Task<T> UsingDbContextAsync<T>(Func<VirtualLdapDbContext, Task<T>> func)
    {
        T result;

        using (var context = LocalIocManager.Resolve<VirtualLdapDbContext>())
        {
            result = await func(context);
            context.SaveChanges();
        }

        return result;
    }
}