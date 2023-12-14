namespace Frank.PulseFlow.Internal;

internal class Conduit : IConduit
{
    private readonly IChannel _messageChannel;

    public Conduit(IChannel messageChannel)
    {
        _messageChannel = messageChannel;
    }

    public Task SendAsync(IPulse message)
    {
        return _messageChannel.SendAsync(message);
    }
}