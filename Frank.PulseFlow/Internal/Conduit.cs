namespace Frank.PulseFlow.Internal;

internal class Conduit(ChannelWriter<IPulse> writer) : IConduit
{
    public async Task SendAsync(IPulse message, CancellationToken cancellationToken) => await writer.WriteAsync(message, cancellationToken);
}