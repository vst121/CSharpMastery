# Distributed Systems

## 1. Essence

A distributed system is a system whose components execute independently and communicate across a network.

The critical architectural consequence is:

> **The network is part of the system.**

Once computation crosses a process, machine, availability-zone, or network boundary, the architecture must account for:

- latency
- partial failure
- message loss
- duplication
- reordering
- network partitions
- time uncertainty
- independent deployment
- distributed state
- coordination
- consistency
- recovery

A useful mental model is:

```text
Local System

A
│
├── Memory
├── CPU
├── Process
└── Database


Distributed System

A
│
├── Network
│
└── B
    │
    ├── Process
    ├── Memory
    └── Database
```

The second architecture introduces failure modes that do not exist inside a single process.

---

# 2. Why Distributed Systems Are Different

In a local process:

```text
Function Call
    ↓
Immediate execution
    ↓
Return value
```

In a distributed system:

```text
Service A
    ↓
Network
    ↓
Service B
    ↓
Database
```

The operation may encounter:

```text
Request lost
Request duplicated
Request delayed
Response lost
Response delayed
Service unavailable
Network partition
Service overloaded
Database unavailable
```

This means:

```text
Remote Call ≠ Local Function Call
```

A remote call has fundamentally different semantics.

For example:

```csharp
var result = paymentService.Process(payment);
```

looks simple.

But architecturally it means:

```text
Serialize
    ↓
Network
    ↓
Authentication
    ↓
Service discovery
    ↓
Service processing
    ↓
Database
    ↓
Network
    ↓
Deserialize
```

Every boundary introduces uncertainty.

---

# 3. The Fundamental Distributed Systems Problems

Most distributed-system architecture can be understood through a small set of problems:

```text
Distributed Systems
│
├── Communication
│
├── Partial Failure
│
├── Time
│
├── Consistency
│
├── Coordination
│
├── Replication
│
├── Partitioning
│
├── Ordering
│
├── Delivery Semantics
│
├── Consensus
│
├── Recovery
│
└── Observability
```

These problems interact.

For example:

```text
Network Failure
      ↓
Uncertain State
      ↓
Retry
      ↓
Duplicate Request
      ↓
Consistency Problem
      ↓
Idempotency Requirement
```

---

# 4. Core Principles

## Principle 1: Assume Partial Failure

A distributed system should assume that some components can fail while others continue operating.

```text
Service A      Service B      Service C
   ✓              ✗              ✓
```

The system must define what happens next.

---

## Principle 2: The Network Is Unreliable

Network communication can experience:

```text
Latency
Packet loss
Connection reset
Timeout
Partition
Reordering
Congestion
```

Do not architect as though the network were a reliable function-call mechanism.

---

## Principle 3: Time Is Uncertain

Different machines have different clocks.

```text
Machine A
10:00:00.100

Machine B
10:00:00.350
```

Even small clock differences can create problems in:

- ordering
- expiration
- distributed locks
- event timestamps
- conflict resolution
- security tokens

Wall-clock time should not automatically be treated as a globally authoritative ordering mechanism.

---

## Principle 4: Failure Detection Is Imperfect

A timeout does not necessarily mean:

> The operation failed.

It means:

> The caller does not know whether the operation completed.

This distinction is fundamental.

Example:

```text
Client
  ↓
Process Payment
  ↓
Server commits payment
  ↓
Response lost
  ↓
Client timeout
```

The client cannot safely assume:

```text
Payment failed.
```

The payment may have succeeded.

This creates the need for:

```text
Idempotency
Query by transaction ID
Correlation
Durable state
```

---

# 5. Partial Failure

Partial failure is one of the defining properties of distributed systems.

A local application often fails approximately as:

```text
Process
  ↓
Failure
  ↓
Everything stops
```

A distributed system can fail as:

```text
A ✓
B ✗
C ✓
D slow
E partitioned
F overloaded
```

The system is neither completely healthy nor completely down.

This creates difficult architectural states.

Example:

```text
Order Service
     ↓
Payment Service
     ↓
Notification Service
```

Payment succeeds.

Notification fails.

What is the system state?

```text
Order = Paid
Notification = Not Sent
```

This is not necessarily an error in the payment system.

It is a distributed consistency problem.

---

# 6. Failure Taxonomy

Useful failure categories include:

```text
Crash Failure
    ↓
Component stops

Omission Failure
    ↓
Message/request is lost

Timing Failure
    ↓
Response arrives too late

Network Partition
    ↓
Components cannot communicate

Byzantine Failure
    ↓
Component behaves arbitrarily or maliciously
```

Most business applications primarily deal with:

```text
Crash
Timeout
Omission
Partition
Overload
Data failure
```

Byzantine fault tolerance is generally relevant only to specific classes of systems.

---

# 7. Timeouts

Every remote operation should have a bounded timeout.

Bad:

```csharp
await client.SendAsync(request);
```

with no meaningful timeout policy.

Better:

```text
Request
   ↓
Timeout
   ↓
Known failure boundary
```

Timeouts protect the system from waiting indefinitely.

But:

> **Timeouts do not guarantee cancellation of the remote operation.**

This is critical.

```text
Client
  ↓
Request
  ↓
Timeout
  ↓
Client stops waiting

Server
  ↓
May still process request
```

Therefore mutations often require:

```text
Timeout
+
Idempotency
+
Durable operation identity
```

---

# 8. Retries

Retries can improve resilience.

But retries can also amplify failures.

```text
Dependency becomes slow
        ↓
Requests timeout
        ↓
Clients retry
        ↓
Traffic increases
        ↓
Dependency becomes slower
        ↓
More retries
        ↓
Retry storm
```

Retries should therefore use:

```text
Bounded attempts
Exponential backoff
Jitter
Timeout
Retry classification
Circuit breaker where appropriate
```

Never blindly retry every operation.

---

# 9. Retry Classification

Retryability depends on the failure.

Potentially retryable:

```text
Transient network failure
Temporary overload
Temporary dependency unavailability
```

Usually not retryable:

```text
Validation error
Authentication failure
Authorization failure
Malformed request
Business rule violation
```

A useful model:

```text
Failure
   ↓
Is it transient?
   ├── No → Fail
   └── Yes
         ↓
      Is operation safe to retry?
         ├── No → Recover / query state
         └── Yes → Bounded retry
```

---

# 10. Idempotency

An operation is idempotent when repeating it produces the same intended business effect.

Mathematically:

```text
f(f(x)) = f(x)
```

For distributed systems, idempotency is particularly important because:

```text
At-least-once delivery
+
Timeout ambiguity
+
Retries
=
Potential duplicates
```

For TransactionFlow:

```text
Client
  ↓
POST /transactions
Idempotency-Key: ABC123
```

The server records:

```text
ABC123 → Transaction Result
```

A repeated request with the same key can return the existing result.

But:

```text
Same key
+
Different request
```

must be rejected.

---

# 11. Delivery Semantics

Distributed messaging systems commonly expose different delivery semantics.

## At-Most-Once

```text
Message
   ↓
Deliver once
```

Possible outcome:

```text
0 or 1 delivery
```

Advantages:

```text
No duplicates
Lower coordination
```

Risk:

```text
Message loss
```

---

## At-Least-Once

```text
Message
   ↓
Retry until acknowledged
```

Possible outcome:

```text
1 or more deliveries
```

Advantages:

```text
Lower message-loss risk
```

Risk:

```text
Duplicates
```

Requires:

```text
Idempotent consumers
```

---

## Exactly-Once

Exactly-once semantics are often misunderstood.

There are multiple meanings:

```text
Exactly-once delivery
Exactly-once processing
Exactly-once business effect
Exactly-once end-to-end outcome
```

These are not equivalent.

A broker may provide strong guarantees within its own processing model while the overall business operation still encounters:

```text
Database
External API
Network
Retries
Side effects
```

Therefore:

> **Exactly-once business effect is usually an end-to-end architectural property, not simply a broker feature.**

For many systems:

```text
At-least-once delivery
+
Idempotent processing
+
Transactional state management
```

is a practical architecture.

---

# 12. Message Ordering

Distributed systems often need ordering.

But ordering is usually scoped.

For example:

```text
Kafka Partition A

Event 1
Event 2
Event 3
Event 4
```

Ordering may be guaranteed within the partition.

Across partitions:

```text
Partition A     Partition B

Event 1         Event 2
Event 3         Event 4
```

There may be no global ordering.

Architects should therefore ask:

```text
What must be ordered?

Per entity?
Per partition?
Per aggregate?
Globally?
```

Global ordering is usually expensive and limits scalability.

A better model is often:

```text
Order events by business key
```

For example:

```text
TransactionId
CustomerId
AccountId
```

---

# 13. Consistency

Consistency describes how system state is observed and coordinated.

Common models include:

```text
Strong Consistency
Eventual Consistency
Causal Consistency
Read-Your-Writes
Monotonic Reads
```

Do not assume every part of a system requires the same consistency model.

TransactionFlow may use:

```text
Strong consistency
    ↓
Payment state
Transaction identity
Idempotency state
Authorization result
```

while using:

```text
Eventual consistency
    ↓
Analytics
Notifications
Reporting
Dashboards
Customer activity views
```

Architecture should therefore define consistency **per business invariant**, not globally.

---

# 14. Strong Consistency

A strongly consistent operation provides a well-defined view of committed state according to the chosen consistency model.

Useful when:

```text
Money
Inventory
Identity
Authorization
Critical business invariants
```

are involved.

Costs may include:

```text
Coordination
Latency
Reduced availability under partitions
Operational complexity
```

The correct question is not:

> Is strong consistency good?

It is:

> Which business invariants actually require it?

---

# 15. Eventual Consistency

With eventual consistency:

```text
Write
  ↓
State changes
  ↓
Propagation
  ↓
Other components converge
```

Example:

```text
Payment Service
      ↓
PaymentCompleted
      ↓
Analytics
      ↓
Reporting
```

Analytics may temporarily lag behind payment state.

This is acceptable when:

```text
Freshness requirements permit it.
```

Therefore eventual consistency requires explicit definitions such as:

```text
Expected propagation delay
Maximum acceptable staleness
Reconciliation strategy
Failure behavior
```

"Eventually" should not mean "unknown."

---

# 16. CAP Theorem

CAP describes a distributed data system under network partition.

The three properties are:

```text
Consistency
Availability
Partition Tolerance
```

Under a network partition:

```text
You cannot guarantee both
strong consistency and availability
for all operations.
```

Partition tolerance is not simply an optional feature in a networked distributed system.

Networks can partition.

Therefore the practical question becomes:

```text
During partition:
Do we preserve consistency?
or
Do we continue accepting operations?
```

Different systems make different choices.

CAP should not be interpreted as:

```text
Choose any two of C, A, P.
```

That oversimplifies the theorem.

---

# 17. PACELC

PACELC extends the reasoning beyond partitions.

```text
If Partition:
    choose Availability or Consistency

Else:
    choose Latency or Consistency
```

This highlights an important reality:

Even when the network is healthy, stronger coordination can increase latency.

Example:

```text
Local read
    ↓
Low latency

Remote quorum read
    ↓
Network coordination
    ↓
Higher latency
```

PACELC is useful for reasoning about distributed data architectures.

---

# 18. Replication

Replication maintains multiple copies of data.

Common approaches:

```text
Primary / Replica
Leader / Followers
Multi-primary
Quorum-based
Synchronous replication
Asynchronous replication
```

Benefits:

```text
Availability
Read scaling
Disaster recovery
Geographic distribution
```

Costs:

```text
Consistency complexity
Replication lag
Conflict resolution
Storage
Network traffic
Operational complexity
```

---

# 19. Synchronous vs Asynchronous Replication

Synchronous:

```text
Write
 ↓
Primary
 ↓
Replica confirms
 ↓
Commit
```

Potential benefit:

```text
Stronger durability/consistency
```

Potential cost:

```text
Higher latency
Reduced availability if replicas are unavailable
```

Asynchronous:

```text
Write
 ↓
Primary commits
 ↓
Replica later receives update
```

Potential benefit:

```text
Lower write latency
Better availability
```

Potential cost:

```text
Replication lag
Possible data loss during certain failures
```

The choice should be driven by:

```text
RPO
RTO
Consistency requirements
Latency requirements
Failure model
```

---

# 20. Quorum

Quorum-based systems use a subset of replicas to establish agreement.

For example:

```text
N = 3 replicas
W = 2 writes required
R = 2 reads required
```

If:

```text
W + R > N
```

read and write quorums overlap.

Quorum systems introduce trade-offs involving:

```text
Availability
Latency
Consistency
Failure tolerance
```

The details depend on the actual storage system and its consistency model.

---

# 21. Partitioning

Partitioning distributes data or workload across nodes.

Example:

```text
Transactions

Partition 1 → Customer A-D
Partition 2 → Customer E-H
Partition 3 → Customer I-M
...
```

Common partition keys:

```text
CustomerId
TenantId
TransactionId
AccountId
```

A good partition key should provide:

```text
Even distribution
Stable routing
Required locality
Low hotspot risk
Operational manageability
```

---

# 22. Hot Partitions

Poor partitioning can create hotspots.

Example:

```text
Partition 1 → 80% traffic
Partition 2 → 10%
Partition 3 → 10%
```

The system has technically scaled to three partitions, but practical capacity is limited by the hot partition.

This is why scalability experiments must measure:

```text
Per-partition throughput
Per-partition latency
Partition skew
Consumer lag
CPU distribution
```

---

# 23. Distributed Transactions

A distributed transaction spans multiple independently managed resources.

Example:

```text
Order DB
     +
Payment DB
     +
Inventory DB
```

One possible approach is two-phase commit:

```text
Prepare
   ↓
Commit
```

This provides coordination but introduces:

```text
Blocking
Coordination overhead
Failure complexity
Operational complexity
```

Modern architectures often prefer local transactions plus asynchronous coordination.

For example:

```text
Payment Transaction
      ↓
Local DB Transaction
      ↓
Outbox
      ↓
Event
      ↓
Inventory
```

This shifts the problem from distributed atomicity to:

```text
Workflow
Consistency
Idempotency
Compensation
Recovery
```

---

# 24. Saga

A Saga coordinates a business transaction through multiple local transactions.

Example:

```text
Create Order
     ↓
Reserve Inventory
     ↓
Authorize Payment
     ↓
Confirm Order
```

If payment fails:

```text
Cancel Order
     ↓
Release Inventory
```

These are compensating actions.

A Saga may be:

```text
Orchestration
```

where a coordinator controls the workflow.

Or:

```text
Choreography
```

where services react to events.

The important architectural properties are:

```text
Explicit state
Idempotent actions
Compensation
Timeouts
Recovery
Observability
```

---

# 25. Consensus

Consensus is the problem of multiple distributed nodes agreeing on a value or decision despite failures.

Examples of systems using consensus algorithms include implementations based on:

```text
Raft
Paxos
```

Consensus is used for problems such as:

```text
Leader election
Cluster membership
Configuration
Coordination
Distributed metadata
```

Consensus is expensive because nodes must communicate and reach agreement.

A key architectural principle is:

> **Do not introduce distributed consensus when local ownership or simpler coordination solves the problem.**

---

# 26. Leader Election

A distributed system may require one node to act as leader.

```text
Node A
Node B
Node C

      ↓

Leader
  A
```

If A fails:

```text
B or C
    ↓
New Leader
```

Problems include:

```text
Split brain
Stale leader
Network partition
Delayed failure detection
Concurrent leaders
```

Therefore leader election often requires:

```text
Epochs
Terms
Fencing
Quorum
Leases
```

depending on the system.

---

# 27. Split Brain

Split brain occurs when multiple components independently believe they are authoritative.

Example:

```text
Partition

Cluster A       Cluster B
   ↓               ↓
Leader A          Leader B
```

Both may accept writes.

Potential consequences:

```text
Conflicting state
Duplicate processing
Data corruption
Divergent histories
```

Controls can include:

```text
Quorum
Leader fencing
Epoch numbers
Leases
Consensus
Single-writer ownership
```

---

# 28. Distributed Locks

Distributed locks are often used to coordinate access to shared resources.

But distributed locks are difficult.

Potential problems:

```text
Lock holder crashes
Network partition
Lease expiration
Clock uncertainty
Stale lock holder
Split brain
```

A lock should not automatically be treated as proof of authority.

For critical resources, consider:

```text
Fencing tokens
Version numbers
Database constraints
Optimistic concurrency
Single ownership
```

Often the best distributed lock is avoiding the need for one.

---

# 29. Ordering and Causality

Global time is unreliable as an ordering mechanism.

Consider:

```text
Service A:
Event A at 10:00:01

Service B:
Event B at 10:00:00.900
```

Clock differences may make timestamps misleading.

Distributed systems can instead reason about:

```text
Causality
Sequence numbers
Offsets
Logical clocks
Version numbers
```

For many business workflows, explicit versions are safer:

```text
Account version 41
Account version 42
Account version 43
```

---

# 30. Concurrency Control

Distributed concurrency can be managed using:

```text
Optimistic concurrency
Pessimistic locking
Version numbers
Compare-and-swap
Single-writer ownership
Partitioning
Queues
```

Optimistic concurrency example:

```text
Read:
Version = 10

Update:
WHERE Id = X
AND Version = 10

Set:
Version = 11
```

If another writer already changed the record:

```text
0 rows updated
```

The conflict becomes explicit.

---

# 31. Backpressure

Backpressure prevents fast producers from overwhelming slower consumers.

```text
Producer
   ↓
Queue
   ↓
Consumer
```

If:

```text
Producer rate > Consumer rate
```

then:

```text
Queue grows
```

Without control:

```text
Memory grows
Storage grows
Latency grows
System eventually fails
```

Backpressure mechanisms include:

```text
Bounded queues
Rate limiting
Flow control
Consumer scaling
Load shedding
Admission control
```

---

# 32. Load Shedding

When a system is overloaded, attempting to process everything can cause total failure.

Instead:

```text
Normal load
    ↓
Process all

Overload
    ↓
Reject lower-priority work
    ↓
Protect critical work
```

For example:

```text
Priority 1:
Payment authorization

Priority 2:
Customer notification

Priority 3:
Analytics enrichment
```

During overload:

```text
Protect Priority 1
Degrade Priority 3
```

This is graceful degradation.

---

# 33. Bulkheads

Bulkheads isolate resource pools.

Without isolation:

```text
One dependency
     ↓
Consumes all threads
     ↓
All requests affected
```

With isolation:

```text
Critical traffic → Pool A
Normal traffic   → Pool B
External API     → Pool C
```

A failure in one dependency cannot consume the entire system's resources.

This reduces blast radius.

---

# 34. Distributed Caching

Caching introduces another distributed state.

Potential benefits:

```text
Lower latency
Reduced database load
Higher throughput
```

Potential problems:

```text
Stale data
Cache invalidation
Stampede
Cold start
Eviction
Inconsistent replicas
```

Important questions:

```text
What can be stale?

For how long?

Who owns the source of truth?

What happens when the cache is unavailable?
```

A cache should usually not become an accidental system of record.

---

# 35. Cache Stampede

A common distributed failure mode:

```text
Cache entry expires
       ↓
10,000 requests miss
       ↓
10,000 database requests
       ↓
Database overload
```

Mitigations include:

```text
Jittered expiration
Request coalescing
Refresh-ahead
Stale-while-revalidate
Rate limiting
Distributed coordination where justified
```

---

# 36. Distributed Observability

A distributed system cannot be operated effectively using isolated logs.

You need correlation.

```text
Request
  ↓
TraceId
  ├── Service A
  ├── Service B
  ├── Database
  ├── Kafka producer
  └── Kafka consumer
```

Useful identifiers:

```text
TraceId
SpanId
CorrelationId
CausationId
RequestId
MessageId
```

For asynchronous workflows, preserve causality.

Example:

```text
Command
   ↓
MessageId
   ↓
Event
   ↓
CausationId
   ↓
Consumer action
```

---

# 37. Distributed Debugging

When something fails, operators should be able to answer:

```text
What request started this?

Which services processed it?

Which database operations occurred?

Which messages were published?

Which consumers processed them?

Where did latency increase?

Where did failure begin?

What was retried?

What was duplicated?
```

Without distributed tracing and correlation, these questions become extremely expensive to answer.

Therefore:

> **Observability is part of the distributed architecture, not merely an operational add-on.**

---

# 38. Distributed System Invariants

Important invariants include:

```text
Remote operations have bounded timeouts.

Retries are bounded.

Retryable operations are explicitly classified.

Mutating operations have idempotency strategy.

Messages have stable identities.

Consumers tolerate duplicates.

Critical state has explicit ownership.

Cross-service data access occurs through defined contracts.

Distributed workflows have explicit state.

Failures have recovery behavior.

Observability preserves correlation and causality.

No unbounded queue is allowed without capacity controls.

Critical operations have defined consistency requirements.
```

These should become architecture tests and operational policies where possible.

---

# 39. Distributed System Failure Modes

Common failure modes:

```text
Distributed Monolith
Shared Database
Retry Storm
Cascading Failure
Duplicate Processing
Message Loss
Poison Message
Split Brain
Network Partition
Thundering Herd
Cache Stampede
Hot Partition
Consumer Lag
Deadlock
Stale Read
Replication Lag
Inconsistent State
Lost Update
```

These are not theoretical edge cases.

They should become experiments in:

```text
ArchitectureExperiments/FailureModes/
```

---

# 40. Distributed System Smells

## Distributed Monolith

```text
Many services
+
Strong synchronous dependencies
+
Shared database
+
Coordinated deployments
```

Result:

```text
Distributed runtime
Monolithic coupling
```

---

## Chatty Services

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

One business operation requires many network calls.

This increases:

```text
Latency
Failure probability
Operational complexity
```

---

## Shared Database

```text
Service A ─┐
Service B ─┼── PostgreSQL
Service C ─┘
```

The services appear independent but remain tightly coupled through data.

---

## Unbounded Retries

```text
Failure
 ↓
Retry
 ↓
Failure
 ↓
Retry
 ↓
...
```

This can create retry storms.

---

## Global Coordination

If every operation requires:

```text
Distributed lock
+
Consensus
+
Global transaction
```

the architecture may have excessive coordination.

Prefer local ownership where possible.

---

# 41. The Distributed State Principle

A useful architectural principle is:

> **Distributed state creates distributed coordination.**

If multiple services can independently modify the same business state:

```text
Service A ─┐
           ├── Shared State
Service B ─┘
```

coordination becomes necessary.

A stronger architecture often establishes:

```text
One Owner
    ↓
Authoritative State
    ↓
Events / APIs
    ↓
Derived Views
```

This reduces conflicting writers.

---

# 42. The Ownership Principle

Every important piece of distributed state should have an explicit owner.

For example:

```text
Payment Service
    ↓
Owns Payment State

Inventory Service
    ↓
Owns Inventory State

Notification Service
    ↓
Owns Notification State
```

Other services consume information through:

```text
API
Events
Read Models
```

rather than directly modifying the owner's storage.

Ownership reduces ambiguity.

---

# 43. Distributed System Design Questions

When introducing a distributed boundary, ask:

```text
1. Why must this component be distributed?

2. What capability owns the state?

3. What happens if the network fails?

4. What happens if the remote service is slow?

5. What happens if the response is lost?

6. Can the operation be retried?

7. What happens if the operation is duplicated?

8. What consistency is required?

9. What ordering is required?

10. What happens if messages arrive late?

11. What happens if messages arrive twice?

12. What happens if messages arrive out of order?

13. Who detects failure?

14. How is recovery performed?

15. How is the workflow observed?

16. What is the blast radius?

17. Can the boundary be removed later?
```

These questions should become part of architectural review.

---

# 44. Distributed Systems and Quality Attributes

Distributed architecture affects many quality attributes simultaneously.

| Quality Attribute | Distributed-System Concern              |
| ----------------- | --------------------------------------- |
| Latency           | Network hops                            |
| Throughput        | Parallelism and partitioning            |
| Availability      | Redundancy and failure handling         |
| Reliability       | Duplicate/lost operations               |
| Consistency       | Replication and coordination            |
| Resilience        | Failure isolation                       |
| Scalability       | Partitioning and horizontal scaling     |
| Maintainability   | Service boundaries                      |
| Operability       | Distributed observability               |
| Security          | Service identity and network boundaries |
| Cost              | Infrastructure and network traffic      |

There is no free distribution.

Distribution creates capabilities and costs simultaneously.

---

# 45. Distributed Systems and CAP

CAP should be used as a reasoning tool, not as a slogan.

Ask:

```text
What happens during a network partition?
```

Then ask:

```text
Can we continue accepting writes?

If yes:
What inconsistency can occur?

If no:
What availability impact occurs?

How is the system reconciled?

Which business invariants must remain protected?
```

This leads to an architecture decision grounded in business requirements.

---

# 46. Distributed Systems and PACELC

When the system is healthy:

```text
Do we optimize for:

Lower latency
or
Stronger consistency?
```

When partitioned:

```text
Do we optimize for:

Availability
or
Consistency?
```

This helps explain why architecture decisions cannot be made only by looking at failure scenarios.

Normal operating behavior matters too.

---

# 47. Distributed Systems and Evolution

A system does not need to become distributed immediately.

A safer evolution path is often:

```text
Monolith
   ↓
Modular Monolith
   ↓
Explicit Ownership
   ↓
Explicit Contracts
   ↓
Asynchronous Boundaries
   ↓
Selective Distribution
   ↓
Independent Deployment
```

This allows architectural boundaries to mature before introducing network boundaries.

The principle is:

> **Introduce distribution when its benefits justify its coordination and operational costs.**

---

# 48. Distributed Systems in Modern .NET

Modern .NET provides strong building blocks.

## Communication

```text
ASP.NET Core
REST
gRPC
HttpClient
IHttpClientFactory
```

## Resilience

```text
Microsoft.Extensions.Resilience
Timeout
Retry
Circuit Breaker
Rate Limiting
```

## Messaging

```text
Kafka
RabbitMQ
Azure Service Bus
System.Threading.Channels
```

## Observability

```text
OpenTelemetry
ILogger
Metrics
Distributed tracing
Health checks
```

## Data

```text
PostgreSQL
SQL Server
EF Core
Npgsql
Dapper
```

## Testing

```text
xUnit
Testcontainers
WireMock
BenchmarkDotNet
Architecture tests
Load tests
Chaos/failure tests
```

## Cloud / Runtime

```text
Docker
Kubernetes
.NET Aspire
Azure
```

Technology provides mechanisms.

Architecture determines:

```text
Ownership
Boundaries
Consistency
Failure behavior
Recovery
```

---

# 49. Example: TransactionFlow

Consider:

```text
Client
   ↓
Transaction API
   ↓
Transaction Service
   ↓
PostgreSQL
   ↓
Outbox
   ↓
Kafka
   ↓
Consumers
```

Potential distributed concerns:

```text
Client timeout
    ↓
Idempotency

Database commit
    ↓
Outbox

Kafka unavailable
    ↓
Event remains durable

Consumer failure
    ↓
Retry / DLQ

Duplicate event
    ↓
Idempotent consumer

Consumer lag
    ↓
Backpressure / scaling

Kafka partition failure
    ↓
Broker recovery / replication

Distributed tracing
    ↓
TraceId + MessageId + CausationId
```

This is a useful example because the system can remain strongly consistent around the core transaction while allowing derived workflows to be eventually consistent.

---

# 50. Distributed System Architecture Test Examples

## Dependency Boundary

```text
Service A
    ↓
Service B API
```

Test:

```text
Service A must not reference
Service B's persistence project.
```

---

## Idempotency

```text
Same command
    ↓
Same business effect
```

Test:

```text
Send same transaction twice.

Expected:
Exactly one business effect.
```

---

## Timeout

```text
Dependency delay
    ↓
Caller timeout
```

Test:

```text
Dependency takes 10 seconds.

Expected:
Caller terminates according to timeout policy.
```

---

## Failure Isolation

```text
Consumer unavailable
```

Expected:

```text
Transaction processing remains operational.
```

---

## Retry Bound

```text
Dependency unavailable
```

Expected:

```text
Retry count ≤ configured maximum.
```

---

# 51. Recommended Distributed-System Experiments

Create or extend:

```text
ArchitectureExperiments/
```

with experiments such as:

```text
01-NetworkLatency
02-TimeoutBehavior
03-RetryStorm
04-CircuitBreaker
05-Idempotency
06-DuplicateProcessing
07-MessageOrdering
08-AtLeastOnceDelivery
09-ConsumerLag
10-Backpressure
11-HotPartition
12-ReplicationLag
13-ConsistencyModels
14-SagaFailure
15-SplitBrain
16-LeaderElection
17-DistributedLock
18-CascadingFailure
19-CacheStampede
20-FailureIsolation
```

The experimental loop should be:

```text
Question
   ↓
Hypothesis
   ↓
Architecture
   ↓
Failure Injection
   ↓
Measurement
   ↓
Observed Behavior
   ↓
Trade-off
   ↓
Architectural Implication
```

---

# 52. Recommended Metrics

Distributed-system experiments should measure:

```text
Latency
P50
P95
P99
P99.9

Throughput
Requests/sec
Transactions/sec
Messages/sec

Reliability
Error rate
Duplicate count
Lost messages
Failed operations

Messaging
Queue depth
Consumer lag
Retry count
DLQ count

Resources
CPU
Memory
Network
Database connections

Recovery
MTTD
MTTR
RTO
RPO

Consistency
Replication lag
Staleness
Conflict count
Reconciliation count

Scalability
Capacity increase
Scaling efficiency
Partition utilization
```

Never evaluate distributed architecture using only throughput.

---

# 53. Distributed Systems Architecture Checklist

Before introducing a distributed boundary:

```text
Boundary
[ ] Business capability is clearly defined
[ ] Ownership is explicit
[ ] Contract is explicit
[ ] Distribution has a concrete reason

Communication
[ ] Protocol selected
[ ] Timeout defined
[ ] Failure behavior defined
[ ] Retry policy defined
[ ] Backpressure considered

State
[ ] Source of truth defined
[ ] Data ownership defined
[ ] Consistency requirement defined
[ ] Replication strategy defined

Messaging
[ ] Delivery semantics defined
[ ] Message identity defined
[ ] Ordering requirement defined
[ ] Duplicate handling defined
[ ] Poison message strategy defined
[ ] DLQ strategy defined

Failure
[ ] Partial failure considered
[ ] Dependency failure tested
[ ] Cascading failure considered
[ ] Failure isolation defined
[ ] Recovery defined

Observability
[ ] TraceId propagated
[ ] Correlation defined
[ ] Metrics defined
[ ] Logs structured
[ ] Distributed traces available
[ ] Alerts defined

Security
[ ] Service identity defined
[ ] Authorization defined
[ ] Network security defined
[ ] Secrets managed
[ ] Auditability defined

Operations
[ ] Deployment strategy defined
[ ] Scaling strategy defined
[ ] Health checks defined
[ ] Capacity model defined
[ ] Runbooks defined

Evolution
[ ] Contract versioning defined
[ ] Migration strategy defined
[ ] Rollback considered
[ ] Ownership can evolve
```

---

# 54. Senior Architect Interview Questions

## Fundamentals

1. What makes a system distributed?
2. Why is a remote call fundamentally different from a local function call?
3. What is partial failure?
4. Why are timeouts necessary?
5. Why does a timeout not prove that an operation failed?

## Reliability

6. Why are retries dangerous?
7. How do you prevent retry storms?
8. What makes an operation idempotent?
9. How would you design an idempotent payment API?
10. What is the difference between at-most-once and at-least-once delivery?

## Consistency

11. What is eventual consistency?
12. When would you use strong consistency?
13. Explain CAP without using the "pick two" oversimplification.
14. What does PACELC add?
15. How do you define consistency requirements for a business domain?

## Messaging

16. How do you handle duplicate messages?
17. How do you guarantee ordering?
18. What happens when a consumer is slower than the producer?
19. How do you handle poison messages?
20. What is backpressure?

## Coordination

21. What is consensus?
22. When would you need leader election?
23. Why are distributed locks dangerous?
24. What is split brain?
25. How does fencing help?

## Architecture

26. When should a system become distributed?
27. How do you avoid creating a distributed monolith?
28. How do you design failure isolation?
29. How do you debug a distributed transaction?
30. How do you measure whether distribution actually improved the architecture?

A strong Senior Architect answer should usually follow:

```text
Requirement
    ↓
Failure Model
    ↓
Consistency Requirement
    ↓
Ownership
    ↓
Communication
    ↓
Failure Handling
    ↓
Observability
    ↓
Trade-offs
    ↓
Evidence
```

---

# 55. Definition of Done

You should be able to:

```text
[ ] Explain partial failure.

[ ] Explain why remote calls are fundamentally different from local calls.

[ ] Design timeout and retry policies.

[ ] Explain retry storms and cascading failures.

[ ] Design idempotent distributed operations.

[ ] Explain at-most-once and at-least-once delivery.

[ ] Explain the practical limitations of "exactly once."

[ ] Explain ordering and causality.

[ ] Explain strong and eventual consistency.

[ ] Explain CAP and PACELC correctly.

[ ] Explain replication and partitioning.

[ ] Explain quorum.

[ ] Explain distributed transactions.

[ ] Explain Saga patterns.

[ ] Explain consensus and leader election.

[ ] Explain split brain and fencing.

[ ] Explain distributed locks and their risks.

[ ] Design backpressure and load shedding.

[ ] Design failure isolation.

[ ] Design distributed observability.

[ ] Identify distributed-system smells.

[ ] Design distributed-system failure experiments.

[ ] Measure distributed-system behavior.

[ ] Connect distributed-system decisions to ADRs.

[ ] Explain how modern .NET implements distributed systems.
```

---

# 56. Key Takeaways

The most important principles are:

> **The network is part of the architecture.**

> **Partial failure is normal in distributed systems.**

> **A timeout means uncertainty, not necessarily failure.**

> **Retries require bounded policies and idempotency.**

> **At-least-once delivery requires duplicate-safe processing.**

> **Exactly-once business behavior is an end-to-end property, not simply a messaging feature.**

> **Distributed state creates distributed coordination.**

> **Explicit ownership reduces distributed complexity.**

> **Consistency should be defined around business invariants.**

> **Global coordination should be minimized.**

> **Failure isolation is an architectural property.**

> **Observability is part of the distributed system itself.**

> **Distribution should be introduced because it solves a concrete architectural problem, not because distribution is inherently better.**

The core reasoning loop is:

```text
Business Requirement
        ↓
Quality Attribute
        ↓
Distributed Boundary
        ↓
Ownership
        ↓
Communication
        ↓
Consistency
        ↓
Failure Model
        ↓
Recovery
        ↓
Observability
        ↓
Measurement
        ↓
Evidence
        ↓
ADR
```
