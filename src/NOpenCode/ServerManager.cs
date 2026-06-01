using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    internal class ServerManager : IAsyncDisposable
    {
        private Process? _process;
        private readonly string _baseUrl;
        private readonly bool _externallyManaged;
        private bool _disposed;

        public string BaseUrl => _baseUrl;
        public bool IsExternallyManaged => _externallyManaged;

        private ServerManager(string baseUrl, bool externallyManaged)
        {
            _baseUrl = baseUrl;
            _externallyManaged = externallyManaged;
        }

        public static async Task<ServerManager> Start(ServerOptions options)
        {
            var portsToTry = new List<int>();
            if (options.Port.HasValue)
                portsToTry.Add(options.Port.Value);
            portsToTry.AddRange(new[] { 4096, 54321, 9123 });

            foreach (var port in portsToTry)
            {
                var url = $"http://127.0.0.1:{port}";
                if (await TryHealthCheck(url))
                {
                    return new ServerManager(url, externallyManaged: true);
                }
            }

            var cliPath = FindOpenCodeCli();
            if (cliPath == null)
                throw new NOpenCodeNotInstalledException();

            var randomPort = options.Port ?? GetRandomPort();
            var serverUrl = $"http://127.0.0.1:{randomPort}";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = cliPath,
                    Arguments = $"serve --port {randomPort} --hostname 127.0.0.1",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            process.Start();

            var timeout = TimeSpan.FromSeconds(options.StartTimeoutSeconds);
            var deadline = DateTime.UtcNow + timeout;

            while (DateTime.UtcNow < deadline)
            {
                await Task.Delay(500);
                if (process.HasExited)
                {
                    var stderr = await process.StandardError.ReadToEndAsync();
                    throw new NOpenCodeServerException(
                        $"opencode serve exited immediately: {stderr}");
                }
                if (await TryHealthCheck(serverUrl))
                {
                    var mgr = new ServerManager(serverUrl, externallyManaged: false);
                    mgr._process = process;
                    return mgr;
                }
            }

            throw new NOpenCodeServerException(
                $"opencode serve did not become healthy within {timeout.TotalSeconds}s");
        }

        private static async Task<bool> TryHealthCheck(string url)
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
                var response = await client.GetAsync($"{url}/global/health");
                if (!response.IsSuccessStatusCode) return false;
                var body = await response.Content.ReadAsStringAsync();
                var health = JsonSerializer.Deserialize<HealthInfo>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return health?.Healthy == true;
            }
            catch
            {
                return false;
            }
        }

        private static string? FindOpenCodeCli()
        {
            try
            {
                using var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "where",
                        Arguments = "opencode",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                proc.Start();
                var output = proc.StandardOutput.ReadToEnd();
                proc.WaitForExit(3000);
                if (proc.ExitCode == 0)
                {
                    var line = output.Trim().Split('\n')[0].Trim('\r');
                    if (string.IsNullOrEmpty(line))
                        return null;
                    if (IsWindows() && !IsExecutable(line))
                    {
                        var cmdPath = line + ".cmd";
                        if (System.IO.File.Exists(cmdPath))
                            return cmdPath;
                    }
                    return line;
                }
            }
            catch { }

            try
            {
                using var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "opencode",
                        Arguments = "--version",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                proc.Start();
                proc.WaitForExit(3000);
                if (proc.ExitCode == 0)
                    return "opencode";
            }
            catch { }

            return null;
        }

        private static bool IsWindows()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        private static bool IsExecutable(string path)
        {
            return path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase)
                || path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase);
        }

        private static int GetRandomPort()
        {
            return new Random().Next(10000, 60000);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            if (!_externallyManaged && _process != null && !_process.HasExited)
            {
                try
                {
                    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
                    await client.PostAsync($"{_baseUrl}/instance/dispose", null);
                }
                catch { }

                try
                {
                    _process.Kill();
                    _process.WaitForExit(5000);
                }
                catch { }

                _process.Dispose();
                _process = null;
            }
        }
    }

    public class ServerOptions
    {
        public int? Port { get; set; }
        public int StartTimeoutSeconds { get; set; } = 30;
    }
}
