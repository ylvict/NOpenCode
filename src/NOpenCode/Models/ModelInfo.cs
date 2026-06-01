using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class ModelInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("provider")]
        public string? Provider { get; set; }
    }
}
