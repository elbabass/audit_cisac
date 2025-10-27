using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using SpanishPoint.Azure.Iswc.Business.Managers;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Jobs.Functions
{
    internal class ProcessAuditJob
    {
        private readonly IAuditManager auditManager;

        public ProcessAuditJob(IAuditManager auditManager)
        {
            this.auditManager = auditManager;
        }

        [Timeout("12:00:00")]
        [FunctionName("ProcessAuditJob")]
        public async Task RunAsync(
            [TimerTrigger("%processAuditNcrontab%")] TimerInfo myTimer,
            [Blob("submission-audit", Connection = "ConnectionString-AzureStorage")] CloudBlobContainer container)
        {
            await auditManager.ProcessAuditChanges(container);
        }
    }
}
