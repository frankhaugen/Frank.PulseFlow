# Frank.PulseFlow documentation

Welcome to the documentation for **Frank.PulseFlow** and **Frank.PulseFlow.Logging**—in-process messaging built on `System.Threading.Channels`, integrated with `Microsoft.Extensions.DependencyInjection` and the generic host.

## Start here

| If you want to… | Read |
|-----------------|------|
| Understand what this is and when to use it | [Overview: what is PulseFlow?](overview/what-is-pulseflow.md) · [When to use (and when not to)](overview/when-to-use.md) |
| Install and run a minimal host | [Installation](guides/installation.md) · [Quickstart with the generic host](guides/quickstart-host.md) |
| Learn the vocabulary (Pulse, Conduit, Nexus, Flow) | [Concepts index](concepts/README.md) |
| See how messages move at runtime | [Runtime model](architecture/runtime-model.md) · [Dispatch and ordering](architecture/dispatch-and-ordering.md) |
| Route `ILogger` through the same pipe | [Logging integration](guides/logging-integration.md) · [Logging package](nuget/pulseflow-logging.md) |
| Look up APIs and registration | [Public API surface](reference/public-api.md) · [Extension methods](reference/extension-methods.md) |
| Build from source | [Building and testing](development/building.md) |
| Quick definitions | [Glossary](glossary.md) |
| Common questions | [FAQ](faq.md) |
| Deep-dive critiques (architecture, ops, trade-offs) | [Evaluations index](evaluations/README.md) |

## Documentation map

### Overview

- [What is PulseFlow?](overview/what-is-pulseflow.md)
- [When to use](overview/when-to-use.md)
- [Comparison with similar patterns](overview/comparison-with-similar-patterns.md)

### Concepts

- [Concepts index](concepts/README.md)
- [Pulses (`IPulse`)](concepts/pulses.md)
- [Conduit and channel](concepts/conduit-and-channel.md)
- [Nexus (consumer)](concepts/nexus.md)
- [Flows and handlers](concepts/flows-and-handlers.md)

### Architecture

- [Runtime model](architecture/runtime-model.md)
- [Dispatch and ordering](architecture/dispatch-and-ordering.md)
- [Dependency injection wiring](architecture/dependency-injection-wiring.md)

### Guides

- [Installation](guides/installation.md)
- [Quickstart (generic host)](guides/quickstart-host.md)
- [Custom flows](guides/custom-flows.md)
- [Logging integration](guides/logging-integration.md)
- [Testing applications that use PulseFlow](guides/testing.md)

### NuGet (documentation)

- [Frank.PulseFlow (core)](nuget/frank-pulseflow.md)
- [Frank.PulseFlow.Logging](nuget/pulseflow-logging.md)

### Reference

- [Public API surface](reference/public-api.md)
- [Extension methods](reference/extension-methods.md)
- [Exceptions](reference/exceptions.md)

### Development

- [Building and testing](development/building.md)
- [Repository layout](development/repository-layout.md)

### Evaluations (dated, subject-scoped)

- [Evaluations index](evaluations/README.md) — critical analysis, not tutorial prose

### Other

- [Glossary](glossary.md)
- [FAQ](faq.md)
- [Diagrams (Mermaid)](diagrams/README.md)

## Related material in the repo

- Root [README.md](../README.md) — high-level introduction, diagrams, and narrative use cases
- [CONTRIBUTING.md](../CONTRIBUTING.md) — how to contribute
- [STYLE.md](../STYLE.md) — coding style
- Solution file: [Frank.PulseFlow.slnx](../Frank.PulseFlow.slnx)

## Conventions

- Code samples assume **.NET 10** (or a current LTS aligned with the repository target framework) unless noted otherwise.
- `Frank.Channels.DependencyInjection` is a required dependency of the core package; channel lifetime and options come from that package.
