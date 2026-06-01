using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class HealthInfo
    {
        [JsonPropertyName("healthy")]
        public bool Healthy { get; set; }

        [JsonPropertyName("version")]
        public string? Version { get; set; }
    }
}
