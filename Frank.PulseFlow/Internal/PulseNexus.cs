using System.Diagnostics;
using System.Linq;

using Microsoft.Extensions.Options;

namespace Frank.PulseFlow.Internal;

internal class PulseNexus(
    ChannelReader<IPulse> reader,
    IEnumerable<IFlow> pulseFlows,
    IOptions<PulseFlowDiagnosticsOptions> diagnosticsOptions) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (IPulse pulse in reader.ReadAllAsync(stoppingToken))
        {
            var flows = pulseFlows.Where(x => x.CanHandle(pulse.GetType())).ToArray();
            if (flows.Length == 0)
            {
                NotifyUnmatched(pulse);
                continue;
            }

            await Task.WhenAll(flows.Select(flow => InvokeFlowAsync(flow, pulse, stoppingToken)));
        }
    }

    private void NotifyUnmatched(IPulse pulse)
    {
        var callback = diagnosticsOptions.Value.UnmatchedPulse;
        if (callback is null)
            return;

        try
        {
            callback(new PulseUnmatchedContext(pulse));
        }
        catch (Exception ex)
        {
            Trace.TraceError("[Frank.PulseFlow] PulseFlowDiagnosticsOptions.UnmatchedPulse callback threw: {0}", ex);
        }
    }

    private async Task InvokeFlowAsync(IFlow flow, IPulse pulse, CancellationToken stoppingToken)
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
            NotifyFlowFault(flow, pulse, ex);
        }
    }

    private void NotifyFlowFault(IFlow flow, IPulse pulse, Exception exception)
    {
        var callback = diagnosticsOptions.Value.FlowFault;
        if (callback is null)
            return;

        try
        {
            callback(new PulseFlowFaultContext
            {
                FlowType = flow.GetType(),
                Pulse = pulse,
                Exception = exception
            });
        }
        catch (Exception ex)
        {
            Trace.TraceError("[Frank.PulseFlow] PulseFlowDiagnosticsOptions.FlowFault callback threw: {0}", ex);
        }
    }
}