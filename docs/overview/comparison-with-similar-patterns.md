# Comparison with similar patterns

This page situates PulseFlow next to common .NET patterns. It is opinionated but practical.

## Raw `System.Threading.Channels`

| Aspect | Raw channels | PulseFlow |
|--------|--------------|-----------|
| API | `Channel<T>`, `Reader`/`Writer` | `IConduit`, `IFlow`, optional `IPulseHandler<T>` |
| DI | You register reader/writer yourself | `AddPulseFlow*` wires channel, conduit, nexus |
| Dispatch | You write the consumer loop | `PulseNexus` provides a default loop + `IFlow` fan-out |
| Type on the wire | Your choice | `IPulse` on a shared channel |

PulseFlow **is** channels under the hood; it adds **naming, registration, and a dispatch policy**.

## MediatR-style in-process bus

MediatR (and similar) often emphasize **one handler per request type** (or explicit pipelines). PulseFlow emphasizes:

- A **single channel** of **`IPulse`**.
- **Multiple `IFlow`** instances that may all handle the **same** pulse type **in parallel**.

If you need **pipeline behaviors** (validation → handler → post-processing) per message, MediatR pipelines or explicit orchestration may fit better unless you model that as a single `IFlow` internally.

## `IHostedService` + hand-rolled queue

PulseFlow’s **`PulseNexus`** *is* an `IHostedService` (`BackgroundService`) plus a dispatch loop. Using PulseFlow saves repeating:

- Registration of reader/writer singletons (via **Frank.Channels.DependencyInjection**).
- The `ReadAllAsync` loop.
- A consistent extension point (`IFlow`) for new behaviors.

## TPL Dataflow (`TransformBlock`, etc.)

Dataflow offers **graph-shaped** pipelines with backpressure and linking. PulseFlow is **simpler**: one reader, many flows, **no built-in block graph**. Choose Dataflow when you need complex graphs; choose PulseFlow when you want a **single bus** and **orthogonal handlers**.

## Logging pipelines (`ILogger` + providers)

Standard logging uses **`ILoggerProvider`** and sinks that are independent of PulseFlow. Flows can inject **`ILogger<T>`** and log as usual; that does not route log events through **`IConduit`** unless you build that yourself.

## Summary

- **Channels alone**: maximum control, more boilerplate.
- **PulseFlow**: opinionated single-bus dispatch with DI integration.
- **MediatR / Dataflow**: different trade-offs for pipeline shape and handler cardinality.
