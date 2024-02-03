namespace Frank.PulseFlow.Internal;

internal class GenericFlow<TPulse, THandler>(THandler handler) : IFlow
    where TPulse : IPulse
    where THandler : IPulseHandler<TPulse>
{
    public Task HandleAsync(IPulse pulse, CancellationToken cancellationToken)
    {
        if (pulse is TPulse t)
            return handler.HandleAsync(t, cancellationToken);
        throw new IncompatibleFlowException($"The pulse is not of type {typeof(TPulse).Name}. This is impossible and should never happen, so please report this as a bug on GitHub ASAP. Thank you!");
    }

    public bool CanHandle(Type pulseType) => pulseType == typeof(TPulse);
}