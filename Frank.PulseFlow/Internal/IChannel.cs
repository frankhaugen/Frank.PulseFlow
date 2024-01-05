namespace Frank.PulseFlow.Internal;

/// <summary>
/// Represents a channel for sending and receiving pulses.
/// </summary>
internal interface IChannel
{
    /// <summary>
    /// Sends an asynchronous pulse message.
    /// </summary>
    /// <param name="message">The pulse message to send.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendAsync(IPulse message);

    /// <summary>
    /// Reads all available pulses asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token used to cancel the operation.</param>
    /// <returns>An asynchronous enumerable of IPulse objects.</returns>
    IAsyncEnumerable<IPulse> ReadAllAsync(CancellationToken cancellationToken);
}