using System;
using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class MessageInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("role")]
        public string? Role { get; set; }

        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }
    }
}
