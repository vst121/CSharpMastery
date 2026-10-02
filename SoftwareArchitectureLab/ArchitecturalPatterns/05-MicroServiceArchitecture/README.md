# Microservice Architecture

## 1. Essence

Microservice Architecture structures a system as a collection of independently deployable services, where each service owns a well-defined business capability, its data, and its operational lifecycle.

The essential architectural property is **independent evolution**.

A microservice is not simply:

- a small application
- a REST API
- a Docker container
- a database-backed service
- a class or module exposed over HTTP

A meaningful microservice represents a **business capability with an explicit boundary and independent operational ownership**.

The architecture introduces distributed-system characteristics in exchange for organizational and deployment independence.

Core principle:

> **A microservice boundary should create meaningful independence in business capability, data ownership, deployment, scaling, and evolution.**

---

## 2. Problem

Large systems become difficult to evolve when everything is tightly coupled.

Typical problems include:

- large deployment units
- long release cycles
- shared database coupling
- tightly coupled teams
- large regression surfaces
- inability to scale individual capabilities
- technology lock-in
- unclear ownership
- increasing dependency complexity
- changes requiring coordination across the entire system

Microservices attempt to address these problems by decomposing the system into independently evolving services.

However, decomposition introduces new complexity:

- network communication
- distributed transactions
- eventual consistency
- service discovery
- distributed observability
- failure propagation
- deployment coordination
- contract compatibility
- operational overhead

Therefore:

> **Microservices move complexity from inside the application into the architecture and operations of the system.**

---

## 3. Intent

The intent of Microservice Architecture is to enable:

- independent deployment
- independent scaling
- independent ownership
- business capability isolation
- controlled coupling
- technology flexibility
- autonomous teams
- incremental evolution
- fault isolation
- shorter delivery cycles
- explicit service contracts

The goal is not to maximize the number of services.

The goal is to create **meaningful boundaries that enable independent evolution**.

---

## 4. Core Principles

### 4.1 Business Capability First

Services should normally represent business capabilities rather than technical layers.

Prefer:

```text
Order Service
Payment Service
Inventory Service
Customer Service
Shipping Service
```

over:

```text
Database Service
Validation Service
Repository Service
Logging Service
Utility Service
```

The service boundary should reflect business ownership.

---

### 4.2 Independent Deployment

A service should be deployable without requiring simultaneous deployment of unrelated services.

This requires:

- compatible contracts
- backward compatibility
- versioning strategies
- independent configuration
- independent deployment pipelines
- controlled dependencies

---

### 4.3 Data Ownership

A service should own the data required to implement its business capability.

Conceptually:

```text
Order Service
    |
    +-- Order Database

Payment Service
    |
    +-- Payment Database

Inventory Service
    |
    +-- Inventory Database
```

Other services should not directly manipulate another service's database.

---

### 4.4 Explicit Contracts

Communication between services must use explicit contracts.

Examples:

- REST
- gRPC
- messaging
- events
- commands
- streams

Contracts should define:

- request structure
- response structure
- error semantics
- compatibility rules
- versioning
- ownership
- operational expectations

---

### 4.5 Failure Is Normal

Every remote call can fail.

A microservice architecture must explicitly handle:

- timeout
- network failure
- service unavailable
- partial failure
- duplicate messages
- duplicate requests
- stale data
- retries
- ordering problems
- dependency overload

---

### 4.6 Minimize Coupling

A service should not require another service for every small operation.

Avoid:

```text
A -> B -> C -> D -> E
```

especially when the entire request depends on every hop.

Prefer architectures where services can operate independently and asynchronous communication is used where appropriate.

---

### 4.7 Ownership Must Be Clear

Every important architectural responsibility should have an owner.

Examples:

```text
Order Service
    owns:
        Orders
        Order lifecycle
        Order invariants

Payment Service
    owns:
        Payments
        Payment lifecycle
        Payment state
```

Ownership should include:

- code
- data
- contracts
- operational responsibility
- failure handling
- evolution

---

## 5. Architectural Model

A typical microservice system can be represented as:

```text
                    ┌─────────────────┐
                    │ API Gateway /   │
                    │ BFF             │
                    └────────┬────────┘
                             |
             ┌───────────────┼────────────────┐
             |               |                |
             v               v                v
      ┌────────────┐  ┌────────────┐  ┌────────────┐
      │ Order      │  │ Customer   │  │ Product    │
      │ Service    │  │ Service    │  │ Service    │
      └─────┬──────┘  └─────┬──────┘  └─────┬──────┘
            |                |                |
            v                v                v
       Order DB        Customer DB       Product DB

             |                |                |
             └──────────┬─────┴────────────────┘
                        |
                        v
                Event Infrastructure
```

The architecture normally contains:

- independently deployable services
- service-owned data
- synchronous APIs
- asynchronous messaging
- event infrastructure
- gateways or BFFs
- observability infrastructure
- centralized or federated identity
- deployment infrastructure

---

## 6. Service Boundaries

Service boundaries are the most important architectural decision.

A useful service boundary usually has:

- cohesive business responsibility
- clear ownership
- limited external dependencies
- meaningful internal complexity
- independent evolution potential
- clear data ownership
- stable business language

A boundary should be evaluated by asking:

> Can this capability evolve without requiring constant coordination with other services?

If not, the boundary may be incorrect.

---

## 7. Service Granularity

There is no universal service size.

Avoid both extremes.

### Monolithic Service

```text
Everything
    |
    +-- Orders
    +-- Payments
    +-- Customers
    +-- Inventory
    +-- Reporting
```

### Nano Services

```text
OrderValidationService
OrderCalculationService
OrderAddressService
OrderItemService
OrderNumberService
```

The second design can create excessive:

- network calls
- deployment overhead
- operational complexity
- failure points
- observability requirements
- coordination

The objective is **appropriate granularity**.

---

## 8. Communication

Microservices commonly communicate using:

### Synchronous Communication

Examples:

```text
HTTP
REST
gRPC
```

Useful when the caller needs an immediate response.

Example:

```text
Order Service
     |
     | Check Product
     v
Product Service
```

Risks:

- latency propagation
- availability coupling
- cascading failure
- dependency chains

---

### Asynchronous Communication

Examples:

```text
Kafka
RabbitMQ
Azure Service Bus
```

Useful when immediate responses are unnecessary.

Example:

```text
Order Service
      |
      | OrderCreated
      v
 Message Broker
      |
      +----> Inventory Service
      |
      +----> Notification Service
      |
      +----> Analytics Service
```

Benefits include:

- temporal decoupling
- better failure isolation
- independent processing
- scalable consumers

But asynchronous systems introduce:

- eventual consistency
- duplicate delivery
- ordering problems
- replay considerations
- idempotency requirements

---

## 9. API Design

Service APIs should be designed as contracts rather than implementation details.

Important considerations:

- resource modeling
- commands
- queries
- DTOs
- error contracts
- pagination
- filtering
- idempotency
- versioning
- compatibility
- rate limiting

Avoid exposing internal domain models directly.

For example:

```text
Domain Model
     |
     v
Application Contract
     |
     v
External API
```

This prevents internal implementation changes from unnecessarily becoming contract changes.

---

## 10. Data Ownership

Each service should normally own its persistence model.

Example:

```text
Order Service
    |
    +-- PostgreSQL

Payment Service
    |
    +-- PostgreSQL

Search Service
    |
    +-- Elasticsearch

Analytics Service
    |
    +-- Data Warehouse
```

Different services may use different storage technologies when justified by their workload.

This is known as **polyglot persistence**.

The architectural requirement is not that every service must use a different database.

The requirement is:

> **Data ownership must be explicit.**

---

## 11. Distributed Transactions

Traditional ACID transactions work naturally inside a service boundary.

Across services, they become significantly more difficult.

Avoid:

```text
Transaction
    |
    +-- Order DB
    +-- Payment DB
    +-- Inventory DB
```

Prefer local transactions combined with distributed coordination.

Typical approaches:

- Saga
- orchestration
- choreography
- transactional outbox
- idempotent consumers
- compensating actions
- eventual consistency

Example:

```text
Create Order
     |
     v
Order Created
     |
     v
Reserve Inventory
     |
     v
Process Payment
     |
     v
Confirm Order
```

If payment fails:

```text
Payment Failed
      |
      v
Release Inventory
      |
      v
Cancel Order
```

The system therefore models distributed business workflows explicitly.

---

## 12. Saga Architecture

A Saga coordinates a distributed business transaction through a sequence of local transactions.

Two common styles exist.

### Orchestration

```text
              ┌───────────────┐
              │ Saga          │
              │ Orchestrator  │
              └───────┬───────┘
                      |
        ┌─────────────┼─────────────┐
        v             v             v
     Order        Inventory       Payment
```

The orchestrator controls the workflow.

---

### Choreography

```text
OrderCreated
     |
     v
Inventory
     |
InventoryReserved
     |
     v
Payment
     |
PaymentCompleted
     |
     v
Order
```

Services react to events without a central coordinator.

The architecture must explicitly define:

- state transitions
- compensating actions
- failure handling
- timeout behavior
- idempotency
- observability

---

## 13. Transactional Outbox

A common reliability problem is:

```text
Database transaction succeeds
        |
        X
Message publishing fails
```

The service can use a transactional outbox:

```text
┌─────────────────────────────┐
│ Service Database            │
│                             │
│ Business Data               │
│ Outbox Messages             │
└──────────────┬──────────────┘
               |
               v
        Outbox Publisher
               |
               v
        Message Broker
```

The business state and outgoing message are persisted in the same local transaction.

This reduces the risk of losing integration events.

---

## 14. Resilience

Every service boundary must define resilience behavior.

Typical controls include:

### Timeout

Never allow remote operations to wait indefinitely.

### Retry

Retries should be:

- bounded
- selective
- exponential
- jittered
- aware of idempotency

### Circuit Breaker

Stops repeatedly calling an unhealthy dependency.

### Bulkhead

Separates resource pools to prevent one dependency from exhausting the entire service.

### Rate Limiting

Controls incoming request pressure.

### Load Shedding

Rejects lower-priority work when the system is overloaded.

### Backpressure

Allows downstream capacity to influence upstream production.

---

## 15. Failure Isolation

Microservices should limit the blast radius of failures.

Example:

```text
Notification Service DOWN
          |
          X
          |
     Order Service
          |
          v
   Order Processing
       continues
```

This requires:

- asynchronous communication where appropriate
- bounded queues
- timeouts
- circuit breakers
- bulkheads
- graceful degradation
- fallback strategies

A microservice architecture that fails globally when one service fails has weak failure isolation.

---

## 16. Idempotency

Distributed systems frequently encounter duplicate operations.

For example:

```text
Client
  |
  | Request
  v
Payment Service
  |
  | Process
  v
Payment DB
```

The client may retry because the response was lost:

```text
Request
   |
   v
Payment succeeds
   |
   X
Response lost
   |
   v
Client retries
```

Without idempotency, payment may be processed twice.

Use:

- idempotency keys
- unique constraints
- deduplication tables
- message IDs
- state-machine validation

Example:

```text
IdempotencyKey = 8f4c...

First request:
    Process payment

Duplicate request:
    Return existing result
```

---

## 17. Service Discovery

Distributed services need a mechanism for locating dependencies.

Possible approaches include:

- DNS
- service registries
- Kubernetes Services
- cloud-native discovery
- .NET Aspire service discovery

The architecture should avoid hard-coded service locations.

For example:

```text
http://10.10.2.17:5001
```

should normally not become part of application logic.

Instead:

```text
http://orders
```

or a service-discovery abstraction can resolve the destination.

---

## 18. API Gateway and BFF

An API Gateway can provide a controlled external entry point.

Typical responsibilities:

- routing
- authentication
- authorization
- rate limiting
- TLS termination
- request transformation
- observability
- aggregation

A Backend-for-Frontend can specialize APIs for specific clients.

Example:

```text
                Mobile App
                    |
                Mobile BFF
                    |
        ┌───────────┼───────────┐
        v           v           v
      Orders     Customer    Products
```

The gateway should not become a new monolith containing business logic.

---

## 19. Observability

Distributed systems require distributed observability.

Every request should be traceable across service boundaries.

Important identifiers include:

```text
TraceId
SpanId
CorrelationId
CausationId
RequestId
MessageId
```

Example:

```text
Client
  |
  v
API Gateway
  |
  v
Order Service
  |
  +----> Inventory Service
  |
  +----> Payment Service
```

A distributed trace should allow engineers to reconstruct the entire interaction.

Use:

- structured logging
- metrics
- distributed tracing
- health checks
- dependency metrics
- queue metrics
- error tracking
- alerting

OpenTelemetry should be a core part of the implementation.

---

## 20. Security

Every service boundary is a security boundary.

Important concerns include:

- authentication
- authorization
- service identity
- TLS
- secret management
- token validation
- least privilege
- network segmentation
- audit logging
- data protection
- zero-trust principles

Avoid assuming:

```text
Internal network = trusted network
```

Each service should validate the identity and authorization context appropriate to its responsibility.

---

## 21. Scalability

One major capability of microservices is independent scaling.

Example:

```text
Order Service
    3 instances

Search Service
    20 instances

Payment Service
    5 instances
```

Scaling decisions should be based on actual workload characteristics.

Useful techniques include:

- horizontal scaling
- stateless services
- asynchronous processing
- queue-based load leveling
- caching
- read replicas
- partitioning
- sharding where justified
- workload isolation

Scaling the number of services does not automatically improve scalability.

The bottleneck may instead be:

- database
- network
- message broker
- external API
- synchronization point
- shared infrastructure

---

## 22. Architectural Invariants

The implementation should enforce the following invariants wherever possible.

### Service Boundaries

- Every service owns a clear business capability.
- Services have explicit responsibilities.
- Services do not bypass ownership boundaries.

### Data

- Services own their data.
- Other services do not directly modify another service's database.
- Shared databases are treated as explicit coupling.

### Communication

- Remote calls have bounded timeouts.
- Retry policies are bounded.
- Mutating operations are idempotent where required.
- Contracts are explicit and version-compatible.

### Deployment

- Services can be independently deployed.
- Backward compatibility is maintained during rolling deployments.
- Configuration is independently managed.

### Resilience

- Failures are isolated where possible.
- Dependency failures do not automatically become system-wide failures.
- Queues have bounded capacity.
- Retry storms are prevented.

### Observability

- Cross-service requests are traceable.
- Errors contain sufficient diagnostic context.
- Service health and dependency health are observable.

These invariants should become automated architecture tests where possible.

---

## 23. Failure Modes and Architectural Smells

### Distributed Monolith

Services are technically separate but must always be deployed and changed together.

### Shared Database Coupling

Multiple services directly depend on the same database schema.

### Nano Services

Services are so small that network and operational complexity dominate their business value.

### Chatty Services

A single operation requires many remote calls.

```text
A -> B
A -> C
A -> D
A -> E
A -> F
```

### Long Synchronous Chains

```text
A -> B -> C -> D -> E
```

A failure in E can affect the entire request.

### Distributed Big Ball of Mud

The system contains many services but no meaningful boundaries.

### Shared Domain Model

Multiple services depend on the same internal domain model or shared mutable abstractions.

### Excessive Distributed Transactions

Business operations constantly require coordination across many services.

### Event Storm

Every internal change becomes an integration event without clear business meaning.

### Retry Storm

Multiple layers retry the same failed operation.

### Hidden Coupling

Services appear independent but depend on:

- shared databases
- shared schemas
- shared deployment
- shared infrastructure assumptions
- undocumented contracts

### False Independence

A service is technically deployable independently but cannot evolve independently.

---

## 24. Testing Strategy

Microservice architecture requires multiple levels of testing.

### Unit Tests

Test service-local business logic.

### Integration Tests

Test:

- databases
- message brokers
- external integrations
- infrastructure

### Contract Tests

Verify that service consumers and providers agree on contracts.

Examples:

- HTTP contracts
- gRPC contracts
- event schemas

### Architecture Tests

Verify:

- dependency rules
- service boundaries
- forbidden references
- data ownership
- contract rules

### Resilience Tests

Inject:

- latency
- timeouts
- unavailable services
- duplicate messages
- broker failures
- database failures

### Chaos Experiments

Deliberately introduce failures into the distributed system.

Examples:

```text
Kill Payment Service
Delay Inventory Service
Drop messages
Add network latency
Exhaust connection pool
Restart database
```

The objective is not simply to prove that the system works.

The objective is to understand **how it fails**.

---

## 25. Evolution and Migration

Microservices should usually evolve incrementally.

A common migration path is:

```text
Existing Monolith
       |
       v
Identify Business Boundary
       |
       v
Create Internal Module
       |
       v
Define Contract
       |
       v
Separate Data Ownership
       |
       v
Extract Service
       |
       v
Independent Deployment
       |
       v
Operate Independently
```

Avoid beginning with infrastructure.

The first question should be:

> What business boundary deserves independent ownership?

Useful migration techniques include:

- Strangler Fig
- Branch by Abstraction
- Parallel Change
- Expand and Contract
- Anti-Corruption Layer
- CDC
- incremental data migration
- event-based synchronization

---

## 26. .NET Implementation Notes

Modern .NET provides strong building blocks for microservice systems.

### ASP.NET Core

For HTTP APIs:

```text
ASP.NET Core
```

### gRPC

Useful for efficient internal service-to-service communication.

### HttpClientFactory

Provides managed HTTP client lifetimes and integration with resilience policies.

### Resilience Pipelines

Use modern .NET resilience capabilities for:

- retries
- timeouts
- circuit breakers
- hedging
- rate limiting

### BackgroundService

Useful for:

- message consumers
- outbox processing
- background workflows

### OpenTelemetry

Use for:

- traces
- metrics
- logs

### .NET Aspire

Useful for local distributed application development and orchestration.

Example conceptual structure:

```text
AppHost
 |
 +-- OrderService
 |
 +-- PaymentService
 |
 +-- InventoryService
 |
 +-- PostgreSQL
 |
 +-- Kafka
 |
 +-- OpenTelemetry
```

### Strongly Typed Contracts

Prefer explicit DTOs and contracts over leaking internal domain types.

### Health Checks

Expose:

```text
Liveness
Readiness
Dependency Health
```

Avoid treating every dependency failure as a service-wide liveness failure.

---

## 27. Production Checklist and Architecture Experiments

### Service Design

- [ ] Business capability identified
- [ ] Service ownership defined
- [ ] Boundary documented
- [ ] Data ownership defined
- [ ] API contract defined
- [ ] Events defined where required

### Distributed Communication

- [ ] Synchronous calls justified
- [ ] Asynchronous communication considered
- [ ] Timeouts configured
- [ ] Retries bounded
- [ ] Idempotency implemented
- [ ] Contract compatibility tested

### Data

- [ ] Database ownership defined
- [ ] Cross-service database access prohibited
- [ ] Consistency requirements documented
- [ ] Distributed transaction strategy defined
- [ ] Outbox considered

### Resilience

- [ ] Circuit breaker where appropriate
- [ ] Bulkhead where appropriate
- [ ] Backpressure considered
- [ ] Load shedding considered
- [ ] Failure isolation tested

### Observability

- [ ] Distributed tracing
- [ ] Structured logging
- [ ] Metrics
- [ ] Health checks
- [ ] Dependency monitoring
- [ ] Alerting

### Security

- [ ] Authentication
- [ ] Authorization
- [ ] Service identity
- [ ] TLS
- [ ] Secret management
- [ ] Audit logging

### Deployment

- [ ] Independent deployment
- [ ] Backward-compatible contracts
- [ ] Containerization
- [ ] Health probes
- [ ] Graceful shutdown
- [ ] Rollback strategy

### Recommended Lab Experiments

1. Build two independently deployable services.
2. Give each service its own PostgreSQL database.
3. Implement synchronous REST communication.
4. Replace one synchronous interaction with an event.
5. Add Kafka or RabbitMQ.
6. Implement transactional outbox.
7. Implement idempotent message processing.
8. Implement a Saga.
9. Inject service failures.
10. Add retry and circuit breaker.
11. Add distributed tracing with OpenTelemetry.
12. Measure latency across synchronous dependency chains.
13. Implement API versioning.
14. Perform a rolling deployment with incompatible and compatible contracts.
15. Simulate duplicate messages.
16. Simulate message reordering.
17. Simulate network latency.
18. Perform a chaos experiment.
19. Load-test individual services independently.
20. Measure the blast radius of dependency failures.

Recommended experiment workflow:

```text
Problem
   ↓
Service Boundary
   ↓
Contract
   ↓
Implementation
   ↓
Architecture Tests
   ↓
Failure Experiment
   ↓
Load / Performance Test
   ↓
Observability
   ↓
Measurement
   ↓
Trade-offs
   ↓
ADR
```

---

## 28. Key Takeaways

Microservice Architecture is fundamentally about **independent evolution**.

The important architectural properties are:

- Business capability boundaries
- Explicit service ownership
- Independent deployment
- Data ownership
- Explicit contracts
- Controlled coupling
- Failure isolation
- Independent scaling
- Operational ownership
- Observable boundaries

Microservices introduce real distributed-system costs:

- Network latency
- Partial failure
- Eventual consistency
- Distributed transactions
- Contract evolution
- Operational complexity
- Debugging complexity
- Deployment complexity

Therefore:

> **Microservices are not a decomposition technique alone. They are an organizational, operational, data, deployment, and distributed-systems architecture.**

A service boundary is valuable when it creates meaningful independence.

A service boundary is harmful when it only moves complexity from method calls to network calls.

The architecture should therefore optimize for:

```text
Meaningful Boundaries
        +
Independent Ownership
        +
Independent Deployment
        +
Controlled Coupling
        +
Failure Isolation
        +
Observable Distributed Behavior
```

The ultimate test is not:

> "How many services do we have?"

It is:

> "Can these business capabilities evolve, deploy, scale, and fail independently?"

Architecture becomes executable when these principles are enforced through:

- architecture tests
- contract tests
- resilience tests
- failure experiments
- chaos testing
- load testing
- distributed tracing
- production telemetry

**Microservices should create independence, not merely distribution.**
