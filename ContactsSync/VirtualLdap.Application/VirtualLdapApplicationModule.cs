using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using VirtualLdap.Application.Jobs;
using VirtualLdap.Core;

namespace VirtualLdap.Application
{
    [DependsOn(
        typeof(VirtualLdapCoreModule),
        typeof(AbpAutoMapperModule))]
    public class VirtualLdapApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(VirtualLdapApplicationModule).GetAssembly());

            Configuration.Modules.AbpAutoMapper().Configurators
                .Add(t => t.AddMaps(typeof(VirtualLdapApplicationModule)));
        }

        public override void PostInitialize()
        {
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<VirtualLdapBackgroundWorker>());
        }
    }
}