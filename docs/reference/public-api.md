# Public API surface

This is a concise map of **documented public** types in **Frank.PulseFlow** and **Frank.PulseFlow.Logging**. Internal implementation types in `Frank.PulseFlow.Internal` are omitted.

## Frank.PulseFlow

| Type | Kind | Summary |
|------|------|---------|
| `IPulse` | Interface | Pulse identity: `Id`, `Created` |
| `BasePulse` | Class | Default `Id` / `Created` |
| `IConduit` | Interface | `SendAsync(IPulse, CancellationToken)` |
| `IFlow` | Interface | `HandleAsync`, `CanHandle(Type)` |
| `IPulseHandler<T>` | Interface | Typed `HandleAsync(T, CancellationToken)` |
| `IFlowBuilder` | Interface | Fluent registration (`AddFlow<T>`) |
| `ServiceCollectionExtensions` | Static class | `AddPulseFlow` overloads, `ConfigurePulseFlowDiagnostics` |
| `PulseFlowDiagnosticsOptions` | Class | Optional callbacks for unmatched pulses and flow faults |
| `PulseUnmatchedContext` | Class | Pulse type and instance when no `IFlow` matched |
| `PulseFlowFaultContext` | Class | Flow type, pulse instance, exception (non-cancellation faults) |
| `IncompatibleFlowException` | Exception | `GenericFlow` guard |

## Frank.PulseFlow.Logging

| Type | Kind | Summary |
|------|------|---------|
| `LoggingBuilderExtensions` | Static class | `AddPulseFlow(this ILoggingBuilder)` |
| `PulseFlowLoggerProvider` | Class | `ILoggerProvider` |
| `PulseFlowLogger` | Class | `ILogger` |
| `PulseFlowLoggerScope<TState>` | Class | Scope disposable |
| `LogPulse` | Class | Log payload as `IPulse` (`State`, `Scope`) |

## Source of truth

Generated XML documentation in the build output and IntelliSense in your IDE. This table may lag the code; prefer the repository source when in doubt.

## See also

- [Extension methods](extension-methods.md)
