namespace Frank.PulseFlow;

public interface IPulseFlowBuilder
{
    /// <summary>
    /// Adds a flow of type T to the pulse flow builder.
    /// </summary>
    /// <typeparam name="T">The type of flow to add.</typeparam>
    /// <returns>The pulse flow builder instance with the added flow.</returns>
    IPulseFlowBuilder AddFlow<T>() where T : class, IPulseFlow;
}