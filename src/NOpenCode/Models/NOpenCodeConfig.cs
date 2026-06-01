using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class NOpenCodeConfig
    {
        [JsonPropertyName("model")]
        public string? Model { get; set; }

        [JsonPropertyName("agent")]
        public string? Agent { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }
}
