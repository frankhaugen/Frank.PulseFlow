namespace Frank.PulseFlow;

/// <summary>
/// Optional callbacks for observing PulseFlow runtime behavior. Configure via the
/// <c>ConfigurePulseFlowDiagnostics</c> extension method or <c>services.Configure&lt;PulseFlowDiagnosticsOptions&gt;(...)</c>.
/// </summary>
public sealed class PulseFlowDiagnosticsOptions
{
    /// <summary>
    /// Invoked when a pulse is read from the channel but no registered <see cref="IFlow"/> returned true from
    /// <see cref="IFlow.CanHandle(System.Type)"/>. Keep callbacks short; they run on the nexus consumer path.
    /// </summary>
    public Action<PulseUnmatchedContext>? UnmatchedPulse { get; set; }

    /// <summary>
    /// Invoked when an <see cref="IFlow"/> throws from <see cref="IFlow.HandleAsync"/> (excluding host cancellation).
    /// <c>System.Diagnostics.Trace.TraceError</c> is still written. Keep callbacks short.
    /// </summary>
    public Action<PulseFlowFaultContext>? FlowFault { get; set; }
}
