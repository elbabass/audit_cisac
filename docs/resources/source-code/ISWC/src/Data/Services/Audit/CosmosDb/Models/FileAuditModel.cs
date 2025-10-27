using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Data.Services.Audit.CosmosDb.Models
{
    internal class FileAuditModel : BaseModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid AuditId { get; internal set; }
        [JsonProperty]
        public string AgencyCode { get; internal set; }
        [JsonProperty]
        public DateTime DatePickedUp { get; internal set; }
        [JsonProperty]
        public DateTime? DateAckGenerated { get; internal set; }
        [JsonProperty]
        public string PartitionKey { get; internal set; }
        [JsonProperty]
        public long? SubmittingPublisherIPNameNumber { get; internal set; }
        [JsonProperty]
        public string FileName { get; internal set; }
        [JsonProperty]
        public string AckFileName { get; internal set; }
        [JsonProperty]
        public string Status { get; internal set; }
    }
}
