using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    internal class NOpenCodeHttpClient : IDisposable
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json;
        private readonly string _baseUrl;

        public string BaseUrl => _baseUrl;

        public NOpenCodeHttpClient(string baseUrl, TimeSpan? timeout = null)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _http = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl, UriKind.Absolute),
                Timeout = timeout ?? TimeSpan.FromMinutes(5)
            };
            _json = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public void SetAuth(string username, string password)
        {
            var bytes = Encoding.ASCII.GetBytes($"{username}:{password}");
            _http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(bytes));
        }

        public async Task<T> Get<T>(string path, CancellationToken ct = default)
        {
            var response = await _http.GetAsync(path, ct);
            return await HandleResponse<T>(response, path, ct);
        }

        public async Task<T> Post<T>(string path, object? body = null, CancellationToken ct = default)
        {
            var content = SerializeBody(body);
            var response = await _http.PostAsync(path, content, ct);
            return await HandleResponse<T>(response, path, ct);
        }

        public async Task<T> Patch<T>(string path, object? body = null, CancellationToken ct = default)
        {
            var content = SerializeBody(body);
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), path) { Content = content };
            var response = await _http.SendAsync(request, ct);
            return await HandleResponse<T>(response, path, ct);
        }

        public async Task<T> Delete<T>(string path, CancellationToken ct = default)
        {
            var response = await _http.DeleteAsync(path, ct);
            return await HandleResponse<T>(response, path, ct);
        }

        public async Task<bool> Post(string path, object? body = null, CancellationToken ct = default)
        {
            var content = SerializeBody(body);
            var response = await _http.PostAsync(path, content, ct);
            response.EnsureSuccessStatusCode();
            return true;
        }

        public async Task<Stream> GetStream(string path, CancellationToken ct = default)
        {
            var response = await _http.GetAsync(path, HttpCompletionOption.ResponseHeadersRead, ct);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new NOpenCodeRequestException(
                    path, $"Server returned {response.StatusCode}", (int)response.StatusCode, body);
            }
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> PostStream(string path, object? body = null, CancellationToken ct = default)
        {
            var content = SerializeBody(body);
            var request = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = content ?? new StringContent("", Encoding.UTF8, "application/json")
            };
            var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                throw new NOpenCodeRequestException(
                    path, $"Server returned {response.StatusCode}", (int)response.StatusCode, responseBody);
            }
            return await response.Content.ReadAsStreamAsync();
        }

        public async IAsyncEnumerable<string> ReadSse(string path, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, path);
            using var response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            while (!ct.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (line == null) break;
                if (line.StartsWith("data: "))
                {
                    yield return line.Substring(6);
                }
            }
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response, string path, CancellationToken ct)
        {
            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new NOpenCodeRequestException(
                    path, $"Server returned {response.StatusCode}", (int)response.StatusCode, body);
            }
            return JsonSerializer.Deserialize<T>(body, _json)
                ?? throw new NOpenCodeException($"Unexpected null response from {path}");
        }

        private StringContent? SerializeBody(object? body)
        {
            if (body == null) return null;
            var json = JsonSerializer.Serialize(body, _json);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public void Dispose()
        {
            _http?.Dispose();
        }
    }
}
