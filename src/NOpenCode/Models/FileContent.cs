using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class FileContent
    {
        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }
    }
}
