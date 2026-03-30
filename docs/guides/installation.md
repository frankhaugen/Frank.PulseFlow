# Installation

## NuGet packages

| Package | Purpose |
|---------|---------|
| [Frank.PulseFlow](https://www.nuget.org/packages/Frank.PulseFlow) | Core: `IConduit`, `IFlow`, `AddPulseFlow*` |
| [Frank.PulseFlow.Logging](https://www.nuget.org/packages/Frank.PulseFlow.Logging) | Optional: `AddPulseFlow()` for `ILogger` → `LogPulse` |

### .NET CLI

```bash
dotnet add package Frank.PulseFlow
# optional:
dotnet add package Frank.PulseFlow.Logging
```

### Transitive dependency

**Frank.PulseFlow** depends on **Frank.Channels.DependencyInjection**, which registers **`Channel<T>`** and related types. You do not need to reference it explicitly unless you want to configure channel options at the source.

## Framework

Align your project with the **target framework** used by this repository (see root `Directory.Build.props`, currently **`net10.0`**).

## Solution layout (contributors)

Open **`Frank.PulseFlow.slnx`** at the repository root. See [Repository layout](../development/repository-layout.md).

## Next step

[Quickstart with the generic host](quickstart-host.md)
