using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Framework.CosmosDb.Models;
using System;

namespace SpanishPoint.Azure.Iswc.Data.Services.IswcService.CosmosDb.Models
{
    internal class IswcModel : BaseModel
    {
        [JsonProperty(PropertyName = "id")]
        public Guid IswcId { get; internal set; }
        public string Iswc { get; set; }
    }
}
