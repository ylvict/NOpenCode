using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NOpenCode
{
    public static class NOpenCodeServiceCollectionExtensions
    {
        public static IServiceCollection AddNOpenCode(this IServiceCollection services)
        {
            return AddNOpenCode(services, _ => { });
        }

        public static IServiceCollection AddNOpenCode(this IServiceCollection services, Action<NOpenCodeBuilder> configure)
        {
            var builder = new NOpenCodeBuilder();
            configure(builder);

            services.AddSingleton(builder);
            services.AddSingleton<OpenCodeClient>(sp =>
            {
                var client = builder.Launch()
                    .GetAwaiter().GetResult();
                return client;
            });

            services.AddSingleton<IHostedService>(sp =>
            {
                var client = sp.GetRequiredService<OpenCodeClient>();
                return new NOpenCodeHostedService(client);
            });

            return services;
        }
    }

    internal class NOpenCodeHostedService : IHostedService
    {
        private readonly OpenCodeClient _client;

        public NOpenCodeHostedService(OpenCodeClient client)
        {
            _client = client;
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.DisposeAsync();
        }
    }
}
