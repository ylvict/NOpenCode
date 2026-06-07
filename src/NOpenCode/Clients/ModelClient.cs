using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class ModelClient
    {
        private readonly NOpenCodeHttpClient _http;

        internal ModelClient(NOpenCodeHttpClient http)
        {
            _http = http;
        }

        public async Task<List<ModelInfo>> List(string? provider = null, CancellationToken ct = default)
        {
            var path = "/config/providers";
            var result = await _http.Get<ProvidersResponse>(path, ct);

            if (result?.Providers == null)
                return new List<ModelInfo>();

            return result.Providers
                .Where(p => provider == null || p.Id == provider)
                .Where(p => p.Models != null)
                .SelectMany(p => p.Models!.Values.Select(m => new ModelInfo
                {
                    Id = m.Id ?? "",
                    Name = m.Name ?? m.Id,
                    Provider = p.Id
                }))
                .ToList();
        }
    }

    internal class ProvidersResponse
    {
        public List<ProviderEntry>? Providers { get; set; }
        public System.Text.Json.JsonElement? Default { get; set; }
    }

    internal class ProviderEntry
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public Dictionary<string, ModelEntry>? Models { get; set; }
    }

    internal class ModelEntry
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}
