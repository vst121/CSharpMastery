# Failure Modes

## 1. Essence

`FailureModes` is the experimental laboratory for understanding how architectures behave when things go wrong.

The goal is not simply to identify failures, but to understand:

- why the failure occurs
- which architectural assumptions caused it
- how the failure propagates
- what the blast radius is
- how the system detects the failure
- how the system recovers
- which architectural controls reduce the probability or impact
- what operational cost those controls introduce

Architecture should be evaluated not only by how it behaves under normal conditions, but by how it behaves under stress, failure, duplication, delay, inconsistency, and partial availability.

The central experimental loop is:

```text
Failure Scenario
      ↓
Architectural Assumption
      ↓
Failure Injection
      ↓
Observed Behavior
      ↓
Blast Radius
      ↓
Detection
      ↓
Recovery
      ↓
Mitigation
      ↓
Architectural Implication
```

The objective is to turn failure handling from an assumption into measurable architectural evidence.

---

# 2. Purpose

This laboratory focuses on failure modes that appear frequently in distributed, event-driven, microservice, and asynchronous architectures.

The initial failure-mode set is:

```text
01-DistributedMonolith
02-SharedDatabase
03-RetryStorm
04-CascadingFailure
05-DuplicateProcessing
06-MessageLoss
07-PoisonMessage
08-SplitBrain
```

These failures are related.

For example:

```text
Shared Database
      ↓
Coupling
      ↓
Distributed Monolith
      ↓
Synchronous Dependencies
      ↓
Cascading Failure
      ↓
Retries
      ↓
Retry Storm
```

Similarly:

```text
Asynchronous Messaging
      ↓
Duplicate Delivery
      ↓
Duplicate Processing
```

and:

```text
Message Failure
      ↓
Poison Message
      ↓
Consumer Retry
      ↓
Retry Storm
      ↓
Consumer Saturation
```

Failure modes should therefore be studied individually and as interacting failure chains.

---

# 3. Failure Experiment Philosophy

A failure experiment should answer:

> What happens when an architectural assumption is violated?

Examples:

```text
Assumption:
"Retries will improve reliability."

Failure:
Dependency remains unavailable.

Question:
Do retries improve recovery or amplify the failure?
```

```text
Assumption:
"Messages will be processed once."

Failure:
The broker delivers the same message twice.

Question:
Is the consumer idempotent?
```

```text
Assumption:
"Service boundaries provide independence."

Failure:
Multiple services depend on the same database.

Question:
Are the services actually independent?
```

The experiment should measure the resulting behavior rather than relying on architectural theory alone.

---

# 4. Common Experimental Model

All failure experiments should use a common domain where possible.

Recommended domain:

```text
TransactionFlow
```

Basic architecture:

```text
Client
  ↓
API
  ↓
Transaction Service
  ↓
Processing
  ↓
Persistence
```

For distributed experiments:

```text
Client
  ↓
API
  ↓
Transaction Service
  ↓
Payment Service
  ↓
Database
```

For asynchronous experiments:

```text
Transaction Service
        ↓
      Broker
        ↓
Transaction Processor
        ↓
     Database
```

The same business behavior should be preserved while the failure condition changes.

---

# 5. Common Failure Experiment Structure

Each experiment should follow:

```text
Baseline
    ↓
Failure Injection
    ↓
Observation
    ↓
Measurement
    ↓
Recovery
    ↓
Mitigation
    ↓
Re-run
    ↓
Compare
```

Recommended structure:

```text
FailureModes/
├── README.md
│
├── DistributedMonolith/
├── SharedDatabase/
├── RetryStorm/
├── CascadingFailure/
├── DuplicateProcessing/
├── MessageLoss/
├── PoisonMessage/
└── SplitBrain/
```

Each experiment may eventually contain:

```text
Experiment/
├── README.md
├── src/
├── tests/
├── benchmarks/
├── failure/
├── results/
└── docker-compose.yml
```

---

# 6. Common Failure Dimensions

Every failure experiment should investigate the following dimensions.

## 6.1 Detection

How quickly can the system detect the failure?

Examples:

- health checks
- metrics
- logs
- traces
- alerts
- broker metrics
- database metrics
- application-level invariants

---

## 6.2 Blast Radius

Which components are affected?

Measure:

```text
Failure Origin
      ↓
Directly Affected Components
      ↓
Indirectly Affected Components
      ↓
User Impact
```

Questions:

- Does one component fail?
- Does the failure propagate?
- Does the entire system become unavailable?
- Are only specific transactions affected?

---

## 6.3 Recovery

Measure:

- time to detection
- time to recovery
- failed requests
- lost messages
- duplicate operations
- inconsistent state
- manual intervention

Important metrics:

```text
MTTD = Mean Time To Detect

MTTR = Mean Time To Recover
```

---

## 6.4 Data Integrity

Determine whether the failure produces:

- data loss
- duplicate data
- inconsistent data
- partial transactions
- stale data
- conflicting writes
- corrupted state

---

## 6.5 Availability

Measure:

- successful requests
- failed requests
- degraded requests
- unavailable operations
- recovery time

---

## 6.6 Performance

Measure:

- latency
- throughput
- CPU
- memory
- database connections
- queue depth
- consumer lag
- network traffic

---

## 6.7 Operational Complexity

Record:

- number of components involved
- manual recovery steps
- configuration required
- monitoring required
- operational dependencies
- rollback complexity

---

# 7. Failure Mode 01 - Distributed Monolith

## 7.1 Essence

A distributed monolith is a system that is physically distributed but architecturally tightly coupled.

The system may contain many services, containers, or deployments, but changing or operating one component still requires coordination with many others.

The problem is not the number of services.

The problem is the lack of meaningful independence.

---

## 7.2 Typical Architecture

```text
Client
  ↓
Order Service
  ↓
Payment Service
  ↓
Inventory Service
  ↓
Customer Service
  ↓
Database
```

If every request requires all services to be available:

```text
A → B → C → D
```

the architecture may have distributed deployment without distributed independence.

---

## 7.3 Failure Scenario

Stop one service:

```text
Payment Service
      X
```

Observe:

```text
Order Service
     ↓
Payment Service X
     ↓
Order Failure
```

Then measure whether unrelated functionality also fails.

---

## 7.4 Symptoms

Typical symptoms:

- long synchronous dependency chains
- shared database
- shared domain models
- synchronized deployments
- frequent cross-service calls
- coordinated releases
- distributed transactions everywhere
- one service outage affecting many services
- difficult local development
- difficult testing

---

## 7.5 Measurements

Measure:

- number of synchronous dependencies
- dependency depth
- deployment coupling
- failure propagation
- request latency
- cross-service traffic
- recovery time
- percentage of requests requiring multiple services

---

## 7.6 Architectural Controls

Possible controls:

- stronger service boundaries
- asynchronous communication
- owned data
- local projections
- bounded contexts
- failure isolation
- timeout policies
- circuit breakers
- bulkheads
- independent deployment

---

## 7.7 Key Question

> Does distribution create independence, or only introduce network calls?

---

# 8. Failure Mode 02 - Shared Database

## 8.1 Essence

A shared database occurs when multiple independently deployed components directly access the same database or database schema.

```text
Orders ─────┐
Payments ───┼──→ Shared Database
Customers ──┘
```

The database becomes a hidden integration mechanism.

---

## 8.2 Failure Scenario

Change the schema used by Payments:

```text
Payments
   ↓
Schema Change
   ↓
Orders
   X
```

Then stop the database:

```text
Shared Database
       X
       ↓
Orders
Payments
Customers
       X
```

---

## 8.3 Failure Characteristics

Shared databases can create:

- deployment coupling
- schema coupling
- transaction coupling
- performance coupling
- security coupling
- availability coupling
- ownership ambiguity

---

## 8.4 Measurements

Measure:

- number of consumers per table
- cross-component queries
- schema dependencies
- database connection usage
- transaction scope
- deployment dependencies
- failure propagation

---

## 8.5 Failure Experiment

1. Run all components.
2. Stop the database.
3. Measure affected functionality.
4. Modify a shared schema.
5. Deploy one component.
6. Test all consumers.
7. Measure compatibility failures.

---

## 8.6 Architectural Controls

Possible controls:

- explicit data ownership
- separate databases where justified
- APIs
- events
- local projections
- CDC
- transactional outbox
- scoped credentials
- architecture tests

---

## 8.7 Key Question

> Who owns the data, and what happens when that owner or its storage becomes unavailable?

---

# 9. Failure Mode 03 - Retry Storm

## 9.1 Essence

A retry storm occurs when many clients or services repeatedly retry failed operations, increasing load on an already failing dependency.

```text
Dependency
    X
   ↑ ↑ ↑
Retry Retry Retry
   ↑ ↑ ↑
Clients
```

Instead of helping recovery, retries can amplify the failure.

---

## 9.2 Typical Failure Chain

```text
Dependency Failure
       ↓
Requests Fail
       ↓
Retries
       ↓
More Requests
       ↓
Higher Load
       ↓
Dependency Becomes Less Healthy
       ↓
More Failures
       ↓
More Retries
```

This creates a positive feedback loop.

---

## 9.3 Failure Scenario

Configure:

```text
100 clients
3 retries
no backoff
```

Then make the downstream service unavailable.

Observe:

- request rate
- retry rate
- downstream load
- connection usage
- CPU
- latency

---

## 9.4 Important Variables

Experiment with:

```text
No Retry
Immediate Retry
Fixed Backoff
Exponential Backoff
Exponential Backoff + Jitter
Circuit Breaker
Retry + Circuit Breaker
```

---

## 9.5 Measurements

Measure:

- original request rate
- retry request rate
- total request rate
- downstream CPU
- latency
- failure rate
- recovery time
- connection count

A useful metric:

```text
Retry Amplification Factor

Total Attempts / Original Requests
```

---

## 9.6 Architectural Controls

Possible controls:

- bounded retries
- exponential backoff
- jitter
- circuit breakers
- request deadlines
- timeout budgets
- rate limiting
- load shedding
- bulkheads
- queue-based buffering

---

## 9.7 Key Question

> Does the retry policy help the dependency recover, or does it make recovery harder?

---

# 10. Failure Mode 04 - Cascading Failure

## 10.1 Essence

A cascading failure occurs when failure in one component propagates through dependencies and causes additional failures.

```text
Service A
   ↓
Service B
   ↓
Service C
   ↓
Service D
```

If C becomes unavailable:

```text
C X
↑
B fails
↑
A fails
```

---

## 10.2 Typical Failure Chain

```text
Dependency Degradation
        ↓
Latency Increase
        ↓
Connection Pool Saturation
        ↓
Request Queue Growth
        ↓
Timeouts
        ↓
Retries
        ↓
Resource Exhaustion
        ↓
More Services Fail
```

---

## 10.3 Failure Scenario

Introduce increasing latency:

```text
0 ms
100 ms
500 ms
1000 ms
3000 ms
```

Observe the entire dependency graph.

---

## 10.4 Measurements

Measure:

- latency propagation
- request queue length
- connection pool usage
- thread/task utilization
- retry rate
- error rate
- affected services
- user impact
- recovery time

---

## 10.5 Architectural Controls

Possible controls:

- timeouts
- circuit breakers
- bulkheads
- load shedding
- rate limiting
- backpressure
- asynchronous boundaries
- graceful degradation
- caching
- dependency isolation

---

## 10.6 Key Question

> How far does one component's failure propagate through the architecture?

---

# 11. Failure Mode 05 - Duplicate Processing

## 11.1 Essence

Duplicate processing occurs when the same command or event is processed more than once.

In distributed systems, at-least-once delivery can intentionally allow duplicates to avoid message loss.

Therefore:

> Duplicate delivery must be treated as a normal possibility.

---

## 11.2 Typical Scenario

```text
Broker
  ↓
Consumer
  ↓
Process Message
  ↓
Database Commit
  ↓
Consumer Crashes
  ↓
Acknowledgement Not Recorded
  ↓
Message Delivered Again
```

The same business operation executes twice.

---

## 11.3 Dangerous Operations

Examples:

- charging a payment
- creating an order
- sending an email
- issuing a refund
- incrementing a balance
- creating a shipment

---

## 11.4 Failure Experiment

Send:

```text
MessageId = 123
```

twice.

Then send it:

```text
10 times
100 times
1000 times
```

Observe the final business state.

---

## 11.5 Measurements

Measure:

- number of deliveries
- number of executions
- duplicate detection rate
- business side effects
- processing latency
- database operations

---

## 11.6 Idempotency Strategies

Possible strategies:

### Idempotency Key

```text
MessageId
```

stored with processing state.

### Processed Message Store

```text
MessageId → Processed
```

### Unique Constraint

```text
UNIQUE(TransactionId)
```

### Business Idempotency

Design the operation itself so repeated execution produces the same final state.

---

## 11.7 Key Question

> What happens when the same business operation executes twice?

---

# 12. Failure Mode 06 - Message Loss

## 12.1 Essence

Message loss occurs when a message is accepted by the producer or broker but never reaches successful processing.

Possible causes include:

- application crash
- broker failure
- incorrect acknowledgement
- consumer crash
- network failure
- non-durable messaging
- incorrect transaction boundaries
- producer failure

---

## 12.2 Typical Failure

```text
Application
    ↓
Publish Message
    X
Application crashes
```

Or:

```text
Database Commit
      ↓
Message Publish
      X
```

The database contains the business state but the corresponding event does not exist.

---

## 12.3 Dual-Write Problem

```text
Database
   ↓
Commit

Message Broker
   ↓
Publish
```

These are two independent operations.

If one succeeds and the other fails, the system becomes inconsistent.

---

## 12.4 Failure Experiment

Inject failures between:

```text
Database Write
      ↓
Message Publish
```

Test:

```text
DB success + message failure
DB failure + message success
Application crash
Broker unavailable
Network failure
Consumer unavailable
```

---

## 12.5 Measurements

Measure:

- messages produced
- messages consumed
- messages acknowledged
- messages missing
- processing lag
- reconciliation differences

---

## 12.6 Architectural Controls

Possible controls:

- transactional outbox
- durable broker
- acknowledgements
- retries
- dead-letter queues
- consumer offsets
- reconciliation
- replay
- CDC

Typical reliable pattern:

```text
Business Data
     +
Outbox Message
     ↓
Single Local Transaction
     ↓
Outbox Publisher
     ↓
Broker
```

---

## 12.7 Key Question

> Can the system prove that every important business event was either processed or is recoverable?

---

# 13. Failure Mode 07 - Poison Message

## 13.1 Essence

A poison message is a message that repeatedly fails processing and cannot currently be successfully consumed.

Examples:

- invalid schema
- corrupted payload
- invalid business state
- unsupported version
- unexpected data
- permanent validation failure
- code defect

---

## 13.2 Typical Failure

```text
Broker
  ↓
Consumer
  ↓
Message X
  ↓
Processing Failure
  ↓
Retry
  ↓
Message X
  ↓
Failure
  ↓
Retry
```

Without isolation, one message can block progress.

---

## 13.3 Failure Scenario

Create a message with invalid data:

```json
{
  "transactionId": "invalid",
  "amount": -100
}
```

Configure retries.

Observe whether:

- the consumer keeps retrying
- valid messages are blocked
- CPU increases
- queue lag grows
- the consumer becomes unavailable

---

## 13.4 Poison Message Lifecycle

A robust lifecycle may be:

```text
Received
   ↓
Processing
   ↓
Failed
   ↓
Retry
   ↓
Retry Limit
   ↓
Dead Letter
   ↓
Investigation
   ↓
Correction / Replay
```

---

## 13.5 Measurements

Measure:

- retry count
- consumer throughput
- queue depth
- consumer lag
- DLQ size
- processing latency
- valid messages delayed

---

## 13.6 Architectural Controls

Possible controls:

- bounded retries
- exponential backoff
- dead-letter queue
- validation
- schema validation
- version compatibility
- quarantine
- replay tooling
- operator visibility

---

## 13.7 Key Question

> Can one permanently failing message prevent healthy messages from progressing?

---

# 14. Failure Mode 08 - Split Brain

## 14.1 Essence

Split brain occurs when two or more components believe they are independently authoritative and make conflicting decisions.

```text
        Network Partition
             X
        ┌────┴────┐
        ↓         ↓
     Node A     Node B
       ↑           ↑
   "I am leader" "I am leader"
```

Both sides may accept writes or perform coordination responsibilities.

---

## 14.2 Typical Causes

Possible causes:

- network partition
- broken leader election
- stale membership information
- incorrect distributed locking
- clock assumptions
- quorum failure
- inconsistent service discovery
- failover bugs

---

## 14.3 Failure Scenario

Start:

```text
Node A = Leader
Node B = Follower
```

Introduce a network partition:

```text
A  X  B
```

Make both nodes believe they are leaders.

Then submit conflicting writes.

---

## 14.4 Dangerous Consequences

Split brain can produce:

- conflicting writes
- duplicate processing
- divergent state
- inconsistent ownership
- duplicated scheduled jobs
- conflicting commands
- corrupted business state

---

## 14.5 Measurements

Measure:

- conflicting writes
- divergent state
- duplicate operations
- leader changes
- election duration
- recovery time
- reconciliation effort

---

## 14.6 Architectural Controls

Possible controls:

- quorum
- leader election
- fencing tokens
- leases
- consensus protocols
- epoch/version numbers
- monotonic leadership terms
- strongly defined ownership
- conflict detection
- reconciliation

A critical concept is **fencing**.

If Node A loses leadership, Node B should not merely become leader.

The system should also prevent stale Node A from continuing to perform authoritative operations.

---

## 14.7 Key Question

> What prevents an old or isolated node from continuing to act as the authoritative owner?

---

# 15. Cross-Failure Relationships

Failure modes rarely occur independently.

Important chains include:

## 15.1 Shared Database → Cascading Failure

```text
Shared Database
      ↓
Single Failure Domain
      ↓
Multiple Components Fail
```

---

## 15.2 Distributed Monolith → Cascading Failure

```text
Tightly Coupled Services
        ↓
Synchronous Dependency Chain
        ↓
One Service Failure
        ↓
Multiple Services Fail
```

---

## 15.3 Cascading Failure → Retry Storm

```text
Dependency Failure
      ↓
Timeouts
      ↓
Retries
      ↓
Higher Load
      ↓
Retry Storm
```

---

## 15.4 Message Failure → Poison Message

```text
Invalid Message
      ↓
Consumer Failure
      ↓
Retry
      ↓
Repeated Failure
      ↓
Poison Message
```

---

## 15.5 At-Least-Once Delivery → Duplicate Processing

```text
Message
  ↓
Processing
  ↓
Consumer Failure
  ↓
Redelivery
  ↓
Duplicate Processing
```

---

## 15.6 Network Partition → Split Brain

```text
Network Partition
      ↓
Membership Information Diverges
      ↓
Multiple Leaders
      ↓
Conflicting Operations
```

---

# 16. Common Failure Controls

Failure controls should be understood as architectural mechanisms rather than isolated framework features.

## 16.1 Timeout

Prevents requests from waiting indefinitely.

```text
Request
   ↓
Timeout
   ↓
Failure
```

---

## 16.2 Retry

Useful for transient failures.

Requirements:

- bounded attempts
- backoff
- jitter
- timeout budget
- idempotency

---

## 16.3 Circuit Breaker

Stops repeatedly calling an unhealthy dependency.

```text
Closed
  ↓
Failures
  ↓
Open
  ↓
Wait
  ↓
Half Open
  ↓
Recovery
```

---

## 16.4 Bulkhead

Isolates resources.

Example:

```text
Orders → Connection Pool A
Payments → Connection Pool B
```

A failing dependency should not consume all shared resources.

---

## 16.5 Rate Limiting

Controls incoming load.

---

## 16.6 Load Shedding

Rejects or deprioritizes work when capacity is exhausted.

---

## 16.7 Backpressure

Allows downstream capacity to influence upstream production.

---

## 16.8 Dead Letter Queue

Separates messages that cannot currently be processed.

---

## 16.9 Idempotency

Allows repeated operations without producing incorrect additional effects.

---

## 16.10 Transactional Outbox

Provides reliable coordination between local database state and outgoing messages.

---

## 16.11 Fencing

Prevents stale leaders or stale owners from continuing authoritative operations.

---

# 17. Common Metrics

Every failure experiment should record a consistent set of metrics.

## Availability

```text
Successful Requests
Failed Requests
Availability
```

## Latency

```text
P50
P95
P99
Maximum
```

## Throughput

```text
Requests/sec
Messages/sec
Transactions/sec
```

## Resource Utilization

```text
CPU
Memory
Network
Database Connections
Thread/Task Usage
```

## Distributed-System Metrics

```text
Queue Depth
Consumer Lag
Retry Count
DLQ Count
Duplicate Count
Message Loss Count
Connection Count
Circuit Breaker State
```

## Recovery

```text
MTTD
MTTR
Recovery Point
Manual Intervention
```

---

# 18. Blast Radius

Every experiment should explicitly document its blast radius.

Use:

```text
Failure Origin
      ↓
Component
      ↓
Dependent Components
      ↓
Business Capability
      ↓
Users
```

Example:

```text
Payment Service
      ↓
Order Processing
      ↓
Checkout
      ↓
Customers
```

Record:

- directly affected components
- indirectly affected components
- unaffected components
- degraded functionality
- completely unavailable functionality

---

# 19. Architecture Fitness Functions

Failure assumptions should become executable where possible.

Examples:

```text
All external calls must have bounded timeouts.
```

```text
Retries must have a maximum attempt count.
```

```text
Retry policies must use backoff.
```

```text
Message consumers must be idempotent.
```

```text
Poison messages must not block healthy messages.
```

```text
Services must not directly access another service's database.
```

```text
Critical events must use reliable publication mechanisms.
```

```text
Distributed ownership must have an explicit authority mechanism.
```

```text
Leader-controlled operations must use fencing or equivalent protection.
```

Architecture tests should fail when these invariants are violated.

---

# 20. Failure Injection

Failure should be deliberately injected rather than waiting for accidental failures.

Possible techniques:

### Application Failure

```text
Throw exception
Kill process
Restart container
```

### Network Failure

```text
Latency
Packet loss
Connection reset
Network partition
```

### Database Failure

```text
Stop database
Slow queries
Connection exhaustion
Lock contention
```

### Broker Failure

```text
Broker unavailable
Consumer unavailable
Delayed delivery
Duplicate delivery
```

### Resource Failure

```text
CPU saturation
Memory pressure
Connection exhaustion
Queue saturation
```

### Timing Failure

```text
Slow dependency
Timeout
Delayed acknowledgement
Delayed message
```

---

# 21. Failure Experiment Method

Each experiment should follow the same process.

## Step 1 - Establish Baseline

Run the system without failure.

Record:

- latency
- throughput
- resource utilization
- error rate
- queue behavior

---

## Step 2 - Define Failure

Example:

```text
Payment Service unavailable for 60 seconds.
```

---

## Step 3 - Define Hypothesis

Example:

```text
The circuit breaker should prevent Payment Service
failure from exhausting Order Service resources.
```

---

## Step 4 - Inject Failure

Introduce the failure in a controlled way.

---

## Step 5 - Observe

Collect:

- logs
- metrics
- traces
- state changes
- queue behavior
- database state

---

## Step 6 - Measure Blast Radius

Identify affected and unaffected components.

---

## Step 7 - Measure Recovery

Record:

```text
Failure Start
Failure Detection
Recovery Start
Recovery Complete
```

---

## Step 8 - Apply Mitigation

Examples:

```text
Timeout
Retry
Backoff
Circuit Breaker
Bulkhead
Outbox
Idempotency
DLQ
Fencing
```

---

## Step 9 - Repeat

Run the same failure again.

Compare:

```text
Before Mitigation
        vs
After Mitigation
```

---

# 22. Failure Experiment Matrix

| Failure Mode         | Primary Risk             | Typical Trigger           | Main Control               |
| -------------------- | ------------------------ | ------------------------- | -------------------------- |
| Distributed Monolith | Hidden coupling          | Service dependency        | Strong boundaries          |
| Shared Database      | Data/deployment coupling | Shared schema             | Explicit ownership         |
| Retry Storm          | Load amplification       | Dependency failure        | Backoff + limits           |
| Cascading Failure    | Failure propagation      | Dependency degradation    | Isolation                  |
| Duplicate Processing | Repeated side effects    | Redelivery                | Idempotency                |
| Message Loss         | Missing business events  | Publish failure           | Outbox / durable messaging |
| Poison Message       | Consumer blockage        | Permanent message failure | DLQ + bounded retry        |
| Split Brain          | Conflicting authority    | Network partition         | Quorum + fencing           |

---

# 23. What Not To Do

Avoid experiments that:

- only test the happy path
- inject unrealistic failures
- measure only latency
- ignore data integrity
- ignore recovery
- ignore operational complexity
- use uncontrolled workloads
- change multiple architectural variables simultaneously
- run only once
- hide unexpected results
- assume resilience mechanisms are automatically beneficial

For example:

Do not conclude:

```text
"Retries improve reliability."
```

Instead measure:

```text
Under a 30-second downstream outage,
with 100 concurrent clients,
bounded exponential backoff reduced retry amplification
from X to Y and reduced recovery time from A to B.
```

---

# 24. Reproducibility

Every experiment should record:

```text
Git Commit
.NET Version
OS
CPU
Memory
Database Version
Broker Version
Container Version
Configuration
Dataset
Payload Size
Concurrency
Request Rate
Failure Duration
Failure Type
Benchmark Parameters
```

Results should be reproducible.

---

# 25. Recommended Experiment Sequence

The experiments should build on each other.

## Phase 1 - Basic Failure

Start with:

```text
DuplicateProcessing
MessageLoss
PoisonMessage
```

These establish asynchronous failure fundamentals.

---

## Phase 2 - Resilience

Continue with:

```text
RetryStorm
CascadingFailure
```

These demonstrate failure amplification and propagation.

---

## Phase 3 - Architectural Coupling

Then investigate:

```text
SharedDatabase
DistributedMonolith
```

These demonstrate how architecture can create hidden failure domains.

---

## Phase 4 - Distributed Coordination

Finally:

```text
SplitBrain
```

This introduces distributed ownership, leadership, partition tolerance, and coordination problems.

---

# 26. Suggested Implementation Stack

For the .NET laboratory:

```text
.NET 10
C# 14
ASP.NET Core
PostgreSQL
Kafka
Docker
OpenTelemetry
xUnit
BenchmarkDotNet
Testcontainers
```

Potential supporting technologies:

```text
MassTransit
RabbitMQ
Aspire
Polly / .NET Resilience
Prometheus
Grafana
```

The goal is not to test a framework.

The goal is to observe architectural behavior.

---

# 27. Recommended Tooling

## Architecture Tests

Use architecture tests to enforce:

- dependency boundaries
- ownership rules
- forbidden references
- communication constraints

---

## Integration Tests

Use integration tests to validate:

- service interaction
- database behavior
- broker behavior
- failure recovery

---

## Contract Tests

Validate:

- API contracts
- event schemas
- compatibility
- version evolution

---

## Load Tests

Validate:

- throughput
- latency
- resource saturation
- queue behavior
- retry amplification

---

## Chaos / Failure Tests

Validate:

- failure propagation
- recovery
- isolation
- resilience controls

---

# 28. Experiment Result Format

Each experiment should finish with a concise result.

```text
## Result

### Failure
<What failed?>

### Trigger
<How was the failure injected?>

### Baseline
<Normal system behavior>

### Observed Behavior
<What actually happened?>

### Blast Radius
<What components were affected?>

### Data Integrity
<Was data lost, duplicated, or inconsistent?>

### Detection
<How was the failure detected?>

### Recovery
<How did the system recover?>

### Mitigation
<Which architectural control was introduced?>

### Before
<Observed result>

### After
<Observed result>

### Unexpected Behavior
<What was surprising?>

### Trade-offs
<What complexity did the mitigation introduce?>

### Architectural Implication
<What does the experiment teach us?>
```

---

# 29. Relationship With Other ArchitectureExperiments

`FailureModes` should not exist independently from the other experiment categories.

The broader experimental workflow is:

```text
Pattern Comparison
        ↓
Failure Modes
        ↓
Performance
        ↓
Trade-offs
        ↓
ADR
```

For example:

```text
Pattern Comparison
Modular Monolith vs Microservices
        ↓
Failure Mode
Cascading Failure
        ↓
Performance
Latency + Throughput
        ↓
Trade-off
Independence vs Operational Complexity
        ↓
ADR
Architectural Decision
```

Another example:

```text
Sync vs Async
        ↓
Duplicate Processing
        ↓
Message Loss
        ↓
Poison Message
        ↓
Operational Trade-offs
        ↓
ADR
```

---

# 30. Architecture as Evidence

The purpose of these experiments is not to memorize failure-mode definitions.

The goal is to develop architectural intuition based on evidence.

Instead of saying:

> "Microservices are resilient."

You should be able to explain:

```text
Which failure?
Which dependency?
Which failure boundary?
What is the blast radius?
How is failure detected?
How does recovery work?
What is the operational cost?
```

Instead of saying:

> "Async messaging is reliable."

You should be able to explain:

```text
What happens with duplicate delivery?
What happens when the broker is unavailable?
What happens when a consumer crashes?
What happens to poison messages?
How are lost messages detected?
How is replay performed?
```

Instead of saying:

> "Retries improve reliability."

You should be able to demonstrate:

```text
When retries help
When retries amplify failure
How backoff changes behavior
How jitter affects synchronization
How circuit breakers limit damage
```

This is the difference between knowing architecture patterns and understanding production architecture.

---

# 31. Definition of Done

A failure experiment is complete when it has:

- [ ] Explicit failure scenario
- [ ] Baseline measurement
- [ ] Failure hypothesis
- [ ] Controlled failure injection
- [ ] Reproducible workload
- [ ] Observability
- [ ] Failure measurements
- [ ] Blast-radius analysis
- [ ] Data-integrity analysis
- [ ] Recovery measurement
- [ ] Mitigation strategy
- [ ] Repeated experiment after mitigation
- [ ] Before/after comparison
- [ ] Unexpected behavior documented
- [ ] Trade-offs documented
- [ ] Architecture implication documented
- [ ] Relevant architecture tests
- [ ] Relevant failure tests
- [ ] Results committed to the repository

---

# 32. Key Takeaways

Failure is not an exceptional condition in distributed architecture.

It is part of the architecture.

The most important questions are:

```text
What can fail?
        ↓
What happens when it fails?
        ↓
How far does the failure propagate?
        ↓
How do we detect it?
        ↓
How do we recover?
        ↓
How do we prevent amplification?
        ↓
What architectural trade-off does the protection introduce?
```

The eight failure modes provide a practical foundation:

```text
Distributed Monolith
        ↓
Shared Database
        ↓
Retry Storm
        ↓
Cascading Failure
        ↓
Duplicate Processing
        ↓
Message Loss
        ↓
Poison Message
        ↓
Split Brain
```

The ultimate goal is not to eliminate every failure.

The goal is to design systems where failures are:

```text
Expected
Detectable
Contained
Recoverable
Observable
Testable
```

Architecture becomes stronger when failure behavior is designed deliberately and verified experimentally.

> **Production architecture is not defined only by how a system works when everything works. It is defined by what happens when something does not.**
