using System;
using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class SessionInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("isShareable")]
        public bool? IsShareable { get; set; }
    }
}
