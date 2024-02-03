namespace Frank.PulseFlow.Internal;

internal class PulseNexus(ChannelReader<IPulse> reader, IEnumerable<IFlow> pulseFlows) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (IPulse pulse in reader.ReadAllAsync(stoppingToken))
            await Task.WhenAll(pulseFlows
                .Where(x => x.CanHandle(pulse.GetType()))
                .Select(flow => flow.HandleAsync(pulse, stoppingToken)));
    }
}