using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class CommandClient
    {
        private readonly NOpenCodeHttpClient _http;

        internal CommandClient(NOpenCodeHttpClient http)
        {
            _http = http;
        }

        public async Task<List<CommandInfo>> List(CancellationToken ct = default)
        {
            return await _http.Get<List<CommandInfo>>("/command", ct);
        }
    }

    public class CommandInfo
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
