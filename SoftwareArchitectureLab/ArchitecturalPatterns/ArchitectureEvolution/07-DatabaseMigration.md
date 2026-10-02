# 07 - Database Migration

## 1. Essence

**Database Migration** is the controlled movement of a database schema, data, ownership, or storage technology from a current state to a target state.

The migration may involve:

- schema changes
- data transformation
- database technology changes
- database ownership changes
- splitting or consolidating databases
- moving between infrastructure environments
- changing persistence models

The fundamental transition is:

```text
Current State
     ↓
Migration Strategy
     ↓
Compatibility
     ↓
Data Movement
     ↓
Validation
     ↓
Cutover
     ↓
Target State
```

> **Database migration is not simply moving tables. It is moving data, behavior, ownership, and operational responsibility safely.**

---

## 2. The Problem

Databases are often among the hardest parts of an architecture to change.

Applications may depend on:

```text
Schema
Queries
Indexes
Transactions
Constraints
Stored Procedures
Views
Triggers
Data Semantics
Performance Characteristics
```

Other systems may also depend directly on the database.

```text
                 ┌──────────────┐
                 │  Application │
                 └──────┬───────┘
                        │
                 ┌──────▼───────┐
                 │   Database   │
                 └──────┬───────┘
                        │
          ┌─────────────┼─────────────┐
          ▼             ▼             ▼
      Reporting      ETL Jobs      Other Apps
```

Changing the database without understanding these dependencies can break systems that are not visible from the primary application.

---

## 3. Intent

Move from:

```text
Current Database
```

to:

```text
Target Database
```

while controlling:

- downtime
- data loss
- inconsistency
- application compatibility
- rollback risk
- performance degradation
- operational disruption

---

## 4. Core Principles

### 4.1 Understand Before Moving

Before migration, identify:

```text
Consumers
Schema
Data Volume
Dependencies
Queries
Transactions
Integrations
Performance Characteristics
Operational Constraints
```

---

### 4.2 Separate Schema Migration from Data Migration

A schema change:

```text
Add Column
Rename Column
Create Index
```

is different from:

```text
Move 500 GB of data
Transform records
Copy historical data
```

Treat them as separate concerns even when executed together.

---

### 4.3 Preserve Compatibility

During migration, old and new application versions may coexist.

Therefore:

```text
Old Application
      +
New Application
      ↓
Compatible Database State
```

is often required.

---

### 4.4 Data Ownership Must Be Explicit

Define:

```text
Who owns the data?
Who can write?
Who can read?
What is the source of truth?
```

A database migration is incomplete if ownership remains ambiguous.

---

### 4.5 Validate Before Cutover

Never assume:

```text
Migration completed = Migration correct
```

Validate:

```text
Completeness
Consistency
Integrity
Business Rules
Performance
Application Behavior
```

---

## 5. Migration Types

Database migration can mean several different things.

### Schema Migration

```text
Existing DB
    ↓
New Schema
```

Example:

```text
FullName
   ↓
FirstName + LastName
```

---

### Data Migration

Move or transform data:

```text
Old Representation
       ↓
Transformation
       ↓
New Representation
```

---

### Database-to-Database Migration

```text
Database A
     ↓
Database B
```

Example:

```text
SQL Server
    ↓
PostgreSQL
```

---

### Database Ownership Migration

```text
Shared Database
      ↓
Service-Owned Database
```

This is particularly important when evolving toward independently deployable services.

---

### Database Consolidation

```text
DB A ─┐
      ├──→ DB
DB B ─┘
```

---

### Database Splitting

```text
              DB
              │
       ┌──────┴──────┐
       ▼             ▼
   Orders DB     Payments DB
```

---

### Infrastructure Migration

```text
On-Prem Database
       ↓
Cloud Database
```

The logical data model may remain unchanged while the infrastructure changes.

---

## 6. Architectural Model

A controlled migration can be represented as:

```text
              ┌───────────────┐
              │ Source System │
              └───────┬───────┘
                      │
                 Data Movement
                      │
              ┌───────▼───────┐
              │ Target System │
              └───────┬───────┘
                      │
                  Validation
                      │
                      ▼
                   Cutover
```

For a zero-downtime migration:

```text
                 ┌─────────────┐
                 │ Application │
                 └──────┬──────┘
                        │
                 ┌──────▼──────┐
                 │ Migration   │
                 │ Boundary    │
                 └───┬──────┬──┘
                     │      │
                     ▼      ▼
                  Source   Target
```

---

## 7. Migration Strategies

### 7.1 Offline Migration

Stop the application:

```text
Stop
 ↓
Copy / Transform
 ↓
Validate
 ↓
Start
```

Advantages:

- simple
- easier consistency model
- easier reasoning

Costs:

- downtime
- potentially large maintenance window

Suitable when downtime is acceptable.

---

### 7.2 Big-Bang Migration

Move everything in one operation:

```text
Source
  ↓
Migration
  ↓
Target
```

This can be appropriate for small systems, but the blast radius is large.

---

### 7.3 Incremental Migration

Move data gradually:

```text
Source
 ↓
Batch 1
 ↓
Batch 2
 ↓
Batch 3
 ↓
...
 ↓
Target
```

Benefits:

- smaller operational steps
- easier validation
- reduced migration risk

Costs:

- more complex synchronization
- longer migration period

---

## 8. Dual Write

During migration:

```text
Application
    │
    ├────→ Source DB
    │
    └────→ Target DB
```

This keeps both databases updated.

However:

```text
Source Write ✓
Target Write ✗
```

can create divergence.

Therefore dual writes require:

- idempotency
- retry handling
- reconciliation
- monitoring
- failure handling
- clear source-of-truth rules

Dual writes should not be treated as automatically consistent.

---

## 9. Change Data Capture

Instead of requiring the application to write to both databases:

```text
Application
     │
     ▼
Source DB
     │
     ▼
CDC
     │
     ▼
Target DB
```

Changes are captured from the source and propagated to the target.

This can be useful for:

- large migrations
- low-downtime migrations
- continuous synchronization
- database replacement

CDC introduces its own operational requirements:

- ordering
- replay
- duplicates
- lag
- failure recovery
- schema compatibility

---

## 10. Backfill

Existing data often needs transformation.

```text
Existing Data
      ↓
Backfill
      ↓
New Representation
```

For example:

```text
Customer
FullName = "John Doe"
```

becomes:

```text
FirstName = "John"
LastName  = "Doe"
```

Backfills should usually be:

- idempotent
- restartable
- observable
- rate-limited
- resumable

Avoid one enormous transaction for large datasets unless the database and operational constraints explicitly support it.

---

## 11. Batch Migration

Large datasets should often be migrated in batches.

```text
10,000 records
       ↓
Batch
       ↓
Validate
       ↓
Next Batch
```

Example:

```text
Batch 1: IDs 1-10000
Batch 2: IDs 10001-20000
Batch 3: IDs 20001-30000
```

Track:

```text
Processed
Failed
Skipped
Retried
Remaining
```

---

## 12. Idempotency

A migration should ideally tolerate retries.

Bad:

```text
Run Migration
     ↓
Failure
     ↓
Run Again
     ↓
Duplicate Data
```

Better:

```text
Run Migration
     ↓
Failure
     ↓
Resume / Retry
     ↓
Same Final State
```

Use:

- stable identifiers
- upserts
- checkpoints
- migration markers
- deterministic transformations

---

## 13. Validation

Migration validation should operate at multiple levels.

### Structural Validation

```text
Tables
Columns
Indexes
Constraints
Types
```

### Record Validation

```text
Row counts
Missing records
Duplicate records
Checksums
```

### Referential Validation

```text
Foreign Keys
References
Relationships
```

### Semantic Validation

```text
Business invariants
Aggregates
Domain rules
```

### Application Validation

```text
Queries
Commands
Transactions
Workflows
```

### Performance Validation

```text
Latency
Throughput
CPU
Memory
IO
Locking
Query plans
```

---

## 14. Reconciliation

For large migrations, reconciliation is often essential.

Example:

```text
Source Count = 10,000,000
Target Count = 9,999,812
```

The migration is not complete.

Reconciliation may compare:

```text
Count
Primary Keys
Hashes
Aggregates
Checksums
Business Metrics
```

For example:

```text
SUM(Order.Amount)
Source = Target
```

can detect certain classes of migration errors.

---

## 15. Cutover

Cutover changes the application from:

```text
Source
```

to:

```text
Target
```

Possible approaches:

### Configuration Switch

```text
DatabaseConnection = Target
```

### Feature Flag

```text
UseTargetDatabase = true
```

### Routing Layer

```text
Application
     ↓
Database Router
  ┌──┴──┐
  ▼     ▼
Source Target
```

### DNS / Infrastructure Switch

Useful for infrastructure-level migration.

The cutover mechanism should be explicit and observable.

---

## 16. Shadow Reads

During migration:

```text
Application
    │
    ├──→ Source → Response
    │
    └──→ Target → Compare
```

The target result is compared without becoming authoritative.

This can reveal:

```text
Query Differences
Data Differences
Performance Differences
Semantic Differences
```

Do not expose shadow results to users until they are trusted.

---

## 17. Read Switching

A safer progression can be:

```text
100% Source
     ↓
99% Source / 1% Target
     ↓
90% Source / 10% Target
     ↓
50% / 50%
     ↓
10% / 90%
     ↓
100% Target
```

Each step should have explicit validation criteria.

---

## 18. Rollback

Rollback must be designed before cutover.

Before cutover:

```text
Source ✓
Target ✓
```

After switching:

```text
Application → Target
```

If problems occur:

```text
Application → Source
```

But rollback becomes complicated if new writes exist only in the target.

Therefore define:

```text
Write Strategy
Data Synchronization
Rollback Window
Consistency Rules
Recovery Procedure
```

before switching.

---

## 19. Schema Compatibility

During a rolling deployment:

```text
Application V1
Application V2
```

may run simultaneously.

The database therefore needs a compatible intermediate state.

A common sequence is:

```text
Expand
  ↓
Deploy Compatible Application
  ↓
Migrate Data
  ↓
Switch Consumers
  ↓
Contract
```

This is where **Expand and Contract** becomes a database migration technique.

---

## 20. Database Ownership Migration

Consider:

```text
                 Shared DB
              ┌──────┴──────┐
              ▼             ▼
           Orders        Payments
```

Target:

```text
Orders Service       Payments Service
      │                    │
      ▼                    ▼
Orders DB            Payments DB
```

Migration requires more than copying tables.

You must determine:

```text
Data Ownership
Write Ownership
Read Ownership
Cross-Service Queries
Transactions
References
Reporting
Synchronization
```

The migration is complete only when the old ownership dependency has been removed.

---

## 21. Shared Database to Owned Data

A typical transition:

```text
Shared Database
      ↓
Identify Ownership
      ↓
Create Target Store
      ↓
Backfill
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
```

This is one of the most important database migrations in service-oriented architecture.

---

## 22. Technology Migration

Example:

```text
SQL Server
     ↓
PostgreSQL
```

The challenge is not only copying rows.

Consider differences in:

```text
Data Types
SQL Dialect
Indexes
Constraints
Transactions
Isolation
Sequences
Identity Generation
Functions
Stored Procedures
Date/Time Semantics
Collation
Case Sensitivity
Query Optimizer
Performance
```

Application behavior must be tested against the target database.

---

## 23. Common Failure Modes

### Data Loss

Records disappear during transformation or synchronization.

---

### Data Duplication

Retries or incorrect keys create duplicate records.

---

### Inconsistent Data

Source and target diverge.

---

### Hidden Consumers

Another system directly depends on the old database.

---

### Broken Transactions

A transaction that was local becomes distributed.

---

### Performance Regression

The migrated database produces different query plans or locking behavior.

---

### Long-Running Locks

Migration blocks production traffic.

---

### Non-Idempotent Migration

A failed migration cannot safely resume.

---

### No Rollback Strategy

The team discovers after cutover that returning to the old database is unsafe.

---

### Permanent Dual Storage

Both databases continue to be treated as authoritative indefinitely.

---

## 24. Architectural Invariants

A database migration should enforce:

1. Data ownership is explicit.
2. Source and target are clearly identified.
3. Migration steps are repeatable where possible.
4. Data movement is observable.
5. Transformations are deterministic where possible.
6. Migration progress is measurable.
7. Data integrity is validated.
8. Referential integrity is preserved.
9. Application compatibility is maintained during transition.
10. Cutover is explicit.
11. Rollback requirements are understood.
12. The source of truth is clearly defined.
13. Temporary synchronization mechanisms have a removal plan.
14. No hidden consumers remain.
15. The final architecture has a single authoritative data owner.

---

## 25. Testing Strategy

Use several layers.

```text
Unit Tests
     ↓
Transformation Tests
     ↓
Schema Tests
     ↓
Migration Tests
     ↓
Data Validation
     ↓
Integration Tests
     ↓
Performance Tests
     ↓
Shadow Validation
     ↓
Production Verification
```

Important test categories:

### Transformation Tests

Verify:

```text
Old Record → Expected New Record
```

### Idempotency Tests

Run migration twice and verify the final state is unchanged.

### Failure Recovery Tests

Interrupt migration and resume it.

### Compatibility Tests

Verify old and new application versions against the intermediate schema.

### Performance Tests

Measure:

```text
Migration Throughput
Database Load
Application Latency
Query Performance
Lock Duration
CDC Lag
```

---

## 26. Observability

Track at minimum:

```text
Migration Progress
Records Processed
Records Failed
Records Remaining
Migration Rate
Error Rate
Retry Count
CDC Lag
Data Mismatch
Database CPU
Database IO
Locking
Query Latency
Application Errors
```

Useful migration metrics:

```text
migration_records_total
migration_records_failed
migration_records_remaining
migration_lag_seconds
migration_mismatch_total
migration_duration_seconds
```

Migration logs should include:

```text
MigrationId
BatchId
EntityId
Source
Target
Status
Timestamp
Error
```

---

## 27. Security Considerations

Database migration often temporarily increases access requirements.

Control:

- database credentials
- migration identities
- encryption in transit
- encryption at rest
- secrets
- least privilege
- audit logging
- temporary access
- backup protection
- sensitive data exposure

Migration tooling should not automatically receive unrestricted production database access.

---

## 28. .NET Implementation

Useful technologies include:

- EF Core migrations
- DbUp
- FluentMigrator
- ADO.NET
- Dapper
- BackgroundService
- hosted services
- feature flags
- transactional outbox
- CDC
- Kafka
- OpenTelemetry
- health checks
- Aspire
- Docker

For large data migrations, avoid forcing the entire migration through application request paths.

Prefer dedicated migration workers:

```text
Migration Worker
      │
      ├── Batch
      ├── Transform
      ├── Validate
      ├── Checkpoint
      └── Metrics
```

---

## 29. Migration State Machine

A migration can be modeled explicitly:

```text
Planned
   ↓
Prepared
   ↓
Expanded
   ↓
Backfilling
   ↓
Synchronizing
   ↓
Validated
   ↓
ReadyForCutover
   ↓
Cutover
   ↓
Verified
   ↓
Contracted
   ↓
Completed
```

Failure states should also be explicit:

```text
        ┌───────────┐
        │  Failed   │
        └─────┬─────┘
              │
       Retry / Resume
              │
              ▼
        Previous State
```

This makes the migration itself observable and operationally manageable.

---

## 30. Production Migration Checklist

### Before Migration

- [ ] Identify all consumers.
- [ ] Identify data ownership.
- [ ] Define source of truth.
- [ ] Estimate data volume.
- [ ] Define migration strategy.
- [ ] Define rollback strategy.
- [ ] Create backups.
- [ ] Test migration on production-like data.
- [ ] Define validation criteria.
- [ ] Define observability.
- [ ] Define operational stop conditions.

### During Migration

- [ ] Monitor migration progress.
- [ ] Monitor database load.
- [ ] Monitor application latency.
- [ ] Monitor synchronization lag.
- [ ] Validate batches.
- [ ] Track failures.
- [ ] Reconcile data.
- [ ] Avoid uncontrolled manual changes.

### Before Cutover

- [ ] Data synchronization is current.
- [ ] Validation has passed.
- [ ] Application compatibility is verified.
- [ ] Rollback procedure is tested.
- [ ] Operational owners are ready.

### After Cutover

- [ ] Monitor application behavior.
- [ ] Compare business metrics.
- [ ] Validate data.
- [ ] Monitor target performance.
- [ ] Keep rollback capability for the defined window.
- [ ] Remove temporary synchronization.
- [ ] Remove legacy access.
- [ ] Remove obsolete schema.

---

## 31. Lab Experiment

Build a realistic migration:

```text
SQL Server
    ↓
PostgreSQL
```

Initial application:

```text
Order Service
     │
     ▼
SQL Server
```

Target:

```text
Order Service
     │
     ▼
PostgreSQL
```

### Phase 1: Analyze

Measure:

```text
Tables
Rows
Indexes
Constraints
Queries
Dependencies
Performance
```

### Phase 2: Create Target

Create the PostgreSQL schema.

### Phase 3: Backfill

Move historical orders in batches.

### Phase 4: Validate

Compare:

```text
Counts
Keys
Aggregates
Business Rules
```

### Phase 5: Synchronize

Introduce CDC or another controlled synchronization mechanism.

### Phase 6: Shadow

Read from both databases and compare results.

### Phase 7: Cutover

Switch a controlled percentage of traffic.

### Phase 8: Failure Experiment

Inject:

```text
Target Database Failure
Migration Worker Failure
Network Failure
Data Transformation Error
Synchronization Lag
```

Observe recovery behavior.

### Phase 9: Complete

Move all traffic to PostgreSQL.

### Phase 10: Contract

Remove:

```text
SQL Server Access
Migration Synchronization
Legacy Configuration
Temporary Compatibility Code
```

---

## 32. Key Takeaways

- Database migration is an architectural change, not just a database operation.
- Separate schema evolution, data movement, ownership, and cutover concerns.
- Understand hidden consumers before migrating.
- Define the source of truth explicitly.
- Prefer incremental and observable migration for large or critical systems.
- Make migration operations idempotent and restartable where possible.
- Validate data structurally, technically, and semantically.
- Treat dual writes and synchronization as sources of complexity, not free safety.
- Design rollback before cutover.
- Use Expand and Contract to preserve compatibility during schema evolution.
- Measure migration progress and data correctness.
- Remove temporary migration mechanisms after completion.
- A successful migration changes not only where data lives, but also who owns it and how the system interacts with it.

> **The goal of database migration is not merely to move data. It is to move the system safely from one state of ownership, behavior, and infrastructure to another.**
