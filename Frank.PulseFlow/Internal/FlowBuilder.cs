namespace Frank.PulseFlow.Internal;

internal class FlowBuilder(IServiceCollection services) : IFlowBuilder
{
    public IFlowBuilder AddFlow<T>() where T : class, IFlow
    {
        services.AddPulseFlow<T>();
        return this;
    }
}