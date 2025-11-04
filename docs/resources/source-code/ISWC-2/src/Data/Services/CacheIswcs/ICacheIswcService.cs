using SpanishPoint.Azure.Iswc.Data.Services.Checksum.CosmosDb.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb.Models;

namespace SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs
{
    public interface ICacheIswcService
    {
        Task AddCacheIswc(CacheIswcsModel checksum);
        Task<CacheIswcsModel> GetCacheIswcs(string iswc);
        Task UpdateCacheIswcs(IEnumerable<CacheIswcsModel> cacheIswcs);
    }
}
