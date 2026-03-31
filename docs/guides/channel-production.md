# Channel configuration for production

PulseFlow does **not** register `Channel<IPulse>` itself. Capacity, **full mode**, and related behavior come from **Frank.Channels.DependencyInjection** when you call **`AddChannel<IPulse>()`** (invoked inside **`AddPulseFlow`**).

Use this checklist when moving beyond local development:

1. **Bounded vs unbounded** — Unbounded queues hide slow consumers and can grow memory without bound. Prefer a **bounded** capacity that matches your worst acceptable backlog, or accept explicit **backpressure** / **drop** semantics from the channel package.
2. **`BoundedChannelFullMode`** — Decide whether writers **wait**, **drop**, or **throw** when the queue is full. This choice is part of your **SLO**, not PulseFlow’s default.
3. **Single reader** — **`PulseNexus`** is the only consumer of **`ChannelReader<IPulse>`** by design. Do not add a second reader to the same channel unless you replace the nexus model entirely.

See [Conduit and channel](../concepts/conduit-and-channel.md) and the **Frank.Channels.DependencyInjection** documentation for API details.

## See also

- [Installation](installation.md)
- [Dispatch and ordering](../architecture/dispatch-and-ordering.md)
