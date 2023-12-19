namespace Frank.PulseFlow.Internal;

/// <summary>
/// A wrapper around a <see cref="Channel{T}"/> that sends and receives <see cref="IPulse"/> objects.
/// </summary>
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