# Nexus (`PulseNexus`)

**`PulseNexus`** is an internal **`BackgroundService`** that **consumes** pulses from **`ChannelReader<IPulse>`** and **dispatches** them to registered **`IFlow`** instances.

## Behavior (current implementation)

For each pulse read from the channel:

1. Resolve all flows where **`IFlow.CanHandle(pulse.GetType())`** is true.
2. Run **`HandleAsync`** on **all** matching flows **concurrently** via **`Task.WhenAll`**, with **per-flow exception isolation**: a throwing flow is logged via **`Trace.TraceError`** (including **`IPulse.Id`**) and does not stop sibling flows or the reader loop (cancellation from host shutdown is still propagated). Optional **`FlowFault`** diagnostics can observe faults without using **`ILogger`**.

There is **one** reader loop; pulses are processed **one at a time** from the channel’s perspective, but **handlers for a single pulse** may run **in parallel**.

## Registration

**`AddPulseFlow`** and its overloads ensure:

- At most **one** hosted **`PulseNexus`** is registered (duplicate registrations are guarded).
- **`IConduit`**, **`Channel<IPulse>`**, and flows are registered as needed.

## Lifecycle

- Starts with the host; **`ReadAllAsync`** honors **`CancellationToken`** from the host.
- Stopping the host cancels the token; in-flight handler tasks should cooperate with cancellation.

## Naming

“Nexus” in the README describes the **conceptual hub**; in code the type is **`PulseNexus`** in the **`Frank.PulseFlow.Internal`** namespace and is not part of the public API surface.

## See also

- [Dispatch and ordering](../architecture/dispatch-and-ordering.md) — critical for understanding parallelism
- [Flows and handlers](flows-and-handlers.md)
