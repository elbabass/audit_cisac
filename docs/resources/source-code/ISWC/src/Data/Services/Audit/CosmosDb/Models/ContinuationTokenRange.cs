using Newtonsoft.Json;

namespace SpanishPoint.Azure.Iswc.Data.Services.Audit.CosmosDb.Models
{
    internal class ContinuationTokenRange
    {
        internal ContinuationTokenRange() { }

        internal ContinuationTokenRange(string min, string max)
        {
            Min = min;
            Max = max;
        }

        [JsonProperty(PropertyName = "min", NullValueHandling = NullValueHandling.Ignore)]
        internal string Min { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "max", NullValueHandling = NullValueHandling.Ignore)]
        internal string Max { get; set; } = string.Empty;
    }


}
