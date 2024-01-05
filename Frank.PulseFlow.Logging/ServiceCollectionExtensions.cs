using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Frank.PulseFlow.Logging;

public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds PulseFlow logger provider to the logging builder.
    /// </summary>
    /// <param name="builder">The logging builder.</param>
    /// <returns>The updated logging builder.</returns>
    public static ILoggingBuilder AddPulseFlow(this ILoggingBuilder builder)
    {
        builder.Services.AddSingleton<ILoggerProvider, PulseFlowLoggerProvider>();
        
        return builder;
    }
}