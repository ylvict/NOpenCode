using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class AskOperation
    {
        private readonly NOpenCodeHttpClient _http;
        private readonly NOpenCodeBuilder _config;
        private readonly string _prompt;
        private string? _model;
        private string? _agent;
        private readonly List<string> _files = new();

        internal AskOperation(NOpenCodeHttpClient http, NOpenCodeBuilder config, string prompt)
        {
            _http = http;
            _config = config;
            _prompt = prompt;
        }

        public AskOperation UsingModel(string model)
        {
            _model = model;
            return this;
        }

        public AskOperation WithAgent(string agent)
        {
            _agent = agent;
            return this;
        }

        public AskOperation WithFiles(params string[] files)
        {
            _files.AddRange(files);
            return this;
        }

        public async Task<string> Execute()
        {
            var reply = await ExecuteFull();
            return reply.GetText();
        }

        public async Task<OpenCodeReply> ExecuteFull()
        {
            var session = await CreateSessionAsync();
            var reply = await session.Ask(_prompt);
            await session.Delete();
            return reply;
        }

        public async Task AskStream(
            Action<string> onChunk,
            Action<OpenCodeReply>? onComplete = null,
            Action<Exception>? onError = null,
            CancellationToken ct = default)
        {
            var session = await CreateSessionAsync();
            await session.AskStream(_prompt, null, onChunk, onComplete, onError, ct);
        }

        private async Task<OpenCodeSession> CreateSessionAsync()
        {
            var title = _prompt.Length > 80
                ? _prompt.Substring(0, 80) + "..."
                : _prompt;

            var body = new { title };
            var result = await _http.Post<SessionInfo>("/session", body);
            return new OpenCodeSession(_http, _config, result.Id);
        }
    }
}
