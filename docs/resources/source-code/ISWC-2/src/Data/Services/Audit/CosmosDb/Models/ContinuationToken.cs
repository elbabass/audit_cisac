using Newtonsoft.Json;
using SpanishPoint.Azure.Iswc.Framework.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SpanishPoint.Azure.Iswc.Data.Services.Audit.CosmosDb.Models
{
    internal class ContinuationToken
    {
        internal ContinuationToken() { Range = new ContinuationTokenRange(); }

        internal ContinuationToken(string continuationTokenRequestString)
        {
            var values = continuationTokenRequestString.Split(";");

            if (values.Length < 3) return;

            Token = values[0];
            Range = new ContinuationTokenRange(values[1], values[2]);
        }

        [JsonProperty(PropertyName = "token", NullValueHandling = NullValueHandling.Ignore)]
        internal string Token { get; set; } = string.Empty;

        [JsonProperty(PropertyName = "range", NullValueHandling = NullValueHandling.Ignore)]
        internal ContinuationTokenRange Range { get; set; }

        internal string GetContinuationTokenString() => StringExtensions.CreateSemiColonSeperatedString(new List<string> { Token, Range.Min, Range.Max });

        internal string Serialize() => JsonConvert.SerializeObject(new List<ContinuationToken> { this });

        internal string Deserialize(string continuationToken) => !string.IsNullOrEmpty(continuationToken) ?
            JsonConvert.DeserializeObject<IEnumerable<ContinuationToken>>(continuationToken)?.FirstOrDefault()?.GetContinuationTokenString() : string.Empty;
    }
}
