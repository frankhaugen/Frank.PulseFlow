namespace Frank.PulseFlow;

/// <summary>
/// Represents a pulse object.
/// </summary>
public interface IPulse
{
    /// <summary>
    /// Gets or inits the unique identifier of the property.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets or inits the created date and time of the object.
    /// </summary>
    public DateTime Created { get; init; }
}