using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class TokenUsage
    {
        [JsonPropertyName("input")]
        public int? Input { get; set; }

        [JsonPropertyName("output")]
        public int? Output { get; set; }

        [JsonPropertyName("total")]
        public int? Total { get; set; }
    }

    public class MessageTokens
    {
        [JsonPropertyName("input")]
        public int? Input { get; set; }

        [JsonPropertyName("output")]
        public int? Output { get; set; }
    }

    public class ReplyInfo
    {
        [JsonPropertyName("tokens")]
        public MessageTokens? Tokens { get; set; }
    }
}
