using Abp.Modules;
using Abp.Reflection.Extensions;

namespace VirtualLdap.Core
{
    public class DingDingSyncCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DingDingSyncCoreModule).GetAssembly());
        }
    }
}
