# Evaluation: Type model and routing semantics (2026-03-30)

**Subject:** `IPulse`, `IFlow`, `GenericFlow<TPulse, THandler>`, `CanHandle(Type)`, compile-time vs runtime typing.

---

## 1. Summary verdict

The type model is a **deliberate compromise**: **`Channel<IPulse>`** keeps the **wire** minimal, while **`IPulseHandler<T>`** restores **typed** handlers at the edge. Routing is **`Type`-based** and **exact** for `GenericFlow`, which is **simple to reason about** but **rejecting** common OOP patterns (inheritance-based polymorphic pulses) unless users implement **custom** `IFlow`.

---

## 2. `IPulse` as the universal carrier

`IPulse` exposes **`Id`** and **`Created`**. **`BasePulse`** supplies defaults.

**Strengths:**

- **Uniform** envelope for logging (`LogPulse`) and domain types.
- **Serialization-friendly** baseline if pulses ever cross boundaries (today they do not).

**Weaknesses:**

- **No version field** or schema id—**evolution** of pulse types over time is **application** responsibility.
- **No built-in causality** (parent id, trace id)—must be **added** to domain pulses if needed.

---

## 3. Exact runtime matching in `GenericFlow`

```14:14:Frank.PulseFlow/Internal/GenericFlow.cs
    public bool CanHandle(Type pulseType) => pulseType == typeof(TPulse);
```

### 3.1 Deep implications

1. **Subtype pulses** (`class DerivedPulse : MyPulse`) **do not** route to `AddPulseFlow<MyPulse, ...>`—they **fall through** unless another flow matches; the pulse is **skipped** by default, or observable via **`UnmatchedPulse`** when diagnostics are configured.
2. **Interface-typed** pulses are **awkward**: `typeof` of concrete instance is **implementation** type, not interface—usually fine, but **interface** as `TPulse` is **impossible** for `GenericFlow`’s `typeof` check unless the **actual** object is **exactly** that interface type (rare for classes).

**Evaluation:** This pushes users toward **composition over inheritance** for message types, or **explicit** custom `IFlow` routers—**not bad**, but **must be documented** loudly (done in several docs; still easy to miss).

---

## 4. `IFlow` as extension point

Custom flows may implement **arbitrary** `CanHandle` logic (prefix matching, multi-type, feature flags).

**Risk:** **Overlapping** `CanHandle` predicates can create **unexpected** fan-out. **No** framework guardrails—**pure** composition.

**Benefit:** **Maximum** flexibility without expanding core API surface.

---

## 5. `IncompatibleFlowException`

Thrown when `HandleAsync` receives a non-`TPulse` despite `CanHandle`—treated as **impossible** if invariants hold.

**Evaluation:** Good **fail loud** for **internal bugs**. **Not** user-recoverable; indicates **broken dispatch** or manual misuse of `IFlow` (calling `HandleAsync` directly with wrong type).

---

## 6. Comparison to alternatives (type system)

| Approach | PulseFlow today | Alternative pattern |
|----------|----------------|---------------------|
| Message type id | CLR `Type` | String key, `ushort` opcode |
| Handler lookup | Linear scan + `CanHandle` | Dictionary dispatch table |
| Compile-time safety | At handler boundary | Source generators, generic channels per type |

**Evaluation:** PulseFlow **optimizes for** small-to-medium **handler counts** and **developer ergonomics** over **nanosecond** dispatch.

---

## 7. Conclusion

The type model is **coherent** and **honest** about being **runtime-routed** and **CLR-type-centric**. **Deep limitation** is **polymorphic pulse hierarchies** without custom flows.

**If demand appears:** optional **base-type routing** mode (opt-in, breaking or parallel API) or **source-generated** dispatch tables.
