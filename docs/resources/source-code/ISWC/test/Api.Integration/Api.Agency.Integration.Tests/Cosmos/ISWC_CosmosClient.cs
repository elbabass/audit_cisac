using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Linq;
using System.Threading.Tasks;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.Cosmos.Models;
using System.Collections.Generic;
using SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.Configuration;

namespace SpanishPoint.Azure.Iswc.Api.Agency.Integration.Tests.Cosmos
{

    public class TransactionTypes
    {
        public static string CAR = "CAR";
        public static string CUR = "CUR";
        public static string MER = "MER";
        public static string DMR = "DMR";
        public static string CDR = "CDR";
        public static string FSQ = "FSQ";
    }

    public class ISWC_CosmosClient
    {
        private readonly CosmosClient cosmosClient;

        public ISWC_CosmosClient()
        {
            var config = ConfigurationHelper.GetConfig();
            this.cosmosClient = new CosmosClient(config.CosmosConnectionString);
        }

        public async Task<CSN> GetCsnEntry(string workcode, string transactionType)
        {
            var container = cosmosClient.GetContainer("ISWC", "CsnNotifications");
            using FeedIterator<CSN> setIterator = container.GetItemLinqQueryable<CSN>()
                      .Where(c => c.ReceivingAgencyWorkCode == workcode && c.TransactionType == transactionType)
                      .ToFeedIterator();
            while (setIterator.HasMoreResults)
            {
                var csn = (await setIterator.ReadNextAsync()).FirstOrDefault();
                if (csn != null)
                {
                    return csn;
                }
            }
            return null;
        }

        public async Task<AuditRequestModel> GetAuditRequestRecord(string workcode, string transactionType)
        {
            var container = cosmosClient.GetContainer("ISWC", "AuditRequest");
            using FeedIterator<AuditRequestModel> setIterator = container.GetItemLinqQueryable<AuditRequestModel>()
                      .Where(c => c.Work.WorkNumber.Number == workcode && c.TransactionType == transactionType)
                      .ToFeedIterator();
            while (setIterator.HasMoreResults)
            {
                var record = (await setIterator.ReadNextAsync()).FirstOrDefault();
                if (record != null && record.AgencyCode != null)
                {
                    return record;
                }
            }
            return null;
        }

        public async Task<IList<AuditRequestModel>> GetAuditRequestRecords(string workcode, string transactionType)
        {
            var container = cosmosClient.GetContainer("ISWC", "AuditRequest");
            var results = new List<AuditRequestModel>();
            using FeedIterator<AuditRequestModel> setIterator = container.GetItemLinqQueryable<AuditRequestModel>()
                      .Where(c => c.Work.WorkNumber.Number == workcode && c.TransactionType == transactionType)
                      .ToFeedIterator();
            while (setIterator.HasMoreResults)
            {
                var records = (await setIterator.ReadNextAsync());
                foreach (var record in records)
                {
                    if (record != null && record.AgencyCode != null)
                    {
                        results.Add(record);
                    }
                }
            }
            return results;
        }

        public async Task<AgentRunDocument> GetAgentRunRecord(string runId)
        {
            var container = cosmosClient.GetContainer("ISWC", "AgentRuns");
            using FeedIterator<AgentRunDocument> setIterator = container.GetItemLinqQueryable<AgentRunDocument>()
                      .Where(c => c.RunId == runId) 
                      .ToFeedIterator();
            while (setIterator.HasMoreResults)
            {
                var record = (await setIterator.ReadNextAsync()).FirstOrDefault();
                if (record != null && record.AgencyCode != null)
                {
                    return record;
                }
            }
            return null;

        }
    }
}
