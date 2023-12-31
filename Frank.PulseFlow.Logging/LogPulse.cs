using Microsoft.Extensions.Logging;

namespace Frank.PulseFlow.Logging;

public abstract class LogPulse : BasePulse
{
    public abstract LogLevel LogLevel { get; }
    public abstract EventId EventId { get; }
    public abstract Exception? Exception { get; }
    public abstract string CategoryName { get; }
}
public class LogPulse<TState> : LogPulse
{
    public override LogLevel LogLevel { get; }
    public override EventId EventId { get; }
    public override Exception? Exception { get; }
    public override string CategoryName { get; }

    public TState State { get; }
    public Func<TState, Exception?, string> Formatter { get; }

    public LogPulse(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter, string categoryName)
    {
        LogLevel = logLevel;
        EventId = eventId;
        State = state;
        Exception = exception;
        Formatter = formatter;
        CategoryName = categoryName;
    }

    public override string ToString() => Formatter(State, Exception);
}
