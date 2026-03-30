namespace Frank.PulseFlow;

/// <summary>
/// Represents a pulse object.
/// </summary>
public interface IPulse
{
    /// <summary>
    /// Gets or inits the unique identifier of the pulse.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or inits when the pulse was created.
    /// </summary>
    public DateTime Created { get; init; }
}