using Microsoft.EntityFrameworkCore;
using SpanishPoint.Azure.Iswc.Data.DataModels.Context;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Repositories
{
    public interface IDatabaseMaintenanceRepository
    {
        Task RunIndexOptimizeAsync();
    }

    internal class DatabaseMaintenanceRepository : IDatabaseMaintenanceRepository
    {
        private readonly CsiContextMaintenance context;

        public DatabaseMaintenanceRepository(CsiContextMaintenance context)
        {
            this.context = context;
        }

        public async Task RunIndexOptimizeAsync() => 
            await context.Database.ExecuteSqlRawAsync(@"
                EXECUTE dbo.IndexOptimize @Databases = 'USER_DATABASES',
                    @FragmentationMedium = 'INDEX_REORGANIZE,INDEX_REBUILD_ONLINE',
                    @FragmentationHigh = 'INDEX_REBUILD_ONLINE',
                    @FragmentationLevel1 = 40,
	                @FragmentationLevel2 = 80,
                    @UpdateStatistics = 'ALL',
                    @OnlyModifiedStatistics = 'Y',
                    @LogToTable='Y'
            ");
    }
}
