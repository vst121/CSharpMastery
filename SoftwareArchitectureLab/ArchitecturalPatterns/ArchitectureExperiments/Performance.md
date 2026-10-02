# Performance

## 1. Essence

`Performance` is the experimental laboratory for understanding how architectural decisions affect system performance under controlled workloads.

The goal is not simply to make software "faster".

The goal is to understand:

- where time is spent
- where capacity is consumed
- where bottlenecks emerge
- how performance changes under load
- how architecture affects scalability
- how databases behave under contention
- how messaging affects throughput and latency
- how serialization affects CPU, memory, and network usage
- which optimization improves the system
- which optimization merely moves the bottleneck somewhere else

The central performance loop is:

```text
Question
   ↓
Baseline
   ↓
Workload
   ↓
Measurement
   ↓
Bottleneck
   ↓
Optimization / Architecture Change
   ↓
Measurement
   ↓
Trade-off
   ↓
Architectural Implication
```

Performance should therefore be treated as an architectural property, not merely a code-level optimization problem.

---

# 2. Purpose

This laboratory focuses on six fundamental performance dimensions:

```text
01-Latency
02-Throughput
03-Scalability
04-DatabaseContention
05-Messaging
06-Serialization
```

These dimensions are strongly related.

For example:

```text
Higher Concurrency
       ↓
More Database Connections
       ↓
Database Contention
       ↓
Higher Latency
       ↓
Lower Throughput
```

Another example:

```text
Larger Payload
       ↓
Serialization Cost
       ↓
CPU Cost
       ↓
Network Cost
       ↓
Higher Latency
       ↓
Lower Throughput
```

And:

```text
Higher Message Rate
       ↓
Broker / Consumer Load
       ↓
Consumer Lag
       ↓
Queue Growth
       ↓
Higher End-to-End Latency
```

Performance experiments should therefore investigate both direct effects and secondary bottlenecks.

---

# 3. Performance Philosophy

A performance claim without a workload is incomplete.

For example:

> "gRPC is faster than REST."

is not a useful architectural conclusion by itself.

A meaningful statement is:

> "Under this payload size, concurrency, request rate, hardware, network configuration, and workload, the measured P95 latency and CPU utilization were..."

The objective is to measure behavior under explicit constraints.

---

# 4. Common Experimental Model

Use a common business workload where possible.

Recommended domain:

```text
TransactionFlow
```

Baseline:

```text
Client
  ↓
ASP.NET Core API
  ↓
Application Service
  ↓
Domain Logic
  ↓
PostgreSQL
```

For messaging experiments:

```text
Transaction Service
        ↓
      Broker
        ↓
Transaction Processor
        ↓
     PostgreSQL
```

For distributed experiments:

```text
Client
  ↓
API
  ↓
Service A
  ↓
Service B
  ↓
Database
```

The business behavior should remain constant while the performance variable changes.

---

# 5. Common Performance Variables

Important variables include:

```text
Concurrency
Request Rate
Payload Size
Data Volume
Database Size
Message Rate
Message Size
Number of Consumers
CPU
Memory
Network Bandwidth
Network Latency
Connection Pool Size
Batch Size
Partition Count
Serialization Format
```

Record these variables for every experiment.

---

# 6. Performance Metrics

## 6.1 Latency

Measure:

```text
P50
P90
P95
P99
P99.9
Maximum
```

Do not rely only on averages.

Average latency can hide tail latency.

---

## 6.2 Throughput

Measure:

```text
Requests/sec
Transactions/sec
Messages/sec
Records/sec
Bytes/sec
```

---

## 6.3 Resource Utilization

Measure:

```text
CPU
Memory
GC
Network
Disk IO
Database Connections
Thread Pool
Connection Pool
```

---

## 6.4 Efficiency

Useful metrics include:

```text
Requests / CPU-second
Transactions / GB memory
Messages / CPU-second
Bytes / transaction
Database operations / request
```

---

## 6.5 Saturation

Identify the point where additional load stops increasing useful throughput.

Typical indicators:

```text
CPU → 100%
Memory → exhausted
Connection Pool → saturated
Database → lock contention
Queue → growing
Network → saturated
Thread Pool → exhausted
```

---

# 7. Measurement Principles

## 7.1 Establish a Baseline

Always measure the system before changing architecture or implementation.

---

## 7.2 Warm Up

Allow:

- JIT compilation
- connection establishment
- cache initialization
- database initialization
- broker connections

to stabilize before recording results.

---

## 7.3 Repeat Experiments

Do not rely on a single benchmark run.

Run multiple iterations.

Record:

```text
Mean
Median
Standard Deviation
P95
P99
```

where appropriate.

---

## 7.4 Control Variables

Change one major variable at a time.

For example:

```text
REST vs gRPC
```

should not simultaneously change:

- database
- hardware
- payload
- concurrency
- business logic

---

## 7.5 Measure End-to-End

Component benchmarks are useful, but system performance must also be measured end-to-end.

---

# 8. Performance Experiment 01 - Latency

## 8.1 Essence

Latency measures how long an operation takes from request initiation to completion.

```text
Request
   ↓
Processing
   ↓
Database
   ↓
Response
```

Latency is experienced by the caller.

---

## 8.2 Latency Components

Total latency may consist of:

```text
Network
+
Queueing
+
Application Processing
+
Database
+
Serialization
+
External Dependencies
```

For a distributed system:

```text
L_total =
L_network
+ L_serviceA
+ L_network
+ L_serviceB
+ L_database
+ L_serialization
```

---

## 8.3 Experiment

Measure:

```text
1 concurrent request
10 concurrent requests
100 concurrent requests
500 concurrent requests
1000 concurrent requests
```

Record:

```text
P50
P95
P99
Maximum
```

---

## 8.4 Tail Latency

Tail latency is particularly important for distributed systems.

A system may have:

```text
P50 = 20 ms
P99 = 800 ms
```

The average may still look acceptable while a significant subset of requests experiences poor latency.

---

## 8.5 Latency Experiment Variables

Test:

```text
Payload Size
Database Size
Concurrency
Network Latency
Cache Enabled/Disabled
Sync vs Async
REST vs gRPC
Serialization Format
```

---

## 8.6 Failure / Stress Experiment

Introduce:

```text
100 ms network delay
500 ms network delay
1 s network delay
```

Observe:

- latency propagation
- timeout behavior
- queueing
- throughput reduction

---

## 8.7 Key Questions

> Where is the latency actually coming from?

> Which architectural boundary contributes the most latency?

> Does reducing local processing time matter if network or database latency dominates?

---

# 9. Performance Experiment 02 - Throughput

## 9.1 Essence

Throughput measures how much useful work the system can process over time.

Examples:

```text
1000 transactions/sec
5000 messages/sec
100 requests/sec
```

---

## 9.2 Throughput vs Latency

Increasing throughput can increase latency.

Typical behavior:

```text
Load
 ↓
Throughput ↑
 ↓
Resources become saturated
 ↓
Queueing ↑
 ↓
Latency ↑
 ↓
Throughput stops increasing
```

---

## 9.3 Experiment

Gradually increase request rate:

```text
100 req/s
250 req/s
500 req/s
1000 req/s
2000 req/s
5000 req/s
```

Measure:

```text
Throughput
P50
P95
P99
CPU
Memory
DB utilization
Error rate
```

---

## 9.4 Saturation Point

Identify:

> At what workload does additional load stop producing useful throughput?

Example:

```text
1000 req/s → 1000 req/s processed
2000 req/s → 1900 req/s processed
3000 req/s → 2050 req/s processed
5000 req/s → 2100 req/s processed
```

The system is approaching a throughput ceiling.

---

## 9.5 Throughput Bottlenecks

Possible bottlenecks:

- CPU
- database
- network
- serialization
- thread pool
- connection pool
- broker
- consumer
- external dependency

---

## 9.6 Key Questions

> What is the maximum sustainable throughput?

> What becomes saturated first?

> Does adding capacity increase useful throughput?

---

# 10. Performance Experiment 03 - Scalability

## 10.1 Essence

Scalability describes how system capacity changes as resources or workload increase.

A system may scale:

- vertically
- horizontally
- by partitioning
- by sharding
- by increasing consumers
- by increasing database capacity

---

# 10.2 Vertical Scaling

Example:

```text
2 CPU → 4 CPU → 8 CPU → 16 CPU
```

Measure throughput and latency at each level.

---

# 10.3 Horizontal Scaling

Example:

```text
1 instance
2 instances
4 instances
8 instances
16 instances
```

Measure:

```text
Throughput
Latency
CPU
Network
Database load
Coordination overhead
```

---

# 10.4 Scaling Efficiency

A useful concept is:

```text
Scaling Efficiency =
Actual Capacity Increase
/
Ideal Capacity Increase
```

Example:

```text
1 instance → 1000 req/s

4 instances → 3500 req/s
```

Ideal capacity:

```text
4000 req/s
```

Scaling efficiency:

```text
3500 / 4000 = 87.5%
```

---

# 10.5 Linear vs Non-Linear Scaling

Ideal:

```text
Instances   Throughput

1           1000
2           2000
4           4000
8           8000
```

Real systems may behave like:

```text
Instances   Throughput

1           1000
2           1900
4           3500
8           5600
```

The gap is architectural evidence.

---

# 10.6 Scaling Bottlenecks

Horizontal scaling may fail to improve performance when a shared dependency remains:

```text
Service Instances
  ↓
Single Database
```

Adding more application instances can simply increase database pressure.

---

# 10.7 Experiment

Compare:

```text
1 instance
2 instances
4 instances
8 instances
```

with:

```text
Fixed Workload
Increasing Workload
```

Measure scaling efficiency.

---

# 10.8 Key Questions

> What scales independently?

> What remains shared?

> Where does the bottleneck move as capacity increases?

> Is the architecture actually scalable, or is only one layer scalable?

---

# 11. Performance Experiment 04 - Database Contention

## 11.1 Essence

Database contention occurs when concurrent operations compete for shared database resources.

Possible contention points:

- locks
- rows
- indexes
- pages
- connections
- CPU
- IO
- transactions
- hot partitions

---

# 11.2 Typical Scenario

```text
100 Requests
     ↓
Connection Pool
     ↓
Database
     ↓
Same Hot Row
```

Concurrency increases contention.

---

# 11.3 Lock Contention

Example:

```text
Transaction A
    ↓
Locks Row X

Transaction B
    ↓
Waits for Row X
```

The database may become the latency bottleneck.

---

# 11.4 Connection Pool Contention

Example:

```text
1000 Requests
      ↓
100 DB Connections
      ↓
900 Requests Waiting
```

Increasing application concurrency does not necessarily increase database throughput.

---

# 11.5 Hot Row Experiment

Create a workload where many transactions update the same record.

Compare:

```text
1 worker
10 workers
50 workers
100 workers
500 workers
```

Measure:

```text
Latency
Throughput
Lock Wait
CPU
Connection Pool
Transaction Duration
```

---

# 11.6 Index Contention

Compare:

```text
Indexed Write
Non-Indexed Write
Heavy Indexing
```

Observe:

- write throughput
- query latency
- CPU
- IO

---

# 11.7 Transaction Isolation

Measure different isolation levels where appropriate.

For example:

```text
Read Committed
Repeatable Read
Serializable
```

Observe:

- concurrency
- locking
- latency
- throughput
- consistency behavior

---

# 11.8 Database Scaling

Compare:

```text
Single Database
Read Replica
Partitioning
Sharding
Separate Databases
```

The experiment should explicitly record the architectural complexity introduced by each strategy.

---

# 11.9 Key Questions

> Is the database the bottleneck?

> Is contention caused by data access patterns or infrastructure capacity?

> Does increasing application instances improve or worsen database performance?

---

# 12. Performance Experiment 05 - Messaging

## 12.1 Essence

Messaging performance measures how efficiently a messaging architecture can publish, transport, and consume messages.

Important dimensions:

```text
Producer Throughput
Broker Throughput
Consumer Throughput
End-to-End Latency
Queue Depth
Consumer Lag
```

---

# 12.2 Messaging Pipeline

```text
Producer
   ↓
Serialization
   ↓
Network
   ↓
Broker
   ↓
Network
   ↓
Consumer
   ↓
Deserialization
   ↓
Processing
   ↓
Database
```

Every stage can become a bottleneck.

---

# 12.3 Producer Throughput

Measure:

```text
Messages/sec
Bytes/sec
CPU
Memory
Network
```

with increasing message rates.

---

# 12.4 Consumer Throughput

Test:

```text
1 consumer
2 consumers
4 consumers
8 consumers
16 consumers
```

Measure:

```text
Messages/sec
Consumer Lag
CPU
Memory
Database Load
```

---

# 12.5 Queue Growth

If:

```text
Producer Rate > Consumer Rate
```

then:

```text
Queue Depth ↑
Consumer Lag ↑
End-to-End Latency ↑
```

This is a critical performance condition.

---

# 12.6 Backpressure Experiment

Produce:

```text
1000 msg/s
5000 msg/s
10000 msg/s
20000 msg/s
```

while the consumer processes at a fixed rate.

Measure:

- queue growth
- lag
- memory
- broker storage
- recovery time after load decreases

---

# 12.7 Batch Processing

Compare:

```text
1 message / transaction
10 messages / transaction
100 messages / transaction
1000 messages / transaction
```

Measure:

- throughput
- latency
- database operations
- memory
- failure recovery

Batching can improve throughput while increasing latency and failure granularity.

---

# 12.8 Partitioning

Test different partition counts.

Example:

```text
1 partition
4 partitions
8 partitions
16 partitions
32 partitions
```

Measure:

- producer throughput
- consumer throughput
- ordering behavior
- consumer parallelism
- broker utilization

---

# 12.9 Messaging Latency

Measure:

```text
Publish Time
       ↓
Broker Acceptance
       ↓
Consumer Receipt
       ↓
Processing Complete
```

Separate:

```text
Broker Latency
Consumer Lag
Processing Latency
End-to-End Latency
```

---

# 12.10 Key Questions

> What is the bottleneck: producer, broker, consumer, or downstream processing?

> How does the system behave when producers are faster than consumers?

> How much throughput does additional consumer parallelism provide?

---

# 13. Performance Experiment 06 - Serialization

## 13.1 Essence

Serialization converts application objects into a transferable or storable representation.

Deserialization performs the reverse operation.

Serialization affects:

- CPU
- memory
- payload size
- network bandwidth
- latency
- garbage collection
- storage

---

# 13.2 Typical Pipeline

```text
Object
  ↓
Serialization
  ↓
Bytes
  ↓
Network
  ↓
Bytes
  ↓
Deserialization
  ↓
Object
```

---

# 13.3 Formats

Compare appropriate formats such as:

```text
JSON
MessagePack
Protocol Buffers
```

The comparison should use equivalent business data.

---

# 13.4 Payload Size

Test:

```text
1 KB
10 KB
100 KB
1 MB
10 MB
```

Measure:

```text
Serialization Time
Deserialization Time
Payload Size
CPU
Memory
Network
End-to-End Latency
```

---

# 13.5 Serialization CPU Cost

A smaller payload is not automatically better if serialization requires significantly more CPU.

Measure:

```text
CPU / request
Serialization time / request
```

---

# 13.6 Allocation and GC

Serialization can generate significant temporary allocations.

Measure:

```text
Allocated Bytes
Gen 0 Collections
Gen 1 Collections
Gen 2 Collections
GC Pause Time
```

where relevant.

---

# 13.7 Compression

Compare:

```text
No Compression
Compression
```

Measure the trade-off:

```text
CPU Cost
vs
Network Reduction
```

Compression may reduce network bandwidth while increasing CPU and latency.

---

# 13.8 Schema Evolution

Performance should not be evaluated independently from compatibility.

A highly efficient serialization format may introduce:

- stronger schema coupling
- more complex versioning
- code generation
- compatibility constraints

Performance is therefore only one architectural dimension.

---

# 13.9 Key Questions

> How much CPU is spent converting data?

> How much network bandwidth does the representation consume?

> Does a smaller payload actually improve end-to-end performance?

> What compatibility cost does the serialization format introduce?

---

# 14. Cross-Performance Relationships

The six performance areas should also be studied together.

## 14.1 Latency → Throughput

Higher concurrency can increase throughput until queueing and contention increase latency.

```text
Concurrency
    ↓
Throughput ↑
    ↓
Saturation
    ↓
Latency ↑
```

---

## 14.2 Throughput → Database Contention

Higher throughput may create:

```text
More DB Operations
       ↓
More Locks
       ↓
More Contention
       ↓
Higher Latency
```

---

## 14.3 Throughput → Messaging

If:

```text
Producer > Consumer
```

then:

```text
Queue Depth ↑
Consumer Lag ↑
```

---

## 14.4 Serialization → Latency

```text
Large Payload
     ↓
Serialization
     ↓
Network Transfer
     ↓
Deserialization
     ↓
Higher Latency
```

---

## 14.5 Scalability → Contention

Adding application instances can increase database contention:

```text
1 API Instance
      ↓
DB Load = X

8 API Instances
      ↓
DB Load = 8X
```

The database may become the new bottleneck.

---

## 14.6 Messaging → Database Contention

Increasing consumer parallelism can overload the database:

```text
Consumers ↑
    ↓
DB Operations ↑
    ↓
Lock Contention ↑
    ↓
Consumer Throughput ↓
```

More consumers do not necessarily mean more throughput.

---

# 15. Performance Bottleneck Model

A useful model is:

```text
Client
  ↓
Network
  ↓
API
  ↓
Application
  ↓
Serialization
  ↓
Database / Broker
  ↓
External Dependency
```

At any point:

```text
One bottleneck dominates
```

Optimizing a non-bottleneck component may have little effect.

---

# 16. Little's Law

For systems with stable flow, Little's Law provides a useful relationship:

```text
L = λW
```

Where:

```text
L = average number of items in the system
λ = average arrival rate
W = average time in the system
```

For a messaging system:

```text
Queue Depth ≈ Throughput × Processing Latency
```

Example:

```text
Throughput = 1000 msg/s
Average processing latency = 2 seconds

Queue ≈ 2000 messages
```

This provides a useful way to reason about queue growth and latency.

---

# 17. Performance Saturation

Every system has capacity limits.

A typical curve looks like:

```text
Load
  ↓
Throughput ↑
  ↓
Resource Saturation
  ↓
Queueing ↑
  ↓
Latency ↑
  ↓
Errors ↑
  ↓
Throughput Plateaus
```

The experiment should identify the saturation point rather than only measuring peak throughput.

---

# 18. Performance Budget

Define explicit performance budgets.

Example:

```text
API P95 < 200 ms
API P99 < 500 ms

Throughput > 1000 transactions/sec

Error rate < 0.1%

Consumer lag < 5 seconds

Database CPU < 70%

Connection pool utilization < 80%
```

These values are examples for experiments, not universal production targets.

The important principle is that performance requirements should be explicit and testable.

---

# 19. Performance Regression Testing

Performance should become part of the engineering lifecycle.

Potential checks:

```text
P95 latency regression < 10%
Throughput regression < 5%
Allocation increase < 10%
Database query regression < 10%
Message lag within budget
```

Thresholds should be defined according to the system's actual requirements.

---

# 20. Architecture Fitness Functions

Performance constraints can become executable fitness functions.

Examples:

```text
API P95 latency must remain below the defined budget.
```

```text
Throughput must remain above the required capacity.
```

```text
Consumer lag must remain below the defined threshold.
```

```text
Database connection pool must not remain saturated.
```

```text
Serialization must not exceed the CPU budget.
```

```text
Horizontal scaling efficiency must remain above the defined threshold.
```

The purpose is to detect architectural performance regression automatically.

---

# 21. Performance Failure Experiments

Performance and failure are closely related.

Useful combined experiments include:

### CPU Saturation

```text
Load ↑
CPU → 100%
```

Measure latency and throughput.

### Database Saturation

```text
Concurrent Requests ↑
        ↓
Database Saturation
```

### Queue Saturation

```text
Producer Rate > Consumer Rate
```

### Connection Pool Saturation

```text
Requests > Available Connections
```

### Network Saturation

```text
Payload Size ↑
      ↓
Bandwidth Saturation
```

### Memory Pressure

```text
Concurrency ↑
      ↓
Allocations ↑
      ↓
GC ↑
      ↓
Latency ↑
```

---

# 22. Recommended Workload Types

Different workloads expose different architectural behavior.

## Constant Load

```text
1000 req/s for 10 minutes
```

Useful for stable capacity testing.

---

## Ramp Load

```text
100
200
500
1000
2000
5000 req/s
```

Useful for finding saturation.

---

## Burst Load

```text
100 req/s
      ↓
5000 req/s
      ↓
100 req/s
```

Useful for testing elasticity and backpressure.

---

## Sustained Load

```text
High load
for 30-60 minutes
```

Useful for detecting:

- memory leaks
- resource exhaustion
- queue growth
- GC problems
- connection leaks

---

## Spike Load

A sudden extreme increase.

Useful for testing:

- rate limiting
- load shedding
- autoscaling
- queue buffering

---

# 23. Benchmarking Strategy

Use different tools for different questions.

## BenchmarkDotNet

Useful for:

- serialization
- algorithms
- object allocation
- CPU-bound operations
- microbenchmarks

---

## Load Testing

Useful for:

- APIs
- distributed systems
- concurrency
- throughput
- latency

Potential tools:

```text
k6
NBomber
JMeter
```

---

## Database Testing

Measure:

- query latency
- lock waits
- transaction throughput
- connection pool usage
- CPU
- IO

---

## Messaging Testing

Measure:

- producer throughput
- consumer throughput
- broker throughput
- lag
- queue depth
- end-to-end latency

---

# 24. Observability

Performance experiments require observability.

Use:

```text
OpenTelemetry
Structured Logging
Metrics
Distributed Tracing
```

Important trace attributes:

```text
TraceId
SpanId
Service
Operation
Database
Message
Payload Size
Serialization Format
Duration
Status
```

Important metrics:

```text
Request Rate
Request Duration
Error Rate
CPU
Memory
GC
DB Connections
DB Lock Wait
Queue Depth
Consumer Lag
Messages/sec
Bytes/sec
```

---

# 25. Performance Experiment Result Format

Every experiment should finish with:

```text
## Result

### Question
<What are we trying to understand?>

### Hypothesis
<What do we expect?>

### Environment
<Hardware and software>

### Workload
<Request rate, concurrency, payload, data volume>

### Baseline
<Initial measurement>

### Experiment
<What changed?>

### Measurements
<P50/P95/P99, throughput, CPU, memory, etc.>

### Bottleneck
<What became saturated?>

### Results
<Measured behavior>

### Scaling Behavior
<How did the system behave as load increased?>

### Unexpected Behavior
<What surprised us?>

### Trade-offs
<What did the optimization or architecture change cost?>

### Conclusion
<What does the evidence show?>

### Architectural Implication
<What should an architect learn from this?>
```

---

# 26. Reproducibility

Every performance experiment should record:

```text
Git Commit
.NET Version
C# Version
OS
CPU
Memory
Storage
Network
Database Version
Broker Version
Container Runtime
Configuration
Dataset Size
Payload Size
Concurrency
Request Rate
Duration
Warm-up Duration
Benchmark Tool
Benchmark Version
```

Without reproducibility, performance numbers are difficult to interpret.

---

# 27. Suggested Folder Structure

```text
Performance/
│
├── README.md
│
├── Latency/
│   ├── README.md
│   ├── src/
│   ├── tests/
│   ├── benchmarks/
│   └── results/
│
├── Throughput/
│   ├── README.md
│   ├── src/
│   ├── tests/
│   ├── benchmarks/
│   └── results/
│
├── Scalability/
│   ├── README.md
│   ├── src/
│   ├── tests/
│   ├── benchmarks/
│   └── results/
│
├── DatabaseContention/
│   ├── README.md
│   ├── src/
│   ├── tests/
│   ├── benchmarks/
│   └── results/
│
├── Messaging/
│   ├── README.md
│   ├── src/
│   ├── tests/
│   ├── benchmarks/
│   └── results/
│
└── Serialization/
    ├── README.md
    ├── src/
    ├── tests/
    ├── benchmarks/
    └── results/
```

---

# 28. Recommended Experiment Sequence

The experiments should build progressively.

## Phase 1 - Latency

Start with:

```text
Latency
```

Understand where time is spent.

---

## Phase 2 - Throughput

Then:

```text
Throughput
```

Understand system capacity.

---

## Phase 3 - Scalability

Then:

```text
Scalability
```

Understand how capacity changes when resources increase.

---

## Phase 4 - Database Contention

Then:

```text
DatabaseContention
```

Understand how concurrency affects shared state.

---

## Phase 5 - Messaging

Then:

```text
Messaging
```

Understand asynchronous throughput, lag, batching, and backpressure.

---

## Phase 6 - Serialization

Finally:

```text
Serialization
```

Understand the cost of moving data between architectural boundaries.

---

# 29. Recommended First Experiments

A practical sequence for this repository:

### Experiment 01

```text
ASP.NET Core API
1 → 10 → 100 → 1000 concurrent requests
```

Measure latency.

---

### Experiment 02

```text
100 → 500 → 1000 → 2000 → 5000 req/s
```

Find throughput saturation.

---

### Experiment 03

```text
1 → 2 → 4 → 8 API instances
```

Measure scaling efficiency.

---

### Experiment 04

```text
1 → 10 → 100 → 500 concurrent DB updates
```

Create database contention.

---

### Experiment 05

```text
1000 → 5000 → 10000 messages/sec
```

Measure broker and consumer behavior.

---

### Experiment 06

Compare:

```text
JSON
MessagePack
Protocol Buffers
```

using identical payloads.

---

# 30. Relationship With Other ArchitectureExperiments

Performance should connect with the other experimental categories.

The complete workflow becomes:

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
REST vs gRPC
      ↓
Latency
      ↓
Serialization
      ↓
CPU / Network
      ↓
Throughput
      ↓
Trade-off
      ↓
ADR
```

Another example:

```text
Shared Database vs Owned Data
      ↓
Database Contention
      ↓
Throughput
      ↓
Scalability
      ↓
Failure Isolation
      ↓
Trade-off
      ↓
ADR
```

And:

```text
Sync vs Async
      ↓
Latency
      ↓
Messaging
      ↓
Throughput
      ↓
Consumer Scaling
      ↓
Failure Modes
      ↓
Trade-off
      ↓
ADR
```

---

# 31. What Not To Do

Avoid:

- optimizing before measuring
- benchmarking only one request
- using average latency only
- ignoring P95/P99
- comparing different workloads
- changing multiple variables simultaneously
- benchmarking without warm-up
- ignoring CPU and memory
- ignoring database behavior
- ignoring network behavior
- ignoring serialization
- measuring peak throughput without sustainable throughput
- assuming horizontal scaling is linear
- assuming more consumers always improve throughput
- assuming smaller payloads automatically mean better performance
- treating benchmark numbers as universal truths

Performance results are contextual.

---

# 32. Performance as an Architectural Property

Performance is not only a property of code.

Architecture determines:

```text
Number of Network Calls
Number of Database Calls
Transaction Boundaries
Data Ownership
Communication Model
Serialization
Concurrency
Parallelism
Queueing
Caching
Partitioning
Scaling Boundaries
Failure Isolation
```

Therefore:

```text
Architecture
     ↓
Workload Behavior
     ↓
Resource Usage
     ↓
Performance
```

---

# 33. Architecture as Evidence

Avoid conclusions such as:

> "Async is faster."

Instead:

```text
Under a burst workload of X messages/sec,
the asynchronous architecture absorbed the burst
without blocking request threads, but introduced
processing delay and eventual consistency.
```

Avoid:

> "Microservices scale better."

Instead:

```text
Under an uneven workload where Orders received
significantly more traffic than Payments, independent
service scaling reduced resource allocation for the
lower-volume capability, while introducing additional
network and operational overhead.
```

Avoid:

> "gRPC is faster."

Instead:

```text
Under the tested payload sizes and concurrency levels,
the measured serialization, network, CPU, and P95/P99
latency characteristics were...
```

Performance conclusions should always include:

```text
Workload
+
Environment
+
Measurement
+
Observed Behavior
+
Trade-off
```

---

# 34. Definition of Done

A performance experiment is complete when it has:

- [ ] Explicit performance question
- [ ] Baseline
- [ ] Hypothesis
- [ ] Controlled workload
- [ ] Defined variables
- [ ] Warm-up
- [ ] Repeated measurements
- [ ] P50/P95/P99 where appropriate
- [ ] Throughput measurement
- [ ] CPU measurement
- [ ] Memory measurement
- [ ] Relevant infrastructure metrics
- [ ] Bottleneck identification
- [ ] Saturation analysis
- [ ] Scaling analysis where relevant
- [ ] Failure/stress scenario where relevant
- [ ] Before/after comparison
- [ ] Unexpected behavior documented
- [ ] Trade-offs documented
- [ ] Reproducible environment
- [ ] Results committed
- [ ] Architectural implication documented

---

# 35. Key Takeaways

Performance engineering is not:

```text
"Make it faster."
```

It is:

```text
Understand the workload
        ↓
Measure the baseline
        ↓
Find the bottleneck
        ↓
Change the architecture or implementation
        ↓
Measure again
        ↓
Understand the new bottleneck
        ↓
Evaluate the trade-off
```

The six core areas provide the foundation:

```text
Latency
   ↓
Throughput
   ↓
Scalability
   ↓
Database Contention
   ↓
Messaging
   ↓
Serialization
```

The most important performance questions are:

```text
How fast is it?
How much work can it process?
How does capacity change with more resources?
What becomes the bottleneck?
What happens under contention?
How efficiently does data move?
What happens when load increases?
Where does performance stop scaling?
```

The ultimate goal is not to find the fastest implementation in isolation.

It is to understand:

```text
Under this workload
with these constraints
on this architecture
using these resources
the system behaves like this.
```

That is architectural performance evidence.

> **Performance is not a single number. It is the measurable behavior of an architecture under a defined workload and a defined set of constraints.**
