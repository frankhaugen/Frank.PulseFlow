# Dispatch and ordering

Understanding ordering and parallelism is essential to using PulseFlow safely.

## Between pulses (sequential dequeue)

**`PulseNexus`** uses **`await foreach`** on **`ReadAllAsync`**. A new pulse is not dequeued until the previous iteration completes—**including** waiting for **`Task.WhenAll`** over all matching flows.

**Implication:** If one pulse’s handlers are slow, **later pulses wait** in the channel. This provides **global ordering of processing starts** per pulse, bounded by channel behavior and handler duration.

## Within one pulse (parallel handlers)

For a **single** pulse, every **`IFlow`** with **`CanHandle`** true runs **at the same time** (each invocation is wrapped so failures do not stop sibling flows or the reader loop—see below).

**Implications:**

- Handlers must not assume **exclusive access** to shared mutable state unless they synchronize externally.
- If handler A must complete before handler B for the **same** pulse, you need **one** flow that orchestrates both, **or** split into **two** pulses emitted in sequence, **or** accept sequential composition inside a single handler.

## Type matching

**`GenericFlow<TPulse, THandler>`** uses **exact** type equality:

```csharp
public bool CanHandle(Type pulseType) => pulseType == typeof(TPulse);
```

Subtypes of **`TPulse`** are **not** routed to that flow unless you add another registration or a custom **`IFlow`**.

## Ordering vs logging

If you use **Frank.PulseFlow.Logging**, **log pulses** share the **same** channel as domain pulses. Relative ordering of **log** vs **domain** pulses follows **send order** into the channel (subject to async timing). Heavy logging can delay domain processing if the nexus is backlogged.

## Cancellation

Handlers receive the **`CancellationToken`** from the host. When shutdown is requested, the reader loop stops; in-flight **`Task.WhenAll`** should observe cancellation if individual flows respect the token.

## Handler faults (isolation)

If an **`IFlow`** throws while handling a pulse, **`PulseNexus`** catches the exception (except when the operation was cancelled because the host is stopping), records it with **`System.Diagnostics.Trace.TraceError`**, and **continues** processing other flows for that pulse and **later** pulses.

- **Operational implication:** one bad handler does not permanently stop the channel consumer.
- **Observability:** attach a trace listener in development or monitoring if you rely on noticing these failures; they are not written through **`ILogger`** by default (avoiding feedback loops with **Frank.PulseFlow.Logging**).

## See also

- [Nexus](../concepts/nexus.md)
- [When to use](../overview/when-to-use.md)
