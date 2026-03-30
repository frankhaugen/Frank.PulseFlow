namespace Frank.PulseFlow;

/// <summary>
/// Handles pulses of type <typeparamref name="T"/> for PulseFlow.
/// </summary>
/// <typeparam name="T">The pulse type to handle.</typeparam>
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