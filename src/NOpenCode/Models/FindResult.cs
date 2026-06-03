using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class FindResult
    {
        [JsonPropertyName("path")]
        [JsonConverter(typeof(TextValueConverter))]
        public string Path { get; set; } = "";

        [JsonPropertyName("lines")]
        [JsonConverter(typeof(TextValueConverter))]
        public string? Lines { get; set; }

        [JsonPropertyName("line_number")]
        public int? LineNumber { get; set; }

        [JsonPropertyName("submatches")]
        public List<SubMatch>? Submatches { get; set; }
    }

    public class SubMatch
    {
        [JsonPropertyName("match")]
        public string? Match { get; set; }

        [JsonPropertyName("start")]
        public int? Start { get; set; }

        [JsonPropertyName("end")]
        public int? End { get; set; }
    }

    internal class TextValueConverter : JsonConverter<string?>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
                return reader.GetString();

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                return doc.RootElement.TryGetProperty("text", out var t) ? t.GetString() : null;
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
