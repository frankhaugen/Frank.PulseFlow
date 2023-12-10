using Frank.PulseFlow.Internal;

using Microsoft.Extensions.Hosting;

namespace Frank.PulseFlow;

public class PulseNexus : BackgroundService
{
    private readonly IChannel _channel;
    private readonly IEnumerable<IPulseFlow> _pulseFlows;

    public PulseNexus(IChannel channel, IEnumerable<IPulseFlow> pulseFlows)
    {
        _channel = channel;
        _pulseFlows = pulseFlows;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (IPulse pulse in _channel.ReadAllAsync(stoppingToken))
        {
            await Task.WhenAll(_pulseFlows.Where(x => x.AppliesTo == pulse.GetType())
                .Select(flow => flow.HandleAsync(pulse, stoppingToken)));
        }
    }
}