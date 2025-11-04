using LazyCache;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.Caching.InMemory
{
    [ExcludeFromCodeCoverage]
    public class InMemoryCacheClient : ICacheClient
    {
        private readonly IAppCache appCache;

        public InMemoryCacheClient(IAppCache appCache)
        {
            this.appCache = appCache;
        }

        public async Task<TItem> GetOrCreateAsync<TItem>(Func<Task<TItem>> item, [CallerFilePath] string type = "", [CallerMemberName] string method = "", params string[] keys)
        {
            var key = $"{Path.GetFileNameWithoutExtension(type)}_{method}_{string.Join(',', keys)}";
            return await appCache.GetOrAddAsync(key, a =>
           {
               a.SlidingExpiration = TimeSpan.FromHours(1);
               return item();
           });
        }
    }
}
