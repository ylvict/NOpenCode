using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class McpStatus
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
