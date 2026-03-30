# Glossary

| Term | Meaning |
|------|---------|
| **Pulse** | An instance implementing **`IPulse`**; the unit of message data. |
| **Conduit** | **`IConduit`** — application-facing API for sending pulses (backed by **`ChannelWriter<IPulse>`**). |
| **Nexus** | Conceptual hub; implemented as **`PulseNexus`**, the hosted consumer that reads the channel and dispatches to flows. |
| **Flow** | **`IFlow`** — participates in dispatch via **`CanHandle`** and **`HandleAsync`**. |
| **Handler** | **`IPulseHandler<T>`** — typed handler; wired through **`GenericFlow<TPulse, THandler>`**. |
| **Fan-out** | Multiple flows handling the **same** pulse type; executed **in parallel** for one pulse. |
| **Log pulse** | **`LogPulse`** — logging package type representing a single log event on the bus. |
| **Frank.Channels.DependencyInjection** | Companion package that registers **`Channel<T>`** (and related types) in DI; PulseFlow depends on it. |

## See also

- [Concepts index](concepts/README.md)
