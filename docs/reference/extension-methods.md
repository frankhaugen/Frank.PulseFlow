# Extension methods reference

## `IServiceCollection` (Frank.PulseFlow)

### `AddPulseFlow(Action<IFlowBuilder> configure)`

Runs the delegate with a **`IFlowBuilder`** so you can chain **`AddFlow<T>()`** calls. Each flow type is registered via the same pipeline as **`AddPulseFlow<TFlow>()`**.

### `AddPulseFlow<TPulse, THandler>()`

**Constraints:** `THandler : class, IPulseHandler<TPulse>`, `TPulse : IPulse`

- Registers **`IPulseHandler<TPulse>`** → **`THandler`** and **`THandler`** itself (singleton), idempotently.
- Registers **`IFlow`** → **`GenericFlow<TPulse, THandler>`** if missing.
- Calls **`AddPulseFlow<GenericFlow<TPulse, THandler>>()`** to ensure channel, conduit, nexus, and flow are wired.

### `AddPulseFlow<TFlow>()`

**Constraint:** `TFlow : class, IFlow`

- Registers **`IFlow`** → **`TFlow`** if not already registered for that implementation type.
- Registers **`PulseNexus`** hosted service if not present.
- Registers **`IConduit`** → **`Conduit`** if missing.
- Registers **`Channel<IPulse>`** via **`AddChannel<IPulse>()`** if missing.
- Registers **`IOptions<PulseFlowDiagnosticsOptions>`** (via **`AddOptions`**) the first time **`PulseNexus`** is wired, so **`ConfigurePulseFlowDiagnostics`** can run without extra setup.

### `ConfigurePulseFlowDiagnostics(Action<PulseFlowDiagnosticsOptions> configure)`

Binds optional **`UnmatchedPulse`** and **`FlowFault`** callbacks on **`PulseFlowDiagnosticsOptions`**. When unset, behavior matches the pre-diagnostics defaults: unmatched pulses are skipped silently; flow faults still emit **`System.Diagnostics.Trace.TraceError`**. Callbacks are invoked in a **try/catch** so user code cannot break the nexus.

## See also

- [Dependency injection wiring](../architecture/dependency-injection-wiring.md)
