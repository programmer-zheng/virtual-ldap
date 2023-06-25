using Abp.EntityFrameworkCore;
using Abp.Modules;
using Abp.Reflection.Extensions;
using VirtualLdap.Core;

namespace VirtualLdap.EntityFrameworkCore
{
    [DependsOn(
        typeof(VirtualLdapCoreModule),
        typeof(AbpEntityFrameworkCoreModule))]
    public class VirtualLdapEntityFramworkModule : AbpModule
    {

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(VirtualLdapEntityFramworkModule).GetAssembly());

        }
    }
}
