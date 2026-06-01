using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class SessionClient
    {
        private readonly NOpenCodeHttpClient _http;
        private readonly NOpenCodeBuilder _config;

        internal SessionClient(NOpenCodeHttpClient http, NOpenCodeBuilder config)
        {
            _http = http;
            _config = config;
        }

        public async Task<List<SessionInfo>> List(CancellationToken ct = default)
        {
            return await _http.Get<List<SessionInfo>>("/session", ct);
        }

        public async Task<OpenCodeSession> Create(string? title = null, CancellationToken ct = default)
        {
            var body = new { title = title ?? "NOpenCode Session" };
            var result = await _http.Post<SessionInfo>("/session", body, ct);
            return new OpenCodeSession(_http, _config, result.Id);
        }

        public async Task<SessionInfo> Get(string sessionId, CancellationToken ct = default)
        {
            return await _http.Get<SessionInfo>($"/session/{sessionId}", ct);
        }

        public async Task Delete(string sessionId, CancellationToken ct = default)
        {
            await _http.Delete<bool>($"/session/{sessionId}", ct);
        }
    }
}
