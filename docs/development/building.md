# Building and testing

## Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) matching the repository target (see `Directory.Build.props`).

## Solution

From the repository root:

```bash
dotnet restore Frank.PulseFlow.slnx
dotnet build Frank.PulseFlow.slnx -c Release
dotnet test Frank.PulseFlow.slnx -c Release
```

## Pack (NuGet)

```bash
dotnet pack Frank.PulseFlow.slnx -c Release
```

Packable projects produce `.nupkg` under each project’s `bin/Release` output (see `Directory.Build.props` for pack metadata).

## Continuous integration

GitHub Actions workflows in `.github/workflows` delegate to reusable workflows under **`frankhaugen/Workflows`**. Ensure that shared workflow targets the correct SDK band for this repository.

## See also

- [Repository layout](repository-layout.md)
- [CONTRIBUTING.md](../../CONTRIBUTING.md)
