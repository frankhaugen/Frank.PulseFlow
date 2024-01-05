using System.Threading.Channels;

namespace Frank.PulseFlow.Internal;

/// <summary>
/// A wrapper around a <see cref="Channel{T}"/> that sends and receives <see cref="IPulse"/> objects.
/// </summary>
internal class Channel : IChannel
{
    private readonly Channel<IPulse> _channel = System.Threading.Channels.Channel.CreateUnbounded<IPulse>();

    /// <summary>
    /// Sends the provided IPulse message asynchronously.
    /// </summary>
    /// <param name="message">The IPulse message to send.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task SendAsync(IPulse message) => await _channel.Writer.WriteAsync(message);

    /// <summary>
    /// Reads all available pulses asynchronously from the channel.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>An asynchronous enumerable of IPulse objects.</returns>
    public IAsyncEnumerable<IPulse> ReadAllAsync(CancellationToken cancellationToken) => _channel.Reader.ReadAllAsync(cancellationToken);
}