# Evaluation: Logging bridge (`ILogger` → `LogPulse`) (2026-03-30)

**Subject:** `Frank.PulseFlow.Logging` — `PulseFlowLogger`, `PulseFlowLoggerProvider`, `LogPulse`, interaction with `Microsoft.Extensions.Logging`.

---

## 1. Summary verdict

The bridge is **conceptually elegant**: it unifies **observability events** with **domain pulses** on one channel, enabling **one** ordering and fan-out story. Implementation-wise it carries **classic `ILogger` hazards**: **synchronous** `Log` with **blocking wait** on async I/O, **`CancellationToken.None`**, **scope state not propagated** onto the bus, and **allocations** when materializing structured state.

**Fit:** Strong for **integration tests**, **custom sinks**, and **cross-cutting** pipelines that already think in pulses. **Weaker** for ultra-hot paths or strict **structured logging** fidelity without additional design.

---

## 2. Synchronous `Log` over async `SendAsync`

```39:44:Frank.PulseFlow.Logging/PulseFlowLogger.cs
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter.Invoke(state, exception);
        var structuredState = ExtractStructuredState(state);
        _conduit.SendAsync(new LogPulse(...), CancellationToken.None).GetAwaiter().GetResult();
    }
```

### 2.1 Why this matters

`ILogger.Log` is **void** and **synchronous** in the abstraction. The implementation must either:

- **Block** until the write completes (current approach), or
- **Enqueue** to a memory buffer and return (lossy / complex lifecycle), or
- **Drop** (violates user expectations for “I logged”).

### 2.2 Failure modes (deep)

1. **Thread pool starvation:** If `WriteAsync` on a **bounded** channel blocks frequently under load, **sync contexts** that call logging from few threads can **pile up**.
2. **Deadlock (rare but real):** If `HandleAsync` for a `LogPulse` **synchronously** waits on something that requires the **same thread** that is inside `Log` (classic ASP.NET legacy patterns, UI threads, custom `SynchronizationContext`), **deadlock** is possible. Modern ASP.NET Core is largely safer, but **library code** cannot assume zero custom contexts.
3. **No cancellation:** `CancellationToken.None` means shutdown **cannot** cancel an in-flight log write via the token passed to `SendAsync`. Host stop still cancels the **reader**, but **writers** may briefly pile up.

**Evaluation:** Acceptable for many apps **if** logging volume is moderate and handlers are **non-blocking**. **Not** a drop-in for high-throughput telemetry without profiling.

---

## 3. Structured state extraction

```46:55:Frank.PulseFlow.Logging/PulseFlowLogger.cs
    private static IReadOnlyList<KeyValuePair<string, object?>>? ExtractStructuredState<TState>(TState state)
    {
        if (state is null)
            return null;
        if (state is IReadOnlyList<KeyValuePair<string, object?>> list)
            return list;
        if (state is IEnumerable<KeyValuePair<string, object?>> enumerable)
            return enumerable.ToList();
        return null;
    }
```

### 3.1 Correctness

This fixes the **invalid cast** that rejected arbitrary `TState` types. **Message text** still comes from `formatter.Invoke`, so **human-readable** logs remain correct even when `State` on `LogPulse` is null.

### 3.2 Costs

- **`ToList()`** allocates on every log for enumerable-but-not-list states. For high-volume `LogDebug` with structured templates, this is **GC pressure**.
- **Semantic fidelity:** Some logging states are **lazy** or **single-pass** enumerables; materialization can have **subtle** effects (usually fine for Microsoft’s concrete types, but **not provable** for all third-party providers).

---

## 4. `IsEnabled`

```66:66:Frank.PulseFlow.Logging/PulseFlowLogger.cs
    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;
```

**Rationale (documented in XML):** `LoggerFactory` already applies `LoggerFilterOptions` per provider in `MessageLogger` **before** calling the provider’s `ILogger`. Re-implementing filters inside `PulseFlowLogger` **duplicated** framework behavior and **broke** under evolving default rules.

**Trade-off:** The provider **cannot** implement **provider-specific** “always off” logic via `IsEnabled` alone; it must rely on **factory filters**. That is **usually correct** for a custom provider.

---

## 5. Scopes (`BeginScope`)

`BeginScope` returns `PulseFlowLoggerScope<TState>` which **stores state** and disposes by clearing it. **No** scope information is written to `IConduit`.

**Evaluation:** For consumers expecting **correlation** or **scoped properties** in `LogPulse`, this is a **functional gap**. Fixing it would require either **emitting pulses** on scope enter/exit (noisy) or **attaching scope stacks** to subsequent `LogPulse` instances (non-trivial, allocation-heavy).

---

## 6. Interaction with the core bus

Log pulses share **`Channel<IPulse>`** with domain pulses. **Bursty logging** can **delay** domain processing **without** obvious coupling in application code.

**Mitigation strategies (application-level):** separate channels (not supported first-class today), **rate limiting** in log-handling flows, or **not** using PulseFlow for **all** logging—only for **audited** subsets.

---

## 7. Conclusion

The logging bridge is **architecturally aligned** with PulseFlow’s goals but **implementation-biased** toward **correctness and simplicity** over **maximum throughput** and **full structured-log fidelity**.

**Highest-value future work:** optional **async** pipeline (breaking API surface), **scope/correlation** propagation design, and **benchmarks** documenting throughput limits with bounded channels.
