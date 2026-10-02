# 10 - Legacy Replacement

## 1. Essence

**Legacy Replacement** is an architecture evolution strategy for gradually replacing an existing system, component, platform, or technology with a new implementation while controlling business, technical, and operational risk.

The legacy system may be:

- an entire application
- a monolith
- a subsystem
- a database
- a messaging platform
- an integration layer
- a framework
- a programming language/runtime
- a proprietary platform
- an infrastructure component

The fundamental transition is:

```text
Legacy System
     ↓
Understand
     ↓
Define Target
     ↓
Introduce Compatibility
     ↓
Build Replacement
     ↓
Migrate Incrementally
     ↓
Validate
     ↓
Cut Over
     ↓
Retire Legacy
````

> **Legacy replacement is not a rewrite project. It is a controlled transition from one architectural state to another.**

---

## 2. The Problem

Legacy systems often remain critical even when they are difficult to change.

Typical characteristics include:

```text
Tight Coupling
Outdated Technology
Missing Tests
Implicit Business Rules
Shared State
Poor Observability
High Maintenance Cost
Specialized Knowledge
Unsupported Dependencies
Deployment Constraints
Security Constraints
Scalability Limitations
```

The system may still work correctly.

The problem is often that:

```text
Cost of Change ↑
Risk of Change ↑
Speed of Change ↓
```

Replacing it therefore becomes an architectural evolution problem.

---

## 3. Why Legacy Replacement Is Difficult

The difficulty is usually not writing the new implementation.

The difficult questions are:

```text
What does the legacy system actually do?

Which behaviors are business-critical?

Who depends on it?

Which rules are undocumented?

Where is the data?

What must remain compatible?

How do we migrate safely?

When can the legacy system be removed?
```

A legacy system may contain years of accumulated business behavior that is not documented anywhere else.

---

## 4. Intent

Move from:

```text
Legacy
```

to:

```text
Target Architecture
```

without requiring an uncontrolled big-bang transition.

A controlled migration looks like:

```text
Legacy
  │
  ├── Existing Capability
  │
  └── Migration Boundary
          │
          ▼
      New Capability
          │
          ▼
       Target State
```

---

## 5. Core Principles

### 5.1 Understand Before Replacing

Do not begin with:

```text
"Let's rewrite it."
```

Begin with:

```text
"Let's understand what the system actually does."
```

---

### 5.2 Preserve Business Behavior Before Improving It

During the first migration stage, distinguish:

```text
Behavior Preservation
```

from:

```text
Behavior Improvement
```

Changing architecture and business behavior simultaneously increases migration risk.

---

### 5.3 Create a Migration Seam

Introduce a boundary between consumers and the legacy implementation.

```text
Consumers
    ↓
Migration Boundary
    ↓
┌──────────┬──────────┐
│ Legacy   │   New    │
└──────────┴──────────┘
```

The boundary may be:

* API
* interface
* adapter
* facade
* event
* message
* routing layer
* feature flag

---

### 5.4 Migrate Incrementally

Prefer:

```text
Capability 1
Capability 2
Capability 3
...
```

over:

```text
Rewrite Everything
```

Incremental migration creates smaller validation and rollback boundaries.

---

### 5.5 Make the Legacy System Smaller

Every migration step should reduce the responsibility of the legacy system.

```text
Legacy
████████████████████

After Migration
████████████
██████ New

Later
████ New
██ Legacy

Final
████████████ New
```

If the legacy system remains equally responsible after years of migration, the strategy is not working.

---

## 6. Architectural Model

### Initial State

```text
                 Consumers
                     │
                     ▼
              Legacy System
                     │
                     ▼
               Legacy Data
```

### Transitional State

```text
                 Consumers
                     │
                     ▼
             Migration Boundary
               ┌─────┴─────┐
               ▼           ▼
            Legacy        New
               │           │
               ▼           ▼
          Legacy Data   New Data
```

### Target State

```text
                 Consumers
                     │
                     ▼
                New System
                     │
                     ▼
                  New Data
```

---

## 7. Legacy Discovery

Before migration, document:

```text
Capabilities
Dependencies
Data
Integrations
APIs
Events
Jobs
Batch Processes
Users
Operational Processes
Business Rules
Security Requirements
Performance Characteristics
```

Use multiple sources:

```text
Source Code
Database
Logs
Traces
Production Metrics
API Traffic
Message Traffic
Git History
Deployment History
Team Knowledge
Production Behavior
```

The goal is to discover both documented and undocumented behavior.

---

## 8. Characterization Tests

Legacy systems often have insufficient tests.

Before replacing behavior, create characterization tests.

The purpose is not necessarily to prove that the existing behavior is ideal.

It is to record:

```text
"What does the system currently do?"
```

Example:

```text
Input
  ↓
Legacy System
  ↓
Observed Output
```

Capture:

```text
Output
Errors
Side Effects
Database Changes
Messages
External Calls
```

These observations become migration safety constraints.

---

## 9. Business Capability Decomposition

Do not necessarily migrate technical layers:

```text
Legacy UI
Legacy Service
Legacy Repository
```

Instead identify capabilities:

```text
Orders
Payments
Customers
Pricing
Inventory
Reporting
```

Then migrate capability by capability.

A capability should ideally have:

```text
Cohesive Behavior
Clear Ownership
Manageable Dependencies
Observable Boundaries
```

---

## 10. Migration Seams

A migration seam is a location where legacy behavior can be separated from the rest of the system.

Possible seams:

```text
API Endpoint
Module
Interface
Repository
Integration
Message Consumer
Workflow
Database Table
Business Capability
```

Good seams minimize the amount of legacy behavior that must move simultaneously.

---

## 11. Anti-Corruption Layer

Legacy models should not automatically become the new system's domain model.

Example:

```text
Legacy Customer
     │
     ▼
Legacy Adapter
     │
     ▼
Translation
     │
     ▼
New Customer Model
```

The new architecture should preserve its own concepts.

This prevents legacy terminology and structural decisions from leaking into the target system.

---

## 12. Compatibility Layer

A compatibility layer can protect existing consumers.

```text
Consumer
   ↓
Compatibility API
   ↓
Legacy / New
```

Examples:

* API adapter
* facade
* DTO translator
* protocol adapter
* schema translator
* message translator

Compatibility code should have:

```text
Owner
Purpose
Usage Metrics
Removal Condition
```

Otherwise it can become permanent architecture.

---

## 13. Branch by Abstraction

For implementation replacement:

```text
Consumer
   ↓
Abstraction
   ↓
┌────────────┬────────────┐
│ Legacy     │ New        │
│ Impl       │ Impl       │
└────────────┴────────────┘
```

Consumers migrate to the abstraction first.

Then the implementation can change behind it.

This is particularly useful for:

* repositories
* payment providers
* search engines
* messaging systems
* external APIs
* algorithms
* infrastructure components

---

## 14. Strangler Strategy

For application-level replacement:

```text
Client
  ↓
Routing Boundary
  ↓
┌──────────────┬──────────────┐
│ Legacy       │ New          │
└──────────────┴──────────────┘
```

New functionality gradually takes responsibility for capabilities.

The legacy system becomes smaller until it can be retired.

---

## 15. Parallel Operation

Sometimes old and new implementations need to run simultaneously.

```text
Request
   │
   ├──→ Legacy
   │
   └──→ New
```

Results can be compared:

```text
Legacy Result
     ↕
New Result
```

This is useful for:

* pricing
* calculations
* scoring
* classification
* data transformation
* search
* decision engines

For side-effecting operations, parallel execution must be carefully controlled.

---

## 16. Shadow Traffic

Production traffic can be copied to the new implementation:

```text
Real Request
    │
    ├──→ Legacy → Real Response
    │
    └──→ New → Shadow Response
```

Compare:

```text
Correctness
Latency
Errors
Resource Usage
Business Results
```

The shadow path should not accidentally produce duplicate side effects.

---

## 17. Data Migration

Data is often the hardest part of replacement.

Possible strategies:

```text
Offline Copy
Backfill
Dual Write
CDC
Replication
Event-Based Synchronization
Incremental Migration
```

Typical sequence:

```text
Legacy Data
    ↓
Target Schema
    ↓
Backfill
    ↓
Synchronize Changes
    ↓
Validate
    ↓
Switch Ownership
```

---

## 18. Source of Truth

During migration:

```text
Legacy Data
Target Data
```

may coexist.

Define explicitly:

```text
Current Source of Truth
Target Source of Truth
Synchronization Direction
Conflict Resolution
Cutover Point
Rollback Strategy
```

Avoid:

```text
Legacy = Source of Truth
Target = Source of Truth
```

simultaneously unless the architecture has a very deliberate conflict-resolution model.

---

## 19. Contract Evolution

Legacy interfaces may need to coexist with new contracts.

Use:

```text
Old Contract
     +
New Contract
```

then:

```text
Migrate Consumers
```

then:

```text
Remove Old Contract
```

This is especially important for:

* APIs
* events
* message schemas
* database schemas
* configuration

---

## 20. Database Ownership

If the legacy application owns a shared database:

```text
Legacy
   ↓
Shared DB
```

the target may require:

```text
New System
   ↓
Owned DB
```

The migration must address:

```text
Data Ownership
Readers
Writers
Transactions
Foreign Keys
Reporting
ETL
Synchronization
```

Moving tables without moving ownership does not complete the replacement.

---

## 21. Consistency Changes

Replacing a legacy system may change consistency boundaries.

Before:

```text
One Database Transaction
```

After:

```text
Service A
   ↓
Event
   ↓
Service B
```

The target may require:

* eventual consistency
* Saga
* compensation
* idempotency
* reconciliation

Do not accidentally change business consistency guarantees without understanding the impact.

---

## 22. Cutover Strategies

Possible strategies include:

### Big-Bang Cutover

```text
100% Legacy
     ↓
100% New
```

Simple operational model but larger blast radius.

---

### Gradual Traffic Migration

```text
100% Legacy
     ↓
90 / 10
     ↓
50 / 50
     ↓
10 / 90
     ↓
100% New
```

---

### Capability Cutover

```text
Orders → New
Payments → Legacy
Customers → Legacy
```

Then:

```text
Orders → New
Payments → New
Customers → Legacy
```

---

### Tenant-Based Migration

```text
Tenant A → New
Tenant B → Legacy
Tenant C → Legacy
```

Useful when workloads can be isolated.

---

## 23. Rollback

Rollback should be designed before cutover.

Before switching:

```text
Legacy ✓
New ✓
```

After a controlled cutover:

```text
New = Active
Legacy = Fallback
```

Rollback may become difficult if:

```text
New Writes
     ↓
New Data
```

cannot be synchronized back to the legacy system.

Therefore define:

```text
Rollback Window
Data Synchronization
Write Ownership
State Reconciliation
```

---

## 24. Legacy Freeze

Once migration begins, avoid adding unnecessary new functionality to the legacy system.

Possible policy:

```text
Legacy
  │
  ├── Critical Fixes ✓
  ├── Security Fixes ✓
  └── New Features ✗
```

This prevents the target architecture from constantly moving further away.

A legacy freeze should still allow changes required for:

* security
* compliance
* critical defects
* operational stability
* migration support

---

## 25. Common Failure Modes

### Big-Bang Rewrite

Everything is rebuilt before real production validation.

---

### Rewrite Without Discovery

The team assumes the existing system behaves exactly as documented.

---

### Legacy Behavior Leakage

The new architecture simply reproduces legacy structures and limitations.

---

### Permanent Compatibility Layer

The migration boundary remains indefinitely.

---

### Dual-System Forever

Both systems continue receiving full development investment.

---

### Dual Ownership

Both old and new systems write authoritative versions of the same data.

---

### Data Migration Last

The application is rebuilt but the data strategy is postponed.

---

### Hidden Consumers

Unknown applications still depend on legacy APIs or database tables.

---

### Scope Explosion

A replacement project becomes an opportunity to redesign every business rule simultaneously.

---

### No Retirement Plan

The new system becomes operational while the old system continues consuming resources indefinitely.

---

### No Observability

The team cannot determine which functionality is still using legacy behavior.

---

### False Success

The new system works technically but business behavior differs from the legacy system.

---

## 26. Architectural Invariants

A controlled legacy replacement should enforce:

1. Legacy behavior is understood before replacement.
2. Migration boundaries are explicit.
3. New components do not create unnecessary dependencies on legacy internals.
4. Data ownership is explicit.
5. Source of truth is defined.
6. Compatibility is intentional and measurable.
7. New and legacy implementations can be validated independently.
8. Migration progress is observable.
9. Rollback requirements are understood.
10. Legacy usage decreases over time.
11. New development in legacy is controlled.
12. Temporary migration mechanisms have removal conditions.
13. The target architecture does not reproduce unnecessary legacy coupling.
14. Legacy components have explicit retirement criteria.
15. The final architecture has no operational dependency on the retired system.

---

## 27. Testing Strategy

Use multiple layers:

```text
Characterization Tests
        ↓
Contract Tests
        ↓
Migration Tests
        ↓
Compatibility Tests
        ↓
Integration Tests
        ↓
Shadow Validation
        ↓
Performance Tests
        ↓
Production Verification
```

### Characterization Tests

Capture current legacy behavior.

### Contract Tests

Ensure old consumers remain compatible.

### Comparison Tests

Run legacy and new implementations against identical inputs.

### Migration Tests

Verify data and state transitions.

### Failure Tests

Test:

```text
Legacy Failure
New Failure
Migration Failure
Synchronization Failure
Rollback
Partial Cutover
```

---

## 28. Observability

Track both systems during migration.

Important metrics:

```text
Legacy Requests
New Requests
Legacy Errors
New Errors
Legacy Latency
New Latency
Migration Progress
Data Mismatches
Synchronization Lag
Rollback Events
Legacy Database Access
Compatibility Layer Usage
```

A particularly important metric is:

```text
legacy_usage_total
```

The long-term trend should be:

```text
Legacy Usage ↓
New Usage ↑
```

Also track:

```text
legacy_dependency_total
```

to identify remaining architectural coupling.

---

## 29. Security

Legacy replacement is an opportunity to establish stronger security boundaries.

Review:

* authentication
* authorization
* service identity
* database credentials
* secrets
* encryption
* network access
* audit logging
* privileged operations
* legacy accounts
* unused credentials

Do not allow the new system to inherit unnecessary legacy privileges simply for migration convenience.

---

## 30. Performance

Do not assume the new architecture is automatically faster.

Compare:

```text
Latency
Throughput
CPU
Memory
Database Load
Network Traffic
Concurrency
Error Rate
```

Also measure business-level performance:

```text
Order Processing Time
Payment Processing Time
Search Completion Time
Workflow Completion Time
```

The target architecture should satisfy its defined performance requirements.

---

## 31. .NET Implementation

Useful technologies include:

* ASP.NET Core
* .NET 10
* EF Core
* Dapper
* gRPC
* REST
* YARP
* HttpClientFactory
* resilience pipelines
* Kafka
* RabbitMQ
* MassTransit
* BackgroundService
* OpenTelemetry
* Aspire
* Docker
* Kubernetes
* feature flags
* architecture testing

Common patterns:

```text
Adapter
Facade
Anti-Corruption Layer
Branch by Abstraction
Strangler Fig
Outbox
CDC
Feature Flags
Shadow Traffic
```

---

## 32. Migration State Machine

Treat the replacement as an explicit state machine:

```text
Discovery
    ↓
Characterization
    ↓
TargetDefined
    ↓
MigrationBoundaryCreated
    ↓
NewImplementationReady
    ↓
DataMigrated
    ↓
ShadowValidation
    ↓
PartialCutover
    ↓
FullCutover
    ↓
LegacyRetirement
    ↓
Completed
```

Failure:

```text
       ┌─────────┐
       │ Failed  │
       └────┬────┘
            │
       Recover / Rollback
            │
            ▼
      Previous State
```

---

## 33. Retirement Criteria

The legacy system should not be removed simply because the new system exists.

Define explicit criteria.

For example:

```text
Legacy Traffic = 0
Legacy Writes = 0
Legacy Data Dependencies = 0
Legacy API Consumers = 0
Legacy Jobs = 0
Legacy Integrations = 0
Rollback Requirement Completed
Data Validation Complete
Operational Ownership Transferred
```

Only then should retirement begin.

---

## 34. Production Checklist

### Discovery

* [ ] Identify capabilities.
* [ ] Identify consumers.
* [ ] Identify data.
* [ ] Identify integrations.
* [ ] Identify undocumented behavior.
* [ ] Identify operational dependencies.
* [ ] Create characterization tests.

### Target Design

* [ ] Define target architecture.
* [ ] Define migration boundaries.
* [ ] Define data ownership.
* [ ] Define contracts.
* [ ] Define consistency requirements.
* [ ] Define observability.
* [ ] Define rollback.

### Migration

* [ ] Build new capability.
* [ ] Create compatibility layer.
* [ ] Migrate data.
* [ ] Synchronize data.
* [ ] Validate behavior.
* [ ] Run shadow traffic where appropriate.
* [ ] Migrate consumers.
* [ ] Monitor legacy usage.

### Cutover

* [ ] Define cutover criteria.
* [ ] Define rollback criteria.
* [ ] Migrate traffic gradually where appropriate.
* [ ] Validate business behavior.
* [ ] Monitor performance.
* [ ] Monitor errors.

### Retirement

* [ ] Legacy traffic = 0.
* [ ] Legacy writes = 0.
* [ ] Legacy consumers = 0.
* [ ] Legacy data dependencies = 0.
* [ ] Temporary migration mechanisms removed.
* [ ] Legacy credentials removed.
* [ ] Legacy infrastructure decommissioned.
* [ ] Architecture tests prevent regression.

---

## 35. Lab Experiment

Build a realistic legacy system:

```text
Legacy Order Management System
        │
        ├── Orders
        ├── Customers
        ├── Payments
        ├── Reporting
        └── Shared Database
```

The target architecture is:

```text
Order Service
Customer Service
Payment Service
Reporting Pipeline
Owned Data
Event-Based Integration
```

### Phase 1: Characterize

Document:

```text
APIs
Database Tables
Business Rules
Workflows
Integrations
Failure Behavior
Performance
```

Create characterization tests.

### Phase 2: Define Boundaries

Identify:

```text
Orders
Payments
Customers
Reporting
```

as candidate capabilities.

### Phase 3: Introduce Migration Boundary

Create:

```text
API / Facade / Router
```

between consumers and the legacy implementation.

### Phase 4: Replace One Capability

Move:

```text
Orders
```

to the new implementation.

### Phase 5: Migrate Data

Create:

```text
Orders DB
```

and perform:

```text
Backfill
Synchronization
Validation
```

### Phase 6: Shadow

Run selected traffic through:

```text
Legacy Orders
New Orders
```

and compare results.

### Phase 7: Cut Over

Move traffic gradually.

### Phase 8: Failure Experiments

Test:

```text
New Service Failure
Legacy Failure
Data Synchronization Failure
Network Failure
Partial Cutover
Rollback
Duplicate Processing
```

### Phase 9: Repeat

Migrate:

```text
Payments
Customers
Reporting
```

one capability at a time.

### Phase 10: Retire

Remove:

```text
Legacy APIs
Legacy Database Access
Legacy Jobs
Legacy Integrations
Legacy Infrastructure
```

---

## 36. Success Criteria

A successful replacement looks like:

```text
Legacy System
████████████████████
        ↓
Capability Migration
██████████████
        ↓
████████ New █████
██████ Legacy ███
        ↓
████████████ New █
██ Legacy ██
        ↓
New System
████████████████████
```

The final state should have:

```text
Clear Ownership
Explicit Contracts
Independent Evolution
Observable Boundaries
No Legacy Dependency
```

---

## 37. Key Takeaways

* Legacy replacement is an architectural transition, not simply a rewrite.
* Understand actual behavior before replacing it.
* Characterization tests are valuable when documentation and tests are incomplete.
* Separate behavior preservation from business improvement.
* Use migration seams to isolate old and new implementations.
* Strangler Fig, Branch by Abstraction, Parallel Change, and Database Migration can all be combined during replacement.
* Data migration must be designed together with application migration.
* Define ownership and source of truth explicitly.
* Compatibility layers should have measurable removal conditions.
* Shadow execution can validate new behavior before cutover.
* Cut over incrementally where the architecture allows it.
* Design rollback before migration becomes irreversible.
* Track legacy usage throughout the migration.
* Retirement is part of the architecture, not an optional cleanup task.
* A replacement is not complete while the old system remains an operational dependency.

> **The goal is not to build a new system beside the old one. The goal is to progressively transfer responsibility from the old architecture to the new one until the legacy system can safely disappear.**

