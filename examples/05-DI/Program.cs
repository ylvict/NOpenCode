using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NOpenCode;

// DI integration — register NOpenCode in the service container.
// In a real app this would be in your Program.cs or Startup.cs.
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddNOpenCode(cfg => cfg
    .WithModel("opencode/deepseek-v4-flash-free")
);

builder.Services.AddHostedService<ReviewWorker>();

await builder.Build().RunAsync();

// --- Worker using injected OpenCodeClient ---

public class ReviewWorker(OpenCodeClient AI) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var reply = await AI
            .Ask("What is the capital of France?")
            .Execute();

        Console.WriteLine(reply);

        // The hosted service lifecycle manages the server:
        //   - StartAsync  → server is available
        //   - StopAsync   → server is disposed
    }
}
