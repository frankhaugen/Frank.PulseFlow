using System.Threading.Channels;

namespace Frank.PulseFlow.Internal;

/// <summary>
/// A wrapper around a <see cref="Channel{T}"/> that sends and receives <see cref="IPulse"/> objects.
/// </summary>
internal class Channel : IChannel
{
    private readonly Channel<IPulse> _channel = System.Threading.Channels.Channel.CreateUnbounded<IPulse>();

    public async Task SendAsync(IPulse message) => await _channel.Writer.WriteAsync(message);

    public IAsyncEnumerable<IPulse> ReadAllAsync(CancellationToken cancellationToken) => _channel.Reader.ReadAllAsync(cancellationToken);
}