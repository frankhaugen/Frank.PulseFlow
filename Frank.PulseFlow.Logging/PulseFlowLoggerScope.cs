namespace Frank.PulseFlow.Logging;

public class PulseFlowLoggerScope<TState> : IDisposable
{
    public TState? State { get; private set; }

    public PulseFlowLoggerScope(TState state) => State = state;

    public void Dispose() => State = default;
}