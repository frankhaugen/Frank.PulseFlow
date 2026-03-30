# Repository layout

```
Frank.PulseFlow/
├── Frank.PulseFlow/              # Core library (NuGet: Frank.PulseFlow)
├── Frank.PulseFlow.Logging/      # Logging adapter (NuGet: Frank.PulseFlow.Logging)
├── Frank.PulseFlow.Tests/        # Unit / integration tests
├── docs/                         # This documentation tree
├── Directory.Build.props         # Shared MSBuild properties (TFM, package metadata, analyzers)
├── Frank.PulseFlow.slnx          # Solution (XML format)
├── README.md                     # Project overview and diagrams
├── CONTRIBUTING.md
├── STYLE.md
├── LICENSE
└── .github/workflows/            # CI entry points
```

## Projects

| Project | Output | Role |
|---------|--------|------|
| `Frank.PulseFlow` | `Frank.PulseFlow.dll` | Core messaging |
| `Frank.PulseFlow.Logging` | `Frank.PulseFlow.Logging.dll` | `ILogger` → `LogPulse` |
| `Frank.PulseFlow.Tests` | Test assembly | Validates behavior |

## Documentation

- **Root README** — narrative, marketing-style overview, Mermaid diagrams.
- **`docs/`** — structured, maintainers’ and advanced users’ reference (this tree).

## See also

- [Documentation index](../README.md)
