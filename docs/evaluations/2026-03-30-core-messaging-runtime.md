# Evaluation: Core messaging runtime (2026-03-30)

**Subject:** `PulseNexus`, `Channel<IPulse>`, `IConduit`, fan-out semantics, ordering, and failure isolation.

**Scope:** `Frank.PulseFlow/Internal/PulseNexus.cs`, `Frank.PulseFlow/Internal/Conduit.cs`, interaction with `System.Threading.Channels`.

---

## 1. Summary verdict

The runtime is **small, legible, and appropriate** for a single-process “bus” where **one consumer loop** serializes **dequeue** but **fan-out per message** is explicitly parallel. The recent addition of **per-flow exception isolation** materially improves **liveness** versus an earlier design where one throwing `IFlow` could fault the entire `WhenAll` and stall or tear down the hosted service.

**Residual risks** are mostly about **implicit coupling between slow handlers and head-of-line blocking**, **silent drops when no flow matches**, and **unbounded `IEnumerable<IFlow>` materialization** every pulse.

---

## 2. Control flow analysis

### 2.1 Dequeue granularity

```9:17:Frank.PulseFlow/Internal/PulseNexus.cs
        await foreach (IPulse pulse in reader.ReadAllAsync(stoppingToken))
        {
            var flows = pulseFlows.Where(x => x.CanHandle(pulse.GetType())).ToArray();
            if (flows.Length == 0)
                continue;

            await Task.WhenAll(flows.Select(flow => InvokeFlowAsync(flow, pulse, stoppingToken)));
        }
```

**Implication:** The channel reader does **not** advance to the next pulse until **all** invocations for the current pulse complete (including awaited work inside `HandleAsync`). This is **not** “process one pulse per flow independently off a shared queue”; it is **strict head-of-line blocking** at the nexus.

**Deep consequence:** A **poison pulse** that causes every matching flow to hang (deadlock, infinite await without cancellation) **blocks the entire bus**. Isolation catches **exceptions**, not **stalls**. Mitigation belongs in application code (timeouts, `CancellationToken`, bounded work).

### 2.2 Fan-out parallelism

Matching flows run **concurrently** via `Task.WhenAll`. Shared mutable state **without** external synchronization is a **data race** unless all writers use concurrency primitives or confine mutation to thread-safe structures.

**Deep consequence:** The library **encourages** a microservices-like “multiple subscribers” mental model, but **in-process** parallelism is **not** the same as message broker fan-out with independent consumer groups. Subscribers **share** the same process heap and the **same dequeue cursor**.

### 2.3 No-handler case

If `flows.Length == 0`, the pulse is **dropped** with **no trace** from PulseFlow itself. That may be intentional (optional audit flows), but in many systems this is a **silent logic error** (typo in `CanHandle`, wrong pulse type, missing registration).

**Recommendation (product):** Consider an optional **“dead letter”** hook: delegate, `IOptions`, or `IFlow` that runs when no flow matched—possibly behind a feature flag to avoid noise.

---

## 3. `IConduit` / channel boundary

`Conduit.SendAsync` forwards to `ChannelWriter<IPulse>.WriteAsync`. Backpressure, dropping, and capacity are **not** decided in PulseFlow; they are properties of **Frank.Channels.DependencyInjection**’s `AddChannel<IPulse>()` configuration.

**Evaluation:** This is a **clean separation** for a thin library, but it **centralizes operational risk** in a dependency whose defaults may not match production SLOs. Teams must **read and test** channel configuration as part of adopting PulseFlow.

---

## 4. Exception isolation (`InvokeFlowAsync`)

```20:37:Frank.PulseFlow/Internal/PulseNexus.cs
    private static async Task InvokeFlowAsync(IFlow flow, IPulse pulse, CancellationToken stoppingToken)
    {
        try
        {
            await flow.HandleAsync(pulse, stoppingToken);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            Trace.TraceError(
                "[Frank.PulseFlow] Flow {0} failed while handling pulse {1}: {2}",
                flow.GetType().FullName,
                pulse.GetType().FullName,
                ex);
        }
    }
```

**Strengths:**

- Preserves **host shutdown** semantics by rethrowing cancellation tied to `stoppingToken`.
- Prevents **one bad flow** from killing sibling flows for the same pulse.
- Avoids routing failure logs through `ILogger` in a way that would **recurse** into `PulseFlow.Logging`.

**Weaknesses:**

- **`Trace.TraceError`** is easy to **miss** in production unless listeners are configured.
- **Swallowed exceptions** mean **metrics and alerting** do not move unless something external observes traces.
- **No correlation id** tying failures to `IPulse.Id` in the trace message (only type names).

See the companion evaluation [Observability, faults, and operations](2026-03-30-observability-faults-and-operations.md).

---

## 5. Performance micro-notes

- **`ToArray()`** per pulse allocates; flow count is usually small, but **hot paths** with huge fan-out could optimize (cached array of flows per pulse type, immutable routing table).
- **`Where` over `IEnumerable<IFlow>`** assumes cheap iteration; DI typically resolves to an array-backed enumerable, but this is **not** enforced by the type system.

---

## 6. Conclusion

The core runtime is **coherent** for its stated niche. The main engineering debt is **operational visibility** (no-match drops, trace-only errors) and **implicit performance contracts** (head-of-line blocking, channel config elsewhere).

**Priority improvements if the library grows:** optional dead-letter path, structured error reporting with `IPulse.Id`, and documented **expected handler latency** / timeout patterns for consumers.
