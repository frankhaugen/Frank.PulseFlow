using Frank.PulseFlow.Internal;

using Microsoft.Extensions.DependencyInjection;

namespace Frank.PulseFlow;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPulseFlow(this IServiceCollection services,
        Action<IPulseFlowBuilder> configure)
    {
        PulseFlowBuilder builder = new(services);
        configure(builder);

        services.AddHostedService<PulseNexus>();
        services.AddSingleton<IConduit, Conduit>();
        services.AddSingleton<IChannel, Channel>();

        return services;
    }
}