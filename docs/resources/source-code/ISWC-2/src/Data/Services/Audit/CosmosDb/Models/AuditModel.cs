using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Models;
using System;

namespace SpanishPoint.Azure.Iswc.Data.Services.Audit.CosmosDb.Models
{
    /// <summary>
    /// Top level Audit record. Stores the batch detail.
    /// </summary>
    internal class AuditModel : BaseModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid AuditId { get; internal set; }
        [JsonProperty]
        public string AgencyCode { get; internal set; }
        [JsonProperty]
        public DateTime CreatedDate { get; internal set; }
        [JsonProperty]
        public string PartitionKey { get; internal set; }
        [JsonProperty]
        public int BatchSize { get; set; }
    }
}
