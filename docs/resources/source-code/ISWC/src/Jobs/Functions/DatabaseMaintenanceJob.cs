using Microsoft.Azure.WebJobs;
using SpanishPoint.Azure.Iswc.Business.Managers;
using System.Threading.Tasks;


namespace SpanishPoint.Azure.Iswc.Jobs.Functions
{
    internal class DatabaseMaintenanceJob
    {
        private readonly IDatabaseMaintenanceManager databaseMaintenanceManager;

        public DatabaseMaintenanceJob(IDatabaseMaintenanceManager databaseMaintenanceManager)
        {
            this.databaseMaintenanceManager = databaseMaintenanceManager;
        }

        [Timeout("06:00:00")]
        [FunctionName("DatabaseMaintenanceJob")]
        public async Task RunAsync([TimerTrigger("%databaseMaintenanceNcrontab%")] TimerInfo timer)
        {
            await databaseMaintenanceManager.RunIndexOptimizeAsync();
        }
    }
}
