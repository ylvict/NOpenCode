using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class OpenCodeSession
    {
        private readonly NOpenCodeHttpClient _http;
        private readonly NOpenCodeBuilder _config;
        private readonly string _sessionId;

        public string Id => _sessionId;

        internal OpenCodeSession(NOpenCodeHttpClient http, NOpenCodeBuilder config, string sessionId)
        {
            _http = http;
            _config = config;
            _sessionId = sessionId;
        }

        public async Task<OpenCodeReply> Ask(string message)
        {
            var body = BuildMessageBody(message);
            return await _http.Post<OpenCodeReply>(
                $"/session/{_sessionId}/message", body);
        }

        public async Task<OpenCodeReply> Ask(string message, Action<MessageOptions> configure)
        {
            var options = new MessageOptions();
            configure(options);
            var body = BuildMessageBody(message, options);
            return await _http.Post<OpenCodeReply>(
                $"/session/{_sessionId}/message", body);
        }

        public Task AskStream(
            string message,
            Action<string> onChunk,
            Action<OpenCodeReply>? onComplete = null,
            Action<Exception>? onError = null,
            CancellationToken ct = default)
        {
            return AskStream(message, null, onChunk, onComplete, onError, ct);
        }

        public async Task AskStream(
            string message,
            Action<MessageOptions>? configure,
            Action<string> onChunk,
            Action<OpenCodeReply>? onComplete = null,
            Action<Exception>? onError = null,
            CancellationToken ct = default)
        {
            var options = new MessageOptions();
            configure?.Invoke(options);

            var parts = new List<object>
            {
                new { type = "text", text = message }
            };

            var body = new
            {
                parts,
                model = options.Model ?? _config.Model,
                agent = options.Agent ?? _config.Agent
            };

            try
            {
                var fullReply = await _http.Post<OpenCodeReply>(
                    $"/session/{_sessionId}/message", body, ct);

                var text = fullReply.GetText();
                if (!string.IsNullOrEmpty(text))
                {
                    onChunk(text);
                }

                onComplete?.Invoke(fullReply);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
        }

        public async Task<List<MessageInfo>> GetMessages()
        {
            return await _http.Get<List<MessageInfo>>(
                $"/session/{_sessionId}/message");
        }

        public async Task<List<FileDiff>> GetDiff()
        {
            return await _http.Get<List<FileDiff>>(
                $"/session/{_sessionId}/diff");
        }

        public async Task<OpenCodeSession> Fork(string? messageId = null)
        {
            var body = messageId != null ? new { messageID = messageId } : null;
            var result = await _http.Post<SessionInfo>(
                $"/session/{_sessionId}/fork", body);
            return new OpenCodeSession(_http, _config, result.Id);
        }

        public async Task<string> Share()
        {
            var result = await _http.Post<SessionInfo>(
                $"/session/{_sessionId}/share");
            return result.Id;
        }

        public async Task Unshare()
        {
            await _http.Delete<bool>($"/session/{_sessionId}/share");
        }

        public async Task Abort()
        {
            await _http.Post($"/session/{_sessionId}/abort");
        }

        public async Task Delete()
        {
            await _http.Delete<bool>($"/session/{_sessionId}");
        }

        public async Task<bool> Revert(string messageId, string? partId = null)
        {
            var body = new { messageID = messageId, partID = partId };
            return await _http.Post<bool>(
                $"/session/{_sessionId}/revert", body);
        }

        public async Task<bool> Unrevert()
        {
            return await _http.Post<bool>(
                $"/session/{_sessionId}/unrevert");
        }

        public async Task<bool> Summarize(string? providerId = null, string? modelId = null)
        {
            var body = new { providerID = providerId, modelID = modelId };
            return await _http.Post<bool>(
                $"/session/{_sessionId}/summarize", body);
        }

        public async Task<OpenCodeReply> RunCommand(string command, string? arguments = null)
        {
            var body = new { command, arguments, model = _config.Model, agent = _config.Agent };
            return await _http.Post<OpenCodeReply>(
                $"/session/{_sessionId}/command", body);
        }

        public async Task<OpenCodeReply> RunShell(string command)
        {
            var body = new { command, model = _config.Model, agent = _config.Agent };
            return await _http.Post<OpenCodeReply>(
                $"/session/{_sessionId}/shell", body);
        }

        private object BuildMessageBody(string message, MessageOptions? options = null)
        {
            var parts = new List<object>
            {
                new { type = "text", text = message }
            };

            return new
            {
                parts,
                model = options?.Model ?? _config.Model,
                agent = options?.Agent ?? _config.Agent
            };
        }
    }

    public class MessageOptions
    {
        public string? Model { get; set; }
        public string? Agent { get; set; }
        public List<string>? Files { get; set; }
    }
}
