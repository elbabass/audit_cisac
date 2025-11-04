using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using SpanishPoint.Azure.Iswc.Data.DataModels;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Models;
using System;

namespace SpanishPoint.Azure.Iswc.Data.Services.Notifications.CosmosDb.Models
{
    public class CsnNotifications : BaseModel
    {
        public CsnNotifications()
        {

        }

        public CsnNotifications(WorkInfo workInfo, string agency, DateTime now, TransactionType transactionType, string fromIswc, string toIswc = null, string workflowMessage = null)
        {
            ID = Guid.NewGuid();
            FromIswc = fromIswc;
            ToIswc = toIswc ?? workInfo.Iswc.Iswc1;
            ReceivingAgencyCode = workInfo.AgencyId;
            SubmittingAgencyCode = agency;
            ReceivingAgencyWorkCode = workInfo.AgencyWorkCode;
            WorkflowStatus = string.Empty;
            ProcessingDate = now;
            TransactionType = transactionType;
            PartitionKey = $"{now:yyyyMMdd}{ReceivingAgencyCode}";
            WorkflowMessage = workflowMessage;
        }

        [JsonProperty(PropertyName = "id")]
        public Guid ID { get; internal set; }
        public string SubmittingAgencyCode { get; set; }
        public string ReceivingAgencyCode { get; set; }
        public string ToIswc { get; set; }
        public string FromIswc { get; set; }
        public DateTime ProcessingDate { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public TransactionType TransactionType { get; set; }
        public string WorkflowTaskID { get; set; }
        public string WorkflowStatus { get; set; }
        public string ReceivingAgencyWorkCode { get; set; }
        public string PartitionKey { get; set; }

        public DateTime? ProcessedOnDate { get; }
        public string HttpResponse { get; set; }
        public string WorkflowMessage { get; set; }

        [JsonIgnoreAttribute]
        public string ContinuationToken { get; set; }
    }
}
