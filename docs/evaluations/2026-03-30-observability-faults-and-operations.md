# Evaluation: Observability, faults, and operations (2026-03-30)

**Subject:** How operators and engineers **observe** PulseFlow behavior in development and production: failures, drops, metrics, tracing.

---

## 1. Summary verdict

Operational **observability remains host-driven** for metrics and tracing, but **optional diagnostics callbacks** (`PulseFlowDiagnosticsOptions` via **`ConfigurePulseFlowDiagnostics`**) now cover **unmatched pulses** and **flow faults** without taking a dependency on **`ILogger`**. **Flow failures** still go to **`System.Diagnostics.Trace`** by default; there are **no built-in metrics** (counters, histograms) or **OpenTelemetry** hooks in the core library.

**Fit:** Acceptable for **libraries** that defer telemetry to **hosts**. **Risky** for teams expecting **batteries-included** runbooks without additional instrumentation.

---

## 2. Flow failure path

On non-cancellation exceptions, `PulseNexus` calls **`Trace.TraceError`** with flow type, pulse type, and exception.

### 2.1 Strengths

- **No recursion** into `ILogger` when `PulseFlow.Logging` is enabled.
- **Low dependency** footprint.

### 2.2 Weaknesses (deep)

1. **Many production deployments** do not attach **trace listeners** that surface `Trace` to centralized logging.
2. **No severity levels** beyond the trace call—**cannot** distinguish **transient** vs **fatal** without parsing strings.
3. **No `Activity` / span** integration—**distributed tracing** across pulses is **manual** (add fields to pulses).

---

## 3. Silent drops (no matching `IFlow`)

When no flow matches, the pulse is **skipped**. **Default:** no trace, metric, or callback (same as before). **Optional:** register **`UnmatchedPulse`** on **`PulseFlowDiagnosticsOptions`** to observe mis-routing without changing dispatch semantics.

**Operational impact:** Teams that do not configure diagnostics still see **“sometimes nothing happens”** for no-match cases—same as prior releases.

**Recommendation:** Use **`ConfigurePulseFlowDiagnostics`** in development or production hosts that need visibility; product-level **dead-letter** or **debug-only assert** remains an application concern. See [Core messaging runtime](2026-03-30-core-messaging-runtime.md) §2.3.

---

## 4. Logging bridge observability

When using `Frank.PulseFlow.Logging`, **application logs** become **`LogPulse`** traffic. **Advantages:**

- **Unified** processing pipeline.
- **Testability** via capturing flows.

**Disadvantages:**

- **Log storms** can **mask** domain latency in **absence** of metrics.
- **PII** in logs flows through **same** channel as domain events—**access control** and **redaction** must be **implemented in flows**, not provided by the library.

---

## 5. Cancellation and shutdown

`ReadAllAsync` respects **`stoppingToken`**. In-flight `WhenAll` **should** complete as flows observe cancellation.

**Deep gap:** **`PulseFlowLogger`** uses **`CancellationToken.None`** on send—shutdown **may** briefly **block** on channel backpressure while the host is stopping.

---

## 6. Recommended operational package (external to library)

Teams should **layer**:

1. **Metrics:** pulse count in/out, handler duration histogram, channel depth (if exposed by channel implementation).
2. **Logs:** forward `Trace` to **Serilog / OpenTelemetry** in host setup **or** replace trace calls with **`EventSource`** (more control, more code).
3. **Tracing:** propagate **`Activity.Current`** or explicit correlation ids on **domain** pulses.

---

## 7. Conclusion

PulseFlow is **not** operationally “opaque,” but it is **not** self-describing in production either. **Failures are discoverable** only if the host **wires** `Trace` correctly.

**Library enhancement (done):** **`IOptions<PulseFlowDiagnosticsOptions>`** with **`UnmatchedPulse`** and **`FlowFault`** callbacks, configured via **`ConfigurePulseFlowDiagnostics`**, without a hard dependency on **`ILogger`**.
