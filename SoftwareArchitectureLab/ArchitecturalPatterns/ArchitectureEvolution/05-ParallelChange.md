# 05 - Parallel Change

## 1. Essence

**Parallel Change** is an evolutionary architecture technique for changing a contract, schema, or interface without requiring all consumers to change at the same time.

The migration happens in three stages:

```text
Expand
   ↓
Migrate
   ↓
Contract
```

Instead of:

```text
Old Contract
     ↓
Breaking Change
     ↓
New Contract
```

use:

```text
Old + New
    ↓
Consumers migrate
    ↓
New only
```

> **Make the system support both versions before removing the old one.**

---

## 2. The Problem

A breaking change becomes dangerous when multiple components are deployed independently.

For example:

```text
Producer ─────→ Consumer A
         ├────→ Consumer B
         └────→ Consumer C
```

Changing the contract immediately can break consumers that have not been upgraded yet.

Examples:

```text
API field renamed
Database column renamed
Event schema changed
Message format changed
Service contract changed
```

The core problem is:

> **Old and new components may coexist during deployment.**

Parallel Change makes that coexistence safe.

---

## 3. Intent

Introduce the new contract without immediately removing the old contract.

```text
Before:

Producer → Old Contract → Consumers
```

Then:

```text
Expand:

Producer → Old + New → Consumers
```

Then:

```text
Migrate:

Producer → Old + New
                ↓
         Consumers migrate
```

Finally:

```text
Contract:

Producer → New → Consumers
```

The migration is therefore backward-compatible during the transition.

---

## 4. Core Principles

### 4.1 Expand Before Contract

Never remove the old contract before the new one is available.

```text
❌ Remove → Add

✅ Add → Migrate → Remove
```

---

### 4.2 Compatibility During Deployment

At every deployment boundary, assume:

```text
Old Version
     +
New Version
```

may coexist.

The system must remain operational in that state.

---

### 4.3 Additive Changes Are Safer

Prefer:

```text
Add New Field
Add New Endpoint
Add New Event Version
Add New Column
```

before:

```text
Remove Old Field
Remove Old Endpoint
Remove Old Event
Remove Old Column
```

---

### 4.4 Migrate Consumers Gradually

Consumers should move independently:

```text
Consumer A → New
Consumer B → Old
Consumer C → New
Consumer D → Old
```

Eventually:

```text
A → New
B → New
C → New
D → New
```

Then the old contract can be removed.

---

## 5. Architectural Model

```text
             Producer
                │
        ┌───────┴────────┐
        ▼                ▼
   Old Contract     New Contract
        │                │
        ▼                ▼
 Old Consumers       New Consumers
```

During migration:

```text
             Producer
                │
                ▼
        ┌───────────────┐
        │ Compatibility │
        │     Layer     │
        └───────┬───────┘
                │
          Old + New
```

The exact implementation depends on the type of change.

---

## 6. The Three Phases

### Phase 1: Expand

Introduce the new representation while keeping the old one.

```text
Old ✓
New ✓
```

---

### Phase 2: Migrate

Move consumers, readers, or writers to the new representation.

```text
Old ✓
New ✓
```

but usage gradually becomes:

```text
Old ↓
New ↑
```

---

### Phase 3: Contract

After all consumers have migrated:

```text
Old ✗
New ✓
```

Remove:

- old fields
- old endpoints
- old events
- old database columns
- compatibility code
- migration flags

---

## 7. API Evolution

Suppose the old API returns:

```json
{
  "name": "John Doe"
}
```

The new contract requires:

```json
{
  "firstName": "John",
  "lastName": "Doe"
}
```

Do not immediately remove `name`.

### Expand

Support:

```json
{
  "name": "John Doe",
  "firstName": "John",
  "lastName": "Doe"
}
```

### Migrate

Consumers start using:

```text
firstName
lastName
```

### Contract

After all consumers migrate:

```json
{
  "firstName": "John",
  "lastName": "Doe"
}
```

Then remove `name`.

---

## 8. Database Schema Evolution

A classic example is renaming a column.

Current:

```text
customer.full_name
```

Target:

```text
customer.first_name
customer.last_name
```

Do not simply rename the column if old application versions may still run.

### Expand

```text
full_name
first_name
last_name
```

### Migrate

Application understands both representations.

New writes populate the new columns.

Existing data is backfilled.

### Contract

After all consumers use the new columns:

```text
full_name
```

can be removed.

---

## 9. Database Migration Sequence

A robust migration may look like:

```text
Add New Columns
      ↓
Deploy Compatible Code
      ↓
Backfill Existing Data
      ↓
Synchronize New Writes
      ↓
Migrate Reads
      ↓
Validate
      ↓
Stop Old Writes
      ↓
Validate Again
      ↓
Remove Old Columns
```

This avoids requiring one deployment to perform an irreversible transformation.

---

## 10. Dual Writes

During migration:

```text
Application
    │
    ├────→ Old Storage
    │
    └────→ New Storage
```

This can support gradual migration.

But dual writes introduce consistency risk:

```text
Old Write ✓
New Write ✗
```

Therefore dual writes require:

- error handling
- reconciliation
- idempotency
- monitoring
- consistency checks

For important data, transactional outbox or CDC-based approaches may be safer depending on the architecture.

---

## 11. Event Contract Evolution

Suppose an event originally contains:

```json
{
  "orderId": "123",
  "customerName": "John"
}
```

New consumers need:

```json
{
  "orderId": "123",
  "customerId": "456"
}
```

Do not immediately remove `customerName`.

Expand:

```json
{
  "orderId": "123",
  "customerName": "John",
  "customerId": "456"
}
```

Consumers migrate to:

```text
customerId
```

Then contract:

```json
{
  "orderId": "123",
  "customerId": "456"
}
```

For event-driven systems, consider:

- schema compatibility
- versioning
- replay
- consumers at different versions
- schema registry
- upcasting

---

## 12. API Versioning

Sometimes additive evolution is insufficient.

For example:

```text
/api/v1/orders
/api/v2/orders
```

can coexist temporarily.

```text
Consumers
 ├── v1
 └── v2
```

Migration:

```text
v1 Consumers
     ↓
Migrate
     ↓
v2 Consumers
     ↓
Remove v1
```

The old version should have an explicit deprecation and removal strategy.

---

## 13. Message Consumers

A message producer may need to support old and new consumers simultaneously.

```text
Producer
   │
   ▼
Message
   │
 ┌─┴───────────┐
 ▼             ▼
Old Consumer  New Consumer
```

The producer should avoid introducing a breaking change until the consumer population has migrated.

Consumer compatibility is therefore part of the producer's architecture.

---

## 14. Compatibility Strategies

Depending on the change, use:

```text
Additive Contract
Backward Compatibility
Forward Compatibility
Versioned Contract
Adapter
Translator
Upcaster
Dual Read
Dual Write
CDC
Feature Flag
```

The simplest strategy should be preferred.

Do not introduce complex compatibility machinery when a simple additive change is sufficient.

---

## 15. Data Consistency

Parallel Change often creates temporary duplication:

```text
Old Field
    +
New Field
```

or:

```text
Old Store
    +
New Store
```

The system must define:

```text
Source of Truth
Synchronization Strategy
Consistency Expectations
Validation Method
Cutover Condition
Rollback Strategy
```

Temporary duplication without explicit ownership is a major migration risk.

---

## 16. Rollback

One advantage of Expand/Migrate/Contract is that rollback remains possible during the transition.

Before contraction:

```text
Old ✓
New ✓
```

If the new implementation fails:

```text
Rollback
    ↓
Old
```

After contraction:

```text
Old ✗
New ✓
```

rollback becomes much harder.

Therefore:

> **Do not contract until confidence is high and rollback requirements are understood.**

---

## 17. Architectural Invariants

The migration should enforce:

1. New functionality is introduced before old functionality is removed.
2. Old and new representations can coexist.
3. Existing consumers remain compatible during migration.
4. New consumers can use the new representation.
5. Data synchronization is explicit.
6. Source of truth is defined.
7. Dual writes are monitored when used.
8. Compatibility code has an explicit removal plan.
9. Old contracts are not removed prematurely.
10. Contraction occurs only after migration validation.

---

## 18. Common Failure Modes

### Contract-Then-Break

```text
Remove Old
    ↓
Deploy New
```

Older consumers fail.

---

### Dual-Write Drift

```text
Old DB ✓
New DB ✗
```

The representations diverge.

---

### Permanent Compatibility

```text
Old + New
```

remain forever.

Temporary migration complexity becomes permanent architecture.

---

### Unknown Consumers

The team removes an old field without knowing that another application still depends on it.

Always identify consumers before contraction.

---

### Premature Cleanup

Developers remove the old schema or API immediately after the new version is deployed.

Deployment does not equal migration completion.

---

### Version Explosion

```text
v1
v2
v3
v4
v5
```

without a clear retirement strategy.

Compatibility should be temporary and intentional.

---

## 19. Testing Strategy

Use:

```text
Contract Tests
     ↓
Integration Tests
     ↓
Migration Tests
     ↓
Compatibility Tests
     ↓
Data Consistency Tests
     ↓
Production Validation
```

Important tests include:

### Backward Compatibility

Old consumers work with the expanded contract.

### Forward Compatibility

New consumers work with the new contract.

### Migration Tests

Old and new representations remain consistent.

### Contract Tests

Producer and consumers agree on the contract.

### Reconciliation Tests

Old and new data produce equivalent results where equivalence is expected.

---

## 20. Failure Experiment

Create a simple schema migration.

Initial:

```text
Customer
└── Name
```

Target:

```text
Customer
├── FirstName
└── LastName
```

Implement the migration incorrectly:

```text
Remove Name
    ↓
Add FirstName + LastName
```

Run the old application.

Expected:

```text
❌ Failure
```

Then implement:

```text
Expand
   ↓
Migrate
   ↓
Contract
```

Run old and new application versions simultaneously.

Expected:

```text
Old ✓
New ✓
```

This demonstrates why Parallel Change is important in independently deployed systems.

---

## 21. .NET Implementation

Useful technologies and mechanisms:

- EF Core migrations
- ASP.NET Core API versioning
- DTOs
- JSON serialization
- feature flags
- dependency injection
- Kafka / RabbitMQ
- transactional outbox
- CDC
- OpenTelemetry
- contract testing
- integration testing

Example DTO evolution:

```csharp
public sealed record CustomerResponse(
    string? Name,
    string? FirstName,
    string? LastName);
```

During migration, both representations may be supported.

Later:

```csharp
public sealed record CustomerResponse(
    string FirstName,
    string LastName);
```

The old field is removed only after consumers have migrated.

---

## 22. Lab Experiment

Create a small system:

```text
Order Service
     ↓
Order API
     ↓
Order Consumer
```

Start with:

```json
{
  "orderId": "123",
  "customerName": "John Doe"
}
```

### Phase 1: Expand

Add:

```json
{
  "orderId": "123",
  "customerName": "John Doe",
  "customerId": "456"
}
```

### Phase 2: Migrate

Change the consumer to use:

```text
customerId
```

### Phase 3: Validate

Run old and new consumer versions together.

### Phase 4: Contract

Remove:

```text
customerName
```

### Phase 5: Failure Experiment

Deploy the old consumer against the contracted event.

Observe the failure.

This makes the compatibility requirement concrete.

---

## 23. Success Criteria

The migration is successful when:

```text
Expanded Contract
       ↓
Old + New Compatible
       ↓
Consumers Migrated
       ↓
Data Validated
       ↓
Old Usage = 0
       ↓
Old Contract Removed
```

The important metric is not simply that the new contract exists.

It is that **all relevant consumers have safely migrated**.

---

## 24. Key Takeaways

- Parallel Change enables safe evolution of contracts and data structures.
- The fundamental sequence is **Expand → Migrate → Contract**.
- Old and new representations must coexist temporarily.
- Additive changes are generally safer than breaking changes.
- Database migrations often require the same strategy.
- APIs and events can evolve using compatible contracts and temporary versions.
- Dual writes can help but introduce consistency risks.
- Source-of-truth ownership must be explicit.
- Unknown consumers are a major migration risk.
- Compatibility code should have a removal plan.
- Contracting too early destroys rollback options.
- The migration is complete only when the old representation is no longer needed.

> **Parallel Change makes breaking changes safe by turning one dangerous transition into several compatible steps.**
