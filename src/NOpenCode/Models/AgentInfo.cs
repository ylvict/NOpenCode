using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class AgentInfo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("mode")]
        public string? Mode { get; set; }
    }
}
