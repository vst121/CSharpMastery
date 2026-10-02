# Data Architecture

## 1. Essence

Data architecture defines how data is:

- modeled
- owned
- stored
- accessed
- protected
- validated
- replicated
- integrated
- transformed
- distributed
- governed
- retained
- archived
- deleted

A database is only one component of data architecture.

A useful model is:

```text
Business Domain
      ↓
Data Ownership
      ↓
Data Model
      ↓
Storage
      ↓
Access
      ↓
Consistency
      ↓
Integration
      ↓
Replication
      ↓
Governance
      ↓
Lifecycle
```

The central architectural principle is:

> **Data ownership must be explicit, and every important piece of data should have a clearly defined source of truth.**

Data architecture becomes especially important when systems become:

- modular
- distributed
- event-driven
- multi-tenant
- cloud-native
- analytics-heavy
- AI-enabled

---

# 2. Why Data Architecture Matters

Many architectural problems that appear to be service or application problems are actually data problems.

Examples:

```text
Microservices cannot evolve independently
        ↓
Shared database

Services disagree about state
        ↓
Unclear ownership

Reports overload production database
        ↓
Mixed workloads

Events contain inconsistent information
        ↓
No data contract

AI produces incorrect answers
        ↓
Poor data quality / provenance
```

The architecture must therefore answer:

```text
Who owns the data?

Where is the source of truth?

Who can modify it?

Who can read it?

How consistent must it be?

How does it move?

How long must it exist?

How is it protected?

How is it recovered?

How is it deleted?
```

---

# 3. Data Architecture Concerns

A complete data architecture includes:

```text
Data Architecture
│
├── Data Modeling
│   ├── Domain Model
│   ├── Relational Model
│   ├── Document Model
│   ├── Graph Model
│   └── Event Model
│
├── Data Ownership
│   ├── Source of Truth
│   ├── Write Ownership
│   ├── Read Ownership
│   └── Data Boundaries
│
├── Storage
│   ├── Relational
│   ├── Document
│   ├── Key-Value
│   ├── Graph
│   ├── Object Storage
│   └── Time Series
│
├── Data Access
│   ├── APIs
│   ├── Queries
│   ├── Events
│   ├── CDC
│   └── Replicas
│
├── Consistency
│   ├── Strong
│   ├── Eventual
│   ├── Causal
│   └── Read Models
│
├── Integration
│   ├── Data Contracts
│   ├── Events
│   ├── ETL
│   ├── ELT
│   └── CDC
│
├── Scalability
│   ├── Indexing
│   ├── Partitioning
│   ├── Sharding
│   └── Replication
│
├── Governance
│   ├── Quality
│   ├── Lineage
│   ├── Classification
│   └── Retention
│
└── Security
    ├── Encryption
    ├── Access Control
    ├── Masking
    └── Audit
```

---

# 4. Data Ownership

Data ownership is one of the most important architectural concepts.

Consider:

```text
Customer Service
Payment Service
Order Service
```

Who owns:

```text
Customer Name?
Payment Status?
Order Status?
```

If all services modify the same database:

```text
Customer Service ─┐
Payment Service  ─┼── Shared Database
Order Service    ─┘
```

ownership becomes ambiguous.

A stronger architecture is:

```text
Customer Service
    ↓
Owns Customer State

Payment Service
    ↓
Owns Payment State

Order Service
    ↓
Owns Order State
```

Other components access the information through:

```text
API
Events
Read Models
Projections
```

Ownership should answer:

> **Who has the authority to change this data?**

---

# 5. Source of Truth

A source of truth is the authoritative system for a piece of business state.

Example:

```text
Payment Service
       ↓
Payment Database
       ↓
Source of Truth
```

Other systems may maintain:

```text
Analytics DB
Search Index
Cache
Customer Activity View
Data Warehouse
```

but these are derived representations.

The architecture should make the distinction explicit:

```text
Authoritative State
        ↓
Events / API
        ↓
Derived State
```

A cache should not silently become a second source of truth.

---

# 6. Single Source of Truth

"Single source of truth" is useful but can be oversimplified.

A large system may legitimately have multiple authoritative datasets because different bounded contexts own different concepts.

For example:

```text
Customer Context
    ↓
Customer Profile

Payment Context
    ↓
Payment State

Inventory Context
    ↓
Inventory State
```

There is no requirement that the entire organization have one giant database.

A better principle is:

> **One authoritative owner per business concept within a defined boundary.**

---

# 7. Write Ownership

Write ownership should be explicit.

Example:

```text
Payment Service
    ↓
Payment.Status
```

Other services should not directly execute:

```sql
UPDATE payments
SET status = ...
```

Instead:

```text
Order Service
    ↓
Payment API / Command
    ↓
Payment Service
    ↓
Payment Database
```

or:

```text
Payment Service
    ↓
PaymentCompleted Event
    ↓
Order Projection
```

This preserves the invariant:

```text
The owner controls its state.
```

---

# 8. Read Ownership

Read ownership is different from write ownership.

A service may own writes while other systems maintain local read models.

Example:

```text
Payment Service
      ↓
PaymentCompleted
      ↓
Customer Activity Projection
```

The projection can answer:

```text
What payments did this customer make?
```

without directly accessing the payment database.

This creates:

```text
Write Model
    ↓
Events
    ↓
Read Model
```

which is a common CQRS architecture.

---

# 9. Data Modeling

Data modeling should start with business concepts rather than database technology.

A useful sequence is:

```text
Business Domain
      ↓
Entities
      ↓
Relationships
      ↓
Invariants
      ↓
Access Patterns
      ↓
Data Model
      ↓
Storage Technology
```

Do not begin with:

> Which database should we use?

Begin with:

> What data exists, who owns it, and how will it be used?

---

# 10. Relational Modeling

Relational databases represent data through:

```text
Tables
Rows
Columns
Keys
Constraints
Relationships
Indexes
Transactions
```

They are particularly strong when the system requires:

```text
Strong consistency
Complex transactions
Referential integrity
Structured queries
Mature tooling
```

Example:

```text
Transaction
----------------
Id
CustomerId
Amount
Currency
Status
CreatedAt
```

Constraints should be used to protect important invariants.

For example:

```sql
UNIQUE (IdempotencyKey)
```

can provide a database-level guarantee that application logic alone cannot safely provide under concurrency.

---

# 11. Database Constraints

Architectural invariants should be enforced as close to the authoritative data as practical.

Examples:

```text
PRIMARY KEY
UNIQUE
FOREIGN KEY
CHECK
NOT NULL
```

For example:

```sql
CHECK (Amount > 0)
```

protects a business invariant at the storage boundary.

But database constraints should not replace domain logic.

A useful model is:

```text
Domain Logic
      +
Database Constraints
      +
Transactional Guarantees
```

The combination provides stronger protection.

---

# 12. Transactions

A transaction provides atomicity over a defined resource boundary.

For example:

```text
BEGIN
    Update Payment
    Insert Outbox Event
COMMIT
```

This is extremely valuable.

The architectural question is:

> What must change atomically?

For TransactionFlow:

```text
Payment State
+
Outbox Event
```

may need to be atomic.

But:

```text
Payment
+
Email
+
Analytics
+
Customer Dashboard
```

usually does not need to be one database transaction.

That distinction prevents unnecessary distributed transactions.

---

# 13. Transaction Boundaries

Transaction boundaries should align with business invariants where practical.

Good:

```text
Transaction
    ↓
Local database transaction
    ↓
Business invariant preserved
```

Risky:

```text
Service A
   ↓
Service B
   ↓
Service C
   ↓
Distributed transaction
```

The second introduces coordination and failure complexity.

A useful principle is:

> **Keep transactions local whenever business invariants allow it.**

---

# 14. Concurrency Control

Multiple operations may modify the same data concurrently.

Common strategies:

```text
Optimistic Concurrency
Pessimistic Locking
Version Numbers
Database Constraints
Serializable Transactions
Single Writer
Partition Ownership
```

Optimistic concurrency example:

```text
Record:
Version = 10
```

Update:

```sql
UPDATE Accounts
SET Balance = @Balance,
    Version = 11
WHERE Id = @Id
  AND Version = 10;
```

If zero rows are affected:

```text
Concurrency conflict
```

This is often preferable to holding long-running distributed locks.

---

# 15. Idempotency and Data Architecture

Idempotency is also a data problem.

For TransactionFlow:

```text
IdempotencyKey
       ↓
Database
       ↓
Unique Constraint
```

The database can guarantee:

```text
One idempotency key
    ↓
One logical transaction
```

The architecture should decide:

```text
What identifies the operation?

Where is the key stored?

How long is it valid?

What happens on duplicate requests?

What happens if the payload differs?
```

A robust invariant:

```text
Same idempotency key
+
Same logical request
=
Same business outcome
```

---

# 16. Normalization

Normalization reduces unnecessary duplication and update anomalies.

Common normal forms include:

```text
1NF
2NF
3NF
BCNF
```

Normalization is useful when:

```text
Transactional integrity
Frequent updates
Strong consistency
Complex relationships
```

are important.

However, normalization should be driven by workload and invariants.

Do not treat normalization as an absolute rule.

---

# 17. Denormalization

Denormalization intentionally duplicates or restructures data to optimize access patterns.

Example:

```text
Normalized:

Customer
Order
Payment
```

Read model:

```text
CustomerOrderSummary
---------------------
CustomerName
OrderId
OrderStatus
PaymentStatus
Total
```

Benefits:

```text
Faster reads
Simpler queries
Reduced joins
Independent read scaling
```

Costs:

```text
Duplication
Synchronization
Staleness
Rebuild complexity
```

Denormalization is especially common in:

```text
CQRS
Search
Analytics
Distributed systems
Read-heavy workloads
```

---

# 18. Polyglot Persistence

Different workloads may justify different storage technologies.

Example:

```text
Transactional Data
    ↓
PostgreSQL

Search
    ↓
Search Engine

Analytics
    ↓
Data Warehouse

Documents
    ↓
Object Storage

Cache
    ↓
Redis
```

Polyglot persistence can improve workload fit.

But every additional technology introduces:

```text
Operational burden
Data synchronization
Backup strategy
Security model
Monitoring
Skill requirements
```

Therefore:

> **Use different storage technologies when the workload justifies the additional complexity.**

---

# 19. Database per Service

In a microservice architecture, a common ownership model is:

```text
Payment Service
    ↓
Payment Database

Order Service
    ↓
Order Database

Inventory Service
    ↓
Inventory Database
```

This does not necessarily mean every service needs a completely separate database server.

It means:

> **The service owns its persistence boundary and other services do not bypass it.**

Possible physical implementations include:

```text
Separate server
Separate database
Separate schema
Separate logical ownership
```

The important property is ownership and access control.

---

# 20. Shared Database

A shared database can be appropriate in some architectures.

For example:

```text
Modular Monolith
        ↓
One PostgreSQL Database
        ↓
Module-owned tables
```

This can provide:

```text
Simple transactions
Low latency
Simple operations
Centralized backup
```

The danger is uncontrolled access:

```text
Module A → Module B tables
Module B → Module C tables
Module C → Module A tables
```

This creates hidden coupling.

A shared database becomes problematic when it undermines the architectural boundaries you intended to create.

---

# 21. Data Coupling

Data coupling occurs when components depend on another component's data representation.

Example:

```text
Service A
    ↓
SELECT *
FROM ServiceBTable
```

Now Service A depends on:

```text
Table name
Column names
Indexes
Schema
Database technology
```

This is stronger coupling than:

```text
Service A
    ↓
Service B API
```

or:

```text
Service B
    ↓
Event Contract
```

Data coupling is one of the most important hidden architectural dependencies.

---

# 22. Data Contracts

A data contract defines the structure and semantics of data exchanged between components.

Examples:

```text
API Contract
Event Contract
Schema
Message Contract
Data Product Contract
```

A good contract defines:

```text
Fields
Types
Required fields
Optional fields
Semantics
Version
Compatibility rules
Ownership
```

Example:

```json
{
  "eventType": "PaymentCompleted",
  "version": 1,
  "paymentId": "...",
  "transactionId": "...",
  "occurredAt": "..."
}
```

The schema is only part of the contract.

The meaning of each field matters too.

---

# 23. Schema Evolution

Data schemas evolve.

A safe evolution strategy often follows:

```text
Expand
    ↓
Migrate
    ↓
Validate
    ↓
Contract
```

Example:

```text
Old:
customer_name

New:
first_name
last_name
```

First introduce:

```text
first_name
last_name
```

Then migrate consumers.

Only later remove:

```text
customer_name
```

This is the same principle used in:

```text
ExpandAndContract
ParallelChange
BranchByAbstraction
```

---

# 24. Backward Compatibility

Distributed systems often have multiple versions operating simultaneously.

Therefore data contracts should support:

```text
Old Producer + New Consumer
New Producer + Old Consumer
```

where practical.

Useful compatibility strategies:

```text
Add optional fields
Do not change existing field meaning
Do not remove fields immediately
Version breaking changes
```

Schema evolution should be treated as an architectural concern.

---

# 25. Change Data Capture

CDC captures changes from a database and publishes them to downstream systems.

```text
Database
    ↓
Change Data Capture
    ↓
Event Stream
    ↓
Consumers
```

Useful for:

```text
Data integration
Analytics
Search indexes
Migration
Replication
Event-driven integration
```

CDC can reduce application-level integration work.

But it also introduces:

```text
Ordering concerns
Schema evolution
Operational complexity
Replay requirements
Duplicate processing
```

CDC should not automatically be treated as equivalent to domain events.

---

# 26. Domain Events vs CDC

Domain event:

```text
PaymentCompleted
```

expresses:

> A business event occurred.

CDC event:

```text
payments row updated
```

expresses:

> Data changed in storage.

These have different semantics.

Domain events are useful when consumers care about:

```text
Business meaning
```

CDC is useful when consumers care about:

```text
Data changes
```

Choosing between them depends on the integration requirement.

---

# 27. Eventual Consistency

Distributed data often becomes eventually consistent.

Example:

```text
Payment DB
    ↓
PaymentCompleted
    ↓
Customer Projection
```

There may be a temporary difference:

```text
Payment DB:
Paid

Customer View:
Pending
```

This is acceptable only if the business permits the inconsistency.

Therefore define:

```text
Maximum staleness
Propagation target
Reconciliation strategy
Failure behavior
```

Example:

```text
Customer activity:
< 5 seconds stale

Analytics:
< 60 seconds stale
```

These should be business requirements, not arbitrary technical assumptions.

---

# 28. Reconciliation

Distributed data can diverge.

A robust architecture should define how divergence is detected and repaired.

```text
Source of Truth
      ↓
Compare
      ↓
Detect Difference
      ↓
Repair
      ↓
Verify
```

Possible mechanisms:

```text
Checksums
Counts
Version numbers
Reconciliation jobs
Replay
Event reprocessing
CDC
```

Reconciliation is particularly important for:

```text
Payments
Inventory
Financial data
Distributed projections
Multi-region systems
```

---

# 29. Data Replication

Replication can be used for:

```text
Availability
Read scaling
Disaster recovery
Geographic distribution
Analytics
```

Common models:

```text
Primary → Replica

Primary
   ↙   ↘
Replica Replica
```

or:

```text
Multi-primary
```

Replication introduces:

```text
Replication lag
Conflict handling
Consistency decisions
Network dependency
Storage cost
```

Measure replication lag explicitly.

---

# 30. Read Replicas

Read replicas can offload read traffic.

```text
Writes
  ↓
Primary

Reads
  ↓
Replica
```

But after a write:

```text
Write Primary
    ↓
Read Replica
```

the latest value may not yet be visible.

This creates:

```text
Read-after-write consistency
```

requirements.

Possible strategies:

```text
Read from primary after write
Track replication position
Use session consistency
Accept bounded staleness
```

The business requirement should determine the approach.

---

# 31. Partitioning

Partitioning divides data into manageable subsets.

Example:

```text
Transactions
    ↓
Partition by CustomerId
```

or:

```text
Partition by time
```

such as:

```text
2026-01
2026-02
2026-03
```

Benefits:

```text
Parallelism
Smaller indexes
Maintenance isolation
Scalability
Data lifecycle management
```

Risks:

```text
Hot partitions
Cross-partition queries
Uneven distribution
Rebalancing complexity
```

Partition strategy should be based on actual access patterns.

---

# 32. Sharding

Sharding distributes data across independent database nodes.

```text
Shard 1 → Customers A-F
Shard 2 → Customers G-M
Shard 3 → Customers N-Z
```

Benefits:

```text
Horizontal data scaling
Parallelism
Larger total capacity
```

Costs:

```text
Cross-shard queries
Cross-shard transactions
Rebalancing
Operational complexity
Hot shards
```

Sharding should normally be introduced only when simpler scaling mechanisms are insufficient.

---

# 33. Hot Keys and Hot Partitions

A single key can dominate workload.

Example:

```text
Tenant A → 80% traffic
Tenant B → 10%
Tenant C → 10%
```

Evenly sized partitions do not guarantee evenly distributed workload.

Measure:

```text
Requests per partition
Storage per partition
CPU per partition
Latency per partition
Hot key frequency
```

Scalability is therefore a workload distribution problem, not simply a node-count problem.

---

# 34. Data Lifecycle

Data has a lifecycle:

```text
Create
  ↓
Use
  ↓
Update
  ↓
Archive
  ↓
Retain
  ↓
Delete
```

Architecture should define:

```text
Retention period
Archive strategy
Deletion strategy
Legal requirements
Backup retention
Recovery requirements
```

Data that should have been deleted but remains in:

```text
Database
Backup
Cache
Logs
Search index
Data warehouse
```

can become a governance and security problem.

---

# 35. Backup and Recovery

A backup strategy must answer:

```text
What is backed up?

How frequently?

Where?

How long retained?

How is it encrypted?

How is restoration tested?
```

Backup is not the same as recoverability.

A stronger model is:

```text
Backup
    ↓
Restore Test
    ↓
Validation
    ↓
Recovery Evidence
```

Important metrics:

```text
RPO
RTO
Restore duration
Recovery success rate
Data loss
```

---

# 36. RPO and RTO

### RPO

Recovery Point Objective:

> How much data loss can the business tolerate?

Example:

```text
RPO = 5 minutes
```

means the architecture should limit potential data loss to the defined five-minute target.

### RTO

Recovery Time Objective:

> How long can the system remain unavailable?

Example:

```text
RTO = 10 minutes
```

These requirements influence:

```text
Replication
Backups
Failover
Storage
Deployment
Disaster recovery
```

---

# 37. Data Security

Data security should be considered throughout the lifecycle.

```text
Data
│
├── At Rest
│
├── In Transit
│
├── In Memory
│
├── In Logs
│
├── In Backups
│
└── In Derived Systems
```

Controls include:

```text
Encryption
Access control
Least privilege
Key management
Data masking
Tokenization
Audit logging
Network isolation
Secrets management
```

Security should follow the data, not only the primary database.

---

# 38. Data Classification

Not all data has the same sensitivity.

A useful classification model might include:

```text
Public
Internal
Confidential
Highly Sensitive
```

Classification should influence:

```text
Storage
Encryption
Access
Retention
Logging
Backup
Sharing
Deletion
```

Architectural decisions should be driven by actual regulatory and business requirements.

---

# 39. Data Governance

Data governance establishes how data is managed across organizational boundaries.

Important concerns:

```text
Ownership
Quality
Lineage
Classification
Access
Retention
Privacy
Metadata
Standards
```

Governance should not become a purely bureaucratic process.

The goal is:

> **Make important data understandable, trustworthy, controlled, and usable.**

---

# 40. Data Quality

Data quality can be measured through:

```text
Accuracy
Completeness
Consistency
Timeliness
Uniqueness
Validity
```

Example:

```text
Customer email
```

Potential quality rules:

```text
Valid format
Not unexpectedly null
Unique where required
Recently verified
Consistent across authoritative systems
```

Poor data quality propagates.

```text
Bad Source Data
      ↓
Event
      ↓
Data Warehouse
      ↓
AI / Analytics
      ↓
Bad Decision
```

Therefore:

> **Data quality is an architectural concern, not merely a data-cleaning task.**

---

# 41. Data Lineage

Data lineage answers:

> Where did this data come from, and how was it transformed?

Example:

```text
Payment Database
      ↓
CDC
      ↓
Kafka
      ↓
Data Lake
      ↓
Transformation
      ↓
Analytics Model
```

Lineage should ideally identify:

```text
Source
Transformation
Destination
Version
Timestamp
Ownership
```

This becomes increasingly important for:

```text
Analytics
Compliance
Auditing
AI systems
Financial reporting
```

---

# 42. Data Architecture for AI Systems

AI systems introduce additional data concerns.

Typical architecture:

```text
Operational Data
       ↓
Ingestion
       ↓
Processing
       ↓
Embedding / Indexing
       ↓
Vector Store
       ↓
Retrieval
       ↓
LLM
```

Additional concerns include:

```text
Data provenance
Chunk lineage
Embedding version
Document version
Access control
PII handling
Freshness
Retrieval quality
Deletion propagation
```

If a document is deleted from the source system:

```text
Source
 ↓
Chunk Store
 ↓
Vector Store
 ↓
Cache
```

the deletion may need to propagate through all derived representations.

This is a data architecture problem.

---

# 43. Data Ownership in AI-Native Systems

AI systems often combine:

```text
User Data
Domain Data
Retrieved Data
Conversation State
Memory
Tool Results
Model Outputs
Evaluation Data
```

Each should have explicit ownership.

For example:

```text
Application
    ↓
Owns conversation state

Domain Service
    ↓
Owns business state

Vector Store
    ↓
Derived retrieval representation

LLM
    ↓
Does not own authoritative business state
```

This aligns with the principle:

> **AI provides intelligence, while the application remains responsible for state and control.**

---

# 44. Data Architecture and CQRS

CQRS separates:

```text
Command Model
```

from:

```text
Query Model
```

Example:

```text
Commands
    ↓
Domain
    ↓
Authoritative Database
    ↓
Events
    ↓
Read Models
```

Read models can be optimized for specific queries.

Benefits:

```text
Read scalability
Query optimization
Independent read models
Domain isolation
```

Costs:

```text
Eventual consistency
Projection maintenance
Rebuild complexity
More storage
Operational complexity
```

CQRS should be introduced because read/write characteristics justify it.

---

# 45. Data Architecture and Event Sourcing

Event sourcing stores state as a sequence of events.

```text
Event 1
Event 2
Event 3
Event 4
   ↓
Rebuild State
```

Example:

```text
PaymentCreated
PaymentAuthorized
PaymentCaptured
```

Potential benefits:

```text
Historical state
Auditability
Event-driven integration
Replay
Temporal analysis
```

Costs:

```text
Event schema evolution
Storage growth
Replay complexity
Query model complexity
Operational complexity
```

Event sourcing is therefore a data architecture decision, not simply a messaging pattern.

---

# 46. Data Architecture and Eventual Consistency

Once data is distributed:

```text
Source
   ↓
Event
   ↓
Projection
   ↓
Search
   ↓
Analytics
```

multiple representations can temporarily disagree.

Therefore every derived representation should have:

```text
Owner
Source
Update mechanism
Expected freshness
Rebuild strategy
Deletion strategy
Reconciliation strategy
```

This is essential for production systems.

---

# 47. Data Architecture Smells

## Shared Database Coupling

```text
Multiple services
      ↓
Same tables
```

---

## Database as Integration Layer

```text
Service A
   ↓
Database
   ↓
Service B
```

rather than explicit contracts.

---

## Unclear Source of Truth

```text
System A says Paid
System B says Pending
System C says Completed
```

with no authoritative owner.

---

## Uncontrolled Duplication

Data is copied everywhere without:

```text
Ownership
Freshness
Reconciliation
```

---

## Distributed Transactions Everywhere

```text
Service A
 + Service B
 + Service C
 + Service D
```

inside one transaction boundary.

---

## Analytics on Production Database

Large analytical queries compete with transactional workloads.

---

## No Data Lifecycle

Data is retained indefinitely without a defined business reason.

---

## Schema Coupling

Consumers depend directly on storage schemas they do not own.

---

# 48. Data Architecture Invariants

Important invariants include:

```text
Every important business concept has an explicit owner.

Every authoritative dataset has a defined source of truth.

Only the owner may directly modify authoritative state.

Other services access owned data through defined contracts.

Derived data has a defined source.

Derived data has an expected freshness.

Critical invariants are protected by transactions or constraints.

Schema changes follow compatibility rules.

Sensitive data has explicit protection requirements.

Data retention and deletion rules are defined.

Backups are restorable and tested.

Data lineage exists for important analytical and regulated data.
```

These invariants should become architecture tests, database constraints, governance rules, and operational checks where appropriate.

---

# 49. Data Architecture and Scalability

Data often becomes the scaling bottleneck before compute.

Potential bottlenecks:

```text
Database CPU
Database connections
Disk I/O
Lock contention
Indexes
Network bandwidth
Hot partitions
Large transactions
Replication lag
Storage capacity
```

A useful scaling sequence is:

```text
Measure
  ↓
Optimize Query
  ↓
Indexes
  ↓
Connection Management
  ↓
Caching
  ↓
Read Replicas
  ↓
Partitioning
  ↓
Sharding
```

Do not jump directly to sharding.

---

# 50. Data Access Patterns

Storage architecture should follow access patterns.

Ask:

```text
How is the data read?

How is it written?

What is the read/write ratio?

What queries are required?

What consistency is required?

What is the expected dataset size?

What is the growth rate?

What is the concurrency?

What is the retention period?
```

Example:

```text
Transactional workload
    ↓
PostgreSQL

Full-text search
    ↓
Search engine

Large files
    ↓
Object storage

High-frequency ephemeral state
    ↓
Cache
```

The workload determines the storage strategy.

---

# 51. Data Architecture and Caching

Caching is a derived-data strategy.

```text
Source of Truth
      ↓
Cache
```

Important questions:

```text
What can be cached?

How stale can it be?

Who invalidates it?

What happens when the cache fails?

Can the system rebuild the cache?

What happens during a cache stampede?
```

Never allow:

```text
Cache
   ↓
Accidental Source of Truth
```

unless that is an explicit architectural decision.

---

# 52. Data Migration

Data migration is an architectural change, not simply a script.

A robust migration often follows:

```text
Discover
   ↓
Characterize
   ↓
Prepare
   ↓
Expand
   ↓
Backfill
   ↓
Dual Read / Validate
   ↓
Cutover
   ↓
Monitor
   ↓
Contract
```

Important migration properties:

```text
Reversibility
Validation
Idempotency
Observability
Rollback
Data reconciliation
```

Example:

```text
SQL Server
    ↓
PostgreSQL
```

should include:

```text
Schema migration
Data transformation
Backfill
Validation
Performance comparison
Cutover
Rollback
```

---

# 53. Data Migration Validation

Never assume:

```text
Migration completed
```

means:

```text
Migration succeeded.
```

Validate:

```text
Row counts
Checksums
Aggregates
Foreign-key integrity
Business invariants
Null rates
Duplicate rates
Sampling
Application behavior
Performance
```

For financial systems:

```text
Total amount
+
Transaction count
+
Status distribution
```

may be more meaningful than simply comparing row counts.

---

# 54. Data Architecture and Disaster Recovery

A complete disaster recovery strategy includes:

```text
Backup
Replication
Failover
Restore
Validation
Reconciliation
```

Consider:

```text
Database failure
Region failure
Storage corruption
Accidental deletion
Credential compromise
Ransomware
Operator error
```

Recovery architecture should define:

```text
RTO
RPO
Recovery owner
Recovery procedure
Validation
Rollback
```

---

# 55. Data Architecture Metrics

Important metrics include:

```text
Storage size
Storage growth rate
Read latency
Write latency
Query latency
Throughput
Connection utilization
Lock contention
Cache hit rate
Replication lag
Partition skew
Transaction duration
Deadlocks
Error rate
Data freshness
Data quality
Data loss
RPO
RTO
```

For distributed data:

```text
Consumer lag
Projection lag
Reconciliation failures
Duplicate records
Missing records
```

---

# 56. Data Architecture Comparison Dimensions

When comparing data architectures, measure:

```text
Ownership
Coupling
Consistency
Latency
Throughput
Scalability
Availability
Failure isolation
Operational complexity
Migration complexity
Cost
Governance
```

For example:

```text
Shared Database
        vs
Owned Data
```

should not be evaluated with:

> Which is better?

Instead ask:

```text
Which provides the required ownership model?

What coupling exists?

What consistency is required?

What operational complexity is acceptable?

How will the data evolve?

How will failure be handled?
```

---

# 57. Modern .NET Mapping

Modern .NET provides several useful data architecture building blocks.

## Relational

```text
PostgreSQL
SQL Server
EF Core
Npgsql
Dapper
```

## Distributed Data

```text
Redis
Kafka
RabbitMQ
Azure Service Bus
```

## Persistence Patterns

```text
Repository
Unit of Work
Outbox
Inbox
CQRS
Read Models
```

## Database Migration

```text
EF Core Migrations
Flyway
DbUp
SQL migration pipelines
```

## Testing

```text
Testcontainers
Integration tests
Architecture tests
Migration tests
Performance tests
```

## Observability

```text
OpenTelemetry
Database metrics
Query tracing
Application metrics
```

The architectural choice should remain independent of the specific ORM.

---

# 58. TransactionFlow Data Architecture

A practical architecture:

```text
Client
   ↓
Transaction API
   ↓
Transaction Service
   ↓
PostgreSQL
   │
   ├── Transactions
   ├── Idempotency
   ├── Audit
   └── Outbox
          ↓
        Kafka
          ↓
       Consumers
```

Ownership:

```text
Transaction Service
    ↓
Transaction state

Transaction Service
    ↓
Idempotency state

Transaction Service
    ↓
Audit state
```

Derived:

```text
Kafka
 ↓
Analytics
Search
Notifications
Customer Activity
```

This creates a clear distinction:

```text
Authoritative State
        vs
Derived State
```

---

# 59. TransactionFlow Data Invariants

Example invariants:

```text
Transaction ID is unique.

Idempotency key is unique within its defined scope.

Amount must be valid.

Completed transactions cannot return to Pending.

Only Transaction Service modifies transaction state.

Outbox record is committed atomically with transaction state.

Events contain stable identifiers.

Consumers are idempotent.

Audit records are immutable.

Derived views never modify authoritative transaction state.
```

These should be protected at multiple levels:

```text
Domain
+
Database
+
Architecture Tests
+
Integration Tests
+
Operational Monitoring
```

---

# 60. Data Architecture Experiments

Recommended experiments:

```text
01-IndexingImpact
02-QueryPerformance
03-DatabaseContention
04-ConnectionPoolSaturation
05-OptimisticConcurrency
06-IdempotencyUnderConcurrency
07-ReadReplicaLag
08-CacheStampede
09-PartitionSkew
10-Sharding
11-CDC
12-OutboxReliability
13-SchemaEvolution
14-DataMigration
15-ConsistencyLag
16-DataReconciliation
17-BackupRestore
18-DatabaseFailure
19-ProjectionRebuild
20-DataQuality
```

A common experimental model:

```text
Business Workload
       ↓
Data Architecture
       ↓
Load / Failure
       ↓
Measurement
       ↓
Evidence
```

---

# 61. Data Architecture Failure Experiments

Important scenarios include:

### Database Failure

```text
Primary unavailable
      ↓
Observe
      ↓
Failover
      ↓
Measure RTO/RPO
```

### Replication Lag

```text
High write load
      ↓
Replica lag
      ↓
Measure stale reads
```

### Duplicate Processing

```text
Same event
      ↓
Consumer twice
      ↓
Verify one business effect
```

### Migration Failure

```text
Migration interrupted
      ↓
Restart
      ↓
Verify idempotency
```

### Projection Failure

```text
Read model deleted
      ↓
Replay events
      ↓
Rebuild
```

### Data Corruption

```text
Invalid record
      ↓
Detection
      ↓
Isolation
      ↓
Recovery
```

---

# 62. Senior Architect Interview Questions

## Fundamentals

1. What is data architecture?
2. Why is data ownership important?
3. What is a source of truth?
4. Why is a shared database architectural coupling?
5. How do you decide where data should live?

## Modeling

6. When would you normalize data?
7. When would you denormalize?
8. How do access patterns influence data modeling?
9. How do you choose a partition key?
10. What causes hot partitions?

## Consistency

11. When is strong consistency required?
12. Where is eventual consistency acceptable?
13. How do you define acceptable staleness?
14. How do you reconcile distributed data?

## Distributed Data

15. What is replication?
16. What is the difference between synchronous and asynchronous replication?
17. What is CDC?
18. How is CDC different from domain events?
19. How do you handle schema evolution?

## Transactions

20. What should be inside a transaction boundary?
21. Why should distributed transactions be minimized?
22. How would you implement idempotency?
23. How do you handle concurrent updates?

## Scalability

24. How would you scale a PostgreSQL workload?
25. When would you introduce read replicas?
26. When would you partition?
27. When would you shard?

## Architecture

28. How do you migrate from a shared database to owned data?
29. How do you design a data architecture for microservices?
30. How do you design data architecture for an AI system?
31. How do you ensure data is recoverable?
32. How do you validate a data migration?

A strong Senior Architect answer should follow:

```text
Business Concept
      ↓
Ownership
      ↓
Source of Truth
      ↓
Access Pattern
      ↓
Consistency
      ↓
Storage
      ↓
Integration
      ↓
Failure
      ↓
Recovery
      ↓
Governance
      ↓
Evidence
```

---

# 63. Definition of Done

You should be able to:

```text
[ ] Explain data ownership.

[ ] Define a source of truth.

[ ] Distinguish authoritative data from derived data.

[ ] Design data boundaries around business ownership.

[ ] Explain relational modeling.

[ ] Explain normalization and denormalization.

[ ] Define transaction boundaries.

[ ] Design optimistic concurrency.

[ ] Design idempotency using persistent state.

[ ] Explain data contracts.

[ ] Design backward-compatible schema evolution.

[ ] Explain CDC.

[ ] Distinguish CDC from domain events.

[ ] Explain replication.

[ ] Explain replication lag.

[ ] Explain partitioning.

[ ] Explain sharding.

[ ] Identify hot partitions.

[ ] Explain CQRS read models.

[ ] Explain event sourcing at the data level.

[ ] Design reconciliation.

[ ] Design backup and recovery.

[ ] Define RPO and RTO.

[ ] Design data security.

[ ] Define data lifecycle and retention.

[ ] Explain data lineage.

[ ] Explain data quality.

[ ] Design data architecture for distributed systems.

[ ] Design data architecture for AI systems.

[ ] Connect data architecture to architecture evolution.

[ ] Design data architecture experiments.

[ ] Explain modern .NET data architecture options.
```

---

# 64. Key Takeaways

The most important principles are:

> **Data architecture is about ownership, not simply databases.**

> **Every important business concept should have an explicit authoritative owner.**

> **Derived data is useful, but its source, freshness, rebuild strategy, and lifecycle must be known.**

> **Keep transactions local whenever business invariants allow it.**

> **Use database constraints to protect critical invariants.**

> **Consistency requirements should be defined per business invariant, not globally.**

> **Schema evolution is an architectural concern.**

> **Replication, partitioning, and sharding solve different problems and introduce different costs.**

> **Data duplication is acceptable when ownership, freshness, and reconciliation are explicit.**

> **A database is not an integration contract.**

> **Data quality, lineage, security, retention, and recovery are architectural concerns.**

> **AI systems add derived data, embeddings, memory, provenance, and deletion-propagation problems to traditional data architecture.**

The core reasoning loop is:

```text
Business Concept
       ↓
Data Ownership
       ↓
Source of Truth
       ↓
Access Patterns
       ↓
Data Model
       ↓
Consistency
       ↓
Storage
       ↓
Integration
       ↓
Replication / Partitioning
       ↓
Security + Governance
       ↓
Lifecycle
       ↓
Recovery
       ↓
Measurement
       ↓
Architecture Decision
```
