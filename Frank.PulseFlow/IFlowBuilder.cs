namespace Frank.PulseFlow;

/// <summary>
/// Represents a pulse flow builder that is used to simplify the registration of flows.
/// </summary>
public interface IFlowBuilder
{
    /// <summary>
    /// Adds a flow of type T to the flow builder.
    /// </summary>
    /// <typeparam name="T">The type of flow to add.</typeparam>
    /// <returns>The flow builder instance with the added flow.</returns>
    IFlowBuilder AddFlow<T>() where T : class, IFlow;
}