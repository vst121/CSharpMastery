# 08 - Shared Database to Owned Data

## 1. Essence

**Shared Database to Owned Data** is an architecture evolution strategy for moving from a database shared by multiple modules or services to explicit data ownership.

Before:

```text
                ┌─────────────────┐
                │ Shared Database │
                └────────┬────────┘
                         │
          ┌──────────────┼──────────────┐
          ▼              ▼              ▼
       Orders         Payments      Customers
```

After:

```text
      Orders                 Payments              Customers
         │                      │                      │
         ▼                      ▼                      ▼
    Orders DB             Payments DB            Customers DB
```

The goal is not simply to create multiple databases.

The goal is to establish:

- explicit ownership
- controlled access
- clear write authority
- explicit contracts
- independent evolution
- reduced coupling

> **Owned data is an architectural boundary, not merely a separate database.**

---

## 2. The Problem

A shared database creates coupling between components that may otherwise appear independent.

For example:

```text
Orders Service
      │
      ├──────┐
      │      │
      ▼      ▼
 Orders   Payments
   Tables  Tables
      │      │
      └──┬───┘
         ▼
    Shared Database
```

Even if Orders and Payments have separate application code, they may still depend on:

- each other's tables
- foreign keys
- database views
- stored procedures
- queries
- indexes
- transactions
- schema changes
- reporting structures

The database becomes the real integration boundary.

---

## 3. Intent

Move from:

```text
Multiple Components
        ↓
Shared Data Store
```

to:

```text
Component
    ↓
Own Data
    ↓
Explicit Contract
    ↓
Other Components
```

The target architecture makes ownership visible.

---

## 4. Core Principles

### 4.1 One Owner

Every important business dataset should have a clearly defined owner.

```text
Order Data       → Orders
Payment Data     → Payments
Customer Data    → Customers
```

Ownership means responsibility for:

- writes
- invariants
- schema
- lifecycle
- security
- availability
- evolution

---

### 4.2 No Direct Foreign Access

If Payments owns:

```text
Payment
```

Orders should not execute:

```sql
SELECT * FROM Payments;
```

Instead:

```text
Orders
   ↓
Payment Contract
   ↓
Payments
```

---

### 4.3 Ownership Is More Important Than Physical Separation

These two architectures are not equivalent:

```text
Database A
├── Orders
└── Payments
```

and:

```text
Orders DB
Payments DB
```

Physical separation helps, but ownership must also be enforced.

A system can have multiple databases and still have poor ownership boundaries.

---

### 4.4 Replace Data Coupling with Contract Coupling

Before:

```text
Orders
   ↓
Payments Table
```

After:

```text
Orders
   ↓
Payment API / Event
   ↓
Payments
```

The dependency becomes explicit and versionable.

---

## 5. Architectural Model

### Before

```text
                 Shared Database
        ┌──────────────┼──────────────┐
        ▼              ▼              ▼
     Orders         Payments       Customers
```

### Transition

```text
              Shared Database
        ┌──────────────┼──────────────┐
        ▼              ▼              ▼
     Orders         Payments       Customers
        │
        ▼
    New Orders DB
```

Gradually:

```text
Orders → Orders DB
Payments → Payments DB
Customers → Customers DB
```

Final:

```text
┌────────────┐      ┌─────────────┐
│   Orders   │─────→│  Payments   │
│            │      │             │
│ Orders DB  │      │ Payments DB │
└────────────┘      └─────────────┘
       │
       ▼
 Explicit Contracts
```

---

## 6. Identify Ownership

Before moving data, determine:

```text
Who creates it?
Who updates it?
Who validates it?
Who deletes it?
Who reads it?
Who defines its business meaning?
Who is responsible when it is wrong?
```

A useful ownership matrix:

| Data      | Current Owner | Target Owner | Writers   | Readers           |
| --------- | ------------- | ------------ | --------- | ----------------- |
| Orders    | Shared DB     | Orders       | Orders    | Orders, Reporting |
| Payments  | Shared DB     | Payments     | Payments  | Orders, Reporting |
| Customers | Shared DB     | Customers    | Customers | Orders, Payments  |

The migration should resolve ambiguous ownership before physical extraction.

---

## 7. Discover Hidden Dependencies

The visible dependency may be:

```text
Orders → Payments
```

but the real dependency graph may be:

```text
Orders
 ├── Payments Table
 ├── Payment View
 ├── Payment Stored Procedure
 ├── Payment Foreign Key
 ├── Reporting Query
 └── Nightly ETL
```

Analyze:

- application code
- SQL queries
- stored procedures
- views
- triggers
- foreign keys
- database jobs
- ETL pipelines
- reporting systems
- BI tools
- scripts
- scheduled jobs
- ad-hoc operational tooling

> **You cannot remove a database dependency that you have not discovered.**

---

## 8. Classify Dependencies

Not every dependency requires the same migration strategy.

### Write Dependency

```text
Orders → UPDATE Payments
```

This is usually the strongest ownership violation.

### Read Dependency

```text
Orders → SELECT Payments
```

May be replaced by an API, event, replicated view, or local projection.

### Transaction Dependency

```text
BEGIN
  UPDATE Orders
  UPDATE Payments
COMMIT
```

This is significantly harder because the database currently provides atomicity.

### Referential Dependency

```text
Orders.PaymentId
        ↓
Payments.Id
```

Moving ownership may require removing the database-level foreign key and replacing it with application-level validation or asynchronous consistency.

---

## 9. Migration Strategy

A safe migration can follow:

```text
Discover
   ↓
Define Ownership
   ↓
Create Target Store
   ↓
Copy Data
   ↓
Synchronize
   ↓
Move Reads
   ↓
Move Writes
   ↓
Validate
   ↓
Remove Shared Access
   ↓
Remove Legacy Data
```

The exact sequence may differ depending on consistency and availability requirements.

---

## 10. Create the Owned Store

Suppose Payments currently lives inside:

```text
Shared DB
└── Payments
```

Create:

```text
Payments DB
└── Payments
```

The new database should contain only the data required by the Payments bounded capability.

Avoid copying the entire shared database.

---

## 11. Data Extraction

Move the initial dataset:

```text
Shared Payments
       ↓
Backfill
       ↓
Payments DB
```

For large datasets:

```text
Batch 1
Batch 2
Batch 3
...
Batch N
```

The migration should be:

- resumable
- idempotent
- observable
- validated

---

## 12. Synchronization

After the initial backfill, the source may continue changing.

Therefore:

```text
Shared DB
    │
    ├── Initial Backfill
    │
    └── Change Stream
             ↓
        Payments DB
```

Possible synchronization mechanisms:

- dual writes
- transactional outbox
- CDC
- event publishing
- replication
- change streams

Choose based on consistency requirements and operational constraints.

---

## 13. Move Reads

Initially:

```text
Payments Read
      ↓
Shared DB
```

After migration:

```text
Payments Read
      ↓
Payments DB
```

During transition, shadow reads can compare:

```text
Shared DB Result
       ↕
Payments DB Result
```

This helps identify:

- missing records
- transformation errors
- query differences
- semantic differences

---

## 14. Move Writes

After the target is synchronized:

```text
Payments
    ↓
Payments DB
```

becomes the authoritative write path.

The old shared table should no longer receive new Payments writes.

This is a critical ownership transition:

```text
Before:

Shared DB = Authority


After:

Payments DB = Authority
Shared DB = Legacy
```

---

## 15. Source of Truth

During migration there may temporarily be two copies:

```text
Shared DB
Payments DB
```

Never leave their authority ambiguous.

Define:

```text
Current Source of Truth
Target Source of Truth
Synchronization Direction
Conflict Resolution
Cutover Point
```

For example:

```text
Before Cutover:
Shared DB = Source of Truth

After Cutover:
Payments DB = Source of Truth
```

---

## 16. Cross-Domain Reads

Once Payments owns its data, Orders may still need payment information.

Instead of:

```text
Orders
   ↓
SELECT Payments
```

consider:

### Synchronous API

```text
Orders
   ↓
Payments API
   ↓
Payments DB
```

### Domain Event

```text
Payments
   ↓
PaymentCompleted
   ↓
Orders
```

### Local Projection

```text
Payments
   ↓
Event
   ↓
Orders Projection
```

The correct option depends on consistency and latency requirements.

---

## 17. Local Projections

A service should not need direct access to another service's database simply because it needs some of its data.

Instead:

```text
Payments
    │
    ▼
Payment Events
    │
    ▼
Orders Projection
```

Orders may maintain:

```text
PaymentStatus
PaymentReference
LastPaymentDate
```

locally.

This creates:

```text
Local Read Model
```

without transferring ownership.

---

## 18. Distributed Transactions

Shared databases often hide transactional coupling.

Before:

```text
BEGIN TRANSACTION

Update Order
Update Payment

COMMIT
```

After:

```text
Orders DB
     │
     ▼
OrderCreated
     │
     ▼
Payments
     │
     ▼
PaymentProcessed
```

The system may now require:

- Saga
- orchestration
- choreography
- compensating actions
- eventual consistency

The database migration therefore changes the consistency model.

---

## 19. Referential Integrity

A shared database can enforce:

```text
Orders.PaymentId
      ↓
Payments.Id
```

After separation, the database may no longer be able to enforce that relationship.

Instead:

```text
Orders
   ↓
Payment Contract
```

or:

```text
PaymentCompleted Event
```

provides the relationship.

The architecture must explicitly decide what happens when the referenced entity does not exist.

---

## 20. Reporting and Analytics

Reporting systems often become hidden consumers of shared databases.

Before:

```text
BI
 │
 └──→ Shared DB
```

After:

```text
Orders ──┐
Payments ├──→ Events / CDC ──→ Analytics Store
Customers┘
```

Do not automatically give reporting systems direct access to every owned database.

A dedicated analytical model may be more appropriate.

---

## 21. Transaction Boundaries

Moving from shared data to owned data often changes transaction boundaries.

Before:

```text
One DB Transaction
```

After:

```text
Transaction A
      ↓
Event
      ↓
Transaction B
```

Therefore explicitly define:

```text
Consistency Boundary
Failure Handling
Retry Semantics
Compensation
Idempotency
```

---

## 22. Failure Handling

After ownership separation, expect:

```text
Payments unavailable
Event delayed
Network timeout
Duplicate event
Out-of-order event
Synchronization lag
Partial migration
```

Design for:

- retries
- idempotency
- timeouts
- dead-letter queues
- reconciliation
- compensating actions
- observability

---

## 23. Common Failure Modes

### Shared Database in Disguise

Services receive separate schemas but continue accessing each other's data.

```text
Orders DB
   ↑
Payments directly queries it
```

The ownership boundary is not real.

---

### Shared Tables Through Views

A team creates views that expose another service's tables.

This may simply recreate the original coupling.

---

### Dual Ownership

Two services continue writing the same entity.

```text
Orders ──→ Payment
Payments ─→ Payment
```

There is no clear authority.

---

### Permanent Dual Writes

Both databases remain authoritative indefinitely.

---

### Distributed Transaction Everywhere

The team extracts databases but recreates the old transaction model through synchronous coordination.

---

### Chatty APIs

Replacing:

```sql
SELECT ...
```

with dozens of remote calls does not necessarily improve architecture.

---

### Incomplete Data Extraction

Only the main table is migrated while related data, jobs, or stored procedures remain dependent on the shared database.

---

### Reporting Dependency

A supposedly independent service still cannot change its schema because a BI query depends directly on it.

---

### Hidden Backdoor Access

Developers retain shared database credentials "temporarily."

Temporary access often becomes permanent coupling.

---

## 24. Architectural Invariants

The target architecture should enforce:

1. Every owned dataset has one clear owner.
2. Only the owner writes authoritative data.
3. Other components do not directly access the owner's database.
4. Cross-boundary access uses explicit contracts.
5. Source-of-truth ownership is explicit.
6. Database credentials are scoped to ownership.
7. Cross-boundary transactions are explicit.
8. Eventual consistency is acknowledged where applicable.
9. Local projections do not become new sources of truth.
10. Synchronization is observable.
11. Migration mechanisms have a removal plan.
12. Legacy shared access is eventually removed.
13. Hidden consumers are identified before cutover.
14. Data ownership survives deployment and infrastructure changes.

---

## 25. Database Access as an Architecture Test

A powerful architectural test is:

```text
Orders
  ✗
Payments.Infrastructure
```

and:

```text
Orders
  ✗
Payments.Database
```

Instead:

```text
Orders
  ✓
Payments.Contract
```

The test should fail if an application component directly references another owner's persistence layer.

---

## 26. Testing Strategy

Use:

```text
Ownership Tests
      ↓
Architecture Tests
      ↓
Migration Tests
      ↓
Contract Tests
      ↓
Data Consistency Tests
      ↓
Integration Tests
      ↓
Resilience Tests
```

### Ownership Tests

Verify:

```text
Payments owns Payment writes.
Orders cannot write Payment data.
```

### Migration Tests

Verify:

```text
Shared Data
     ↓
Owned Data
```

produces equivalent business state.

### Contract Tests

Verify that consumers can obtain required information without database access.

### Failure Tests

Simulate:

```text
Owner unavailable
Event delayed
Event duplicated
Target DB unavailable
Synchronization failure
```

---

## 27. Observability

Track:

```text
Migration Progress
Synchronization Lag
Records Migrated
Records Remaining
Data Mismatches
Cross-Service Calls
API Latency
Event Processing Lag
Retry Count
Dead Letters
Legacy DB Access
```

One particularly useful metric is:

```text
legacy_shared_database_access_total
```

The migration should drive this toward:

```text
0
```

before the old shared access is removed.

---

## 28. Security

Data ownership should also define security boundaries.

The Payments service should have credentials that allow:

```text
Payments DB
    ✓ Read
    ✓ Write
```

but:

```text
Orders DB
    ✗
Customers DB
    ✗
```

This turns architectural ownership into an enforceable security boundary.

Also consider:

- least privilege
- database roles
- service identities
- encryption
- audit logging
- secret rotation
- network segmentation

---

## 29. .NET Implementation

Useful technologies:

- EF Core
- PostgreSQL
- SQL Server
- Dapper
- ASP.NET Core
- gRPC
- REST
- Kafka
- RabbitMQ
- transactional outbox
- CDC
- BackgroundService
- OpenTelemetry
- Aspire
- architecture testing

A service should typically have its own persistence boundary:

```text
Payments
├── Domain
├── Application
├── Infrastructure
│   └── PaymentsDbContext
└── API
```

The `PaymentsDbContext` should not become an infrastructure dependency of Orders.

---

## 30. Migration State Machine

Model the migration explicitly:

```text
Discovered
    ↓
OwnershipDefined
    ↓
TargetCreated
    ↓
Backfilled
    ↓
Synchronized
    ↓
ReadsMigrated
    ↓
WritesMigrated
    ↓
Validated
    ↓
TargetAuthoritative
    ↓
LegacyAccessRemoved
    ↓
SharedDataRetired
```

This makes the migration state visible and operationally manageable.

---

## 31. Production Checklist

### Before Migration

- [ ] Define ownership.
- [ ] Identify all writers.
- [ ] Identify all readers.
- [ ] Discover hidden consumers.
- [ ] Identify cross-database transactions.
- [ ] Identify reporting and ETL dependencies.
- [ ] Create target database.
- [ ] Define source of truth.
- [ ] Define synchronization strategy.
- [ ] Define rollback strategy.

### During Migration

- [ ] Backfill data.
- [ ] Validate migrated records.
- [ ] Synchronize changes.
- [ ] Monitor synchronization lag.
- [ ] Move reads gradually.
- [ ] Move writes explicitly.
- [ ] Verify target ownership.
- [ ] Monitor legacy access.

### Before Completion

- [ ] No unauthorized writers remain.
- [ ] No direct cross-database readers remain.
- [ ] Target is authoritative.
- [ ] Data is reconciled.
- [ ] Cross-boundary contracts are tested.
- [ ] Failure scenarios are tested.
- [ ] Legacy access is approaching zero.

### After Completion

- [ ] Remove shared database access.
- [ ] Remove synchronization.
- [ ] Remove migration code.
- [ ] Remove obsolete tables.
- [ ] Remove unused credentials.
- [ ] Update architecture documentation.
- [ ] Add architecture tests preventing regression.

---

## 32. Lab Experiment

Build a realistic shared database:

```text
                    Shared DB
        ┌──────────────┼──────────────┐
        ▼              ▼              ▼
      Orders        Payments       Customers
```

Start with:

```text
Orders
Payments
Customers
```

all using one PostgreSQL database.

### Phase 1: Discover

Find:

```text
Direct SQL Access
Foreign Keys
Cross-Module Queries
Transactions
Reporting Queries
```

### Phase 2: Define Ownership

```text
Orders    → Orders
Payments  → Payments
Customers → Customers
```

### Phase 3: Extract Payments

Create:

```text
Payments DB
```

### Phase 4: Backfill

Copy Payments data.

### Phase 5: Synchronize

Use:

```text
Outbox
```

or:

```text
CDC
```

to synchronize changes.

### Phase 6: Move Reads

Change:

```text
Orders → Shared DB
```

to:

```text
Orders → Payments API
```

or a local projection.

### Phase 7: Move Writes

Make:

```text
Payments DB
```

the authoritative write store.

### Phase 8: Validate

Compare:

```text
Shared Payments
      ↕
Owned Payments
```

### Phase 9: Remove Shared Access

Break:

```text
Orders → Payments Table
```

and verify the architecture test fails if somebody tries to restore it.

### Phase 10: Failure Experiments

Test:

```text
Payments DB unavailable
Event delayed
Event duplicated
Synchronization stopped
Network timeout
Partial migration
```

Observe:

```text
Consistency
Recovery
Retries
Idempotency
Operational Visibility
```

---

## 33. Success Criteria

The migration is complete when:

```text
Shared Data
     ↓
Explicit Ownership
     ↓
Owned Storage
     ↓
Explicit Contracts
     ↓
No Direct Cross-Access
     ↓
Legacy Access = 0
```

The most important success condition is not:

```text
Number of Databases > 1
```

It is:

```text
Number of Authoritative Owners = Clear
```

---

## 34. Key Takeaways

- Shared databases create coupling even when application components appear independent.
- Database ownership is an architectural responsibility, not merely a storage decision.
- One capability should have one authoritative owner for its business data.
- Separate databases without access boundaries do not create real ownership.
- The migration must discover hidden consumers, queries, jobs, reports, and transactions.
- Backfill, synchronization, read migration, write migration, and cutover are separate concerns.
- Cross-boundary data access should use explicit contracts, events, or local projections.
- Moving database ownership can change transaction and consistency boundaries.
- Distributed transactions may need to become Saga-based workflows or eventual consistency.
- Security permissions can enforce architectural ownership.
- Architecture tests should prevent direct access to another component's persistence layer.
- The final goal is not "more databases."

> **The goal is to turn shared data into explicitly owned data, and hidden database coupling into explicit architectural contracts.**
