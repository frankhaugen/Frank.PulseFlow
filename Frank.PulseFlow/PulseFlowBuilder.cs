using Microsoft.Extensions.DependencyInjection;

namespace Frank.PulseFlow;

public class PulseFlowBuilder : IPulseFlowBuilder
{
    private readonly IServiceCollection _services;

    public PulseFlowBuilder(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    /// Adds a flow of type T to the pulse flow builder.
    /// </summary>
    /// <typeparam name="T">The type of the flow to be added.</typeparam>
    /// <returns>The pulse flow builder instance.</returns>
    public IPulseFlowBuilder AddFlow<T>() where T : class, IPulseFlow
    {
        _services.AddTransient<IPulseFlow, T>();
        return this;
    }
}