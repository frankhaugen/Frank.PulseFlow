using FluentAssertions;
using Frank.PulseFlow;
using Frank.Testing.TestBases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Frank.PulseFlow.Tests;

/// <summary>
/// Ensures one failing <see cref="IFlow"/> does not stop the nexus from processing later pulses.
/// </summary>
public sealed class PulseNexusResilienceTests(ITestOutputHelper outputHelper)
    : HostApplicationTestBase(outputHelper, LogLevel.Information, null!)
{
    private readonly PulseDispatchRecorder _recorder = new();

    protected override Task SetupAsync(HostApplicationBuilder builder)
    {
        builder.Services.AddSingleton(_recorder);
        builder.Services.AddPulseFlow<ThrowOnFirstPulseFlow>();
        builder.Services.AddPulseFlow<RecordingFlow>();
        builder.Services.AddHostedService<TwoPulseSender>();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Subsequent_pulses_are_processed_after_a_flow_throws()
    {
        await Task.Delay(800);

        _recorder.Labels.Should().Equal("first", "second");
    }

    private sealed class LabelPulse : BasePulse
    {
        public required string Label { get; init; }
    }

    private sealed class PulseDispatchRecorder
    {
        public List<string> Labels { get; } = [];
    }

    private sealed class ThrowOnFirstPulseFlow : IFlow
    {
        private int _count;

        public bool CanHandle(Type pulseType) => pulseType == typeof(LabelPulse);

        public Task HandleAsync(IPulse pulse, CancellationToken cancellationToken)
        {
            if (Interlocked.Increment(ref _count) == 1)
                throw new InvalidOperationException("Simulated handler fault");
            return Task.CompletedTask;
        }
    }

    private sealed class RecordingFlow(PulseDispatchRecorder recorder) : IFlow
    {
        public bool CanHandle(Type pulseType) => pulseType == typeof(LabelPulse);

        public Task HandleAsync(IPulse pulse, CancellationToken cancellationToken)
        {
            recorder.Labels.Add(((LabelPulse)pulse).Label);
            return Task.CompletedTask;
        }
    }

    private sealed class TwoPulseSender(IConduit conduit) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await conduit.SendAsync(new LabelPulse { Label = "first" }, stoppingToken);
            await conduit.SendAsync(new LabelPulse { Label = "second" }, stoppingToken);
        }
    }
}
