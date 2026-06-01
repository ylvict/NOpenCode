using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class ProjectClient
    {
        private readonly NOpenCodeHttpClient _http;

        internal ProjectClient(NOpenCodeHttpClient http)
        {
            _http = http;
        }

        public async Task<List<ProjectInfo>> List(CancellationToken ct = default)
        {
            return await _http.Get<List<ProjectInfo>>("/project", ct);
        }

        public async Task<ProjectInfo> GetCurrent(CancellationToken ct = default)
        {
            return await _http.Get<ProjectInfo>("/project/current", ct);
        }

        public async Task<VcsInfo> GetVcsInfo(CancellationToken ct = default)
        {
            return await _http.Get<VcsInfo>("/vcs", ct);
        }
    }

    public class ProjectInfo
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Path { get; set; }
    }

    public class VcsInfo
    {
        public string? Branch { get; set; }
        public string? Commit { get; set; }
        public string? Status { get; set; }
    }
}
