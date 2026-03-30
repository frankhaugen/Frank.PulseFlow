# When to use PulseFlow

## Good fits

- **Cross-cutting in-process pipelines** — e.g. “every important domain event also goes to an audit flow” without tangling every call site.
- **Single-writer, multi-reader style processing** — one ordered stream of work with **multiple independent reactions** per message (fan-out).
- **Thread-safe handoff to a single consumer loop** — you want producers to drop work on a channel and not worry about coordinating locks around a shared queue.
- **Unifying logging with other side effects** — when using **Frank.PulseFlow.Logging**, logs become **`LogPulse`** messages so one infrastructure path handles both domain pulses and log lines.
- **Teaching or prototyping channel-based designs** — the **`IConduit` / `IFlow`** split documents intent more clearly than raw `ChannelWriter<IPulse>` everywhere.

## Poor fits

- **Distributed messaging** — use a real broker or cloud messaging service; PulseFlow does not persist or replicate messages across processes or machines.
- **Request/response or RPC** — there is no built-in correlation id / reply channel pattern; **`IConduit`** is send-only from the caller’s perspective.
- **Strict priority queues** — the library exposes a **single** `Channel<IPulse>`; priorities require modeling (separate types, separate registrations, or separate channels) rather than a first-class priority API.
- **Handler-per-type middleware pipelines** — there is no ordered pipeline of stages per message type; **`PulseNexus`** invokes **all matching flows in parallel** for each pulse (see [Dispatch and ordering](../architecture/dispatch-and-ordering.md)).
- **Applications without DI** — registration is built around **`IServiceCollection`**; manual composition is possible but not the supported path today.

## Prerequisite mindset

You should be comfortable with:

- **`async`/`await`** and cancellation tokens.
- **Singleton channel + background reader** semantics (one consumer loop by default).
- **Runtime type-based routing** (`CanHandle(Type)`), not compile-time delegate tables only.

If that matches your scenario, continue with [Installation](../guides/installation.md) and the [Quickstart](../guides/quickstart-host.md).
