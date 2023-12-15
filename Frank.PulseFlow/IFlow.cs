namespace Frank.PulseFlow;

/// <summary>
/// A flow is something that handles a pulse
/// </summary>
public interface IFlow
{
    /// <summary>
    /// Handles the pulse asynchronously.
    /// </summary>
    /// <param name="pulse">The pulse to be handled.</param>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(IPulse pulse, CancellationToken cancellationToken);

    /// <summary>
    /// Determines if the specified pulseType can be handled by this method.
    /// </summary>
    /// <param name="pulseType">The type of pulse.</param>
    /// <returns>True if the pulseType can be handled, false otherwise.</returns>
    bool CanHandle(Type pulseType);
}