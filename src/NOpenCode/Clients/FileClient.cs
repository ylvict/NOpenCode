using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class FileClient
    {
        private readonly NOpenCodeHttpClient _http;

        internal FileClient(NOpenCodeHttpClient http)
        {
            _http = http;
        }

        public async Task<List<FindResult>> Search(string pattern, CancellationToken ct = default)
        {
            var encoded = Uri.EscapeDataString(pattern);
            return await _http.Get<List<FindResult>>($"/find?pattern={encoded}", ct);
        }

        public async Task<List<string>> Find(string query, CancellationToken ct = default)
        {
            var encoded = Uri.EscapeDataString(query);
            return await _http.Get<List<string>>($"/find/file?query={encoded}", ct);
        }

        public async Task<List<FileNode>> List(string? path = null, CancellationToken ct = default)
        {
            var query = path != null ? $"?path={Uri.EscapeDataString(path)}" : "";
            return await _http.Get<List<FileNode>>($"/file{query}", ct);
        }

        public async Task<FileContent> Read(string path, CancellationToken ct = default)
        {
            var encoded = Uri.EscapeDataString(path);
            return await _http.Get<FileContent>($"/file/content?path={encoded}", ct);
        }
    }
}
