using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Frank.PulseFlow.Logging;

public class PulseFlowLogger : ILogger
{
    private readonly string _categoryName;
    private readonly IConduit _conduit;
    private readonly IOptionsMonitor<LoggerFilterOptions> _filterOptions;

    public PulseFlowLogger(string categoryName, IConduit conduit, IOptionsMonitor<LoggerFilterOptions> filterOptions)
    {
        _categoryName = categoryName;
        _conduit = conduit;
        _filterOptions = filterOptions;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) 
        => _conduit.SendAsync(new LogPulse<TState>(logLevel, eventId, state, exception, formatter, _categoryName)).GetAwaiter().GetResult();

    public bool IsEnabled(LogLevel logLevel)
    {
        // If LogLevel is None, logging is disabled.
        if (logLevel == LogLevel.None)
            return false;

        // Get current filter options
        var filterOptions = _filterOptions.CurrentValue;

        // Checking if a filter is set for the logLevel
        foreach (var filter in filterOptions.Rules)
        {
            if(filter.LogLevel.HasValue && filter.LogLevel.Value >= logLevel)
                return true;
        }
            
        // If no filter matches, we  use minimum level.
        return logLevel >= filterOptions.MinLevel;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => new PulseFlowLoggerScope<TState>(state);
}