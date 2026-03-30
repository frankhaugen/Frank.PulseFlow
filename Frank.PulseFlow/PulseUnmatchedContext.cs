namespace Frank.PulseFlow;

/// <summary>
/// Carries information about a pulse that had no matching <see cref="IFlow"/> when diagnostics are enabled.
/// </summary>
public sealed class PulseUnmatchedContext
{
    /// <summary>
    /// Initializes a new instance from the pulse instance.
    /// </summary>
    /// <param name="pulse">The pulse that was not dispatched to any flow.</param>
    public PulseUnmatchedContext(IPulse pulse)
    {
        Pulse = pulse;
        PulseType = pulse.GetType();
    }

    /// <summary>
    /// Gets the pulse instance.
    /// </summary>
    public IPulse Pulse { get; }

    /// <summary>
    /// Gets the runtime type of the pulse (<see cref="object.GetType"/>).
    /// </summary>
    public Type PulseType { get; }
}
