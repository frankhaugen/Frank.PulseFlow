namespace Frank.PulseFlow;

/// <summary>
/// Carries information about an <see cref="IFlow"/> that threw while handling a pulse, when diagnostics are enabled.
/// </summary>
public sealed class PulseFlowFaultContext
{
    /// <summary>
    /// Gets the runtime type of the flow implementation that faulted.
    /// </summary>
    public required Type FlowType { get; init; }

    /// <summary>
    /// Gets the pulse being handled when the fault occurred.
    /// </summary>
    public required IPulse Pulse { get; init; }

    /// <summary>
    /// Gets the exception thrown from <see cref="IFlow.HandleAsync"/>.
    /// </summary>
    public required Exception Exception { get; init; }
}
