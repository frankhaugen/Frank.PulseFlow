using Microsoft.Extensions.Logging;

namespace Frank.PulseFlow.Logging;

public class LogPulse<TState> : BasePulse
{
    public LogLevel LogLevel { get; }
    public EventId EventId { get; }
    public Exception? Exception { get; }
    public string CategoryName { get; }
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
