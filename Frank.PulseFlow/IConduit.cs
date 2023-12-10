namespace Frank.PulseFlow;

/// <summary>
/// Represents a conduit to the underlying infrastructure for sending pulses.
/// </summary>
public interface IConduit
{
    /// <summary>
    /// Sends a pulse to the underlying infrastructure.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    Task SendAsync(IPulse message);
}