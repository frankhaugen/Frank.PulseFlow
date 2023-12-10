using Microsoft.Extensions.Hosting;

namespace Frank.PulseFlow.Internal;

internal class PulseNexus(IChannel channel, IEnumerable<IPulseFlow> pulseFlows) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (IPulse pulse in channel.ReadAllAsync(stoppingToken))
            await Task.WhenAll(pulseFlows
                .Where(x => x.CanHandle(pulse.GetType()))
                .Select(flow => flow.HandleAsync(pulse, stoppingToken)));
    }
}