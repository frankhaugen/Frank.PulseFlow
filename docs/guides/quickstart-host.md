# Quickstart (generic host)

Minimal pattern: register flows, inject **`IConduit`**, send pulses from a hosted service.

## 1. Define a pulse

```csharp
public sealed class GreetingPulse : BasePulse
{
    public required string Name { get; init; }
}
```

## 2. Handle it with a typed handler

```csharp
public sealed class GreetingHandler : IPulseHandler<GreetingPulse>
{
    public Task HandleAsync(GreetingPulse pulse, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Hello, {pulse.Name}!");
        return Task.CompletedTask;
    }
}
```

## 3. Register PulseFlow and run

```csharp
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddPulseFlow<GreetingPulse, GreetingHandler>();

var app = builder.Build();
await app.RunAsync();
```

To **send** a pulse from another service, inject **`IConduit`**:

```csharp
public sealed class GreeterService(IConduit conduit) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await conduit.SendAsync(new GreetingPulse { Name = "PulseFlow" }, stoppingToken);
    }
}
```

Register the greeter:

```csharp
builder.Services.AddHostedService<GreeterService>();
```

## Multiple flows

Use the fluent builder to add several **`IFlow`** types:

```csharp
builder.Services.AddPulseFlow(b => b
    .AddFlow<AuditFlow>()
    .AddFlow<MetricsFlow>());
```

Or call **`AddPulseFlow<TFlow>()`** multiple times.

## Logging (optional)

See [Logging integration](logging-integration.md).

## See also

- [Custom flows](custom-flows.md)
- [Concepts index](../concepts/README.md)
