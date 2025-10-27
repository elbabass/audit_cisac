using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.Caching
{
    public interface ICacheClient
    {
        Task<TItem> GetOrCreateAsync<TItem>(Func<Task<TItem>> item, [CallerFilePath] string type = "", [CallerMemberName] string method = "", params string[] keys);
    }
}
