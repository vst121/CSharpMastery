# Legacy Modernization

## 1. Case Study Overview

This case study describes the modernization of a business-critical financial transaction system.

The existing system is:

- written in .NET Framework 4.8
- primarily a monolithic backend
- tightly coupled to the frontend
- based on legacy application and integration patterns
- difficult to evolve independently
- deployed as a relatively large application unit

The target direction is:

- modernize the backend to .NET 10
- separate frontend and backend responsibilities
- introduce a modern API boundary
- evaluate Blazor and React as frontend technologies
- preserve existing business behavior and transaction integrity
- modernize incrementally rather than performing a big-bang rewrite
- establish stronger observability, testing, security, and deployment practices
- create a foundation for future architectural evolution

The central architectural question is:

> **How can we modernize a business-critical financial transaction system from .NET Framework 4.8 to .NET 10 while separating frontend and backend concerns without introducing unacceptable business, data, operational, or migration risk?**

---

# 2. Modernization Context

The system has been running for many years and contains important business knowledge.

Some business rules are:

- documented
- partially documented
- encoded directly in application code
- encoded in database logic
- encoded in integrations
- known only by experienced engineers or business users

The system therefore cannot be treated as a simple technology migration.

The modernization has two dimensions:

```text
                    Legacy System
                         │
             ┌───────────┴───────────┐
             │                       │
             ▼                       ▼
       Backend Modernization   Frontend Separation
             │                       │
             ▼                       ▼
         .NET 10              Modern Frontend
                                     │
                              ┌──────┴──────┐
                              ▼             ▼
                           Blazor        React
```

These dimensions should be coordinated but not unnecessarily coupled.

---

# 3. Explicit Assumptions

The following assumptions define the case study.

They are assumptions, not verified facts about a real production system.

They should therefore be revisited during the discovery phase.

---

## 3.1 Business Domain

The system processes financial transactions.

Examples include:

- transaction creation
- transaction validation
- transaction authorization
- transaction processing
- transaction status tracking
- payment-related operations
- transaction history
- audit information
- customer/account information
- reporting

The exact financial domain is intentionally abstracted.

The architectural principles should apply to transaction-heavy financial systems without depending on a specific banking or payment product.

---

## 3.2 Existing Backend

The existing backend is implemented primarily using:

```text
.NET Framework 4.8
```

The application is assumed to contain:

- ASP.NET-based backend functionality
- business logic
- data access
- integration logic
- authentication/authorization logic
- transaction processing
- validation
- background or scheduled processing
- legacy infrastructure dependencies

Some components may use older .NET Framework-compatible libraries that cannot be directly moved to .NET 10.

---

## 3.3 Existing Frontend

The existing frontend is tightly coupled to the backend.

The exact technology is assumed to be a legacy web UI running as part of the existing application.

The modernization goal is to establish:

```text
Frontend
    │
    ▼
HTTP API
    │
    ▼
Backend
    │
    ▼
Domain/Application
    │
    ▼
Data
```

The frontend should no longer depend directly on:

- backend implementation details
- internal domain objects
- database structures
- internal service classes
- server-side implementation details that bypass an explicit API contract

---

# 4. Target Backend

The target backend platform is:

```text
.NET 10
```

The modernization should take advantage of the modern .NET platform where justified, including:

- modern ASP.NET Core
- dependency injection
- modern configuration
- structured logging
- OpenTelemetry
- modern authentication/authorization
- health checks
- resilient HTTP communication
- asynchronous programming
- containerization where appropriate
- modern testing infrastructure
- modern deployment practices

However:

> **The goal is not to rewrite everything simply because .NET 10 exists.**

The migration should be driven by architectural and business needs.

---

# 5. Frontend Decision

The frontend will be separated from the backend.

Two primary candidates are being considered:

```text
Option A
.NET 10 Backend
      +
Blazor

Option B
.NET 10 Backend
      +
React
```

At this stage, neither option is considered the predetermined answer.

The decision should consider:

- team skills
- application complexity
- frontend ecosystem
- component ecosystem
- maintainability
- testing
- performance
- accessibility
- developer productivity
- hiring/skills availability
- integration requirements
- long-term ownership
- deployment model
- security
- observability
- API contract maturity
- future product direction

The repository should contain a separate architecture experiment for this decision.

---

# 6. Frontend Architectural Principle

Regardless of whether Blazor or React is selected:

> **The frontend must depend on an explicit backend contract, not on backend implementation details.**

The desired boundary is:

```text
┌─────────────────────┐
│      Frontend       │
│                     │
│ Blazor OR React     │
└──────────┬──────────┘
           │
           │ HTTP / API Contract
           ▼
┌─────────────────────┐
│      Backend        │
│                     │
│ ASP.NET Core/.NET 10│
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│ Application / Domain│
└──────────┬──────────┘
           │
           ▼
┌─────────────────────┐
│       Data          │
└─────────────────────┘
```

This boundary is more important than the frontend technology itself.

---

# 7. Existing Architecture Assumption

The current system is assumed to resemble:

```text
                Browser
                   │
                   ▼
        ┌─────────────────────┐
        │ Legacy Web App      │
        │                     │
        │ UI                  │
        │ Controllers         │
        │ Business Logic      │
        │ Data Access         │
        │ Integrations        │
        └──────────┬──────────┘
                   │
                   ▼
             Legacy Database
```

In practice, the boundaries may not be this clean.

There may be:

- controllers calling repositories directly
- UI code depending on domain objects
- business rules in controllers
- business rules in SQL
- shared utility classes
- static dependencies
- global configuration
- database triggers
- stored procedures
- hidden integration behavior

These dependencies must be discovered rather than assumed away.

---

# 8. Target Architecture Assumption

The desired direction is:

```text
                    Browser
                       │
                       ▼
              ┌─────────────────┐
              │ Modern Frontend │
              │                 │
              │ Blazor / React  │
              └────────┬────────┘
                       │
                       │ HTTPS / API
                       ▼
              ┌─────────────────┐
              │ ASP.NET Core    │
              │ .NET 10 API     │
              └────────┬────────┘
                       │
              ┌────────┴────────┐
              ▼                 ▼
       Application Layer    Integration
              │                 │
              ▼                 ▼
        Domain Logic       External Systems
              │
              ▼
             Data
```

The exact internal architecture remains a design decision.

Possible structures include:

- modular monolith
- layered architecture
- ports and adapters
- vertical slices
- domain-oriented modules
- selective service extraction

The system should not automatically become microservices.

---

# 9. Why a Modular Monolith Is a Valid Target

The first modernization target does not need to be a distributed system.

A possible intermediate architecture is:

```text
                  .NET 10 Application
                         │
        ┌────────────────┼────────────────┐
        │                │                │
        ▼                ▼                ▼
 Transaction        Account          Reporting
 Module             Module           Module
        │                │                │
        └────────────────┼────────────────┘
                         ▼
                       Data
```

This can provide:

- explicit boundaries
- controlled dependencies
- improved testability
- independent module ownership
- easier evolution

without immediately introducing:

- network failures
- distributed transactions
- service discovery
- distributed tracing complexity
- eventual consistency everywhere
- operational overhead

A future service extraction can then be based on evidence.

---

# 10. Migration Strategy

The modernization should be incremental.

The preferred conceptual flow is:

```text
Current System
      │
      ▼
Understand
      │
      ▼
Characterize
      │
      ▼
Establish Boundaries
      │
      ▼
Introduce API Boundary
      │
      ▼
Modernize Backend
      │
      ▼
Separate Frontend
      │
      ▼
Migrate Capabilities
      │
      ▼
Validate
      │
      ▼
Remove Legacy Dependencies
```

The migration architecture is allowed to be temporary.

---

# 11. Migration Architecture

During migration:

```text
                         Browser
                            │
                            ▼
                    Frontend Boundary
                            │
                            ▼
                     API / Routing
                      /          \
                     /            \
                    ▼              ▼
             Legacy Backend     .NET 10 Backend
                    │              │
                    │              │
                    └──────┬───────┘
                           ▼
                     Data Migration
```

The routing strategy may evolve over time.

Initially:

```text
100% Legacy
```

Then:

```text
90% Legacy
10% New
```

Then:

```text
50% Legacy
50% New
```

Eventually:

```text
100% New
```

Traffic migration is only appropriate where the capability boundary and correctness model support it.

---

# 12. Important Constraint: Financial Correctness

The system processes financial transactions.

Therefore modernization cannot compromise critical invariants.

Examples:

```text
A transaction must have a unique identity.

A transaction must not be applied twice.

Accepted transactions must be durable.

Invalid state transitions must be rejected.

Financial amounts must preserve required precision.

Audit records must remain traceable.

Authorization must be enforced.

Transaction state must have an authoritative owner.

Migration must not silently lose transactions.

```

These are architectural invariants.

They should become automated tests wherever possible.

---

# 13. Characterization Before Migration

Before rewriting business functionality, establish what the existing system actually does.

Create:

- characterization tests
- integration tests
- API behavior snapshots
- database behavior tests
- transaction scenario tests
- error behavior tests
- reconciliation tests

Example:

```text
Legacy Input
     │
     ▼
Legacy Processing
     │
     ▼
Observed Result
     │
     ▼
Characterization Test
```

The purpose is not to prove that legacy behavior is ideal.

The purpose is to prevent accidental behavioral changes during migration.

---

# 14. Business Rules

Business rules may currently exist in:

```text
UI
Controllers
Services
Repositories
Domain Classes
Stored Procedures
Database Triggers
Batch Jobs
External Systems
```

The modernization should progressively move business rules toward explicit domain/application boundaries.

Desired direction:

```text
Frontend
   │
   ▼
API
   │
   ▼
Application
   │
   ▼
Domain
   │
   ▼
Infrastructure
```

The migration must not blindly move every existing line of code.

Business behavior must first be understood.

---

# 15. API-First Boundary

The frontend/backend separation should establish a clear API contract.

Possible API styles:

- REST
- HTTP APIs
- potentially gRPC for selected internal communication
- asynchronous events for selected integration scenarios

For the frontend boundary, REST/HTTP APIs are assumed initially unless requirements justify another approach.

The API contract should define:

- resources
- commands
- queries
- validation
- error model
- authentication
- authorization
- pagination
- filtering
- versioning
- idempotency
- correlation
- concurrency behavior

---

# 16. DTO Boundary

The frontend should not consume domain entities directly.

Use explicit API contracts:

```text
Domain Entity
      │
      ▼
Application
      │
      ▼
API DTO
      │
      ▼
Frontend
```

This protects the domain model from becoming a UI contract.

Similarly:

```text
Frontend Model
      │
      ▼
API Contract
      │
      ▼
Application Command
      │
      ▼
Domain
```

The API contract becomes an architectural boundary.

---

# 17. Frontend Architecture

The frontend should be treated as an independent architectural subsystem.

Responsibilities include:

- presentation
- navigation
- client-side state where appropriate
- validation for user experience
- API interaction
- authentication flow
- authorization-aware UI
- error presentation
- accessibility
- frontend observability

The frontend should not own authoritative business state.

For example:

```text
Frontend
   │
   ├── UI State
   ├── Form State
   └── View State

Backend
   │
   ├── Business State
   ├── Transaction State
   ├── Authorization
   └── Business Rules
```

---

# 18. Blazor Option

Blazor provides a .NET-oriented frontend development model.

Potential architectural characteristics:

- C# across backend and frontend
- .NET ecosystem alignment
- shared development language
- component-based UI
- strong integration with ASP.NET Core
- potential reuse of some .NET concepts and libraries

Questions to investigate:

- frontend performance requirements
- rendering model
- browser execution requirements
- JavaScript interoperability
- component ecosystem
- testing strategy
- accessibility
- frontend team expertise
- deployment model
- long-term maintainability

These are evaluation criteria, not predetermined conclusions.

---

# 19. React Option

React provides a JavaScript/TypeScript-oriented frontend ecosystem.

Potential architectural characteristics:

- mature component ecosystem
- TypeScript support
- broad frontend tooling
- strong separation from backend technology
- large ecosystem
- flexible frontend architecture

Questions to investigate:

- TypeScript expertise
- state management
- component standards
- API integration
- testing
- accessibility
- build/deployment model
- dependency management
- long-term maintainability

Again, these are evaluation criteria rather than a recommendation.

---

# 20. Frontend Decision Experiment

Create:

```text
ArchitectureExperiments/
└── PatternComparison/
    └── BlazorVsReact/
```

The experiment should implement the same small financial transaction UI.

For example:

```text
Transaction List
Transaction Search
Transaction Details
Create Transaction
Validation Errors
Transaction Status
```

Keep the backend contract identical.

Measure:

### Developer Experience

- implementation time
- code volume
- onboarding effort
- testing effort
- debugging effort

### Runtime

- initial load
- interaction latency
- rendering performance
- network traffic

### Maintainability

- component complexity
- dependency complexity
- coupling
- testability

### Operational

- build time
- deployment complexity
- observability
- runtime failure modes

### Team

- existing skills
- hiring considerations
- knowledge distribution

The result should become evidence for the architectural decision.

---

# 21. Backend Modernization

The .NET Framework 4.8 backend should not necessarily be rewritten in one step.

Possible strategies include:

### Strategy A: Direct Migration

Move the application toward .NET 10 while preserving most structure.

```text
.NET Framework 4.8
       │
       ▼
.NET 10
```

Useful when:

- internal structure is reasonably healthy
- dependencies are compatible
- migration risk is manageable

---

### Strategy B: Modular Migration

Create explicit modules and migrate capabilities incrementally.

```text
Legacy
  │
  ├── Capability A → .NET 10
  ├── Capability B → .NET 10
  ├── Capability C → .NET 10
  └── Capability D → Legacy
```

---

### Strategy C: Strangler

Introduce a modern API boundary and gradually replace legacy capabilities.

```text
Client
  │
  ▼
Modern Boundary
  │
  ├────► Legacy Capability
  │
  └────► Modern Capability
```

---

# 22. Avoiding the Big-Bang Rewrite

The following approach is considered high risk:

```text
.NET Framework 4.8
       │
       │ 2-3 years
       ▼
Completely new .NET 10 system
       │
       ▼
Big-bang cutover
```

Problems include:

- long feedback cycle
- requirements change during development
- hidden legacy behavior
- migration risk accumulation
- difficult validation
- difficult rollback
- organizational fatigue

A safer architectural strategy is generally:

```text
Small Boundary
     ↓
Small Migration
     ↓
Validate
     ↓
Production
     ↓
Learn
     ↓
Next Boundary
```

---

# 23. Database Assumptions

The existing database is assumed to be shared by several parts of the legacy system.

It may contain:

- transaction tables
- customer/account data
- audit information
- reporting tables
- stored procedures
- triggers
- legacy integration structures

The first migration phase should not necessarily split the database immediately.

Instead:

```text
Legacy Database
       │
       ▼
Understand Ownership
       │
       ▼
Identify Boundaries
       │
       ▼
Migrate Data Ownership
       │
       ▼
Owned Data
```

Database modernization should follow business ownership.

---

# 24. Data Migration

Possible migration mechanisms include:

- batch migration
- backfill
- CDC
- replication
- controlled dual write
- reconciliation
- phased cutover

The chosen approach depends on:

- data volume
- transaction rate
- downtime tolerance
- consistency requirements
- database capabilities
- migration window
- rollback requirements

No single mechanism is universally correct.

---

# 25. Data Ownership Target

The target should move toward:

```text
Transaction Capability
        │
        ▼
Transaction Data Owner
```

rather than:

```text
Many Applications
       │
       ▼
Shared Database
```

A database is an implementation mechanism.

It should not become the architecture's integration contract.

---

# 26. Integration Modernization

The legacy system may contain synchronous integrations.

Target architecture may introduce:

```text
.NET 10 Backend
       │
       ├── Synchronous API
       │
       └── Domain/Event Integration
                    │
                    ▼
                  Broker
```

Events should be introduced where they provide meaningful decoupling.

Potential events:

```text
TransactionCreated
TransactionValidated
TransactionAuthorized
TransactionProcessed
TransactionFailed
TransactionCompleted
```

Event contracts must be versioned and governed.

---

# 27. Transactional Outbox

If a modern transaction service updates its database and publishes an event:

```text
Database Transaction
       │
       ├── Transaction State
       │
       └── Outbox Event
                │
                ▼
          Outbox Publisher
                │
                ▼
              Broker
```

This prevents the classic failure:

```text
Database Commit
      +
Event Publication Failure
      =
Database Changed
but Event Missing
```

Consumers must still be idempotent.

---

# 28. Idempotency

The modern API should support idempotency for operations where duplicate submission is possible.

Example:

```text
POST /transactions

Idempotency-Key: abc-123
```

Processing:

```text
Request
   │
   ▼
Idempotency Check
   │
   ├── Existing → Return Existing Result
   │
   └── New
        │
        ▼
     Process
        │
        ▼
     Persist
```

The database should enforce uniqueness where appropriate.

---

# 29. Security Assumptions

The financial nature of the system means security is a first-class architectural concern.

The modernization must preserve or improve:

- authentication
- authorization
- encryption
- auditability
- secrets management
- identity management
- least privilege
- API security
- data protection

The new frontend/backend separation must not accidentally create:

```text
Browser
   ↓
Overly Powerful API
   ↓
Sensitive Operations
```

Authorization remains a backend responsibility.

---

# 30. Observability

The modernization should introduce consistent observability.

Recommended correlation information:

```text
TraceId
CorrelationId
TransactionId
RequestId
MessageId
CausationId
ApplicationVersion
```

A transaction should be traceable across:

```text
Frontend
   ↓
API
   ↓
Application
   ↓
Database
   ↓
External Integration
   ↓
Event Broker
   ↓
Consumers
```

---

# 31. Deployment Strategy

The legacy system and modern system may need to coexist.

Possible deployment model:

```text
                    Load Balancer
                         │
                         ▼
                   API Boundary
                    /        \
                   /          \
                  ▼            ▼
             Legacy App     .NET 10
                              │
                              ▼
                         New Frontend
```

Deployment should support:

- automated builds
- automated tests
- environment promotion
- health checks
- rollback or forward recovery
- deployment telemetry
- version identification

---

# 32. Compatibility Strategy

During migration, compatibility may be required at:

### API level

Old clients continue to work.

### Data level

Old and new representations coexist temporarily.

### Event level

Old and new event versions coexist.

### Authentication

Legacy and modern identity mechanisms may temporarily coexist.

### Operational level

Legacy operational procedures remain available until the new path is proven.

Compatibility is a migration mechanism, not necessarily a permanent architecture.

---

# 33. Migration Phases

A possible modernization roadmap:

```text
Phase 0
Discovery
   ↓
Phase 1
Characterization
   ↓
Phase 2
API Boundary
   ↓
Phase 3
.NET 10 Foundation
   ↓
Phase 4
Frontend Separation
   ↓
Phase 5
Capability Migration
   ↓
Phase 6
Data Ownership Migration
   ↓
Phase 7
Integration Modernization
   ↓
Phase 8
Legacy Decommissioning
```

The phases may overlap.

They should not be interpreted as a mandatory waterfall sequence.

---

# 34. Phase 0: Discovery

Activities:

- system inventory
- dependency mapping
- code analysis
- database analysis
- integration inventory
- deployment analysis
- operational analysis
- business capability mapping
- stakeholder interviews

Deliverables:

```text
System Map
Dependency Map
Data Map
Capability Map
Risk Register
Architecture Constraints
```

---

# 35. Phase 1: Characterization

Build:

- characterization tests
- API tests
- transaction scenario tests
- reconciliation mechanisms
- performance baseline
- production telemetry baseline

Goal:

> Establish observable evidence of current behavior.

---

# 36. Phase 2: API Boundary

Introduce:

```text
Frontend
    │
    ▼
Explicit API
    │
    ▼
Legacy Backend
```

Initially the API may delegate to legacy implementation.

This creates the architectural seam before replacing the implementation.

This is an important architectural move.

---

# 37. Phase 3: .NET 10 Foundation

Create the modern backend foundation:

```text
.NET 10
ASP.NET Core
Dependency Injection
Configuration
Logging
OpenTelemetry
Health Checks
Authentication
Authorization
Testing
CI/CD
```

The foundation should not become a giant platform project.

Build only what is required by the first migrated capability.

---

# 38. Phase 4: Frontend Separation

The new frontend consumes the API.

Possible architecture:

```text
                 Frontend
              /           \
          Blazor          React
              \           /
               \         /
                ▼       ▼
                  API
                   │
                   ▼
                .NET 10
```

Only one frontend should become the primary production implementation after the decision process.

During experimentation, both can temporarily exist.

---

# 39. Phase 5: Capability Migration

Migrate one business capability at a time.

For example:

```text
Transaction Search
        ↓
Transaction Details
        ↓
Transaction Validation
        ↓
Transaction Processing
        ↓
Transaction Status
        ↓
Audit
```

Each migration should have:

- boundary
- owner
- tests
- telemetry
- migration plan
- rollback/recovery strategy
- success criteria

---

# 40. Phase 6: Data Ownership Migration

After capability boundaries become clear:

```text
Shared Data
    ↓
Identify Owner
    ↓
Create Owned Model
    ↓
Migrate Data
    ↓
Validate
    ↓
Redirect Consumers
    ↓
Remove Legacy Access
```

The goal is not to create databases for architectural fashion.

The goal is to create explicit ownership.

---

# 41. Phase 7: Integration Modernization

Modernize selected integrations.

Possible sequence:

```text
Legacy Synchronous Integration
          ↓
Compatibility Layer
          ↓
Modern API/Event Contract
          ↓
Event-Based Integration
```

Use asynchronous integration where temporal decoupling and independent evolution provide meaningful value.

---

# 42. Phase 8: Legacy Decommissioning

A legacy component can be removed only when:

```text
All Consumers Migrated
        +
All Data Dependencies Removed
        +
All Integrations Migrated
        +
Traffic = 0
        +
Reconciliation Complete
        +
Operational Ownership Transferred
        +
Retention Requirements Satisfied
```

Then:

```text
Disable
   ↓
Observe
   ↓
Confirm
   ↓
Remove
```

---

# 43. Failure Scenarios

The modernization must explicitly test:

### Backend failure

.NET 10 service unavailable.

### Frontend failure

Frontend cannot reach API.

### API timeout

Request reaches backend but response is lost.

### Duplicate request

Client retries transaction submission.

### Database failure

Database becomes unavailable during transaction processing.

### Event publication failure

Transaction succeeds but event publication initially fails.

### Migration mismatch

Legacy and modern implementations produce different results.

### Data synchronization failure

Legacy and modern datasets diverge.

### Partial deployment

Some instances run old code while others run new code.

### Rollback after state mutation

New implementation has already created state before migration is stopped.

---

# 44. Migration Failure Example

Consider:

```text
Frontend
   │
   ▼
.NET 10 API
   │
   ▼
Transaction Service
   │
   ▼
Database Commit
   │
   X
   │
   X Response lost
```

The client does not know whether the transaction succeeded.

It retries.

Without idempotency:

```text
Transaction
      +
Retry
      =
Duplicate Processing
```

Therefore:

> Timeout handling and idempotency are architectural concerns, not merely frontend concerns.

---

# 45. Architecture Invariants

The modernization should enforce:

```text
1. Frontend cannot access the database directly.

2. Frontend cannot depend on domain implementation types.

3. Business state is authoritative in the backend.

4. Critical financial invariants are enforced server-side.

5. Transaction operations are idempotent where required.

6. Every important dataset has an explicit owner.

7. Services do not directly access another service's database.

8. API contracts are versioned and tested.

9. Events have explicit schemas and ownership.

10. Critical operations are observable.

11. Authentication and authorization are enforced at the backend boundary.

12. Migration steps have explicit recovery strategies.

13. Legacy dependencies are measurable.

14. Temporary compatibility mechanisms have retirement criteria.

15. New architecture must not silently change critical financial behavior.
```

---

# 46. Architecture Smells

Watch for:

### Technology-first migration

"We need .NET 10" becomes the primary architectural argument.

### Frontend-driven domain logic

Business rules move into React or Blazor.

### API as pass-through layer

The API simply exposes database tables.

### Shared database survives indefinitely

The new architecture still depends heavily on the old database.

### Big-bang rewrite

Years of development happen before production validation.

### Distributed monolith

Capabilities become services but remain tightly coupled.

### Dual-write without reconciliation

Two data stores are updated independently without a consistency strategy.

### Permanent compatibility layer

Temporary migration infrastructure becomes permanent.

### No retirement plan

The new system is added but the old system never disappears.

---

# 47. Metrics

Measure modernization using multiple dimensions.

## Business

- transaction success rate
- business error rate
- reconciliation differences
- customer-impacting incidents

## Technical

- deployment frequency
- lead time for changes
- API latency
- throughput
- error rate
- resource utilization

## Architecture

- coupling
- dependency count
- shared database dependencies
- deployment independence
- module ownership
- legacy dependency count

## Migration

- migrated capabilities
- migrated traffic
- migrated data
- remaining consumers
- remaining legacy dependencies
- compatibility components remaining

## Operations

- incidents
- MTTR
- deployment failure rate
- rollback/recovery frequency

---

# 48. Frontend Decision Criteria

The Blazor vs React decision should use explicit criteria.

| Dimension              | Questions                                              |
| ---------------------- | ------------------------------------------------------ |
| Team Skills            | Which skills already exist?                            |
| Hiring                 | What skills are realistically available?               |
| Application Complexity | What UI complexity is expected?                        |
| Ecosystem              | Which ecosystem capabilities are required?             |
| Performance            | What are the actual UI performance requirements?       |
| Testing                | How will unit, integration, and E2E testing work?      |
| Accessibility          | What accessibility requirements exist?                 |
| Maintainability        | How will the application evolve over 5+ years?         |
| API Integration        | How cleanly does the frontend consume the API?         |
| Deployment             | What deployment model is required?                     |
| Observability          | How will frontend failures be diagnosed?               |
| Security               | How will authentication and authorization work?        |
| Developer Experience   | How quickly can engineers build and maintain features? |
| Architecture Fit       | Does the choice support the target architecture?       |
| Organizational Fit     | Does the choice align with team ownership?             |

The decision should be based on the actual constraints of the organization.

---

# 49. Important Separation of Decisions

Do not combine these into one decision:

```text
.NET 10
   +
Blazor
   +
Microservices
   +
Kubernetes
   +
Kafka
   +
Cloud
```

Each solves a different architectural problem.

Instead:

```text
Backend Platform
      ↓
.NET 10

Frontend Technology
      ↓
Blazor OR React

Application Structure
      ↓
Modular Monolith initially?

Integration
      ↓
Sync API / Events

Deployment
      ↓
Cloud / Containers / App Platform

Data
      ↓
Ownership + Storage Strategy
```

This reduces architectural coupling between technology decisions.

---

# 50. Recommended Architecture Decision Records

Create ADRs such as:

```text
ADR-001 Modernization Strategy

ADR-002 .NET Framework 4.8 to .NET 10 Migration Strategy

ADR-003 Frontend/Backend Separation

ADR-004 Frontend Technology Selection

ADR-005 API Architecture

ADR-006 Modular Backend Architecture

ADR-007 Transaction Data Ownership

ADR-008 Database Migration Strategy

ADR-009 Transaction Idempotency

ADR-010 Transactional Outbox

ADR-011 Integration Modernization

ADR-012 Observability Strategy

ADR-013 Deployment and Release Strategy

ADR-014 Legacy Decommissioning Criteria
```

The important point is:

> The ADR for frontend technology should be based on evidence from the Blazor vs React experiment, not personal preference.

---

# 51. Recommended Architecture Experiments

Create experiments for:

```text
01-Characterization
02-DotNet48VsDotNet10
03-BlazorVsReact
04-APICompatibility
05-LegacyVsModernBehavior
06-SharedDatabaseVsOwnedData
07-DualWriteVsOutbox
08-SynchronousVsEventDrivenIntegration
09-ShadowTraffic
10-CanaryMigration
11-DataReconciliation
12-MigrationRollback
13-FailureRecovery
14-PerformanceBaseline
```

Each experiment follows:

```text
Question
   ↓
Hypothesis
   ↓
Baseline
   ↓
Implementation
   ↓
Measurement
   ↓
Failure Injection
   ↓
Results
   ↓
Trade-offs
   ↓
Decision
```

---

# 52. Suggested Repository Structure

The case study itself can eventually evolve into:

```text
ArchitectureCaseStudies/
└── 12-LegacyModernization/
    ├── README.md
    │
    ├── docs/
    │   ├── current-architecture.md
    │   ├── target-architecture.md
    │   ├── migration-architecture.md
    │   └── migration-roadmap.md
    │
    ├── adr/
    │   ├── ADR-001-modernization-strategy.md
    │   ├── ADR-002-dotnet10.md
    │   ├── ADR-003-frontend-separation.md
    │   └── ADR-004-frontend-choice.md
    │
    ├── experiments/
    │   ├── DotNet48VsDotNet10/
    │   ├── BlazorVsReact/
    │   ├── ShadowTraffic/
    │   └── DataReconciliation/
    │
    ├── tests/
    │   ├── CharacterizationTests/
    │   ├── ContractTests/
    │   ├── ArchitectureTests/
    │   └── MigrationTests/
    │
    └── results/
        ├── performance/
        ├── compatibility/
        ├── reconciliation/
        └── migration/
```

---

# 53. Architecture Reasoning Framework

When approaching this modernization, the architect should reason in this order:

```text
Business
   ↓
Existing Behavior
   ↓
Constraints
   ↓
Quality Attributes
   ↓
Business Capabilities
   ↓
Dependencies
   ↓
Data Ownership
   ↓
Boundaries
   ↓
Target Architecture
   ↓
Migration Architecture
   ↓
Technology Decisions
   ↓
Experiments
   ↓
Evidence
   ↓
Implementation
   ↓
Production
   ↓
Feedback
   ↓
Evolution
```

Notice that technology appears relatively late.

This is intentional.

---

# 54. What We Know vs What We Assume

A senior architect should explicitly separate facts from assumptions.

### Known for this case study

- Existing backend uses .NET Framework 4.8.
- Target backend is .NET 10.
- Frontend/backend separation is required.
- Blazor and React are candidate frontend technologies.
- The system processes financial transactions.
- Transaction correctness is critical.
- Modernization must preserve required business behavior.

### Assumptions

- The current system is significantly coupled.
- The frontend is coupled to backend implementation details.
- The database is shared.
- Some business rules are poorly documented.
- The system contains legacy integrations.
- Incremental modernization is preferred over a big-bang replacement.
- Existing functionality cannot simply be switched off.
- Characterization testing will be necessary.
- Data migration will be a significant concern.

### Questions to validate

- What is the actual frontend technology?
- What ASP.NET architecture is currently used?
- Which .NET Framework dependencies block migration?
- Which third-party libraries are incompatible with .NET 10?
- What database technology and version are used?
- How large is the database?
- What is the transaction volume?
- What are P95/P99 latency requirements?
- What availability is required?
- What are the RPO/RTO requirements?
- Which integrations exist?
- Which business rules are implemented in SQL?
- Which systems access the database directly?
- Which reports depend on legacy tables?
- What authentication mechanism exists?
- What compliance requirements apply?
- What frontend skills exist in the team?
- What are the actual UI requirements?
- What is the expected lifetime of the new frontend?
- What cloud/deployment environment will be used?

These questions should become part of the discovery phase.

---

# 55. Senior Architect Decision Point

At the beginning of the project, the architect should **not** say:

> "We will rewrite the application in .NET 10 with React."

A stronger architectural statement is:

> "We will establish an explicit frontend/backend boundary, characterize critical legacy behavior, migrate the backend incrementally toward .NET 10, and evaluate the frontend technology independently based on documented requirements and measured evidence."

This keeps the architecture decision-driven rather than technology-driven.

---

# 56. Final Target Direction

The intended architectural evolution is:

```text
CURRENT
────────────────────────────────────

Browser
   │
   ▼
.NET Framework 4.8 Application
   │
   ├── UI
   ├── Business Logic
   ├── Data Access
   ├── Integrations
   └── Legacy Infrastructure
            │
            ▼
       Shared Database


                │
                │
                ▼

TRANSITION
────────────────────────────────────

Browser
   │
   ▼
Modern Frontend
   │
   ▼
API Boundary
   │
   ├──────────────► Legacy Backend
   │
   └──────────────► .NET 10 Backend
                           │
                           ▼
                     Modern Modules
                           │
                           ▼
                    Controlled Data Access


                │
                │
                ▼

TARGET
────────────────────────────────────

Browser
   │
   ▼
Blazor OR React
   │
   ▼
ASP.NET Core / .NET 10
   │
   ▼
Modular Application
   │
   ├── Transaction
   ├── Account
   ├── Payment
   ├── Audit
   └── Reporting
   │
   ▼
Owned Data
   │
   ├── APIs
   └── Events
```

The target architecture should remain intentionally open where evidence is not yet available.

---

# 57. Final Architect Principle

The modernization should not be defined as:

```text
.NET Framework 4.8 → .NET 10
```

That is a technology migration.

The real architectural transformation is:

```text
Tightly Coupled
       ↓
Explicit Boundaries
       ↓
Independent Evolution
       ↓
Observable Components
       ↓
Controlled Data Ownership
       ↓
Safe Deployment
       ↓
Continuous Evolution
```

The .NET 10 migration is an important enabler.

The frontend separation is an important architectural boundary.

Blazor vs React is an important technology decision.

But none of these is the ultimate goal.

The ultimate goal is:

> **Create a system that can evolve safely, independently, observably, and predictably while preserving the financial invariants and business capabilities that the organization depends on.**

---

# 58. Case Study Completion Criteria

This case study is complete when the architect can demonstrate that they can:

- understand the existing legacy architecture
- distinguish facts from assumptions
- discover undocumented dependencies
- characterize legacy behavior
- identify business capabilities
- establish architectural boundaries
- separate frontend and backend responsibilities
- design a .NET 10 modernization strategy
- evaluate Blazor and React objectively
- define API contracts
- protect financial invariants
- design data migration
- establish data ownership
- handle compatibility
- introduce events safely
- design idempotency
- use transactional outbox where appropriate
- design observability
- handle partial failure
- define migration recovery
- execute incremental migration
- measure migration progress
- govern architectural decisions
- define legacy retirement criteria
- prove that modernization improved the system rather than merely changing its technology

---

# 59. Final Mental Model

```text
             LEGACY MODERNIZATION
                     │
                     ▼
              Understand First
                     │
                     ▼
               Characterize
                     │
                     ▼
             Find Boundaries
                     │
                     ▼
             Define Invariants
                     │
                     ▼
             Design Target State
                     │
                     ▼
            Design Migration Path
                     │
          ┌──────────┼──────────┐
          ▼          ▼          ▼
       Backend    Frontend     Data
       .NET 10   Blazor/React  Ownership
          │          │          │
          └──────────┼──────────┘
                     ▼
                 Experiment
                     │
                     ▼
                  Measure
                     │
                     ▼
                 Migrate
                     │
                     ▼
                 Validate
                     │
                     ▼
             Remove Legacy
                     │
                     ▼
              Evolve Further
```

## Core principle

> **Modernization is not the replacement of old technology with new technology. It is the controlled reduction of architectural constraints that prevent the system from evolving.**

````

### One important next step

I would **not decide Blazor vs React yet**.

This case study now gives us the right foundation to create a dedicated experiment:

```text
ArchitectureExperiments/
└── PatternComparison/
    └── BlazorVsReact/
````

And separately:

```text
ArchitectureExperiments/
└── Performance/
    └── DotNetFramework48VsDotNet10/
```

That gives us a very strong architect-level sequence:

**Legacy system → assumptions → characterization → migration architecture → .NET 10 experiment → Blazor vs React experiment → ADRs → incremental implementation.**
