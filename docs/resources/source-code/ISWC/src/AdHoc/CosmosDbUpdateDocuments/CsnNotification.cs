using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SpanishPoint.Azure.Iswc.Bdo.Edi;
using System;
using System.Collections.Generic;
using System.Text;

namespace CosmosDbUpdateDocuments
{
    public class CsnNotification
    {
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

        public DateTime? ProcessedOnDate { get; set; }
        public string HttpResponse { get; set; }
        public string WorkflowMessage { get; set; }

        [JsonIgnoreAttribute]
        public string ContinuationToken { get; set; }
    }
}
