# Custom flows

You can extend PulseFlow in two main ways: **`IPulseHandler<T>`** (preferred for one pulse type) or a **custom `IFlow`** (for richer routing).

## Option A — `IPulseHandler<T>` + `AddPulseFlow<TPulse, THandler>`

Best when:

- One handler class maps to **one** pulse type.
- You want **`HandleAsync(TPulse)`** without casting.

See [Quickstart](quickstart-host.md).

## Option B — Implement `IFlow` directly

Best when:

- One class should handle **several** pulse types.
- You need **custom** `CanHandle` logic (hierarchy, naming, feature flags).

Example sketch:

```csharp
public sealed class MultiPulseFlow : IFlow
{
    public bool CanHandle(Type pulseType) =>
        pulseType == typeof(OrderCreatedPulse)
        || pulseType == typeof(OrderCancelledPulse);

    public async Task HandleAsync(IPulse pulse, CancellationToken cancellationToken)
    {
        switch (pulse)
        {
            case OrderCreatedPulse created:
                await OnCreated(created, cancellationToken);
                break;
            case OrderCancelledPulse cancelled:
                await OnCancelled(cancelled, cancellationToken);
                break;
            default:
                throw new InvalidOperationException($"Unexpected pulse: {pulse.GetType()}");
        }
    }

    private static Task OnCreated(OrderCreatedPulse p, CancellationToken ct) => Task.CompletedTask;
    private static Task OnCancelled(OrderCancelledPulse p, CancellationToken ct) => Task.CompletedTask;
}
```

Register:

```csharp
services.AddPulseFlow<MultiPulseFlow>();
```

## Fan-out

Multiple flows may **`CanHandle`** the same type. All run **in parallel** for one pulse. Design shared state carefully; see [Dispatch and ordering](../architecture/dispatch-and-ordering.md).

## Testing

See [Testing](testing.md).
