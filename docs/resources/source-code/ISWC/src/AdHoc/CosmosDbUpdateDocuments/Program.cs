using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;

namespace CosmosDbUpdateDocuments
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var conn = "";
            var client = new CosmosClient(conn);
            var container = client.GetContainer("ISWC", "CsnNotifications");

            string queryText = "SELECT * FROM c";

            int count = 0;

            using (FeedIterator<CsnNotification> feedIterator = container.GetItemQueryIterator<CsnNotification>(queryText))
            {
                while (feedIterator.HasMoreResults)
                {
                    foreach (var item in await feedIterator.ReadNextAsync())
                    {
                        //item.ProcessedOnDate = null;
                        await container.UpsertItemAsync(item);
                        count++;
                    }
                }
            }

            Console.WriteLine($"{count} documents affected.");
        }
    }
}
