# What is PulseFlow?

**Frank.PulseFlow** is a small library for **in-process asynchronous messaging** in .NET applications. It wraps `System.Threading.Channels` so that:

- Multiple parts of your app can **publish** messages (called **pulses**) through a single abstraction: **`IConduit`**.
- A background **consumer** (**`PulseNexus`**) reads from a shared **`Channel<IPulse>`** and **routes** each pulse to one or more **handlers** (**`IFlow`** implementations).

It is **not** a distributed message bus (no Kafka/RabbitMQ/Azure Service Bus). Everything happens **inside one process**, which keeps latency low and the model easy to reason about.

## Mental model

1. You define types that implement **`IPulse`** (often inheriting **`BasePulse`** for `Id` and `Created`).
2. You register **flows**: classes that implement **`IFlow`** and decide which pulse types they handle via **`CanHandle(Type)`**.
3. You optionally use **`IPulseHandler<T>`** with **`AddPulseFlow<TPulse, THandler>`** so handlers stay strongly typed.
4. At runtime, **`IConduit.SendAsync`** writes to the channel; **`PulseNexus`** reads and dispatches.

## Companion package: Frank.PulseFlow.Logging

**Frank.PulseFlow.Logging** registers an **`ILoggerProvider`** that turns log events into **`LogPulse`** instances on the **same** channel, so logging can be processed by ordinary flows (files, test hooks, custom sinks) with the same threading semantics as the rest of your pulses.

## Next steps

- [When to use (and when not to)](when-to-use.md)
- [Concepts: pulses, conduit, nexus, flows](../concepts/README.md)
- [Quickstart](../guides/quickstart-host.md)
