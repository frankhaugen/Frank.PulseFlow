# Exceptions

## `IncompatibleFlowException` (Frank.PulseFlow)

Thrown from **`GenericFlow<TPulse, THandler>.HandleAsync`** when the pulse is not actually a **`TPulse`**.

**Meaning:** Internal inconsistency—**`CanHandle`** should have prevented this. Not expected from normal user configuration.

**Action:** Report a bug with a minimal repro if this surfaces in production.

## Other failures

- **Channel closed** — writing after completion may throw **`ChannelClosedException`** (from `System.Threading.Channels`); handle shutdown ordering carefully.
- **Operation canceled** — **`SendAsync`** / **`ReadAllAsync`** propagate **`OperationCanceledException`** when the host stops; treat as normal shutdown.

## See also

- [Dispatch and ordering](../architecture/dispatch-and-ordering.md)
