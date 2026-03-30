# Evaluations

This folder holds **dated, subject-scoped engineering evaluations** of Frank.PulseFlow. They are intentionally more critical and exploratory than the neutral guides under [`../guides/`](../guides/README.md).

## Naming convention

`YYYY-MM-DD-<subject-slug>.md` — the date marks when the evaluation was written (or last materially revised), not an expiry.

## Index

| Date | Subject | Document |
|------|---------|----------|
| 2026-03-30 | Core messaging runtime (`PulseNexus`, channel, fan-out) | [2026-03-30-core-messaging-runtime.md](2026-03-30-core-messaging-runtime.md) |
| 2026-03-30 | Logging bridge (`ILogger` → `LogPulse`) | [2026-03-30-logging-ilogger-bridge.md](2026-03-30-logging-ilogger-bridge.md) |
| 2026-03-30 | Dependency injection and registration | [2026-03-30-dependency-injection-registration.md](2026-03-30-dependency-injection-registration.md) |
| 2026-03-30 | Type model and routing semantics | [2026-03-30-type-model-and-routing.md](2026-03-30-type-model-and-routing.md) |
| 2026-03-30 | Observability, faults, and operations | [2026-03-30-observability-faults-and-operations.md](2026-03-30-observability-faults-and-operations.md) |
| 2026-03-30 | Strategic positioning and trade-offs | [2026-03-30-strategic-positioning-and-trade-offs.md](2026-03-30-strategic-positioning-and-trade-offs.md) |

## How to use these documents

- Treat conclusions as **hypotheses to validate** in your own load and failure testing.
- Prefer the **reference source** (the `.cs` files cited in each evaluation) when behavior is ambiguous.
- **Line-number code fences** are approximate; if they drift from `main`, trust the repository source.

**Changelog (substantive):** March 2026 — evaluations updated for **`PulseFlowDiagnosticsOptions`**, **`ConfigurePulseFlowDiagnostics`**, **`IPulse.Id`** in fault traces, and current **`PulseFlowLogger`** / **`GenericFlow`** excerpts.

[← Documentation home](../README.md)
