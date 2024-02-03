using System.Diagnostics;

using FluentAssertions;

using Frank.PulseFlow.Logging;
using Frank.Reflection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Xunit.Abstractions;
using Frank.Testing.TestBases;

using Microsoft.Extensions.Options;

namespace Frank.PulseFlow.Tests;

public class PulseFlowTests(ITestOutputHelper outputHelper) : HostApplicationTestBase(outputHelper)
{
    private readonly ITestOutputHelper _outputHelper = outputHelper;
    private readonly TestPulseContainer _container = new();

    /// <inheritdoc />
    protected override Task SetupAsync(HostApplicationBuilder builder)
    {
        builder.Logging.AddPulseFlow();
        
        builder.Services.AddPulseFlow<FileLoggerFlow>();
        builder.Services.AddPulseFlow(x => x.AddFlow<BlueOutputFlow>().AddFlow<TestOutputHelperFlow>());
        builder.Services.AddPulseFlow<RedOutputFlow>();
        builder.Services.AddPulseFlow<TimerPulse, TimerHandler>();
        builder.Services.AddPulseFlow<TimerPulse, TimerHandler2>();
        builder.Services.AddHostedService<MyService>();
        builder.Services.AddSingleton(_container);
        
        builder.Services.Configure<FileLoggerSettings>(x => x.LogPath = "logs.log");
        _outputHelper.WriteTable(builder.Services.Select(x => new { Service = x.ServiceType.GetFriendlyName(), Implementation = x.ImplementationType?.GetFriendlyName(), x.Lifetime }).OrderBy(x => x.Service));
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Test1()
    {
        await Task.Delay(500);
        var overview = new []
        {
            new
            {
                Name = "Blue", Count = _container.BlueMessages.Count,
            },
            new
            {
                Name = "Red", Count = _container.RedMessages.Count,
            },
            new
            {
                Name = "Log", Count = _container.LogMessages.Count,
            },
            new
            {
                Name = "Timer", Count = _container.TimerPulses.Count,
            },
            new
            {
                Name = "Timer2", Count = _container.TimerPulses2.Count,
            },
        };
        
        _outputHelper.WriteTable(overview);
        
        await Task.Delay(500);
        
        _container.BlueMessages.Should().NotBeEmpty();
        _container.RedMessages.Should().NotBeEmpty();
        _container.LogMessages.Should().NotBeEmpty();
        _container.TimerPulses.Should().NotBeEmpty();
        _container.TimerPulses2.Should().NotBeEmpty();
    }

    private class TestOutputHelperFlow(TestPulseContainer container) : IFlow
    {
        public async Task HandleAsync(IPulse pulse, CancellationToken cancellationToken)
        {
            var thing = pulse as LogPulse;
            container.LogMessages.Add(thing!);
            await Task.CompletedTask;
        }

        public bool CanHandle(Type pulseType)
        {
            return pulseType == typeof(LogPulse);
        }
    }
    
    private class BlueOutputFlow(TestPulseContainer container) : IFlow
    {
        public async Task HandleAsync(IPulse pulse, CancellationToken cancellationToken)
        {
            var thing = pulse as MyMessage;
            container.BlueMessages.Add(thing!);
            await Task.CompletedTask;
        }

        public bool CanHandle(Type pulseType)
        {
            return pulseType == typeof(MyMessage);
        }
    }
    
    private class RedOutputFlow(TestPulseContainer container) : IFlow
    {
        public async Task HandleAsync(IPulse pulse, CancellationToken cancellationToken)
        {
            var thing = pulse as MyMessage;
            container.RedMessages.Add(thing!);
            await Task.CompletedTask;
        }

        public bool CanHandle(Type pulseType)
        {
            return pulseType == typeof(MyMessage);
        }
    }
    
    private class MyService(IConduit conduit) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var stopWatch = Stopwatch.StartNew();
            while (!stoppingToken.IsCancellationRequested && stopWatch.Elapsed < TimeSpan.FromSeconds(1))
            {
                await conduit.SendAsync(new MyMessage("Hello, World! " + stopWatch.Elapsed.ToString("c")), stoppingToken);
                await conduit.SendAsync(new TimerPulse() {Elapsed = stopWatch.Elapsed}, stoppingToken);
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
        public List<MyMessage> BlueMessages { get; } = new();
        public List<MyMessage> RedMessages { get; } = new();
        public List<LogPulse> LogMessages { get; } = new();
        public List<TimerPulse> TimerPulses { get; } = new();
        public List<TimerPulse> TimerPulses2 { get; } = new();
        
    }
    
    public class FileLoggerFlow(IOptions<FileLoggerSettings> options) : IFlow
    {
        private readonly FileLoggerSettings _settings = options.Value;
        
        public async Task HandleAsync(IPulse pulse, CancellationToken cancellationToken)
        {
            var thing = pulse as LogPulse;
            await File.AppendAllTextAsync(_settings.LogPath!, thing! + Environment.NewLine, cancellationToken);
            await Task.CompletedTask;
        }

        public bool CanHandle(Type pulseType) => pulseType == typeof(LogPulse);
    }

    public class FileLoggerSettings
    {
        public string? LogPath { get; set; }
    }
}
