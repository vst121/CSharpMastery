# Event-Based Architecture

## 1. Essence

**Event-Based Architecture** structures a system around the production, distribution, and consumption of events.

An event represents something that **has happened**:

```text
OrderPlaced
PaymentAuthorized
ShipmentCreated
CustomerRegistered
TransactionProcessed
```

Instead of requiring the producer to know and directly invoke every consumer, the producer publishes an event and interested consumers react to it.

The fundamental relationship is:

```text
Producer
   │
   │ publishes
   ▼
  Event
   │
   ├──────────────► Consumer A
   │
   ├──────────────► Consumer B
   │
   └──────────────► Consumer C
```

The architecture therefore shifts communication from:

```text
"Call this component and wait for its response."
```

toward:

```text
"Something happened. Whoever is interested can react."
```

The primary architectural goals are:

- Loose coupling
- Asynchronous communication
- Independent evolution
- Scalable event processing
- Explicit business or system facts
- Temporal decoupling
- Failure isolation
- Extensibility

Event-Based Architecture is not simply about using a message broker.

It is about making **events first-class architectural contracts**.

---

## 2. Problem

Traditional synchronous systems often create strong coupling between components:

```text
OrderService
    │
    ├──► PaymentService
    ├──► InventoryService
    ├──► NotificationService
    └──► AnalyticsService
```

The producer must know:

- who needs the information
- how to call them
- where they are located
- what API they expose
- whether they are available
- how failures should be handled
- how retries should work

As the system grows, the dependency graph becomes increasingly complex.

Event-Based Architecture introduces an asynchronous communication model:

```text
OrderService
     │
     │ OrderPlaced
     ▼
 Event Infrastructure
     │
     ├──► Payment
     ├──► Inventory
     ├──► Notification
     └──► Analytics
```

The producer does not need direct knowledge of every consumer.

This allows new consumers to be added without necessarily modifying the producer.

---

## 3. Intent

Event-Based Architecture aims to:

- Decouple producers from consumers
- Enable asynchronous communication
- Support independent component evolution
- Distribute information to multiple consumers
- Reduce direct dependency chains
- Support scalable processing
- Model important occurrences explicitly
- Enable event-driven workflows
- Isolate consumer failures
- Support integration across architectural boundaries
- Enable independent deployment where appropriate

The central architectural statement is:

> **Something happened.**

The architecture then allows interested components to react to that fact.

---

## 4. Core Principles

### 4.1 Events Represent Facts

An event normally describes something that has already happened.

Prefer:

```text
OrderPlaced
PaymentAuthorized
CustomerRegistered
InvoiceIssued
```

over:

```text
PlaceOrder
AuthorizePayment
CreateInvoice
```

A useful distinction is:

```text
Command
    "Please do this."

Event
    "This happened."
```

Commands request behavior.

Events communicate facts.

---

### 4.2 Producers Should Not Depend on Consumers

A producer publishes an event without requiring knowledge of every consumer.

```text
OrderService
     │
     ▼
 OrderPlaced
     │
     ├──► Payment
     ├──► Inventory
     ├──► Notification
     └──► Analytics
```

Adding a new consumer should ideally not require changing the producer.

---

### 4.3 Consumers Own Their Reaction

Each consumer determines what the event means for its own responsibility.

For example:

```text
OrderPlaced
```

may trigger:

```text
PaymentService
    → initiate payment

InventoryService
    → reserve inventory

NotificationService
    → send confirmation

AnalyticsService
    → record business metric
```

The producer should not encode these reactions.

---

### 4.4 Events Are Contracts

Once an event crosses an architectural boundary, its schema becomes a contract.

Example:

```json
{
  "eventId": "01J...",
  "eventType": "OrderPlaced",
  "version": 1,
  "occurredAt": "2026-10-02T10:30:00Z",
  "orderId": "ORD-123",
  "customerId": "CUS-456",
  "totalAmount": 249.99,
  "currency": "EUR"
}
```

Event contracts should be:

- Explicit
- Versioned
- Documented
- Testable
- Observable
- Evolvable

---

### 4.5 Delivery Is Not Processing

Successfully delivering an event does not mean that the business operation succeeded.

There are multiple stages:

```text
Event Published
      │
      ▼
Event Delivered
      │
      ▼
Event Received
      │
      ▼
Event Processed
      │
      ▼
Business Effect Committed
```

Each stage can fail independently.

The architecture must therefore distinguish:

- Publication
- Transport
- Delivery
- Consumption
- Processing
- Persistence
- Acknowledgement

---

### 4.6 Asynchronous Does Not Mean Decoupled

A system can use asynchronous messaging and still be tightly coupled.

For example:

```text
A → B → C → D → E
```

If every component requires a specific event sequence from another component, the architecture may still have strong temporal and semantic coupling.

Asynchrony is a mechanism.

Decoupling is an architectural property.

---

## 5. Architectural Model

A typical Event-Based Architecture contains:

```text
┌─────────────────────┐
│       Producer      │
└──────────┬──────────┘
           │
           │ Event
           ▼
┌─────────────────────┐
│   Event Transport   │
│ / Broker / Stream   │
└──────────┬──────────┘
           │
     ┌─────┼─────┐
     │     │     │
     ▼     ▼     ▼
 Consumer A  Consumer B  Consumer C
     │     │     │
     ▼     ▼     ▼
   State / Side Effects
```

The event infrastructure may provide:

- Buffering
- Persistence
- Routing
- Delivery
- Partitioning
- Ordering
- Retry
- Replay
- Fan-out
- Consumer isolation
- Backpressure

The infrastructure is responsible for transporting events.

The business meaning of an event should remain independent of the transport technology where practical.

---

## 6. Building Blocks

### 6.1 Event

An event is an immutable representation of something that happened.

```csharp
public sealed record OrderPlaced(
    Guid EventId,
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    string Currency,
    DateTimeOffset OccurredAt);
```

Events should normally be immutable.

---

### 6.2 Event Producer

The producer is responsible for publishing an event.

```csharp
await eventPublisher.PublishAsync(
    new OrderPlaced(
        EventId: Guid.NewGuid(),
        OrderId: order.Id,
        CustomerId: order.CustomerId,
        TotalAmount: order.TotalAmount,
        Currency: "EUR",
        OccurredAt: clock.UtcNow));
```

The producer owns the fact that the event occurred.

It should not own the implementation details of every consumer.

---

### 6.3 Event Consumer

A consumer subscribes to and processes an event.

```csharp
public sealed class OrderPlacedHandler
{
    public async Task Handle(
        OrderPlaced @event,
        CancellationToken cancellationToken)
    {
        // React to the event
    }
}
```

A consumer should own its own processing logic and state.

---

### 6.4 Event Broker / Transport

The infrastructure responsible for transporting events.

Examples include:

- Apache Kafka
- Azure Event Hubs
- Azure Service Bus
- RabbitMQ
- Amazon SNS/SQS
- NATS
- Redis Streams

The domain should not become unnecessarily coupled to broker-specific APIs.

---

### 6.5 Subscription

A subscription defines which events a consumer receives.

```text
OrderPlaced
     │
     ├──► PaymentSubscription
     ├──► InventorySubscription
     └──► NotificationSubscription
```

Subscriptions should be independently manageable where appropriate.

---

### 6.6 Consumer Group

Multiple instances of a consumer can form a logical processing group.

```text
             OrderPlaced
                  │
                  ▼
          ┌───────────────┐
          │ Consumer Group│
          └───────┬───────┘
                  │
          ┌───────┼───────┐
          ▼       ▼       ▼
       Worker 1 Worker 2 Worker 3
```

Consumer groups enable horizontal processing.

---

### 6.7 Event Handler

The event handler interprets an event and performs the consumer's responsibility.

```text
Transport
    │
    ▼
Deserializer
    │
    ▼
Event Handler
    │
    ├──► Domain Logic
    ├──► Database
    ├──► External Service
    └──► New Event
```

Handlers should remain focused on the consumer's responsibility.

---

## 7. Event Categories

### 7.1 Domain Events

Represent important business occurrences inside a domain model.

Examples:

```text
OrderPlaced
PaymentAuthorized
CreditLimitExceeded
```

They primarily express domain meaning.

---

### 7.2 Integration Events

Represent information intentionally exposed across an architectural boundary.

Example:

```text
CustomerRegisteredIntegrationEvent
```

Integration events should be treated as external contracts.

They should not expose internal implementation details unnecessarily.

---

### 7.3 Infrastructure Events

Represent technical occurrences.

Examples:

```text
FileUploaded
JobCompleted
NodeRegistered
CacheInvalidated
```

These describe infrastructure activity rather than business facts.

---

## 8. Event Contract Design

A good event contract should answer:

- What happened?
- When did it happen?
- Which entity or operation was affected?
- Who produced it?
- Which version is this?
- How can consumers identify it uniquely?
- Can it safely be processed more than once?

A useful event envelope may contain:

```text
EventId
EventType
EventVersion
OccurredAt
Producer
CorrelationId
CausationId
Payload
```

Example:

```json
{
  "eventId": "01J...",
  "eventType": "OrderPlaced",
  "eventVersion": 1,
  "occurredAt": "2026-10-02T10:30:00Z",
  "producer": "OrderService",
  "correlationId": "COR-123",
  "causationId": "CMD-456",
  "payload": {
    "orderId": "ORD-123",
    "customerId": "CUS-456"
  }
}
```

The envelope and payload should have clearly defined ownership.

---

## 9. Event Identity and Causality

### 9.1 Event Identity

Every externally meaningful event should have a unique identity.

```text
EventId = identity of the event
```

This is different from:

```text
OrderId = identity of the business entity
```

A single order may generate:

```text
OrderPlaced
PaymentAuthorized
OrderConfirmed
OrderShipped
OrderCompleted
```

Each event has its own identity.

---

### 9.2 Correlation

A correlation identifier groups events belonging to the same business flow.

```text
CorrelationId
    └── identifies the overall business flow
```

---

### 9.3 Causation

A causation identifier identifies the event or action that caused another event.

```text
OrderPlaced
     │
     ▼
PaymentRequested
     │
     ▼
PaymentAuthorized
```

Conceptually:

```text
EventId
    → identifies this event

CorrelationId
    → identifies the overall flow

CausationId
    → identifies what caused this event
```

This information is critical for distributed tracing and operational debugging.

---

## 10. Event Delivery and Processing

### 10.1 Delivery Semantics

Common delivery models include:

```text
At-most-once
At-least-once
Effectively-once
Exactly-once
```

The architecture must not assume that message delivery guarantees automatically provide exactly-once business processing.

For example:

```text
Event delivered
      │
      ▼
Consumer processes event
      │
      ▼
Database commit succeeds
      │
      ▼
Acknowledgement fails
```

The broker may redeliver the event.

Therefore:

```text
Duplicate delivery
        ≠
Duplicate business effect
```

---

### 10.2 Acknowledgement

Acknowledgement should happen only when the consumer has reached the intended processing boundary.

Conceptually:

```text
Receive
   │
   ▼
Process
   │
   ▼
Commit
   │
   ▼
Acknowledge
```

The exact semantics depend on the messaging infrastructure.

---

### 10.3 Ordering

Ordering is not automatically guaranteed.

Possible ordering scopes include:

```text
Global ordering
Partition ordering
Entity ordering
Consumer-local ordering
```

For example, events for one order may need to remain ordered:

```text
OrderPlaced
    ↓
PaymentAuthorized
    ↓
OrderConfirmed
```

while unrelated orders can be processed concurrently.

A common strategy is partitioning by business key:

```text
PartitionKey = OrderId
```

The architectural question is:

> What must be ordered, and what can be processed concurrently?

---

## 11. State, Consistency, and Ownership

### 11.1 Consumer-Owned State

Consumers should normally own the state they maintain as a consequence of events.

For example:

```text
OrderPlaced
     │
     ├──► Payment Service
     │       └── Payment State
     │
     ├──► Inventory Service
     │       └── Inventory State
     │
     └──► Analytics Service
             └── Analytics State
```

Consumers should avoid directly modifying another consumer's internal state.

---

### 11.2 Eventual Consistency

Event-based systems frequently introduce temporal separation between state changes.

For example:

```text
Order = Placed
Payment = Pending
Inventory = Reserving
Notification = Not Sent
```

These states may exist simultaneously for a period of time.

The architecture must explicitly define:

- Acceptable consistency windows
- Intermediate states
- Retry behavior
- User-visible behavior
- Reconciliation

---

### 11.3 Consistency Boundaries

Not every operation requires immediate global consistency.

The design should identify:

```text
Strong consistency boundary
        │
        ▼
Eventual consistency boundary
```

This should be an intentional business decision rather than an accidental side effect of messaging.

---

## 12. Reliability and Failure Handling

Failures are normal in distributed event processing.

Possible failures include:

- Application errors
- Database outages
- Network failures
- Dependency failures
- Malformed events
- Schema incompatibility
- Timeouts
- Resource exhaustion
- Poison messages

A production architecture should define a failure strategy.

```text
Event
  │
  ▼
Consumer
  │
  ├── Success ─────────► Acknowledge
  │
  └── Failure
       │
       ├── Retry
       │
       ├── Delayed Retry
       │
       ├── Dead Letter
       │
       └── Alert
```

---

### 12.1 Retry

Retry policies should distinguish transient failures from permanent failures.

Transient failures:

```text
Database temporarily unavailable
Network timeout
Temporary dependency failure
```

Permanent failures:

```text
Invalid event schema
Missing required data
Unsupported version
Permanent business validation failure
```

Retry policies should define:

- Maximum attempts
- Backoff
- Jitter
- Retryable failures
- Non-retryable failures
- Dead-letter behavior

---

### 12.2 Dead-Letter Handling

Events that cannot be processed successfully after the retry policy should be isolated.

```text
Main Stream
    │
    ▼
Consumer
    │
    ├── Success ───────► Completed
    │
    └── Failure
          │
          ▼
      Retry Policy
          │
          ▼
      Dead Letter
```

Dead-letter events should remain observable and recoverable.

A dead-letter queue should not become a permanent graveyard.

---

### 12.3 Poison Events

A poison event repeatedly causes processing failure.

```text
Event
  │
  ▼
Consumer
  │
  ▼
Failure
  │
  ▼
Retry
  │
  ▼
Failure
```

Controls include:

- Retry limits
- Exponential backoff
- Dead-letter queues
- Consumer isolation
- Alerting
- Quarantine workflows

---

## 13. Idempotency, Transactions, and Outbox

### 13.1 Idempotent Consumers

Consumers should normally tolerate duplicate delivery.

A common approach uses a processed-event store:

```text
ProcessedEvents
-------------------------
EventId
Consumer
ProcessedAt
```

Processing can follow:

```text
Receive Event
     │
     ▼
Check EventId
     │
 ┌───┴────┐
 │        │
Seen     New
 │        │
 ▼        ▼
Skip    Process
          │
          ▼
      Record EventId
```

A database uniqueness constraint can enforce the identity rule.

---

### 13.2 Transactional Consumer Processing

A consumer may need atomicity between:

```text
Event Processing
+
Business State Change
+
Processed Event Registration
```

Conceptually:

```text
BEGIN TRANSACTION

Check EventId

Apply business change

Record EventId

COMMIT
```

This helps prevent duplicate business effects.

---

### 13.3 Transactional Publishing

A common failure occurs when a database update and event publication happen independently.

```text
BEGIN DATABASE TRANSACTION

Update Order

COMMIT

Publish OrderPlaced
```

Failure can occur between:

```text
Database Commit
      │
      X
      │
Event Publication
```

The business state changed but the event was never published.

---

### 13.4 Transactional Outbox

The Outbox Pattern addresses this problem by storing the business change and outgoing event in the same database transaction.

```text
                 Database
              ┌──────────────┐
              │ Order         │
              │ Outbox Event  │
              └──────┬───────┘
                     │
                Same Transaction
                     │
                     ▼
               Outbox Publisher
                     │
                     ▼
                   Broker
```

Conceptually:

```text
BEGIN TRANSACTION

Update Order

Insert OutboxEvent

COMMIT
```

A separate publisher then delivers the event.

The important guarantee is:

> The business state change and the intent to publish are committed together.

---

## 14. Event Persistence, Replay, and Backpressure

### 14.1 Event Persistence

Events may be:

- Transient
- Temporarily persisted
- Durably persisted
- Retained for replay

Retention should be an explicit architectural decision.

Questions include:

- How long must events be retained?
- Are events replayable?
- Is replay safe?
- Can consumers rebuild their state?
- What storage cost is acceptable?

---

### 14.2 Replay

Replay means processing historical events again.

Possible uses:

```text
Rebuild a projection
Recover derived state
Create a new consumer
Fix a processing bug
Backfill analytics
Recover from corruption
```

Replay requires:

- Deterministic processing
- Idempotency
- Version compatibility
- Side-effect control
- Replay isolation
- Observability

A consumer that sends an email for every event may not be safely replayable.

---

### 14.3 Backpressure

Consumers may process events more slowly than producers generate them.

```text
Producer
  │
  │ 10,000 events/sec
  ▼
Broker
  │
  │ 2,000 events/sec
  ▼
Consumer
```

The resulting backlog must be visible.

Important metrics include:

```text
Queue Depth
Consumer Lag
Processing Rate
Arrival Rate
Oldest Event Age
```

Possible strategies include:

- Scaling consumers
- Controlling producer throughput
- Batching
- Partitioning
- Load shedding
- Prioritization
- Flow control

---

## 15. Event Evolution and Schema Management

### 15.1 Versioning

Events evolve.

Version 1:

```json
{
  "orderId": "123",
  "total": 100
}
```

Version 2:

```json
{
  "orderId": "123",
  "total": 100,
  "currency": "EUR"
}
```

Possible strategies include:

- Backward-compatible evolution
- Explicit event versions
- Schema negotiation
- Parallel versions
- Consumer migration
- Upcasting

---

### 15.2 Schema Evolution

Safer changes often include:

```text
Add optional field
Add metadata
Add backward-compatible information
```

Riskier changes include:

```text
Rename field
Change type
Remove field
Change semantics
```

Technical compatibility is not enough.

The important question is:

> Does the consumer still interpret the event correctly?

Semantic compatibility matters as much as serialization compatibility.

---

### 15.3 Event Ownership

Every externally meaningful event should have a clearly defined owner.

The owner is responsible for:

- Meaning
- Contract
- Versioning
- Documentation
- Compatibility
- Deprecation

Without ownership, event contracts tend to become accidental shared infrastructure.

---

## 16. Architectural Invariants

The following rules should become architectural constraints wherever possible.

### Event Immutability

Published events must not be modified after publication.

### Unique Event Identity

Every externally meaningful event must have a unique identity.

### Explicit Contracts

Integration events must use explicit and versioned contracts.

### Producer Independence

Adding a consumer should not require modifying the producer.

### Consumer Ownership

Consumers own their reaction and internal state.

### Consumer Idempotency

Consumers must tolerate duplicate delivery when the delivery model permits duplicates.

### Controlled Ordering

Ordering requirements must be explicit.

### Failure Isolation

One consumer failure should not unnecessarily stop unrelated consumers.

### Explicit Retry Policy

Every asynchronous consumer must have a defined retry strategy.

### Observable Processing

Event processing must be traceable from publication through consumption.

### Infrastructure Isolation

Domain and application code should not depend directly on broker-specific implementation details.

---

## 17. Failure Modes and Architectural Smells

### 17.1 Event Everything

Every method call becomes an event:

```text
ValidateCustomer
CalculatePrice
LoadProduct
GetAddress
```

This creates unnecessary asynchronous complexity.

---

### 17.2 Events Used as Commands

```text
ProcessOrder
SendEmail
CreateInvoice
```

These are commands rather than historical facts.

---

### 17.3 Generic Events

```text
DataChanged
ObjectUpdated
SomethingHappened
```

Such events provide little domain meaning.

Prefer explicit events.

---

### 17.4 Event Leakage

Internal implementation details become external contracts.

This creates long-term compatibility costs.

---

### 17.5 Hidden Coupling

The producer appears independent, but consumers depend on undocumented behavior.

Event contracts must be explicit.

---

### 17.6 Distributed Monolith

Components are technically asynchronous but still require a strict sequence to operate:

```text
A → B → C → D → E
```

The system may have replaced synchronous calls with asynchronous messages without actually reducing coupling.

---

### 17.7 Infinite Retry

A poison event is retried indefinitely.

This consumes resources and hides the underlying problem.

---

### 17.8 Uncontrolled Replay

Historical events are replayed without understanding their side effects.

This can cause:

- Duplicate notifications
- Duplicate external operations
- Duplicate business effects
- Inconsistent state

Replay must be an explicit operational capability.

---

### 17.9 Event Storm

Poorly designed event chains can create excessive event traffic:

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
│
▼
A
```

Potential consequences include:

- Event loops
- Cascading load
- Hidden coupling
- Difficult debugging
- Unpredictable latency

Events should represent meaningful architectural facts.

---

## 18. Testing Strategy

Testing should verify both individual behavior and architectural properties.

### 18.1 Unit Tests

Test:

- Event creation
- Domain behavior
- Event handlers
- Business rules
- Idempotency logic

---

### 18.2 Contract Tests

Verify producer and consumer compatibility.

```text
Producer
   │
   ▼
Event Contract
   │
   ▼
Consumer
```

Contract tests should validate:

- Required fields
- Serialization
- Version compatibility
- Semantic expectations

---

### 18.3 Architecture Tests

Automate architectural invariants.

Examples:

```text
Domain must not reference Kafka
Domain must not reference Infrastructure
Events must be immutable
Integration contracts must be explicit
```

Potential tools:

- NetArchTest
- ArchUnitNET
- Roslyn analyzers
- Custom dependency validation

---

### 18.4 Integration Tests

Test the complete flow:

```text
Publish
   ↓
Transport
   ↓
Consumer
   ↓
Database
   ↓
Acknowledgement
```

---

### 18.5 Failure Tests

Deliberately inject failures:

```text
Broker unavailable
Database unavailable
Consumer crash
Duplicate event
Out-of-order event
Malformed event
Poison event
Network timeout
```

The architecture should demonstrate its behavior under failure.

---

### 18.6 Replay Tests

If replay is supported, verify:

- Deterministic processing
- Idempotency
- Side-effect control
- Schema compatibility
- State reconstruction

---

## 19. Observability and Security

### 19.1 Observability

Event-based systems require end-to-end observability.

Important signals include:

```text
Events Published
Events Consumed
Processing Latency
Consumer Lag
Retry Count
Dead-Letter Count
Failure Rate
Throughput
Backlog Size
Oldest Event Age
```

Distributed tracing should connect:

```text
Request
  │
  ▼
Command
  │
  ▼
Event
  │
  ▼
Consumer
  │
  ▼
Database
  │
  ▼
Next Event
```

Useful identifiers include:

```text
TraceId
CorrelationId
CausationId
EventId
```

---

### 19.2 Metrics

A production consumer should expose metrics such as:

```text
events.published
events.consumed
events.failed
events.retried
events.dead_lettered

consumer.lag
consumer.processing_duration
consumer.throughput

event.age
event.backlog
```

The goal is to understand not only whether a service is running, but where the event flow is slowing down or failing.

---

### 19.3 Logging

Structured logs should contain event context.

```json
{
  "eventType": "OrderPlaced",
  "eventId": "01J...",
  "correlationId": "COR-123",
  "consumer": "PaymentService",
  "attempt": 2,
  "durationMs": 84
}
```

Avoid logging sensitive event payloads unnecessarily.

---

### 19.4 Security

Events may contain sensitive business or personal information.

Consider:

- Encryption in transit
- Encryption at rest
- Access control
- Topic/stream permissions
- Consumer authorization
- Payload minimization
- Retention policies
- Auditability
- Sensitive-field protection
- Tenant isolation

Consumers should receive only the events they are authorized to process.

---

## 20. Scalability and Evolution

### 20.1 Horizontal Scalability

Event consumers can often scale horizontally:

```text
                Event Stream
                     │
          ┌──────────┼──────────┐
          ▼          ▼          ▼
       Worker 1   Worker 2   Worker 3
```

Scalability depends on:

- Partitioning
- Consumer groups
- Workload characteristics
- Ordering requirements
- Broker capacity
- Database throughput
- Downstream dependencies

Adding more consumers does not help if the database is the bottleneck.

---

### 20.2 Partitioning

Partitioning should balance:

```text
Ordering
+
Parallelism
+
Load Distribution
```

A common strategy is:

```text
PartitionKey = BusinessEntityId
```

For example:

```text
PartitionKey = OrderId
```

This can preserve ordering for a specific order while allowing different orders to process concurrently.

---

### 20.3 Hot Partitions

Poor partition-key selection can create uneven load:

```text
Partition 1 ███████████████████
Partition 2 ██
Partition 3 █
Partition 4 ███
```

The architecture should identify and monitor hot partitions.

---

### 20.4 Event Retention

Long retention increases storage requirements:

```text
Event Volume
      ×
Event Size
      ×
Retention Period
```

Retention should reflect actual requirements:

- Replay
- Audit
- Analytics
- Recovery
- Compliance
- Debugging

Not every event requires indefinite retention.

---

### 20.5 Evolution and Migration

Event-based systems should evolve incrementally.

A typical migration can be:

```text
Version 1
   │
   ▼
Introduce compatible Version 2
   │
   ▼
Migrate consumers
   │
   ▼
Monitor
   │
   ▼
Retire Version 1
```

Do not remove an event contract simply because the producer no longer uses it.

Consumers may still depend on it.

---

## 21. .NET Implementation Notes

Modern .NET provides useful primitives for Event-Based Architecture.

### 21.1 Immutable Events

Records are well suited to immutable event contracts:

```csharp
public sealed record OrderPlaced(
    Guid EventId,
    Guid OrderId,
    DateTimeOffset OccurredAt);
```

---

### 21.2 Strongly Typed Contracts

Prefer explicit event types:

```csharp
public interface IEvent
{
    Guid EventId { get; }
    DateTimeOffset OccurredAt { get; }
}
```

Then:

```csharp
public sealed record OrderPlaced(
    Guid EventId,
    Guid OrderId,
    DateTimeOffset OccurredAt) : IEvent;
```

Strong typing helps move correctness toward compile time.

---

### 21.3 Cancellation

Consumers should respect cancellation:

```csharp
public async Task HandleAsync(
    OrderPlaced @event,
    CancellationToken cancellationToken)
{
    await repository.SaveAsync(
        @event.OrderId,
        cancellationToken);
}
```

---

### 21.4 Background Workers

A .NET consumer can use:

```text
BackgroundService
```

as its hosting abstraction.

Conceptually:

```text
BackgroundService
       │
       ▼
Receive Event
       │
       ▼
Deserialize
       │
       ▼
Handle
       │
       ▼
Commit / Acknowledge
```

---

### 21.5 Typed Configuration

Messaging configuration should use strongly typed options.

```csharp
public sealed class MessagingOptions
{
    public required string Topic { get; init; }
    public required string ConsumerGroup { get; init; }
}
```

---

### 21.6 Compile-Time Architecture Enforcement

For larger systems, source generators or Roslyn analyzers can enforce:

- Event registration
- Naming conventions
- Required metadata
- Serialization contracts
- Versioning rules
- Handler discovery
- Architectural dependencies

The goal is to make architecture increasingly **executable**.

---

## 22. Production Checklist and Architecture Experiments

### 22.1 Event Design

- [ ] Events represent meaningful facts
- [ ] Event names are explicit
- [ ] Event ownership is documented
- [ ] Events are immutable
- [ ] Event identity is defined
- [ ] Event versioning strategy exists
- [ ] Contract compatibility is tested

### 22.2 Delivery

- [ ] Delivery semantics are explicit
- [ ] Duplicate delivery is handled
- [ ] Ordering requirements are documented
- [ ] Partition strategy is defined
- [ ] Consumer groups are defined

### 22.3 Reliability

- [ ] Retry policy exists
- [ ] Backoff is configured
- [ ] Poison events are isolated
- [ ] Dead-letter handling exists
- [ ] Consumer failures are isolated
- [ ] Backpressure is understood

### 22.4 Consistency

- [ ] Eventual consistency boundaries are explicit
- [ ] Transactional publishing is addressed
- [ ] Outbox strategy is considered
- [ ] Consumer idempotency is implemented where required
- [ ] Reconciliation strategy exists

### 22.5 Observability

- [ ] EventId is available
- [ ] CorrelationId is available
- [ ] CausationId is available
- [ ] Distributed tracing exists
- [ ] Consumer lag is monitored
- [ ] Processing latency is measured
- [ ] Dead-letter events are observable

### 22.6 Security

- [ ] Event access is authorized
- [ ] Sensitive data is minimized
- [ ] Transport is encrypted
- [ ] Retention is defined
- [ ] Tenant isolation is considered

### 22.7 Architecture Experiment Methodology

Each experiment should follow:

```text
Problem
   ↓
Architecture
   ↓
Event Model
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
Trade-offs
   ↓
ADR
```

Recommended experiments include:

#### Experiment 01 - Basic Event Flow

```text
OrderService
    ↓
OrderPlaced
    ↓
NotificationService
```

Measure latency, throughput, and failure behavior.

#### Experiment 02 - Multiple Consumers

```text
OrderPlaced
    │
    ├──► Payment
    ├──► Inventory
    └──► Notification
```

Demonstrate consumer independence.

#### Experiment 03 - Duplicate Delivery

Force the same event to be delivered twice.

Expected business behavior:

```text
2 deliveries
      ↓
1 business effect
```

#### Experiment 04 - Retry and Dead Letter

Create a consumer that intentionally fails.

```text
Failure
 ↓
Retry
 ↓
Retry
 ↓
Dead Letter
```

#### Experiment 05 - Transactional Outbox

Implement:

```text
Business Transaction
       +
Outbox Event
       ↓
   Publisher
       ↓
     Broker
```

Then terminate the application at different points and verify event reliability.

#### Experiment 06 - Consumer Lag

Generate events faster than consumers can process them.

Measure:

```text
Producer Rate
Consumer Rate
Queue Depth
Consumer Lag
Oldest Event Age
```

Then scale consumers.

#### Experiment 07 - Ordering

Generate events for multiple orders:

```text
Order A
Order B
Order C
```

Verify:

```text
A1 → A2 → A3
B1 → B2 → B3
C1 → C2 → C3
```

while allowing different orders to process concurrently.

#### Experiment 08 - Replay

Persist events and rebuild a projection from historical events.

Verify:

```text
Current Projection
       ▲
       │
   Replay Events
       ▲
       │
 Event Store
```

#### Experiment 09 - Schema Evolution

Introduce:

```text
OrderPlaced v1
OrderPlaced v2
```

Migrate consumers without breaking existing processing.

#### Experiment 10 - Failure Injection

Introduce:

```text
Broker outage
Database outage
Consumer crash
Duplicate delivery
Delayed delivery
Out-of-order delivery
Malformed event
Poison event
Network timeout
```

Document the observed behavior.

---

## 23. Key Takeaways

1. **An event represents something that happened.**

2. **Producers publish facts without needing to know every consumer.**

3. **Consumers own their reactions and their internal state.**

4. **Events become architectural contracts when they cross meaningful boundaries.**

5. **Asynchronous communication does not automatically mean loose coupling.**

6. **Delivery guarantees and business-processing guarantees are different.**

7. **Duplicate delivery should be expected unless stronger guarantees are explicitly established.**

8. **Idempotency is one of the most important properties of an event consumer.**

9. **Ordering must be explicitly designed and scoped.**

10. **Eventual consistency must be understood and deliberately accepted where appropriate.**

11. **Retries, dead letters, backpressure, and replay are architectural concerns.**

12. **Transactional Outbox addresses the consistency gap between state changes and event publication.**

13. **Event contracts must evolve deliberately and remain semantically compatible.**

14. **Observability must follow an event across its entire lifecycle.**

15. **Failure behavior should be tested, not merely documented.**

16. **Architecture tests should enforce event boundaries, dependency rules, and contract constraints automatically.**

17. **A message broker is infrastructure. Event-Based Architecture is the architectural model built around explicit events, ownership, contracts, processing semantics, and operational behavior.**

18. **The strongest event-based systems make asynchronous behavior explicit, observable, testable, and resilient.**
