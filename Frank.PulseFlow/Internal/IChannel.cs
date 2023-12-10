namespace Frank.PulseFlow.Internal;

public interface IChannel
{
    Task SendAsync(IPulse message);
    IAsyncEnumerable<IPulse> ReadAllAsync(CancellationToken cancellationToken);
}