# 03 - Strangler Fig

## 1. Essence

The **Strangler Fig Pattern** is an incremental modernization strategy where a new system gradually takes over functionality from an existing system.

The legacy system continues running while new capabilities are introduced around it.

```text
                ┌──────────────┐
                │   Clients    │
                └──────┬───────┘
                       │
                       ▼
              ┌─────────────────┐
              │ Routing Boundary│
              └───────┬─────────┘
                      / \
                     /   \
                    ▼     ▼
             ┌─────────┐ ┌─────────┐
             │   New   │ │ Legacy  │
             │ System  │ │ System  │
             └─────────┘ └─────────┘
```

Over time:

```text
Legacy
████████████████████

New
░░░░░░░░
```

becomes:

```text
Legacy
████████

New
████████████████
```

until:

```text
Legacy
   X

New
████████████████████
```

The legacy system can then be retired.

---

## 2. The Problem

Large legacy systems are rarely safe to replace in one step.

A big-bang rewrite creates:

- large migration risk
- long delivery cycles
- unknown behavior
- difficult rollback
- data migration problems
- parallel development costs
- delayed business value

The Strangler Fig pattern allows:

> **Replace the system one capability at a time.**

---

## 3. Intent

Create a new architectural path around the legacy system and gradually move functionality to it.

```text
Legacy System
     ↓
Identify Capability
     ↓
Build New Implementation
     ↓
Route Traffic
     ↓
Validate
     ↓
Remove Legacy Capability
     ↓
Repeat
```

The migration is therefore a sequence of **small architectural transitions** rather than one large rewrite.

---

## 4. Core Principles

### 4.1 Keep the Legacy System Running

Do not remove the old system before the replacement is proven.

```text
Legacy + New
     ↓
Validation
     ↓
New
```

The old system is temporarily part of the production architecture.

---

### 4.2 Introduce a Routing Boundary

The central mechanism is traffic interception.

```text
Client
  │
  ▼
Router
 ┌┴──────────┐
 ▼           ▼
New        Legacy
```

The router decides which implementation handles the request.

Possible mechanisms:

- API Gateway
- Reverse Proxy
- Load Balancer
- Backend-for-Frontend
- Application routing
- Feature flags
- Service mesh

---

### 4.3 Migrate by Capability

Do not migrate arbitrary technical layers.

Prefer:

```text
Customer Management
Order Management
Payments
Reporting
Inventory
```

over:

```text
Controllers
Repositories
Services
Database Layer
```

A capability should ideally be migrated as a meaningful vertical slice.

---

### 4.4 Keep the Migration Incremental

A typical sequence:

```text
Phase 1
100% Legacy

Phase 2
90% Legacy
10% New

Phase 3
50% Legacy
50% New

Phase 4
10% Legacy
90% New

Phase 5
100% New
```

The exact percentages are optional.

The important concept is **controlled traffic movement**.

---

## 5. Architectural Model

```text
                    Clients
                       │
                       ▼
              ┌─────────────────┐
              │ Routing / Proxy │
              └───────┬─────────┘
                      │
              ┌───────┴────────┐
              │                 │
              ▼                 ▼
       ┌─────────────┐   ┌─────────────┐
       │ New System  │   │   Legacy    │
       │             │   │   System    │
       └──────┬──────┘   └──────┬──────┘
              │                 │
              ▼                 ▼
          New Data          Legacy Data
```

As migration progresses:

```text
              Routing
                 │
          ┌──────┴──────┐
          ▼             ▼
        New           Legacy
       ███████████     ███
```

Eventually:

```text
              Routing
                 │
                 ▼
                New
          ███████████████

Legacy → Removed
```

---

## 6. Step 1: Understand the Legacy System

Before replacing anything, discover:

- capabilities
- dependencies
- data flows
- APIs
- integrations
- business rules
- consumers
- runtime behavior
- operational constraints

Useful techniques:

```text
Code analysis
Dependency analysis
Database analysis
Runtime tracing
Production telemetry
Architecture diagrams
Git history
```

The first objective is:

> **Understand what the legacy system actually does.**

Documentation alone may not be sufficient.

---

## 7. Step 2: Identify a Migration Seam

A **seam** is a place where the old implementation can be replaced without changing the entire system.

Examples:

```text
API endpoint
Business capability
Module
Integration boundary
Message consumer
Database access layer
```

A good seam has:

- clear responsibility
- limited dependencies
- observable behavior
- manageable data ownership
- controllable traffic

---

## 8. Step 3: Build the New Path

Implement the replacement independently.

```text
Legacy
  │
  │ existing capability
  │
  └───────────────┐
                  │
New ──────────────┘
```

The new implementation should not unnecessarily reproduce the legacy architecture.

Use the migration as an opportunity to establish better boundaries.

---

## 9. Step 4: Route Traffic

Initially:

```text
Router
  │
  └──→ Legacy
```

After introducing the new implementation:

```text
Router
 ┌───────┐
 ▼       ▼
New    Legacy
```

The routing decision may be based on:

- endpoint
- capability
- tenant
- user
- feature flag
- request header
- geographic region
- percentage
- business condition

The routing mechanism becomes a critical part of the migration architecture.

---

## 10. Step 5: Validate the New Implementation

Before moving all traffic, compare the new path with the legacy behavior.

Validation can include:

```text
Functional tests
Contract tests
Integration tests
Performance tests
Shadow traffic
Output comparison
Business-rule validation
Data consistency checks
```

The question is not simply:

> "Does the new system work?"

It is:

> **"Does the new system correctly replace the behavior we depend on?"**

---

## 11. Shadow Execution

For high-risk migrations, both systems can process the same request.

```text
             Request
                │
                ▼
             Router
                │
          ┌─────┴─────┐
          ▼           ▼
       Legacy         New
          │           │
          └─────┬─────┘
                ▼
          Compare Results
```

The new result is observed but not necessarily returned to the user.

This allows behavioral validation before traffic is switched.

Important:

> Shadow execution must control side effects.

Do not accidentally charge a customer twice because both systems execute a payment.

---

## 12. Data Migration

Data is often harder than code.

Possible strategies:

```text
Read Legacy
     ↓
Transform
     ↓
Write New
```

or:

```text
Legacy
   │
   ├── Existing Writes
   │
   └── Synchronization
          ↓
       New Store
```

Techniques include:

- backfill
- dual writes
- CDC
- replication
- event propagation
- incremental migration
- read synchronization
- reconciliation

Every migration should define:

```text
Source of Truth
Consistency Model
Migration Progress
Validation Strategy
Cutover Strategy
Rollback Strategy
```

---

## 13. Source of Truth

During migration, ambiguity about ownership is dangerous.

For example:

```text
Legacy Customer DB
        +
New Customer DB
```

raises the question:

> Which one is authoritative?

Define explicitly:

```text
Before cutover:
Legacy = Source of Truth

After cutover:
New = Source of Truth
```

The transition between these states must be controlled and observable.

---

## 14. Compatibility Layer

Legacy systems often expose old contracts.

The new system may use a different model.

Introduce an adapter:

```text
Legacy Contract
      │
      ▼
Compatibility Layer
      │
      ▼
New Domain Model
```

The compatibility layer protects the new architecture from legacy concepts.

This is often implemented as:

- Adapter
- Facade
- Anti-Corruption Layer
- Translation layer
- API adapter
- Schema adapter

---

## 15. Traffic Cutover

A controlled migration might look like:

```text
Step 1
Legacy 100%
New      0%

Step 2
Legacy 90%
New     10%

Step 3
Legacy 50%
New     50%

Step 4
Legacy 10%
New     90%

Step 5
Legacy 0%
New    100%
```

After each step, observe:

- error rate
- latency
- throughput
- business metrics
- data consistency
- infrastructure health

Do not increase traffic automatically without validation.

---

## 16. Rollback

One of the major advantages of incremental migration is controlled rollback.

```text
New
 │
 │ Problem
 ▼
Router
 │
 ▼
Legacy
```

But rollback is only simple when:

- contracts remain compatible
- data changes are reversible
- new writes have not created incompatible state
- the legacy system can still handle the traffic

Therefore:

> **Rollback must be designed before cutover.**

---

## 17. Removing the Legacy Path

Once the new implementation reaches full ownership:

```text
New = 100%
Legacy = 0%
```

Do not immediately delete the legacy code.

First verify:

- no remaining consumers
- no remaining data dependencies
- no remaining integrations
- no rollback requirement
- no operational dependency

Then:

```text
Disable
   ↓
Observe
   ↓
Remove Traffic Route
   ↓
Remove Code
   ↓
Remove Data
   ↓
Remove Infrastructure
```

Legacy removal is part of the migration, not an optional cleanup task.

---

## 18. Architectural Invariants

The migration should enforce:

1. Legacy and new implementations can coexist.
2. Traffic routing is explicit.
3. Every migrated capability has a defined owner.
4. New code does not introduce new legacy dependencies unnecessarily.
5. Data ownership is explicit.
6. Source of truth is defined.
7. Compatibility layers have a removal plan.
8. Migration progress is observable.
9. Cutover is reversible where technically possible.
10. Legacy functionality is removed after successful migration.
11. Shadow execution cannot create unintended side effects.
12. New dependencies do not recreate the old architecture.

---

## 19. Common Failure Modes

### Big-Bang Rewrite

```text
Legacy
   ↓
Rewrite Everything
   ↓
New
```

This eliminates the incremental safety mechanism.

---

### Strangler Shell

A new frontend or gateway is created, but almost all business logic still lives in the legacy system.

The architecture looks new while the dependency structure remains old.

---

### Permanent Compatibility Layer

```text
New
 ↓
Adapter
 ↓
Legacy
```

continues indefinitely.

The migration becomes a permanent architectural dependency.

---

### Dual-Write Inconsistency

```text
Legacy DB
   ↑
   │
Application
   │
   ↓
New DB
```

One write succeeds while the other fails.

Without a reconciliation strategy, the systems diverge.

---

### Migrating Technical Layers Instead of Capabilities

Moving repositories or controllers without moving business ownership produces little architectural improvement.

---

### Shadow Side Effects

Both systems perform real mutations:

```text
Legacy → Charge Customer
New    → Charge Customer
```

This can create duplicate operations.

---

### No Rollback

Traffic is moved to the new implementation without a safe route back.

---

## 20. Testing Strategy

Use:

```text
Characterization Tests
        ↓
Contract Tests
        ↓
Integration Tests
        ↓
Migration Tests
        ↓
Shadow Tests
        ↓
Performance Tests
        ↓
Production Validation
```

Particularly important:

### Characterization Tests

Capture current legacy behavior before changing it.

### Contract Tests

Verify that old consumers continue to work.

### Comparison Tests

Compare old and new outputs.

### Migration Tests

Verify data movement and reconciliation.

### Routing Tests

Verify that traffic reaches the intended implementation.

---

## 21. Observability

Track migration-specific metrics:

```text
Legacy Requests
New Requests
Migration Percentage
Error Rate by Path
Latency by Path
Data Mismatch Count
Rollback Count
Migration Progress
```

Use distributed tracing to distinguish:

```text
trace → legacy
trace → new
trace → compatibility layer
```

A migration without observability is difficult to control.

---

## 22. .NET Implementation

Useful technologies and mechanisms:

- ASP.NET Core routing
- YARP
- API Gateway
- Feature Flags
- Dependency Injection
- `HttpClientFactory`
- EF Core
- Background Services
- OpenTelemetry
- Kafka / RabbitMQ
- CDC
- .NET Aspire
- Docker

A simple routing abstraction might look like:

```csharp
public interface IOrderPath
{
    Task<OrderResult> ExecuteAsync(OrderRequest request);
}
```

with implementations:

```text
LegacyOrderPath
NewOrderPath
```

A routing component determines which implementation receives the request.

The important architectural idea is the **seam**, not the specific framework.

---

## 23. Lab Experiment

Start with a deliberately legacy application:

```text
LegacyShop
├── Orders
├── Customers
├── Payments
└── Inventory
```

Choose `Orders` as the migration target.

### Phase 1

Characterize the existing behavior.

### Phase 2

Create the new Orders implementation.

### Phase 3

Introduce a routing boundary.

### Phase 4

Run shadow execution.

### Phase 5

Compare legacy and new results.

### Phase 6

Route a small percentage of traffic to the new path.

### Phase 7

Increase traffic gradually.

### Phase 8

Introduce a failure in the new system.

### Phase 9

Rollback traffic to the legacy path.

### Phase 10

Complete the migration.

### Phase 11

Remove the legacy Orders implementation.

### Phase 12

Repeat with another capability.

---

## 24. Success Criteria

The migration is successful when:

```text
New capability
      +
Controlled traffic
      +
Independent ownership
      +
Validated behavior
      +
Data consistency
      +
Observable migration
      +
Rollback capability
      +
Legacy removal
```

The final state should not depend on the legacy implementation.

---

## 25. Key Takeaways

- Strangler Fig is an **incremental replacement strategy**.
- The legacy system remains operational during migration.
- A routing boundary is the central architectural mechanism.
- Migrate business capabilities, not merely technical layers.
- Characterize existing behavior before replacing it.
- Shadow execution can reduce migration uncertainty.
- Data migration is often harder than code migration.
- Source-of-truth ownership must be explicit.
- Compatibility layers protect the new architecture from legacy concepts.
- Every traffic cutover should have validation and rollback criteria.
- Compatibility layers should be temporary.
- Legacy removal is part of the architecture evolution.
- The migration itself must be observable.

> **The Strangler Fig pattern does not replace the old system overnight.**
>
> **It gradually makes the old system smaller until there is nothing left to strangle.**
