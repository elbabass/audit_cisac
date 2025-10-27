using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.CosmosDb.Repositories
{
    public interface ICosmosDbRepository<T> where T : class
    {
        Task<ItemResponse<T>> UpsertItemAsync(T item);
        Task DeleteItemAsync(string id, string partitionKey);
        Task<T> GetItemAsync(string id, string partitionKey);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, int? maxResults);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, int? maxResults, string partitionKey);
        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, string? partitionKey);
        Task<ItemResponse<T>> UpdateItemAsync(string id, T item);
        FeedIterator<T> GetChangeFeedIterator(DateTime dateTimeSince);
        FeedIterator<T> GetItemsFeedIterator(Expression<Func<T, bool>> predicate);
        FeedIterator<T> GetItemsFeedIterator(Expression<Func<T, bool>> predicate, string continuationToken, int pageNumber);
    }
}
