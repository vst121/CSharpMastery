# Quality Attributes

## 1. Essence

Quality attributes describe **how well a system must behave**, rather than what business functionality it provides.

Functional requirement:

> The system must process a payment.

Quality attribute:

> The system must process 1,000 payment requests per second with P99 latency below 300 ms while maintaining idempotency and remaining available during the loss of one application instance.

Architecture exists largely to satisfy these quality attributes under defined constraints.

A useful model is:

```text
Business Requirements
        ↓
Quality Attributes
        ↓
Quality Attribute Scenarios
        ↓
Architectural Constraints
        ↓
Architecture
        ↓
Implementation
        ↓
Measurement
        ↓
Fitness Functions
```

The central architectural principle is:

> **Quality attributes are not adjectives. They are measurable system properties under defined conditions.**

Words such as:

- scalable
- secure
- reliable
- fast
- maintainable
- highly available
- resilient
- observable

are incomplete architectural requirements until their meaning and measurement are defined.

---

# 2. Why Quality Attributes Matter

Two architectures can implement exactly the same business functionality while having dramatically different operational characteristics.

For example:

```text
Architecture A
    ↓
Simple deployment
Low operational complexity
Strong consistency
Limited independent scaling
Single failure domain
```

versus:

```text
Architecture B
    ↓
Independent scaling
Independent deployment
Failure isolation
Eventual consistency
Higher operational complexity
Distributed failure modes
```

Neither architecture is universally better.

The appropriate architecture depends on the required quality attributes, constraints, and business context.

This is why architecture should start with:

```text
What must the system be able to do?
        +
How well must it do it?
        +
Under what conditions?
```

---

# 3. Functional Requirements vs Quality Attributes

Functional requirements describe **capabilities**.

Quality attributes describe **properties of those capabilities**.

Example:

```text
Functional:
Process transactions.

Quality:
- Throughput: 1,000 transactions/sec
- P99 latency: < 300 ms
- Availability: 99.95%
- Duplicate processing: zero
- Recovery: RTO < 5 minutes
- Data loss: RPO < 1 minute
- Auditability: every transaction traceable
```

A useful distinction:

```text
Functional Requirement
    ↓
What does the system do?

Quality Attribute
    ↓
How well does it do it?

Constraint
    ↓
What limits the solution space?

Architecture
    ↓
How do we satisfy the requirements?
```

---

# 4. Quality Attribute Categories

Quality attributes can be grouped into several major areas.

```text
Quality Attributes
│
├── Performance
│   ├── Latency
│   ├── Throughput
│   ├── Resource efficiency
│   └── Scalability
│
├── Reliability
│   ├── Fault tolerance
│   ├── Recovery
│   ├── Data durability
│   └── Correctness
│
├── Availability
│   ├── Uptime
│   ├── Redundancy
│   └── Failure recovery
│
├── Resilience
│   ├── Failure isolation
│   ├── Graceful degradation
│   ├── Backpressure
│   └── Recovery
│
├── Security
│   ├── Confidentiality
│   ├── Integrity
│   ├── Authentication
│   ├── Authorization
│   └── Auditability
│
├── Maintainability
│   ├── Modifiability
│   ├── Testability
│   ├── Diagnosability
│   └── Understandability
│
├── Deployability
│   ├── Deployment frequency
│   ├── Deployment safety
│   ├── Rollback
│   └── Independent deployment
│
├── Operability
│   ├── Observability
│   ├── Automation
│   ├── Alerting
│   └── Recovery
│
├── Scalability
│   ├── Horizontal scaling
│   ├── Vertical scaling
│   ├── Data scaling
│   └── Organizational scaling
│
└── Evolvability
    ├── Change isolation
    ├── Extensibility
    ├── Technology replacement
    └── Architectural adaptability
```

These categories overlap.

For example:

```text
Scalability
    ↓
Performance
    ↓
Resource utilization
    ↓
Cost
    ↓
Operational complexity
```

Architecture decisions often affect several quality attributes simultaneously.

---

# 5. Quality Attribute Scenarios

A quality attribute becomes architecturally useful when expressed as a **scenario**.

A practical structure is:

```text
Source
    ↓
Stimulus
    ↓
Environment
    ↓
Artifact
    ↓
Response
    ↓
Response Measure
```

Example:

```text
Source:
External client

Stimulus:
1,500 transaction requests/sec

Environment:
Normal production operation

Artifact:
Transaction API

Response:
System accepts and processes requests

Response Measure:
- P99 latency < 300 ms
- throughput ≥ 1,000 txn/sec
- error rate < 0.1%
```

Another example:

```text
Source:
Database infrastructure

Stimulus:
Primary database becomes unavailable

Environment:
Production

Artifact:
Transaction processing system

Response:
Traffic is redirected to the healthy database replica

Response Measure:
- RTO < 60 seconds
- no committed transaction lost
- no duplicate payment
```

This is much more useful than:

> The system must be highly available.

---

# 6. Quality Attribute Requirements Must Be Measurable

Avoid:

```text
The system must be fast.
```

Prefer:

```text
P95 API latency < 200 ms
P99 API latency < 500 ms
```

Avoid:

```text
The system must scale.
```

Prefer:

```text
System must support 10x current traffic
without architectural redesign.
```

Avoid:

```text
The system must be reliable.
```

Prefer:

```text
99.95% monthly availability
with RTO < 5 minutes
and RPO < 1 minute.
```

Avoid:

```text
The system must be secure.
```

Prefer:

```text
All administrative operations require
authenticated users with explicit authorization.
All sensitive actions must be auditable.
```

The transformation is:

```text
Adjective
   ↓
Definition
   ↓
Scenario
   ↓
Metric
   ↓
Threshold
   ↓
Test
```

---

# 7. Performance

Performance describes how efficiently the system responds to workload.

Important dimensions include:

```text
Latency
Throughput
Concurrency
Resource utilization
Response time distribution
```

Do not reduce performance to average latency.

Use distributions:

```text
P50
P90
P95
P99
P99.9
Maximum
```

Example:

```text
P50 = 40 ms
P95 = 120 ms
P99 = 280 ms
P99.9 = 900 ms
```

The P99 value may matter more operationally than the average.

Performance should always be evaluated under a defined workload.

---

# 8. Latency

Latency is the time required to complete an operation.

Architectural sources of latency include:

```text
Client
  ↓
Network
  ↓
API
  ↓
Service
  ↓
Database
  ↓
External Service
```

Every additional boundary can introduce:

- network latency
- serialization
- connection overhead
- retries
- queueing
- contention
- coordination

A distributed architecture can therefore improve independence while increasing latency.

Measure:

```text
P50
P95
P99
P99.9
Timeout rate
```

Also measure latency by dependency.

```text
Total latency
    =
Application
+ Database
+ Network
+ External dependencies
+ Queueing
+ Serialization
```

---

# 9. Throughput

Throughput measures how much work the system can process per unit of time.

Examples:

```text
requests/sec
transactions/sec
messages/sec
events/sec
MB/sec
```

For TransactionFlow:

```text
Target:
1,000 transactions/sec
```

Throughput alone is insufficient.

Always correlate it with:

```text
Latency
CPU
Memory
Database utilization
Queue depth
Error rate
```

A system processing 10,000 requests/sec with unusable latency is not necessarily meeting the business requirement.

---

# 10. Scalability

Scalability describes how system capacity changes as workload increases.

Two common approaches:

```text
Vertical Scaling
      ↓
More CPU
More memory
Faster storage

Horizontal Scaling
      ↓
More instances
More consumers
More partitions
```

Architectural scalability also includes:

```text
Application scalability
Database scalability
Messaging scalability
Organizational scalability
Deployment scalability
```

A system may scale computationally but fail organizationally.

For example:

```text
20 services
+
20 repositories
+
20 deployment pipelines
+
15 teams
+
complex distributed ownership
```

may create organizational bottlenecks even if runtime scaling works.

---

# 11. Availability

Availability describes whether a system is usable when requested.

A simplified model:

```text
Availability =
Uptime / Total Time
```

Common targets:

```text
99%
99.9%
99.95%
99.99%
99.999%
```

Availability should be treated as a business requirement.

For example:

```text
99.9% ≈ 8h 46m downtime/year
99.99% ≈ 52m 36s downtime/year
```

The required level depends on business impact and cost.

Availability architecture may involve:

```text
Redundancy
Load balancing
Failover
Replication
Health checks
Graceful degradation
Disaster recovery
```

---

# 12. Reliability

Reliability concerns the ability of a system to perform correctly over time.

Reliability is broader than availability.

A system can be:

```text
Available
but incorrect.
```

For example:

```text
Payment API responds successfully
but processes the same payment twice.
```

The system is available but unreliable from a business perspective.

Important reliability properties include:

```text
Correctness
Durability
Consistency
Idempotency
Fault tolerance
Recovery
```

---

# 13. Resilience

Resilience describes how the system behaves when components fail or conditions degrade.

Expected failures include:

```text
Database unavailable
Network timeout
Service unavailable
Message broker unavailable
Slow dependency
Expired credential
Disk failure
CPU saturation
Traffic spike
Malformed message
```

A resilient architecture does not assume:

> Components will always work.

It assumes:

> Components will fail and designs controlled behavior around those failures.

Common mechanisms:

```text
Timeout
Retry
Backoff
Jitter
Circuit breaker
Bulkhead
Rate limiting
Load shedding
Backpressure
Dead Letter Queue
Graceful degradation
Failover
```

---

# 14. Failure Isolation

Failure isolation limits the blast radius of failures.

Example:

```text
Payment Service
      ↓
Notification Service
```

If Notification Service fails:

```text
Bad architecture:

Payment
   ↓
Notification
   ↓
Timeout
   ↓
Retry
   ↓
Payment latency
   ↓
Payment failure
```

Better:

```text
Payment
   ↓
Transaction committed
   ↓
Event
   ↓
Notification

Notification failure
       ↓
DLQ / retry
       ↓
Payment remains operational
```

Failure isolation is therefore an architectural property that should be explicitly tested.

---

# 15. Maintainability

Maintainability describes how easily the system can be understood, modified, tested, and repaired.

Important dimensions:

```text
Understandability
Modifiability
Testability
Diagnosability
Change isolation
```

Architectural factors affecting maintainability include:

```text
Cohesion
Coupling
Dependency direction
Module boundaries
Abstraction quality
Code ownership
Observability
```

A useful relationship:

```text
High Cohesion
+
Low Unnecessary Coupling
        ↓
Smaller Change Surface
        ↓
Better Maintainability
```

---

# 16. Modifiability

Modifiability asks:

> How much of the system must change when a requirement changes?

Example:

```text
Change:
Add a new payment provider.
```

Architecture A:

```text
20 modules modified
8 tests broken
4 deployment units affected
```

Architecture B:

```text
1 adapter added
Existing domain unchanged
```

The second architecture has a smaller change surface for that particular change.

Measure:

```text
Files changed
Modules changed
Services changed
Teams involved
Deployment units affected
Tests affected
Time to implement
```

These are useful architectural measurements.

---

# 17. Testability

Testability describes how easily system behavior can be verified.

Consider:

```text
Pure domain logic
    ↓
Easy unit testing
```

versus:

```text
Domain
 ↓
Database
 ↓
Message broker
 ↓
External API
 ↓
Identity provider
```

Testing becomes increasingly expensive.

Important dimensions:

```text
Unit testability
Integration testability
Contract testability
Failure testability
Performance testability
End-to-end testability
```

Architecture should make important behavior testable without requiring the entire production environment.

---

# 18. Deployability

Deployability describes how safely and independently software can be released.

Important dimensions:

```text
Deployment frequency
Deployment duration
Rollback capability
Deployment independence
Blast radius
Release safety
```

Useful measures:

```text
Time to deploy
Time to rollback
Number of components affected
Percentage of traffic exposed
Failed deployment rate
```

Deployment architecture is therefore part of architecture, not merely DevOps.

---

# 19. Operational Burden

Operational burden measures the complexity required to operate the architecture.

Consider:

```text
Monolith
    ↓
1 application
1 deployment
1 log stream
1 health endpoint
```

versus:

```text
20 services
20 deployments
20 health endpoints
20 dashboards
20 alert sets
distributed tracing
service discovery
network policies
secrets
certificates
multiple databases
message broker
```

The second architecture may provide valuable capabilities, but those capabilities have an operational cost.

Measure:

```text
Number of deployable units
Number of infrastructure components
Number of operational dependencies
Number of alerts
Number of dashboards
On-call complexity
Mean time to diagnose
Mean time to recover
```

---

# 20. Observability

Observability allows operators to understand system behavior from externally visible signals.

Core signals:

```text
Logs
Metrics
Traces
```

Modern distributed systems also require:

```text
CorrelationId
TraceId
SpanId
CausationId
RequestId
MessageId
```

A useful model:

```text
Request
   ↓
Trace
   ├── API span
   ├── Database span
   ├── Service span
   ├── Kafka publish span
   └── Consumer span
```

Observability itself is a quality attribute because without it:

```text
Failure
  ↓
Unknown cause
  ↓
Longer diagnosis
  ↓
Longer recovery
  ↓
Higher operational cost
```

---

# 21. Security

Security quality attributes include:

```text
Confidentiality
Integrity
Authentication
Authorization
Accountability
Auditability
Availability
Non-repudiation
```

Architecture should explicitly consider:

```text
Identity
Access control
Secrets
Encryption
Network boundaries
Data protection
Threat modeling
Supply chain security
Audit
```

Security requirements should also be expressed as scenarios.

Example:

```text
Stimulus:
Unauthorized user attempts to access payment data.

Response:
Request rejected.

Measure:
100% of unauthorized requests denied.
Security event recorded.
```

---

# 22. Data Integrity

Data integrity is particularly important in transactional systems.

For TransactionFlow:

```text
Transaction ID
Amount
Currency
Status
Idempotency Key
```

may have invariants such as:

```text
Transaction ID is unique.
Amount cannot be negative.
A completed transaction cannot return to Pending.
An idempotency key cannot represent two different requests.
```

These are quality constraints because correctness is an architectural property.

Architecture must therefore protect:

```text
Invariants
Consistency
Durability
Concurrency safety
Idempotency
Auditability
```

---

# 23. Auditability

Auditability answers:

> Can we determine what happened, when, by whom, and why?

For a transaction:

```text
Transaction
    ↓
Command
    ↓
Decision
    ↓
State Change
    ↓
Event
    ↓
Consumer Actions
```

An audit trail may need:

```text
Who
What
When
Where
Why
Correlation
Previous state
New state
```

Auditability is especially important for:

```text
Payments
Healthcare
Financial systems
Identity
Security
AI decisions
Administrative operations
```

---

# 24. Evolvability

Evolvability describes the system's ability to adapt to changing requirements and technology.

Questions:

```text
Can we replace the database?

Can we introduce a new API version?

Can we replace the message broker?

Can we add a new payment provider?

Can we introduce AI capabilities?

Can one module be extracted?

Can we change the consistency model?

Can teams evolve independently?
```

Architectural boundaries should make important future changes possible without requiring unnecessary system-wide modification.

---

# 25. Quality Attributes Interact

Quality attributes are not independent.

Improving one can negatively affect another.

Example:

```text
More replication
    ↓
Higher availability
    ↓
Higher infrastructure cost
    ↓
More operational complexity
```

Another:

```text
More asynchronous processing
    ↓
Lower temporal coupling
    ↓
Higher resilience
    ↓
Eventual consistency
    ↓
More operational complexity
```

Another:

```text
More abstraction
    ↓
More flexibility
    ↓
More indirection
    ↓
Higher cognitive complexity
```

Therefore:

> **Architecture is often the management of competing quality attributes under constraints.**

---

# 26. Quality Attribute Scenarios and Trade-offs

A useful architecture analysis table is:

| Quality Attribute | Scenario             | Metric                  | Target                     | Architectural Mechanism |
| ----------------- | -------------------- | ----------------------- | -------------------------- | ----------------------- |
| Performance       | 1,000 txn/sec        | Throughput              | ≥ 1,000/sec                | Horizontal scaling      |
| Latency           | Transaction request  | P99                     | < 300 ms                   | Efficient API/data path |
| Availability      | One instance fails   | Availability            | ≥ 99.95%                   | Redundant instances     |
| Reliability       | Duplicate request    | Duplicate processing    | 0                          | Idempotency             |
| Resilience        | Kafka unavailable    | Payment availability    | No interruption            | Transactional outbox    |
| Scalability       | 10x traffic          | Capacity                | ≥ 10x                      | Horizontal scaling      |
| Security          | Unauthorized request | Unauthorized success    | 0                          | Authorization           |
| Testability       | Domain change        | Test isolation          | No infrastructure required | Domain boundaries       |
| Deployability     | Service update       | Deployment blast radius | One service                | Independent deployment  |
| Operability       | Production failure   | MTTD/MTTR               | Defined target             | Observability           |
| Maintainability   | New payment provider | Change surface          | Minimal                    | Port/adapter boundary   |

The important point is that the table connects:

```text
Requirement
    ↓
Measurement
    ↓
Architecture
```

---

# 27. Coupling

Coupling measures dependency between architectural elements.

Important forms include:

```text
Code coupling
Module coupling
Data coupling
Runtime coupling
Temporal coupling
Deployment coupling
Organizational coupling
Technology coupling
```

Example:

```text
Service A
    ↓
Service B
    ↓
Service C
    ↓
Service D
```

This creates runtime coupling.

If A cannot operate when B is unavailable:

```text
A → B
```

is also a failure dependency.

Coupling should therefore be analyzed across multiple dimensions, not simply as:

> Number of dependencies.

---

# 28. Cohesion

Cohesion describes how strongly related the responsibilities inside a component are.

High cohesion:

```text
Payment
├── Payment rules
├── Payment validation
├── Payment state
└── Payment policies
```

Low cohesion:

```text
CommonService
├── Payment
├── Email
├── Customer
├── Reporting
├── Authentication
└── File processing
```

A useful architectural goal:

```text
High Cohesion
+
Controlled Coupling
```

This improves:

```text
Understandability
Change isolation
Testability
Ownership
Evolution
```

---

# 29. Complexity

Complexity exists at several levels:

```text
Code complexity
Domain complexity
Architectural complexity
Distributed complexity
Operational complexity
Organizational complexity
```

A common architectural mistake is reducing code complexity while increasing system complexity.

For example:

```text
Monolith
    ↓
20 microservices
```

may reduce local codebase complexity while increasing:

```text
Network complexity
Deployment complexity
Observability complexity
Data consistency complexity
Operational complexity
```

Architecture should therefore consider **total system complexity**, not only code complexity.

---

# 30. Architecture Fitness Functions

A fitness function continuously verifies an architectural property.

Examples:

```text
Rule:
Modules must not reference infrastructure directly.
```

Automated test:

```text
Architecture test
    ↓
Fail build if dependency rule is violated.
```

Another:

```text
Rule:
No service may access another service's database.
```

Possible validation:

```text
Database permissions
+
Architecture tests
+
Static analysis
```

Another:

```text
Rule:
P99 transaction latency < 300 ms.
```

Validation:

```text
Performance test
    ↓
CI/CD
    ↓
Fitness threshold
```

Fitness functions turn architecture from documentation into continuously validated behavior.

---

# 31. Architecture Invariants

An architectural invariant is a property that must remain true.

Examples:

```text
Domain must not depend on infrastructure.

Modules must not access another module's internal implementation.

Services must not directly access another service's database.

All transaction commands must be idempotent.

All externally visible events must have versioned contracts.

No unbounded retry loop is allowed.

Critical mutations must be auditable.
```

A strong architecture document should identify its invariants explicitly.

---

# 32. Quality Attribute Failure Experiments

Quality attributes should be tested under adverse conditions.

Examples:

### Performance

```text
Increase traffic
    ↓
Observe saturation
```

### Availability

```text
Kill one application instance
    ↓
Measure recovery
```

### Resilience

```text
Disable Kafka
    ↓
Observe transaction processing
```

### Reliability

```text
Send duplicate transaction
    ↓
Verify exactly one business effect
```

### Scalability

```text
1x → 2x → 4x → 8x workload
    ↓
Measure capacity
```

### Security

```text
Attempt unauthorized operation
    ↓
Verify rejection and audit event
```

The experiment should always define:

```text
Hypothesis
Workload
Failure
Metric
Expected behavior
Observed behavior
Architectural implication
```

---

# 33. Quality Attribute Measurement Model

A useful measurement model for architecture experiments is:

```text
                Quality Attribute
                       │
                       ↓
                    Scenario
                       │
                       ↓
                    Workload
                       │
                       ↓
                    Metric
                       │
                       ↓
                    Threshold
                       │
                       ↓
                 Automated Test
                       │
                       ↓
                Fitness Function
```

Example:

```text
Attribute:
Performance

Scenario:
1,000 transactions/sec

Metric:
P99 latency

Threshold:
< 300 ms

Test:
Load test

Fitness Function:
Fail if P99 > 300 ms
```

---

# 34. Common Architectural Smells

Quality attributes are often compromised by recognizable smells.

### Performance Smells

```text
Unbounded queries
N+1 queries
Chatty APIs
Synchronous dependency chains
Large payloads
Uncontrolled retries
```

### Reliability Smells

```text
No idempotency
No durable messaging
No recovery strategy
Hidden shared state
```

### Availability Smells

```text
Single point of failure
No redundancy
No graceful degradation
```

### Maintainability Smells

```text
High coupling
Low cohesion
Circular dependencies
God services
God modules
Shared mutable state
```

### Operability Smells

```text
No tracing
No correlation IDs
No meaningful metrics
No structured logs
No health checks
```

### Security Smells

```text
Shared credentials
Overly broad permissions
Secrets in source control
Implicit trust between services
Missing audit trail
```

---

# 35. Common Mistakes

## Mistake 1: Using vague quality requirements

```text
"Highly scalable"
"Very secure"
"Extremely reliable"
```

Without measurable definitions.

---

## Mistake 2: Optimizing one attribute in isolation

Example:

```text
Maximum throughput
```

while ignoring:

```text
Latency
Cost
Correctness
Operational complexity
```

---

## Mistake 3: Treating technology as the quality attribute

For example:

```text
"We use Kubernetes, therefore the system is scalable."
```

Kubernetes can enable scaling mechanisms, but it does not guarantee an architecture will scale.

---

## Mistake 4: Measuring only the happy path

Real architecture must be evaluated under:

```text
Load
Failure
Partial failure
Dependency degradation
Network problems
Data corruption
Traffic spikes
```

---

## Mistake 5: Confusing availability with reliability

```text
System responds
```

does not mean:

```text
System behaved correctly.
```

---

## Mistake 6: Ignoring operational complexity

An architecture that technically satisfies runtime requirements but cannot be safely operated is incomplete.

---

# 36. Modern .NET Mapping

Quality attributes map directly to modern .NET architecture.

### Performance

```text
ASP.NET Core
async/await
Span<T>
Memory<T>
ArrayPool<T>
System.Threading.Channels
Native AOT where appropriate
BenchmarkDotNet
```

### Resilience

```text
Microsoft.Extensions.Resilience
Timeout
Retry
Circuit Breaker
Rate Limiter
Hedging where justified
```

### Observability

```text
OpenTelemetry
ILogger
Metrics
Distributed tracing
Health checks
```

### Testing

```text
xUnit
NUnit
MSTest
Testcontainers
WireMock
BenchmarkDotNet
Architecture tests
Property-based testing
```

### Architecture Validation

```text
NetArchTest
ArchUnitNET
Custom Roslyn analyzers
CI fitness functions
```

### Distributed Systems

```text
Kafka
RabbitMQ
Azure Service Bus
gRPC
ASP.NET Core
Polly / .NET resilience abstractions
```

### Data

```text
PostgreSQL
SQL Server
EF Core
Dapper
Npgsql
```

The technology is secondary.

The architectural requirement should drive the technology selection.

---

# 37. TransactionFlow Quality Attribute Model

Use `TransactionFlow` as a practical example.

```text
Client
   ↓
ASP.NET Core API
   ↓
Transaction Application Service
   ↓
Domain
   ↓
PostgreSQL
   ↓
Outbox
   ↓
Kafka
   ↓
Consumers
```

Potential requirements:

```text
Throughput:
≥ 1,000 txn/sec

Latency:
P99 < 300 ms

Availability:
≥ 99.95%

Idempotency:
Zero duplicate business effects

Durability:
Committed transactions must survive application failure

Messaging:
No committed transaction may lose its corresponding event

Failure isolation:
Consumer failure must not prevent transaction commitment

Auditability:
Every transaction state change must be traceable

Scalability:
Application and consumers must scale horizontally

Operability:
Distributed transactions must be traceable using OpenTelemetry
```

This turns TransactionFlow into a practical quality-attribute laboratory.

---

# 38. Quality Attribute Experiment Matrix

A useful laboratory matrix:

| Attribute         | Experiment                     | Primary Metrics       |
| ----------------- | ------------------------------ | --------------------- |
| Latency           | API load test                  | P50/P95/P99           |
| Throughput        | Sustained load                 | txn/sec               |
| Scalability       | 1x to 10x workload             | Capacity efficiency   |
| Availability      | Kill instance                  | Recovery time         |
| Reliability       | Duplicate requests             | Duplicate effects     |
| Resilience        | Disable Kafka                  | Business availability |
| Failure isolation | Kill consumer                  | Payment impact        |
| Maintainability   | Add payment provider           | Change surface        |
| Testability       | Domain isolation               | Test execution cost   |
| Deployability     | Independent service deployment | Blast radius          |
| Operability       | Inject failure                 | MTTD/MTTR             |
| Security          | Unauthorized access            | Rejection rate        |
| Auditability      | Trace transaction              | Trace completeness    |

---

# 39. Relationship With ArchitectureExperiments

`ArchitectureFundamentals/01-QualityAttributes` defines the concepts.

`ArchitectureExperiments` provides empirical evidence.

```text
Quality Attribute
       ↓
Definition
       ↓
Scenario
       ↓
Hypothesis
       ↓
Experiment
       ↓
Measurement
       ↓
Evidence
```

For example:

```text
Question:

Does asynchronous transaction processing improve
failure isolation under consumer failure?

Quality Attributes:

Resilience
Failure Isolation
Latency
Operational Complexity

Experiment:

Disable transaction consumer.

Measure:

- Transaction success rate
- Consumer lag
- Queue depth
- End-to-end latency
- Recovery time

Result:

Evidence
```

The result belongs in the experiment, not as an assumption in the fundamentals document.

---

# 40. Relationship With Architecture Evolution

Quality attributes can change during architectural evolution.

Example:

```text
Monolith
    ↓
Modular Monolith
    ↓
Microservices
```

Potential changes:

```text
Deployment independence       ↑
Failure isolation             ↑
Operational complexity        ↑
Network dependency             ↑
Distributed consistency       ↑
Independent scaling            ↑
```

The important architectural question is:

> Which quality attributes are we improving, and which costs are we accepting?

Every major migration should therefore identify:

```text
Current Quality Attributes
        ↓
Target Quality Attributes
        ↓
Migration Constraints
        ↓
Validation
```

---

# 41. Relationship With ADRs

Quality attributes should directly influence architectural decisions.

Example:

```text
ADR:
Use transactional outbox for transaction events.
```

Context:

```text
Event loss is unacceptable.
```

Quality attributes involved:

```text
Reliability
Durability
Consistency
Auditability
```

Decision:

```text
Persist transaction state and event
in the same database transaction.
```

Fitness function:

```text
Every committed transaction must
produce a durable outbox record.
```

This creates a chain:

```text
Quality Attribute
      ↓
Requirement
      ↓
Architecture Decision
      ↓
Implementation
      ↓
Fitness Function
      ↓
Evidence
```

---

# 42. Quality Attribute Decision Framework

When evaluating an architecture, ask:

```text
1. Which quality attributes matter?

2. Why do they matter to the business?

3. How are they defined?

4. What scenario represents the requirement?

5. How will we measure it?

6. What threshold must be achieved?

7. Which architectural mechanisms support it?

8. What trade-offs do those mechanisms introduce?

9. What happens under failure?

10. How will we continuously verify the requirement?
```

This is a strong Senior Architect interview framework.

---

# 43. Architecture Comparison Dimensions

When comparing architectures using the same business problem, evaluate:

```text
Coupling
Cohesion
Complexity
Testability
Deployment Complexity
Runtime Performance
Failure Isolation
Team Scalability
Operational Burden
```

Do not ask:

> Which architecture is better?

Ask:

> Which architecture satisfies the required quality attributes under the given constraints?

This keeps architectural reasoning evidence-driven.

---

# 44. Recommended Quality Attribute Checklist

Before approving an architecture:

```text
Requirements
[ ] Functional requirements identified
[ ] Quality attributes identified
[ ] Business impact understood

Scenarios
[ ] Quality scenarios defined
[ ] Stimulus defined
[ ] Environment defined
[ ] Response defined
[ ] Response measure defined

Performance
[ ] Latency target defined
[ ] Throughput target defined
[ ] Scalability target defined

Reliability
[ ] Failure modes identified
[ ] Recovery defined
[ ] Idempotency considered
[ ] Data durability defined

Availability
[ ] Availability target defined
[ ] Failure domains identified
[ ] Redundancy defined
[ ] RTO defined
[ ] RPO defined

Security
[ ] Authentication defined
[ ] Authorization defined
[ ] Data protection defined
[ ] Auditability defined

Maintainability
[ ] Coupling controlled
[ ] Cohesion defined
[ ] Change boundaries identified
[ ] Testability preserved

Operations
[ ] Logs defined
[ ] Metrics defined
[ ] Tracing defined
[ ] Health checks defined
[ ] Alerting defined
[ ] Recovery procedures defined

Architecture Validation
[ ] Architecture tests exist
[ ] Fitness functions exist
[ ] Performance tests exist
[ ] Failure experiments exist
[ ] Trade-offs documented
[ ] ADRs created
```

---

# 45. Recommended Experiments

Create experiments under:

```text
ArchitectureExperiments/
```

Recommended quality-attribute experiments:

```text
01-LatencyUnderLoad
02-ThroughputSaturation
03-HorizontalScaling
04-DatabaseContention
05-FailureIsolation
06-CascadingFailure
07-ResilienceUnderDependencyFailure
08-DeploymentBlastRadius
09-ArchitectureTestability
10-ObservabilityOverhead
11-RetryAndLatency
12-BackpressureAndQueueGrowth
```

Use `TransactionFlow` as the common business workload wherever practical.

---

# 46. Senior Architect Interview Questions

### Fundamentals

1. What is a quality attribute?
2. How is a quality attribute different from a functional requirement?
3. Why are quality attributes architectural concerns?
4. How do you turn "highly scalable" into an engineering requirement?
5. What is a quality attribute scenario?

### Performance

6. What is the difference between latency and throughput?
7. Why is P99 often more useful than average latency?
8. How would you determine whether a system is actually scalable?
9. What causes performance degradation as concurrency increases?

### Reliability and Resilience

10. What is the difference between reliability, availability, and resilience?
11. How would you isolate failures between services?
12. How do retries create cascading failures?
13. How would you design for partial failure?

### Maintainability

14. How do coupling and cohesion affect architecture?
15. How do you measure architectural complexity?
16. How do you design for change?

### Operations

17. What makes an architecture operable?
18. What observability signals do you require?
19. How would you reduce MTTR?

### Architecture Decision Making

20. How do quality attributes influence architectural pattern selection?
21. How do you resolve conflicting quality attributes?
22. How do you prove that an architecture satisfies its requirements?
23. What is an architecture fitness function?
24. How do you prevent architectural degradation over time?

A strong answer should move from:

```text
Requirement
    ↓
Quality Attribute
    ↓
Scenario
    ↓
Metric
    ↓
Architecture
    ↓
Trade-off
    ↓
Experiment
    ↓
Evidence
```

---

# 47. Definition of Done

`01-QualityAttributes` is complete when you can:

```text
[ ] Explain the difference between functional requirements and quality attributes.

[ ] Convert vague quality requirements into measurable scenarios.

[ ] Define latency, throughput, scalability, availability,
    reliability, resilience, maintainability, and operability.

[ ] Explain coupling and cohesion as architectural properties.

[ ] Identify quality-attribute trade-offs.

[ ] Define RTO and RPO.

[ ] Design failure scenarios for quality attributes.

[ ] Define architecture fitness functions.

[ ] Connect quality attributes to architectural patterns.

[ ] Connect quality attributes to architecture evolution.

[ ] Connect quality attributes to ADRs.

[ ] Design experiments that measure architectural properties.

[ ] Explain how modern .NET supports these properties.

[ ] Defend an architectural decision using measurable quality attributes.
```

---

# 48. Key Takeaways

The most important ideas are:

> **Architecture is largely about satisfying quality attributes under constraints.**

> **Quality attributes should be expressed as scenarios, not adjectives.**

> **Every important quality requirement should have a measurement strategy.**

> **Quality attributes interact and create architectural trade-offs.**

> **Failure behavior is part of the quality attribute, not an afterthought.**

> **Coupling, cohesion, complexity, testability, deployment, performance, failure isolation, team scalability, and operational burden are architectural dimensions that can be measured.**

> **Architecture fitness functions turn quality requirements into continuously validated constraints.**

> **The goal is not to maximize every quality attribute. The goal is to satisfy the attributes that matter to the business while making the trade-offs explicit.**

The core Senior Architect loop is:

```text
Business Need
     ↓
Quality Attribute
     ↓
Quality Attribute Scenario
     ↓
Metric + Threshold
     ↓
Architectural Constraint
     ↓
Architecture
     ↓
Implementation
     ↓
Experiment
     ↓
Evidence
     ↓
Fitness Function
     ↓
Continuous Validation
```
