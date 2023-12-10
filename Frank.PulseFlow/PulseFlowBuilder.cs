using Microsoft.Extensions.DependencyInjection;

namespace Frank.PulseFlow;

public class PulseFlowBuilder(IServiceCollection services) : IPulseFlowBuilder
{
    public IPulseFlowBuilder AddFlow<T>() where T : class, IPulseFlow
    {
        services.AddTransient<IPulseFlow, T>();
        return this;
    }
}