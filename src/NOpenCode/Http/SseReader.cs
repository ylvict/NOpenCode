using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    internal class SseReader : IDisposable
    {
        private readonly StreamReader _reader;

        public SseReader(Stream stream)
        {
            _reader = new StreamReader(stream);
        }

        public async Task<SseEvent?> ReadEventAsync(CancellationToken ct = default)
        {
            string? data = null;
            string? eventType = null;

            while (!ct.IsCancellationRequested)
            {
                var line = await _reader.ReadLineAsync();
                if (line == null) return null;

                if (line.StartsWith("event: "))
                    eventType = line.Substring(7);
                else if (line.StartsWith("data: "))
                    data = line.Substring(6);
                else if (line == "")
                {
                    if (data != null)
                        return new SseEvent { Type = eventType ?? "message", Data = data };
                    continue;
                }
            }

            return null;
        }

        public void Dispose()
        {
            _reader?.Dispose();
        }
    }

    public class SseEvent
    {
        public string Type { get; set; } = "message";
        public string Data { get; set; } = "";
    }
}
