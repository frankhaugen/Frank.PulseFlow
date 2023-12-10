namespace Frank.PulseFlow;

public interface IPulseFlow
{
    Task HandleAsync(IPulse pulse, CancellationToken cancellationToken);

    Type AppliesTo { get; }
}