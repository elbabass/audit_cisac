using SpanishPoint.Azure.Iswc.Data.Repositories;
using SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.UpdateWorkflowHistory.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb
{
    public class CosmosDbCacheIswcService : ICacheIswcService
    {
        private readonly ICosmosDbRepository<CacheIswcsModel> cacheIswcsContainer;

        public CosmosDbCacheIswcService(ICosmosDbRepository<CacheIswcsModel> cacheIswcsContainer)
        {
            this.cacheIswcsContainer = cacheIswcsContainer;
        }

        public async Task AddCacheIswc(CacheIswcsModel CacheIswc)
        {
            await cacheIswcsContainer.UpsertItemAsync(CacheIswc);
        }

        public async Task UpdateCacheIswcs(IEnumerable<CacheIswcsModel> cacheIswcs)
        {
            foreach (var item in cacheIswcs)
            {
                await cacheIswcsContainer.UpsertItemAsync(item);
            }
        }

        public async Task<CacheIswcsModel> GetCacheIswcs(string id)
            => await cacheIswcsContainer.GetItemAsync(id, id);
    }
}
