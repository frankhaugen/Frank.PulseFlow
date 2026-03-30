using System.Diagnostics;
using System.Linq;

namespace Frank.PulseFlow.Internal;

internal class PulseNexus(ChannelReader<IPulse> reader, IEnumerable<IFlow> pulseFlows) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (IPulse pulse in reader.ReadAllAsync(stoppingToken))
        {
            var flows = pulseFlows.Where(x => x.CanHandle(pulse.GetType())).ToArray();
            if (flows.Length == 0)
                continue;

            await Task.WhenAll(flows.Select(flow => InvokeFlowAsync(flow, pulse, stoppingToken)));
        }
    }

    private static async Task InvokeFlowAsync(IFlow flow, IPulse pulse, CancellationToken stoppingToken)
    {
        try
        {
            await flow.HandleAsync(pulse, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            Trace.TraceError(
                "[Frank.PulseFlow] Flow {0} failed while handling pulse {1}: {2}",
                flow.GetType().FullName,
                pulse.GetType().FullName,
                ex);
        }
    }
}