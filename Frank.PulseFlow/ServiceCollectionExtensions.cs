using Frank.PulseFlow.Internal;

using Microsoft.Extensions.DependencyInjection;

namespace Frank.PulseFlow;

/// <summary>
/// Provides extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the PulseFlow to the service collection.
    /// </summary>
    /// <example>
    /// <code lang="csharp">
    /// services.AddPulseFlow(builder =>
    /// {
    ///   builder.AddFlow[Flow]();
    ///   builder.AddFlow[AnotherFlow]();
    /// });
    /// </code>
    /// </example>
    /// <param name="services">The service collection to which the PulseFlow components will be added.</param>
    /// <param name="configure">An action delegate to configure the PulseFlow builder.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddPulseFlow(this IServiceCollection services, Action<IFlowBuilder> configure)
    {
        FlowBuilder builder = new(services);
        configure(builder);

        services.AddHostedService<PulseNexus>();
        services.AddSingleton<IConduit, Conduit>();
        services.AddSingleton<IChannel, Channel>();
        
        return services;
    }
}