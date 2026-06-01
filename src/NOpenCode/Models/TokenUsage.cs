using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class TokenUsage
    {
        [JsonPropertyName("input")]
        public int? Input { get; set; }

        [JsonPropertyName("output")]
        public int? Output { get; set; }

        [JsonPropertyName("total")]
        public int? Total { get; set; }
    }
}
