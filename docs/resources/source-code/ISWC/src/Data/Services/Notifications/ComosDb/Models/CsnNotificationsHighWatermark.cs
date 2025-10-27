using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Models;

namespace SpanishPoint.Azure.Iswc.Data.Services.Notifications.CosmosDb.Models
{
    public class CsnNotificationsHighWatermark : BaseModel
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; internal set; }
        public string HighWatermark { get; set; }
        public string PartitionKey { get; set; }
    }
}
