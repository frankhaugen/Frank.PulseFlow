# Testing applications that use PulseFlow

## Unit testing handlers

Test **`IPulseHandler<T>`** (or **`IFlow`**) in isolation by calling **`HandleAsync`** with a constructed pulse and a **`CancellationToken`** (e.g. **`CancellationToken.None`** or a **`CancellationTokenSource`**).

No channel or host is required for **pure** handler logic.

## Integration testing with the generic host

Patterns that work well:

1. Build a **`HostApplicationBuilder`** (or **`Host.CreateDefaultBuilder`**).
2. Register **`AddPulseFlow*`** the same way as production.
3. Replace **`IConduit`** with a **test double** that records pulses, **or** run the real channel and assert on side effects (files, mocks, in-memory stores).

If you use **`Frank.Testing.TestBases.HostApplicationTestBase`**, pass an explicit **`LogLevel`** to the base constructor when you rely on **`Frank.PulseFlow.Logging`** so logger providers are not filtered out entirely by minimum-level configuration.

## Asserting ordering

Remember: **multiple flows** for one pulse run **in parallel**. Tests that assume sequential side-effect order across flows may be flaky unless you synchronize or consolidate logic into one flow.

## See also

- [Dispatch and ordering](../architecture/dispatch-and-ordering.md)
- [Building and testing](../development/building.md)
