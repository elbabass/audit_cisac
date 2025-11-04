using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Models;

namespace SpanishPoint.Azure.Iswc.Data.Services.Checksum.CosmosDb.Models
{
    public class SubmissionChecksums : BaseModel
    {
        public SubmissionChecksums()
        {

        }

        public SubmissionChecksums(string agency, string agencyWorkCode, string hashValue)
        {
            var id = string.Concat(agency, agencyWorkCode);
            ID = id;
            PartitionKey = id;
            Hash = hashValue;
        }

        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }
        public string PartitionKey { get; set; }
        public string Hash { get; set; }
    }
}
