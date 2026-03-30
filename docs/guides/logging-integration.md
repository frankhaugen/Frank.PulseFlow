# Logging integration

**Frank.PulseFlow.Logging** bridges **`Microsoft.Extensions.Logging`** into the PulseFlow channel by emitting **`LogPulse`** messages.

## Setup

```csharp
using Frank.PulseFlow.Logging;

builder.Logging.AddPulseFlow();
builder.Services.AddPulseFlow<YourLogSinkFlow>(); // IFlow that handles LogPulse
```

Implement a flow that handles **`LogPulse`** (see **`Frank.PulseFlow.Logging.LogPulse`**):

```csharp
public sealed class FileLogFlow(IOptions<MyLogOptions> options) : IFlow
{
    public bool CanHandle(Type pulseType) => pulseType == typeof(LogPulse);

    public async Task HandleAsync(IPulse pulse, CancellationToken cancellationToken)
    {
        var log = (LogPulse)pulse;
        await File.AppendAllTextAsync(
            options.Value.Path,
            log + Environment.NewLine,
            cancellationToken);
    }
}
```

## Filtering and host configuration

`ILogger` filtering is applied by the **logging factory** before your provider is invoked. When hosting tests or minimal hosts, ensure **`LoggerFilterOptions`** minimum levels allow events you expect (see test bases or `LoggingBuilder.SetMinimumLevel` as appropriate).

## Implementation note

`PulseFlowLogger.Log` forwards work asynchronously via **`IConduit`**. Be aware of **sync-over-async** (`GetAwaiter().GetResult()`) in the current implementation when reasoning about deadlocks in advanced scenarios.

## See also

- [Logging package](../nuget/pulseflow-logging.md)
- [Frank.PulseFlow.Logging README](../../Frank.PulseFlow.Logging/README.md)
