using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NOpenCode;

namespace Examples;

static class Example05_DI
{
    public static async Task Run(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddNOpenCode(cfg =>
        {
            // No model specified — the SDK auto-picks a free model at startup.
        });

        builder.Services.AddHostedService<ReviewWorker>();

        await builder.Build().RunAsync();
    }

    class ReviewWorker(OpenCodeClient AI, IHostApplicationLifetime host) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var reply = await AI
                .Ask("What is the capital of France?")
                .Execute();

            Console.WriteLine(reply);

            host.StopApplication();
        }
    }
}
