# Evaluation: Dependency injection and registration (2026-03-30)

**Subject:** `ServiceCollectionExtensions`, `FlowBuilder`, interaction with `Frank.Channels.DependencyInjection`, hosted service lifetime.

---

## 1. Summary verdict

Registration APIs are **ergonomic** and **idempotent** in the common case (multiple feature modules calling `AddPulseFlow`). Coupling to **`AddChannel<IPulse>()`** is **necessary** but **opaque**: channel semantics are **imported**, not **owned**, by PulseFlow.

**Main deep concern:** **`IEnumerable<IFlow>`** resolution **order** is **not part of the public contract**, yet it affects **observable** behavior when combined with **parallel** execution (races on shared resources) and **debugging** (non-deterministic first-failure ordering in traces).

---

## 2. Idempotency analysis

### 2.1 `AddPulseFlow<TFlow>`

```52:65:Frank.PulseFlow/ServiceCollectionExtensions.cs
        if (!services.Any(service => service.ServiceType == typeof(IFlow) && service.ImplementationType == typeof(TFlow)))
            services.AddSingleton<IFlow, TFlow>();
        
        if (!services.Any(service => service.ServiceType == typeof(BackgroundService) && service.ImplementationType == typeof(PulseNexus)))
            services.AddHostedService<PulseNexus>();
        
        if (services.All(service => service.ServiceType != typeof(IConduit)))
            services.AddSingleton<IConduit, Conduit>();
        
        if (services.All(service => service.ServiceType != typeof(Channel<IPulse>)))
            services.AddChannel<IPulse>();
```

**Strengths:**

- Prevents duplicate **`IFlow` registrations** for the same implementation type.
- Ensures **single** `PulseNexus` and **single** `Channel<IPulse>` / `IConduit`.

**Limits:**

- Duplicate registration of **different** `IFlow` types is **intended**; duplicate **same** `TFlow` type is **blocked**—good.
- The check uses **`ImplementationType == typeof(TFlow)`**; generic flows like `GenericFlow<TPulse, THandler>` are **distinct** closed types—good.

### 2.2 `AddPulseFlow<TPulse, THandler>`

Guards **`IPulseHandler<TPulse>`** and **`GenericFlow<TPulse, THandler>`** similarly. **Two handlers** for the **same** `TPulse` **must** differ by **`THandler`** type—this matches C# type system reality.

**Edge case:** If someone registers **two** `IFlow` implementations **both** handling the same pulse type **without** using `GenericFlow` twice (two custom flows), both run—**by design**.

---

## 3. `Frank.Channels.DependencyInjection` coupling

PulseFlow **does not** abstract channel options. **Evaluation:**

- **Pro:** Avoids duplicating configuration surface area.
- **Con:** **Discoverability** suffers—new users read PulseFlow docs but must **also** read channel package docs for **production** behavior.

**Recommendation:** Keep a **short, opinionated** “production checklist” in main docs: bounded vs unbounded, `FullMode`, single-reader assumptions.

---

## 4. `PulseNexus` constructor injection

`PulseNexus` receives **`IEnumerable<IFlow>`**. In Microsoft.Extensions.DependencyInjection, this resolves **all** registered `IFlow` services. **Order** follows registration order **in typical containers**, but **this is not a guaranteed public API contract** across containers or future DI versions.

**Deep impact:** Because handlers for one pulse run **in parallel**, **order** is usually irrelevant **unless** flows contend on shared resources—then **order** can influence **race** outcomes under load.

---

## 5. Fluent `IFlowBuilder`

`AddPulseFlow(Action<IFlowBuilder>)` delegates to `FlowBuilder.AddFlow<T>()`, which calls `AddPulseFlow<T>()` per flow. **Evaluation:** Clean **syntax**; no additional semantic layer.

---

## 6. Conclusion

DI design is **appropriate** for a small library. The **deepest** architectural tension is **externalized channel policy** combined with **implicit IEnumerable ordering** for debugging and contention analysis.

**If the library matures:** consider an **`IPulseRouter`** or **immutable dispatch table** built at startup (explicit order, possibly keyed by pulse type) rather than **filtering** `IEnumerable<IFlow>` per message.
