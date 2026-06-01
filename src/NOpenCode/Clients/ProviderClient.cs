using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class ProviderClient
    {
        private readonly NOpenCodeHttpClient _http;

        internal ProviderClient(NOpenCodeHttpClient http)
        {
            _http = http;
        }

        public async Task<ProviderListResponse> List(CancellationToken ct = default)
        {
            return await _http.Get<ProviderListResponse>("/provider", ct);
        }
    }
}
