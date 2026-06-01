using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class AgentClient
    {
        private readonly NOpenCodeHttpClient _http;

        internal AgentClient(NOpenCodeHttpClient http)
        {
            _http = http;
        }

        public async Task<List<AgentInfo>> List(CancellationToken ct = default)
        {
            return await _http.Get<List<AgentInfo>>("/agent", ct);
        }
    }
}
