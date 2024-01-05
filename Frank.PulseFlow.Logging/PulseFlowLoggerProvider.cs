using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Frank.PulseFlow.Logging;

/// <summary>
/// Provides instances of PulseFlowLogger.
/// </summary>
public class PulseFlowLoggerProvider : ILoggerProvider
{
    private readonly IConduit _conduit;
    private readonly IOptionsMonitor<LoggerFilterOptions> _filterOptions;
    private readonly ConcurrentDictionary<string, PulseFlowLogger> _loggers = new();

    /// <summary>
    /// Represents a PulseFlowLoggerProvider object that provides logging services for PulseFlow application.
    /// </summary>
    /// <remarks>
    /// This class is responsible for initializing and configuring the PulseFlow logger provider. It takes an IConduit object and an IOptionsMonitor{LoggerFilterOptions} object as input
    /// parameters.
    /// </remarks>
    /// <param name="conduit">An IConduit object that represents the conduit for transmitting logs.</param>
    /// <param name="filterOptions">An IOptionsMonitor{LoggerFilterOptions} object that represents the various options for filtering logs.</param>
    /// <exception cref="ArgumentNullException">Thrown when either conduit or filterOptions is null.</exception>
    /// <example>
    /// <code>
    /// IConduit conduit = new MyConduit();
    /// IOptionsMonitor{LoggerFilterOptions} filterOptions = new MyFilterOptions();
    /// PulseFlowLoggerProvider loggerProvider = new PulseFlowLoggerProvider(conduit, filterOptions);
    /// </code>
    /// </example>
    public PulseFlowLoggerProvider(IConduit conduit, IOptionsMonitor<LoggerFilterOptions> filterOptions)
    {
        _conduit = conduit;
        _filterOptions = filterOptions;
    }

    /// <summary>
    /// Disposes the resources used by the object and clears all the loggers.
    /// </summary>
    public void Dispose() => _loggers.Clear();

    /// <summary>
    /// Creates a new instance of ILogger for the specified category name.
    /// </summary>
    /// <param name="categoryName">The name of the category to create the logger for.</param>
    /// <returns>
    /// The ILogger instance for the specified category name.
    /// </returns>
    public ILogger CreateLogger(string categoryName) =>
        // ReSharper disable once HeapView.CanAvoidClosure
        _loggers.GetOrAdd(categoryName, name => new PulseFlowLogger(name, _conduit, _filterOptions));
}