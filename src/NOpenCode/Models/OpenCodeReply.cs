using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class OpenCodeReply
    {
        [JsonPropertyName("parts")]
        public List<Part>? Parts { get; set; }

        [JsonPropertyName("usage")]
        public TokenUsage? Usage { get; set; }

        [JsonPropertyName("messageId")]
        public string? MessageId { get; set; }

        [JsonPropertyName("info")]
        public ReplyInfo? Info { get; set; }

        public string GetText() => ExtractTextFromParts();

        public TokenUsage? GetUsage()
        {
            if (Usage != null) return Usage;
            if (Info?.Tokens != null)
                return new TokenUsage
                {
                    Input = Info.Tokens.Input,
                    Output = Info.Tokens.Output,
                    Total = Info.Tokens.Input + Info.Tokens.Output
                };
            return null;
        }

        private string ExtractTextFromParts()
        {
            if (Parts == null || Parts.Count == 0) return "";
            var texts = new List<string>();
            foreach (var part in Parts)
            {
                if (part.Type == "text" && part.Text != null)
                    texts.Add(part.Text);
            }
            return string.Join("\n", texts);
        }
    }
}
