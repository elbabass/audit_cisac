using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
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

        [Function("ProcessAuditJob")]
        public async Task RunAsync(
           [TimerTrigger("%processAuditNcrontab%")] TimerInfo myTimer,
           [BlobInput("submission-audit", Connection = "ConnectionString-AzureStorage")]
            BlobContainerClient container)
        {
            await auditManager.ProcessAuditChanges(container);
        }
    }
}
