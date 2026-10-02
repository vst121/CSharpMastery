# Pattern Comparison

## 1. Essence

**Pattern Comparison** is the comparative analysis area of the ArchitectureExperiments laboratory.

The purpose is not to determine which architecture is universally better.

The purpose is to investigate how different architectural approaches behave under comparable conditions.

The core model is:

```text
Architecture A
      │
      ├── Same Business Problem
      ├── Same Workload
      ├── Same Constraints
      └── Same Measurement Criteria
                │
                ▼
          Architecture B
                │
                ▼
        Evidence + Trade-offs
```

The fundamental question is:

> **Under which conditions does an architectural choice produce a meaningful advantage, and what does that advantage cost?**

---

# 2. Purpose

Architecture discussions often become statements such as:

```text
"Microservices scale better."

"Async is more resilient."

"gRPC is faster."

"Shared databases are bad."

"Distributed systems are more scalable."

"Layered architecture is simpler."
```

These statements may contain useful intuition, but they are incomplete without context.

Architecture depends on:

```text
Workload
Scale
Consistency Requirements
Failure Model
Team Structure
Deployment Model
Data Ownership
Latency Requirements
Operational Capability
Security
Cost
Change Frequency
```

PatternComparison turns these assumptions into experiments.

---

# 3. Comparison Philosophy

A comparison should not ask:

```text
Which architecture is better?
```

It should ask:

```text
What problem are we solving?

What constraints exist?

What changes between the architectures?

What remains constant?

How does each architecture behave?

What trade-offs appear?

Under which conditions is each approach appropriate?
```

The output is evidence, not a universal ranking.

---

# 4. Comparison Categories

This laboratory contains six major comparisons:

```text
01 - Modular vs Layered
02 - Modular Monolith vs Microservices
03 - Synchronous vs Asynchronous
04 - REST vs gRPC
05 - Shared Database vs Owned Data
06 - Monolith vs Distributed
```

Each comparison investigates a different architectural dimension.

---

# 5. Common Experimental Model

All comparisons should follow:

```text
Question
   ↓
Context
   ↓
Baseline
   ↓
Architecture A
   ↓
Architecture B
   ↓
Controlled Workload
   ↓
Measurement
   ↓
Failure Experiment
   ↓
Trade-off Analysis
   ↓
Architectural Implication
```

The comparison should change the architectural variable while keeping other variables as stable as practical.

---

# 6. Common Experimental Domain

Where practical, use a common business domain across experiments.

A good candidate is:

```text
Transaction Processing
```

Example:

```text
Client
  ↓
Transaction API
  ↓
Transaction Processing
  ↓
Persistence
  ↓
Result
```

The same business behavior can then be implemented using different architectures.

This makes comparisons more meaningful because the experiment changes the architecture rather than the business problem.

---

# 7. Common Baseline

A simple baseline can be:

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

Then architectural characteristics can be introduced one at a time.

For example:

```text
Baseline
   ↓
Modular
   ↓
Microservices
   ↓
Async
   ↓
Owned Data
   ↓
Distributed
```

The exact sequence is not mandatory.

---

# 8. Comparison 01 - Modular vs Layered

## 8.1 Question

How does explicit modular structure behave compared with conventional layered architecture when the system grows in business capabilities and development teams?

---

## 8.2 Layered Architecture

Typical structure:

```text
Presentation
     ↓
Application
     ↓
Domain
     ↓
Infrastructure
```

All features may share the same layers.

Example:

```text
Application
├── Orders
├── Payments
├── Customers
└── Inventory
```

Infrastructure may contain:

```text
Repositories
Database
Messaging
External APIs
```

---

## 8.3 Modular Architecture

A modular architecture organizes around business boundaries:

```text
Orders
├── Domain
├── Application
├── Infrastructure
└── Public

Payments
├── Domain
├── Application
├── Infrastructure
└── Public

Customers
├── Domain
├── Application
├── Infrastructure
└── Public
```

The important difference is not the number of folders.

It is the ownership boundary.

---

## 8.4 Hypothesis

A modular architecture should make dependency boundaries and change ownership more explicit than a conventional layered architecture.

The cost is additional structural complexity and stricter dependency management.

---

## 8.5 Measure

Measure:

```text
Dependency Count
Cross-Module References
Changed Files per Feature
Build Impact
Test Isolation
Architecture Violations
Code Ownership
Change Coupling
```

A useful metric is:

```text
Change Coupling =
Number of unrelated modules changed
for one business change
```

---

## 8.6 Failure Experiment

Intentionally introduce:

```text
Orders
   ↓
Payments.Infrastructure
```

Then verify that architecture tests detect the violation.

The experiment demonstrates whether the intended module boundary actually exists.

---

## 8.7 Trade-offs

### Layered

Characteristics:

```text
Simple Mental Model
Familiar Structure
Low Initial Complexity
Easy Small-System Development
```

Potential costs:

```text
Weak Business Boundaries
Cross-Feature Coupling
Shared Infrastructure
Large Change Surface
```

### Modular

Characteristics:

```text
Explicit Boundaries
Business Ownership
Dependency Control
Better Change Isolation
```

Potential costs:

```text
More Design Effort
More Interfaces
Boundary Management
Architecture Testing
```

---

## 8.8 Architectural Questions

```text
How often do changes cross module boundaries?

Can teams work independently?

Can modules be tested independently?

Are dependencies directional?

Can one module evolve without changing others?
```

---

# 9. Comparison 02 - Modular Monolith vs Microservices

## 9.1 Question

When does a modular monolith provide sufficient architectural isolation, and when does independent deployment justify the additional cost of microservices?

---

## 9.2 Modular Monolith

```text
             Application
        ┌───────┼────────┐
        ↓       ↓        ↓
     Orders  Payments Customers
        │       │        │
        └───────┼────────┘
                ↓
           Infrastructure
```

One deployment unit.

Multiple architectural boundaries.

---

## 9.3 Microservices

```text
        Orders Service
             │
        Orders DB

        Payments Service
             │
        Payments DB

        Customers Service
             │
        Customers DB
```

Each service can potentially have:

```text
Independent Deployment
Independent Scaling
Independent Runtime
Independent Data
Independent Failure Domain
```

---

## 9.4 Hypothesis

A modular monolith should reduce operational complexity while preserving many benefits of explicit internal boundaries.

Microservices should provide stronger deployment and failure isolation at the cost of distributed-system complexity.

---

## 9.5 Measure

```text
Deployment Time
Build Time
Request Latency
Network Calls
Failure Propagation
Operational Components
Resource Usage
Independent Scaling
Team Coupling
Data Coupling
```

---

## 9.6 Failure Experiment

Simulate:

```text
Payments Failure
```

Compare:

```text
Modular Monolith
```

against:

```text
Microservices
```

Measure:

```text
Blast Radius
Recovery
User Impact
Failure Detection
```

---

## 9.7 Distributed-System Cost

The microservice implementation introduces:

```text
Network Calls
Timeouts
Retries
Service Discovery
Distributed Tracing
Contract Versioning
Distributed Transactions
Eventual Consistency
Independent Deployment
```

These should be treated as architectural costs.

---

## 9.8 Trade-offs

### Modular Monolith

```text
Simple Deployment
Low Network Overhead
Easy Transactions
Simple Debugging
Lower Operational Burden
```

Potential limitations:

```text
Shared Runtime
Shared Failure Domain
Deployment Coupling
Scaling Coupling
```

### Microservices

```text
Independent Deployment
Independent Scaling
Failure Isolation
Team Ownership
Technology Independence
```

Potential costs:

```text
Network Failure
Distributed Consistency
Operational Complexity
Observability
Deployment Complexity
```

---

## 9.9 Architectural Questions

```text
Do services need independent deployment?

Do components scale differently?

Are failure domains required?

Are teams independently responsible?

Is operational complexity justified?

Can the modular monolith satisfy the requirements?
```

---

# 10. Comparison 03 - Synchronous vs Asynchronous

## 10.1 Question

How does synchronous communication compare with asynchronous communication in terms of latency, throughput, coupling, resilience, consistency, and operational complexity?

---

## 10.2 Synchronous

```text
Client
  ↓
Service A
  ↓
Service B
  ↓
Response
```

Service A waits for Service B.

---

## 10.3 Asynchronous

```text
Service A
   ↓
Message Broker
   ↓
Service B
```

Service A does not need to wait for Service B to complete processing.

---

## 10.4 Hypothesis

Asynchronous communication should reduce temporal coupling and absorb workload bursts, but introduce additional consistency, delivery, ordering, and observability complexity.

---

## 10.5 Measure

```text
Request Latency
End-to-End Latency
Throughput
Queue Depth
Consumer Lag
Error Rate
Retry Rate
Resource Utilization
```

---

## 10.6 Burst Experiment

Generate:

```text
100 requests/sec
```

then:

```text
1000 requests/sec
```

then:

```text
5000 requests/sec
```

Observe:

```text
Synchronous:
Latency
Timeouts
Connection Pool
CPU

Asynchronous:
Queue Depth
Consumer Lag
Processing Rate
End-to-End Latency
```

---

## 10.7 Failure Experiment

Stop the downstream processor.

### Synchronous

Observe:

```text
Timeouts
Connection Growth
Retry Behavior
Request Failure
```

### Asynchronous

Observe:

```text
Queue Growth
Consumer Lag
Delayed Processing
DLQ Behavior
```

---

## 10.8 Trade-offs

### Synchronous

Advantages:

```text
Simple Request/Response
Immediate Result
Simple User Interaction
Straightforward Debugging
```

Costs:

```text
Temporal Coupling
Failure Propagation
Limited Burst Absorption
Dependency Availability
```

### Asynchronous

Advantages:

```text
Temporal Decoupling
Queue-Based Load Leveling
Independent Processing
Natural Background Execution
```

Costs:

```text
Eventual Consistency
Duplicate Handling
Ordering
Message Operations
DLQ
Distributed Tracing
```

---

## 10.9 Architectural Questions

```text
Does the caller need an immediate result?

Can processing be delayed?

Can eventual consistency be accepted?

How important is burst absorption?

What happens when the consumer is unavailable?

Can the operation be idempotent?
```

---

# 11. Comparison 04 - REST vs gRPC

## 11.1 Question

How do REST and gRPC behave under equivalent service-to-service workloads?

---

## 11.2 REST

Typical architecture:

```text
Client
  ↓
HTTP
  ↓
JSON
  ↓
REST API
```

Characteristics:

```text
Human-Readable
HTTP-Based
Broad Ecosystem
Easy Browser Integration
Flexible Payloads
```

---

## 11.3 gRPC

Typical architecture:

```text
Client
  ↓
HTTP/2
  ↓
Protocol Buffers
  ↓
gRPC Service
```

Characteristics:

```text
Strong Contracts
Binary Serialization
Code Generation
HTTP/2
Streaming
Efficient Service-to-Service Communication
```

---

## 11.4 Hypothesis

For appropriate internal service-to-service workloads, gRPC may reduce serialization overhead and provide strong contracts, while REST may provide broader interoperability and simpler external integration.

The experiment should measure the actual workload rather than assuming either result.

---

## 11.5 Measure

```text
Requests/sec
P50 Latency
P95 Latency
P99 Latency
Payload Size
CPU
Memory
Serialization Time
Network Bandwidth
Error Rate
```

---

## 11.6 Workloads

Test different payload sizes:

```text
1 KB
10 KB
100 KB
1 MB
```

Test different concurrency:

```text
1
10
100
1000
```

Test:

```text
Unary Calls
Streaming
Large Payloads
Small Payloads
```

---

## 11.7 Failure Experiment

Introduce:

```text
Network Delay
Network Failure
Server Timeout
Connection Saturation
```

Observe how each communication model behaves.

---

## 11.8 Trade-offs

### REST

```text
Broad Compatibility
Simple Tooling
Human Readability
Browser Friendly
Easy External APIs
```

Potential costs:

```text
JSON Overhead
Less Strict Contracts
Potentially Larger Payloads
```

### gRPC

```text
Strong Contracts
Binary Serialization
Code Generation
Streaming
Efficient Internal Communication
```

Potential costs:

```text
More Specialized Tooling
Less Direct Browser Usage
Protocol Complexity
```

---

## 11.9 Architectural Questions

```text
Is this an external or internal API?

Is browser compatibility required?

Is strong schema enforcement important?

Are streaming capabilities required?

Is payload efficiency important?

Does the team have gRPC operational experience?
```

---

# 12. Comparison 05 - Shared Database vs Owned Data

## 12.1 Question

How does shared database access compare with explicit data ownership in terms of coupling, consistency, autonomy, and operational complexity?

---

## 12.2 Shared Database

```text
Orders ──────┐
Payments ────┼──→ Shared Database
Customers ───┘
```

Multiple components access the same database.

---

## 12.3 Owned Data

```text
Orders
  ↓
Orders DB

Payments
  ↓
Payments DB

Customers
  ↓
Customers DB
```

Each component owns its authoritative data.

---

## 12.4 Hypothesis

A shared database simplifies local transactions and cross-domain queries but creates structural coupling.

Owned data increases autonomy and isolation but introduces distributed consistency and integration complexity.

---

## 12.5 Measure

```text
Cross-Component Queries
Schema Dependencies
Deployment Coupling
Transaction Complexity
Data Synchronization
Query Latency
Consistency Lag
Failure Propagation
```

---

## 12.6 Failure Experiment

Make the Payments database unavailable.

Compare:

### Shared Database

```text
Shared DB Failure
      ↓
Orders
Payments
Customers
```

### Owned Data

```text
Payments DB Failure
      ↓
Payments
```

Then measure actual system behavior.

---

## 12.7 Consistency Experiment

Compare:

```text
Single Database Transaction
```

against:

```text
Service A
  ↓
Event
  ↓
Service B
```

Measure:

```text
Consistency Delay
Failure Recovery
Duplicate Events
Reconciliation
```

---

## 12.8 Trade-offs

### Shared Database

Advantages:

```text
Simple Transactions
Simple Queries
Low Data Synchronization
Simple Reporting
```

Costs:

```text
Strong Coupling
Shared Schema
Deployment Coordination
Limited Ownership
Large Failure Domain
```

### Owned Data

Advantages:

```text
Clear Ownership
Independent Evolution
Isolation
Independent Scaling
```

Costs:

```text
Distributed Queries
Eventual Consistency
Synchronization
More Infrastructure
More Complex Transactions
```

---

## 12.9 Architectural Questions

```text
Who owns the data?

Who is allowed to write it?

Are cross-domain transactions required?

Can eventual consistency be accepted?

Can reporting use a separate analytical model?

Do teams need independent evolution?
```

---

# 13. Comparison 06 - Monolith vs Distributed

## 13.1 Question

What architectural costs and benefits appear when a system moves from a single process to multiple independently executing components?

---

## 13.2 Monolith

```text
Client
  ↓
Application
  ├── Orders
  ├── Payments
  ├── Customers
  └── Inventory
       ↓
    Database
```

Components execute inside one application boundary.

---

## 13.3 Distributed Architecture

```text
          ┌── Orders
Client ───┼── Payments
          ├── Customers
          └── Inventory
```

Components communicate across network boundaries.

---

## 13.4 Hypothesis

A distributed architecture can provide independent scaling, deployment, and failure boundaries, but introduces network failure, latency, consistency, coordination, and operational complexity.

---

## 13.5 Measure

```text
Latency
Throughput
Deployment Independence
Failure Isolation
Network Traffic
Operational Components
Resource Utilization
Recovery Time
Observability Overhead
```

---

## 13.6 Network Failure Experiment

Introduce:

```text
100 ms latency
500 ms latency
Packet loss
Connection failure
Service timeout
```

Compare:

```text
Monolith
```

against:

```text
Distributed
```

Observe the difference created by the network boundary.

---

## 13.7 Scaling Experiment

Generate uneven workload:

```text
Orders:     1000 req/s
Payments:     50 req/s
Customers:   100 req/s
```

A monolith may scale the entire application.

A distributed system can potentially scale components independently.

Measure:

```text
Resource Waste
Scaling Granularity
Cost
Throughput
```

---

## 13.8 Failure Isolation Experiment

Stop one component.

Measure:

```text
Affected Components
Affected Requests
User Impact
Recovery
```

This reveals whether the architecture actually created a useful failure boundary.

---

## 13.9 Trade-offs

### Monolith

```text
Simple Deployment
Simple Calls
Simple Transactions
Simple Debugging
Low Network Overhead
```

Potential costs:

```text
Shared Scaling
Shared Deployment
Larger Failure Domain
Potential Internal Coupling
```

### Distributed

```text
Independent Deployment
Independent Scaling
Failure Isolation
Team Autonomy
```

Potential costs:

```text
Network Failure
Distributed State
Observability
Deployment Complexity
Operational Burden
```

---

## 13.10 Architectural Questions

```text
Is independent scaling required?

Is independent deployment required?

Are failure boundaries required?

Can network failure be tolerated?

Is the operational capability available?

Does distribution solve a real problem?
```

---

# 14. Cross-Comparison Dimensions

All six comparisons can use a common evaluation model.

| Dimension          | Questions                                            |
| ------------------ | ---------------------------------------------------- |
| Complexity         | How much architectural complexity is introduced?     |
| Coupling           | How strongly are components dependent on each other? |
| Cohesion           | Are related responsibilities kept together?          |
| Deployment         | Can components evolve independently?                 |
| Scaling            | Can capacity be scaled independently?                |
| Latency            | What communication overhead exists?                  |
| Throughput         | How does the architecture behave under load?         |
| Consistency        | What consistency model is required?                  |
| Failure Isolation  | How far does failure propagate?                      |
| Observability      | How difficult is diagnosis?                          |
| Security           | Where are trust boundaries?                          |
| Cost               | What infrastructure and operational cost exists?     |
| Team Ownership     | Can teams work independently?                        |
| Changeability      | How large is the change surface?                     |
| Operational Burden | How difficult is the system to operate?              |

These dimensions should be measured or documented rather than converted into a single score.

---

# 15. Common Metrics

## Performance

```text
Throughput
P50
P95
P99
Maximum Latency
CPU
Memory
Network
Database IO
```

## Reliability

```text
Error Rate
Failure Rate
Recovery Time
Recovery Point
Availability
Retry Rate
DLQ Rate
```

## Architecture

```text
Dependency Count
Cross-Boundary Calls
Coupling
Change Surface
Deployment Units
Data Owners
```

## Operations

```text
Number of Services
Number of Databases
Number of Queues
Number of Deployments
Configuration Complexity
Monitoring Complexity
```

---

# 16. Failure Testing

Every comparison involving distributed or asynchronous behavior should include failure experiments.

Common failures:

```text
Dependency Down
Timeout
Network Delay
Network Loss
Database Failure
Broker Failure
Consumer Failure
Duplicate Message
Message Reordering
Resource Exhaustion
```

The important question is:

```text
What happens when the architecture operates outside the happy path?
```

---

# 17. Architecture Fitness Functions

Comparisons should also verify structural properties.

Examples:

```text
Modules must not access another module's infrastructure.
```

```text
Services must not access another service's database.
```

```text
External calls must have bounded timeouts.
```

```text
Message consumers must be idempotent.
```

```text
Public contracts must not expose internal domain models.
```

```text
Architecture layers must preserve dependency direction.
```

These rules can become automated architecture tests.

---

# 18. Reproducibility

Each comparison should record:

```text
Git Commit
.NET Version
Database Version
Broker Version
Container Versions
Hardware
CPU
Memory
Dataset
Payload Size
Concurrency
Request Rate
Configuration
Benchmark Parameters
```

The same experiment should be executable again.

---

# 19. Experiment Result Format

Each experiment should produce:

```text
Question
Hypothesis
Environment
Architecture A
Architecture B
Workload
Measurements
Failure Scenarios
Results
Unexpected Behavior
Trade-offs
Architectural Implications
```

Example:

```text
Question:
Does asynchronous messaging absorb traffic bursts better?

Hypothesis:
Yes.

Baseline:
Synchronous HTTP.

Experiment:
HTTP vs Kafka.

Result:
Measured throughput and queue behavior.

Unexpected:
End-to-end latency increased.

Trade-off:
Better burst absorption at the cost of delayed completion.

Implication:
Async is useful for workloads where temporal decoupling is more important than immediate completion.
```

---

# 20. What Not To Do

Avoid:

### Technology Benchmarking Without Architecture

```text
Kafka benchmark
gRPC benchmark
PostgreSQL benchmark
```

without an architectural question.

---

### Uncontrolled Comparisons

Changing:

```text
Architecture
Database
Hardware
Workload
Serialization
Caching
```

all at once makes the result difficult to interpret.

---

### Single-Run Conclusions

One benchmark run is not reliable evidence.

---

### Only Measuring Speed

Architecture is not only performance.

Measure:

```text
Performance
Reliability
Complexity
Coupling
Failure Isolation
Operational Burden
Cost
```

---

### Ignoring Failure

An architecture that performs well under ideal conditions may behave very differently under dependency failure.

---

### Universal Conclusions

Avoid:

```text
"Microservices are better."
```

Prefer:

```text
"Under this workload and these constraints, the experiment showed..."
```

---

# 21. Recommended Implementation Strategy

Build the comparisons progressively.

### Phase 1

Implement a common domain:

```text
TransactionFlow
```

with:

```text
ASP.NET Core
PostgreSQL
.NET 10
```

### Phase 2

Implement:

```text
Modular
Layered
```

### Phase 3

Add:

```text
Microservices
```

### Phase 4

Add:

```text
Synchronous
Asynchronous
```

### Phase 5

Add:

```text
REST
gRPC
```

### Phase 6

Add:

```text
Shared Database
Owned Data
```

### Phase 7

Add:

```text
Monolith
Distributed
```

### Phase 8

Introduce:

```text
Load Tests
Failure Injection
OpenTelemetry
Architecture Tests
```

### Phase 9

Document:

```text
Measurements
Trade-offs
Unexpected Results
Architectural Lessons
```

### Phase 10

Create ADRs from the evidence.

---

# 22. Suggested Folder Structure

```text
PatternComparison/
│
├── README.md
│
├── ModularVsLayered/
│   ├── README.md
│   ├── Modular/
│   ├── Layered/
│   ├── tests/
│   └── benchmarks/
│
├── ModularVsMicroservices/
│   ├── README.md
│   ├── ModularMonolith/
│   ├── Microservices/
│   ├── tests/
│   └── benchmarks/
│
├── SyncVsAsync/
│   ├── README.md
│   ├── Synchronous/
│   ├── Asynchronous/
│   ├── tests/
│   └── benchmarks/
│
├── RESTVsGrpc/
│   ├── README.md
│   ├── REST/
│   ├── Grpc/
│   ├── tests/
│   └── benchmarks/
│
├── SharedDatabaseVsOwnedData/
│   ├── README.md
│   ├── SharedDatabase/
│   ├── OwnedData/
│   ├── tests/
│   └── benchmarks/
│
└── MonolithVsDistributed/
    ├── README.md
    ├── Monolith/
    ├── Distributed/
    ├── tests/
    └── benchmarks/
```

The subdirectories can initially remain empty while this README defines the experimental contract.

---

# 23. Architecture Comparison Matrix

| Comparison                        | Primary Question                                               | Main Dimensions                        |
| --------------------------------- | -------------------------------------------------------------- | -------------------------------------- |
| Modular vs Layered                | How much does explicit modularity improve change isolation?    | Coupling, cohesion, dependencies       |
| Modular Monolith vs Microservices | When is deployment independence worth distributed complexity?  | Deployment, scaling, failure isolation |
| Sync vs Async                     | When does temporal decoupling justify eventual consistency?    | Latency, throughput, resilience        |
| REST vs gRPC                      | How do communication models behave under equivalent workloads? | Latency, serialization, contracts      |
| Shared DB vs Owned Data           | What is the cost of explicit data ownership?                   | Coupling, consistency, autonomy        |
| Monolith vs Distributed           | What does introducing network boundaries actually change?      | Failure, latency, scaling, operations  |

---

# 24. Relationship With Other ArchitectureExperiments

PatternComparison should not exist in isolation.

The broader experimental structure is:

```text
ArchitectureExperiments
│
├── PatternComparison
│       │
│       └── Compare architectural alternatives
│
├── FailureModes
│       │
│       └── Break architectural assumptions
│
├── Performance
│       │
│       └── Measure system behavior
│
├── Tradeoffs
│       │
│       └── Investigate competing architectural properties
│
└── ADRs
        │
        └── Capture decisions based on evidence
```

The relationship is:

```text
Pattern Comparison
       ↓
Failure Experiment
       ↓
Performance Measurement
       ↓
Trade-off Analysis
       ↓
ADR
```

---

# 25. Architecture as Evidence

The ultimate goal is not to collect benchmarks.

It is to build architectural reasoning skills.

Instead of:

```text
"I know microservices."
```

the experiment should enable:

```text
"I understand what changes when this system becomes
distributed, which failure modes appear, what the
operational costs are, and under which constraints
those costs are justified."
```

Instead of:

```text
"Async is faster."
```

the evidence should show:

```text
"Async changed the system's temporal coupling,
burst-handling behavior, consistency model, and
failure handling. Under the tested workload, these
changes produced measurable effects."
```

That is the level of understanding expected from senior and principal architecture work.

---

# 26. Definition of Done

A PatternComparison experiment is complete when:

- [ ] Architectural question is explicit.
- [ ] Hypothesis is documented.
- [ ] Both architectures implement equivalent business behavior.
- [ ] Baseline is defined.
- [ ] Major variables are controlled.
- [ ] Workload is reproducible.
- [ ] Metrics are defined.
- [ ] Performance is measured.
- [ ] Failure behavior is tested where relevant.
- [ ] Architecture constraints are tested.
- [ ] Results are recorded.
- [ ] Unexpected behavior is documented.
- [ ] Trade-offs are documented.
- [ ] No universal ranking is inferred from the experiment.
- [ ] Architectural implications are documented.
- [ ] Relevant ADR is created or updated.

---

# 27. Key Takeaways

- Pattern comparison is an experiment, not a technology popularity contest.
- Compare architectures against the same business problem whenever practical.
- Establish a baseline before changing the architecture.
- Change one major architectural variable at a time.
- Measure both benefits and costs.
- Include failure behavior, not only happy-path performance.
- Compare latency, throughput, coupling, consistency, failure isolation, cost, and operational burden.
- Use architecture tests to verify structural assumptions.
- Use benchmarks to measure runtime behavior.
- Use failure injection to expose hidden architectural weaknesses.
- Record unexpected results instead of hiding them.
- Avoid universal conclusions from context-specific experiments.
- Convert important findings into ADRs.
- The objective is not to find a universal "best architecture."
- The objective is to understand the conditions, constraints, and trade-offs under which an architecture behaves as expected.

> **Architecture experiments turn architectural opinions into testable hypotheses, measurable behavior, and evidence-based decisions.**
