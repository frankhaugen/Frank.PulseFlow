namespace Frank.PulseFlow;

public abstract class BasePulse : IPulse
{
    /// <summary>
    /// The unique identifier of the pulse.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();
    
    /// <summary>
    /// The date and time the pulse was created.
    /// </summary>
    public DateTime Created { get; init; } = DateTime.UtcNow;
}