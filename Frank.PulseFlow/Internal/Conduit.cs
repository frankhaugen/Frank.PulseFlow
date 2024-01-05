namespace Frank.PulseFlow.Internal;

/// <summary>
/// A wrapper around a <see cref="Channel{T}"/> that sends and receives <see cref="IPulse"/> objects.
/// </summary>
internal class Conduit : IConduit
{
    private readonly IChannel _messageChannel;

    /// <summary>
    /// Represents a conduit for message communication between channels.
    /// </summary>
    /// <param name="messageChannel">The channel used for message communication.</param>
    public Conduit(IChannel messageChannel) => _messageChannel = messageChannel;

    /// <summary>
    /// Sends a pulse message asynchronously.
    /// </summary>
    /// <param name="message">The pulse message to be sent.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task SendAsync(IPulse message) => _messageChannel.SendAsync(message);
}