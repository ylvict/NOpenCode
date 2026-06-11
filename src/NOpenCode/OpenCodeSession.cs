using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
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

            var body = BuildMessageDict(message, options);
            body["stream"] = true;

            try
            {
                using var stream = await _http.PostStream(
                    $"/session/{_sessionId}/message", body, ct);

                using var reader = new SseReader(stream);
                OpenCodeReply? finalReply = null;

                while (!ct.IsCancellationRequested)
                {
                    var evt = await reader.ReadEventAsync(ct);
                    if (evt == null) break;

                    if (evt.Type == "chunk")
                    {
                        var part = JsonSerializer.Deserialize<Part>(evt.Data);
                        if (part?.Type == "text" && part.Text != null)
                            onChunk(part.Text);
                    }
                    else if (evt.Type == "complete")
                    {
                        finalReply = JsonSerializer.Deserialize<OpenCodeReply>(evt.Data);
                    }
                    else if (evt.Type == "error")
                    {
                        throw new NOpenCodeException(
                            $"Server streaming error: {evt.Data}");
                    }
                }

                onComplete?.Invoke(finalReply ?? new OpenCodeReply());
            }
            catch (Exception ex) when (onError != null)
            {
                onError(ex);
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
            var body = BuildCommandDict(command, arguments);
            return await _http.Post<OpenCodeReply>(
                $"/session/{_sessionId}/command", body);
        }

        public async Task<OpenCodeReply> RunShell(string command)
        {
            var body = BuildCommandDict(command, null);
            return await _http.Post<OpenCodeReply>(
                $"/session/{_sessionId}/shell", body);
        }

        private JsonObject BuildMessageBody(string message, MessageOptions? options = null)
        {
            return BuildMessageDict(message, options);
        }

        private JsonObject BuildMessageDict(string message, MessageOptions? options)
        {
            var parts = new JsonArray
            {
                new JsonObject { ["type"] = "text", ["text"] = message }
            };

            if (options?.Files != null)
            {
                foreach (var file in options.Files)
                {
                    parts.Add(new JsonObject
                    {
                        ["type"] = "file",
                        ["uri"] = file
                    });
                }
            }

            var body = new JsonObject { ["parts"] = parts };

            var modelId = options?.Model ?? _config.Model;
            if (modelId != null)
                body["model"] = BuildModelObject(modelId);

            var agent = options?.Agent ?? _config.Agent;
            if (agent != null)
                body["agent"] = agent;

            return body;
        }

        private JsonObject BuildCommandDict(string command, string? arguments)
        {
            var body = new JsonObject
            {
                ["command"] = command,
                ["arguments"] = arguments
            };

            if (_config.Model != null)
                body["model"] = BuildModelObject(_config.Model);
            if (_config.Agent != null)
                body["agent"] = _config.Agent;

            return body;
        }

        public static JsonObject BuildModelObject(string modelString)
        {
            var obj = new JsonObject();
            var slashIndex = modelString.IndexOf('/');
            if (slashIndex > 0 && slashIndex < modelString.Length - 1)
            {
                obj["providerID"] = modelString.Substring(0, slashIndex);
                obj["modelID"] = modelString.Substring(slashIndex + 1);
            }
            else
            {
                obj["modelID"] = modelString;
            }
            return obj;
        }
    }

    public class MessageOptions
    {
        public string? Model { get; set; }
        public string? Agent { get; set; }
        public List<string>? Files { get; set; }
    }
}
