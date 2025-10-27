using SpanishPoint.Azure.Iswc.Framework.Search.Azure;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SpanishPoint.Azure.Iswc.Framework.Search
{
    public interface ISearchClient<TOptions> where TOptions : AzureSearchOptions, new()
    {
        Task Delete(string filterPredicate);
        Task<IEnumerable<T>> SearchAsync<T>(string searchText, string filterPredicate, IList<string> searchFields);
        Task UploadAsync<T>(IEnumerable<T> t);
        Task MergeOrUploadAsync<T>(IEnumerable<T> t);
    }
}

