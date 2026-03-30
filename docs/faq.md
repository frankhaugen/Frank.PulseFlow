# Frequently asked questions

## Is PulseFlow a message broker?

No. It is **in-process** only, built on **`System.Threading.Channels`**. There is no cross-machine delivery, persistence, or replay.

## Can multiple handlers run for one pulse?

Yes. Every **`IFlow`** with **`CanHandle`** true runs, **in parallel**, for that pulse. See [Dispatch and ordering](architecture/dispatch-and-ordering.md).

## How do I handle pulse subtypes?

**`GenericFlow`** matches **`pulse.GetType() == typeof(TPulse)`** only. Use a **custom `IFlow`** with broader **`CanHandle`** logic, or register separate handler pairs per concrete type.

## Where is backpressure configured?

Channel capacity and behavior come from **Frank.Channels.DependencyInjection** (`AddChannel<IPulse>()`). PulseFlow does not add its own channel options API.

## Why is my `LogPulse` flow never called?

Common causes:

- **`LoggerFilterOptions`** minimum level excludes your log events.
- **`AddPulseFlow()`** on logging was not called on **`ILoggingBuilder`**.
- No **`IFlow`** registered with **`CanHandle(typeof(LogPulse))`**.

See [Logging integration](guides/logging-integration.md).

## Does `SendAsync` block?

It **awaits** the channel write. For **bounded** channels, **`WriteAsync`** can wait until space is available. For **unbounded** channels, it typically completes quickly unless the system is under extreme memory pressure.

## Can I use PulseFlow without generic host?

The supported path is **`IServiceCollection`** registration. You could build a **`ServiceProvider`** manually, but that is not documented in detail here.

## Where is the full API list?

Start at [Public API surface](reference/public-api.md) and use IDE/XML docs for parameter-level detail.

## What happens if my `IFlow` throws?

**`PulseNexus`** isolates failures: the exception is written with **`System.Diagnostics.Trace.TraceError`**, other flows for the same pulse still run, and **later pulses** are still processed. Host shutdown cancellation is not swallowed. See [Dispatch and ordering](architecture/dispatch-and-ordering.md#handler-faults-isolation).

## Why is `LogPulse.State` sometimes null?

Structured log state is only captured when the logging framework passes a state object that implements **`IReadOnlyList<KeyValuePair<string, object?>>`** or **`IEnumerable<KeyValuePair<string, object?>>`**. Custom or third-party state types may leave **`State`** null while **`Message`** remains fully formatted.
