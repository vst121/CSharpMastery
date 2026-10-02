# Cloud Architecture

## 1. Essence

Cloud Architecture defines how applications, data, infrastructure, identities, networks, and operational capabilities are structured to run effectively in a cloud environment.

Cloud architecture is not:

```text
Azure Service
    +
Azure Service
    +
Azure Service
```

It is the deliberate design of:

- compute
- networking
- storage
- data
- identity
- security
- availability
- scalability
- resilience
- observability
- deployment
- cost
- governance
- disaster recovery
- vendor dependencies

A useful definition is:

> Cloud Architecture is the design of a system's runtime, infrastructure, data, networking, identity, operational, and economic boundaries using cloud capabilities.

The central question is not:

> Which Azure service should I use?

It is:

> What system properties do I need, and which cloud capabilities provide them with acceptable trade-offs?

---

# 2. Cloud Architecture Is Architecture

Moving an application to the cloud does not automatically make it:

- scalable
- resilient
- secure
- cost efficient
- highly available

For example:

```text
On-Premise Application
        ↓
Move to Azure VM
        ↓
Still the same architecture
```

This may change infrastructure location without changing architectural properties.

A stronger cloud architecture considers:

```text
Business Requirements
        ↓
Quality Attributes
        ↓
Architecture
        ↓
Cloud Capabilities
        ↓
Deployment Topology
        ↓
Operations
        ↓
Measurement
```

---

# 3. Cloud Architecture Objectives

Typical cloud objectives include:

```text
Availability
Scalability
Elasticity
Resilience
Security
Performance
Cost Efficiency
Operational Simplicity
Deployability
Recoverability
Observability
Governance
```

These objectives can conflict.

For example:

```text
More Redundancy
      ↓
Higher Availability
      ↓
Higher Cost
```

or:

```text
More Managed Services
      ↓
Lower Operational Burden
      ↓
Potentially Higher Vendor Coupling
```

Cloud architecture is therefore fundamentally a trade-off discipline.

---

# 4. Cloud Service Models

The major cloud service models are:

```text
IaaS
PaaS
SaaS
Serverless
Managed Services
```

---

## 4.1 IaaS

Infrastructure as a Service.

Examples:

```text
Azure Virtual Machines
Azure Virtual Network
Managed Disks
```

You manage more of:

- operating system
- runtime
- patches
- configuration
- scaling
- security

You receive more infrastructure control.

---

## 4.2 PaaS

Platform as a Service.

Examples:

```text
Azure App Service
Azure Database for PostgreSQL
Azure Functions
Azure Container Apps
Azure Service Bus
Azure Event Hubs
```

The provider manages more infrastructure.

You focus more on:

```text
Application
Configuration
Data
Business Logic
```

---

## 4.3 SaaS

Software as a Service.

Examples include cloud-hosted:

```text
Identity
Collaboration
Monitoring
CRM
Security
Analytics
```

The customer primarily consumes a capability rather than operating the underlying platform.

---

# 5. Managed Service Principle

A useful cloud architecture question is:

> Is this infrastructure capability strategically important enough for us to operate ourselves?

For example:

```text
Self-Managed PostgreSQL
        vs
Azure Database for PostgreSQL
```

The decision should consider:

```text
Operational Burden
Control
Performance
Cost
Security
Availability
Backup
Recovery
Skills
Vendor Dependency
Customization
```

Do not assume managed services are always better.

Instead:

> Use managed capabilities when they reduce operational complexity without violating important architectural constraints.

---

# 6. Shared Responsibility

Cloud security and operations follow a shared-responsibility model.

Conceptually:

```text
Cloud Provider
        +
Customer
```

The provider may manage:

```text
Physical Infrastructure
Datacenter
Underlying Hardware
Core Cloud Platform
```

The customer still manages some combination of:

```text
Identity
Permissions
Data
Application
Configuration
Secrets
Network Rules
Workloads
Monitoring
```

The exact responsibility depends on the service model.

---

# 7. Azure Resource Hierarchy

Azure architecture should be understood as a hierarchy.

A simplified model:

```text
Microsoft Entra Tenant
        ↓
Management Groups
        ↓
Subscriptions
        ↓
Resource Groups
        ↓
Resources
```

These boundaries can support:

- governance
- billing
- isolation
- permissions
- lifecycle management

---

# 8. Subscription Boundaries

Azure subscriptions can provide organizational and operational boundaries.

They can be separated by:

```text
Environment
Business Unit
Application
Security Boundary
Cost Center
Lifecycle
```

Example:

```text
Platform
 ├── Production Subscription
 ├── Staging Subscription
 └── Development Subscription
```

For larger organizations:

```text
Management Group
 ├── Production
 │    ├── Subscription A
 │    └── Subscription B
 │
 └── NonProduction
      ├── Subscription C
      └── Subscription D
```

The correct structure depends on governance and isolation requirements.

---

# 9. Resource Groups

Resource Groups provide lifecycle and organizational boundaries.

For example:

```text
TransactionFlow-Production
    ├── Container App
    ├── PostgreSQL
    ├── Key Vault
    ├── Application Insights
    └── Storage
```

Resources with related lifecycle dependencies can often be grouped together.

Resource Groups should not become arbitrary folders.

They should reflect meaningful operational ownership.

---

# 10. Regions

A cloud region is a geographic deployment location.

Choosing a region involves:

```text
Latency
Data Residency
Regulation
Service Availability
Cost
Disaster Recovery
Network Connectivity
Customer Location
```

For a European application:

```text
Users
  ↓
Nearest Appropriate Azure Region
```

But geographic proximity alone is not sufficient.

---

# 11. Availability Zones

Availability Zones provide physically separated locations within an Azure region.

Conceptually:

```text
Region
 ├── Zone 1
 ├── Zone 2
 └── Zone 3
```

Deploying across zones can reduce the impact of:

- hardware failures
- datacenter failures
- localized infrastructure failures

But zone redundancy usually introduces:

- additional cost
- network complexity
- cross-zone traffic
- operational considerations

---

# 12. Region Redundancy

For stronger disaster recovery:

```text
Primary Region
      ↓
Secondary Region
```

For example:

```text
West Europe
      +
North Europe
```

The exact regions should be selected according to:

- business requirements
- supported services
- data residency
- RPO
- RTO
- cost
- network topology

Multi-region architecture is not automatically justified.

---

# 13. Availability

Availability describes whether a service is usable when required.

A simple model:

```text
Availability =
Uptime / Total Time
```

For multiple independent components:

```text
Service A
   ↓
Service B
   ↓
Database
```

The overall availability may be constrained by the dependency chain.

For sequential mandatory dependencies:

```text
A_availability × B_availability × DB_availability
```

Therefore:

> Adding dependencies can reduce availability even when every dependency is individually reliable.

---

# 14. High Availability

High availability typically requires:

```text
Redundancy
+
Failure Detection
+
Automatic Recovery
+
Traffic Distribution
```

Example:

```text
Load Balancer
      ↓
 ┌────┴────┐
 ↓         ↓
Instance A Instance B
      ↓
Managed Database
```

But redundancy without failure isolation is not sufficient.

---

# 15. Resilience

Availability asks:

> Is the system available?

Resilience asks:

> How does the system behave when something fails?

Examples:

```text
Timeout
Retry
Circuit Breaker
Bulkhead
Fallback
Load Shedding
Backpressure
Queueing
Graceful Degradation
Failover
Recovery
```

Cloud architecture should design for failure rather than assuming infrastructure will not fail.

---

# 16. Failure Domains

Cloud resources exist within failure domains.

Examples:

```text
Process
Container
VM
Availability Zone
Region
Cloud Service
Cloud Provider
```

Architecture should decide which failures it must survive.

Example:

```text
Requirement:
Survive single VM failure
```

does not necessarily imply:

```text
Requirement:
Survive regional failure
```

The larger the failure domain, the higher the architectural and economic cost of protection.

---

# 17. Compute Architecture

Cloud compute options include:

```text
Virtual Machines
App Service
Azure Functions
Container Apps
AKS
Batch
Serverless
```

Selection should be driven by workload characteristics.

Ask:

```text
Is the workload long-running?
Stateful?
Stateless?
Event-driven?
Containerized?
GPU-intensive?
Latency-sensitive?
Burst-oriented?
Operationally complex?
```

---

# 18. Azure Compute Decision

A simplified decision model:

```text
Simple Web Application
        ↓
App Service

Containerized Application
        ↓
Container Apps

Complex Kubernetes Platform
        ↓
AKS

Event-Driven Function
        ↓
Azure Functions

Full Infrastructure Control
        ↓
Virtual Machines
```

This is not a universal rule.

The architecture should consider:

```text
Control
Operational Burden
Scaling
Networking
Security
Portability
Cost
Team Expertise
```

---

# 19. Virtual Machines

VMs provide significant control.

You manage more:

```text
OS
Runtime
Patching
Configuration
Scaling
Monitoring
Security
```

Use cases may include:

- legacy applications
- specialized workloads
- custom infrastructure
- workloads requiring OS-level control

The cost is operational responsibility.

---

# 20. App Service

Azure App Service abstracts much infrastructure management.

Useful for:

```text
ASP.NET Core APIs
Web Applications
Background Applications
```

Advantages:

```text
Simple Deployment
Managed Platform
Integrated Scaling
TLS
Deployment Slots
Monitoring
```

Trade-offs include:

```text
Less Infrastructure Control
Platform Constraints
Potential Vendor Coupling
```

---

# 21. Azure Container Apps

Container Apps can provide a middle ground:

```text
Container
   ↓
Managed Platform
   ↓
Autoscaling
   ↓
Cloud Runtime
```

Useful for:

- APIs
- microservices
- workers
- event-driven services
- containerized applications

It can reduce operational burden compared with operating Kubernetes directly.

---

# 22. AKS

Azure Kubernetes Service provides Kubernetes orchestration.

AKS is appropriate when the architecture genuinely benefits from Kubernetes capabilities such as:

```text
Complex Scheduling
Custom Networking
Service Mesh
Advanced Workload Management
Operator Ecosystem
Multi-Workload Platform
Kubernetes APIs
```

Do not choose Kubernetes simply because:

```text
"Microservices use Kubernetes."
```

Kubernetes introduces operational complexity.

The real question is:

> Does the organization need Kubernetes capabilities strongly enough to justify operating a Kubernetes platform?

---

# 23. Serverless

Serverless architecture shifts infrastructure responsibility toward the cloud provider.

Example:

```text
Event
 ↓
Azure Function
 ↓
Processing
 ↓
Storage
```

Benefits:

```text
Elasticity
Low Operational Burden
Event-Driven Scaling
Potentially Efficient Cost Model
```

Trade-offs:

```text
Cold Starts
Execution Limits
Debugging Complexity
Platform Coupling
Distributed Observability
```

---

# 24. Stateless Compute

Stateless compute is highly valuable for cloud scaling.

Prefer:

```text
Instance
   ↓
Request
   ↓
External State
```

instead of:

```text
Instance
   ↓
Local Session State
```

Externalize state to:

```text
Database
Cache
Object Storage
Message Broker
Distributed State Store
```

This enables easier:

- scaling
- replacement
- failover
- deployment

---

# 25. Stateful Workloads

Stateful systems require explicit architecture.

Examples:

```text
Database
Kafka
Distributed Cache
File Storage
Vector Database
```

Questions:

```text
Where is state stored?
Who owns it?
How is it replicated?
How is it recovered?
What is the consistency model?
What happens during failover?
```

Cloud compute architecture should avoid accidentally creating hidden state.

---

# 26. Autoscaling

Autoscaling changes capacity according to demand.

Possible signals:

```text
CPU
Memory
Requests
Queue Length
Kafka Lag
Custom Metrics
Concurrent Connections
```

Example:

```text
Queue Depth
     ↓
Autoscaling Policy
     ↓
More Workers
```

Autoscaling should follow actual workload pressure rather than arbitrary infrastructure metrics.

---

# 27. Scaling Dimensions

Cloud architecture should distinguish:

```text
Vertical Scaling
Horizontal Scaling
Elastic Scaling
Partition Scaling
Database Scaling
Read Scaling
Write Scaling
```

For example:

```text
API
 ↓
Horizontal Scaling
```

may be easy.

But:

```text
PostgreSQL
 ↓
Write Scaling
```

may be significantly more complex.

Therefore:

> Cloud compute may scale easily while the data architecture remains the actual bottleneck.

---

# 28. Networking

Cloud networking is an architectural boundary.

Typical Azure components:

```text
Virtual Network
Subnet
Network Security Group
Private Endpoint
Load Balancer
Application Gateway
Azure Front Door
NAT Gateway
Firewall
Private DNS
VPN
ExpressRoute
```

Architecture should explicitly define:

```text
Ingress
Egress
Internal Communication
Private Access
Public Access
Network Segmentation
```

---

# 29. Virtual Network

A Virtual Network provides logical network isolation.

Example:

```text
VNet
 ├── Public Subnet
 ├── Application Subnet
 └── Data Subnet
```

Modern architectures often minimize public exposure.

Prefer:

```text
Internet
   ↓
Public Edge
   ↓
Private Application
   ↓
Private Data
```

rather than:

```text
Internet
   ↓
Everything Public
```

---

# 30. Private Endpoints

Private endpoints allow access to supported Azure services through private networking.

Example:

```text
Application
    ↓
Private Network
    ↓
Azure PostgreSQL
```

This can reduce public exposure.

But private networking introduces operational considerations:

```text
DNS
Routing
Connectivity
Troubleshooting
Network Governance
```

Private does not automatically mean secure.

Authorization is still required.

---

# 31. Network Security

Network security controls can include:

```text
NSG
Azure Firewall
Network Policies
Private Endpoints
Application Gateway
WAF
DDoS Protection
Egress Controls
```

Use network controls to reinforce architectural boundaries.

Do not rely on network location as the only authorization mechanism.

---

# 32. API Edge Architecture

A typical cloud API architecture:

```text
Internet
   ↓
Azure Front Door / Application Gateway
   ↓
WAF
   ↓
API
   ↓
Application
   ↓
Data
```

Depending on requirements, additional capabilities may include:

```text
Rate Limiting
TLS Termination
Authentication
Routing
Caching
DDoS Protection
Bot Protection
Observability
```

---

# 33. Azure Front Door

Azure Front Door can provide a global application entry point.

Typical responsibilities:

```text
Global Routing
TLS
Caching
WAF Integration
Traffic Distribution
Edge Optimization
```

It can be useful for globally distributed applications.

For a regional internal application, it may introduce unnecessary complexity.

---

# 34. API Management

Azure API Management can provide:

```text
API Gateway
Authentication Integration
Rate Limiting
Quotas
Transformation
Versioning
Developer Portal
Analytics
```

Use it when API governance and lifecycle management justify the additional component.

Avoid introducing gateways simply because:

```text
"Microservices need an API Gateway."
```

The gateway itself becomes:

```text
Operational Dependency
Security Boundary
Performance Hop
Potential Bottleneck
```

---

# 35. Data Architecture in the Cloud

Cloud data architecture includes:

```text
Relational Databases
NoSQL
Object Storage
Caches
Search
Data Warehouses
Event Streams
Data Lakes
Vector Stores
```

Azure examples:

```text
Azure Database for PostgreSQL
Azure SQL Database
Azure Cosmos DB
Azure Blob Storage
Azure Cache for Redis
Azure Data Explorer
Azure Data Lake Storage
Azure Event Hubs
```

The choice should follow data access patterns and consistency requirements.

---

# 36. Azure Database for PostgreSQL

For TransactionFlow, PostgreSQL is a strong default.

Architecture:

```text
ASP.NET Core
      ↓
Azure Database for PostgreSQL
      ↓
Transactional Data
```

Important concerns:

```text
HA
Backups
RPO
RTO
Connection Pooling
Scaling
Read Replicas
Network Isolation
Encryption
Monitoring
Maintenance
```

A managed PostgreSQL service removes infrastructure operations, but not database architecture responsibilities.

---

# 37. Object Storage

Object storage is appropriate for:

```text
Documents
Images
Audio
Video
Backups
Logs
Artifacts
Large Files
AI Datasets
Model Assets
```

Azure Blob Storage is a common choice.

Avoid using relational databases for large binary objects simply because the application already has a database.

---

# 38. Cache Architecture

Azure Cache for Redis can reduce load and latency.

Typical architecture:

```text
API
 ↓
Cache
 ↓
Database
```

But cache introduces:

```text
Stale Data
Invalidation
Memory Cost
Consistency Questions
Failure Modes
```

A cache is not automatically a source of truth.

The source of truth should remain explicit.

---

# 39. Message and Event Architecture

Azure provides multiple messaging capabilities.

Examples:

```text
Azure Service Bus
Azure Event Hubs
Event Grid
Kafka
```

These solve different problems.

Conceptually:

```text
Commands / Business Messaging
        ↓
Service Bus

High-Throughput Event Streaming
        ↓
Event Hubs / Kafka

Reactive Cloud Events
        ↓
Event Grid
```

The architecture should start from messaging semantics rather than product names.

---

# 40. Messaging Questions

Before selecting a messaging technology ask:

```text
Do we need queues?
Streams?
Ordering?
Replay?
Consumer Groups?
Transactions?
Dead Lettering?
Long Retention?
High Throughput?
Exactly-Once-Like Processing?
Event Routing?
```

Messaging technology should follow those requirements.

---

# 41. Event-Driven Cloud Architecture

Example:

```text
TransactionService
       ↓
Outbox
       ↓
Kafka / Event Hubs
       ↓
Consumers
       ↓
Read Models
```

Important concerns:

```text
At-Least-Once Delivery
Idempotency
Ordering
Partitioning
Consumer Lag
Dead Lettering
Schema Evolution
Observability
Recovery
```

Cloud infrastructure does not remove distributed-system problems.

---

# 42. Identity and Access Management

Azure identity architecture commonly uses:

```text
Microsoft Entra ID
Managed Identity
Azure RBAC
Application Roles
Workload Identity
Key Vault
```

A useful model:

```text
Human
  ↓
Entra ID
  ↓
Application
  ↓
Authorization Policy
```

and:

```text
Workload
  ↓
Managed Identity
  ↓
Azure Resource
```

---

# 43. Managed Identity

Managed Identity is particularly useful for Azure workloads.

Example:

```text
Container App
     ↓
Managed Identity
     ↓
Key Vault
```

or:

```text
Application
     ↓
Managed Identity
     ↓
Azure Storage
```

This reduces the need to distribute long-lived secrets.

---

# 44. Azure RBAC

Azure RBAC controls access to Azure resources.

Architectural principle:

> Grant the narrowest role at the narrowest practical scope.

For example:

```text
Application Identity
    ↓
Specific Resource
    ↓
Specific Role
```

rather than:

```text
Application Identity
    ↓
Subscription Owner
```

---

# 45. Secrets and Key Management

Azure Key Vault can provide centralized management of:

```text
Secrets
Keys
Certificates
```

Typical architecture:

```text
Application
     ↓
Managed Identity
     ↓
Key Vault
     ↓
Secret
```

This creates a dependency.

Therefore consider:

```text
What happens if Key Vault is unavailable?
Does the application cache configuration?
How quickly can secrets rotate?
How is access audited?
```

---

# 46. Observability

Cloud architecture must include:

```text
Logs
Metrics
Traces
Alerts
Dashboards
Audit Events
```

Azure services include:

```text
Azure Monitor
Application Insights
Log Analytics
```

For distributed systems, use OpenTelemetry where practical.

A useful architecture:

```text
Application
    ↓
OpenTelemetry
    ↓
Metrics + Logs + Traces
    ↓
Azure Monitor / Application Insights
```

---

# 47. Distributed Tracing

A cloud request may cross:

```text
Client
 ↓
Front Door
 ↓
API
 ↓
Service
 ↓
Database
 ↓
Kafka
 ↓
Consumer
```

Without distributed tracing, diagnosing failures becomes difficult.

Propagate:

```text
TraceId
SpanId
CorrelationId
RequestId
MessageId
CausationId
```

---

# 48. Cloud Logging

Centralized logging is useful for:

```text
Operations
Security
Debugging
Performance
Incident Response
```

But centralized logging creates:

```text
Cost
Data Volume
Retention Decisions
Sensitive Data Risk
Query Performance
```

Never assume:

```text
More Logs = Better Observability
```

Useful logs are:

```text
Structured
Correlated
Actionable
Appropriately Retained
Safe
```

---

# 49. Reliability Architecture

Cloud reliability should be designed across layers.

```text
Application
   ↓
Compute
   ↓
Network
   ↓
Data
   ↓
Messaging
   ↓
Cloud Region
   ↓
Cloud Provider
```

For each layer ask:

```text
What can fail?
How is failure detected?
How is traffic redirected?
What state is lost?
How is recovery performed?
```

---

# 50. Health Checks

Cloud workloads should expose meaningful health signals.

Distinguish:

```text
Liveness
Readiness
Startup
Dependency Health
```

Example:

```text
Liveness
→ Is the process alive?

Readiness
→ Can this instance receive traffic?

Dependency Health
→ Are required dependencies available?
```

Do not make liveness depend on every external dependency.

Otherwise a database outage can cause every instance to restart.

---

# 51. Graceful Shutdown

Cloud platforms routinely replace instances.

Applications should support:

```text
Stop accepting new work
        ↓
Finish active work
        ↓
Commit state
        ↓
Publish required events
        ↓
Close resources
        ↓
Exit
```

For message consumers:

```text
Stop consumption
        ↓
Finish current message
        ↓
Commit acknowledgment
        ↓
Shutdown
```

Graceful shutdown reduces duplicate processing and partial work.

---

# 52. Deployment Architecture

Cloud deployment strategies include:

```text
Rolling Deployment
Blue/Green
Canary
Feature Flags
Shadow Traffic
Progressive Delivery
```

Choose based on:

```text
Risk
Rollback Speed
Traffic Volume
State Compatibility
Database Changes
Operational Capability
```

---

# 53. Blue/Green Deployment

Conceptually:

```text
             Load Balancer
                  ↓
          ┌───────┴───────┐
          ↓               ↓
        Blue             Green
       Version N        Version N+1
```

Traffic can move between environments.

Advantages:

```text
Fast Rollback
Isolation
Controlled Cutover
```

Costs:

```text
Additional Infrastructure
Data Compatibility
Deployment Complexity
```

---

# 54. Canary Deployment

Canary deployment gradually shifts traffic:

```text
Version A
   ↓
95% Traffic

Version B
   ↓
5% Traffic
```

Then:

```text
90 / 10
75 / 25
50 / 50
0 / 100
```

The decision should be based on telemetry such as:

```text
Error Rate
Latency
Business Errors
Resource Usage
Security Signals
```

Canary deployment is especially valuable for high-risk changes.

---

# 55. Infrastructure as Code

Cloud infrastructure should be reproducible.

Common tools include:

```text
Bicep
Terraform
Azure CLI
ARM Templates
Pulumi
```

Azure-native environments often use:

```text
Bicep
```

Infrastructure should be treated like software:

```text
Version Control
Code Review
Testing
Validation
CI/CD
Change History
```

---

# 56. Infrastructure Drift

Manual changes can cause:

```text
Declared Infrastructure
        ≠
Actual Infrastructure
```

This is configuration drift.

Infrastructure as Code reduces drift by making desired state explicit.

A mature architecture should detect:

```text
Unexpected Changes
Missing Resources
Permission Changes
Network Changes
Configuration Drift
```

---

# 57. Environment Architecture

Typical environments:

```text
Development
Test
Staging
Production
```

Avoid making environments fundamentally different unless necessary.

Prefer:

```text
Same Architecture
Different Scale / Configuration
```

where practical.

Environment-specific differences should be explicit.

---

# 58. Configuration Architecture

Separate:

```text
Code
Configuration
Secrets
Environment State
```

For example:

```text
Code
   +
Environment Configuration
   +
Secret Store
```

Do not bake environment-specific credentials into application binaries.

---

# 59. Cost Architecture

Cloud introduces a new architectural property:

> Economic efficiency.

Cost depends on:

```text
Compute
Storage
Network
Database
Messaging
Observability
Data Transfer
Requests
Licensing
Reserved Capacity
AI Inference
```

Architecture should make cost measurable.

---

# 60. Cost Is an Architectural Constraint

Example:

```text
Architecture A
10 VMs
High Control
High Operational Cost

Architecture B
Managed Containers
Lower Operational Cost
Different Platform Constraints
```

Neither is universally cheaper.

Cost must be measured under the actual workload.

---

# 61. Unit Economics

Instead of only asking:

```text
What is our monthly cloud bill?
```

measure:

```text
Cost per Request
Cost per Transaction
Cost per Customer
Cost per GB
Cost per Event
Cost per AI Task
```

For TransactionFlow:

```text
Cloud Cost
     /
Processed Transactions
```

This provides a more useful architectural metric.

---

# 62. Autoscaling and Cost

Autoscaling can reduce idle infrastructure.

But aggressive scaling may increase:

```text
Compute Cost
Startup Cost
Database Connections
Network Cost
Cold Start Frequency
```

Example:

```text
Queue Spike
   ↓
Autoscaling
   ↓
100 Workers
   ↓
Database Saturation
```

Scaling compute without scaling dependencies can worsen the system.

---

# 63. Cloud Cost and Architecture

Cost optimization should not simply mean:

```text
Use cheaper VM
```

Architecture-level optimization includes:

```text
Caching
Async Processing
Right-Sizing
Autoscaling
Managed Services
Data Lifecycle
Storage Tiers
Batch Processing
Efficient Queries
Reduced Network Traffic
Observability Sampling
```

---

# 64. Data Transfer Costs

Cloud network traffic can become an important cost driver.

Example:

```text
Region A
   ↓
Region B
   ↓
Large Data Transfer
```

or:

```text
Application
   ↓
Database
   ↓
Different Region
```

Architecture should consider:

```text
Data Locality
Replication
Region Placement
Payload Size
Caching
Compression
```

---

# 65. Disaster Recovery

Disaster recovery is broader than backups.

It includes:

```text
Backup
Replication
Failover
Recovery Procedures
Infrastructure Recreation
Data Restoration
Validation
Operational Runbooks
```

The goal is:

```text
Failure
  ↓
Recovery
  ↓
Known Target State
```

---

# 66. RPO

Recovery Point Objective:

> How much data loss can the business tolerate?

Example:

```text
RPO = 5 minutes
```

means the architecture should target no more than approximately five minutes of recoverable data loss under the defined disaster scenario.

RPO influences:

```text
Replication
Backup Frequency
CDC
Storage
Messaging
Database Architecture
```

---

# 67. RTO

Recovery Time Objective:

> How long can the service remain unavailable?

Example:

```text
RTO = 30 minutes
```

RTO influences:

```text
Automation
Failover
Infrastructure Readiness
Backup Restoration
Multi-Region Architecture
Operational Procedures
```

---

# 68. Backup vs Replication

Backup:

```text
Historical Recovery
```

Replication:

```text
Operational Continuity
```

Replication does not replace backup.

If corrupted data is replicated:

```text
Primary
  ↓
Corrupted Data
  ↓
Replica
  ↓
Corrupted Data
```

Therefore:

> Replication protects availability. Backup protects recovery from historical state corruption and certain forms of data loss.

---

# 69. Multi-Region Architecture

Possible architecture:

```text
                 Global Traffic
                       ↓
              ┌────────┴────────┐
              ↓                 ↓
          Region A           Region B
              ↓                 ↓
          Application        Application
              ↓                 ↓
             Data Replication
```

Questions:

```text
Active/Active or Active/Passive?
How is traffic routed?
How is data replicated?
What is the consistency model?
What happens during partition?
How is failback performed?
```

Multi-region introduces significant complexity.

---

# 70. Active/Passive

```text
Region A
   ↓
ACTIVE

Region B
   ↓
PASSIVE
```

Advantages:

```text
Simpler Consistency
Simpler Operations
Lower Cost
```

Trade-offs:

```text
Failover Time
Unused Capacity
Potentially Higher Recovery Complexity
```

---

# 71. Active/Active

```text
Region A
   ↓
ACTIVE

Region B
   ↓
ACTIVE
```

Advantages:

```text
Better Resource Utilization
Potentially Lower Failover Impact
Global Traffic Distribution
```

Costs:

```text
Data Consistency
Conflict Resolution
Operational Complexity
Network Traffic
Testing Complexity
```

Active/active should be justified by actual requirements.

---

# 72. Cloud-Native vs Cloud-Hosted

These concepts are different.

Cloud-hosted:

```text
Existing Application
       ↓
Azure VM
```

Cloud-native:

```text
Managed Compute
+
Managed Data
+
Elasticity
+
Automation
+
Observability
+
Infrastructure as Code
+
Failure-Aware Architecture
```

Cloud-native is primarily about architectural properties and operating model, not simply containers.

---

# 73. Cloud-Native Principles

Useful principles include:

```text
Automate Everything
Design for Failure
Prefer Stateless Compute
Externalize State
Use Managed Capabilities Carefully
Scale Horizontally
Observe Everything Important
Treat Infrastructure as Code
Make Deployments Reversible
Minimize Manual Operations
```

---

# 74. Platform Engineering

As systems grow, teams may create an internal platform.

Example:

```text
Application Teams
       ↓
Internal Platform
       ↓
Cloud Capabilities
```

The platform may provide:

```text
Deployment Templates
Identity
Observability
Networking
Secrets
Logging
Databases
Messaging
Security Policies
CI/CD
```

The goal is to reduce repeated infrastructure decisions.

---

# 75. Golden Paths

A golden path is a supported default architecture for common workloads.

Example:

```text
ASP.NET Core API
      ↓
Container Apps
      ↓
Managed Identity
      ↓
Key Vault
      ↓
PostgreSQL
      ↓
OpenTelemetry
      ↓
CI/CD
```

Golden paths should reduce cognitive and operational load without preventing justified exceptions.

---

# 76. Platform vs Application Responsibility

A healthy platform architecture separates responsibilities.

Platform:

```text
Networking
Identity Integration
Observability
Deployment
Security Guardrails
Infrastructure
```

Application:

```text
Business Logic
Domain Rules
Application State
Business Authorization
Data Semantics
```

Avoid putting business logic into infrastructure platforms.

---

# 77. Governance

Cloud governance includes:

```text
Naming
Tagging
Policies
Identity
Networking
Cost Controls
Resource Standards
Security Standards
Compliance
Lifecycle Management
```

Azure Policy can enforce organizational rules.

Examples:

```text
Require Tags
Restrict Regions
Require Encryption
Restrict Public Access
Allowed Resource Types
```

Governance should provide guardrails rather than creating unnecessary bureaucracy.

---

# 78. Policy as Code

Cloud architecture becomes stronger when rules are executable.

Example:

```text
Rule:
Production database must not be publicly accessible.
```

Instead of documentation only:

```text
Policy
   ↓
Automated Validation
   ↓
Deployment Block
```

This follows the same principle as architecture fitness functions:

> Architectural constraints should become executable whenever possible.

---

# 79. Cloud Security Architecture

Cloud security combines:

```text
Identity
+
Network
+
Data
+
Workload
+
Secrets
+
Supply Chain
+
Observability
+
Governance
```

A useful Azure-oriented architecture:

```text
Microsoft Entra ID
       ↓
Managed Identity
       ↓
Azure RBAC
       ↓
Private Network
       ↓
Workload
       ↓
Key Vault
       ↓
Managed Data
       ↓
Azure Monitor
```

---

# 80. Cloud Architecture and Distributed Systems

Cloud makes distribution easier.

That does not make distributed systems simpler.

Cloud systems still experience:

```text
Latency
Timeouts
Partial Failure
Retries
Duplicates
Network Partitions
Consistency Problems
Ordering Problems
Dependency Failures
```

The cloud provides infrastructure capabilities.

It does not eliminate distributed-system theory.

---

# 81. Cloud Architecture and Data Architecture

Cloud architecture must respect data ownership.

Example:

```text
Application
     ↓
PostgreSQL
```

If multiple services require the same data:

```text
Service A
Service B
Service C
```

do not automatically expose the database to all services.

Prefer:

```text
Data Owner
     ↓
API / Event
     ↓
Consumers
```

or:

```text
Data Owner
     ↓
Event
     ↓
Local Read Model
```

---

# 82. Cloud Architecture and Security Architecture

Cloud boundaries should reinforce security boundaries.

Example:

```text
Public Edge
    ↓
Private Application
    ↓
Private Data
```

Combined with:

```text
Identity
Authorization
Network Policy
Managed Identity
Key Vault
Audit
```

Cloud networking alone is not a security model.

---

# 83. Cloud Architecture and AI

AI workloads introduce additional cloud architecture requirements.

Examples:

```text
GPU Compute
Model Hosting
Inference Scaling
Vector Storage
RAG Data
Object Storage
Evaluation Pipelines
Agent Runtime
Sandboxing
Observability
Model Routing
Cost Control
```

Azure examples may include:

```text
Azure AI Foundry
Azure OpenAI
Azure Kubernetes Service
Azure Container Apps
Azure Machine Learning
Azure AI Search
Azure Blob Storage
Azure Database for PostgreSQL
Azure Monitor
```

The exact service choice should follow workload requirements and current platform capabilities.

---

# 84. AI Compute Architecture

AI workloads may require:

```text
CPU
GPU
Memory
High-Speed Storage
High Network Bandwidth
```

A typical inference architecture:

```text
Client
  ↓
API
  ↓
AI Gateway
  ↓
Model Router
  ↓
Model
  ↓
Tools / RAG
```

For larger models:

```text
Request
  ↓
Inference Service
  ↓
GPU Pool
```

Scaling GPU workloads requires careful consideration of:

```text
GPU Utilization
Memory
Batching
Queueing
Cold Start
Model Loading
Cost
Latency
```

---

# 85. AI Cost Architecture

AI cloud architecture must treat inference as an economic workload.

Measure:

```text
Cost per Request
Cost per Token
Cost per Agent Task
GPU Utilization
Model Latency
Context Size
Cache Hit Rate
Tool Calls
```

For agentic systems:

```text
User Request
    ↓
Agent
    ↓
LLM Call
    ↓
Tool
    ↓
LLM Call
    ↓
Tool
    ↓
LLM Call
```

The cost can grow with agent steps.

Therefore:

> Agent autonomy must have architectural cost boundaries.

---

# 86. AI Cloud Security

AI cloud architectures should include:

```text
Model Access Control
Data Isolation
Prompt Protection
Tool Authorization
Network Policy
Sandbox
Secret Isolation
Auditability
Human Gate
```

For example:

```text
Agent
  ↓
Permission Policy
  ↓
Sandbox
  ↓
Network Policy
  ↓
Tool
  ↓
External Resource
```

This connects directly to the Security Architecture fundamentals.

---

# 87. AI Blast Radius

A cloud-hosted agent should not automatically have:

```text
Subscription Owner
Production Database Admin
Unlimited Internet
All Customer Data
```

Instead:

```text
Agent
  ↓
Restricted Identity
  ↓
Restricted Resources
  ↓
Restricted Network
  ↓
Verified Actions
```

This minimizes blast radius.

---

# 88. Cloud Architecture Smells

## 88.1 Lift and Shift Without Analysis

```text
On-Premise VM
      ↓
Azure VM
```

without evaluating:

- scalability
- availability
- security
- operational burden
- cost

---

## 88.2 Service Explosion

Using dozens of managed services for a simple application.

Problem:

```text
More Services
    ↓
More Dependencies
    ↓
More Failure Modes
    ↓
More Operational Burden
```

---

## 88.3 Kubernetes by Default

```text
Application
    ↓
AKS
```

without a Kubernetes-specific requirement.

Problem:

```text
Operational Complexity
Platform Maintenance
Networking Complexity
Security Complexity
```

---

## 88.4 Public Everything

```text
Internet
   ↓
Public API
   ↓
Public Database
   ↓
Public Storage
```

This creates unnecessary attack surface.

---

## 88.5 Cloud Service Sprawl

Using many cloud services without clear ownership.

Consequences:

```text
Operational Burden
Cost Visibility Problems
Security Complexity
Vendor Coupling
```

---

## 88.6 Autoscaling Without Dependency Scaling

```text
API
 ↓
100 Instances
 ↓
Database
 ↓
Saturation
```

Scaling one layer can overload another.

---

## 88.7 Multi-Region Without a Business Requirement

Multi-region architecture can introduce:

```text
Complexity
Cost
Data Consistency Problems
Operational Burden
```

without providing meaningful business value.

---

## 88.8 No Cost Architecture

Treating cloud billing as an operations problem rather than an architectural constraint.

---

## 88.9 Manual Infrastructure

```text
Portal
 ↓
Click
 ↓
Production
```

without reproducibility.

This creates configuration drift and weak change control.

---

## 88.10 Managed-Service Cargo Cult

Using a managed service simply because it exists.

The correct question is:

> What architectural problem does this service solve?

---

# 89. Cloud Architecture Invariants

Useful cloud architecture invariants include:

```text
1. Production infrastructure is reproducible from version-controlled definitions.

2. Public exposure is explicit rather than accidental.

3. Sensitive data does not require public network access.

4. Workloads use least-privilege identities.

5. Production secrets are not embedded in application artifacts.

6. Critical workloads have defined RPO and RTO.

7. Failure domains are explicitly identified.

8. Health checks distinguish liveness from readiness.

9. Applications support graceful shutdown.

10. Cloud resources have explicit ownership.

11. Infrastructure changes are auditable.

12. Cost is measurable at meaningful business boundaries.

13. Autoscaling policies are based on workload behavior.

14. Scaling one component does not create uncontrolled dependency overload.

15. Disaster recovery procedures are tested.

16. Security policies are enforced automatically where practical.

17. Observability exists across distributed boundaries.

18. Critical cloud dependencies have documented failure behavior.

19. AI workloads have explicit resource and cost boundaries.

20. Agent workloads have explicit identity, network, and permission boundaries.
```

---

# 90. Cloud Fitness Functions

Cloud architecture can become executable.

Example:

```text
Rule:
Production PostgreSQL must not have public network access.
```

Automated check:

```text
Infrastructure Definition
        ↓
Policy Validation
        ↓
Build Fails
```

Another:

```text
Rule:
Production resources must have Owner and Environment tags.
```

Another:

```text
Rule:
Application workloads must use managed identity.
```

Another:

```text
Rule:
Public endpoints require explicit approval.
```

Another:

```text
Rule:
Production deployments must use verified artifacts.
```

Cloud architecture becomes stronger when these rules are executable.

---

# 91. Cloud Architecture Experiments

Recommended experiments:

## Compute

```text
01-AppServiceVsContainerApps
02-ContainerAppsVsAKS
03-ServerlessVsLongRunningWorker
04-VMVsManagedCompute
```

## Scaling

```text
05-Autoscaling
06-QueueBasedScaling
07-CPUScalingVsCustomMetricScaling
08-ScaleOutDependencyFailure
```

## Networking

```text
09-PublicVsPrivateDatabase
10-PrivateEndpoint
11-NetworkSegmentation
12-EgressControl
```

## Reliability

```text
13-ZoneFailure
14-RegionFailure
15-DependencyFailure
16-Failover
17-GracefulShutdown
```

## Data

```text
18-ManagedPostgreSQL
19-ReadReplica
20-BackupRecovery
21-DataReplication
```

## Messaging

```text
22-ServiceBusVsKafka
23-EventHubsVsKafka
24-QueueBackpressure
25-ConsumerScaling
```

## Cost

```text
26-ComputeCost
27-DatabaseCost
28-NetworkTransferCost
29-ObservabilityCost
30-AutoscalingCost
```

## AI

```text
31-GPUInferenceScaling
32-ModelRouting
33-AgentCostBoundaries
34-RAGInfrastructureCost
35-AgentSandbox
```

---

# 92. Cloud Failure Experiments

Important failure scenarios:

```text
Region Failure
Availability Zone Failure
Database Failure
Cache Failure
Message Broker Failure
Identity Service Failure
Key Vault Failure
Network Partition
DNS Failure
Expired Certificate
Credential Expiration
Deployment Failure
Configuration Drift
Autoscaling Failure
Dependency Saturation
Unexpected Cloud Cost Spike
GPU Capacity Exhaustion
AI Provider Failure
```

For each experiment:

```text
Failure
   ↓
Detection
   ↓
Blast Radius
   ↓
Failover
   ↓
Recovery
   ↓
Data Integrity
   ↓
User Impact
   ↓
Cost Impact
   ↓
Architectural Improvement
```

---

# 93. Cloud Performance Metrics

Measure:

### Compute

```text
CPU
Memory
Requests/sec
Concurrency
Startup Time
Scale-Out Time
Scale-In Time
```

### Application

```text
P50
P95
P99
Error Rate
Throughput
Queue Time
```

### Database

```text
Connections
CPU
IO
Lock Wait
Query Latency
Transaction Rate
```

### Messaging

```text
Throughput
Queue Depth
Consumer Lag
Retry Count
DLQ Count
```

### Network

```text
Bandwidth
Latency
Packet Loss
Cross-Region Traffic
Egress Volume
```

### Cost

```text
Cost/hour
Cost/day
Cost/request
Cost/transaction
Cost/customer
Cost/AI task
```

---

# 94. Cloud Cost Experiments

Use the same workload and compare architectures.

Example:

```text
TransactionFlow
1000 transactions/sec
```

Compare:

```text
Architecture A
App Service
PostgreSQL
Service Bus

Architecture B
Container Apps
PostgreSQL
Kafka

Architecture C
AKS
PostgreSQL
Kafka
```

Measure:

```text
Latency
Throughput
Availability
Operational Burden
Infrastructure Cost
Scaling Behavior
Failure Recovery
```

Do not conclude:

```text
Architecture X is cheaper.
```

Conclude:

```text
Under workload W,
with configuration C,
Architecture X produced measured cost M
and operational characteristics O.
```

---

# 95. Cloud Architecture and TransactionFlow

Target architecture:

```text
                         Internet
                            |
                            v
                  Azure Front Door / Gateway
                            |
                            v
                  ASP.NET Core API
                            |
                  +---------+---------+
                  |                   |
                  v                   v
          Transaction Domain       Auth
                  |
                  v
          Azure PostgreSQL
                  |
               Outbox
                  |
                  v
             Kafka / Event Hub
                  |
          +-------+-------+
          |               |
          v               v
      Consumer A       Consumer B
```

Supporting infrastructure:

```text
Microsoft Entra ID
Managed Identity
Key Vault
Azure Monitor
Application Insights
Private Networking
Infrastructure as Code
```

---

# 96. TransactionFlow Cloud Invariants

```text
1. API workloads are horizontally scalable.

2. Application instances are stateless.

3. PostgreSQL is the transactional source of truth.

4. Database access is private.

5. Services use managed identities where supported.

6. Secrets are not embedded in application configuration artifacts.

7. Transaction events are published through a reliable delivery mechanism.

8. Consumers are idempotent.

9. Queue or stream backlog cannot grow without detection.

10. Database saturation is observable.

11. Deployment can be rolled back safely.

12. Infrastructure is reproducible.

13. Production resources have ownership and environment metadata.

14. Critical data has defined RPO and RTO.

15. Recovery procedures are tested.

16. Security-sensitive actions are auditable.

17. Cloud cost can be associated with meaningful workload units.

18. Failure of one application instance does not cause transaction state loss.

19. Graceful shutdown prevents unnecessary duplicate work.

20. AI workloads cannot access TransactionFlow resources without explicit authorization.
```

---

# 97. Cloud Architecture Decision Framework

When selecting a cloud architecture, ask:

## Business

```text
What business capability are we supporting?
What availability does the business require?
What latency is acceptable?
What are the recovery requirements?
```

## Compute

```text
What type of workload is this?
Stateless?
Stateful?
Event-driven?
Long-running?
Bursting?
GPU?
```

## Data

```text
What is the source of truth?
What consistency is required?
What data must be local?
What can be eventually consistent?
```

## Networking

```text
What must be public?
What must be private?
What traffic is allowed?
What traffic must be blocked?
```

## Security

```text
Who can access what?
Which identities exist?
What is the blast radius?
```

## Reliability

```text
What failure domains must we survive?
What are RPO and RTO?
```

## Operations

```text
Who operates the system?
How much automation exists?
What is the operational burden?
```

## Cost

```text
What is the cost per business unit?
How does cost scale with traffic?
```

## Evolution

```text
Can we change the architecture safely?
How coupled are we to the cloud provider?
```

---

# 98. Cloud Architecture Trade-offs

Important trade-offs include:

```text
Control
    vs
Operational Simplicity
```

```text
Portability
    vs
Cloud-Native Capabilities
```

```text
Availability
    vs
Cost
```

```text
Performance
    vs
Cost
```

```text
Managed Services
    vs
Vendor Coupling
```

```text
Multi-Region
    vs
Complexity
```

```text
Kubernetes
    vs
Operational Burden
```

```text
Redundancy
    vs
Infrastructure Cost
```

```text
Security Isolation
    vs
Operational Complexity
```

```text
AI Autonomy
    vs
Control
```

---

# 99. Cloud Portability

Portability exists at different levels.

```text
Application Portability
Runtime Portability
Container Portability
Data Portability
Infrastructure Portability
Operational Portability
```

For example:

```text
.NET Application
   ↓
Container
   ↓
AKS
```

may be more portable at the compute layer than:

```text
.NET Application
   ↓
Azure-specific APIs
   ↓
Azure-only Architecture
```

But portability has a cost.

Avoid optimizing for theoretical portability when cloud-native capabilities provide meaningful business value.

---

# 100. Vendor Lock-In

Vendor coupling should be explicit.

Potential coupling points:

```text
Identity
Database
Messaging
Storage
AI Models
Observability
Infrastructure
Deployment
Networking
Serverless APIs
```

A useful architecture question is:

> Which dependencies would be expensive or difficult to replace?

Classify them:

```text
Easy to Replace
Moderate
Strategic
Very Difficult
```

Not all vendor lock-in is bad.

The question is:

> Is the value received worth the switching cost and strategic dependency?

---

# 101. Cloud Architecture and Reversibility

Architectural decisions differ in reversibility.

Easy to change:

```text
Application Configuration
Deployment Configuration
Compute Size
Scaling Threshold
```

Harder to change:

```text
Database Technology
Data Model
Messaging Semantics
Identity Architecture
Multi-Region Data Model
AI Provider Integration
```

Use ADRs for high-impact, difficult-to-reverse cloud decisions.

---

# 102. Cloud Architecture Maturity

A useful progression:

```text
Level 1
Cloud Hosted
```

```text
Level 2
Automated Infrastructure
```

```text
Level 3
Cloud-Native Operations
```

```text
Level 4
Highly Automated Platform
```

```text
Level 5
Evidence-Driven Cloud Architecture
```

At the highest level:

```text
Architecture
   ↓
Infrastructure as Code
   ↓
Automated Policies
   ↓
Observability
   ↓
Experiments
   ↓
Cost Measurement
   ↓
Failure Testing
   ↓
Continuous Evolution
```

---

# 103. The Senior Architect Mental Model

A junior cloud discussion often starts with:

```text
Which Azure service should we use?
```

A senior architect starts with:

```text
What are the business requirements?
```

Then:

```text
What quality attributes matter?
```

Then:

```text
What failure must we survive?
```

Then:

```text
What data and state exist?
```

Then:

```text
What trust boundaries exist?
```

Then:

```text
What must scale?
```

Then:

```text
What should be managed by the cloud provider?
```

Then:

```text
What is the operational burden?
```

Then:

```text
What is the cost model?
```

Then:

```text
How reversible is this decision?
```

Finally:

```text
How will we prove that the architecture works?
```

---

# 104. Cloud Architecture Reasoning Loop

A reusable model:

```text
Business Requirement
        ↓
Quality Attributes
        ↓
Workload Characteristics
        ↓
Failure Domains
        ↓
Data Architecture
        ↓
Security Boundaries
        ↓
Compute
        ↓
Networking
        ↓
Messaging
        ↓
Observability
        ↓
Deployment
        ↓
Cost
        ↓
Recovery
        ↓
Measurement
        ↓
Architecture Decision
```

---

# 105. Cloud Architecture Evidence Loop

Cloud architecture should be validated experimentally.

```text
Question
   ↓
Hypothesis
   ↓
Architecture
   ↓
Implementation
   ↓
Infrastructure as Code
   ↓
Workload
   ↓
Measurement
   ↓
Failure Experiment
   ↓
Cost Measurement
   ↓
Trade-off
   ↓
ADR
```

This connects Cloud Architecture directly to the `ArchitectureExperiments` part of the repository.

---

# 106. Definition of Done

A cloud architecture is not complete when:

```text
The application runs in Azure.
```

It is complete when:

```text
[ ] Compute model is intentional

[ ] Data architecture is defined

[ ] Network boundaries are explicit

[ ] Identity architecture is defined

[ ] Authorization is defined

[ ] Secrets are managed securely

[ ] Availability requirements are defined

[ ] Failure domains are understood

[ ] Scaling behavior is tested

[ ] Autoscaling has meaningful signals

[ ] Dependencies have failure behavior

[ ] Health checks are meaningful

[ ] Graceful shutdown is implemented

[ ] Infrastructure is reproducible

[ ] Configuration is externalized

[ ] Observability is implemented

[ ] Deployment strategy is defined

[ ] Rollback is possible

[ ] RPO is defined

[ ] RTO is defined

[ ] Disaster recovery is tested

[ ] Cost is measurable

[ ] Cloud governance is defined

[ ] Security policies are executable where possible

[ ] Vendor coupling is understood

[ ] Major cloud decisions have ADRs

[ ] Critical assumptions have experiments

[ ] AI workloads have explicit resource, security, and cost boundaries
```

---

# 107. Senior Architect Interview Questions

## Cloud Fundamentals

1. What makes an architecture cloud-native?

2. What is the difference between cloud-hosted and cloud-native?

3. When would you choose IaaS over PaaS?

4. When would you use a managed service?

5. What are the major trade-offs of managed services?

---

## Azure

6. How would you structure Azure subscriptions?

7. How would you design resource groups?

8. What are availability zones?

9. When would you use multiple regions?

10. What is the role of Microsoft Entra ID?

11. How does Managed Identity work?

12. How would you secure an Azure workload?

---

## Compute

13. App Service vs Container Apps vs AKS?

14. When is Kubernetes justified?

15. What makes a workload suitable for serverless?

16. How do you design stateless cloud applications?

17. How do you scale stateful workloads?

---

## Networking

18. How would you design a secure Azure network?

19. What is the difference between public and private endpoints?

20. How would you control egress?

21. What is the role of an API Gateway?

22. When would you use Azure Front Door?

---

## Reliability

23. How do you design for zone failure?

24. How do you design for regional failure?

25. What is the difference between RPO and RTO?

26. Backup vs replication?

27. Active/active vs active/passive?

---

## Cost

28. How would you measure cloud cost?

29. What is unit economics in cloud architecture?

30. How can autoscaling increase rather than decrease cost?

31. How can architecture reduce network costs?

---

## Operations

32. How do you prevent infrastructure drift?

33. Why is Infrastructure as Code important?

34. How do you design safe cloud deployments?

35. How do you implement observability across distributed services?

---

## Architecture

36. How do you evaluate cloud vendor lock-in?

37. How do you decide between portability and cloud-native capabilities?

38. How do you measure operational burden?

39. How do you validate a cloud architecture experimentally?

40. What cloud architecture decisions should become ADRs?

---

## AI

41. How would you architect GPU inference workloads?

42. How would you scale an LLM inference service?

43. How would you control AI inference costs?

44. How would you secure an autonomous agent running in Azure?

45. How would you isolate an agent from production infrastructure?

46. How would you design AI model failover?

47. How would you observe and evaluate agentic workloads in production?

---

# 108. Key Takeaways

1. Cloud architecture is not cloud service selection.

2. Start with business requirements and quality attributes.

3. Managed services reduce operational burden but may increase vendor coupling.

4. Cloud does not eliminate distributed-system problems.

5. Stateless compute simplifies scaling and recovery.

6. Stateful systems require explicit ownership, consistency, replication, and recovery design.

7. Availability zones and regions solve different failure-domain problems.

8. Multi-region architecture should be driven by business requirements, not fashion.

9. Autoscaling must consider the entire dependency graph.

10. Network architecture is a security and reliability boundary.

11. Private networking does not replace authorization.

12. Infrastructure as Code makes cloud architecture reproducible and testable.

13. Policy as Code turns governance requirements into executable constraints.

14. Observability is part of the architecture, not an afterthought.

15. RPO and RTO are business-driven architecture constraints.

16. Backup and replication solve different problems.

17. Cloud cost is an architectural property.

18. Unit economics is more useful than looking only at the total cloud bill.

19. Platform engineering can reduce repeated operational complexity.

20. Golden paths should provide safe defaults without eliminating architectural judgment.

21. Vendor lock-in is not automatically bad. It should be intentional and understood.

22. Cloud-native architecture is about properties and operating models, not simply containers.

23. AI workloads add GPU, inference, data, evaluation, security, and cost architecture concerns.

24. Agentic systems require explicit identity, permissions, sandboxing, network policies, verification, and cost boundaries.

25. Cloud architecture should be validated with measurement and failure experiments.

26. The central cloud architecture question is:

> **Given our business requirements, workload, failure domains, security constraints, operational capabilities, and cost limits, which cloud architecture provides the required properties with an acceptable level of complexity?**

````

### How this fits the Architecture Fundamentals sequence

The five areas now form a useful reasoning chain:

```text
01 Quality Attributes
        │
        │ What properties matter?
        ▼
02 Distributed Systems
        │
        │ What happens when components communicate
        │ across unreliable boundaries?
        ▼
03 Data Architecture
        │
        │ Where does state live and who owns it?
        ▼
04 Security Architecture
        │
        │ Who can access and change that state?
        ▼
05 Cloud Architecture
        │
        │ Where and how do we operate all of this?
        ▼
Architecture Decision
````
