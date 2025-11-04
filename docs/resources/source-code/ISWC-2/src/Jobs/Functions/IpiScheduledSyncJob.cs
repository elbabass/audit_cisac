using Microsoft.Azure.Functions.Worker;
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
        private readonly ILogger<IpiScheduledSync> logger;

        public IpiScheduledSync(ISynchronisationManager synchronisationManager, IConfiguration configuration, ILogger<IpiScheduledSync> logger)
        {
            this.synchronisationManager = synchronisationManager;
            this.configuration = configuration;
            this.logger = logger;
        }

        [Function("IpiScheduledSyncJob")]
        public async Task RunAsync([TimerTrigger("%ipiScheduledSyncNcrontab%")] TimerInfo timer)
        {
            var config = configuration["Ipi-ScheduledSyncBatchSize"];

            if (!int.TryParse(config, out int scheduledSyncBatchSize))
                throw new ArgumentException($"ScheduledSyncBatchSize value is invalid:{config}");

            await synchronisationManager.IpiScheduledSync(scheduledSyncBatchSize, logger);
        }
    }
}
