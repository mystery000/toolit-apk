using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Toolit.Converters
{
    public class JsonI18nStringConverter : JsonConverter<List<I18nString>>
    {
        public override bool CanConvert(Type typeToConvert) => typeof(List<I18nString>).IsAssignableFrom(typeToConvert);

        public override List<I18nString> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            List<I18nString> ns = new List<I18nString>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return ns;
                }
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    var n = new I18nString();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndObject)
                        {
                            ns.Add(n);
                            break;
                        }

                        if (reader.TokenType == JsonTokenType.PropertyName)
                        {
                            var propertyName = reader.GetString();
                            reader.Read();
                            switch (propertyName)
                            {
                                case "l":
                                    n.L = reader.GetString();
                                    break;
                                case "s":
                                    n.S = reader.GetString();
                                    break;
                                default:
                                    throw new JsonException($"Unrecognized field {propertyName}.");
                            }
                        }
                    }
                }
            }
            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, List<I18nString> a, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var s in a) 
            {
                writer.WriteStartObject();
                writer.WriteString("l", s.L);
                writer.WriteString("s", s.S);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
    }
}