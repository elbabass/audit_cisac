using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Data.Services.Notifications.CosmosDb.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpanishPoint.Azure.Iswc.Data.Services.Notifications
{
    public interface INotificationService
    {
        Task AddCsnNotifications(IEnumerable<CsnNotifications> csnRecords, WorkflowInstance workflowInstance);
        Task<IEnumerable<CsnNotifications>> GetCsnNotifications(int count);
        Task<IEnumerable<CsnNotifications>> GetCsnNotifications(int count, string paritionKey);
        Task<CsnNotificationsHighWatermark> GetCsnNotificationHighWatermark(string agency);
        Task UpdateCsnNotificationHighWatermark(CsnNotificationsHighWatermark csnNotificationsHighWatermark);
        Task UpdateCsnNotifications(IEnumerable<CsnNotifications> csnNotifications);
        Task<(string continuationToken, IEnumerable<CsnNotifications>)> GetCsnNotifications(string agency, TransactionType transactionType, DateTime fromDate, int pageNumber, string continuationToken);
    }
}
