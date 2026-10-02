# 06 - Expand and Contract

## 1. Essence

**Expand and Contract** is a migration pattern for evolving schemas, APIs, and contracts without requiring all components to change simultaneously.

The migration happens in three stages:

```text
Expand
   ↓
Migrate
   ↓
Contract
```

During the transition, the system supports both the old and new representation.

```text
Old + New
    ↓
Consumers migrate
    ↓
New only
```

> **Add before removing. Migrate before contracting.**

---

## 2. The Problem

In a distributed or independently deployed system, different versions of an application may run simultaneously.

For example:

```text
        Deployment
            │
      ┌─────┴─────┐
      ▼           ▼
   Version 1    Version 2
```

If Version 2 immediately removes something required by Version 1:

```text
Version 1 → Old Contract
                 X
              Removed
```

the deployment becomes unsafe.

Expand and Contract creates a compatibility window.

---

## 3. Intent

Change the system without creating a breaking transition.

```text
Before:

Old ───────────────→ Old


Expand:

Old ───────────────→ Old
New ───────────────→ New


Migrate:

Old ───────────────→ New
New ───────────────→ New


Contract:

New ───────────────→ New
```

The old representation is removed only after it is no longer needed.

---

## 4. Core Principles

### 4.1 Expand First

Introduce the new structure without removing the old one.

```text
Old ✓
New ✓
```

---

### 4.2 Preserve Compatibility

During migration:

```text
Old Consumer
      +
New Consumer
```

must be able to coexist.

This is particularly important with:

- rolling deployments
- blue/green deployments
- canary deployments
- independently deployed services

---

### 4.3 Migrate Explicitly

The existence of the new structure does not mean migration is complete.

Track:

```text
Old Usage
New Usage
Migration Progress
```

---

### 4.4 Contract Last

Remove the old representation only when:

```text
Old Consumers = 0
```

or when all remaining consumers are known to support the new contract.

---

## 5. Architectural Model

```text
             ┌─────────────┐
             │   Producer  │
             └──────┬──────┘
                    │
              Old + New
                    │
          ┌─────────┴─────────┐
          ▼                   ▼
    Old Consumers        New Consumers
```

After migration:

```text
             ┌─────────────┐
             │   Producer  │
             └──────┬──────┘
                    │
                   New
                    │
                    ▼
              New Consumers
```

---

## 6. The Three Phases

### Phase 1: Expand

Add the new representation.

```text
Old ✓
New ✓
```

Nothing dependent on the old representation is broken.

---

### Phase 2: Migrate

Move consumers and data to the new representation.

```text
Old Usage ↓
New Usage ↑
```

During this phase:

```text
Old + New
```

continue to coexist.

---

### Phase 3: Contract

Remove the old representation.

```text
Old ✗
New ✓
```

Remove:

- old fields
- old columns
- old endpoints
- old events
- compatibility code
- migration flags

---

## 7. Database Schema Migration

This is the classic use case.

Suppose the database contains:

```text
Customer
└── FullName
```

The target schema is:

```text
Customer
├── FirstName
└── LastName
```

Do not immediately rename or remove `FullName`.

---

## 8. Expand

Add:

```sql
FirstName
LastName
```

The database temporarily contains:

```text
Customer
├── FullName
├── FirstName
└── LastName
```

The application can still use `FullName`.

---

## 9. Migrate

Backfill:

```text
FullName
   ↓
FirstName + LastName
```

For example:

```text
"John Doe"
      ↓
FirstName = "John"
LastName  = "Doe"
```

New application versions should begin writing the new representation.

During the transition, compatibility logic may need to maintain both representations.

---

## 10. Validate

Before contraction, verify:

```text
Old Data
   ↔
New Data
```

Examples:

```text
Row count
Nullability
Business invariants
Checksums
Aggregates
Sample comparisons
Application behavior
```

The validation strategy depends on the data.

---

## 11. Contract

Once the old representation is no longer required:

```sql
DROP COLUMN FullName;
```

The final schema becomes:

```text
Customer
├── FirstName
└── LastName
```

The migration is complete.

---

## 12. Zero-Downtime Deployment

Consider two application versions:

```text
Version 1
Version 2
```

During deployment:

```text
┌──────────────┐
│   Database   │
│              │
│ Old ✓        │
│ New ✓        │
└──────────────┘
```

Both versions can operate safely.

After all instances run Version 2:

```text
Version 1 = 0
```

the old database structure can be removed.

This is the foundation of safe rolling schema migrations.

---

## 13. API Evolution

Suppose:

```http
GET /customers/{id}
```

originally returns:

```json
{
  "name": "John Doe"
}
```

The new representation requires:

```json
{
  "firstName": "John",
  "lastName": "Doe"
}
```

### Expand

Temporarily support both:

```json
{
  "name": "John Doe",
  "firstName": "John",
  "lastName": "Doe"
}
```

### Migrate

Consumers switch to:

```text
firstName
lastName
```

### Contract

Remove:

```text
name
```

Only after consumers have migrated.

---

## 14. Event Schema Evolution

The same pattern applies to events.

Old:

```json
{
  "orderId": "123",
  "customerName": "John Doe"
}
```

Expanded:

```json
{
  "orderId": "123",
  "customerName": "John Doe",
  "customerId": "456"
}
```

Consumers migrate to:

```text
customerId
```

Then the old field can be removed.

For event-driven systems, also consider:

- schema compatibility
- replay
- old messages
- consumer versions
- schema registry
- upcasting

---

## 15. Renaming Safely

A simple rename:

```text
OldName → NewName
```

is actually a breaking change if consumers depend on `OldName`.

Use:

```text
Add NewName
      ↓
Populate NewName
      ↓
Migrate Consumers
      ↓
Stop Using OldName
      ↓
Remove OldName
```

This applies to:

- database columns
- API fields
- event fields
- configuration keys
- environment variables
- message properties

---

## 16. Changing Data Types

Suppose:

```text
Amount: integer
```

must become:

```text
Amount: decimal
```

Instead of changing the type directly:

```text
Amount: integer
        ↓
        X
Amount: decimal
```

use:

```text
Amount
OldAmount: integer
NewAmount: decimal
```

Then:

```text
Backfill
   ↓
Migrate Writers
   ↓
Migrate Readers
   ↓
Validate
   ↓
Remove Old
```

This is particularly important when precision or semantic meaning changes.

---

## 17. Changing Relationships

Suppose:

```text
Order
└── CustomerName
```

becomes:

```text
Order
└── CustomerId
```

Expand:

```text
CustomerName
CustomerId
```

Migrate:

```text
CustomerName
      ↓
CustomerId
```

Then consumers use:

```text
CustomerId
```

Finally:

```text
CustomerName
```

is removed.

This can also represent a transition from denormalized to normalized data.

---

## 18. Dual Writes

During migration:

```text
Application
    │
    ├────→ Old
    │
    └────→ New
```

This can maintain compatibility while consumers migrate.

However:

```text
Old Write ✓
New Write ✗
```

can create inconsistent state.

Therefore dual writes require:

- idempotency
- failure handling
- reconciliation
- monitoring
- clear source-of-truth rules

When appropriate, transactional outbox or CDC can reduce some synchronization risks.

---

## 19. Dual Reads

Another migration technique:

```text
Read New
   │
   ├── Found → Return
   │
   └── Missing → Read Old
```

This can help during gradual data migration.

For example:

```text
New Store
   ↓
Data exists?
 ├── Yes → Use New
 └── No  → Legacy
```

After migration reaches completion:

```text
Remove Legacy Read
```

Dual reads should also have a clear removal condition.

---

## 20. Compatibility Matrix

During migration, explicitly understand which combinations are supported.

Example:

| Producer | Consumer | Supported           |
| -------- | -------- | ------------------- |
| Old      | Old      | Yes                 |
| New      | Old      | Yes                 |
| New      | New      | Yes                 |
| Old      | New      | Depends on contract |

The exact matrix depends on the direction of compatibility.

The important principle is:

> **Do not assume compatibility. Define and test it.**

---

## 21. Rollback

Before contraction:

```text
Old ✓
New ✓
```

Rollback can usually return to:

```text
Old
```

After contraction:

```text
Old ✗
New ✓
```

rollback becomes more difficult.

Therefore:

```text
Expand
   ↓
Migrate
   ↓
Validate
   ↓
Contract
```

should not be compressed into one deployment.

---

## 22. Architectural Invariants

The migration should enforce:

1. Expansion happens before removal.
2. Old and new structures can coexist.
3. Existing consumers remain functional during migration.
4. New consumers can use the new representation.
5. Data synchronization is explicit.
6. Source of truth is defined.
7. Compatibility is tested.
8. Old usage is measurable.
9. Contraction occurs only after migration completion.
10. Temporary compatibility code has a removal plan.
11. Rollback requirements are understood before contraction.
12. The final architecture contains no unnecessary migration artifacts.

---

## 23. Common Failure Modes

### Breaking During Expand

The new schema is deployed but old application versions cannot use it.

---

### Contract Too Early

```text
New ✓
Old ✗
```

while old consumers still exist.

---

### Uncontrolled Dual Writes

Old and new representations silently diverge.

---

### Unknown Consumers

An API or database field is removed because the team did not know another system depended on it.

---

### Permanent Compatibility

```text
Old + New
```

remain indefinitely.

---

### No Migration Measurement

Nobody knows:

```text
How many consumers remain?
How much data remains?
Is the new path complete?
```

Without measurement, contraction becomes guesswork.

---

### Rollback After Irreversible Change

The team contracts the old schema and then discovers that the new implementation cannot safely operate.

---

## 24. Testing Strategy

Use:

```text
Schema Tests
      ↓
Contract Tests
      ↓
Compatibility Tests
      ↓
Migration Tests
      ↓
Data Consistency Tests
      ↓
Integration Tests
      ↓
Production Validation
```

Important tests include:

### Old Application + Expanded Schema

Must continue working.

### New Application + Expanded Schema

Must work.

### Old Consumer + New Contract

Must behave according to the compatibility contract.

### Data Migration

Old and new representations satisfy expected invariants.

### Contract Phase

Verify that no consumer still requires the removed representation.

---

## 25. .NET Implementation

Useful technologies:

- EF Core migrations
- ASP.NET Core
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

Example EF Core migration:

```csharp
migrationBuilder.AddColumn<string>(
    name: "FirstName",
    table: "Customers");

migrationBuilder.AddColumn<string>(
    name: "LastName",
    table: "Customers");
```

Then application code is migrated.

Only later:

```csharp
migrationBuilder.DropColumn(
    name: "FullName",
    table: "Customers");
```

The database migration is therefore split into compatible steps.

---

## 26. Lab Experiment

Create:

```text
Customer Service
```

Initial schema:

```text
Customer
├── Id
└── FullName
```

Target:

```text
Customer
├── Id
├── FirstName
└── LastName
```

### Phase 1: Expand

Add:

```text
FirstName
LastName
```

Keep:

```text
FullName
```

### Phase 2: Backfill

Migrate existing data.

### Phase 3: Dual Compatibility

Support old and new application versions.

### Phase 4: Migrate Readers

Readers use:

```text
FirstName
LastName
```

### Phase 5: Migrate Writers

New writes use the new representation.

### Phase 6: Validate

Check:

```text
Data consistency
Application behavior
Error rate
Performance
```

### Phase 7: Contract

Remove:

```text
FullName
```

### Phase 8: Failure Experiment

Deploy an old application version after contraction.

Observe why the migration is no longer backward compatible.

---

## 27. Success Criteria

The migration is complete when:

```text
Expanded
   ↓
Compatible
   ↓
Migrated
   ↓
Validated
   ↓
Old Usage = 0
   ↓
Contracted
```

The final system should contain only the new representation and no unnecessary compatibility mechanisms.

---

## 28. Key Takeaways

- Expand and Contract is a safe way to evolve schemas and contracts.
- The fundamental sequence is **Expand → Migrate → Contract**.
- Add new structures before removing old ones.
- Old and new versions must coexist during the migration window.
- Database schema changes are a major use case.
- APIs and event contracts can follow the same model.
- Dual reads and dual writes can support migration but introduce complexity.
- Source-of-truth ownership must be explicit.
- Compatibility must be tested, not assumed.
- Measure migration progress before contracting.
- Contract only after all relevant consumers have migrated.
- Rollback becomes harder after contraction.
- Temporary migration mechanisms should be removed after completion.

> **Expand creates compatibility. Migrate moves the system. Contract removes the past.**
