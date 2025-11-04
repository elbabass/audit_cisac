using Microsoft.Azure.Functions.Worker;
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

        [Function("DatabaseMaintenanceJob")]
        public async Task RunAsync([TimerTrigger("%databaseMaintenanceNcrontab%")] TimerInfo timer)
        {
            await databaseMaintenanceManager.RunIndexOptimizeAsync();
        }
    }
}
