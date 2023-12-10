namespace Frank.PulseFlow;

public interface IConduit
{
    Task SendAsync(IPulse message);
}