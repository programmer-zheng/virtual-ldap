using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using AutoMapper;
using DingDingSync.Application.Jobs;
using DingDingSync.Core;

namespace DingDingSync.Application
{
    [DependsOn(
        typeof(DingDingSyncCoreModule),
        typeof(AbpAutoMapperModule))]
    public class DingDingSyncApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(DingDingSyncApplicationModule).GetAssembly());

            Configuration.Modules.AbpAutoMapper().Configurators
                .Add(t => t.AddMaps(typeof(DingDingSyncApplicationModule)));
        }

        public override void PostInitialize()
        {
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<DingDingSyncBackgroundWorker>());
        }
    }
}