using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using DingDingSync.Core;

namespace DingDingSync.EntityFrameworkCore
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
