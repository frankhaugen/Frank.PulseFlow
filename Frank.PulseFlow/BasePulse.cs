namespace Frank.PulseFlow;

public abstract class BasePulse : IPulse
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTime Created { get; init; } = DateTime.UtcNow;
}