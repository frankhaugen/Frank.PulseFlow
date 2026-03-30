# Concepts

PulseFlow uses a small vocabulary. These documents mirror how the code is structured.

| Concept | Role | Document |
|---------|------|----------|
| **Pulse** | Message payload implementing **`IPulse`** | [Pulses](pulses.md) |
| **Conduit** | Send-side API over **`ChannelWriter<IPulse>`** | [Conduit and channel](conduit-and-channel.md) |
| **Nexus** | Hosted consumer reading the channel | [Nexus](nexus.md) |
| **Flow** | Handler participating in dispatch (`IFlow`) | [Flows and handlers](flows-and-handlers.md) |

## Reading order

1. [Pulses](pulses.md) — what you put on the bus.  
2. [Conduit and channel](conduit-and-channel.md) — how pulses enter the system.  
3. [Nexus](nexus.md) — how they leave the channel.  
4. [Flows and handlers](flows-and-handlers.md) — how you react to them.

Then see [Runtime model](../architecture/runtime-model.md) for an end-to-end picture.
