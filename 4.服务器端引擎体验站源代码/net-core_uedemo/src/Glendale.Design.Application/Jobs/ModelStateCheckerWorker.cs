using Glendale.Design.Dtos.Document;
using Glendale.Design.Dtos.DocumentVer;
using Glendale.Design.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Glendale.Design.Jobs
{
    /// <summary>
    /// 模型状态检查
    /// </summary>
    public class ModelStateCheckerWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public ModelStateCheckerWorker(AbpAsyncTimer timer, IServiceScopeFactory serviceScopeFactory) : base(timer, serviceScopeFactory)
        {
            Timer.Period = 10 * 1000; //1 分钟
        }

        protected async override Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            var documentHandleAppService = workerContext.ServiceProvider.GetRequiredService<DocumentHandleAppService>();

            await documentHandleAppService.GetQueryModelInfoAsync();

            //var planModelService = workerContext.ServiceProvider.GetRequiredService<PlanModelService>();

            //await planModelService.GetQueryModelInfoAsync();

        }
    }
}
