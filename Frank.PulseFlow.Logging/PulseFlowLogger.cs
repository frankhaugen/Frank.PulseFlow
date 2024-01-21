using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Frank.PulseFlow.Logging;

/// <summary>
/// Implementation of ILogger interface that logs messages using pulse flow.
/// </summary>
public class PulseFlowLogger : ILogger
{
    private readonly string _categoryName;
    private readonly IConduit _conduit;
    private readonly IOptionsMonitor<LoggerFilterOptions> _filterOptions;

    /// <summary>
    /// Initializes a new instance of the PulseFlowLogger class with the specified category name, conduit, and filter options.
    /// </summary>
    /// <param name="categoryName">The category name of the logger.</param>
    /// <param name="conduit">The conduit used for logging.</param>
    /// <param name="filterOptions">The options monitor for logger filter options.</param>
    public PulseFlowLogger(string categoryName, IConduit conduit, IOptionsMonitor<LoggerFilterOptions> filterOptions)
    {
        _categoryName = categoryName;
        _conduit = conduit;
        _filterOptions = filterOptions;
    }

    /// <summary>
    /// Logs a message with the specified log level, event ID, state, exception, and formatter.
    /// </summary>
    /// <typeparam name="TState">The type of the state object.</typeparam>
    /// <param name="logLevel">The log level.</param>
    /// <param name="eventId">The event ID associated with the log entry.</param>
    /// <param name="state">The state object.</param>
    /// <param name="exception">The exception associated with the log entry.</param>
    /// <param name="formatter">A function that formats the log message.</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) 
        => _conduit.SendAsync(new LogPulse(logLevel, eventId, exception, _categoryName, formatter.Invoke(state, exception), state as IReadOnlyList<KeyValuePair<string, object?>>)).GetAwaiter().GetResult();

    /// <summary>
    /// Checks if logging is enabled for the specified log level.
    /// </summary>
    /// <param name="logLevel">The log level to check.</param>
    /// <returns>Returns true if logging is enabled for the specified log level, otherwise false.</returns>
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

    /// <summary>
    /// Begins a new log scope with the specified state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="state">The state object for the log scope.</param>
    /// <returns>An object that represents the log scope. Dispose this object to end the log scope.</returns>
    /// <remarks>
    /// The log scope allows grouping related log entries together.
    /// </remarks>
    /// <seealso cref="PulseFlowLoggerScope{TState}"/>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => new PulseFlowLoggerScope<TState>(state);
}