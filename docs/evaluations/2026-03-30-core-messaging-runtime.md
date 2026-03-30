# Evaluation: Core messaging runtime (2026-03-30)

**Subject:** `PulseNexus`, `Channel<IPulse>`, `IConduit`, fan-out semantics, ordering, and failure isolation.

**Scope:** `Frank.PulseFlow/Internal/PulseNexus.cs`, `Frank.PulseFlow/Internal/Conduit.cs`, interaction with `System.Threading.Channels`.

---

## 1. Summary verdict

The runtime is **small, legible, and appropriate** for a single-process “bus” where **one consumer loop** serializes **dequeue** but **fan-out per message** is explicitly parallel. The recent addition of **per-flow exception isolation** materially improves **liveness** versus an earlier design where one throwing `IFlow` could fault the entire `WhenAll` and stall or tear down the hosted service.

**Residual risks** are mostly **implicit coupling between slow handlers and head-of-line blocking** and **unbounded `IEnumerable<IFlow>` materialization** every pulse. **No-match** and **handler fault** visibility are available via **`ConfigurePulseFlowDiagnostics`** and **`Trace`** (see reference docs).

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

### 2.3 No-handler and diagnostics

If `flows.Length == 0`, the pulse is **skipped**; **`NotifyUnmatched`** calls **`UnmatchedPulse`** when configured (**`ConfigurePulseFlowDiagnostics`**), otherwise the nexus emits **no** trace for that case. **`NotifyFlowFault`** mirrors that pattern for handler exceptions after **`Trace.TraceError`**. Callbacks are wrapped in **try/catch** so user code cannot fault the reader loop.

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

**Remaining limits:** **`Trace`** requires host listeners; **first-party metrics** and **`Activity`** integration are still **out of scope** for the core library—use **`FlowFault`** / **`UnmatchedPulse`** or external telemetry.

---

## 5. Performance micro-notes

- **`ToArray()`** per pulse allocates; flow count is usually small, but **hot paths** with huge fan-out could optimize (cached array of flows per pulse type, immutable routing table).
- **`Where` over `IEnumerable<IFlow>`** assumes cheap iteration; DI typically resolves to an array-backed enumerable, but this is **not** enforced by the type system.

---

## 6. Conclusion

The core runtime is **coherent** for its stated niche. **Forward-looking:** immutable dispatch tables, documented **handler latency** / timeout patterns, and optional **first-party metrics** if demand appears.
