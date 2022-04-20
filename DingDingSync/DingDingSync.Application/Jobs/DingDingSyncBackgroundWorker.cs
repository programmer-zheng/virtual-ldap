using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using DingDingSync.Application.AppService;
using System;
using Castle.Core.Logging;

namespace DingDingSync.Application.Jobs
{
    public class DingDingSyncBackgroundWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        public ILogger _logger { get; set; }

        public IUserAppService _userAppService { get; set; }


        public DingDingSyncBackgroundWorker(AbpTimer timer
        ) : base(timer)
        {
            timer.Period = 1000 * 30;
        }

        [UnitOfWork]
        protected override void DoWork()
        {
            Timer.Stop();
            _logger.Debug("同步工作开始……");
            try
            {
                _userAppService.SyncDepartmentAndUser();
            }
            catch (Exception e)
            {
                _logger.Error("同步发生异常……", e);
                _logger.Error("同步发生异常", e);
            }

            _logger.Debug("同步工作结束……");
        }
    }
}