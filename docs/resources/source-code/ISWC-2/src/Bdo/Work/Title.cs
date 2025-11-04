using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SpanishPoint.Azure.Iswc.Bdo.Work
{
    public class Title
    {
        public long? TitleID { get; set; }
        public string? Name { get; set; }
        public string? StandardizedName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TitleType Type { get; set; }
    }
}
