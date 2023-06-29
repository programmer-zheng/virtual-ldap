using Abp.Modules;
using Abp.Reflection.Extensions;

namespace VirtualLdap.Core
{
    public class VirtualLdapCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(VirtualLdapCoreModule).GetAssembly());
        }
    }
}
