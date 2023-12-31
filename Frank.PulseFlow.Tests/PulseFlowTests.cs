using Frank.PulseFlow.Logging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;
using Frank.Reflection;

namespace Frank.PulseFlow.Tests;

public class PulseFlowTests
{
    private readonly ITestOutputHelper _outputHelper;

    public PulseFlowTests(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public void Test1()
    {
        var host = CreateHostBuilder().Build();
        
        host.Start();
    }

    private class MyService : BackgroundService
    {
        private readonly ILogger<MyService> _logger;

        public MyService(ILogger<MyService> logger) => _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Hello from {ServiceName}", nameof(MyService));
            try
            {
                throw new Exception("This is an exception");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "This is an exception in {ServiceName}", nameof(MyService));
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
    
    private IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddPulseFlow();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<ITestOutputHelper>(_outputHelper);
                services.AddPulseFlow(builder =>
                {
                    builder.AddFlow<TestOutputFlow>();
                });
                
                services.AddHostedService<MyService>();
            });
    }

    private class TestOutputFlow : IFlow
    {
        private readonly ITestOutputHelper _outputHelper;

        public TestOutputFlow(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        public async Task HandleAsync(IPulse pulse, CancellationToken cancellationToken)
        {
            var thing = pulse as LogPulse;
            var message = thing!.ToString();
            _outputHelper.WriteLine(message);
            await Task.CompletedTask;
        }

        public bool CanHandle(Type pulseType)
        {
            _outputHelper.WriteLine($"CanHandle: {pulseType.GetFriendlyName()}");
            return pulseType.BaseType == typeof(LogPulse);
        }
    }
}