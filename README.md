# Frank.PulseFlow

Frank.PulseFlow is an innovative C# library that wraps the Channel<T> type, enhancing its DI friendliness and intuitive use in data-vent-driven architectures. It provides a streamlined approach for efficient message handling in various applications.

## Key Features
- **Pulse**: Represents messages in the system.
- **Conduit (IConduit)**: Acts as a message transmitter.
- **PulseFlow (IPulseFlow)**: Serves as a message handler.
- **Nexus**: A BackgroundService that processes all Pulses and routes them to the appropriate IPulseFlow handlers.

## Getting Started
To integrate Frank.PulseFlow, add it as a NuGet package and use `.AddPulseFlow()` on `IServiceCollection`.

## Usage
Implement the `IPulseFlow` interface in your classes and register these implementations with the `AddPulseFlow` method in your service collection.

Frank.PulseFlow simplifies message handling, making your .NET applications more efficient and modular.
