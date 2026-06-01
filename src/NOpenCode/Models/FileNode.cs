using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class FileNode
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("path")]
        public string? Path { get; set; }

        [JsonPropertyName("children")]
        public List<FileNode>? Children { get; set; }
    }
}
