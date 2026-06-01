using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class FileDiff
    {
        [JsonPropertyName("path")]
        public string Path { get; set; } = "";

        [JsonPropertyName("original")]
        public string? Original { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
