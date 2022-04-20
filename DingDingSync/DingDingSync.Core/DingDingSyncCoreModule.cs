using Abp.Modules;
using Abp.Reflection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DingDingSync.Core
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
