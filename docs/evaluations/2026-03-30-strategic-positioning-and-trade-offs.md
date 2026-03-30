# Evaluation: Strategic positioning and trade-offs (2026-03-30)

**Subject:** Where Frank.PulseFlow **fits** in the .NET ecosystem, what it **optimizes for**, and what **problems it should not be asked to solve**.

---

## 1. Problem statement the library solves well

**In-process modular reactions** to **typed events** with:

- **Single consumer** discipline borrowed from **channels** (ordered dequeue, explicit backpressure policy via channel config).
- **Multiple parallel reactions** per event (fan-out) **without** inventing a custom thread pool.
- **DI-native** registration suitable for **ASP.NET Core**, **workers**, and **generic host** apps.

**Canonical sweet spot:** “**Audit + domain + side effect**” style processing where **ordering between events** matters more than **ordering between independent reactions to the same event**.

---

## 2. Problems it does not solve (and should not be mistaken for)

1. **Distributed messaging** — no durability, no cross-process delivery guarantees.
2. **Request/response** — no correlation or reply channel in the core API.
3. **Workflow orchestration** — no saga, compensation, or state machine; **sagas** must be **hand-coded** in flows.
4. **Priority scheduling** — no first-class priority API; only modeling via **separate types/channels/hosts**.
5. **High-volume telemetry** — logging bridge is **convenient**, not **performance-tuned** for massive log throughput (see [Logging bridge](2026-03-30-logging-ilogger-bridge.md)).

---

## 3. Competitive stance (conceptual)

| Pattern | When PulseFlow is competitive | When alternatives win |
|---------|--------------------------------|----------------------|
| Raw `Channel<T>` | Want **naming**, **DI**, **multi-handler** policy | Want **minimal** abstraction |
| MediatR | Want **parallel** multi-handler per message, **channel** backpressure | Want **pipeline behaviors**, **single** handler semantics |
| TPL Dataflow | Want **simple** bus + **DI** | Want **graph** of blocks, **linking** |
| Message brokers | In-process **only** | Cross-service **reliability** |

**Deep point:** PulseFlow’s **parallel fan-out** is **not** MediatR’s default mental model. **Choosing** PulseFlow is partly choosing **orthogonal handlers** over **sequential pipelines**.

---

## 4. Maintenance and evolution risks

1. **Dependency on `Frank.Channels.DependencyInjection`** — behavior changes there **ripple** here.
2. **`Microsoft.Extensions.Logging` evolution** — provider integration must **track** filter and state object patterns.
3. **API stability** — public surface is **small**, which is **good**; breaking changes should be **rare** and **documented**.

---

## 5. Test strategy as quality signal

The test suite mixes **fast unit tests** (`PulseFlowLoggerTests`, `GenericFlowTests`) with **host-based** integration (`PulseFlowTests`, `PulseNexusResilienceTests`, **`PulseFlowDiagnosticsTests`**). **Evaluation:** **Directionally correct**; **gaps** remain around **channel backpressure**, **shutdown races**, and **multi-threaded stress** (not necessarily required for v1 library).

---

## 6. Strategic recommendation

**Positioning:** Market as **“channels + DI + fan-out dispatch”**, not as **“enterprise service bus.”**

**Investment order if extending:**

1. **Benchmarks** + guidance on **bounded channels** and **handler time budgets**.
2. **Optional** async logging pipeline **only** if users prove **sync `Log`** is a **bottleneck**.
3. **First-party metrics or `EventSource`** only if **`ConfigurePulseFlowDiagnostics`** proves insufficient for host telemetry needs.

---

## 7. Conclusion

Frank.PulseFlow is **architecturally narrow** and **strong within that narrow band**. The **deepest strategic risk** is **expectation drift**: users treating it as **infrastructure-grade messaging** rather than **structured in-process orchestration sugar** over **System.Threading.Channels**.

Clear **docs + evaluations** (this folder) reduce that risk by stating **non-goals** explicitly.
