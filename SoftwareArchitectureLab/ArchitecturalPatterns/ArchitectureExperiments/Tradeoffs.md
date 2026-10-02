# Trade-offs

## 1. Essence

`Trade-offs` is the architectural decision laboratory for understanding what is gained and what is sacrificed when choosing between competing architectural properties.

Architecture rarely provides an absolute improvement.

Most architectural decisions improve one dimension while increasing cost, complexity, coupling, operational burden, latency, risk, or loss of control somewhere else.

The central idea is:

```text
Architectural Choice
        ↓
Benefit
        +
Cost
        +
Constraint
        ↓
Trade-off
        ↓
Context
        ↓
Decision
```

The goal is not to find a universally "best" architecture.

The goal is to understand:

- which properties improve
- which properties become more expensive
- which constraints drive the decision
- which risks are introduced
- which risks are reduced
- how the trade-off can be measured
- when the trade-off is justified
- when the additional complexity is unnecessary

---

# 2. Purpose

This laboratory focuses on six fundamental architectural trade-offs:

```text
01-ConsistencyVsAvailability
02-CouplingVsIndependence
03-SimplicityVsFlexibility
04-LatencyVsThroughput
05-ReliabilityVsCost
06-AutonomyVsControl
```

These trade-offs appear repeatedly across:

- distributed systems
- microservices
- event-driven architecture
- cloud architecture
- data architecture
- AI-native systems
- platform engineering
- organizational architecture

They are not independent.

For example:

```text
Independence
    ↓
More Distribution
    ↓
More Network Communication
    ↓
Higher Operational Complexity
    ↓
More Failure Modes
```

And:

```text
Higher Reliability
    ↓
More Redundancy
    ↓
More Infrastructure
    ↓
Higher Cost
```

The purpose of this laboratory is to make these relationships explicit and measurable.

---

# 3. Trade-off Philosophy

A trade-off should not be described as:

> "Architecture A is better."

Instead ask:

```text
What problem are we solving?
What property are we optimizing?
What property are we sacrificing?
How much does it cost?
What constraints matter?
What failure modes appear?
Can the trade-off be measured?
Is the added complexity justified?
```

A useful architectural statement is:

```text
Under these constraints,
we intentionally sacrifice X
to improve Y.
```

For example:

```text
We accept eventual consistency
to reduce temporal coupling
and improve availability during
downstream failures.
```

Or:

```text
We accept higher operational complexity
to gain independent deployment and scaling.
```

The decision is contextual.

---

# 4. Common Experimental Model

Use a common domain where possible.

Recommended:

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

Alternative architectures can then be introduced while keeping:

- business behavior
- workload
- dataset
- hardware
- functional requirements

as constant as possible.

---

# 5. Trade-off Experiment Model

Every trade-off experiment should follow:

```text
Architectural Question
        ↓
Option A
        ↓
Option B
        ↓
Common Workload
        ↓
Measure Both
        ↓
Identify Benefits
        ↓
Identify Costs
        ↓
Identify Constraints
        ↓
Failure Experiment
        ↓
Trade-off Analysis
        ↓
Decision
```

The output should not be a winner.

The output should be an architectural understanding.

---

# 6. Common Trade-off Dimensions

Evaluate architectural choices across:

```text
Correctness
Consistency
Availability
Latency
Throughput
Scalability
Coupling
Independence
Complexity
Flexibility
Reliability
Cost
Operational Burden
Security
Observability
Team Autonomy
Governance
Changeability
Failure Isolation
```

A decision should consider more than one dimension.

---

# 7. Trade-off 01 - Consistency vs Availability

## 7.1 Essence

Consistency and availability represent competing priorities in distributed systems.

A system may prioritize:

```text
Immediate consistency
```

or:

```text
Continued availability with eventual convergence
```

The architectural choice affects:

- user experience
- data correctness
- failure behavior
- latency
- coordination
- availability
- operational complexity

---

## 7.2 Strong Consistency

A strong consistency model attempts to ensure that once a successful write occurs, subsequent reads observe the appropriate latest state according to the chosen consistency guarantees.

Typical architecture:

```text
Service A
   ↓
Database
   ↓
Service B
```

or coordinated distributed writes.

Advantages may include:

- simpler business reasoning
- immediate visibility
- fewer reconciliation workflows

Costs may include:

- coordination
- higher latency
- reduced availability during some failures
- distributed transaction complexity

---

## 7.3 Eventual Consistency

Typical architecture:

```text
Service A
   ↓
Database
   ↓
Event
   ↓
Service B
   ↓
Local Projection
```

The system accepts temporary divergence.

Advantages may include:

- lower temporal coupling
- better failure isolation
- higher availability
- independent processing

Costs include:

- stale reads
- reconciliation
- duplicate handling
- ordering concerns
- more complex business workflows

---

## 7.4 Experiment

Compare:

```text
Option A
Synchronous Consistency
```

with:

```text
Option B
Asynchronous Eventual Consistency
```

Measure:

```text
Write Latency
Read Latency
Consistency Delay
Failure Behavior
Recovery Time
Throughput
Availability
Reconciliation Effort
```

---

## 7.5 Failure Scenario

Stop the downstream service.

Observe:

### Strongly coordinated model

```text
Write
 ↓
Dependency unavailable
 ↓
Operation fails
```

### Eventually consistent model

```text
Write
 ↓
Event
 ↓
Dependency unavailable
 ↓
Event remains pending
 ↓
Processing resumes later
```

---

## 7.6 Key Questions

> How stale can data be?

> Which business operations require immediate consistency?

> Can the business tolerate reconciliation?

> What happens when the downstream component is unavailable?

---

# 8. Trade-off 02 - Coupling vs Independence

## 8.1 Essence

Architectural independence allows components, teams, or services to evolve separately.

But independence usually requires additional boundaries.

```text
More Independence
        ↓
More Explicit Contracts
        ↓
More Coordination Mechanisms
        ↓
More Architectural Complexity
```

---

## 8.2 High Coupling

Example:

```text
Orders
  ↓
Shared Domain
  ↓
Payments
  ↓
Shared Database
```

Benefits may include:

- simpler communication
- easy transactions
- fewer contracts
- simpler local development

Costs include:

- coordinated changes
- deployment coupling
- failure propagation
- reduced team autonomy

---

## 8.3 High Independence

Example:

```text
Orders → Orders DB

Payments → Payments DB

Customers → Customers DB
```

Communication occurs through:

```text
API
Events
Messages
```

Benefits may include:

- independent deployment
- independent scaling
- clearer ownership
- stronger failure boundaries

Costs include:

- network calls
- distributed consistency
- contract management
- observability complexity
- operational burden

---

## 8.4 Experiment

Compare:

```text
Modular Monolith
```

with:

```text
Microservices
```

Measure:

```text
Deployment Independence
Change Coupling
Latency
Failure Propagation
Operational Components
Data Coupling
Team Coordination
Scaling Independence
```

---

## 8.5 Failure Scenario

Change or stop Payments.

Measure:

```text
Orders Impact
Customers Impact
Deployment Impact
Recovery
```

---

## 8.6 Key Questions

> Which components actually need independent evolution?

> Is the independence worth the distributed-system complexity?

> Are we creating independence or merely distribution?

---

# 9. Trade-off 03 - Simplicity vs Flexibility

## 9.1 Essence

Simple architectures reduce cognitive and operational complexity.

Flexible architectures provide more options for future change.

These goals can conflict.

```text
More Flexibility
      ↓
More Abstractions
      ↓
More Configuration
      ↓
More Possible States
      ↓
More Complexity
```

---

## 9.2 Simplicity

Example:

```text
ASP.NET Core
    ↓
Application
    ↓
PostgreSQL
```

Potential benefits:

- easy to understand
- easy to operate
- fewer failure modes
- low infrastructure overhead
- fast development

Potential limitations:

- fewer independent scaling options
- tighter deployment boundaries
- technology choices may be more centralized

---

## 9.3 Flexibility

Example:

```text
API
 ↓
Abstraction
 ↓
Provider A / Provider B / Provider C
```

Or:

```text
Service
 ↓
Message Broker
 ↓
Multiple Consumers
```

Potential benefits:

- replaceable implementations
- extensibility
- multiple deployment strategies
- technology choice

Potential costs:

- more abstractions
- more configuration
- more testing
- more operational states
- more failure modes

---

## 9.4 Experiment

Build two implementations.

### Simple

```text
Direct Dependency
```

### Flexible

```text
Abstraction
    ↓
Multiple Implementations
```

Measure:

```text
Development Complexity
Lines of Configuration
Test Complexity
Runtime Overhead
Change Effort
Failure Modes
Operational Complexity
```

---

## 9.5 Important Question

> Are we paying for flexibility that the system actually needs?

---

## 9.6 Architectural Principle

Flexibility should be intentional.

Do not introduce:

- abstraction
- plugin architecture
- service mesh
- event bus
- multiple databases
- multiple providers

simply because they might become useful later.

Future flexibility has a cost today.

---

# 10. Trade-off 04 - Latency vs Throughput

## 10.1 Essence

Latency and throughput are related but different performance properties.

```text
Latency
=
How long one operation takes
```

```text
Throughput
=
How much work the system processes over time
```

Optimizing one can sometimes reduce the other.

---

## 10.2 Low Latency

Typical strategies:

- caching
- precomputation
- smaller payloads
- synchronous processing
- local data
- fewer network calls
- optimized queries

Potential cost:

- duplicated data
- more memory
- reduced batching
- more infrastructure

---

## 10.3 High Throughput

Typical strategies:

- batching
- asynchronous processing
- buffering
- parallelism
- partitioning
- queue-based load leveling

Potential cost:

- increased latency
- eventual consistency
- more complex recovery

---

## 10.4 Example

Individual processing:

```text
Request
 ↓
Process
 ↓
Commit
```

Batch processing:

```text
Requests
 ↓
Batch
 ↓
Process Together
 ↓
Commit
```

Batching may increase throughput while increasing individual request latency.

---

## 10.5 Experiment

Compare:

```text
Batch Size = 1
Batch Size = 10
Batch Size = 100
Batch Size = 1000
```

Measure:

```text
P50
P95
P99
Throughput
CPU
Memory
Database Operations
Failure Recovery
```

---

## 10.6 Queueing Effect

As utilization approaches system capacity:

```text
Load
 ↓
Resource Utilization ↑
 ↓
Queueing ↑
 ↓
Latency ↑
```

Therefore:

> Maximum throughput is not necessarily the same as optimal operating point.

---

## 10.7 Key Questions

> Is the business optimizing response time or total processing capacity?

> Can the user tolerate deferred processing?

> Can batching improve throughput without violating latency requirements?

---

# 11. Trade-off 05 - Reliability vs Cost

## 11.1 Essence

Reliability usually requires redundancy, isolation, monitoring, testing, recovery mechanisms, and operational capacity.

These increase cost.

```text
More Reliability
      ↓
More Redundancy
      ↓
More Infrastructure
      ↓
More Operational Complexity
      ↓
Higher Cost
```

The objective is not maximum reliability at any price.

The objective is appropriate reliability for the business requirement.

---

## 11.2 Reliability Mechanisms

Examples:

```text
Redundancy
Replication
Failover
Backups
Multi-AZ
Multi-Region
Circuit Breakers
Queues
Retries
Disaster Recovery
Chaos Testing
Monitoring
```

Each mechanism introduces cost.

---

## 11.3 Reliability Levels

Example:

```text
Single Instance
      ↓
Multiple Instances
      ↓
Multi-AZ
      ↓
Multi-Region
```

Availability and recovery characteristics may improve, while infrastructure and operational complexity increase.

---

## 11.4 Experiment

Compare:

```text
Architecture A
Single Region
```

with:

```text
Architecture B
Multi-AZ
```

and potentially:

```text
Architecture C
Multi-Region
```

Measure:

```text
Availability
Recovery Time
Data Loss
Infrastructure Cost
Operational Complexity
Failure Isolation
```

---

## 11.5 Failure Scenario

Introduce:

```text
Instance Failure
Database Failure
Availability Zone Failure
Region Failure
```

Measure:

```text
Detection
Recovery
Data Loss
User Impact
Cost
```

---

## 11.6 Reliability Budget

Define explicit requirements:

```text
Availability Target
Recovery Time Objective
Recovery Point Objective
Maximum Acceptable Data Loss
Maximum Downtime
```

Then design the architecture accordingly.

---

## 11.7 Key Questions

> How much reliability does the business actually require?

> What is the cost of one hour of downtime?

> What is the cost of the reliability mechanism?

> Are we paying for reliability beyond the business requirement?

---

# 12. Trade-off 06 - Autonomy vs Control

## 12.1 Essence

Autonomy allows components, teams, services, or agents to make decisions independently.

Control ensures that decisions remain within defined boundaries.

This trade-off is especially important in:

- microservices
- platform engineering
- event-driven systems
- distributed teams
- AI-native systems
- agentic systems

---

## 12.2 High Autonomy

Example:

```text
Team / Service
      ↓
Own Code
Own Data
Own Deployment
Own Scaling
Own Technology
```

Benefits may include:

- faster local decisions
- independent evolution
- reduced coordination
- ownership

Costs may include:

- inconsistency
- duplicated infrastructure
- governance challenges
- security variation
- operational fragmentation

---

## 12.3 High Control

Example:

```text
Central Platform
      ↓
Standard Infrastructure
      ↓
Standard Deployment
      ↓
Standard Security
      ↓
Standard Observability
```

Benefits may include:

- consistency
- governance
- security
- standardization
- operational visibility

Costs may include:

- slower decisions
- centralized bottlenecks
- reduced flexibility
- platform dependency

---

## 12.4 AI-Native Example

The trade-off becomes particularly important with agents.

High autonomy:

```text
Agent
  ↓
Reason
  ↓
Choose Tool
  ↓
Execute
  ↓
Observe
  ↓
Continue
```

High control:

```text
Agent
  ↓
Policy
  ↓
Permission
  ↓
Sandbox
  ↓
Verification
  ↓
Human Gate
  ↓
Execute
```

The goal is not necessarily maximum autonomy.

The important question is:

> Where should autonomy stop and control begin?

---

## 12.5 Experiment

Compare:

```text
Unrestricted Component
```

with:

```text
Bounded Component
```

Measure:

```text
Decision Speed
Operational Complexity
Failure Rate
Policy Violations
Recovery
Human Intervention
Change Velocity
```

For AI systems additionally measure:

```text
Tool Calls
Agent Steps
Cost
Policy Decisions
Verification Failures
Human Approvals
```

---

## 12.6 Key Questions

> Which decisions require local autonomy?

> Which decisions require centralized governance?

> What happens when autonomous behavior is wrong?

> Where should policy enforcement live?

---

# 13. Cross-Trade-off Relationships

The six trade-offs interact continuously.

---

## 13.1 Consistency vs Availability → Coupling vs Independence

Strong consistency often requires coordination.

```text
Strong Consistency
       ↓
Coordination
       ↓
Coupling
```

Eventual consistency can reduce coordination:

```text
Eventual Consistency
       ↓
Asynchronous Communication
       ↓
Independence
```

But introduces:

```text
Staleness
Reconciliation
Complexity
```

---

## 13.2 Coupling vs Independence → Simplicity vs Flexibility

More independent services:

```text
Independence
   ↓
More Contracts
   ↓
More Infrastructure
   ↓
More Flexibility
   ↓
More Complexity
```

---

## 13.3 Latency vs Throughput → Reliability vs Cost

Reducing latency may require:

- more instances
- caching
- dedicated resources
- lower batching
- more infrastructure

which increases cost.

---

## 13.4 Reliability vs Cost → Autonomy vs Control

More reliability controls may require centralized infrastructure and governance.

```text
Reliability
    ↓
Standardization
    ↓
Central Controls
    ↓
Less Local Autonomy
```

---

## 13.5 Autonomy vs Control → Simplicity vs Flexibility

Autonomous teams may choose different:

- databases
- frameworks
- deployment models
- observability systems
- messaging technologies

This increases flexibility but can increase organizational and operational complexity.

---

# 14. Trade-off Matrix

| Trade-off                   | Option A             | Option B             | Main Benefit                       | Main Cost                         |
| --------------------------- | -------------------- | -------------------- | ---------------------------------- | --------------------------------- |
| Consistency vs Availability | Strong consistency   | Eventual consistency | Correctness / immediate visibility | Coordination / staleness          |
| Coupling vs Independence    | Shared boundaries    | Explicit boundaries  | Simplicity / coordination          | Distributed complexity            |
| Simplicity vs Flexibility   | Fewer abstractions   | More abstractions    | Lower complexity                   | Less extensibility                |
| Latency vs Throughput       | Immediate processing | Batching / async     | Fast response                      | Lower processing efficiency       |
| Reliability vs Cost         | Minimal redundancy   | High redundancy      | Lower failure impact               | Higher infrastructure cost        |
| Autonomy vs Control         | Local decisions      | Central governance   | Independence                       | Inconsistency / governance burden |

This matrix is not a ranking.

It identifies the architectural property being exchanged.

---

# 15. Measuring Trade-offs

A trade-off becomes useful when it can be measured.

For each option record:

```text
Performance
Reliability
Complexity
Cost
Coupling
Operational Burden
Changeability
Failure Behavior
Security
Governance
```

Example:

```text
Option A
Latency:       50 ms
Throughput:    1000 req/s
Cost:          €X
Availability:  Y
Complexity:    Low

Option B
Latency:       80 ms
Throughput:    5000 req/s
Cost:          €Z
Availability:  Y + Δ
Complexity:    High
```

The objective is not to select a universal winner.

The objective is to understand the consequences.

---

# 16. Qualitative vs Quantitative Trade-offs

Not every architectural property can be measured with one number.

## Quantitative

Examples:

```text
Latency
Throughput
Cost
Availability
Recovery Time
CPU
Memory
Network
```

## Qualitative

Examples:

```text
Team Autonomy
Cognitive Complexity
Governance
Maintainability
Flexibility
Organizational Coupling
```

Qualitative dimensions should still be made explicit.

---

# 17. Decision Context

A trade-off only makes sense in context.

Record:

```text
Business Requirements
Technical Constraints
Team Structure
Operational Capability
Security Requirements
Compliance Requirements
Traffic Profile
Data Characteristics
Failure Tolerance
Budget
Expected Growth
```

The same architectural choice can produce different conclusions under different constraints.

---

# 18. Trade-off Experiment Format

Every trade-off experiment should use:

```text
## Question

What architectural trade-off are we investigating?

## Context

What constraints matter?

## Option A

Describe the first architecture.

## Option B

Describe the second architecture.

## Hypothesis

What do we expect?

## Workload

What workload will be used?

## Metrics

What will be measured?

## Failure Scenario

What happens when something fails?

## Results

What actually happened?

## Benefits

What improved?

## Costs

What became more expensive?

## Risks

What new risks appeared?

## Constraints

Under what conditions is each option appropriate?

## Unexpected Behavior

What surprised us?

## Architectural Implication

What should an architect learn?

## Decision

What decision is appropriate for this specific context?
```

---

# 19. Failure Experiments

Trade-offs should also be tested under failure.

Examples:

### Consistency vs Availability

Stop a downstream service.

Measure:

```text
Consistency
Availability
Recovery
```

### Coupling vs Independence

Stop one service.

Measure:

```text
Blast Radius
```

### Simplicity vs Flexibility

Replace an implementation.

Measure:

```text
Change Effort
Regression Risk
```

### Latency vs Throughput

Introduce increasing load.

Measure:

```text
Latency
Throughput
Queueing
```

### Reliability vs Cost

Remove redundancy.

Measure:

```text
Failure Impact
Recovery
Cost
```

### Autonomy vs Control

Allow local decisions to bypass centralized policy.

Measure:

```text
Decision Speed
Policy Violations
Operational Risk
```

---

# 20. Common Architectural Traps

## 20.1 Optimizing One Dimension

Example:

```text
Maximum Throughput
```

while ignoring:

```text
Latency
Cost
Complexity
Reliability
```

---

## 20.2 Solving Hypothetical Problems

Adding flexibility for a future requirement that may never happen.

---

## 20.3 Paying Complexity Too Early

Introducing:

- microservices
- event streaming
- service mesh
- multiple databases
- multi-region deployment

before the business requires them.

---

## 20.4 Ignoring Organizational Constraints

Architecture is implemented by teams.

A technically elegant architecture can become operationally expensive if the organization cannot support it.

---

## 20.5 Treating Trade-offs as Permanent

Some trade-offs can evolve.

For example:

```text
Simple Monolith
      ↓
Modular Monolith
      ↓
Selective Distribution
```

The architecture can change as constraints change.

---

# 21. Trade-off Evolution

Architectural trade-offs change over time.

Example:

```text
Early Product
      ↓
Low Traffic
      ↓
Optimize Simplicity
```

Later:

```text
Growing Product
      ↓
Independent Scaling Required
      ↓
Optimize Independence
```

Later:

```text
Global Platform
      ↓
Reliability Requirements Increase
      ↓
Optimize Reliability
```

Architecture should therefore be evaluated against the current constraint set.

---

# 22. Architecture Fitness Functions

Trade-offs can become explicit constraints.

Examples:

```text
P95 latency < defined budget
```

```text
Availability > defined target
```

```text
Recovery Time < defined RTO
```

```text
Service must be independently deployable
```

```text
Critical data must satisfy consistency requirement
```

```text
Infrastructure cost must remain within budget
```

```text
Agent actions must remain within permission boundaries
```

These constraints make architectural trade-offs executable.

---

# 23. ADR Integration

Every important trade-off should eventually produce an ADR.

Example:

```text
ADR-0012: Use Eventual Consistency for Order Projections
```

The ADR should record:

```text
Context
Decision
Trade-off
Alternatives
Consequences
Risks
Measurements
Revisit Conditions
```

A useful decision statement is:

```text
We accept X
because Y
under constraints Z.
```

---

# 24. Recommended Experiment Sequence

The experiments should build progressively.

## Phase 1 - Consistency

```text
ConsistencyVsAvailability
```

Understand distributed state and availability.

---

## Phase 2 - Boundaries

```text
CouplingVsIndependence
```

Understand architectural boundaries.

---

## Phase 3 - Complexity

```text
SimplicityVsFlexibility
```

Understand the cost of abstraction and extensibility.

---

## Phase 4 - Performance

```text
LatencyVsThroughput
```

Understand workload and capacity trade-offs.

---

## Phase 5 - Economics

```text
ReliabilityVsCost
```

Understand the economic cost of resilience.

---

## Phase 6 - Governance

```text
AutonomyVsControl
```

Understand where independence should end and policy should begin.

---

# 25. Suggested Folder Structure

```text
Tradeoffs/
│
├── README.md
│
├── ConsistencyVsAvailability/
│   ├── README.md
│   ├── src/
│   ├── tests/
│   ├── benchmarks/
│   └── results/
│
├── CouplingVsIndependence/
│   ├── README.md
│   ├── src/
│   ├── tests/
│   ├── benchmarks/
│   └── results/
│
├── SimplicityVsFlexibility/
│   ├── README.md
│   ├── src/
│   ├── tests/
│   ├── benchmarks/
│   └── results/
│
├── LatencyVsThroughput/
│   ├── README.md
│   ├── src/
│   ├── tests/
│   ├── benchmarks/
│   └── results/
│
├── ReliabilityVsCost/
│   ├── README.md
│   ├── src/
│   ├── tests/
│   ├── benchmarks/
│   └── results/
│
└── AutonomyVsControl/
    ├── README.md
    ├── src/
    ├── tests/
    ├── benchmarks/
    └── results/
```

---

# 26. Relationship With Other ArchitectureExperiments

The complete architecture laboratory now follows:

```text
Pattern Comparison
        ↓
Failure Modes
        ↓
Performance
        ↓
Trade-offs
        ↓
ADRs
```

The relationship is important.

### Pattern Comparison

Answers:

> How do architectural approaches behave differently?

### Failure Modes

Answers:

> What happens when things go wrong?

### Performance

Answers:

> How does the architecture behave under workload?

### Trade-offs

Answers:

> What do we gain and what do we give up?

### ADRs

Answers:

> Given the evidence and constraints, what decision are we making?

---

# 27. Architecture as Evidence

The objective is to move from:

```text
Opinion
```

to:

```text
Hypothesis
   ↓
Experiment
   ↓
Measurement
   ↓
Failure Testing
   ↓
Trade-off Analysis
   ↓
Decision
```

Instead of:

> "Microservices are more scalable."

We ask:

```text
Which workload?
Which scaling dimension?
Which component?
What is shared?
What is the bottleneck?
What is the cost?
What happens under failure?
```

Instead of:

> "Eventual consistency is better."

We ask:

```text
What consistency does the business require?
How much staleness is acceptable?
What happens during failure?
How is reconciliation performed?
What latency and availability benefit is obtained?
```

Instead of:

> "More autonomy is better."

We ask:

```text
Which decisions need autonomy?
Which decisions require governance?
What is the blast radius of an incorrect decision?
Where should policy enforcement happen?
```

This is architectural reasoning.

---

# 28. Definition of Done

A trade-off experiment is complete when it has:

- [ ] Explicit architectural question
- [ ] Context and constraints
- [ ] At least two meaningful alternatives
- [ ] Clear hypothesis
- [ ] Common workload
- [ ] Defined metrics
- [ ] Baseline
- [ ] Failure scenario
- [ ] Quantitative measurements where possible
- [ ] Qualitative consequences documented
- [ ] Benefits documented
- [ ] Costs documented
- [ ] Risks documented
- [ ] Operational implications documented
- [ ] Unexpected behavior documented
- [ ] Reproducible experiment
- [ ] Architectural implications documented
- [ ] Decision or revisit condition documented
- [ ] ADR created when appropriate

---

# 29. Key Takeaways

Architecture is fundamentally about trade-offs.

There is rarely a free improvement.

```text
Consistency
      ↔
Availability

Coupling
      ↔
Independence

Simplicity
      ↔
Flexibility

Latency
      ↔
Throughput

Reliability
      ↔
Cost

Autonomy
      ↔
Control
```

The important question is not:

> "Which side is better?"

The important questions are:

```text
What does the business need?
What constraint are we optimizing?
What are we willing to sacrifice?
What new risks are introduced?
What will it cost?
How can we measure the consequences?
When should the decision be revisited?
```

A strong architect does not eliminate trade-offs.

A strong architect makes them **explicit, measurable, intentional, and reversible where possible**.

The final decision should therefore look like:

```text
Context
   ↓
Constraints
   ↓
Alternatives
   ↓
Evidence
   ↓
Trade-offs
   ↓
Decision
   ↓
Consequences
   ↓
Revisit Conditions
```

> **Architecture is not the art of finding the perfect solution. It is the discipline of making explicit choices about which properties matter most under real constraints.**
