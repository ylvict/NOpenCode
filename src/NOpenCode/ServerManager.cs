using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace NOpenCode
{
    internal class ServerManager : IAsyncDisposable
    {
        private static readonly ConcurrentDictionary<int, ServerManager> ActiveServers = new();

        private static readonly int[] DefaultPorts = { 4096, 54321, 9123 };

        private Process? _process;
        private readonly string _baseUrl;
        private readonly int _port;
        private readonly bool _externallyManaged;
        private readonly string? _dataDir;
        private bool _disposed;

        public string BaseUrl => _baseUrl;

        private ServerManager(string baseUrl, int port, bool externallyManaged, string? dataDir = null)
        {
            _baseUrl = baseUrl;
            _port = port;
            _externallyManaged = externallyManaged;
            _dataDir = dataDir;
        }

        public static async Task<ServerManager> Start(ServerOptions options)
        {
            var existing = await TryReuseExisting(options);
            if (existing != null)
                return existing;

            return await StartNewServer(options);
        }

        private static async Task<ServerManager?> TryReuseExisting(ServerOptions options)
        {
            var ports = new List<int>();

            if (options.Port.HasValue)
                ports.Add(options.Port.Value);

            foreach (var kvp in ActiveServers)
            {
                if (!ports.Contains(kvp.Key))
                    ports.Add(kvp.Key);
            }

            ports.AddRange(DefaultPorts);

            foreach (var port in ports)
            {
                var url = $"http://127.0.0.1:{port}";
                if (await IsHealthy(url))
                    return new ServerManager(url, port, externallyManaged: true);
            }

            return null;
        }

        private static async Task<ServerManager> StartNewServer(ServerOptions options)
        {
            var cliPath = FindOpenCodeCli();
            if (cliPath == null)
                throw new NOpenCodeNotInstalledException();

            var port = options.Port ?? GetRandomPort();
            var url = $"http://127.0.0.1:{port}";

            var dataDir = Path.Combine(Path.GetTempPath(), "opencode-sdk", Path.GetRandomFileName());
            Directory.CreateDirectory(dataDir);

            var process = StartProcess(cliPath, port, dataDir);

            var mgr = new ServerManager(url, port, externallyManaged: false, dataDir)
            {
                _process = process
            };

            ActiveServers[port] = mgr;

            await WaitForHealthOrThrow(process, url, port,
                TimeSpan.FromSeconds(options.StartTimeoutSeconds));

            return mgr;
        }

        private static Process StartProcess(string cliPath, int port, string dataDir)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = cliPath,
                    Arguments = $"serve --port {port} --hostname 127.0.0.1",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                },
                EnableRaisingEvents = true
            };

            process.StartInfo.EnvironmentVariables["XDG_DATA_HOME"] = dataDir;

            process.Start();
            return process;
        }

        private static async Task WaitForHealthOrThrow(Process process, string url, int port, TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow + timeout;

            while (DateTime.UtcNow < deadline)
            {
                await Task.Delay(500);

                if (process.HasExited)
                {
                    ActiveServers.TryRemove(port, out _);
                    var error = await process.StandardError.ReadToEndAsync();
                    throw new NOpenCodeServerException(
                        $"opencode serve exited immediately: {error}");
                }

                if (await IsHealthy(url))
                    return;
            }

            ActiveServers.TryRemove(port, out _);
            throw new NOpenCodeServerException(
                $"opencode serve did not become healthy within {timeout.TotalSeconds}s");
        }

        private static string? FindOpenCodeCli()
        {
            var fromWhere = FindCliViaWhere();
            if (fromWhere != null)
                return fromWhere;

            if (CanRunDirectly())
                return "opencode";

            var npmCli = FindNpmGlobalCli();
            if (npmCli != null)
                return npmCli;

            return null;
        }

        private static string? FindNpmGlobalCli()
        {
            try
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                if (string.IsNullOrEmpty(home))
                    return null;

                var candidates = new[]
                {
                    Path.Combine(home, ".opencode", "bin", "opencode"),
                    Path.Combine(home, ".opencode", "bin", "opencode.cmd"),
                };

                if (!IsWindows())
                {
                    Array.Resize(ref candidates, 1);
                }

                foreach (var candidate in candidates)
                {
                    if (File.Exists(candidate))
                        return candidate;
                }
            }
            catch
            {
            }

            return null;
        }

        private static string? FindCliViaWhere()
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

                if (proc.ExitCode != 0)
                    return null;

                var path = output.Trim().Split('\n')[0].Trim('\r');
                if (string.IsNullOrEmpty(path))
                    return null;

                if (IsWindows() && !IsExecutable(path))
                {
                    var cmdPath = path + ".cmd";
                    if (File.Exists(cmdPath))
                        return cmdPath;
                }

                return path;
            }
            catch
            {
                return null;
            }
        }

        private static bool CanRunDirectly()
        {
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
                proc.StandardOutput.ReadToEnd();
                proc.WaitForExit(3000);
                return proc.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }

        private static async Task<bool> IsHealthy(string url)
        {
            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
                var response = await client.GetAsync($"{url}/global/health");
                if (!response.IsSuccessStatusCode)
                    return false;

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

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            ActiveServers.TryRemove(_port, out _);

            if (_externallyManaged || _process == null || _process.HasExited)
                return;

            try
            {
                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
                await client.PostAsync($"{_baseUrl}/instance/dispose", null);
            }
            catch { }

            try
            {
                KillProcessTree(_process);
            }
            catch { }

            _process.Dispose();
            _process = null;

            CleanupDataDir();
        }

        private void CleanupDataDir()
        {
            if (_dataDir == null)
                return;

            try
            {
                if (Directory.Exists(_dataDir))
                    Directory.Delete(_dataDir, recursive: true);
            }
            catch { }
        }

        private static void KillProcessTree(Process process)
        {
            if (IsWindows())
            {
                using var killer = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "taskkill",
                        Arguments = $"/F /T /PID {process.Id}",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    }
                };
                killer.Start();
                killer.WaitForExit(5000);
            }
            else
            {
                process.Kill();
                process.WaitForExit(5000);
            }
        }

        private static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        private static bool IsExecutable(string path) =>
            path.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(".bat", StringComparison.OrdinalIgnoreCase);

        private static int GetRandomPort() =>
            new Random().Next(10000, 60000);
    }

    public class ServerOptions
    {
        public int? Port { get; set; }
        public int StartTimeoutSeconds { get; set; } = 30;
    }
}