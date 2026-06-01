using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class ConfigClient
    {
        private readonly NOpenCodeHttpClient _http;

        internal ConfigClient(NOpenCodeHttpClient http)
        {
            _http = http;
        }

        public async Task<NOpenCodeConfig> Get(CancellationToken ct = default)
        {
            return await _http.Get<NOpenCodeConfig>("/config", ct);
        }

        public async Task<NOpenCodeConfig> Update(object patch, CancellationToken ct = default)
        {
            return await _http.Patch<NOpenCodeConfig>("/config", patch, ct);
        }
    }
}
