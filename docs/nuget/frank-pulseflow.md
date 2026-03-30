# Package: Frank.PulseFlow

The core library for in-process pulse messaging.

## Contents

- **Abstractions:** `IPulse`, `IConduit`, `IFlow`, `IPulseHandler<T>`, `IFlowBuilder`
- **Registration:** `ServiceCollectionExtensions.AddPulseFlow*` on `IServiceCollection`
- **Types for apps:** `BasePulse`, `IncompatibleFlowException`, optional `PulseFlowDiagnosticsOptions` / `ConfigurePulseFlowDiagnostics` for unmatched pulses and flow faults
- **Internal runtime:** `PulseNexus`, `Conduit`, `GenericFlow`, `FlowBuilder` (not intended for direct use)

## Dependencies

- **Frank.Channels.DependencyInjection** — registers `Channel<IPulse>` and writer/reader for DI
- **Microsoft.Extensions.Hosting.Abstractions** — `BackgroundService` for `PulseNexus`
- **Microsoft.Extensions.Options** — `IOptions<PulseFlowDiagnosticsOptions>` for optional diagnostics

## Typical usage

```csharp
services.AddPulseFlow<MyPulse, MyHandler>();
// or
services.AddPulseFlow(b => b.AddFlow<MyFlow>());
```

## XML documentation

The package builds with XML documentation enabled (see `Directory.Build.props` in the repo). IDE tooltips surface summaries from the shipped `.xml` file.

## Related

- [Extension methods reference](../reference/extension-methods.md)
- [Logging package](pulseflow-logging.md)
