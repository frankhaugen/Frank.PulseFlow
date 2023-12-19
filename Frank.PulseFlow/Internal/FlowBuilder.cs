using Microsoft.Extensions.DependencyInjection;

namespace Frank.PulseFlow.Internal;

internal class FlowBuilder : IFlowBuilder
{
    private readonly IServiceCollection _services;

    public FlowBuilder(IServiceCollection services) => _services = services;

    /// <summary>
    /// Adds a flow of type T to the pulse flow builder.
    /// </summary>
    /// <typeparam name="T">The type of the flow to be added.</typeparam>
    /// <returns>The pulse flow builder instance.</returns>
    public IFlowBuilder AddFlow<T>() where T : class, IFlow
    {
        _services.AddTransient<IFlow, T>();
        return this;
    }
}