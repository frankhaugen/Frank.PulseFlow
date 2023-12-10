// See https://aka.ms/new-console-template for more information

using Frank.PulseFlow;
using Frank.PulseFlow.Tests.Cli;

IHostBuilder builder = Host.CreateDefaultBuilder();

builder.ConfigureServices((context, services) =>
{
    services.AddPulseFlow(messagingBuilder =>
    {
        messagingBuilder.AddFlow<TextPulseFlow>();
    });
    services.AddHostedService<TestingService>();
});

IHost app = builder.Build();


await app.RunAsync();


//Extension Method