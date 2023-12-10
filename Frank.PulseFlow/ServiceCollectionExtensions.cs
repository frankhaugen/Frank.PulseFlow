using Frank.PulseFlow.Internal;

using Microsoft.Extensions.DependencyInjection;

namespace Frank.PulseFlow;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the PulseFlow to the service collection.
    /// </summary>
    /// <param name="services">The service collection to which the PulseFlow components will be added.</param>
    /// <param name="configure">An action delegate to configure the PulseFlow builder.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddPulseFlow(this IServiceCollection services, Action<IPulseFlowBuilder> configure)
    {
        PulseFlowBuilder builder = new(services);
        configure(builder);

        services.AddHostedService<PulseNexus>();
        services.AddSingleton<IConduit, Conduit>();
        services.AddSingleton<IChannel, Channel>();

        return services;
    }
}