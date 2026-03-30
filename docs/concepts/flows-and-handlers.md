# Flows and handlers

## `IFlow`

A **flow** is anything that implements **`IFlow`**:

```csharp
Task HandleAsync(IPulse pulse, CancellationToken cancellationToken);
bool CanHandle(Type pulseType);
```

- **`CanHandle`** — whether this flow participates when a pulse of that **runtime type** is dispatched.
- **`HandleAsync`** — async work for that pulse; receives **`IPulse`** (cast as needed).

Multiple flows may return true for the **same** `pulseType`; **`PulseNexus`** invokes **all** of them (in parallel). That is intentional **fan-out**.

## `IPulseHandler<T>`

**`IPulseHandler<T>`** is a **typed** handler API:

```csharp
Task HandleAsync(T pulse, CancellationToken cancellationToken);
```

**`AddPulseFlow<TPulse, THandler>`** registers:

- **`IPulseHandler<TPulse>`** and **`THandler`** as singletons.
- **`GenericFlow<TPulse, THandler>`** as an **`IFlow`** that forwards to the handler when the pulse type matches **`TPulse`** exactly.

This keeps domain code **`HandleAsync(MyPulse)`** instead of casting from **`IPulse`**.

## Custom `IFlow` without `IPulseHandler<T>`

Implement **`IFlow`** directly when you need:

- One flow handling **several** pulse types (multiple **`CanHandle`** rules).
- **Prefix** or **wildcard** style matching (not provided by **`GenericFlow`**).
- Logic that does not map cleanly to a single **`TPulse`**.

Use **`AddPulseFlow<TFlow>()`** or **`AddPulseFlow(services, b => b.AddFlow<TFlow>())`** to register.

## `IFlowBuilder`

**`AddPulseFlow(Action<IFlowBuilder>)`** exposes **`IFlowBuilder.AddFlow<T>()`** for fluent registration of multiple flow types in one call.

## Exceptions

**`GenericFlow`** throws **`IncompatibleFlowException`** if **`HandleAsync`** is invoked with a pulse that is not **`TPulse`**—that indicates an internal consistency bug rather than a user misconfiguration.

## See also

- [Custom flows guide](../guides/custom-flows.md)
- [Extension methods reference](../reference/extension-methods.md)
