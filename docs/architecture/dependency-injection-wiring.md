# Dependency injection wiring

PulseFlow is designed for **`Microsoft.Extensions.DependencyInjection`** and the generic host.

## What gets registered

Calling **`AddPulseFlow`** (any overload) eventually ensures:

| Service | Lifetime | Notes |
|---------|----------|--------|
| `Channel<IPulse>` | Singleton | Via `AddChannel<IPulse>()` from Frank.Channels.DependencyInjection |
| `IConduit` → `Conduit` | Singleton | First registration wins if already present |
| Each `IFlow` | Singleton | Multiple `IFlow` registrations coexist |
| `PulseNexus` | Hosted service | Single `BackgroundService` consumer |
| `IOptions<PulseFlowDiagnosticsOptions>` | Options | Registered (via `AddOptions`) the first time `PulseNexus` is wired; configure with `ConfigurePulseFlowDiagnostics` |

**`AddPulseFlow<TPulse, THandler>`** additionally registers:

- `IPulseHandler<TPulse>` → `THandler`
- `THandler` as itself
- `IFlow` → `GenericFlow<TPulse, THandler>` (if not already present)

## Idempotent guards

The extension methods use **`Any(...)`** checks to avoid duplicate **`IFlow`**, **`PulseNexus`**, **`IConduit`**, and **`Channel<IPulse>`** registrations when you call **`AddPulseFlow`** multiple times (e.g. multiple feature modules).

## Constructor injection in `PulseNexus`

The internal **`PulseNexus`** type is constructed with **`ChannelReader<IPulse>`**, **`IEnumerable<IFlow>`**, and **`IOptions<PulseFlowDiagnosticsOptions>`**. The DI container resolves **all** registered **`IFlow`** implementations into the enumerable.

With **Microsoft.Extensions.DependencyInjection**, the **order** of **`IFlow`** instances in that enumerable typically follows **registration order**. That order is **not** a portable contract across all DI containers, but it can matter for **debugging** and for **races** when parallel flows touch shared mutable state (prefer no shared mutable state).

## Frank.Channels.DependencyInjection

The core package depends on **Frank.Channels.DependencyInjection** for channel registration. Channel **options** (bounded capacity, full mode, etc.) are configured through that package’s API, not PulseFlow’s.

## See also

- [Extension methods reference](../reference/extension-methods.md)
- [Building and testing](../development/building.md)
