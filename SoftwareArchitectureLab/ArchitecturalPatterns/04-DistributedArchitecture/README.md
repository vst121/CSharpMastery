# Distributed Architecture

## 1. Essence

**Distributed Architecture** structures a system as a collection of independently executing components that communicate across process, machine, network, or deployment boundaries.

A distributed system may consist of:

```text
┌─────────────────┐
│   API Service   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Order Service   │
└────────┬────────┘
         │
         ├──────────────► Payment Service
         │
         ├──────────────► Inventory Service
         │
         └──────────────► Notification Service
```

Unlike an in-process architecture, components cannot assume:

- Shared memory
- Shared execution context
- Instant communication
- Reliable communication
- Synchronized clocks
- Immediate consistency
- Guaranteed availability

The network becomes part of the architecture.

The fundamental architectural shift is:

```text
Local execution
    ↓
Remote execution
    ↓
Distributed coordination
```

The primary goal is not simply to distribute code across machines.

The goal is to establish **clear ownership, controlled communication, independent failure boundaries, and predictable behavior across distributed components**.

---

## 2. Problem

A large system eventually encounters constraints that make a single execution boundary insufficient.

Typical drivers include:

- Independent scaling requirements
- Team ownership boundaries
- Deployment independence
- Fault isolation
- Geographic distribution
- Technology boundaries
- Organizational boundaries
- Workload isolation
- Regulatory requirements
- Availability requirements

A centralized system may look like:

```text
┌──────────────────────────────┐
│         Application          │
│                              │
│ Orders                       │
│ Payments                     │
│ Inventory                    │
│ Notifications                │
└──────────────┬───────────────┘
               │
               ▼
            Database
```

A distributed architecture introduces explicit boundaries:

```text
┌──────────────┐       ┌──────────────┐
│    Orders    │──────►│   Payments   │
└──────┬───────┘       └──────────────┘
       │
       │
       ▼
┌──────────────┐       ┌──────────────┐
│  Inventory   │       │ Notifications│
└──────────────┘       └──────────────┘
```

This introduces new classes of problems:

```text
Network Failure
Partial Failure
Latency
Timeouts
Retries
Duplicate Requests
Consistency
Coordination
Distributed Transactions
Service Discovery
Observability
Deployment Complexity
```

Distribution therefore creates both capabilities and new failure modes.

---

## 3. Intent

Distributed Architecture aims to provide:

- Independent deployment
- Independent scaling
- Fault isolation
- Clear ownership boundaries
- Controlled communication
- Technology independence where justified
- Geographic distribution where required
- Workload isolation
- Explicit consistency boundaries
- Operational independence

The architecture should make the following questions explicit:

> Who owns this capability?

> Who owns this data?

> How do components communicate?

> What happens when communication fails?

> What consistency guarantees are required?

> What happens when only part of the system is available?

---

## 4. Core Principles

### 4.1 Distribution Is a Boundary

A distributed boundary is not simply a project or namespace.

A real distributed boundary introduces:

```text
Process Boundary
Network Boundary
Failure Boundary
Deployment Boundary
Ownership Boundary
```

Every such boundary has a cost.

---

### 4.2 The Network Is Unreliable

Remote communication must be treated as fallible.

A request can:

```text
Timeout
Fail
Arrive late
Arrive twice
Arrive out of order
Be processed but lose its response
```

Therefore:

```text
Remote Call ≠ Local Method Call
```

This is one of the most important principles in distributed architecture.

---

### 4.3 Partial Failure Is Normal

A distributed system can be partially available.

For example:

```text
Orders       ✓
Payments     ✓
Inventory    ✗
Notifications ✓
```

The system must define what happens in this state.

A failure of one component should not automatically become a failure of the entire system.

---

### 4.4 Ownership Must Be Explicit

Every distributed component should have clear ownership of:

- Business capability
- Data
- APIs
- Events
- Operational responsibility
- Security boundaries

Shared ownership creates ambiguity.

---

### 4.5 Minimize Distributed Coordination

Coordination across network boundaries is expensive.

Prefer:

```text
Local decision
    ↓
Asynchronous propagation
```

when business requirements permit it.

Avoid unnecessary chains such as:

```text
A
 ↓
B
 ↓
C
 ↓
D
 ↓
E
```

where every request requires all components to be available.

---

### 4.6 Independence Has a Cost

Independent deployment and scaling are valuable only when the system actually needs them.

Every distributed boundary introduces:

- Network latency
- Operational complexity
- Observability requirements
- Failure handling
- Security configuration
- Deployment coordination
- Data consistency challenges

Distribution should therefore be intentional.

---

### 4.7 Design for Failure

A distributed system should be designed around failure scenarios rather than only the happy path.

Important scenarios include:

```text
Service unavailable
Database unavailable
Network partition
Timeout
Duplicate request
Duplicate event
Partial deployment
Version mismatch
Resource exhaustion
Dependency degradation
```

---

## 5. Architectural Model

A distributed architecture can be modeled as:

```text
                   ┌─────────────────┐
                   │     Client      │
                   └────────┬────────┘
                            │
                            ▼
                    ┌───────────────┐
                    │    Gateway    │
                    └───────┬───────┘
                            │
              ┌─────────────┼─────────────┐
              │             │             │
              ▼             ▼             ▼
        ┌──────────┐  ┌──────────┐  ┌──────────┐
        │  Orders  │  │ Payments │  │Inventory │
        └────┬─────┘  └────┬─────┘  └────┬─────┘
             │             │             │
             ▼             ▼             ▼
        ┌──────────┐  ┌──────────┐  ┌──────────┐
        │ Order DB │  │Payment DB│  │Inventory │
        │          │  │          │  │    DB    │
        └──────────┘  └──────────┘  └──────────┘
```

Communication may occur through:

```text
Synchronous APIs
Asynchronous Events
Message Queues
Streams
RPC
Batch Interfaces
```

The architecture should explicitly define which communication model is used at each boundary.

---

## 6. Building Blocks

### 6.1 Distributed Component

A distributed component is independently executing software with its own runtime boundary.

Examples:

```text
Order Service
Payment Service
Inventory Service
Search Service
Notification Worker
```

A component should have a clear responsibility.

---

### 6.2 Service Boundary

A service boundary defines:

- Capability ownership
- API ownership
- Data ownership
- Deployment boundary
- Failure boundary

A service should represent a meaningful architectural boundary rather than simply a technical layer.

---

### 6.3 API

A distributed component may expose:

```text
REST
gRPC
GraphQL
WebSocket
```

The protocol should be chosen based on communication requirements rather than fashion.

---

### 6.4 Message Broker

Asynchronous communication can use:

```text
Kafka
RabbitMQ
Azure Service Bus
NATS
Amazon SQS
Redis Streams
```

The broker can provide:

- Buffering
- Delivery
- Retry
- Ordering
- Partitioning
- Persistence
- Fan-out

---

### 6.5 Database

A distributed component should normally have explicit ownership of its persistent state.

Conceptually:

```text
Order Service
     │
     ▼
Order Database

Payment Service
     │
     ▼
Payment Database
```

This avoids uncontrolled cross-service database coupling.

---

### 6.6 Service Discovery

Distributed systems need a way to locate components.

Possible mechanisms include:

```text
DNS
Service Registry
Kubernetes Service
Cloud Service Discovery
API Gateway
```

Service discovery should be transparent to business logic.

---

### 6.7 Configuration

Distributed configuration may include:

- Connection endpoints
- Feature flags
- Timeouts
- Retry policies
- Credentials
- Service URLs
- Resource limits

Configuration should be externalized and securely managed.

---

### 6.8 Distributed Trace

A distributed trace connects operations across multiple components.

```text
Request
   │
   ▼
API
   │
   ▼
Orders
   │
   ├──► Payments
   │
   └──► Inventory
```

A trace identifier allows the entire execution path to be reconstructed.

---

## 7. Communication and Interaction

### 7.1 Synchronous Communication

Example:

```text
Orders
   │
   │ HTTP/gRPC
   ▼
Payments
```

The caller waits for a response.

Advantages include:

- Immediate response
- Simple request/response semantics
- Straightforward error propagation

Costs include:

- Temporal coupling
- Network latency
- Dependency availability
- Cascading failures

---

### 7.2 Asynchronous Communication

Example:

```text
Orders
   │
   │ OrderPlaced
   ▼
Message Broker
   │
   ├──► Payments
   ├──► Inventory
   └──► Notifications
```

The producer does not wait for every consumer.

Benefits include:

- Temporal decoupling
- Buffering
- Independent processing
- Fan-out
- Better failure isolation

Costs include:

- Eventual consistency
- More complex debugging
- Duplicate delivery
- Ordering concerns
- Replay considerations

---

### 7.3 Communication Direction

Prefer explicit communication direction:

```text
A → B
```

rather than uncontrolled bidirectional dependencies:

```text
A ↔ B
```

Circular communication can make system behavior difficult to reason about.

---

## 8. Data Ownership and Consistency

### 8.1 Data Ownership

Every important piece of business data should have a clear owner.

For example:

```text
Order Service
    └── owns Order data

Payment Service
    └── owns Payment data

Inventory Service
    └── owns Inventory data
```

Other components should access the information through explicit contracts.

---

### 8.2 Shared Database

A shared database can create hidden coupling:

```text
Service A ──┐
            ├──► Shared Database
Service B ──┘
```

Changes to the database can affect multiple services simultaneously.

If a shared database is necessary, ownership and access rules should still be explicit.

---

### 8.3 Distributed Data

A more isolated model is:

```text
Service A ──► Database A

Service B ──► Database B

Service C ──► Database C
```

This increases independence but also introduces data synchronization challenges.

---

### 8.4 Eventual Consistency

Distributed components may observe different states temporarily.

Example:

```text
Order = Confirmed
Payment = Authorized
Inventory = Reserving
```

The architecture must define:

- Acceptable inconsistency
- Synchronization mechanism
- Reconciliation
- User-visible state
- Failure recovery

---

### 8.5 Consistency Boundaries

Not every business operation requires global strong consistency.

The architecture should identify where strong consistency is necessary and where eventual consistency is acceptable.

---

## 9. Distributed Transactions and Coordination

### 9.1 Local Transactions

Prefer transactions within a single ownership boundary.

```text
BEGIN

Update Order
Insert Outbox Event

COMMIT
```

Local transactions are easier to reason about.

---

### 9.2 Distributed Transactions

A distributed transaction spans multiple independent resources.

Conceptually:

```text
Order DB
   │
   ├──── transaction ────┐
   │                     │
Payment DB               │
   │                     │
   └─────────────────────┘
```

Distributed transactions introduce substantial coordination complexity.

They should not be introduced simply because multiple components need to participate in one business workflow.

---

### 9.3 Saga

A Saga coordinates a long-running business operation through a sequence of local transactions.

Example:

```text
Create Order
    │
    ▼
Reserve Inventory
    │
    ▼
Authorize Payment
    │
    ▼
Confirm Order
```

If a later step fails, compensating actions may be required:

```text
Authorize Payment
        │
        X
        │
Release Inventory
        │
        ▼
Cancel Order
```

A Saga can be coordinated through:

```text
Orchestration
Choreography
```

The important architectural property is explicit management of distributed workflow state.

---

## 10. Resilience and Failure Handling

Distributed systems require explicit resilience mechanisms.

### 10.1 Timeout

Every remote call should have a bounded timeout.

```text
Service A
   │
   │ request
   ▼
Service B
   │
   X
   │
 timeout
```

An unbounded request can consume threads, connections, memory, and other resources.

---

### 10.2 Retry

Retries can recover from transient failures.

However:

```text
Retry
+
High Load
+
Already Degraded Service
=
Potential Failure Amplification
```

Retry policies should define:

- Maximum attempts
- Backoff
- Jitter
- Retryable failures
- Request idempotency

---

### 10.3 Circuit Breaker

A circuit breaker can stop sending requests to a failing dependency.

Conceptually:

```text
Closed
  │
  │ failures
  ▼
Open
  │
  │ recovery window
  ▼
Half-Open
  │
  ├── success → Closed
  └── failure → Open
```

---

### 10.4 Bulkhead

Bulkheads isolate resource pools.

For example:

```text
                 Application
                     │
          ┌──────────┼──────────┐
          ▼          ▼          ▼
       Payment    Search    Notifications
        Pool       Pool        Pool
```

Failure in one workload should not exhaust resources needed by another.

---

### 10.5 Load Shedding

When the system is overloaded, rejecting lower-priority work can protect critical operations.

```text
Incoming Load
      │
      ▼
Capacity Limit
      │
 ┌────┴────┐
 │         │
Accept    Reject
```

Load shedding should be intentional and observable.

---

### 10.6 Backpressure

When downstream capacity is lower than incoming traffic, the system must control the flow.

Important signals include:

```text
Queue Depth
Consumer Lag
Request Rate
Processing Rate
Latency
Resource Utilization
```

---

## 11. Distributed Failure Modes

Distributed architecture creates failure modes that do not exist in the same form inside a single process.

### 11.1 Partial Failure

```text
A ✓
B ✓
C ✗
D ✓
```

The system must continue operating safely where possible.

---

### 11.2 Network Partition

Two components may both be running but unable to communicate.

```text
Service A
   │
   X
   │
Service B
```

The architecture must define behavior during the partition.

---

### 11.3 Timeout Ambiguity

A request can time out even though the remote operation succeeded.

```text
Client
  │
  │ Request
  ▼
Service
  │
  │ Operation succeeds
  ▼
Database

Response
   X
   │
Timeout
```

The client may not know whether it should retry.

This is why idempotency and operation identifiers are important.

---

### 11.4 Duplicate Requests

A retry can produce:

```text
Request 1
Request 2
```

even though the caller intended one operation.

Operations that mutate state should therefore use idempotency mechanisms where required.

---

### 11.5 Cascading Failure

A single failing service can cause dependent services to become overloaded:

```text
A
│
▼
B
│
▼
C
│
▼
D
```

If C becomes slow:

```text
B waits
 ↓
A waits
 ↓
Clients wait
```

Timeouts, circuit breakers, bulkheads, and bounded concurrency can limit the blast radius.

---

### 11.6 Retry Storm

When many clients retry simultaneously:

```text
Failure
  ↓
Retries
  ↓
More Load
  ↓
More Failure
  ↓
More Retries
```

This can turn a temporary failure into a systemic outage.

Use:

- Exponential backoff
- Jitter
- Retry budgets
- Circuit breakers
- Rate limiting

---

### 11.7 Thundering Herd

A large number of clients may simultaneously request the same resource after an event such as:

```text
Cache Expiration
Service Recovery
Scheduled Job
Configuration Change
```

The resulting spike can overwhelm the dependency.

Mitigation may include:

- Request coalescing
- Jittered expiration
- Caching
- Rate limiting
- Load shedding

---

## 12. Service Boundaries and Dependency Management

### 12.1 Explicit Boundaries

A service should have:

- Clear responsibility
- Clear ownership
- Clear contracts
- Clear data boundaries
- Clear operational responsibility

---

### 12.2 Dependency Direction

Prefer predictable dependency direction:

```text
Service A
    │
    ▼
Contract
    │
    ▼
Service B
```

Avoid uncontrolled dependency graphs:

```text
A ↔ B ↔ C
↑       ↓
└── D ←─┘
```

---

### 12.3 Dependency Graph

The architecture should explicitly model:

```text
Service
   │
   ├── API Dependencies
   ├── Event Dependencies
   ├── Database Dependencies
   ├── Infrastructure Dependencies
   └── External Dependencies
```

Dependency graphs should be observable and periodically reviewed.

---

### 12.4 Dependency Depth

Long synchronous chains increase latency and failure propagation:

```text
A → B → C → D → E
```

Prefer shorter dependency paths where practical.

---

## 13. Deployment and Operational Architecture

### 13.1 Independent Deployment

A distributed component should ideally be deployable without requiring simultaneous deployment of unrelated components.

This requires:

- Backward-compatible contracts
- Versioning
- Deployment sequencing
- Feature flags
- Compatibility testing

---

### 13.2 Backward Compatibility

During deployment:

```text
Version N
    +
Version N+1
```

may coexist.

Contracts must therefore tolerate mixed versions.

---

### 13.3 Health Checks

Distinguish between:

```text
Liveness
Readiness
Dependency Health
```

A service being alive does not mean it can successfully process requests.

---

### 13.4 Graceful Shutdown

Distributed workers should stop accepting new work and finish or safely abandon in-flight work.

Conceptually:

```text
Running
   │
   ▼
Stopping
   │
   ▼
Drain
   │
   ▼
Shutdown
```

---

### 13.5 Deployment Strategies

Possible strategies include:

```text
Rolling Deployment
Blue/Green
Canary
Feature Flags
Shadow Traffic
```

The architecture should support safe coexistence of versions.

---

## 14. Observability

Distributed systems require observability across boundaries.

### 14.1 Distributed Tracing

A request should be traceable across services:

```text
Trace
 │
 ├── API
 │
 ├── Orders
 │    └── Database
 │
 ├── Payments
 │    └── Database
 │
 └── Inventory
      └── Database
```

---

### 14.2 Correlation

Important identifiers include:

```text
TraceId
CorrelationId
CausationId
RequestId
OperationId
MessageId
```

These should be propagated consistently.

---

### 14.3 Metrics

Important distributed-system metrics include:

```text
Request Rate
Error Rate
Latency
Timeout Rate
Retry Rate
Circuit Breaker State
Queue Depth
Consumer Lag
Dependency Availability
Resource Utilization
```

The system should expose both local and end-to-end metrics.

---

### 14.4 Dependency Monitoring

Monitor dependencies independently:

```text
Service A
 ├── Service B
 ├── Service C
 ├── Database
 └── External API
```

A service can be healthy locally while its dependencies are degraded.

---

### 14.5 Logging

Logs should be structured and correlated.

Example:

```json
{
  "service": "OrderService",
  "traceId": "abc123",
  "correlationId": "cor-456",
  "operation": "CreateOrder",
  "dependency": "PaymentService",
  "durationMs": 142,
  "result": "success"
}
```

---

## 15. Security

Distributed boundaries increase the security surface.

Important concerns include:

- Authentication
- Authorization
- Service-to-service identity
- TLS
- Certificate management
- Secret management
- Network segmentation
- API authorization
- Message authorization
- Audit logging
- Tenant isolation

### Service Identity

Services should have explicit identities.

```text
OrderService
     │
     │ authenticated request
     ▼
PaymentService
```

Trust should not be based solely on network location.

---

### Zero Trust Principles

Do not assume:

```text
Internal = Trusted
```

Each request should be evaluated according to:

- Identity
- Authorization
- Context
- Resource
- Action

---

## 16. Scalability

Distributed architecture can enable independent scaling.

Example:

```text
Orders
  │
  ├── 3 instances
  │
Payments
  │
  └── 10 instances
```

Each workload can scale according to its own demand.

However, scaling a component may expose a downstream bottleneck:

```text
100 API instances
       │
       ▼
10 Database Connections
       │
       ▼
Database
```

The architecture must therefore consider the entire dependency chain.

---

### 16.1 Horizontal Scaling

Prefer stateless components where practical:

```text
Load Balancer
     │
 ┌───┼───┐
 ▼   ▼   ▼
 A   A   A
```

State should be externalized when independent instance scaling is required.

---

### 16.2 Capacity Planning

Consider:

```text
CPU
Memory
Network
Connections
Storage
Queue Throughput
Database Throughput
External Dependency Capacity
```

Scaling one dimension does not necessarily increase overall system capacity.

---

## 17. Architectural Invariants

The following rules should become automated architectural constraints where possible.

### Explicit Ownership

Every distributed component must have a clearly defined business and operational owner.

### Explicit Contracts

All cross-boundary communication must use explicit contracts.

### No Direct Database Coupling

A component must not directly modify another component's owned data.

### Bounded Remote Calls

Every synchronous remote call must have a defined timeout.

### Idempotent Mutations

State-changing operations must support idempotency where retries or duplicate delivery are possible.

### Failure Isolation

Failure of one component must not unnecessarily terminate unrelated components.

### Observable Boundaries

Cross-service operations must be traceable.

### Version Compatibility

Components must tolerate supported versions of their contracts.

### Controlled Dependencies

Dependencies between distributed components must be explicit and intentional.

### No Unbounded Retries

Retry behavior must have explicit limits and backoff.

### Graceful Degradation

Where business requirements permit it, non-critical dependency failures should degrade functionality rather than cause total system failure.

---

## 18. Failure Modes and Architectural Smells

### 18.1 Distributed Monolith

Services are deployed separately but cannot evolve independently.

Symptoms include:

- Synchronized deployments
- Shared database schemas
- Long synchronous chains
- Strong temporal coupling
- Shared internal libraries
- Cross-service transactions everywhere

---

### 18.2 Nano-Services

A business capability is split into excessively small services.

```text
CreateUserService
ValidateUserService
LoadUserService
SaveUserService
```

This creates excessive network and operational overhead.

---

### 18.3 Shared Database Coupling

Multiple services directly depend on the same tables.

One schema change can break many services.

---

### 18.4 Synchronous Dependency Chain

```text
A → B → C → D → E
```

This increases:

- Latency
- Failure propagation
- Operational coupling
- Debugging complexity

---

### 18.5 Chatty Services

A service performs many small remote calls:

```text
A → B
A → B
A → B
A → B
A → B
```

Network communication becomes the dominant cost.

---

### 18.6 Distributed Big Ball of Mud

Services have unclear responsibilities and uncontrolled dependencies.

Distribution hides the complexity instead of reducing it.

---

### 18.7 False Independence

A component appears independent but depends on:

- Shared database
- Shared deployment
- Shared configuration
- Shared runtime assumptions
- Internal libraries

The deployment boundary does not represent a real architectural boundary.

---

### 18.8 Over-Reliance on Distributed Transactions

Using distributed transactions to compensate for poor ownership boundaries increases coordination complexity.

Prefer clear ownership and explicit workflows.

---

## 19. Testing Strategy

Distributed architecture requires testing beyond unit tests.

### 19.1 Unit Tests

Test:

- Domain behavior
- Business rules
- Local services
- Failure handling
- Retry policies

---

### 19.2 Contract Tests

Verify compatibility between services:

```text
Producer
   │
   ▼
Contract
   │
   ▼
Consumer
```

Test:

- Request schema
- Response schema
- Event schema
- Required fields
- Version compatibility
- Error semantics

---

### 19.3 Integration Tests

Verify:

```text
Service
  ↓
Database
  ↓
Message Broker
  ↓
Consumer
```

---

### 19.4 Architecture Tests

Verify architectural constraints:

```text
Service A cannot access Service B database
Domain cannot depend on Infrastructure
External contracts must be versioned
Forbidden dependencies do not exist
```

---

### 19.5 Resilience Tests

Deliberately introduce:

```text
Latency
Timeout
Service Failure
Network Failure
Database Failure
Message Duplication
Message Delay
Resource Exhaustion
```

Observe the resulting behavior.

---

### 19.6 Chaos Testing

Chaos experiments deliberately introduce controlled failures.

Examples:

```text
Kill Service Instance
Introduce Network Latency
Block Dependency
Exhaust Connection Pool
Restart Database
Drop Messages
```

The goal is to validate resilience assumptions.

---

### 19.7 Load Testing

Measure:

```text
Throughput
Latency
Error Rate
CPU
Memory
Network
Database Load
Queue Lag
```

Test both normal and degraded conditions.

---

## 20. Evolution and Migration

Distributed architecture should evolve incrementally.

### 20.1 Extracting a Component

A common evolution path is:

```text
Monolith
   │
   ▼
Identify Boundary
   │
   ▼
Create Internal Module
   │
   ▼
Define Contract
   │
   ▼
Extract Deployment
   │
   ▼
Operate Independently
```

The architectural boundary should exist logically before becoming physically distributed.

---

### 20.2 Strangler Evolution

A legacy capability can gradually move behind a new boundary:

```text
                Gateway
                   │
          ┌────────┴────────┐
          ▼                 ▼
     New Service        Legacy System
```

Traffic gradually moves toward the new component.

---

### 20.3 Contract-First Migration

Before extraction:

1. Define the contract
2. Introduce compatibility
3. Implement the new component
4. Redirect traffic
5. Observe
6. Remove legacy dependency

---

### 20.4 Data Migration

Data migration should be separated from service extraction when possible.

Possible strategies include:

```text
Dual Write
Backfill
CDC
Replication
Incremental Migration
Cutover
```

Each introduces consistency and operational considerations.

---

## 21. .NET Implementation Notes

Modern .NET provides strong primitives for distributed systems.

### 21.1 ASP.NET Core

Useful for:

- REST APIs
- gRPC
- Middleware
- Authentication
- Authorization
- Health checks
- OpenTelemetry integration

---

### 21.2 gRPC

Useful for strongly typed internal service communication where low latency and efficient serialization are important.

```text
Service A
   │
   │ gRPC
   ▼
Service B
```

---

### 21.3 HttpClientFactory

Use `IHttpClientFactory` for managed HTTP client lifetimes and centralized configuration.

It can be combined with resilience policies.

---

### 21.4 Resilience Pipelines

Modern .NET applications can use resilience pipelines for:

```text
Timeout
Retry
Circuit Breaker
Rate Limiting
Fallback
```

Policies should be explicit and aligned with the semantics of each dependency.

---

### 21.5 BackgroundService

Use `BackgroundService` for:

```text
Message Consumers
Scheduled Processing
Outbox Publishers
Background Workflows
```

---

### 21.6 OpenTelemetry

Distributed tracing and metrics should use standardized telemetry.

A typical flow:

```text
ASP.NET Core
    │
    ▼
OpenTelemetry
    │
    ├──► Traces
    ├──► Metrics
    └──► Logs
```

---

### 21.7 Aspire

For local distributed application development, .NET Aspire can provide:

- Service orchestration
- Dependency modeling
- Local service discovery
- Configuration
- Telemetry
- Developer dashboard

It can make distributed architecture easier to run and observe during development.

---

### 21.8 Strongly Typed Contracts

Prefer explicit DTOs and contracts:

```csharp
public sealed record PaymentRequest(
    Guid OrderId,
    decimal Amount,
    string Currency);
```

Avoid leaking domain entities directly across service boundaries.

---

## 22. Production Checklist and Architecture Experiments

### 22.1 Architecture

- [ ] Every component has a clear responsibility
- [ ] Ownership is explicit
- [ ] Boundaries are intentional
- [ ] Dependencies are documented
- [ ] Data ownership is explicit
- [ ] Communication styles are intentional

### 22.2 Reliability

- [ ] Timeouts exist
- [ ] Retry policies are explicit
- [ ] Idempotency is implemented
- [ ] Circuit breakers are considered
- [ ] Bulkheads are considered
- [ ] Backpressure is handled
- [ ] Failure isolation is tested

### 22.3 Data

- [ ] Data ownership is explicit
- [ ] Cross-service database access is controlled
- [ ] Consistency requirements are documented
- [ ] Distributed transactions are minimized
- [ ] Reconciliation exists where necessary

### 22.4 Operations

- [ ] Distributed tracing exists
- [ ] Metrics exist
- [ ] Structured logging exists
- [ ] Health checks exist
- [ ] Alerting exists
- [ ] Runbooks exist
- [ ] Capacity limits are understood

### 22.5 Security

- [ ] Service identities exist
- [ ] Authentication is enforced
- [ ] Authorization is enforced
- [ ] TLS is used
- [ ] Secrets are managed securely
- [ ] Network access is controlled
- [ ] Auditability exists

### 22.6 Deployment

- [ ] Independent deployment is possible where required
- [ ] Contracts are backward compatible
- [ ] Mixed versions are supported
- [ ] Rollback strategy exists
- [ ] Graceful shutdown exists
- [ ] Deployment health is observable

---

### 22.7 Architecture Experiment Methodology

Each experiment should follow:

```text
Problem
   ↓
Boundary Design
   ↓
Communication Model
   ↓
Data Ownership
   ↓
Implementation
   ↓
Architecture Tests
   ↓
Failure Injection
   ↓
Load / Performance Test
   ↓
Observability
   ↓
Trade-offs
   ↓
ADR
```

Recommended experiments:

#### Experiment 01 - Two Distributed Services

```text
Order Service
      │
      │ HTTP / gRPC
      ▼
Payment Service
```

Measure:

- Latency
- Failure behavior
- Timeout behavior
- Dependency availability

---

#### Experiment 02 - Independent Databases

Implement:

```text
Order Service ──► Order DB

Payment Service ──► Payment DB
```

Verify that services cannot directly access each other's data.

---

#### Experiment 03 - Failure Injection

Introduce:

```text
Payment unavailable
Payment slow
Network timeout
Database unavailable
```

Observe the effect on Orders.

---

#### Experiment 04 - Retry and Circuit Breaker

Create a failing dependency.

Measure:

```text
Failure
 ↓
Retry
 ↓
Retry
 ↓
Circuit Open
```

Observe whether the failure remains isolated.

---

#### Experiment 05 - Idempotent Request

Send the same request multiple times:

```text
Request A
Request A
Request A
```

Verify that only one business effect occurs.

---

#### Experiment 06 - Event-Based Communication

Replace a synchronous dependency with:

```text
Order Service
      │
      ▼
OrderPlaced
      │
      ▼
Payment Consumer
```

Measure changes in:

- Coupling
- Latency
- Consistency
- Failure behavior

---

#### Experiment 07 - Saga

Implement:

```text
Create Order
     ↓
Reserve Inventory
     ↓
Authorize Payment
     ↓
Confirm Order
```

Then introduce a failure and implement compensation.

---

#### Experiment 08 - Distributed Tracing

Trace:

```text
Client
 ↓
Gateway
 ↓
Order Service
 ↓
Payment Service
 ↓
Payment DB
```

Verify that a single trace can reconstruct the complete flow.

---

#### Experiment 09 - Load and Scaling

Generate increasing load.

Measure:

```text
Request Rate
Latency
Error Rate
CPU
Memory
Database Load
Network Load
```

Then scale individual services independently.

---

#### Experiment 10 - Chaos Experiment

Introduce:

```text
Service Crash
Network Latency
Network Partition
Database Restart
Connection Exhaustion
Message Delay
```

Document:

```text
Expected Behavior
Observed Behavior
Failure Blast Radius
Recovery Time
Required Improvements
```

---

## 23. Key Takeaways

1. **A distributed architecture introduces real process, network, failure, and deployment boundaries.**

2. **The network must be treated as unreliable.**

3. **Remote calls are fundamentally different from local method calls.**

4. **Partial failure is a normal operating condition, not an exceptional theoretical case.**

5. **Every distributed component should have explicit responsibility and ownership.**

6. **Data ownership is one of the most important architectural boundaries.**

7. **Synchronous communication creates temporal and availability coupling.**

8. **Asynchronous communication provides temporal decoupling but introduces consistency and operational complexity.**

9. **Distributed transactions should not be the default mechanism for coordinating business workflows.**

10. **Idempotency is essential whenever retries or duplicate delivery are possible.**

11. **Timeouts are mandatory protection against unbounded dependency latency.**

12. **Retries must be bounded and designed to avoid retry storms and failure amplification.**

13. **Circuit breakers, bulkheads, backpressure, and load shedding help control failure blast radius.**

14. **Distributed tracing is essential for understanding behavior across service boundaries.**

15. **Independent deployment requires compatible contracts and deliberate version evolution.**

16. **Scaling one component does not guarantee scaling the entire system.**

17. **A distributed system can easily become a distributed monolith if boundaries are poorly designed.**

18. **Distribution should solve a real architectural problem rather than become an architectural goal itself.**

19. **The strongest distributed architectures make ownership, communication, consistency, failure, and operational behavior explicit.**

20. **Architecture becomes executable when these assumptions are continuously validated through architecture tests, failure experiments, load tests, and production observability.**
