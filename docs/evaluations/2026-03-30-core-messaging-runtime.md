# Evaluation: Core messaging runtime (2026-03-30)

**Subject:** `PulseNexus`, `Channel<IPulse>`, `IConduit`, fan-out semantics, ordering, and failure isolation.

**Scope:** `Frank.PulseFlow/Internal/PulseNexus.cs`, `Frank.PulseFlow/Internal/Conduit.cs`, interaction with `System.Threading.Channels`.

---

## 1. Summary verdict

The runtime is **small, legible, and appropriate** for a single-process “bus” where **one consumer loop** serializes **dequeue** but **fan-out per message** is explicitly parallel. The recent addition of **per-flow exception isolation** materially improves **liveness** versus an earlier design where one throwing `IFlow` could fault the entire `WhenAll` and stall or tear down the hosted service.

**Residual risks** are mostly about **implicit coupling between slow handlers and head-of-line blocking**, **no-match visibility only when hosts opt into diagnostics**, and **unbounded `IEnumerable<IFlow>` materialization** every pulse.

---

## 2. Control flow analysis

### 2.1 Dequeue granularity

```13:26:Frank.PulseFlow/Internal/PulseNexus.cs
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (IPulse pulse in reader.ReadAllAsync(stoppingToken))
        {
            var flows = pulseFlows.Where(x => x.CanHandle(pulse.GetType())).ToArray();
            if (flows.Length == 0)
            {
                NotifyUnmatched(pulse);
                continue;
            }

            await Task.WhenAll(flows.Select(flow => InvokeFlowAsync(flow, pulse, stoppingToken)));
        }
    }
```

**Implication:** The channel reader does **not** advance to the next pulse until **all** invocations for the current pulse complete (including awaited work inside `HandleAsync`). This is **not** “process one pulse per flow independently off a shared queue”; it is **strict head-of-line blocking** at the nexus.

**Deep consequence:** A **poison pulse** that causes every matching flow to hang (deadlock, infinite await without cancellation) **blocks the entire bus**. Isolation catches **exceptions**, not **stalls**. Mitigation belongs in application code (timeouts, `CancellationToken`, bounded work).

### 2.2 Fan-out parallelism

Matching flows run **concurrently** via `Task.WhenAll`. Shared mutable state **without** external synchronization is a **data race** unless all writers use concurrency primitives or confine mutation to thread-safe structures.

**Deep consequence:** The library **encourages** a microservices-like “multiple subscribers” mental model, but **in-process** parallelism is **not** the same as message broker fan-out with independent consumer groups. Subscribers **share** the same process heap and the **same dequeue cursor**.

### 2.3 No-handler case

If `flows.Length == 0`, the pulse is **skipped**. **Default:** no trace from PulseFlow (same as before diagnostics). **Optional:** `NotifyUnmatched` invokes **`PulseFlowDiagnosticsOptions.UnmatchedPulse`** when configured via **`ConfigurePulseFlowDiagnostics`**, so hosts can log, meter, or assert without changing dispatch semantics.

**Recommendation (product):** Enable **`UnmatchedPulse`** in environments where mis-routing must be visible; **dead-letter queues** and **feature flags** remain application concerns.

### 2.4 Diagnostics hooks (summary)

**`NotifyUnmatched`** and **`NotifyFlowFault`** read **`IOptions<PulseFlowDiagnosticsOptions>`**; callbacks are wrapped in **try/catch** so user code cannot fault the reader loop. See **`PulseNexus`** for the full implementation alongside **`InvokeFlowAsync`**.

---

## 3. `IConduit` / channel boundary

`Conduit.SendAsync` forwards to `ChannelWriter<IPulse>.WriteAsync`. Backpressure, dropping, and capacity are **not** decided in PulseFlow; they are properties of **Frank.Channels.DependencyInjection**’s `AddChannel<IPulse>()` configuration.

**Evaluation:** This is a **clean separation** for a thin library, but it **centralizes operational risk** in a dependency whose defaults may not match production SLOs. Teams must **read and test** channel configuration as part of adopting PulseFlow.

---

## 4. Exception isolation (`InvokeFlowAsync`)

```47:67:Frank.PulseFlow/Internal/PulseNexus.cs
    private async Task InvokeFlowAsync(IFlow flow, IPulse pulse, CancellationToken stoppingToken)
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
                "[Frank.PulseFlow] Flow {0} failed while handling pulse {1} (pulseId={2}): {3}",
                flow.GetType().FullName,
                pulse.GetType().FullName,
                pulse.Id,
                ex);
            NotifyFlowFault(flow, pulse, ex);
        }
    }
```

**Strengths:**

- Preserves **host shutdown** semantics by rethrowing cancellation tied to `stoppingToken`.
- Prevents **one bad flow** from killing sibling flows for the same pulse.
- Avoids routing failure logs through `ILogger` in a way that would **recurse** into `PulseFlow.Logging`.
- **`Trace.TraceError`** includes **`IPulse.Id`** for correlation alongside CLR type names.
- Optional **`FlowFault`** callback supplies structured context without forcing **`ILogger`**.

**Weaknesses:**

- **`Trace.TraceError`** is still easy to **miss** in production unless listeners are configured.
- **Swallowed handler exceptions** mean **metrics and alerting** do not move unless traces or **`FlowFault`** are wired.

See the companion evaluation [Observability, faults, and operations](2026-03-30-observability-faults-and-operations.md).

---

## 5. Performance micro-notes

- **`ToArray()`** per pulse allocates; flow count is usually small, but **hot paths** with huge fan-out could optimize (cached array of flows per pulse type, immutable routing table).
- **`Where` over `IEnumerable<IFlow>`** assumes cheap iteration; DI typically resolves to an array-backed enumerable, but this is **not** enforced by the type system.

---

## 6. Conclusion

The core runtime is **coherent** for its stated niche. **Optional diagnostics** address **no-match** and **fault** visibility without **`ILogger`**; **trace lines** include **`IPulse.Id`**. Remaining debt is **trace-only** defaults for teams that do not configure listeners or callbacks, plus **implicit performance contracts** (head-of-line blocking, channel config elsewhere).

**If the library grows:** immutable dispatch tables, documented **handler latency** / timeout patterns, and application-level **dead-letter** policies beyond callbacks.
