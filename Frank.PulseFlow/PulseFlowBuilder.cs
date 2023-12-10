using Microsoft.Extensions.DependencyInjection;

namespace Frank.PulseFlow;

public class PulseFlowBuilder(IServiceCollection services) : IPulseFlowBuilder
{
    /// <summary>
    /// Adds a flow of type T to the pulse flow builder.
    /// </summary>
    /// <typeparam name="T">The type of the flow to be added.</typeparam>
    /// <returns>The pulse flow builder instance.</returns>
    public IPulseFlowBuilder AddFlow<T>() where T : class, IPulseFlow
    {
        services.AddTransient<IPulseFlow, T>();
        return this;
    }
}