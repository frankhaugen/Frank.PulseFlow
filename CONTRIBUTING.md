# Contributing

Not all contributions are code! We welcome contributions from everyone. Please see our [contributing guide](#guide) for more information.

## Guide

All contributions to this repository should follow a few simple rules:

1. All contributions must be licensed under the [MIT License](LICENSE)
2. All contributions are subject to the [Code of Conduct](#code-of-conduct)
3. All contributions must be made via pull request
4. All contributions are subject to review by a maintainer of the repository, and may be rejected for any reason at any time, even after being merged to `main`
5. Feature requests and bug reports should be made via [GitHub Issues](https://github.com/frankhaugen/Frank.PulseFlow/issues)
6. No contribution is too small, but they can be too large. If you're not sure if your contribution is too large, split it into multiple pull requests, or ask a maintainer for guidance, (You can find a list of maintainers in the [MAINTAINERS](MAINTAINERS.md) file)
    6.1. You can have multiple pull requests open at the same time, and reference each other in the description of the pull request and the same issue, (this is called a "pull request chain" and please use X of Y in the title of the pull request to indicate that it is part of a chain)
7. All contributions must be made in English (this includes comments, commit messages, pull request descriptions, etc.)
8. All contributions must be made in a way that is consistent with the existing codebase, (e.g. use the same coding style, naming conventions, etc.), see [Coding Style](STYLE.md) for more information


## Getting Started

1. Fork the repository on GitHub
2. Install the latest version of [.NET (Core)](https://dotnet.microsoft.com/download)
3. Install the latest version of [Visual Studio](https://visualstudio.microsoft.com/downloads/) or [Visual Studio Code](https://code.visualstudio.com/download) (or your preferred editor)
4. Clone the forked repository to your local machine
5. Open the solution in Visual Studio or Visual Studio Code
6. Create a branch for your changes
7. Make your changes
8. Build and test your changes
9. Commit and push your changes to your forked repository
10. Create a pull request to the `main` branch of the repository, and mention the issue number that you're addressing, (e.g. `#1234`) in the description of the pull request. Create a new issue if one does not already exist, describing the problem and how you solved it.
11. Wait for a maintainer to review your pull request and either merge it or request changes

## Building and Testing

### Using command line

1. Navigate to the root directory of the repository
2. Run `dotnet build` to build the solution or `./build.ps1` on Windows or `pwsh ./build.ps1` on Linux/macOS
3. Run `dotnet test` to run the tests or `./test.ps1` on Windows or `pwsh ./test.ps1` on Linux/macOS
4. Run `dotnet pack` to create NuGet packages or `./pack.ps1` on Windows or `pwsh ./pack.ps1` on Linux/macOS
5. Use the packages in the `artifacts/packages` directory in your application to test that they work as expected

## Code of Conduct

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact the maintainers of this project (see below).

## Maintainers

Repository owners and maintainers are listed in the [MAINTAINERS](MAINTAINERS.md) file.