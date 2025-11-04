using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SpanishPoint.Azure.Iswc.Business.Managers;

namespace SpanishPoint.Azure.Iswc.Jobs.Functions
{
    internal class ProcessAgencyStatisticsJob
    {
        private readonly IAuditManager auditManager;

        public ProcessAgencyStatisticsJob(IAuditManager auditManager)
        {
            this.auditManager = auditManager;
        }

        [Function("ProcessAgencyStatisticsJob")]
        public async Task Run([TimerTrigger("%processAgencyStatisticsNcrontab%")] TimerInfo myTimer)
        {
            await auditManager.GenerateAgencyStatistics();
        }
    }
}
