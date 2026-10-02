# 02 - Modular Monolith to Microservices

## 1. Essence

**Modular Monolith → Microservices** is the evolution of a well-structured modular monolith into independently deployable services.

The difficult part is not creating APIs or containers.

The difficult part is creating **real independence**:

- independent deployment
- independent data ownership
- independent scaling
- explicit contracts
- failure isolation
- autonomous change

> **Extract a service only when independence creates real value.**

---

## 2. The Problem

A modular monolith may already have strong boundaries:

```text
┌──────────────────────────────────────┐
│          Modular Monolith            │
│                                      │
│  ┌────────┐ ┌────────┐ ┌─────────┐ │
│  │ Orders │ │Payment │ │Inventory│ │
│  └────────┘ └────────┘ └─────────┘ │
│                                      │
│          One Deployment              │
│          One Runtime                 │
│          Often One Database          │
└──────────────────────────────────────┘
````

Over time, real requirements may appear:

* Orders must scale independently.
* Payments require stronger isolation.
* Different teams need independent deployments.
* A capability needs a different technology.
* One component has very different availability requirements.
* Deployment frequency differs significantly.
* Fault isolation becomes important.

The architecture may then evolve toward:

```text
┌────────────┐      ┌────────────┐
│   Orders   │ ───→ │  Payments  │
│  Service   │      │  Service   │
└────────────┘      └────────────┘
      │
      ▼
┌────────────┐
│ Inventory  │
│  Service   │
└────────────┘
```

---

## 3. Intent

Extract selected modules from the modular monolith into independently deployable services while preserving system behavior.

The goal is:

```text
Module
   ↓
Service Boundary
   ↓
Independent Runtime
   ↓
Independent Deployment
   ↓
Independent Data Ownership
```

Not:

```text
Module
   ↓
REST API
   ↓
Docker Container
```

A container alone does not create a microservice.

---

## 4. When Should a Module Become a Service?

Look for real architectural drivers.

### Independent Deployment

The module changes much more frequently than the rest of the system.

### Independent Scaling

```text
Orders:      100 req/s
Payments:    10 req/s
Inventory:   1,000 req/s
```

Different scaling requirements may justify separation.

### Fault Isolation

A failure in one capability should not bring down unrelated capabilities.

### Team Ownership

A team needs autonomous ownership of the capability.

### Technology Independence

A capability has legitimate reasons to use a different runtime or technology.

### Security Isolation

A capability requires stronger security boundaries.

### Operational Independence

Different availability, deployment, or compliance requirements exist.

---

## 5. When Not to Extract

Do not extract a module simply because:

* it has a separate folder
* it has a separate project
* it has a database table
* it has a REST endpoint
* microservices are popular
* "the architecture should be scalable"

If the module has:

```text
High coupling
Frequent synchronous calls
Shared transactions
Shared database
Shared domain objects
Shared deployment lifecycle
```

then extraction may simply create a **distributed monolith**.

---

## 6. Core Principles

### 6.1 Preserve the Boundary

The modular boundary becomes the candidate service boundary.

```text
Modular Monolith

Orders
   │
   ├── Domain
   ├── Application
   └── Infrastructure

        ↓

Orders Service
   │
   ├── Domain
   ├── Application
   └── Infrastructure
```

---

### 6.2 Service Owns Its Data

Before:

```text
Orders ──┐
Payments ├── Shared Database
Inventory ┘
```

After:

```text
Orders ─────→ Orders DB
Payments ───→ Payments DB
Inventory ──→ Inventory DB
```

The database boundary is part of the service boundary.

---

### 6.3 Contracts Replace Internal References

Before:

```text
Orders
   ↓
Payments.Domain
```

After:

```text
Orders
   ↓
Payments API / Event Contract
   ↓
Payments
```

---

### 6.4 Remote Calls Are Different

Inside a monolith:

```csharp
paymentService.Authorize();
```

Across services:

```text
Orders
   ↓
Network
   ↓
Payments
```

Now the architecture must handle:

* latency
* timeout
* retry
* duplicate requests
* partial failure
* unavailable dependencies
* version compatibility

---

## 7. Target Architecture

Example:

```text
                    ┌───────────────┐
                    │ API Gateway   │
                    └───────┬───────┘
                            │
            ┌───────────────┼───────────────┐
            ▼               ▼               ▼
      ┌──────────┐    ┌──────────┐    ┌──────────┐
      │  Orders  │    │ Payments │    │ Inventory│
      │ Service  │    │ Service  │    │ Service  │
      └────┬─────┘    └────┬─────┘    └────┬─────┘
           │               │               │
           ▼               ▼               ▼
      Orders DB       Payments DB      Inventory DB
```

Communication may be:

```text
Synchronous:
Orders → Payments

Asynchronous:
Orders → OrderCreated → Inventory
```

---

## 8. Migration Strategy

Do not extract everything at once.

Use:

```text
1. Identify candidate
        ↓
2. Validate boundary
        ↓
3. Define service contract
        ↓
4. Separate data ownership
        ↓
5. Introduce compatibility layer
        ↓
6. Extract implementation
        ↓
7. Deploy independently
        ↓
8. Route traffic
        ↓
9. Observe
        ↓
10. Remove monolith implementation
```

---

## 9. Step 1: Select a Candidate

Evaluate:

```text
Business cohesion
Data ownership
Dependency count
Change frequency
Scaling requirements
Failure isolation
Team ownership
Security requirements
Operational complexity
```

A good candidate usually has:

```text
High cohesion
+
Clear ownership
+
Limited dependencies
+
Meaningful reason for independence
```

---

## 10. Step 2: Define the Service Contract

Before extraction, define what the service exposes.

For example:

```http
POST /payments/authorize
GET  /payments/{id}
POST /payments/{id}/refund
```

Or asynchronous contracts:

```text
PaymentAuthorized
PaymentRejected
PaymentRefunded
```

The contract becomes the new architectural boundary.

---

## 11. Step 3: Separate Data Ownership

This is often the hardest part.

Before:

```text
Orders
Payments
     ↓
Shared DbContext
     ↓
Shared Database
```

Target:

```text
Orders Service
     ↓
OrdersDb

Payments Service
     ↓
PaymentsDb
```

Migration may require:

```text
Create new schema
       ↓
Backfill data
       ↓
Synchronize changes
       ↓
Validate
       ↓
Switch reads
       ↓
Switch writes
       ↓
Remove old access
```

---

## 12. Step 4: Introduce a Compatibility Layer

The monolith may temporarily call the new service.

```text
Monolith
   │
   ▼
Payments Adapter
   │
   ▼
Payments Service
```

This allows gradual migration without changing every consumer simultaneously.

The compatibility layer should have an explicit removal plan.

---

## 13. Step 5: Extract the Service

Move the module into its own deployable application:

```text
src/
├── Services/
│   ├── Orders/
│   ├── Payments/
│   └── Inventory/
│
└── Monolith/
```

The extracted service should own:

```text
Code
Runtime
Deployment
Data
Configuration
Observability
Security
Scaling
```

---

## 14. Step 6: Replace In-Process Communication

Before:

```text
Orders
   ↓
IPaymentService
   ↓
Payments
```

After:

```text
Orders
   ↓
HTTP / gRPC
   ↓
Payments Service
```

Or:

```text
Orders
   ↓
Event
   ↓
Message Broker
   ↓
Payments
```

This is where distributed-system concerns become real.

---

## 15. Synchronous vs Asynchronous Communication

### Synchronous

```text
Orders
   │
   │ HTTP/gRPC
   ▼
Payments
```

Useful when the caller requires an immediate response.

Costs:

* temporal coupling
* latency
* timeout
* dependency availability
* cascading failure

### Asynchronous

```text
Orders
   │
   ▼
OrderCreated
   │
   ▼
Broker
   │
   ▼
Inventory
```

Benefits:

* temporal decoupling
* buffering
* independent processing

Costs:

* eventual consistency
* duplicate messages
* ordering
* replay
* idempotency

The communication model should follow the business interaction.

---

## 16. Distributed Transactions

A monolith may have:

```text
BEGIN TRANSACTION

Create Order
Authorize Payment
Reserve Inventory

COMMIT
```

After extraction, this becomes difficult.

Avoid assuming that a distributed transaction should replace the local transaction.

Instead consider:

```text
OrderCreated
     ↓
PaymentAuthorized
     ↓
InventoryReserved
```

with compensation when necessary.

This introduces the **Saga** pattern.

---

## 17. Failure Handling

Every remote dependency requires explicit failure behavior.

Consider:

```text
Timeout
Retry
Circuit Breaker
Bulkhead
Rate Limit
Load Shedding
Fallback
Idempotency
```

For example:

```text
Orders
  │
  ▼
Payments
  │
  X Timeout
  │
  ▼
Retry
  │
  ▼
Circuit Breaker
```

Retries must be bounded.

Otherwise:

```text
Failure
   ↓
Retry
   ↓
Retry
   ↓
Retry
   ↓
Retry Storm
   ↓
System Failure
```

---

## 18. Idempotency

Network communication can produce duplicates.

For example:

```text
Authorize Payment
Request #123
```

The caller times out.

It retries.

The payment service receives:

```text
Request #123
Request #123
```

The operation must safely handle the duplicate.

Typical mechanism:

```text
Idempotency-Key
       ↓
Payment Service
       ↓
Existing Result?
   ├── Yes → Return result
   └── No  → Process
```

---

## 19. Observability

The architecture must become observable across service boundaries.

Track:

```text
TraceId
SpanId
CorrelationId
CausationId
RequestId
MessageId
```

Measure:

* latency
* throughput
* error rate
* dependency failures
* retry rate
* queue depth
* database performance
* deployment health

Distributed tracing becomes essential.

---

## 20. Architectural Invariants

The extracted service should enforce:

1. It owns its business capability.
2. It owns its data.
3. Other services cannot directly access its database.
4. Communication occurs through explicit contracts.
5. Contracts are version compatible.
6. Mutating operations are idempotent where required.
7. Remote calls have bounded timeouts.
8. Retries are bounded.
9. Failures cannot propagate without control.
10. The service can be deployed independently.
11. The service can be scaled independently.
12. The service has independent observability.
13. Legacy dependencies are removed after extraction.

---

## 21. Common Failure Modes

### Distributed Monolith

Services exist physically but remain tightly coupled.

```text
A → B → C → D → E
```

Everything must be available for the system to work.

---

### Shared Database

```text
Orders ──┐
Payments ├── Same Tables
Inventory┘
```

The services are separated in code but coupled through data.

---

### Chatty Services

```text
Orders
  ↓
Customer
  ↓
Address
  ↓
Pricing
  ↓
Inventory
  ↓
Payments
```

One request creates a long synchronous chain.

---

### Distributed Transaction Everywhere

Trying to preserve monolithic transaction semantics across every service usually creates substantial complexity.

---

### Nano-Services

A tiny piece of functionality becomes a separate service without a meaningful reason for independence.

---

### API-Only Extraction

The module is exposed through HTTP but still depends on:

```text
Shared database
Shared domain model
Shared transaction
Shared deployment
```

This is not true service independence.

---

## 22. Testing Strategy

Use multiple levels:

```text
Unit Tests
     ↓
Integration Tests
     ↓
Architecture Tests
     ↓
Contract Tests
     ↓
Migration Tests
     ↓
Resilience Tests
     ↓
Load Tests
     ↓
Chaos Tests
```

Important tests include:

* API compatibility
* event compatibility
* data consistency
* idempotency
* timeout behavior
* retry behavior
* service unavailability
* database failure
* message duplication
* message ordering
* deployment compatibility

---

## 23. Rollback Strategy

Every extraction should define:

```text
Old Path
   ↓
New Path
   ↓
Validation
   ↓
Traffic Switch
```

If problems occur:

```text
New Service
    X
    ↓
Rollback
    ↓
Monolith Path
```

Be especially careful with data migrations.

Once data ownership has moved, rollback may no longer be as simple as switching traffic.

---

## 24. .NET Implementation

Typical technologies:

* ASP.NET Core
* gRPC
* `HttpClientFactory`
* .NET resilience pipelines
* Kafka / RabbitMQ / Azure Service Bus
* EF Core
* PostgreSQL / SQL Server
* OpenTelemetry
* .NET Aspire
* Docker
* Kubernetes

Useful project structure:

```text
src/
├── Monolith/
├── Services/
│   └── Payments/
│       ├── Domain/
│       ├── Application/
│       ├── Infrastructure/
│       └── Api/
│
└── Contracts/
```

Avoid turning `Contracts/` into a shared domain model.

Contracts should represent communication, not internal business implementation.

---

## 25. Lab Experiment

Start with the modular monolith from:

```text
01-MonolithToModularMonolith
```

Use:

```text
Orders
Payments
Inventory
```

Then extract **Payments**.

### Phase 1

Run everything inside one process.

```text
Orders
Payments
Inventory
```

### Phase 2

Define the Payments public contract.

### Phase 3

Give Payments explicit data ownership.

### Phase 4

Create a Payments service.

### Phase 5

Move Payments implementation.

### Phase 6

Introduce HTTP or gRPC communication.

### Phase 7

Add idempotency.

### Phase 8

Add timeout and retry.

### Phase 9

Introduce failure injection.

### Phase 10

Add distributed tracing.

### Phase 11

Deploy Payments independently.

### Phase 12

Remove the old Payments implementation from the monolith.

---

## 26. Failure Experiment

Intentionally introduce:

```text
Payments unavailable
```

Observe:

```text
Orders
   ↓
Payments
   X
```

Then test:

* timeout
* retry
* circuit breaker
* fallback
* idempotency
* graceful degradation

The objective is not simply to make the request succeed.

The objective is to understand the **blast radius of failure**.

---

## 27. Success Criteria

The migration is successful when the extracted service has meaningful independence:

```text
Independent Code
        +
Independent Data
        +
Independent Deployment
        +
Independent Scaling
        +
Explicit Contracts
        +
Failure Isolation
        +
Observability
```

If only the deployment changed, the architecture has not meaningfully evolved.

---

## 28. Key Takeaways

* A modular monolith is the strongest starting point for selective service extraction.
* A module is a candidate for extraction, not automatically a microservice.
* Independent deployment is necessary but not sufficient.
* Data ownership is one of the most important boundaries.
* APIs and events replace in-process dependencies.
* Remote communication introduces latency and failure.
* Distributed transactions should be minimized.
* Idempotency becomes essential.
* Observability must cross service boundaries.
* Service extraction should be incremental.
* Compatibility layers enable gradual migration.
* Rollback becomes harder once data ownership changes.
* Not every module should become a service.
* A successful extraction creates **independence**, not simply distribution.

> **The goal is not to turn a modular monolith into many services.**
>
> **The goal is to turn selected boundaries into independently evolvable systems.**
