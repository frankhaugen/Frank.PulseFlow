namespace Frank.PulseFlow.Logging;

/// <summary>
/// Represents a logger scope that can pulse flow and logs a state of type <typeparamref name="TState"/>.
/// </summary>
/// <typeparam name="TState">The type of the state to be logged.</typeparam>
public class PulseFlowLoggerScope<TState> : IDisposable
{
    /// <summary>
    /// Gets or sets the state of the object.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <returns>The current state of the object.</returns>
    public TState? State { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PulseFlowLoggerScope{TState}"/> class with the specified state.
    /// </summary>
    /// <param name="state">The state to assign to the logger scope.</param>
    public PulseFlowLoggerScope(TState state) => State = state;

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting resources.
    /// </summary>
    public void Dispose() => State = default;
}