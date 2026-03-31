using System.Diagnostics;

using FluentAssertions;

using Frank.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Xunit.Abstractions;
using Frank.Testing.TestBases;

namespace Frank.PulseFlow.Tests;

public class PulseFlowTests(ITestOutputHelper outputHelper)
    : HostApplicationTestBase(outputHelper, LogLevel.Information, null!)
{
    private readonly ITestOutputHelper _outputHelper = outputHelper;
    private readonly TestPulseContainer _container = new();

    /// <inheritdoc />
    protected override Task SetupAsync(HostApplicationBuilder builder)
    {
        builder.Services.AddPulseFlow(x => x.AddFlow<BlueOutputFlow>());
        builder.Services.AddPulseFlow<RedOutputFlow>();
        builder.Services.AddPulseFlow<TimerPulse, TimerHandler>();
        builder.Services.AddPulseFlow<TimerPulse, TimerHandler2>();
        builder.Services.AddHostedService<MyService>();
        builder.Services.AddSingleton(_container);

        _outputHelper.WriteTable(builder.Services.Select(x => new { Service = x.ServiceType.GetFriendlyName(), Implementation = x.ImplementationType?.GetFriendlyName(), x.Lifetime }).OrderBy(x => x.Service));
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Test1()
    {
        await Task.Delay(500);
        var overview = new[]
        {
            new { Name = "Blue", Count = _container.BlueMessages.Count },
            new { Name = "Red", Count = _container.RedMessages.Count },
            new { Name = "Timer", Count = _container.TimerPulses.Count },
            new { Name = "Timer2", Count = _container.TimerPulses2.Count },
        };

        _outputHelper.WriteTable(overview);

        await Task.Delay(500);

        _container.BlueMessages.Should().NotBeEmpty();
        _container.RedMessages.Should().NotBeEmpty();
        _container.TimerPulses.Should().NotBeEmpty();
        _container.TimerPulses2.Should().NotBeEmpty();
    }

    private class BlueOutputFlow(TestPulseContainer container) : IFlow
    {
        public async Task HandleAsync(IPulse pulse, CancellationToken cancellationToken)
        {
            var thing = pulse as MyMessage;
            container.BlueMessages.Add(thing!);
            await Task.CompletedTask;
        }

        public bool CanHandle(Type pulseType) => pulseType == typeof(MyMessage);
    }

    private class RedOutputFlow(TestPulseContainer container) : IFlow
    {
        public async Task HandleAsync(IPulse pulse, CancellationToken cancellationToken)
        {
            var thing = pulse as MyMessage;
            container.RedMessages.Add(thing!);
            await Task.CompletedTask;
        }

        public bool CanHandle(Type pulseType) => pulseType == typeof(MyMessage);
    }

    private class MyService(IConduit conduit) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopWatch = Stopwatch.StartNew();
            while (!stoppingToken.IsCancellationRequested && stopWatch.Elapsed < TimeSpan.FromSeconds(1))
            {
                await conduit.SendAsync(new MyMessage("Hello, World! " + stopWatch.Elapsed.ToString("c")), stoppingToken);
                await conduit.SendAsync(new TimerPulse { Elapsed = stopWatch.Elapsed }, stoppingToken);
            }
        }
    }

    private class TimerHandler(TestPulseContainer container) : IPulseHandler<TimerPulse>
    {
        public async Task HandleAsync(TimerPulse pulse, CancellationToken cancellationToken)
        {
            container.TimerPulses.Add(pulse);
            await Task.CompletedTask;
        }
    }

    private class TimerHandler2(TestPulseContainer container) : IPulseHandler<TimerPulse>
    {
        public async Task HandleAsync(TimerPulse pulse, CancellationToken cancellationToken)
        {
            container.TimerPulses2.Add(pulse);
            await Task.CompletedTask;
        }
    }

    private class MyMessage(string message) : BasePulse
    {
        public string Message { get; set; } = message;

        public override string ToString() => $"MyMessage: {Message}";
    }

    private class TimerPulse : BasePulse
    {
        public TimeSpan Elapsed { get; set; }
    }

    private class TestPulseContainer
    {
        public List<MyMessage> BlueMessages { get; } = [];
        public List<MyMessage> RedMessages { get; } = [];
        public List<TimerPulse> TimerPulses { get; } = [];
        public List<TimerPulse> TimerPulses2 { get; } = [];
    }
}
