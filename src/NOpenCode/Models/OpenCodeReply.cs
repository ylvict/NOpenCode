using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class OpenCodeReply
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonPropertyName("parts")]
        public List<Part>? Parts { get; set; }

        [JsonPropertyName("usage")]
        public TokenUsage? Usage { get; set; }

        [JsonPropertyName("messageId")]
        public string? MessageId { get; set; }

        public string GetText() => Text ?? ExtractTextFromParts();

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
