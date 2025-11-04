using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using SpanishPoint.Azure.Iswc.Business.Managers;
using System;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Jobs.Functions
{
    internal class UpdateWorkflowsJob
    {
        private readonly IUpdateWorkflowsManager updateWorkflowsManager;
        private readonly IConfiguration configuration;

        public UpdateWorkflowsJob(IUpdateWorkflowsManager updateWorkflowsManager, IConfiguration configuration)
        {
            this.updateWorkflowsManager = updateWorkflowsManager;
            this.configuration = configuration;
        }

        [Function("UpdateWorkflowsJob")]
        public async Task RunAsync([TimerTrigger("%updateWorkflowsNcrontab%")] TimerInfo timer)
        {
            var config = configuration["UpdateWorkflows-MinimumAgeInDays"];

            if (!int.TryParse(config, out int minimumAgeInDays))
                throw new ArgumentException($"MinimumAgeInDays value is invalid:{config}");

            await updateWorkflowsManager.CompleteWorkflows(minimumAgeInDays);
        }
    }
}
