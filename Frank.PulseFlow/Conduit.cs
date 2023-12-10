using Frank.PulseFlow.Internal;

namespace Frank.PulseFlow;

public class Conduit(IChannel messageChannel) : IConduit
{
    public Task SendAsync(IPulse message)
    {
        return messageChannel.SendAsync(message);
    }
}