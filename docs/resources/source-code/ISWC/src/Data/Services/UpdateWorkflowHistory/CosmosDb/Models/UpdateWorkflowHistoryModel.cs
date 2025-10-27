using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Bdo.Submissions;
using SpanishPoint.Azure.Iswc.Framework.Converters;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Models;

namespace SpanishPoint.Azure.Iswc.Data.Services.UpdateWorkflowHistory.CosmosDb.Models
{
    internal class UpdateWorkflowHistoryModel : BaseModel
    {
        [JsonProperty(PropertyName = "id")]
        [JsonConverter(typeof(ToStringJsonConverter))]
        public long WorkInfoId { get; set; }
        public string PreferredIswc { get; set; }
        public Submission Model { get; set; }
    }
}
