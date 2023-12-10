namespace Frank.PulseFlow;

public interface IPulseFlowBuilder
{
    IPulseFlowBuilder AddFlow<T>() where T : class, IPulseFlow;
}