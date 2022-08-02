using System;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using DingDingSync.Application.DingDingUtils;
using DingDingSync.Application.IKuai;
using DingDingSync.Application.Jobs;
using DingDingSync.Application.Jobs.EventInfo;
using DingDingSync.Core.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Castle.Core.Logging;
using DingDingSync.Application.AppService;

namespace DingDingSync.Application.Jobs.EventHandler
{
    /// <summary>
    /// 企业被解散
    /// </summary>
    public class OrgRemoveEventHandler : DingdingBaseEventHandler
    {
        private readonly ILogger _logger;

        public OrgRemoveEventHandler(ILogger logger)
        {
            _logger = logger;
        }

        public override void Do(string msg)
        {
            var classname = GetType().Name;
            var eventinfo = JsonConvert.DeserializeObject<OrgRemoveEvent>(msg);
            _logger.Info("企业解散...");
            _logger.Info($"{classname}:{string.Join(",", eventinfo.ID)}");
        }
    }
}