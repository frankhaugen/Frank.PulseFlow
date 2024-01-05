using Microsoft.Extensions.Hosting;

namespace Frank.PulseFlow.Internal;

/// <summary>
/// Internal class representing the PulseNexus.
/// </summary>
internal class PulseNexus : BackgroundService
{
    private readonly IChannel _channel;
    private readonly IEnumerable<IFlow> _pulseFlows;

    /// <summary>
    /// Represents a class that provides a nexus for pulse flows.
    /// </summary>
    public PulseNexus(IChannel channel, IEnumerable<IFlow> pulseFlows)
    {
        _channel = channel;
        _pulseFlows = pulseFlows;
    }

    /// <summary>
    /// Executes asynchronous tasks for handling pulses.
    /// </summary>
    /// <param name="stoppingToken">The cancellation token that can be used to stop the execution.</param>
    /// <returns>A task representing the asynchronous execution.</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (IPulse pulse in _channel.ReadAllAsync(stoppingToken))
            await Task.WhenAll(_pulseFlows
                .Where(x => x.CanHandle(pulse.GetType()))
                .Select(flow => flow.HandleAsync(pulse, stoppingToken)));
    }
}