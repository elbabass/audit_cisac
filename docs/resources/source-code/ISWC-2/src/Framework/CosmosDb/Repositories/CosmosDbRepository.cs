using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Options;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.CosmosDb.Repositories
{
    [ExcludeFromCodeCoverage]
    public class CosmosDbRepository<T> : ICosmosDbRepository<T> where T : BaseModel, new()
    {
        private readonly Container container;

        public CosmosDbRepository(IOptions<CosmosDbOptions> options, CosmosClient client)
        {
            container = client.GetContainer(options.Value.DatabaseId, containerId: typeof(T).Name.Trim("Model".ToArray()));
        }

        public async Task<T> GetItemAsync(string id, string partitionKey)
        {
            try
            {
                var resource = await container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
                resource.Resource.ETag = resource.ETag;
                return resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return default!;
            }
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, string? partitionKey)
        {
            QueryRequestOptions options = new QueryRequestOptions();

            if (!string.IsNullOrWhiteSpace(partitionKey))
                options.PartitionKey = new PartitionKey(partitionKey);

            var results = new List<T>();

            var feedIterator = container.GetItemLinqQueryable<T>(false, null, options).Where(predicate).ToFeedIterator();
            while (feedIterator.HasMoreResults)
            {
                foreach (var item in await feedIterator.ReadNextAsync())
                {
                    results.Add(item);
                }
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, int? maxResults)
        {
            var results = new List<T>();

            var feedIterator = container.GetItemLinqQueryable<T>().Where(predicate).ToFeedIterator();
            while (feedIterator.HasMoreResults && results.Count < maxResults)
            {
                foreach (var item in await feedIterator.ReadNextAsync())
                {
                    if (results.Count == maxResults)
                        break;

                    results.Add(item);
                }
            }

            return results;
        }

        public async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate, int? maxResults, string partitionKey)
        {
            QueryRequestOptions options = new QueryRequestOptions();

            if (!string.IsNullOrWhiteSpace(partitionKey))
                options.PartitionKey = new PartitionKey(partitionKey);

            var results = new List<T>();

            var feedIterator = container.GetItemLinqQueryable<T>(false, null, options).Where(predicate).ToFeedIterator();
            while (feedIterator.HasMoreResults && results.Count < maxResults)
            {
                foreach (var item in await feedIterator.ReadNextAsync())
                {
                    if (results.Count == maxResults)
                        break;

                    results.Add(item);
                }
            }

            return results;
        }

        public FeedIterator<T> GetItemsFeedIterator(Expression<Func<T, bool>> predicate) 
            => container.GetItemLinqQueryable<T>().Where(predicate).ToFeedIterator();

        public FeedIterator<T> GetItemsFeedIterator(Expression<Func<T, bool>> predicate, string continuationToken, int pageNumber) 
            => container.GetItemLinqQueryable<T>(false, continuationToken, new QueryRequestOptions { MaxItemCount = pageNumber }).Where(predicate).ToFeedIterator();

        public async Task<ItemResponse<T>> UpsertItemAsync(T item)
        {
            return await container.UpsertItemAsync(item);
        }

        public async Task<ItemResponse<T>> UpdateItemAsync(string id, T item)
        {
            return await container.ReplaceItemAsync(item, id, requestOptions: new ItemRequestOptions
            {
                IfMatchEtag = item.ETag
            });
        }

        public async Task DeleteItemAsync(string id, string partitionKey)
        {
            await container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
        }

        public FeedIterator<T> GetChangeFeedIterator(DateTime dateTimeSince)
        {
            return container.GetChangeFeedIterator<T>(ChangeFeedStartFrom.Time(dateTimeSince), ChangeFeedMode.Incremental);
        }
    }
}
