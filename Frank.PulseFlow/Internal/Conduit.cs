namespace Frank.PulseFlow.Internal;

internal class Conduit(IChannel messageChannel) : IConduit
{
    public Task SendAsync(IPulse message)
    {
        return messageChannel.SendAsync(message);
    }
}