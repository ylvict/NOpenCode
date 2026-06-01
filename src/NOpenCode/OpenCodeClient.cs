using System;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class OpenCodeClient : IAsyncDisposable
    {
        private readonly ServerManager _server;
        private readonly NOpenCodeHttpClient _http;
        private readonly NOpenCodeBuilder _config;
        private bool _disposed;

        internal NOpenCodeHttpClient Http => _http;
        internal NOpenCodeBuilder Config => _config;
        internal Action<string>? Log => _config.LogCallback;

        public SessionClient Sessions { get; }
        public ConfigClient ConfigClient { get; }
        public ProviderClient Providers { get; }
        public ModelClient Models { get; }
        public AgentClient Agents { get; }
        public FileClient Files { get; }
        public McpClient Mcp { get; }
        public ProjectClient Projects { get; }
        public CommandClient Commands { get; }
        public EventClient Events { get; }
        public DiagnosticsClient Diagnostics { get; }

        internal OpenCodeClient(ServerManager server, NOpenCodeBuilder config)
        {
            _server = server;
            _config = config;
            _http = new NOpenCodeHttpClient(server.BaseUrl);

            if (!string.IsNullOrEmpty(config.Username) && !string.IsNullOrEmpty(config.Password))
                _http.SetAuth(config.Username!, config.Password!);

            Sessions = new SessionClient(_http, config);
            ConfigClient = new ConfigClient(_http);
            Providers = new ProviderClient(_http);
            Models = new ModelClient(_http);
            Agents = new AgentClient(_http);
            Files = new FileClient(_http);
            Mcp = new McpClient(_http);
            Projects = new ProjectClient(_http);
            Commands = new CommandClient(_http);
            Events = new EventClient(_http);
            Diagnostics = new DiagnosticsClient(_http);
        }

        public AskOperation Ask(string prompt)
        {
            return new AskOperation(_http, _config, prompt);
        }

        public SessionBuilder NewSession(string? title = null)
        {
            return new SessionBuilder(_http, _config, title);
        }

        public OpenCodeSession Session(string sessionId)
        {
            return new OpenCodeSession(_http, _config, sessionId);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;
            _http.Dispose();
            await _server.DisposeAsync();
        }
    }
}
