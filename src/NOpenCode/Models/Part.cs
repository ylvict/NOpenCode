using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class Part
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("toolName")]
        public string? ToolName { get; set; }

        [JsonPropertyName("toolArgs")]
        public string? ToolArgs { get; set; }

        [JsonPropertyName("result")]
        public string? Result { get; set; }
    }
}
