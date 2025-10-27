using Newtonsoft.Json;
using System;

namespace SpanishPoint.Azure.Iswc.Bdo.Rules
{
    internal class ErrorCodeConverter : JsonConverter<ErrorCode>
    {
        public override ErrorCode ReadJson(JsonReader reader, Type objectType, ErrorCode existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return Enum.Parse<ErrorCode>(reader.Value?.ToString() ?? string.Empty);
        }

        public override void WriteJson(JsonWriter writer, ErrorCode value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString().TrimStart('_'));
        }
    }

    [JsonConverter(typeof(ErrorCodeConverter))]
    public enum ErrorCode
    {
        _100,
        _102,
        _103,
        _104,
        _105,
        _106,
        _107,
        _108,
        _109,
        _110,
        _111,
        _112,
        _113,
        _114,
        _115,
        _116,
        _117,
        _118,
        _119,
        _120,
        _121,
        _122,
        _123,
        _124,
        _125,
        _126,
        _127,
        _128,
        _129,
        _130,
        _131,
        _132,
        _133,
        _134,
        _135,
        _136,
        _137,
        _138,
        _139,
        _140,
        _141,
        _143,
        _144,
        _145,
        _146,
        _147,
        _148,
        _149,
        _150,
        _151,
        _152,
        _153,
        _154,
        _155,
        _156,
        _157,
        _159,
        _158,
        _160,
        _161,
        _162,
        _163,
        _164,
        _166,
        _167,
        _168,
        _169,
        _170,
        _171,
        _172,
        _173,
        _174,
        _175,
        _176,
        _177,
        _178,
        _179,
        _180,
        _181,
        _182,
        _201,
        _202,
        _203,
        _247,
        _248,
        _249,
        _250,
        _251,
    }   
}
