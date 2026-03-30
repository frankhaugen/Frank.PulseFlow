# Pulses (`IPulse`)

A **pulse** is a unit of data flowing through PulseFlow. All pulses implement **`IPulse`**.

## `IPulse`

The interface defines shared metadata:

- **`Id`** — `Guid`, identifies the pulse instance.
- **`Created`** — `DateTime`, typically UTC creation time.

Implementations are free to add domain-specific properties and behavior.

## `BasePulse`

The library provides **`BasePulse`**, an abstract class that:

- Initializes **`Id`** with **`Guid.NewGuid()`**.
- Sets **`Created`** to **`DateTime.UtcNow`** at construction.

Most application-specific pulses inherit **`BasePulse`** for consistency.

## Design notes

- Pulses on the wire are typed as **`IPulse`** inside **`Channel<IPulse>`**. Strong typing appears again at **`IPulseHandler<T>`** and inside **`IFlow`** implementations that cast or pattern-match.
- **`GenericFlow<TPulse, THandler>`** routes only when **`pulse.GetType()`** equals **`typeof(TPulse)`** exactly—inheritance-based routing requires a custom **`IFlow`**.

## Related types

- **`LogPulse`** (logging package) — carries **`LogLevel`**, **`EventId`**, message text, and optional state; see [Logging package](../nuget/pulseflow-logging.md).

## See also

- [Flows and handlers](flows-and-handlers.md)
- [Dispatch and ordering](../architecture/dispatch-and-ordering.md)
