using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

using Microsoft.Extensions.Options;

namespace Frank.PulseFlow.Internal;

internal class PulseNexus(
    ChannelReader<IPulse> reader,
    IEnumerable<IFlow> pulseFlows,
    IOptions<PulseFlowDiagnosticsOptions> diagnosticsOptions) : BackgroundService
{
    /// <summary>
    /// Caches matching flows per pulse runtime type. Assumes each flow's <see cref="IFlow.CanHandle"/> is
    /// stable for the lifetime of the nexus (typical for singleton flows registered at startup).
    /// </summary>
    private readonly ConcurrentDictionary<Type, IFlow[]> _flowsByPulseType = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (IPulse pulse in reader.ReadAllAsync(stoppingToken))
        {
            var pulseType = pulse.GetType();
            var flows = _flowsByPulseType.GetOrAdd(
                pulseType,
                _ => pulseFlows.Where(x => x.CanHandle(pulseType)).ToArray());
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
            Trace.TraceError(
                "[Frank.PulseFlow] PulseFlowDiagnosticsOptions.UnmatchedPulse callback threw (pulseId={0}): {1}",
                pulse.Id,
                ex);
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
                "[Frank.PulseFlow] Flow {0} failed while handling pulse {1} (pulseId={2}): {3}",
                flow.GetType().FullName,
                pulse.GetType().FullName,
                pulse.Id,
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
            Trace.TraceError(
                "[Frank.PulseFlow] PulseFlowDiagnosticsOptions.FlowFault callback threw (pulseId={0}): {1}",
                pulse.Id,
                ex);
        }
    }
}