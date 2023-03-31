using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Toolit.Converters
{
    public class JsonDecimalConverter : JsonConverter<decimal>
    {
        public override bool CanConvert(Type typeToConvert) => typeof(decimal).IsAssignableFrom(typeToConvert);

        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("decimal is not a string.");
            }
            return decimal.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, decimal a, JsonSerializerOptions options)
        {
            writer.WriteStringValue(a.ToString());
        }
    }
}