using System.Threading.Tasks;

namespace NOpenCode
{
    public class SessionBuilder
    {
        private readonly NOpenCodeHttpClient _http;
        private readonly NOpenCodeBuilder _config;
        private readonly string? _title;
        private string? _model;
        private string? _agent;
        private string? _directory;

        internal SessionBuilder(NOpenCodeHttpClient http, NOpenCodeBuilder config, string? title)
        {
            _http = http;
            _config = config;
            _title = title;
        }

        public SessionBuilder UsingModel(string model)
        {
            _model = model;
            return this;
        }

        public SessionBuilder WithAgent(string agent)
        {
            _agent = agent;
            return this;
        }

        public SessionBuilder InDirectory(string directory)
        {
            _directory = directory;
            return this;
        }

        public async Task<OpenCodeSession> Create()
        {
            var title = _title ?? _directory ?? "NOpenCode Session";
            var body = new { title };
            var result = await _http.Post<SessionInfo>("/session", body);
            return new OpenCodeSession(_http, _config, result.Id);
        }
    }
}
