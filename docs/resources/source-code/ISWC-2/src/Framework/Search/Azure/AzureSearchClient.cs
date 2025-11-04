using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Framework.Search.Azure
{
    [ExcludeFromCodeCoverage]
    public class AzureSearchClient<TOptions> : ISearchClient<TOptions> where TOptions : AzureSearchOptions, new()
    {
        private readonly ISearchIndexClient searchIndexClient = default!;

        public AzureSearchClient(IOptions<TOptions> options)
        {
            if (options.Value.SearchServiceName != null)
                searchIndexClient = new SearchIndexClient(options.Value.SearchServiceName, options.Value.IndexName, new SearchCredentials(options.Value.ApiKey));
        }


        public async Task Delete(string filterPredicate)
        {
            bool finished = false;

            while (!finished)
            {
                var documentsToDelete = await searchIndexClient.Documents.SearchAsync("*", new SearchParameters
                {
                    Filter = filterPredicate,
                    Top = 1000
                });

                finished = documentsToDelete.Results.Count == 0;

                if (!finished)
                {
                    var batch = new IndexBatch<Document>(documentsToDelete.Results.Select(x => IndexAction.Delete(x.Document)));
                    searchIndexClient.Documents.Index(batch);
                }
            }
        }


        public async Task<IEnumerable<T>> SearchAsync<T>(string searchText, string filterPredicate, IList<string> searchFields)
        {
            var searchResults = await searchIndexClient.Documents.SearchAsync<T>(searchText, new SearchParameters
            {
                Filter = filterPredicate,
                Top = 1000,
                SearchFields = searchFields
            });

            return searchResults.Results.Select(x => x.Document);
        }

        public async Task UploadAsync<T>(IEnumerable<T> t)
        {
            if (t != null)
            {
                var actions = new List<IndexAction<T>>();

                foreach (var item in t)
                {
                    actions.Add(IndexAction.Upload(item));
                };

                var batch = IndexBatch.New(actions.ToArray());

                await searchIndexClient.Documents.IndexAsync(batch);
            }
        }

        public async Task MergeOrUploadAsync<T>(IEnumerable<T> t)
        {
            if (t != null)
            {
                var actions = new List<IndexAction<T>>();

                foreach (var item in t)
                {
                    actions.Add(IndexAction.MergeOrUpload(item));
                };

                var batch = IndexBatch.New(actions.ToArray());

                await searchIndexClient.Documents.IndexAsync(batch);
            }
        }
    }
}