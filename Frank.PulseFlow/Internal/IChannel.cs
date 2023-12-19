namespace Frank.PulseFlow.Internal;

/// <summary>
/// Represents a channel for sending and receiving pulses.
/// </summary>
internal interface IChannel
{
    Task SendAsync(IPulse message);
    IAsyncEnumerable<IPulse> ReadAllAsync(CancellationToken cancellationToken);
}