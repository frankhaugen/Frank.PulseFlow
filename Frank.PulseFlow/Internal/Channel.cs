using System.Threading.Channels;

namespace Frank.PulseFlow.Internal;

public class Channel : IChannel
{
    private readonly Channel<IPulse> _channel = System.Threading.Channels.Channel.CreateUnbounded<IPulse>();

    public async Task SendAsync(IPulse message)
    {
        await _channel.Writer.WriteAsync(message);
    }

    public IAsyncEnumerable<IPulse> ReadAllAsync(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}