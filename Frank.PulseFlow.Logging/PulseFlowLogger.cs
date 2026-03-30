using System.Collections.Generic;
using System.Linq;

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

    /// <summary>
    /// Initializes a new instance of the PulseFlowLogger class with the specified category name, conduit, and filter options.
    /// </summary>
    /// <param name="categoryName">The category name of the logger.</param>
    /// <param name="conduit">The conduit used for logging.</param>
    /// <param name="filterOptions">Reserved for DI compatibility; filtering is applied by the logging factory.</param>
    public PulseFlowLogger(string categoryName, IConduit conduit, IOptionsMonitor<LoggerFilterOptions> filterOptions)
    {
        _categoryName = categoryName;
        _conduit = conduit;
        _ = filterOptions;
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
    {
        var message = formatter.Invoke(state, exception);
        var structuredState = ExtractStructuredState(state);
        _conduit.SendAsync(new LogPulse(logLevel, eventId, exception, _categoryName, message, structuredState), CancellationToken.None).GetAwaiter().GetResult();
    }

    private static IReadOnlyList<KeyValuePair<string, object?>>? ExtractStructuredState<TState>(TState state)
    {
        if (state is null)
            return null;
        if (state is IReadOnlyList<KeyValuePair<string, object?>> list)
            return list;
        if (state is IEnumerable<KeyValuePair<string, object?>> enumerable)
            return enumerable.ToList();
        return null;
    }

    /// <summary>
    /// Checks if logging is enabled for the specified log level.
    /// </summary>
    /// <param name="logLevel">The log level to check.</param>
    /// <returns>Returns true if logging is enabled for the specified log level, otherwise false.</returns>
    /// <remarks>
    /// <see cref="LoggerFactory"/> applies <see cref="LoggerFilterOptions"/> per provider before invoking this logger.
    /// Re-applying filter options inside the provider was incorrect and broke under newer runtimes.
    /// </remarks>
    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

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