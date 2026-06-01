using System;
using System.Threading;
using System.Threading.Tasks;

namespace NOpenCode
{
    public class EventClient
    {
        private readonly NOpenCodeHttpClient _http;

        internal EventClient(NOpenCodeHttpClient http)
        {
            _http = http;
        }

        public async Task Subscribe(
            Action<SseEvent> onEvent,
            Action<Exception>? onError = null,
            Action? onEnd = null,
            CancellationToken ct = default)
        {
            try
            {
                var stream = await _http.GetStream("/event", ct);
                using var reader = new SseReader(stream);

                while (!ct.IsCancellationRequested)
                {
                    var evt = await reader.ReadEventAsync(ct);
                    if (evt == null) break;
                    onEvent(evt);
                }

                onEnd?.Invoke();
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
        }
    }
}
