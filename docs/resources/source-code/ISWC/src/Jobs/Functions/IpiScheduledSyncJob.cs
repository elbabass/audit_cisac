using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SpanishPoint.Azure.Iswc.Business.Managers;
using System;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Jobs.Functions
{
    internal class IpiScheduledSync
    {
        private readonly ISynchronisationManager synchronisationManager;
        private readonly IConfiguration configuration;

        public IpiScheduledSync(ISynchronisationManager synchronisationManager, IConfiguration configuration)
        {
            this.synchronisationManager = synchronisationManager;
            this.configuration = configuration;
        }

        [FunctionName("IpiScheduledSyncJob")]
        public async Task RunAsync([TimerTrigger("%ipiScheduledSyncNcrontab%")] TimerInfo timer, ILogger logger)
        {
            var config = configuration["Ipi-ScheduledSyncBatchSize"];

            if (!int.TryParse(config, out int scheduledSyncBatchSize))
                throw new ArgumentException($"ScheduledSyncBatchSize value is invalid:{config}");

            await synchronisationManager.IpiScheduledSync(scheduledSyncBatchSize, logger);
        }
    }
}
