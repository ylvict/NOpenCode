using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class McpClient
    {
        private readonly NOpenCodeHttpClient _http;

        internal McpClient(NOpenCodeHttpClient http)
        {
            _http = http;
        }

        public async Task<Dictionary<string, McpStatus>> List(CancellationToken ct = default)
        {
            return await _http.Get<Dictionary<string, McpStatus>>("/mcp", ct);
        }

        public async Task<McpStatus> Add(string name, object config, CancellationToken ct = default)
        {
            var body = new { name, config };
            return await _http.Post<McpStatus>("/mcp", body, ct);
        }
    }
}
