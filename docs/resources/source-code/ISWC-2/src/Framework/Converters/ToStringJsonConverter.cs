using Newtonsoft.Json;
using System;

namespace SpanishPoint.Azure.Iswc.Framework.Converters
{
    public class ToStringJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => 
            true;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(value.ToString());
        }

        public override bool CanRead =>
            false;

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) => 
            throw new NotImplementedException();
    }

}
