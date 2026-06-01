using System.Collections.Generic;
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
            var path = provider != null ? $"/config/providers" : "/config/providers";
            var result = await _http.Get<ProvidersResponse>(path, ct);
            var models = new List<ModelInfo>();
            if (result?.Providers != null)
            {
                foreach (var p in result.Providers)
                {
                    if (provider == null || p.Id == provider)
                    {
                        if (p.Models != null)
                        {
                            foreach (var m in p.Models)
                            {
                                models.Add(new ModelInfo
                                {
                                    Id = m.Id ?? "",
                                    Name = m.Name ?? m.Id,
                                    Provider = p.Id
                                });
                            }
                        }
                    }
                }
            }
            return models;
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
        public List<ModelEntry>? Models { get; set; }
    }

    internal class ModelEntry
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }
}
