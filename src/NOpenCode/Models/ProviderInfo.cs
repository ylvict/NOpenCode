using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NOpenCode
{
    public class ProviderInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("connected")]
        public bool? Connected { get; set; }
    }

    public class ProviderListResponse
    {
        [JsonPropertyName("all")]
        public List<ProviderInfo>? All { get; set; }

        [JsonPropertyName("connected")]
        public List<string>? Connected { get; set; }
    }
}
