using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class FindResult
    {
        [JsonPropertyName("path")]
        public string Path { get; set; } = "";

        [JsonPropertyName("lines")]
        public string? Lines { get; set; }

        [JsonPropertyName("line_number")]
        public int? LineNumber { get; set; }

        [JsonPropertyName("submatches")]
        public List<SubMatch>? Submatches { get; set; }
    }

    public class SubMatch
    {
        [JsonPropertyName("match")]
        public string? Match { get; set; }

        [JsonPropertyName("start")]
        public int? Start { get; set; }

        [JsonPropertyName("end")]
        public int? End { get; set; }
    }
}
