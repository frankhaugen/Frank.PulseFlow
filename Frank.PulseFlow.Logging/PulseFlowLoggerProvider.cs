using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Frank.PulseFlow.Logging;

public class PulseFlowLoggerProvider : ILoggerProvider
{
    private readonly IConduit _conduit;
    private readonly IOptionsMonitor<LoggerFilterOptions> _filterOptions;
    private readonly ConcurrentDictionary<string, PulseFlowLogger> _loggers = new();

    public PulseFlowLoggerProvider(IConduit conduit, IOptionsMonitor<LoggerFilterOptions> filterOptions)
    {
        _conduit = conduit;
        _filterOptions = filterOptions;
    }

    public void Dispose() => _loggers.Clear();

    public ILogger CreateLogger(string categoryName) =>
        // ReSharper disable once HeapView.CanAvoidClosure
        _loggers.GetOrAdd(categoryName, name => new PulseFlowLogger(name, _conduit, _filterOptions));
}