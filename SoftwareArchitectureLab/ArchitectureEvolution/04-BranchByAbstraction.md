# 04 - Branch by Abstraction

## 1. Essence

**Branch by Abstraction** is an evolutionary architecture technique for replacing an implementation behind a stable abstraction.

Instead of creating a long-lived code branch:

```text
Old Implementation
        ↓
Long-lived Git Branch
        ↓
New Implementation
        ↓
Merge Everything
```

introduce an abstraction in the existing codebase:

```text
Consumers
    │
    ▼
Abstraction
   / \
  ▼   ▼
Old  New
```

Consumers can continue using the same contract while the implementation changes underneath them.

> **Change the implementation behind a stable boundary.**

---

## 2. The Problem

Large refactorings often affect many consumers.

For example:

```text
OrderService
CustomerService
Reporting
BackgroundJobs
API
```

all depend on:

```text
IPricingService
```

Replacing the pricing implementation directly could require changing all consumers at once.

That creates:

- large change sets
- difficult merges
- long-lived branches
- high regression risk
- difficult rollback

Branch by Abstraction creates a controlled transition.

---

## 3. Intent

Introduce an abstraction around the existing implementation:

```text
Before:

Consumers
    │
    ▼
LegacyPricingService
```

Then:

```text
After:

Consumers
    │
    ▼
IPricingService
    │
    ▼
LegacyPricingService
```

Then introduce the new implementation:

```text
Consumers
    │
    ▼
IPricingService
   / \
  ▼   ▼
Old  New
```

Finally:

```text
Consumers
    │
    ▼
IPricingService
    │
    ▼
NewPricingService
```

The abstraction becomes the migration seam.

---

## 4. Core Principles

### 4.1 Stable Contract

Consumers should depend on a stable abstraction:

```csharp
public interface IPricingService
{
    Task<Price> CalculateAsync(Order order);
}
```

The implementation can change without changing the consumer.

---

### 4.2 One Abstraction, Multiple Implementations

During migration:

```text
IPricingService
      │
 ┌────┴────┐
 ▼         ▼
Legacy    New
```

Both implementations satisfy the same contract.

---

### 4.3 Small Migration Steps

Do not change everything simultaneously.

```text
Introduce abstraction
        ↓
Wrap old implementation
        ↓
Build new implementation
        ↓
Validate new implementation
        ↓
Switch consumers
        ↓
Remove old implementation
```

---

### 4.4 The Abstraction Must Represent Behavior

Avoid creating an abstraction simply because:

```text
"Every class should have an interface."
```

The abstraction should represent a meaningful architectural seam.

Good:

```text
IPaymentGateway
IPricingService
INotificationSender
IOrderRepository
```

Potentially weak:

```text
IUserService1
IHelper
IManager
```

The abstraction should have a clear responsibility.

---

## 5. Architectural Model

```text
                 Consumers
                     │
                     ▼
              ┌─────────────┐
              │ Abstraction │
              └──────┬──────┘
                     │
              ┌──────┴──────┐
              ▼             ▼
       ┌────────────┐ ┌────────────┐
       │    Old     │ │    New     │
       │Implementation│ │Implementation│
       └────────────┘ └────────────┘
```

A selector controls which implementation is active:

```text
             Abstraction
                  │
                  ▼
          Implementation
             Selector
             /       \
            ▼         ▼
          Old         New
```

The selector may use:

- configuration
- feature flags
- dependency injection
- runtime routing
- tenant configuration
- percentage rollout

---

## 6. Migration Sequence

A typical migration:

```text
1. Identify Change
        ↓
2. Define Abstraction
        ↓
3. Wrap Existing Implementation
        ↓
4. Move Consumers to Abstraction
        ↓
5. Verify Behavior
        ↓
6. Build New Implementation
        ↓
7. Run Old and New
        ↓
8. Validate New
        ↓
9. Switch Implementation
        ↓
10. Remove Old Implementation
        ↓
11. Remove Temporary Abstraction
          if no longer needed
```

The migration is complete only when the old path is removed.

---

## 7. Step 1: Identify the Seam

Look for a component that:

- has multiple consumers
- has clear behavior
- needs implementation replacement
- has manageable inputs and outputs
- can be isolated behind a contract

Examples:

```text
Database Access
Payment Provider
Messaging Provider
Search Engine
Caching Layer
External API
Legacy Algorithm
Persistence Technology
```

---

## 8. Step 2: Introduce the Abstraction

Suppose the existing system contains:

```csharp
public class LegacyPaymentGateway
{
    public PaymentResult Pay(PaymentRequest request)
    {
        // Legacy implementation
    }
}
```

Introduce:

```csharp
public interface IPaymentGateway
{
    PaymentResult Pay(PaymentRequest request);
}
```

Then adapt the existing implementation:

```csharp
public class LegacyPaymentGatewayAdapter
    : IPaymentGateway
{
    private readonly LegacyPaymentGateway _legacy;

    public PaymentResult Pay(PaymentRequest request)
        => _legacy.Pay(request);
}
```

Now consumers depend on:

```text
IPaymentGateway
```

instead of:

```text
LegacyPaymentGateway
```

---

## 9. Step 3: Move Consumers

Before:

```text
OrderService
     │
     ▼
LegacyPaymentGateway
```

After:

```text
OrderService
     │
     ▼
IPaymentGateway
     │
     ▼
LegacyPaymentGatewayAdapter
```

At this point, behavior should remain unchanged.

This is important.

> **First change the structure, then change the behavior.**

---

## 10. Step 4: Implement the New Path

Create:

```text
LegacyPaymentGatewayAdapter
NewPaymentGateway
```

Both implement:

```text
IPaymentGateway
```

Architecture:

```text
                 IPaymentGateway
                       │
                ┌──────┴──────┐
                ▼             ▼
        Legacy Adapter    New Gateway
```

The consumers do not need to know which implementation they are using.

---

## 11. Step 5: Select the Implementation

The simplest approach is configuration:

```json
{
  "PaymentGateway": "New"
}
```

Then:

```csharp
services.AddScoped<IPaymentGateway>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();

    return configuration["PaymentGateway"] == "New"
        ? sp.GetRequiredService<NewPaymentGateway>()
        : sp.GetRequiredService<LegacyPaymentGatewayAdapter>();
});
```

For production systems, a feature-flag system may provide safer control.

---

## 12. Step 6: Validate the New Implementation

Before switching completely, compare behavior.

Validation may include:

```text
Functional Tests
Contract Tests
Integration Tests
Performance Tests
Output Comparison
Shadow Execution
Production Metrics
```

For deterministic operations:

```text
Old Result == New Result
```

For operations where results may legitimately differ:

```text
Business Invariants
        +
Expected Behavioral Differences
```

should be validated instead.

---

## 13. Parallel Execution

For high-risk changes, both implementations can execute:

```text
Request
   │
   ▼
Abstraction
   │
   ├──────────────→ Old
   │
   └──────────────→ New
                       │
                       ▼
                  Compare
```

But only one result should normally control the business operation.

Be careful with side effects.

For example:

```text
❌ Old → Charge Credit Card
❌ New → Charge Credit Card
```

Parallel execution of non-idempotent operations can cause real-world duplication.

Prefer:

```text
Old → Real Operation
New → Shadow / Simulation
```

where possible.

---

## 14. Consumer-by-Consumer Migration

Sometimes the new implementation cannot replace the old implementation for everyone immediately.

For example:

```text
Consumer A → New
Consumer B → Old
Consumer C → New
Consumer D → Old
```

The abstraction allows this transition.

Eventually:

```text
A → New
B → New
C → New
D → New
```

Then:

```text
Remove Old
```

This is especially useful when different teams or components migrate at different speeds.

---

## 15. Database Example

Branch by Abstraction is not limited to classes.

It can support persistence migration.

Before:

```text
Application
     │
     ▼
SQL Server
```

Introduce:

```text
Application
     │
     ▼
IOrderRepository
     │
     ▼
SQL Server
```

Then:

```text
IOrderRepository
      │
 ┌────┴─────┐
 ▼          ▼
SQL Server  PostgreSQL
```

Consumers remain unchanged while persistence evolves.

The data migration itself requires additional techniques such as:

- backfill
- synchronization
- dual writes
- CDC
- reconciliation
- cutover

The abstraction solves the **code dependency**, not the entire data migration problem.

---

## 16. Technology Migration Example

Branch by Abstraction can be used for:

```text
Entity Framework
      ↓
New Data Access Technology
```

or:

```text
REST Client
      ↓
gRPC Client
```

or:

```text
RabbitMQ
      ↓
Kafka
```

or:

```text
Cloud Provider A
      ↓
Cloud Provider B
```

The abstraction isolates consumers from the implementation technology.

---

## 17. Architectural Invariants

The migration should enforce:

1. Consumers depend on the abstraction.
2. Consumers do not directly reference the legacy implementation.
3. Old and new implementations satisfy the same contract.
4. The abstraction represents meaningful behavior.
5. Implementation selection is explicit.
6. New implementation can be tested independently.
7. Migration can be rolled back while both implementations exist.
8. Parallel execution does not create unintended side effects.
9. Temporary migration code has a removal plan.
10. The old implementation is removed after successful migration.

---

## 18. Common Failure Modes

### Abstraction Leakage

The abstraction exposes legacy-specific concepts:

```csharp
LegacyPaymentResult CalculateLegacyFee(...)
```

The new implementation is now forced to understand legacy details.

---

### Fake Abstraction

```text
IService
    ↓
LegacyService
```

but the interface simply exposes the entire legacy implementation.

No meaningful boundary was created.

---

### Permanent Dual Implementation

```text
Old + New
```

remain indefinitely.

This increases maintenance and testing cost.

---

### Feature-Flag Explosion

```text
if Old...
if New...
if Old...
if New...
```

throughout the codebase.

The implementation-selection logic should remain localized.

---

### Abstraction with No Removal Plan

The team creates an interface for migration but never defines when it can disappear.

Temporary architecture becomes permanent complexity.

---

### Shared State

Old and new implementations modify the same state differently.

This can create subtle consistency problems.

---

## 19. Testing Strategy

Use:

```text
Unit Tests
    ↓
Contract Tests
    ↓
Characterization Tests
    ↓
Implementation Tests
    ↓
Comparison Tests
    ↓
Integration Tests
```

Important tests:

### Contract Tests

Both implementations must satisfy the same expected contract.

### Characterization Tests

Capture the behavior of the legacy implementation.

### Comparison Tests

Verify old and new behavior where equivalence is expected.

### Architecture Tests

Ensure consumers cannot bypass the abstraction.

---

## 20. Failure Experiment

Create two implementations:

```text
LegacyPricing
NewPricing
```

Introduce an intentional difference:

```text
Legacy → 100.00
New    → 101.00
```

Run the comparison tests.

Expected result:

```text
❌ Behavioral mismatch
```

Then correct the implementation.

This demonstrates that the abstraction alone does not guarantee behavioral compatibility.

---

## 21. .NET Implementation

Useful .NET mechanisms:

- Interfaces
- Dependency Injection
- `IServiceCollection`
- Configuration
- Options pattern
- Feature flags
- Decorators
- Adapters
- Factory patterns
- OpenTelemetry
- Architecture testing
- Integration testing

A clean structure:

```text
src/
├── Application/
│   └── Pricing/
│       └── IPricingService.cs
│
├── Infrastructure/
│   └── Pricing/
│       ├── LegacyPricingService.cs
│       └── NewPricingService.cs
│
└── Tests/
    ├── PricingContractTests/
    └── ArchitectureTests/
```

The abstraction belongs to the architectural boundary.

The implementations belong behind it.

---

## 22. Lab Experiment

Create a legacy application containing:

```text
OrderService
     ↓
LegacyPricingEngine
```

### Phase 1

Characterize the existing pricing behavior.

### Phase 2

Introduce:

```text
IPricingEngine
```

### Phase 3

Wrap the legacy implementation.

### Phase 4

Move `OrderService` to the abstraction.

### Phase 5

Create `NewPricingEngine`.

### Phase 6

Run contract tests against both implementations.

### Phase 7

Add implementation switching.

### Phase 8

Run both implementations in shadow mode.

### Phase 9

Compare results.

### Phase 10

Switch traffic to the new implementation.

### Phase 11

Monitor.

### Phase 12

Remove the legacy implementation and migration code.

---

## 23. Success Criteria

The migration is successful when:

```text
Stable Contract
      +
Old Implementation
      ↓
New Implementation
      +
Validated Behavior
      +
Controlled Switching
      +
Rollback Capability
      +
Old Implementation Removed
```

The consumer code should not need to know which implementation is active.

---

## 24. Key Takeaways

- Branch by Abstraction replaces implementations behind a stable boundary.
- It reduces the need for long-lived feature branches.
- The abstraction is the migration seam.
- Consumers migrate before the implementation does.
- Old and new implementations can coexist temporarily.
- Both implementations should satisfy the same meaningful contract.
- Feature flags or configuration can control the transition.
- Shadow execution can validate behavior before cutover.
- Side effects require special care during parallel execution.
- The technique can be applied to code, databases, infrastructure, APIs, and technologies.
- The abstraction should not leak legacy concepts.
- Temporary migration mechanisms should have explicit removal plans.
- The goal is not to keep two implementations forever.

> **Branch by Abstraction lets the system change its implementation without forcing the entire system to change at once.**
