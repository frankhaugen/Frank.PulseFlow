# Conduit and channel

## Channel

PulseFlow registers a **singleton** **`Channel<IPulse>`** using **Frank.Channels.DependencyInjection** (`AddChannel<IPulse>()`). That package supplies:

- A shared **`Channel<IPulse>`** instance.
- **`ChannelWriter<IPulse>`** and **`ChannelReader<IPulse>`** as injectable dependencies (per that package’s design).

Channel capacity, full-mode behavior, and single- vs multi-reader semantics are determined by **Frank.Channels.DependencyInjection**, not by PulseFlow itself. Consult that package’s documentation when tuning backpressure.

## Conduit

**`IConduit`** is the library’s **send-side** abstraction:

```csharp
Task SendAsync(IPulse message, CancellationToken cancellationToken);
```

The default implementation **`Conduit`** forwards **`SendAsync`** to **`ChannelWriter<IPulse>.WriteAsync`**.

### Why use `IConduit` instead of injecting the writer?

- **Clear intent** — call sites depend on “send a pulse,” not on channel primitives.
- **Easier testing** — replace **`IConduit`** with a fake or in-memory implementation.
- **Future extensibility** — interception, metrics, or fan-out could be implemented behind the same interface.

## Producer responsibilities

- Producers should pass a **`CancellationToken`** appropriate to the operation (e.g. request aborted, host shutdown).
- If the channel is bounded and full, **`WriteAsync`** may await; design capacity accordingly.

## See also

- [Nexus](nexus.md) — consumer side of the same channel
- [Runtime model](../architecture/runtime-model.md)
