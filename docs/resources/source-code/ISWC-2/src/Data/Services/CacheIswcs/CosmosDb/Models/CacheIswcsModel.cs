using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Bdo.Iswc;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Models;
using System;
using System.Collections.Generic;
using System.Security.Policy;
using System.Text;

namespace SpanishPoint.Azure.Iswc.Data.Services.CacheIswcs.CosmosDb.Models
{
    public class CacheIswcsModel : BaseModel
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "PartitionKey")]
        public string PartitionKey { get; set; }

        [JsonProperty(PropertyName = "IswcMetadata")]
        public CacheIswcMetadata IswcMetadata { get; set; }
    }
}
