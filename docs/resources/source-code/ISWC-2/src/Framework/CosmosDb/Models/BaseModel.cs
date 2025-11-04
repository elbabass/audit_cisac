using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace SpanishPoint.Azure.Iswc.Framework.CosmosDb.Models
{
    [ExcludeFromCodeCoverage]
    public class BaseModel
    {
        public BaseModel()
        {
            ETag = string.Empty;
        }

        [JsonIgnore]
        public string ETag { get; set; }
    }
}
