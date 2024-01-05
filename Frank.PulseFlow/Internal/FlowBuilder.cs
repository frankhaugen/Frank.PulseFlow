using Microsoft.Extensions.DependencyInjection;

namespace Frank.PulseFlow.Internal;

/// <summary>
/// Represents a builder for configuring the pulse flow.
/// </summary>
internal class FlowBuilder : IFlowBuilder
{
    private readonly IServiceCollection _services;

    /// <summary>
    /// Creates a new instance of the FlowBuilder class with the specified services.
    /// </summary>
    /// <param name="services">The services collection used for dependency injection.</param>
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