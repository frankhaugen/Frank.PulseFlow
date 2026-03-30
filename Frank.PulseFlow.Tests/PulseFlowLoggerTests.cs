using FluentAssertions;
using Frank.PulseFlow;
using Frank.PulseFlow.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Frank.PulseFlow.Tests;

/// <summary>
/// Validates that common <see cref="ILogger"/> usage patterns do not throw when routed through <see cref="PulseFlowLogger"/>.
/// </summary>
public sealed class PulseFlowLoggerTests
{
    [Fact]
    public void Log_with_string_template_and_parameters_does_not_throw_and_sends_LogPulse()
    {
        var conduit = new CaptureConduit();
        var logger = new PulseFlowLogger("Test.Category", conduit, new StaticOptionsMonitor(new LoggerFilterOptions()));

        var act = () => logger.LogInformation("Hello {UserId} from {Place}", 42, "tests");

        act.Should().NotThrow();

        conduit.Pulses.Should().ContainSingle();
        var pulse = conduit.Pulses[0].Should().BeOfType<LogPulse>().Subject;
        pulse.CategoryName.Should().Be("Test.Category");
        pulse.LogLevel.Should().Be(LogLevel.Information);
        pulse.Message.Should().Contain("42");
        pulse.Message.Should().Contain("tests");
    }

    [Fact]
    public void Log_with_plain_string_sends_LogPulse()
    {
        var conduit = new CaptureConduit();
        var logger = new PulseFlowLogger("Cat", conduit, new StaticOptionsMonitor(new LoggerFilterOptions()));

        logger.LogWarning("Plain message");

        conduit.Pulses.Should().ContainSingle().Which.Should().BeOfType<LogPulse>().Which.Message.Should().Be("Plain message");
    }

    [Theory]
    [InlineData(LogLevel.Trace)]
    [InlineData(LogLevel.Debug)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Critical)]
    public void IsEnabled_returns_true_for_real_levels(LogLevel level)
    {
        var logger = new PulseFlowLogger("x", new CaptureConduit(), new StaticOptionsMonitor(new LoggerFilterOptions()));
        logger.IsEnabled(level).Should().BeTrue();
    }

    [Fact]
    public void IsEnabled_returns_false_for_None()
    {
        var logger = new PulseFlowLogger("x", new CaptureConduit(), new StaticOptionsMonitor(new LoggerFilterOptions()));
        logger.IsEnabled(LogLevel.None).Should().BeFalse();
    }

    /// <summary>
    /// Custom log state types (e.g. third-party) are not structured key-value lists; logging must still work.
    /// </summary>
    [Fact]
    public void Log_within_nested_BeginScope_populates_LogPulse_Scope_outer_to_inner()
    {
        var conduit = new CaptureConduit();
        var logger = new PulseFlowLogger("Test.Category", conduit, new StaticOptionsMonitor(new LoggerFilterOptions()));

        using (logger.BeginScope("outer"))
        using (logger.BeginScope(42))
            logger.LogInformation("inside");

        conduit.Pulses.Should().ContainSingle();
        var pulse = conduit.Pulses[0].Should().BeOfType<LogPulse>().Subject;
        pulse.Scope.Should().NotBeNull();
        pulse.Scope!.Should().Equal("outer", 42);
    }

    [Fact]
    public void Log_when_shutdown_token_is_canceled_throws_operation_canceled()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var conduit = new TokenObservingConduit();
        var logger = new PulseFlowLogger("c", conduit, new StaticOptionsMonitor(new LoggerFilterOptions()), cts.Token);

        var act = () => logger.LogInformation("x");

        act.Should().Throw<OperationCanceledException>();
    }

    [Fact]
    public void Log_with_custom_state_type_does_not_throw()
    {
        var conduit = new CaptureConduit();
        var logger = new PulseFlowLogger("Cat", conduit, new StaticOptionsMonitor(new LoggerFilterOptions()));

        var act = () => logger.Log(LogLevel.Information, 0, new CustomLogState { Detail = "x" }, null, (s, _) => $"formatted:{s.Detail}");

        act.Should().NotThrow();
        conduit.Pulses.Should().ContainSingle().Which.Should().BeOfType<LogPulse>().Which
            .Message.Should().Be("formatted:x");
        ((LogPulse)conduit.Pulses[0]).State.Should().BeNull();
    }

    private sealed class CustomLogState
    {
        public string Detail { get; init; } = "";
    }

    private sealed class CaptureConduit : IConduit
    {
        public List<IPulse> Pulses { get; } = [];

        public Task SendAsync(IPulse message, CancellationToken cancellationToken)
        {
            Pulses.Add(message);
            return Task.CompletedTask;
        }
    }

    /// <summary>Mirrors channel behavior: canceled token yields canceled task.</summary>
    private sealed class TokenObservingConduit : IConduit
    {
        public Task SendAsync(IPulse message, CancellationToken cancellationToken) =>
            cancellationToken.IsCancellationRequested
                ? Task.FromCanceled(cancellationToken)
                : Task.CompletedTask;
    }

    /// <summary>Minimal <see cref="IOptionsMonitor{T}"/> for tests (no change notifications).</summary>
    private sealed class StaticOptionsMonitor(LoggerFilterOptions value) : IOptionsMonitor<LoggerFilterOptions>
    {
        public LoggerFilterOptions CurrentValue { get; } = value;

        public LoggerFilterOptions Get(string? name) => CurrentValue;

        public IDisposable OnChange(Action<LoggerFilterOptions, string?> listener) => NullChangeDisposable.Instance;

        private sealed class NullChangeDisposable : IDisposable
        {
            public static readonly NullChangeDisposable Instance = new();
            public void Dispose() { }
        }
    }
}
