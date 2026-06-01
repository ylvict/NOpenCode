using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class DiagnosticsClient
    {
        private readonly NOpenCodeHttpClient _http;

        internal DiagnosticsClient(NOpenCodeHttpClient http)
        {
            _http = http;
        }

        public async Task<HealthInfo> GetHealth(CancellationToken ct = default)
        {
            return await _http.Get<HealthInfo>("/global/health", ct);
        }
    }
}
