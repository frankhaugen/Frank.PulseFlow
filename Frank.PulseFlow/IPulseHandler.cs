namespace Frank.PulseFlow;

/// IPulseHandler Interface
/// Represents a handler for processing pulses.
/// /// <typeparam name="T">The type of pulse to handle.</typeparam>
public interface IPulseHandler<in T> where T : IPulse
{
    /// <summary>
    /// Handles the pulse asynchronously.
    /// </summary>
    /// <param name="pulse">The pulse to be handled.</param>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(T pulse, CancellationToken cancellationToken);
}