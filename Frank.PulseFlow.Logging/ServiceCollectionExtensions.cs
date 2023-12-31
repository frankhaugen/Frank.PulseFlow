using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Frank.PulseFlow.Logging;

public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddPulseFlow(this ILoggingBuilder builder)
    {
        builder.Services.AddSingleton<ILoggerProvider, PulseFlowLoggerProvider>();
        return builder;
    }
}