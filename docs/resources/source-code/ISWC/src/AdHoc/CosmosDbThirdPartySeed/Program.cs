using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Api.Agency.V1;
using SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb;
using Microsoft.Extensions.Options;
using SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb;
using CacheIswcs = SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb.Models;
using EFCore.BulkExtensions;
using CosmosDbThirdPartySeed.Models;
using Microsoft.EntityFrameworkCore;

namespace CosmosDbThirdPartySeed
{
    internal class Program
    {
        private CosmosClient cosmosClient;
        private Container cosmosContainer;
        private HttpClient httpClient;
        private CosmosDbCacheIswcService cacheIswcService;
        private static string cachedIswcsPath, uncachedIswcsPath, checkpointPath;
        private int bulkInsertBatchSize, apiBatchSize;

        public Program()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddUserSecrets<Program>()
                .Build();

            cosmosClient = new CosmosClient(config["ConnectionString-ISWCCosmosDb"]);
            cosmosContainer = cosmosClient.GetContainer("ISWC", "CacheIswcs");

            httpClient = new HttpClient
            {
                BaseAddress = new Uri(config["BaseAddress"])
            };
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", config["Subscription-Key"]);

            var options = Options.Create(new CosmosDbOptions
            {
                DatabaseId = "ISWC"
            });

            var repository = new CosmosDbRepository<CacheIswcsModel>(options, cosmosClient);

            cacheIswcService = new CosmosDbCacheIswcService(repository);
        
            cachedIswcsPath = config["CachedIswcsPath"];
            uncachedIswcsPath = config["UncachedIswcsPath"];
            checkpointPath = config["CheckpointPath"];
            bulkInsertBatchSize = config.GetValue<int>("BulkInsertBatchSize"); 
            apiBatchSize = config.GetValue<int>("ApiBatchSize");
        }

        static async Task Main(string[] args)
        {
            Program program = new Program();

            await program.WriteCachedIswcsToFileAsync();

            await program.RetrieveUncachedIswcsAsync();

            await program.CacheRemainingIswcsAsync();
        }

        private async Task WriteCachedIswcsToFileAsync()
        {
            Console.WriteLine("Writing cached ISWCs to file.");
            if (!File.Exists(cachedIswcsPath))
            {
                Console.WriteLine($"File not found. Path: {cachedIswcsPath}");
                return;
            }
            File.WriteAllText(cachedIswcsPath, string.Empty);

            string queryText = "SELECT c.id FROM c WHERE c.id LIKE 'T[0-9]%' AND LENGTH(c.id) = 11";

            using (FeedIterator<CacheIswcsModel> feedIterator = cosmosContainer.GetItemQueryIterator<CacheIswcsModel>(queryText))
            {
                using (StreamWriter writer = new StreamWriter(cachedIswcsPath, true))
                {
                    while (feedIterator.HasMoreResults)
                    {
                        foreach (var item in await feedIterator.ReadNextAsync())
                        {
                            writer.WriteLine(item.ID);
                        }
                    }
                }
            }
            Console.WriteLine("Cached ISWCs written to file.");
        }

        private async Task RetrieveUncachedIswcsAsync()
        {
            await SaveCachedIswcsInDatabaseAsync();
            await ExportIswcsNotInCacheAsync();
        }

        private async Task BackupTableAsync(string tableName)
        {
            Console.WriteLine($"Backup {tableName} table started.");
            string backupTableName = tableName + DateTime.Now.ToString("ddMMyyy");
            using (var context = new ApplicationDbContext())
            {
                await context.Database.ExecuteSqlRawAsync($@"
                    IF OBJECT_ID('{tableName}', 'U') IS NOT NULL
                        exec sp_rename '{tableName}', '{backupTableName}'
                ");
            }
            Console.WriteLine($"Backup {tableName} table complete.");
        }

        private async Task CreateTableAsync(string tableName)
        {
            Console.WriteLine($"Creating {tableName} table.");
            using (var context = new ApplicationDbContext())
            {
                await context.Database.ExecuteSqlRawAsync($@"
                    CREATE TABLE {tableName}
                    (
                        [Iswc] [nvarchar](11) NULL
                    )
                ");                
            }
            Console.WriteLine($"Created {tableName} table.");
        }

        private async Task CreateUncachedTableAsync(string tableName)
        {
            Console.WriteLine($"Creating {tableName} table.");
            using (var context = new ApplicationDbContext())
            {
                await context.Database.ExecuteSqlRawAsync($@"
                    CREATE TABLE {tableName}
                    (
                        [Iswc] [nvarchar](11) NULL,
	                    [WorkCount] [INT] NULL
                    )
                ");
            }
            Console.WriteLine($"Created {tableName} table.");
        }

        private async Task SaveCachedIswcsInDatabaseAsync()
        {
            await BackupTableAsync("CachedIswcs");
            await CreateTableAsync("CachedIswcs");

            Console.WriteLine("Saving cached ISWCs in database.");
            if (!File.Exists(cachedIswcsPath))
            {
                Console.WriteLine($"File not found. Path: {cachedIswcsPath}");
                return;
            }

            var batch = new List<CachedIswcs>();
            int recordCount = 0;

            using (var reader = new StreamReader(cachedIswcsPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    batch.Add(new CachedIswcs { Iswc = line });
                    recordCount++;

                    if (batch.Count >= bulkInsertBatchSize)
                    {
                        await BulkInsertIswcsAsync(batch);
                        batch.Clear();
                        Console.WriteLine($"Inserted {recordCount} records.");
                    }
                }
            }
            if (batch.Count > 0)
            {
                await BulkInsertIswcsAsync(batch);
                Console.WriteLine($"Inserted {recordCount} records.");
            }
            Console.WriteLine("Saving cached ISWCs in database completed.");

            async Task BulkInsertIswcsAsync(List<CachedIswcs> iswcs)
            {
                using (var context = new ApplicationDbContext())
                {
                    var bulkConfig = new BulkConfig
                    {
                        BatchSize = bulkInsertBatchSize
                    };

                    await context.BulkInsertAsync(iswcs, bulkConfig);
                }
            }
        }

        private async Task ExportIswcsNotInCacheAsync()
        {
            Console.WriteLine("Export started.");
            if (!File.Exists(uncachedIswcsPath))
            {
                Console.WriteLine($"File not found. Path: {uncachedIswcsPath}");
                return;
            }
            File.WriteAllText(uncachedIswcsPath, string.Empty);

            await BackupTableAsync("UncachedIswcs");
            await CreateUncachedTableAsync("UncachedIswcs");

            Console.WriteLine($"Insert uncached Iswcs into table started.");
            using (var context = new ApplicationDbContext())
            {
                await context.Database.ExecuteSqlRawAsync($@"
                    ;with societies as (
                        select distinct wi.AgencyID, wi.IswcID from iswc.WorkInfo wi	
                    )
                    insert into dbo.UncachedIswcs
                    select i.Iswc, count(*) WorkCount  from iswc.ISWC i
                    join societies s on i.IswcID = s.IswcID
                    where i.Iswc not in (select Iswc from dbo.CachedIswcs)
                    group by i.Iswc
                ");
            }
            Console.WriteLine($"Insert uncached Iswcs into table complete.");

            using (var context = new ApplicationDbContext())
            {
                int batchSize = 10_000_000;
                int offset = 0;
                int totalCount = await context.UncachedIswcs.CountAsync();
                int remainingCount = totalCount;

                Console.WriteLine($"Exporting {totalCount} records.");
                using (var writer = new StreamWriter(uncachedIswcsPath))
                {
                    while (remainingCount > 0)
                    {
                        var iswcsNotInCache = await context.UncachedIswcs
                            .OrderByDescending(iswc => iswc.WorkCount).ThenByDescending(iswc => iswc.Iswc)
                            .Skip(offset)
                            .Take(batchSize)
                            .Select(iswc => iswc.Iswc)
                            .ToListAsync();

                        foreach (var iswc in iswcsNotInCache)
                        {
                            await writer.WriteLineAsync(iswc);
                        }

                        offset += batchSize;
                        remainingCount -= batchSize;
                        Console.WriteLine($"{iswcsNotInCache.Count} records processed.");
                    }
                }

                Console.WriteLine($"Export completed. {totalCount} records written to {uncachedIswcsPath}");
            }
        }

        private async Task CacheRemainingIswcsAsync()
        {
            Console.WriteLine("Caching remaining ISWCs.");
            if (!File.Exists(uncachedIswcsPath))
            {
                Console.WriteLine($"File not found. Path: {uncachedIswcsPath}");
                return;
            }

            Console.WriteLine("Do you want to clear Checkpoint file (Y/N)?");
            var clearCheckpoint = Console.ReadLine();

            if (clearCheckpoint.Equals("Y", StringComparison.OrdinalIgnoreCase))
            {
                File.WriteAllText(checkpointPath, string.Empty);
                Console.WriteLine("Checkpoint file cleared.");
            }

            int lastProcessedLine = await GetLastProcessedLineAsync();

            using (var reader = new StreamReader(uncachedIswcsPath))
            {
                string line;
                int currentLine = 0;
                List<string> batch = new List<string>();

                while ((line = reader.ReadLine()) != null)
                {
                    currentLine++;

                    if (currentLine <= lastProcessedLine)
                        continue;

                    batch.Add(line);

                    if (batch.Count == apiBatchSize)
                    {
                        await PrcoessBatch(currentLine, batch);
                    }
                }
                if(batch.Count > 0)
                    await PrcoessBatch(currentLine, batch);
            }

            Console.WriteLine("Caching complete.");

            async Task PrcoessBatch(int currentLine, List<string> batch)
            {
                var searchIswcs = batch.Select(x => new { iswc = x });

                var response = await (await httpClient
                .PostAsJsonAsync("iswc/searchByIswc/batch", searchIswcs))
                .EnsureSuccessStatusCodeAsync();

                var iswcs = JsonConvert.DeserializeObject<ICollection<ISWCMetadataBatch>>(await response.Content.ReadAsStringAsync());

                IList<CacheIswcsModel> hydrateCacheIswcs = new List<CacheIswcsModel>();

                foreach (var iswc in batch)
                {
                    var iswcMetadata = iswcs.FirstOrDefault(x => x.SearchResults?.FirstOrDefault()?.Iswc == iswc)?.SearchResults.FirstOrDefault();

                    foreach (var cachedIswcModel in CacheIswcs(iswcMetadata))
                    {
                        hydrateCacheIswcs.Add(cachedIswcModel);
                    }
                }
                await cacheIswcService.UpdateCacheIswcs(hydrateCacheIswcs);

                Console.WriteLine($"Cached {batch.Count} ISWCs.");

                UpdateCheckpoint(currentLine);
                batch.Clear();
            }

            async Task<int> GetLastProcessedLineAsync()
            {
                if (File.Exists(checkpointPath))
                {
                    string lastLine = await File.ReadAllTextAsync(checkpointPath);
                    return int.TryParse(lastLine, out int lastProcessedLine) ? lastProcessedLine : 0;
                }

                return 0;
            }

            void UpdateCheckpoint(int lineNumber)
            {
                File.WriteAllText(checkpointPath, lineNumber.ToString());
            }

            static List<CacheIswcsModel> CacheIswcs(ISWCMetadata iswcMetadata)
            {
                var cacheIswcs = new List<CacheIswcsModel>();

                if (iswcMetadata != null)
                {
                    var interestedParties = new List<CacheIswcs.InterestedParty>();
                    var otherTitles = new List<CacheIswcs.Title>();

                    iswcMetadata?.OtherTitles.ForEach(x =>
                        otherTitles.Add(new CacheIswcs.Title
                        {
                            Title1 = x.Title1,
                            Type = x.Type.ToString(),
                            AdditionalProperties = x.AdditionalProperties
                        })
                    );

                    iswcMetadata?.InterestedParties.ForEach(x =>
                        interestedParties.Add(new CacheIswcs.InterestedParty
                        {
                            IPNameNumber = x.NameNumber,
                            LastName = x.LastName,
                            LegalEntityType = (SpanishPoint.Azure.Iswc.Bdo.Ipi.LegalEntityType)x.LegalEntityType,
                            Name = x.Name,
                            Role = x.Role.ToString(),
                            Affiliation = x.Affiliation
                        })
                    );

                    foreach (var work in iswcMetadata?.Works)
                    {
                        cacheIswcs.Add(new CacheIswcsModel
                        {
                            ID = string.Concat(work.Agency, work.Workcode),
                            PartitionKey = string.Concat(work.Agency, work.Workcode),
                            IswcMetadata = new CacheIswcMetadata
                            {
                                Iswc = iswcMetadata.Iswc,
                                ParentISWC = iswcMetadata.ParentISWC,
                                OverallParentISWC = iswcMetadata.OverallParentISWC,
                                OtherTitles = otherTitles,
                                Agency = iswcMetadata.Agency,
                                CreatedDate = iswcMetadata.CreatedDate,
                                InterestedParties = interestedParties,
                                LastModifiedDate = iswcMetadata.LastModifiedDate,
                                LinkedISWC = iswcMetadata.LinkedISWC,
                                OriginalTitle = iswcMetadata.OriginalTitle
                            }
                        });
                    }

                    cacheIswcs.Add(new CacheIswcsModel
                    {
                        ID = iswcMetadata.Iswc,
                        PartitionKey = iswcMetadata.Iswc,
                        IswcMetadata = new CacheIswcMetadata
                        {
                            Iswc = iswcMetadata.Iswc,
                            ParentISWC = iswcMetadata.ParentISWC,
                            OverallParentISWC = iswcMetadata.OverallParentISWC,
                            OtherTitles = otherTitles,
                            Agency = iswcMetadata.Agency,
                            CreatedDate = iswcMetadata.CreatedDate,
                            InterestedParties = interestedParties,
                            LastModifiedDate = iswcMetadata.LastModifiedDate,
                            LinkedISWC = iswcMetadata.LinkedISWC,
                            OriginalTitle = iswcMetadata.OriginalTitle
                        }
                    });
                }

                return cacheIswcs;
            }
        }
    }
}
