using Frank.PulseFlow;
using Frank.PulseFlow.Tests.Cli;

IHostBuilder builder = Host.CreateDefaultBuilder();

builder.ConfigureServices((context, services) =>
{
    services.AddPulseFlow(messagingBuilder =>
    {
        messagingBuilder.AddFlow<TextFlow>();
    });
    services.AddHostedService<TestingService>();
});

IHost app = builder.Build();

await app.RunAsync();