# 09 - Synchronous to Asynchronous

## 1. Essence

**Synchronous to Asynchronous** is an architecture evolution strategy for replacing blocking, request-driven interactions with asynchronous communication and processing.

Before:

```text
Caller
  │
  │ Request
  ▼
Service A
  │
  │ synchronous call
  ▼
Service B
  │
  ▼
Response
  │
  ▼
Caller
````

After:

```text
Caller
  │
  │ Command / Event
  ▼
Service A
  │
  ▼
Message Broker
  │
  ▼
Service B
  │
  ▼
Async Processing
```

The caller and receiver no longer need to be executing at the same time.

> **Asynchronous architecture removes temporal coupling, but introduces new consistency, delivery, ordering, and observability concerns.**

---

## 2. The Problem

Synchronous communication creates temporal coupling.

If:

```text
A → B
```

then A depends on B being:

* reachable
* available
* responsive
* compatible
* sufficiently fast

A request chain can become:

```text
A
 ↓
B
 ↓
C
 ↓
D
```

The overall latency becomes dependent on the entire chain.

If C is slow:

```text
A → B → C
          ↑
        Slow
```

the delay propagates upstream.

If C is unavailable:

```text
A → B → C ✗
```

the failure may propagate through the entire request path.

---

## 3. Intent

Reduce temporal and runtime coupling by allowing components to communicate through asynchronous boundaries.

```text
Synchronous:

A → B → Response


Asynchronous:

A → Message → Broker
                  ↓
                  B
```

The sender does not need the receiver to complete processing before continuing.

---

## 4. Core Principles

### 4.1 Decouple Time

Synchronous:

```text
A and B must be available together.
```

Asynchronous:

```text
A can continue
while B processes later.
```

---

### 4.2 Explicit Delivery Semantics

Define:

```text
At-most-once
At-least-once
Effectively-once
```

Do not assume:

```text
Message sent = Message processed exactly once
```

---

### 4.3 Idempotency Is Mandatory

At-least-once delivery can produce:

```text
Message
   ↓
Processing
   ↓
Retry
   ↓
Same Message
```

Consumers must safely handle duplicates.

---

### 4.4 Accept Explicit Consistency Trade-offs

Asynchronous processing often changes:

```text
Immediate consistency
```

into:

```text
Eventual consistency
```

The architecture must make this behavior explicit.

---

### 4.5 Preserve Business Semantics

Do not make something asynchronous simply because a message broker is available.

Ask:

```text
Does the caller actually need the result now?
```

If yes, synchronous communication may still be appropriate.

---

## 5. Architectural Model

### Before

```text
┌─────────┐
│ Caller  │
└────┬────┘
     │
     ▼
┌─────────┐
│ Service │
│    A    │
└────┬────┘
     │
     ▼
┌─────────┐
│ Service │
│    B    │
└─────────┘
```

### After

```text
┌─────────┐
│ Service │
│    A    │
└────┬────┘
     │
     ▼
┌─────────┐
│ Broker  │
└────┬────┘
     │
     ▼
┌─────────┐
│ Service │
│    B    │
└─────────┘
```

The broker becomes a temporal decoupling boundary.

---

## 6. What Actually Changes?

Moving from synchronous to asynchronous communication changes several architectural properties.

| Concern                 | Synchronous      | Asynchronous               |
| ----------------------- | ---------------- | -------------------------- |
| Temporal coupling       | High             | Lower                      |
| Response                | Immediate        | Deferred                   |
| Availability dependency | Immediate        | Reduced                    |
| Consistency             | Often immediate  | Often eventual             |
| Failure handling        | Request failure  | Retry/reprocess            |
| Backpressure            | Request-level    | Queue-level                |
| Ordering                | Call order       | Must be designed           |
| Duplicate handling      | Usually simpler  | Essential                  |
| Observability           | Request trace    | Distributed message trace  |
| User experience         | Immediate result | Status/poll/callback/event |
| Scaling                 | Request-driven   | Queue/workload-driven      |

---

## 7. Identify the Right Boundary

Not every synchronous call should become asynchronous.

Good candidates often include:

```text
Long-running processing
Background jobs
Notifications
Email
Document processing
Video processing
AI inference pipelines
Reporting
Analytics
Search indexing
Data synchronization
Integration workflows
Batch processing
```

Poor candidates may include:

```text
Immediate validation
Interactive queries
User-facing reads
Operations requiring an immediate response
Simple local method calls
```

The decision should be based on business semantics and operational characteristics.

---

## 8. Command vs Event

One important architectural distinction is:

### Command

```text
ProcessPayment
```

The sender asks a specific consumer to perform an action.

### Event

```text
PaymentCompleted
```

The producer announces something that already happened.

The migration should preserve this semantic distinction.

Do not convert every synchronous API call into an event simply because a broker is being introduced.

---

## 9. Request/Response to Command

Synchronous:

```http
POST /payments
```

The caller waits for the result.

Asynchronous:

```text
ProcessPayment
      ↓
Message Broker
      ↓
Payment Service
```

The initial response may be:

```http
202 Accepted
```

with:

```text
OperationId
```

The caller can later query:

```http
GET /operations/{operationId}
```

or receive a callback/event.

---

## 10. Long-Running Operations

A synchronous operation:

```text
Request
  ↓
Process for 60 seconds
  ↓
Response
```

can become:

```text
Request
  ↓
Create Operation
  ↓
Queue Command
  ↓
202 Accepted
```

Then:

```text
Worker
  ↓
Process
  ↓
Update Status
```

The caller observes:

```text
Pending
Processing
Completed
Failed
```

---

## 11. Backpressure

Synchronous systems often expose overload directly through request latency.

Asynchronous systems can absorb work temporarily:

```text
Producer
   ↓
████████ Queue
   ↓
Consumers
```

The queue becomes a buffer.

However, the queue is not unlimited capacity.

If:

```text
Arrival Rate > Processing Rate
```

then backlog grows.

Therefore monitor:

```text
Queue Depth
Consumer Lag
Processing Rate
Failure Rate
Age of Oldest Message
```

---

## 12. Queue-Based Load Leveling

Suppose:

```text
Incoming = 1,000 requests/sec
Processing = 500 messages/sec
```

The queue absorbs the difference temporarily.

```text
Producer
   ↓
Queue
████████████
   ↓
Consumers
```

But the backlog continues to grow.

Asynchronous architecture therefore requires capacity planning.

---

## 13. Retry Semantics

Synchronous systems often retry the request:

```text
Request
  ↓
Failure
  ↓
Retry
```

Asynchronous systems may retry message processing:

```text
Message
  ↓
Consumer
  ↓
Failure
  ↓
Retry
  ↓
Consumer
```

Define:

```text
Maximum Attempts
Retry Delay
Backoff
Jitter
Retryable Errors
Non-Retryable Errors
Dead-Letter Policy
```

---

## 14. Dead-Letter Queue

Messages that cannot be processed successfully should not retry forever.

```text
Message
  ↓
Consumer
  ↓
Failure
  ↓
Retry
  ↓
Retry
  ↓
Retry Limit
  ↓
Dead Letter
```

The DLQ should support:

* inspection
* alerting
* diagnosis
* replay
* controlled recovery

---

## 15. Idempotency

Consider:

```text
PaymentCompleted
```

If the consumer receives it twice:

```text
PaymentCompleted
PaymentCompleted
```

the business result should not incorrectly execute twice.

Possible strategies:

```text
MessageId
Idempotency Key
Processed Message Store
Unique Constraint
Business Key
```

Example:

```text
ProcessedMessages
-----------------
MessageId
ProcessedAt
```

Before processing:

```text
Message already processed?
```

If yes:

```text
Ignore duplicate
```

---

## 16. Ordering

Synchronous calls naturally establish a call sequence.

Asynchronous messages do not automatically preserve the business order you need.

Example:

```text
OrderCreated
OrderCancelled
```

must not be processed as:

```text
OrderCancelled
OrderCreated
```

Define ordering requirements explicitly.

Possible strategies include:

* partitioning
* partition keys
* sequence numbers
* version numbers
* optimistic concurrency
* per-aggregate ordering

Do not assume global ordering unless the messaging infrastructure actually provides and guarantees it.

---

## 17. Outbox Pattern

A common problem is:

```text
Database Commit
       +
Message Publish
```

What if the database succeeds but publishing fails?

```text
DB ✓
Message ✗
```

The system becomes inconsistent.

Use a transactional outbox:

```text
Application
    │
    ├── Business Data
    │
    └── Outbox Message
             │
       Same Transaction
             │
             ▼
         Outbox
             │
             ▼
         Publisher
             │
             ▼
          Broker
```

This creates a reliable bridge between local state changes and asynchronous communication.

---

## 18. Inbox Pattern

The consumer side can use an inbox or processed-message store:

```text
Broker
  ↓
Consumer
  ↓
Inbox / Deduplication
  ↓
Business Logic
```

This supports idempotent message processing.

Together:

```text
Outbox → Broker → Inbox
```

can provide strong practical delivery guarantees without requiring distributed transactions.

---

## 19. Transactions

A synchronous database transaction may look like:

```text
BEGIN

Update Order
Call Payment
Update Order

COMMIT
```

After asynchronous migration:

```text
Transaction A
    ↓
PaymentRequested
    ↓
Transaction B
    ↓
PaymentCompleted
    ↓
Transaction C
```

The original transaction boundary no longer exists.

The architecture may require:

* Saga
* compensation
* state machines
* eventual consistency

---

## 20. Saga

Example:

```text
OrderCreated
     ↓
ReserveInventory
     ↓
InventoryReserved
     ↓
ProcessPayment
     ↓
PaymentCompleted
     ↓
OrderConfirmed
```

Failure:

```text
PaymentFailed
     ↓
ReleaseInventory
     ↓
OrderCancelled
```

The workflow is distributed across multiple local transactions.

---

## 21. User Experience

Asynchronous architecture changes the user experience.

Instead of:

```text
Click
  ↓
Wait
  ↓
Result
```

the experience may become:

```text
Submit
  ↓
Accepted
  ↓
Processing
  ↓
Completed
```

Possible mechanisms:

* polling
* WebSocket
* Server-Sent Events
* push notification
* webhook
* email
* application notification

The user-facing contract must reflect the asynchronous nature of the operation.

---

## 22. Operation Tracking

Use an explicit operation identifier:

```text
OperationId
```

Example:

```json
{
  "operationId": "8b1c...",
  "status": "processing"
}
```

The operation state may be:

```text
Pending
Processing
Completed
Failed
Cancelled
```

This is particularly useful for long-running workflows.

---

## 23. Observability

Distributed asynchronous systems require stronger tracing.

Track:

```text
TraceId
SpanId
CorrelationId
CausationId
MessageId
OperationId
Producer
Consumer
Topic
Partition
Offset
```

Example:

```text
HTTP Request
     │
     └── TraceId
           │
           ▼
       Message
           │
           └── CausationId
                 │
                 ▼
              Consumer
```

The goal is to reconstruct:

```text
Who produced this?
Why was it produced?
Which operation caused it?
Who processed it?
What happened afterward?
```

---

## 24. Schema Evolution

Once communication becomes asynchronous, messages become durable contracts.

Therefore define:

```text
Schema
Version
Compatibility
Ownership
Retention
Replay Semantics
```

Avoid breaking consumers by changing an event contract without migration.

Use:

```text
Expand
  ↓
Migrate
  ↓
Contract
```

for message evolution.

---

## 25. Security

Asynchronous systems introduce additional security boundaries.

Secure:

* broker authentication
* authorization
* topic permissions
* queue permissions
* encryption
* message integrity
* service identity
* sensitive payloads
* dead-letter queues
* replay access

A message broker should not become a universal read/write channel.

---

## 26. Common Failure Modes

### Asynchronous Everything

Making every operation asynchronous increases complexity without solving a real problem.

---

### Hidden Synchronous Dependency

```text
A
 ↓
Queue
 ↓
B
 ↓
Synchronous C
 ↓
Synchronous D
```

The system may still have strong temporal coupling.

---

### Queue as Database

Treating the broker as the permanent system of record creates unclear ownership.

---

### Unbounded Queue

The queue grows indefinitely because consumers cannot keep up.

---

### Retry Storm

Many consumers retry simultaneously and overload the failing dependency.

---

### Duplicate Processing

Messages are processed more than once and business operations are not idempotent.

---

### Ordering Assumptions

The architecture assumes ordering that the messaging infrastructure does not guarantee.

---

### Lost Messages

Messages are published without reliable delivery or without an outbox.

---

### Poison Messages

One invalid message repeatedly blocks processing.

---

### Event Explosion

Every internal change becomes an event, creating unnecessary coupling and operational complexity.

---

### Distributed Monolith

Services communicate asynchronously but still require every downstream service to succeed before business operations can complete.

---

## 27. Architectural Invariants

The migration should enforce:

1. Every asynchronous message has a clear producer and consumer.
2. Message ownership is explicit.
3. Delivery semantics are documented.
4. Consumers are idempotent.
5. Retry policies are bounded.
6. Poison messages can be isolated.
7. Dead-letter handling exists where required.
8. Ordering requirements are explicit.
9. Message contracts are versioned.
10. Database changes and message publication are reliable.
11. Cross-service consistency is explicitly modeled.
12. Queue capacity is monitored.
13. Backpressure is measurable.
14. Distributed traces can follow message flow.
15. Asynchronous workflows have explicit completion and failure states.

---

## 28. Testing Strategy

Use:

```text
Unit Tests
    ↓
Message Contract Tests
    ↓
Consumer Tests
    ↓
Integration Tests
    ↓
Idempotency Tests
    ↓
Failure Tests
    ↓
Resilience Tests
    ↓
Load Tests
    ↓
Chaos Tests
```

Important scenarios:

### Duplicate Message

Deliver the same message twice.

Expected:

```text
One Business Effect
```

### Out-of-Order Message

Deliver messages in the wrong sequence.

Verify the system rejects, delays, or safely handles them.

### Consumer Failure

Kill the consumer during processing.

Verify:

```text
Retry
Recovery
Idempotency
```

### Broker Failure

Test producer and consumer behavior when the broker is unavailable.

### Poison Message

Verify that repeated failure eventually moves the message to the DLQ.

---

## 29. Performance and Scalability

Measure:

```text
Throughput
Queue Depth
Consumer Lag
Processing Latency
End-to-End Latency
Retry Rate
DLQ Rate
CPU
Memory
Broker Throughput
```

A useful distinction:

```text
Request Latency
```

versus:

```text
End-to-End Processing Latency
```

For asynchronous systems, both matter.

---

## 30. .NET Implementation

Useful technologies:

* ASP.NET Core
* `BackgroundService`
* `IHostedService`
* Kafka
* RabbitMQ
* Azure Service Bus
* MassTransit
* Channels
* System.Threading.Channels
* transactional outbox
* OpenTelemetry
* Polly / resilience pipelines
* EF Core
* Aspire

A simple consumer architecture:

```text
Message Broker
      ↓
Consumer
      ↓
Deserialize
      ↓
Validate
      ↓
Idempotency Check
      ↓
Business Logic
      ↓
Transaction
      ↓
Acknowledgement
```

Keep business processing separate from transport-specific code.

---

## 31. Migration Strategy

A safe migration often follows:

```text
Identify Synchronous Dependency
          ↓
Understand Business Semantics
          ↓
Define Command / Event
          ↓
Introduce Message Contract
          ↓
Build Consumer
          ↓
Introduce Outbox
          ↓
Run Shadow / Parallel Processing
          ↓
Switch Processing
          ↓
Monitor
          ↓
Remove Synchronous Path
```

Do not remove the synchronous path before the asynchronous path is operationally proven.

---

## 32. Parallel Processing

During migration:

```text
Request
   │
   ├──→ Existing Synchronous Path
   │
   └──→ New Async Path
```

The asynchronous path can initially operate in shadow mode.

For operations with side effects:

```text
Do not execute both blindly.
```

Instead compare:

```text
Decision
Result
Latency
Errors
Expected Side Effects
```

before making the asynchronous path authoritative.

---

## 33. Cutover

Possible progression:

```text
100% Sync
    ↓
Async Shadow
    ↓
10% Async
    ↓
50% Async
    ↓
90% Async
    ↓
100% Async
```

At each stage monitor:

```text
Error Rate
Processing Latency
Queue Depth
Consumer Lag
Business Failures
Duplicate Effects
DLQ
```

The exact percentages depend on the system and the risk profile.

---

## 34. Rollback

Before the asynchronous path becomes authoritative:

```text
Sync ✓
Async ✓
```

Rollback can return to:

```text
Sync
```

After asynchronous processing becomes authoritative, rollback may be more complicated because:

```text
Messages may already be processed
State may already have changed
Consumers may have advanced
```

Therefore define:

```text
Cutover Point
Message Drain Strategy
State Reconciliation
Duplicate Handling
Rollback Window
```

---

## 35. Migration State Machine

```text
Identified
    ↓
ContractDefined
    ↓
ConsumerReady
    ↓
OutboxEnabled
    ↓
ShadowProcessing
    ↓
AsyncEnabled
    ↓
Validated
    ↓
SyncDisabled
    ↓
AsyncAuthoritative
    ↓
LegacyRemoved
```

Failure:

```text
       ┌─────────┐
       │ Failed  │
       └────┬────┘
            │
       Recover / Retry
            │
            ▼
      Previous State
```

---

## 36. Production Checklist

### Before Migration

* [ ] Identify synchronous dependency.
* [ ] Determine whether asynchronous processing is actually appropriate.
* [ ] Define command/event semantics.
* [ ] Define message contract.
* [ ] Define delivery semantics.
* [ ] Define idempotency strategy.
* [ ] Define ordering requirements.
* [ ] Define retry policy.
* [ ] Define DLQ strategy.
* [ ] Define observability.
* [ ] Define rollback.

### During Migration

* [ ] Introduce consumer.
* [ ] Introduce outbox where required.
* [ ] Validate message delivery.
* [ ] Run shadow processing where appropriate.
* [ ] Measure queue depth.
* [ ] Measure consumer lag.
* [ ] Validate business outcomes.
* [ ] Test duplicate processing.
* [ ] Test failure recovery.

### Before Cutover

* [ ] Consumer is production-ready.
* [ ] Idempotency is verified.
* [ ] Retry behavior is bounded.
* [ ] DLQ is operational.
* [ ] Tracing is available.
* [ ] Capacity is validated.
* [ ] Rollback is understood.

### After Cutover

* [ ] Monitor end-to-end latency.
* [ ] Monitor queue depth.
* [ ] Monitor DLQ.
* [ ] Monitor retries.
* [ ] Validate business metrics.
* [ ] Remove synchronous dependency.
* [ ] Remove obsolete code and configuration.

---

## 37. Lab Experiment

Build:

```text
Order Service
      │
      │ synchronous
      ▼
Payment Service
```

Initial flow:

```text
POST /orders
     ↓
Create Order
     ↓
Call Payment
     ↓
Payment Response
     ↓
Confirm Order
```

### Phase 1: Identify the Boundary

Determine what the caller actually needs immediately.

### Phase 2: Introduce Command

```text
ProcessPayment
```

### Phase 3: Add Broker

```text
Order Service
      ↓
Message Broker
      ↓
Payment Service
```

### Phase 4: Add Outbox

Persist:

```text
Order
PaymentRequested
```

in the same local transaction.

### Phase 5: Add Idempotent Consumer

Verify duplicate `PaymentRequested` messages produce only one business effect.

### Phase 6: Introduce State Machine

```text
Order
 ├── PendingPayment
 ├── PaymentCompleted
 ├── PaymentFailed
 └── Cancelled
```

### Phase 7: Add Failure Experiments

Test:

```text
Payment Service Down
Broker Down
Duplicate Message
Out-of-Order Message
Consumer Crash
Network Timeout
Poison Message
Growing Queue
```

### Phase 8: Add Observability

Trace:

```text
HTTP Request
    ↓
OrderCreated
    ↓
PaymentRequested
    ↓
PaymentProcessed
    ↓
PaymentCompleted
    ↓
OrderConfirmed
```

### Phase 9: Cut Over

Move traffic gradually from:

```text
Synchronous
```

to:

```text
Asynchronous
```

### Phase 10: Remove Legacy Path

Remove the synchronous dependency only after the asynchronous workflow is proven.

---

## 38. Success Criteria

The migration is complete when:

```text
Synchronous Dependency
        ↓
Explicit Message Contract
        ↓
Reliable Publication
        ↓
Idempotent Consumer
        ↓
Observable Processing
        ↓
Controlled Consistency
        ↓
Async Authoritative Path
        ↓
Synchronous Path Removed
```

The goal is not:

```text
REST → Kafka
```

The goal is:

```text
Temporal Coupling
        ↓
Explicit Asynchronous Workflow
```

---

## 39. Key Takeaways

* Synchronous to asynchronous migration changes the architecture's temporal model.
* It is not simply replacing REST with a message broker.
* The migration changes latency, consistency, failure handling, ordering, retries, and observability.
* Use asynchronous communication where the business process can tolerate deferred completion.
* Commands and events have different semantics.
* At-least-once delivery requires idempotent consumers.
* Outbox helps reliably connect local transactions to message publication.
* Inbox or equivalent deduplication can protect consumers.
* Queues provide buffering and backpressure, not unlimited capacity.
* Ordering must be explicitly designed.
* Long-running operations need explicit state and operation tracking.
* Distributed workflows may require Saga and compensation.
* Message contracts must evolve safely.
* Observability must cross message boundaries.
* Do not make everything asynchronous.

> **The goal is not to remove synchronous communication. The goal is to remove unnecessary temporal coupling while making asynchronous behavior explicit, reliable, observable, and recoverable.**
