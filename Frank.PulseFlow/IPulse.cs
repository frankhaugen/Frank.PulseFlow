namespace Frank.PulseFlow;

public interface IPulse
{
    public Guid Id { get; init; }
    public DateTime Created { get; init; }
}