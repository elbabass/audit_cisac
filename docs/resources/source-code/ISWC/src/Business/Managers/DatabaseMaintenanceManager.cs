using SpanishPoint.Azure.Iswc.Data.Repositories;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Business.Managers
{
    public interface IDatabaseMaintenanceManager
    {
        Task RunIndexOptimizeAsync();
    }

    internal class DatabaseMaintenanceManager : IDatabaseMaintenanceManager
    {
        private readonly IDatabaseMaintenanceRepository repository;

        public DatabaseMaintenanceManager(IDatabaseMaintenanceRepository repository)
        {
            this.repository = repository;
        }

        public async Task RunIndexOptimizeAsync()
            => await repository.RunIndexOptimizeAsync();
    }
}
