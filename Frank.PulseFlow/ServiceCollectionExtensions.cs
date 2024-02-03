namespace Frank.PulseFlow;

/// <summary>
/// Provides extension methods for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the PulseFlow to the service collection.
    /// </summary>
    /// <param name="services">The service collection to which the PulseFlow components will be added.</param>
    /// <param name="configure">An action delegate to configure the PulseFlow builder.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddPulseFlow(this IServiceCollection services, Action<IFlowBuilder> configure)
    {
        FlowBuilder builder = new(services);
        configure(builder);
        return services;
    }

    /// <summary>
    /// Adds the specified pulse handler to the service collection for the PulseFlow.
    /// </summary>
    /// <typeparam name="TPulse">The type of pulse that the handler can handle.</typeparam>
    /// <typeparam name="THandler">The type of the pulse handler.</typeparam>
    /// <param name="services">The service collection to which the pulse handler will be added.</param>
    /// <returns>The updated service collection.</returns>
    /// <remarks>
    /// This method adds a singleton instance of the specified pulse handler type to the service collection.
    /// The pulse handler should implement the <see cref="IPulseHandler{T}"/> interface, where T is the type of the pulse.
    /// </remarks>
    public static IServiceCollection AddPulseFlow<TPulse, THandler>(this IServiceCollection services) where THandler : class, IPulseHandler<TPulse> where TPulse : IPulse
    {
        if (!services.Any(service => service.ServiceType == typeof(IPulseHandler<TPulse>) && service.ImplementationType == typeof(THandler)))
        {
            services.AddSingleton<IPulseHandler<TPulse>, THandler>();
            services.AddSingleton<THandler>();
        }

        if (!services.Any(service => service.ServiceType == typeof(IFlow) && service.ImplementationType == typeof(GenericFlow<TPulse, THandler>)))
            services.AddSingleton<IFlow, GenericFlow<TPulse, THandler>>();
        
        return services.AddPulseFlow<GenericFlow<TPulse, THandler>>();
    }

    /// <summary>
    /// Adds PulseFlow services to the <see cref="IServiceCollection"/> with the specified configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> after the services have been added.</returns>
    public static IServiceCollection AddPulseFlow<TFlow>(this IServiceCollection services) where TFlow : class, IFlow
    {
        if (!services.Any(service => service.ServiceType == typeof(IFlow) && service.ImplementationType == typeof(TFlow)))
            services.AddSingleton<IFlow, TFlow>();
        
        if (!services.Any(service => service.ServiceType == typeof(BackgroundService) && service.ImplementationType == typeof(PulseNexus)))
            services.AddHostedService<PulseNexus>();
        
        if (services.All(service => service.ServiceType != typeof(IConduit)))
            services.AddSingleton<IConduit, Conduit>();
        
        if (services.All(service => service.ServiceType != typeof(Channel<IPulse>)))
            services.AddChannel<IPulse>();
        
        return services;
    }
}