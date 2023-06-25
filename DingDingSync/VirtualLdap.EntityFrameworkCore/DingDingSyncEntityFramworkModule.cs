using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Reflection.Extensions;
using VirtualLdap.Core;

namespace VirtualLdap.EntityFrameworkCore
{
    [DependsOn(
        typeof(DingDingSyncCoreModule),
        typeof(AbpEntityFrameworkCoreModule))]
    public class DingDingSyncEntityFramworkModule : AbpModule
    {

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DingDingSyncEntityFramworkModule).GetAssembly());

        }
    }
}
