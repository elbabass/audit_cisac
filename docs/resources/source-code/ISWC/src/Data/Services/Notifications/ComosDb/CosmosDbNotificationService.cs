using LinqKit;
using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Services.Audit.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications.CosmosDb.Models;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Repositories;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.Notifications.ComosDb
{
    public class CosmosDbNotificationService : INotificationService
    {
        private readonly ICosmosDbRepository<CsnNotifications> csnRepository;
        private readonly ICosmosDbRepository<CsnNotificationsHighWatermark> csnHighwatermarkRepository;


        public CosmosDbNotificationService(ICosmosDbRepository<CsnNotifications> repository, ICosmosDbRepository<CsnNotificationsHighWatermark> csnHighwatermarkRepository)
        {
            this.csnRepository = repository;
            this.csnHighwatermarkRepository = csnHighwatermarkRepository;
        }

        public async Task AddCsnNotifications(IEnumerable<CsnNotifications> csnRecords, WorkflowInstance workflowInstance = null)
        {
            string workflowMessages = workflowInstance != null ? GetWorkflowMessages(workflowInstance) : null;
            foreach (var csnRecord in csnRecords)
            {
                if (workflowInstance != null)
                {
                    foreach (var workflow in workflowInstance.WorkflowTask)
                    {
                        if (csnRecord.ReceivingAgencyCode == workflow.AssignedAgencyId)
                        {
                            csnRecord.WorkflowTaskID = workflow.WorkflowTaskId.ToString();
                            csnRecord.WorkflowStatus = workflow.TaskStatus.ToString();
                        }
                    }
                    csnRecord.WorkflowMessage = workflowMessages;
                }
                await csnRepository.UpsertItemAsync(csnRecord);
            }
        }

        public Task<IEnumerable<CsnNotifications>> GetCsnNotifications(int count)
           => csnRepository.GetItemsAsync(x => x.ProcessedOnDate == null && x.HttpResponse == null, count);

        public Task<IEnumerable<CsnNotifications>> GetCsnNotifications(int count, string partitionKey )
            => csnRepository.GetItemsAsync(x => x.ProcessedOnDate == null && x.HttpResponse == null, count, partitionKey);

        public async Task UpdateCsnNotifications(IEnumerable<CsnNotifications> csnNotifications)
        {
            foreach (var item in csnNotifications)
            {
                await csnRepository.UpsertItemAsync(item);
            }
        }

        public async Task<(string continuationToken, IEnumerable<CsnNotifications>)> GetCsnNotifications(string agency, TransactionType transactionType, DateTime fromDate, int pageNumber, string continuationToken)
        {
            var predicate = GetPredicate();
            var continuationTokenObject = new ContinuationToken();

            if (!string.IsNullOrWhiteSpace(continuationToken))
                continuationToken = new ContinuationToken(continuationToken).Serialize();

            var feedIterator = csnRepository.GetItemsFeedIterator(predicate, continuationToken, pageNumber);

            var results = new List<CsnNotifications>();
            while (feedIterator.HasMoreResults && results.Count() < pageNumber)
            {
                var response = await feedIterator.ReadNextAsync();

                continuationToken = continuationTokenObject.Deserialize(response.ContinuationToken);

                results.AddRange(response.Resource);
            }

            return (continuationToken, results);

            ExpressionStarter<CsnNotifications> GetPredicate()
            {
                var predicate = PredicateBuilder.New<CsnNotifications>();

                predicate.Start(x => x.HttpResponse != null && x.TransactionType == transactionType && x.ProcessingDate >= fromDate && x.ReceivingAgencyCode == agency);
                return predicate;
            }
        }

        private string GetWorkflowMessages(WorkflowInstance workflowInstance = null)
        {
            var workflow = workflowInstance.WorkflowTask.FirstOrDefault(x => !string.IsNullOrEmpty(x.Message));
            return workflow?.Message;
        }

        public async Task<CsnNotificationsHighWatermark> GetCsnNotificationHighWatermark(string agency)
        {
            return await csnHighwatermarkRepository.GetItemAsync(agency, agency);
        }

        public async Task UpdateCsnNotificationHighWatermark(CsnNotificationsHighWatermark item)
        {
            await csnHighwatermarkRepository.UpsertItemAsync(item);
        }
    }
}

