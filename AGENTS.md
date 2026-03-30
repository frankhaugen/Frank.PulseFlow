# Agent guide (Frank.PulseFlow)

Concise instructions for AI assistants and automation working in this repository.

## What this is

- **Frank.PulseFlow** — In-process messaging over `System.Threading.Channels`, wired for `Microsoft.Extensions.DependencyInjection`: `IConduit` → `Channel<IPulse>` → internal `PulseNexus` (`BackgroundService`) → `IFlow` / `IPulseHandler<T>`.
- **Frank.PulseFlow.Logging** — Optional `ILoggerProvider` that sends `LogPulse` through the same `IConduit`.
- Depends on **Frank.Channels.DependencyInjection** for `Channel<T>` registration (capacity / backpressure are configured there, not in PulseFlow).

## Repository layout

| Path | Role |
|------|------|
| `Frank.PulseFlow/` | Core library |
| `Frank.PulseFlow.Logging/` | Logging adapter |
| `Frank.PulseFlow.Tests/` | xUnit tests |
| `docs/` | Structured documentation ([`docs/README.md`](docs/README.md)) |
| `Frank.PulseFlow.slnx` | Solution file (preferred entry point) |

## Build, test, pack

Run from the repository root:

```bash
dotnet restore Frank.PulseFlow.slnx
dotnet build Frank.PulseFlow.slnx -c Release
dotnet test Frank.PulseFlow.slnx -c Release
dotnet pack Frank.PulseFlow.slnx -c Release
```

Prefer **Release** for CI-parity checks. Do not assume tests were run unless you executed `dotnet test`.

## Conventions

- **Target framework:** `net10.0` (see `Directory.Build.props`).
- **Nullable** and **implicit usings** are enabled.
- **XML documentation** is generated; match existing doc style on public APIs.
- Follow **[STYLE.md](STYLE.md)** and **[CONTRIBUTING.md](CONTRIBUTING.md)** for edits and PRs.

## Architecture notes (for code changes)

- **Dispatch:** One reader loop; for each pulse, all matching `IFlow` instances run in parallel; **per-flow exceptions are isolated** (logged with `System.Diagnostics.Trace.TraceError`, host cancellation still propagates). Matching flows are **cached per pulse runtime type**; treat **`CanHandle`** as stable after startup. Optional **`PulseFlowDiagnosticsOptions`** callbacks (`ConfigurePulseFlowDiagnostics`) can observe unmatched pulses and non-cancellation faults. See `Internal/PulseNexus.cs`.
- **Routing:** `GenericFlow<TPulse, THandler>` matches **`pulse.GetType() == typeof(TPulse)`** only (no subtype routing).
- **Logging:** `PulseFlowLogger` extracts structured `TState` when it matches key/value shapes; otherwise `LogPulse.State` is null. `BeginScope` snapshots appear on `LogPulse.Scope`. `ILoggingBuilder.AddPulseFlow()` uses `CancellationToken.None` on `SendAsync` (avoid resolving `IHostApplicationLifetime` during host logger factory setup). Avoid unsafe casts on `state`.

## Tests

- **Frank.PulseFlow.Tests** has `InternalsVisibleTo` access to `Frank.PulseFlow` internals (see `Directory.Build.props`).
- Integration-style tests using **`Frank.Testing.TestBases.HostApplicationTestBase`**: pass an explicit **`LogLevel`** to the base constructor when **`Frank.PulseFlow.Logging`** is used (e.g. `LogLevel.Information`), or log-related assertions may see no pulses.
- Prefer **temp paths** for file sinks in tests (`Path.GetTempPath()`, unique file names)—do not write artifacts into the repo root.

## CI

Workflows under `.github/workflows/` call reusable workflows from **`frankhaugen/Workflows`**; ensure that pipeline uses a **.NET 10** SDK compatible with this repo.

## Documentation

- Narrative overview and diagrams: root **[README.md](README.md)**.
- Deeper topics: **[docs/README.md](docs/README.md)**.
When adding features, update **XML docs** and, if behavior is user-visible, a short **docs/** page or **FAQ** entry where appropriate.
