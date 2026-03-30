# Package: Frank.PulseFlow.Logging

Optional integration between **`Microsoft.Extensions.Logging`** and PulseFlow.

## Role

Registers **`PulseFlowLoggerProvider`** as an **`ILoggerProvider`**. Log calls that pass filtering become **`LogPulse`** instances sent through **`IConduit`**, so any **`IFlow`** that handles **`LogPulse`** can persist, transform, or observe logs like any other pulse.

## Key types

| Type | Purpose |
|------|---------|
| `LoggingBuilderExtensions.AddPulseFlow` | Extension on `ILoggingBuilder` |
| `PulseFlowLoggerProvider` | `ILoggerProvider` implementation |
| `PulseFlowLogger` | `ILogger` implementation |
| `LogPulse` | `IPulse` carrying log metadata and message |
| `PulseFlowLoggerScope` | Scope support for `BeginScope` |

## Dependency

- **Frank.PulseFlow** (project reference in repo; NuGet dependency when consumed)

## Usage summary

```csharp
builder.Logging.AddPulseFlow();
services.AddPulseFlow<MyFileLogFlow>(); // handles LogPulse
```

More detail: [Logging integration guide](../guides/logging-integration.md) and [Frank.PulseFlow.Logging README](../../Frank.PulseFlow.Logging/README.md).

## Behavior notes

- Respect **`LoggerFilterOptions`** and category rules from the host.
- Understand **sync-over-async** in `PulseFlowLogger.Log` for advanced hosting models (see guide).
