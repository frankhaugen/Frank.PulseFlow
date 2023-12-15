using Microsoft.Extensions.Hosting;

namespace Frank.PulseFlow.Internal;

internal class PulseNexus : BackgroundService
{
    private readonly IChannel _channel;
    private readonly IEnumerable<IFlow> _pulseFlows;

    public PulseNexus(IChannel channel, IEnumerable<IFlow> pulseFlows)
    {
        _channel = channel;
        _pulseFlows = pulseFlows;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (IPulse pulse in _channel.ReadAllAsync(stoppingToken))
            await Task.WhenAll(_pulseFlows
                .Where(x => x.CanHandle(pulse.GetType()))
                .Select(flow => flow.HandleAsync(pulse, stoppingToken)));
    }
}