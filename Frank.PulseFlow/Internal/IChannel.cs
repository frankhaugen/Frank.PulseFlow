namespace Frank.PulseFlow.Internal;

internal interface IChannel
{
    Task SendAsync(IPulse message);
    IAsyncEnumerable<IPulse> ReadAllAsync(CancellationToken cancellationToken);
}