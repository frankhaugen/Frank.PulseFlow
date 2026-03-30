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

## `ILoggingBuilder` (Frank.PulseFlow.Logging)

### `AddPulseFlow()`

Registers **`ILoggerProvider`** → **`PulseFlowLoggerProvider`** as a singleton service on the builder’s **`Services`** collection.

## See also

- [Dependency injection wiring](../architecture/dependency-injection-wiring.md)
