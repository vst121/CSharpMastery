# Security Architecture

## 1. Essence

Security Architecture defines how a system protects:

- identities
- data
- services
- infrastructure
- communication
- business capabilities
- operational interfaces
- supply chains
- AI capabilities

Security is not a feature added after the architecture is complete.

It is an architectural property that influences:

- system boundaries
- trust boundaries
- data ownership
- communication protocols
- deployment topology
- identity models
- authorization
- storage
- observability
- failure handling
- operational procedures
- software delivery
- third-party dependencies

A useful definition is:

> Security Architecture is the deliberate design of trust boundaries, identities, permissions, protections, detection mechanisms, and recovery capabilities across the system.

The goal is not to eliminate all risk.

The goal is to make security risks:

- understood
- bounded
- observable
- enforceable
- recoverable
- continuously testable

A secure architecture should answer:

> Who is allowed to do what, to which resource, under which conditions, and how do we detect and recover when those assumptions fail?

---

# 2. Why Security Is an Architectural Concern

Security decisions often determine architecture.

For example:

```text
Authentication
    ↓
Identity Model
    ↓
Authorization Model
    ↓
Trust Boundaries
    ↓
Service Boundaries
    ↓
Network Boundaries
    ↓
Data Access
    ↓
Auditability
```

Changing one layer can affect the others.

For example:

```text
Shared Database
       ↓
Shared Credentials
       ↓
Broad Database Permissions
       ↓
Weak Service Isolation
       ↓
Large Blast Radius
```

Security architecture therefore cannot be reduced to:

```text
Add HTTPS
Add JWT
Add Firewall
Add Antivirus
```

Those are implementation mechanisms.

Architecture asks:

```text
What do we trust?
Where do we trust it?
What happens when that trust is violated?
How far can an attacker move?
What can they access?
How do we detect it?
How do we recover?
```

---

# 3. Security Objectives

A useful security architecture starts with explicit objectives.

The classical security objectives are:

```text
Confidentiality
Integrity
Availability
```

Often extended with:

```text
Authenticity
Authorization
Accountability
Non-repudiation
Privacy
Resilience
```

## Confidentiality

Prevent unauthorized access to information.

Examples:

- customer data
- payment information
- credentials
- tokens
- internal architecture
- personal information
- AI prompts and retrieved documents

---

## Integrity

Prevent unauthorized or undetected modification.

Examples:

```text
Transaction amount
Payment status
User permissions
Configuration
Audit records
Domain events
AI-generated actions
```

Integrity is especially important for transactional systems.

---

## Availability

Ensure authorized users and services can access required capabilities.

Security and availability often interact.

For example:

```text
Aggressive Rate Limiting
        ↓
Improved Abuse Protection
        ↓
Potential Availability Impact
```

Security decisions therefore require architectural trade-offs.

---

# 4. Threat Modeling

Threat modeling is the systematic identification of:

- assets
- actors
- trust boundaries
- attack surfaces
- threats
- vulnerabilities
- mitigations
- residual risks

A simple process:

```text
Assets
   ↓
Actors
   ↓
Trust Boundaries
   ↓
Attack Surface
   ↓
Threats
   ↓
Security Controls
   ↓
Residual Risk
```

A useful question is:

> What can an attacker control, influence, observe, modify, or cause the system to execute?

---

# 5. Assets

Security architecture starts with identifying what must be protected.

Typical assets include:

```text
User Identity
Credentials
Access Tokens
Customer Data
Financial Data
Business Rules
Transactions
Messages
Database
Encryption Keys
Secrets
Source Code
Build Artifacts
Cloud Resources
AI Models
Prompts
RAG Documents
Embeddings
Agent Tools
Audit Logs
```

Not all assets have the same security requirements.

Classify them by:

- confidentiality
- integrity
- availability
- regulatory sensitivity
- business criticality
- recovery requirements

---

# 6. Threat Actors

Security architecture should identify potential actors.

Examples:

```text
Unauthenticated Internet User
Authenticated User
Compromised User Account
Malicious Insider
Compromised Service
Compromised Dependency
External Attacker
Supply Chain Attacker
Compromised Cloud Credential
Automated Bot
Malicious Agent Tool
```

Do not assume:

```text
Internal = Trusted
External = Untrusted
```

Modern systems increasingly use:

> Never trust a request simply because it originated inside the network.

---

# 7. Trust Boundaries

A trust boundary is a location where security assumptions change.

Examples:

```text
Internet
   ↓
API Gateway
   ↓
Application
   ↓
Database
```

Each boundary represents a change in trust.

Other examples:

```text
Browser → Backend
Service A → Service B
Application → Database
Application → External API
Developer → Production
CI/CD → Cloud
Agent → Tool
Model → Application
Tenant A → Tenant B
```

Trust boundaries should be explicit.

---

# 8. Attack Surface

Attack surface is the collection of interfaces through which a system can be influenced or attacked.

Examples:

```text
HTTP APIs
gRPC endpoints
Message brokers
WebSockets
Admin APIs
Database ports
Management interfaces
Cloud APIs
CI/CD pipelines
Package dependencies
Container images
AI tools
MCP servers
File uploads
Webhooks
```

A useful architectural principle:

> Every exposed interface should have an explicit security model.

---

# 9. Identity

Identity answers:

> Who or what is making this request?

Identity can represent:

```text
Human User
Service
Device
Application
Workload
Agent
Administrator
External System
```

Modern architectures require more than human authentication.

For example:

```text
User Identity
      +
Service Identity
      +
Workload Identity
      +
Device Identity
      +
Agent Identity
```

---

# 10. Authentication

Authentication establishes identity.

Common mechanisms include:

```text
Password
MFA
OAuth 2.0
OpenID Connect
Certificates
Managed Identity
Workload Identity
API Keys
Signed Requests
mTLS
```

Architectural principle:

> Authentication establishes who the caller is. It does not establish what the caller is allowed to do.

That distinction is fundamental.

---

# 11. Authorization

Authorization determines:

> What is this identity allowed to do?

Common models:

```text
RBAC
ABAC
ReBAC
Policy-Based Authorization
Capability-Based Access
```

## RBAC

Role-Based Access Control:

```text
User
 ↓
Role
 ↓
Permissions
```

Example:

```text
PaymentOperator
    ↓
CreatePayment
ApprovePayment
ViewPayment
```

---

## ABAC

Attribute-Based Access Control:

```text
Subject
Resource
Action
Context
     ↓
Authorization Policy
```

Example:

```text
User.department == Payment
AND
Transaction.region == EU
AND
Action == Approve
```

---

## ReBAC

Relationship-Based Access Control:

```text
User
 ↓
Relationship
 ↓
Resource
```

Useful for:

- organizations
- teams
- projects
- documents
- hierarchical resources

---

# 12. Least Privilege

Every identity should receive only the permissions required for its responsibilities.

Instead of:

```text
Application
    ↓
Admin Database Account
```

prefer:

```text
Payment Service
    ↓
Payment Database Role
    ↓
Required Tables / Operations
```

Least privilege applies to:

- users
- services
- containers
- workloads
- CI/CD pipelines
- administrators
- AI agents
- tools

---

# 13. Permission Boundaries

Permissions should be constrained by architecture.

For example:

```text
Payment API
    ↓
Payment Service Identity
    ↓
Payment Database
```

The service should not automatically have:

```text
Customer Database
HR Database
Infrastructure Database
Production Administration
```

A permission boundary limits blast radius.

---

# 14. Zero Trust

Zero Trust is an architectural model based on continuously validating access rather than trusting network location.

Core ideas:

```text
Verify explicitly
Use least privilege
Assume breach
Continuously evaluate
Minimize implicit trust
```

Traditional model:

```text
Internet
   ↓
Firewall
   ↓
Trusted Internal Network
   ↓
Everything
```

Zero Trust:

```text
Identity
   +
Device / Workload
   +
Resource
   +
Action
   +
Context
   ↓
Authorization Decision
```

---

# 15. Service Identity

In distributed systems, service identity becomes critical.

Example:

```text
OrderService
      ↓
PaymentService
```

The PaymentService should know:

```text
Who called me?
Is this caller authenticated?
Is this caller authorized?
What operation is requested?
Under which context?
```

Avoid architecture such as:

```text
All services
     ↓
Shared API Key
```

This creates:

- excessive trust
- difficult rotation
- poor attribution
- large blast radius

---

# 16. Workload Identity

Modern cloud systems should prefer workload identities over long-lived credentials when possible.

Example:

```text
Application
    ↓
Managed Identity
    ↓
Azure Resource
```

Instead of:

```text
Application
    ↓
Hardcoded Client Secret
    ↓
Azure Resource
```

Benefits include:

- reduced secret exposure
- automatic credential management
- stronger attribution
- easier rotation
- smaller attack surface

---

# 17. Secrets Management

Secrets include:

```text
Passwords
API Keys
Certificates
Private Keys
Connection Strings
Tokens
Encryption Keys
Signing Keys
```

Do not store secrets in:

```text
Source Code
Git Repository
Dockerfile
Plain Configuration
Logs
Exception Messages
```

Prefer dedicated secret management systems.

Examples:

```text
Azure Key Vault
AWS Secrets Manager
HashiCorp Vault
Kubernetes Secrets + External Secret Management
```

---

# 18. Secret Rotation

Secrets should have a lifecycle:

```text
Create
  ↓
Distribute
  ↓
Use
  ↓
Rotate
  ↓
Revoke
  ↓
Destroy
```

Architecture should support rotation without requiring application redesign.

A system that cannot rotate credentials safely has an architectural security weakness.

---

# 19. Encryption

Encryption protects confidentiality.

Two major categories:

```text
Encryption in Transit
Encryption at Rest
```

---

## Encryption in Transit

Examples:

```text
HTTPS
TLS
mTLS
Secure Messaging
```

Protect communication between:

```text
Client → API
Service → Service
Application → Database
Producer → Kafka
Consumer → Kafka
```

---

## Encryption at Rest

Examples:

```text
Database Encryption
Disk Encryption
Object Storage Encryption
Backup Encryption
```

Sensitive data may require application-level or field-level encryption in addition to storage encryption.

---

# 20. Key Management

Encryption is only as strong as its key management.

Architecture should define:

```text
Key Generation
Key Storage
Key Access
Key Rotation
Key Versioning
Key Revocation
Key Recovery
Key Destruction
```

Keys should have stronger protection than the data they protect.

---

# 21. Data Security

Security architecture must follow data ownership.

For every sensitive dataset ask:

```text
Who owns it?
Who can read it?
Who can modify it?
Who can delete it?
Where is it stored?
Where is it replicated?
How long is it retained?
How is it encrypted?
How is access audited?
```

Data security therefore connects directly to:

```text
Data Architecture
Identity
Authorization
Governance
Compliance
Observability
Disaster Recovery
```

---

# 22. API Security

Every API should define:

```text
Authentication
Authorization
Input Validation
Rate Limiting
Request Size Limits
Output Filtering
Error Handling
Auditability
Versioning
Abuse Protection
```

A typical secure request path:

```text
Client
  ↓
TLS
  ↓
API Gateway
  ↓
Authentication
  ↓
Authorization
  ↓
Validation
  ↓
Rate Limiting
  ↓
Application
  ↓
Domain
  ↓
Data
```

---

# 23. Input Validation

Never assume external input is trustworthy.

Validate:

```text
Type
Length
Range
Format
Allowed Values
Encoding
Business Constraints
```

Validation should occur at appropriate boundaries.

Example:

```text
HTTP DTO Validation
        ↓
Application Validation
        ↓
Domain Invariants
        ↓
Persistence Constraints
```

Validation should not rely on a single defensive layer.

---

# 24. Output Security

Security is also about what the system returns.

Avoid leaking:

```text
Database Details
Stack Traces
Secrets
Internal IDs
Infrastructure Details
Authorization Metadata
Sensitive Personal Data
```

For example:

Bad:

```json
{
  "error": "SqlException: PostgreSQL connection failed..."
}
```

Better:

```json
{
  "error": {
    "code": "PAYMENT_UNAVAILABLE",
    "message": "The payment could not be processed."
  }
}
```

Detailed diagnostics belong in controlled logs, not public responses.

---

# 25. Rate Limiting

Rate limiting protects:

- availability
- infrastructure
- APIs
- databases
- downstream services

Possible dimensions:

```text
Per IP
Per User
Per Client
Per API Key
Per Tenant
Per Endpoint
Per Resource
```

Rate limiting should be aligned with business behavior.

For example:

```text
Login
    → strict

Payment Creation
    → controlled

Read Product Catalog
    → higher capacity
```

---

# 26. Network Security

Network security should enforce architecture rather than compensate for poor boundaries.

Controls include:

```text
Firewall
Network Segmentation
Private Networks
Security Groups
Network Policies
Ingress Controls
Egress Controls
Private Endpoints
mTLS
Service Mesh
```

A useful principle:

> If a service does not need network access, it should not have network access.

---

# 27. Egress Control

Ingress receives significant security attention.

Egress is equally important.

A compromised service with unrestricted outbound access can become a pivot point.

Example:

```text
Compromised Service
      ↓
Unrestricted Internet
      ↓
Credential Theft / Data Exfiltration / C2
```

Better:

```text
Service
   ↓
Network Policy
   ↓
Allowed Destinations
```

Egress policies are particularly important for:

- agents
- workers
- containers
- serverless workloads
- plugins
- MCP servers

---

# 28. Auditability

Security-sensitive actions should be auditable.

Examples:

```text
Login
Permission Changes
Administrative Actions
Data Access
Payment Approval
Credential Changes
Configuration Changes
Agent Tool Calls
Policy Overrides
Human Approvals
```

Audit events should answer:

```text
Who?
What?
When?
Where?
Against which resource?
From which context?
What was the result?
```

Useful fields:

```text
ActorId
ActorType
Action
Resource
Timestamp
TraceId
RequestId
CorrelationId
Result
Reason
Source
```

---

# 29. Logging vs Auditing

Application logs and audit logs are not identical.

Application logs:

```text
Debugging
Operations
Performance
Diagnostics
```

Audit logs:

```text
Accountability
Security Investigation
Compliance
Forensics
```

Audit logs should have stronger:

- integrity guarantees
- retention policies
- access controls
- monitoring
- tamper resistance

---

# 30. Security Observability

Security requires detection, not only prevention.

Useful signals include:

```text
Authentication Failures
Authorization Failures
Unusual Access
Privilege Changes
Token Anomalies
Network Anomalies
Data Export
Repeated Errors
Rate-Limit Violations
Suspicious Tool Calls
Unexpected Egress
```

A useful security architecture loop:

```text
Prevent
  ↓
Detect
  ↓
Investigate
  ↓
Contain
  ↓
Recover
  ↓
Learn
  ↓
Improve
```

---

# 31. Security and Resilience

Security failures can become reliability failures.

Example:

```text
Credential Compromise
       ↓
Unauthorized Requests
       ↓
Resource Exhaustion
       ↓
Database Saturation
       ↓
Service Degradation
```

Conversely, resilience mechanisms can create security problems.

Example:

```text
Automatic Retry
       ↓
Attacker Causes Repeated Failure
       ↓
Retry Storm
       ↓
Resource Exhaustion
```

Therefore:

> Security and resilience must be designed together.

---

# 32. Blast Radius

Blast radius describes how much of the system can be affected when a component, identity, or credential is compromised.

Example:

```text
One Shared Admin Credential
          ↓
All Services
          ↓
Entire Platform
```

versus:

```text
Service Identity
      ↓
One Service
      ↓
Limited Resources
```

Security architecture should minimize:

```text
Credential Blast Radius
Network Blast Radius
Data Blast Radius
Service Blast Radius
Tenant Blast Radius
Agent Blast Radius
```

---

# 33. Isolation

Isolation limits the consequences of failure or compromise.

Possible boundaries:

```text
Process
Container
VM
Namespace
Network
Database
Tenant
Service
Cloud Account / Subscription
Agent Sandbox
```

Isolation is especially important for untrusted or semi-trusted workloads.

---

# 34. Multi-Tenant Security

Multi-tenant systems require explicit tenant isolation.

Potential models:

```text
Shared Database
Shared Schema
Tenant Column
Separate Schema
Separate Database
Separate Account / Subscription
```

Security architecture must prevent:

```text
Tenant A
   ↓
Tenant B Data
```

Tenant identity must propagate through the entire request path.

For example:

```text
User
 ↓
API
 ↓
Application
 ↓
Repository
 ↓
Database Query
```

The tenant boundary should not depend solely on a UI filter.

---

# 35. Tenant Context

A secure architecture should make tenant context explicit.

For example:

```text
TenantId
UserId
Roles
Permissions
CorrelationId
```

Tenant information should be:

- validated
- authenticated
- authorized
- propagated
- audited

Never trust a client-provided tenant ID without validating ownership.

---

# 36. Supply Chain Security

Modern applications depend on:

```text
NuGet Packages
npm Packages
Docker Images
Base Images
GitHub Actions
Cloud Services
Third-Party APIs
AI Models
Model Packages
MCP Servers
Build Tools
```

Every dependency increases the trust surface.

Supply chain security includes:

```text
Dependency Pinning
Dependency Scanning
SBOM
Signed Artifacts
Image Scanning
Build Isolation
Protected CI/CD
Provenance
Artifact Verification
```

---

# 37. Software Supply Chain

A secure delivery pipeline should resemble:

```text
Source
  ↓
Review
  ↓
Build
  ↓
Test
  ↓
Security Scan
  ↓
Artifact
  ↓
Sign
  ↓
Verify
  ↓
Deploy
```

Production should execute verified artifacts rather than arbitrary build output.

---

# 38. CI/CD Security

CI/CD systems often have powerful credentials.

For example:

```text
CI Pipeline
    ↓
Cloud Credentials
    ↓
Production
```

If the pipeline is compromised, the attacker may gain production access.

Protect:

- pipeline definitions
- deployment credentials
- secrets
- artifact repositories
- environment approvals
- production deployment permissions

Use separate identities and permissions for:

```text
Build
Test
Staging
Production
```

---

# 39. Container Security

Containers should follow least privilege.

Prefer:

```text
Non-root User
Read-only Filesystem
Minimal Base Image
Dropped Linux Capabilities
Restricted Network Access
Resource Limits
Signed Images
Scanned Images
```

Avoid:

```text
Privileged Containers
Host Network
Host Filesystem
Unnecessary Capabilities
Root Processes
```

Container security is part of runtime architecture.

---

# 40. Kubernetes Security

Kubernetes introduces additional security boundaries:

```text
Cluster
Namespace
Service Account
Pod
Container
Network Policy
Secret
RBAC
Admission Policy
```

Security architecture should define:

```text
Who can deploy?
Who can read secrets?
Which pods can communicate?
Which workloads can access cloud resources?
Which workloads can access the Internet?
```

---

# 41. Cloud Security

Cloud security follows a shared-responsibility model.

Architects must distinguish:

```text
Cloud Provider Responsibility
        +
Customer Responsibility
```

Typical customer responsibilities include:

```text
Identity
Permissions
Data
Application Security
Network Configuration
Secrets
Workloads
Logging
Configuration
```

Moving to the cloud does not move security responsibility to the provider.

---

# 42. Security Architecture and Domain Architecture

Security must protect business invariants.

For example:

```text
Payment
```

may have:

```text
Create
Approve
Capture
Refund
Cancel
```

These actions should not automatically share the same permission.

Example:

```text
CreatePayment
ApprovePayment
RefundPayment
CancelPayment
```

Business authorization should therefore align with domain capabilities.

---

# 43. Security Architecture and Data Architecture

Security follows data ownership.

Example:

```text
PaymentService
      ↓
Payment Database
```

Only authorized identities should access payment data.

Avoid:

```text
All Services
      ↓
Shared Database
      ↓
Broad Permissions
```

Shared databases can therefore create both:

```text
Architectural Coupling
Security Coupling
```

---

# 44. Security Architecture and Distributed Systems

Distributed systems create additional security boundaries.

Every network hop creates questions:

```text
Who is calling?
How is identity propagated?
How is the request authenticated?
How is authorization evaluated?
How is the request traced?
How are credentials rotated?
What happens if identity infrastructure fails?
```

A request may look like:

```text
User
 ↓
API Gateway
 ↓
OrderService
 ↓
PaymentService
 ↓
Kafka
 ↓
PaymentWorker
 ↓
Database
```

Identity and authorization must remain meaningful across this entire path.

---

# 45. Identity Propagation

Identity propagation must be designed explicitly.

There is a difference between:

```text
User Identity
```

and:

```text
Service Identity
```

For example:

```text
User
  ↓
OrderService
  ↓
PaymentService
```

PaymentService may need to know:

```text
Calling Service = OrderService
Original User = User123
```

This is different from simply forwarding an arbitrary bearer token everywhere.

Architectural decisions should define:

- token audience
- token lifetime
- delegation
- service identity
- impersonation
- authorization context
- audit identity

---

# 46. Security Failure Modes

Common failure modes include:

```text
Broken Authentication
Broken Authorization
Privilege Escalation
Credential Leakage
Secret Leakage
Injection
Data Exposure
Cross-Tenant Access
Replay
Request Forgery
Supply Chain Compromise
Insufficient Logging
Unrestricted Egress
Excessive Permissions
Weak Isolation
```

---

# 47. Security Architecture Smells

## 47.1 Shared Admin Credentials

```text
Everything
   ↓
Admin Credential
```

Problem:

- large blast radius
- poor attribution
- difficult rotation

---

## 47.2 Trust Based on Network Location

```text
Internal Network = Trusted
```

Problem:

- compromised internal workloads become trusted
- lateral movement becomes easier

---

## 47.3 Authorization Only in the UI

```text
UI hides button
      ↓
API still accepts operation
```

Problem:

The security boundary is in the wrong place.

Authorization must be enforced at the actual resource/action boundary.

---

## 47.4 Shared Database Credentials

```text
All Services
     ↓
Same DB Account
```

Problem:

- no isolation
- weak attribution
- excessive privilege

---

## 47.5 Secrets in Configuration Repositories

```text
Git
 ↓
appsettings.json
 ↓
Production
```

Problem:

Git history can preserve leaked secrets permanently.

---

## 47.6 Unrestricted Service Egress

```text
Compromised Service
       ↓
Internet
```

Problem:

Large exfiltration and lateral movement surface.

---

## 47.7 No Audit Trail

```text
Unauthorized Action
        ↓
No Evidence
```

Problem:

Detection and investigation become difficult.

---

## 47.8 Security Through Obscurity

Examples:

```text
Hidden Endpoint
Random URL
Undocumented API
```

These may reduce accidental discovery but are not authorization mechanisms.

---

## 47.9 Over-Privileged Service

```text
One Service
   ↓
Everything
```

This creates a large blast radius.

---

# 48. Security Invariants

Security architecture should define explicit invariants.

Examples:

```text
1. Every protected resource requires authentication.

2. Authentication does not imply authorization.

3. Authorization is enforced at the resource boundary.

4. Service identities receive least privilege.

5. Production secrets are not stored in source control.

6. Sensitive communication uses authenticated encryption.

7. Administrative actions are auditable.

8. Tenant boundaries cannot be bypassed through client-controlled identifiers.

9. Compromise of one service must not automatically grant access to all services.

10. Production deployment requires verified artifacts.

11. Untrusted workloads have restricted network access.

12. Security-sensitive actions are observable.

13. Credentials can be rotated without application redesign.

14. Security failures fail closed where appropriate.

15. Recovery mechanisms do not bypass authorization.
```

These invariants should become automated tests wherever possible.

---

# 49. Security Fitness Functions

Security principles can become executable architecture rules.

Example:

```text
Rule:
No application service may use an administrator database role.
```

Possible architecture test:

```text
Services
    ↓
Inspect DB configuration
    ↓
Reject privileged roles
```

Another:

```text
Rule:
All public APIs require authentication.
```

Architecture test:

```text
Enumerate Endpoints
       ↓
Check Authorization Metadata
       ↓
Fail Build if Missing
```

The goal is:

> Security requirements should become executable constraints whenever possible.

---

# 50. Security Testing

Security testing operates at multiple levels.

```text
Unit Tests
Integration Tests
Architecture Tests
Contract Tests
Dependency Scanning
Static Analysis
Dynamic Testing
Penetration Testing
Threat Modeling
Chaos / Resilience Testing
Configuration Testing
```

---

# 51. Unit Security Tests

Test security-sensitive business rules.

Example:

```text
CanUserRefundPayment()
CanUserApprovePayment()
CanUserAccessTenant()
CanServiceModifyTransaction()
```

These tests protect authorization behavior.

---

# 52. Integration Security Tests

Verify real boundaries.

Examples:

```text
API → Authentication
API → Authorization
Service → Database
Service → Kafka
Service → External API
Tenant → Data Isolation
```

Use realistic infrastructure where possible.

For .NET:

```text
Testcontainers
ASP.NET Core Integration Tests
PostgreSQL
Kafka
```

---

# 53. Architecture Security Tests

Architecture tests should verify structural rules.

Examples:

```text
Infrastructure layer cannot bypass authorization boundary.

PaymentService cannot reference HR infrastructure.

Public APIs must define authorization policy.

Services cannot share privileged database credentials.

Sensitive modules cannot expose internal persistence models.
```

---

# 54. Failure Experiments

Security should be tested under failure and compromise scenarios.

Examples:

```text
Compromised Service Identity
Expired Token
Revoked Credential
Stolen API Key
Unauthorized Tenant Access
Database Credential Compromise
Malicious Dependency
Compromised Container
Untrusted Agent Tool
Unexpected Network Egress
```

Measure:

```text
Detection Time
Blast Radius
Containment Time
Recovery Time
Unauthorized Actions
Affected Resources
Audit Completeness
```

---

# 55. Security and Availability

Security controls themselves can become failure points.

Example:

```text
Authorization Service
       ↓
Unavailable
       ↓
All Requests Fail
```

Architecture must define the failure policy.

Possible behaviors:

```text
Fail Closed
Fail Open
Cached Authorization
Graceful Degradation
Emergency Break-Glass Access
```

The correct choice depends on the resource.

For example:

```text
Payment Approval
    → likely fail closed

Public Product Catalog
    → may tolerate different behavior
```

Security failure behavior is an architectural decision.

---

# 56. Break-Glass Access

Critical systems sometimes require emergency access.

Break-glass access should be:

```text
Rare
Explicit
Time Limited
Strongly Authenticated
Audited
Monitored
Revocable
```

Example:

```text
Production Incident
      ↓
Emergency Identity
      ↓
Temporary Elevated Access
      ↓
Operation
      ↓
Automatic Expiration
      ↓
Audit Review
```

Break-glass must not become the normal operating model.

---

# 57. Security and Observability Correlation

Security events should connect to operational telemetry.

Useful identifiers:

```text
TraceId
SpanId
RequestId
CorrelationId
ActorId
TenantId
ServiceId
MessageId
```

Example:

```text
Unauthorized Payment Approval
        ↓
Audit Event
        ↓
TraceId
        ↓
Distributed Trace
        ↓
Service Logs
        ↓
Database Access
```

This enables investigation across architectural boundaries.

---

# 58. AI-Native Security Architecture

AI systems introduce new security boundaries.

Typical architecture:

```text
User
 ↓
Application
 ↓
Agent
 ↓
Model
 ↓
Context
 ↓
Tools
 ↓
External Systems
```

Every arrow creates a security question.

---

# 59. AI Agent Identity

An agent should not automatically inherit unrestricted user permissions.

For example:

```text
User
  ↓
Agent
  ↓
Tool
  ↓
Production Database
```

This can create excessive agency.

Prefer:

```text
User
  ↓
Agent
  ↓
Policy
  ↓
Allowed Tool
  ↓
Restricted Resource
```

---

# 60. Agent Permission Policy

Agent permissions should be explicit.

Example:

```text
Agent
 ├── read_customer
 ├── create_draft
 ├── search_documents
 └── request_human_approval
```

Avoid:

```text
Agent
 └── full_system_access
```

Permissions should consider:

```text
Tool
Action
Resource
User
Tenant
Context
Risk
Environment
```

---

# 61. Agent Sandbox

Agents executing code or manipulating files should operate inside controlled environments.

A sandbox may restrict:

```text
Filesystem
Network
Processes
Environment Variables
Credentials
CPU
Memory
Execution Time
Installed Tools
```

Example:

```text
Agent
  ↓
Sandbox
  ├── Restricted Filesystem
  ├── Restricted Network
  ├── Limited CPU
  ├── Limited Memory
  └── No Production Credentials
```

---

# 62. AI Network Policy

Agentic systems require explicit egress policies.

Example:

```text
Agent
  ↓
Network Policy
  ├── api.example.com
  ├── internal-search
  └── package registry
```

Everything else:

```text
DENY
```

This limits:

- data exfiltration
- malicious tool behavior
- unauthorized API access
- accidental external communication

---

# 63. Tool Security

Tools are privileged capabilities.

Examples:

```text
Database Query
Email
File System
Shell
Browser
Payment API
Cloud API
GitHub
MCP Server
```

Treat tools as security boundaries.

A tool should define:

```text
Identity
Permissions
Input Schema
Output Schema
Allowed Resources
Rate Limits
Audit Events
Timeout
Failure Policy
```

---

# 64. Verification

AI-generated actions should not automatically become trusted actions.

A useful architecture:

```text
Agent Decision
      ↓
Policy Check
      ↓
Verification
      ↓
Human Gate if Required
      ↓
Execution
      ↓
Audit
```

This separates:

```text
Intelligence
```

from:

```text
Authority
```

A model can recommend an action without automatically having permission to perform it.

---

# 65. Human Gate

High-risk actions may require human approval.

Examples:

```text
Payment
Production Deployment
Data Deletion
Permission Changes
External Communication
Legal Commitments
Customer Account Changes
```

Architecture:

```text
Agent
 ↓
Action Proposal
 ↓
Risk Classification
 ↓
Human Approval
 ↓
Execution
```

Human approval should itself be auditable.

---

# 66. Prompt Injection

AI systems can receive untrusted instructions through:

```text
Documents
Web Pages
Emails
Tickets
User Input
Retrieved Content
Tool Responses
```

A retrieved document may contain instructions such as:

```text
Ignore previous instructions...
```

The architecture should not treat retrieved content as trusted policy.

Separate:

```text
System Policy
Application Policy
User Input
Retrieved Content
Tool Output
```

Trust boundaries must remain explicit.

---

# 67. AI Data Exfiltration

AI systems can accidentally expose sensitive information through:

```text
Prompt Context
RAG Retrieval
Agent Memory
Tool Calls
Logs
Model Outputs
Telemetry
```

Security architecture should define:

```text
What data may enter the model?
What data may leave the system?
Which tools may receive sensitive data?
Which data may be stored?
How long is it retained?
```

---

# 68. AI Memory Security

Memory introduces additional persistence.

Examples:

```text
Conversation Memory
User Preferences
Agent State
Vector Store
Long-Term Memory
Task History
```

Memory requires:

```text
Ownership
Authorization
Tenant Isolation
Retention
Deletion
Auditability
Provenance
```

Memory should not become an uncontrolled secondary database.

---

# 69. AI Security Principle

A useful principle for AI-native architecture is:

> AI provides intelligence, while the application remains responsible for authority, state, policy, and security.

Therefore:

```text
LLM
 ↓
Recommendation
 ↓
Application Policy
 ↓
Authorization
 ↓
Verification
 ↓
Execution
```

Not:

```text
LLM
 ↓
Everything
```

---

# 70. Security Architecture and Blast Radius

Security architecture should continuously ask:

```text
If this identity is compromised,
what can it access?

If this service is compromised,
what can it call?

If this database credential is stolen,
what data becomes available?

If this agent is manipulated,
what actions can it execute?

If this tenant is compromised,
what other tenants can be affected?
```

This turns security from a checklist into architectural reasoning.

---

# 71. Security Architecture Metrics

Useful metrics include:

### Identity

```text
Authentication Failure Rate
MFA Coverage
Privileged Accounts
Inactive Accounts
Credential Age
Credential Rotation Success
```

### Authorization

```text
Authorization Failures
Privilege Escalation Attempts
Policy Violations
Excessive Permission Findings
```

### Runtime

```text
Blocked Requests
Rate-Limit Violations
Unexpected Egress
Suspicious Requests
Security Exceptions
```

### Detection

```text
MTTD
MTTR
Incident Count
False Positive Rate
Audit Coverage
```

### Supply Chain

```text
Vulnerable Dependencies
Critical CVEs
Unsigned Artifacts
Unverified Images
Dependency Age
```

### AI

```text
Tool Policy Violations
Blocked Tool Calls
Human Approval Rate
Prompt Injection Detection
Unauthorized Data Retrieval
Agent Sandbox Violations
Unexpected Egress
```

---

# 72. Security Architecture Comparison Dimensions

When comparing security architectures, evaluate:

```text
Confidentiality
Integrity
Availability
Blast Radius
Isolation
Least Privilege
Auditability
Operational Complexity
Performance Overhead
Developer Experience
Deployment Complexity
Recovery Capability
Scalability
Cost
Compliance Impact
```

Avoid statements such as:

```text
Architecture A is more secure.
```

Instead ask:

```text
More secure against which threat?
Under which assumptions?
At what operational cost?
With what residual risk?
```

Security is always contextual.

---

# 73. Modern .NET Mapping

Modern .NET provides strong security primitives.

Typical stack:

```text
ASP.NET Core
ASP.NET Core Identity
OpenID Connect
OAuth 2.0
JWT Bearer Authentication
Policy-Based Authorization
Claims
Data Protection
HTTPS / TLS
gRPC Security
Rate Limiting Middleware
Health Checks
Dependency Injection
Configuration Providers
Azure Managed Identity
Azure Key Vault
OpenTelemetry
```

Typical architecture:

```text
ASP.NET Core
    ↓
Authentication Middleware
    ↓
Authorization Middleware
    ↓
Application Layer
    ↓
Domain Layer
    ↓
Infrastructure
```

---

# 74. .NET Authorization Example

Prefer policy-based authorization for complex rules.

Conceptually:

```csharp
[Authorize(Policy = "CanApprovePayment")]
public async Task<IActionResult> Approve(...)
{
    ...
}
```

For domain-sensitive rules, authorization may additionally require:

```text
User
Tenant
Resource
Action
Business State
```

For example:

```text
CanApprovePayment
```

may depend on:

```text
Role
+
Tenant
+
Payment State
+
Approval Limit
+
Segregation of Duties
```

Authorization therefore may cross infrastructure and domain concerns.

---

# 75. Security Architecture for TransactionFlow

TransactionFlow:

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

Security boundaries:

```text
Internet
   ↓
API
   ↓
Application
   ↓
Database
   ↓
Message Broker
   ↓
Consumers
```

---

# 76. TransactionFlow Security Model

Possible identities:

```text
Customer
PaymentOperator
TransactionService
KafkaProducer
KafkaConsumer
DatabaseIdentity
Administrator
```

Permissions should be separated.

Example:

```text
Customer
 ├── CreateTransaction
 └── ViewOwnTransaction

PaymentOperator
 ├── ViewTransaction
 └── ApproveTransaction

TransactionService
 ├── ReadTransaction
 ├── WriteTransaction
 └── PublishTransactionEvent

KafkaConsumer
 └── ConsumeTransactionEvents
```

---

# 77. TransactionFlow Security Invariants

Example invariants:

```text
1. Every transaction mutation requires authentication.

2. A customer can access only transactions belonging to the authorized tenant/account.

3. Transaction approval requires explicit authorization.

4. TransactionService cannot access unrelated databases.

5. Kafka credentials are scoped to required topics.

6. Database credentials are scoped to required operations.

7. Production secrets are externalized.

8. All transaction mutations are auditable.

9. Sensitive data is not written to application logs.

10. Duplicate requests cannot bypass authorization.

11. Administrative operations require stronger authentication.

12. Security events contain TraceId and ActorId.

13. Consumers cannot publish to topics they do not own.

14. Compromise of one consumer does not grant access to the complete Kafka cluster.

15. High-risk transaction operations can require human approval.
```

---

# 78. TransactionFlow Threat Model

Example:

```text
Threat:
Compromised API Credential

Attacker
   ↓
API
   ↓
Transaction Creation
```

Questions:

```text
Can the attacker create transactions?
Can they approve transactions?
Can they access other tenants?
Can they access customer data?
Can they publish Kafka events?
Can they access the database directly?
Can they escalate privileges?
```

Controls:

```text
Authentication
Authorization
Least Privilege
Tenant Isolation
Rate Limiting
Audit
Database Constraints
Kafka ACLs
Network Policies
```

---

# 79. Security Failure Experiment

Experiment:

```text
Compromised TransactionService Identity
```

Hypothesis:

> Compromising TransactionService should not provide unrestricted access to unrelated services or infrastructure.

Test:

```text
Compromise Service Credential
        ↓
Attempt Database Access
        ↓
Attempt Kafka Access
        ↓
Attempt Other Service Access
        ↓
Attempt Cloud Resource Access
        ↓
Measure Blast Radius
```

Measure:

```text
Accessible Resources
Unauthorized Operations
Detection Time
Containment Time
Audit Completeness
Recovery Time
```

---

# 80. Security Architecture Experiments

Recommended experiments:

### Identity

```text
01-AuthenticationBoundary
02-ServiceIdentity
03-WorkloadIdentity
04-TokenPropagation
```

### Authorization

```text
05-RBAC
06-ABAC
07-PolicyBasedAuthorization
08-TenantIsolation
```

### Secrets

```text
09-SecretRotation
10-ManagedIdentity
11-SecretLeakDetection
```

### Network

```text
12-NetworkSegmentation
13-EgressControl
14-ServiceToServiceSecurity
```

### Runtime

```text
15-ContainerIsolation
16-ResourceLimits
17-RateLimiting
18-CredentialCompromise
```

### Supply Chain

```text
19-DependencyScanning
20-ContainerImageVerification
21-SignedArtifacts
22-SBOM
```

### Observability

```text
23-SecurityAudit
24-SecurityEventCorrelation
25-IncidentDetection
```

### AI Security

```text
26-AgentPermissionPolicy
27-AgentSandbox
28-AgentNetworkPolicy
29-PromptInjection
30-ToolAuthorization
31-HumanGate
32-AgentDataIsolation
33-AgentBlastRadius
```

---

# 81. Security Failure Experiments

Important failure scenarios:

```text
Expired Access Token
Revoked Credential
Compromised Service Identity
Leaked API Key
Privilege Escalation
Cross-Tenant Access
Database Credential Compromise
Unauthorized Kafka Producer
Malicious Dependency
Compromised Container
Unrestricted Egress
Audit Pipeline Failure
Authorization Service Failure
Agent Tool Abuse
Prompt Injection
Agent Data Exfiltration
```

For each experiment:

```text
Threat
   ↓
Security Assumption
   ↓
Failure Injection
   ↓
Observed Behavior
   ↓
Blast Radius
   ↓
Detection
   ↓
Containment
   ↓
Recovery
   ↓
Architectural Improvement
```

---

# 82. Security Architecture and ADRs

Important security decisions should be recorded as ADRs.

Recommended TransactionFlow ADRs:

```text
ADR-011 Authentication Model
ADR-012 Authorization Model
ADR-013 Service Identity Strategy
ADR-014 Secret Management
ADR-015 Database Access Permissions
ADR-016 Kafka Access Control
ADR-017 Tenant Isolation
ADR-018 Audit Logging
ADR-019 Network Segmentation
ADR-020 Production Break-Glass Access
ADR-021 AI Agent Permission Policy
ADR-022 Agent Sandbox Strategy
ADR-023 AI Tool Authorization
```

Each ADR should document:

```text
Context
Threat Model
Decision
Alternatives
Trade-offs
Security Assumptions
Residual Risk
Evidence
Revisit Conditions
```

---

# 83. Security Architecture Decision Framework

When evaluating a security architecture, ask:

### Identity

```text
Who is calling?
How is identity established?
How is identity propagated?
```

### Authorization

```text
What can they do?
On which resources?
Under which conditions?
```

### Trust

```text
Where are trust boundaries?
What assumptions exist at each boundary?
```

### Data

```text
What data is exposed?
Where is it stored?
Who owns it?
```

### Network

```text
What can communicate?
What cannot communicate?
What can leave the system?
```

### Runtime

```text
What happens if a component is compromised?
```

### Detection

```text
How do we know something went wrong?
```

### Recovery

```text
How quickly can we contain and recover?
```

### Evolution

```text
Can security controls evolve without redesigning the system?
```

---

# 84. Security Architecture Review Checklist

Before production:

## Identity

```text
[ ] Human authentication defined
[ ] Service identity defined
[ ] Workload identity defined
[ ] MFA considered where appropriate
[ ] Credential rotation supported
```

## Authorization

```text
[ ] Least privilege applied
[ ] Resource-level authorization enforced
[ ] Tenant isolation verified
[ ] Privileged operations separated
[ ] Break-glass process defined
```

## Data

```text
[ ] Sensitive data classified
[ ] Encryption defined
[ ] Data access audited
[ ] Retention defined
[ ] Deletion defined
```

## Network

```text
[ ] Trust boundaries documented
[ ] Ingress controlled
[ ] Egress controlled
[ ] Service communication authenticated
[ ] Unnecessary network paths removed
```

## Secrets

```text
[ ] Secrets externalized
[ ] Rotation supported
[ ] Production credentials separated
[ ] Secret access audited
```

## Runtime

```text
[ ] Containers run with least privilege
[ ] Resource limits configured
[ ] Isolation boundaries defined
[ ] Security policies enforced
```

## Supply Chain

```text
[ ] Dependencies scanned
[ ] Images scanned
[ ] Artifacts verified
[ ] SBOM available
[ ] CI/CD permissions restricted
```

## Observability

```text
[ ] Security events logged
[ ] Audit trail implemented
[ ] Trace correlation available
[ ] Alerts configured
[ ] Incident response tested
```

## AI

```text
[ ] Agent identity defined
[ ] Tool permissions restricted
[ ] Sandbox defined
[ ] Network policy defined
[ ] Sensitive data boundaries defined
[ ] Prompt injection considered
[ ] Verification implemented
[ ] Human gate defined for high-risk actions
[ ] Agent actions audited
```

---

# 85. Definition of Done

A security architecture is not complete when:

```text
HTTPS is enabled
JWT exists
Firewall exists
```

It is complete when:

```text
[ ] Assets are identified

[ ] Threat model exists

[ ] Trust boundaries are explicit

[ ] Identity model is defined

[ ] Authorization model is defined

[ ] Least privilege is enforced

[ ] Secrets are externally managed

[ ] Encryption requirements are defined

[ ] Network boundaries are defined

[ ] Egress is controlled

[ ] Sensitive data is classified

[ ] Security-sensitive operations are auditable

[ ] Security events are observable

[ ] Failure behavior is defined

[ ] Blast radius is understood

[ ] Recovery mechanisms exist

[ ] Security invariants are automated where possible

[ ] Supply chain controls exist

[ ] Security testing exists

[ ] Incident response has been exercised

[ ] Security ADRs document major decisions

[ ] AI capabilities have explicit permission and isolation boundaries
```

---

# 86. Senior Architect Interview Questions

## Fundamentals

1. What makes security an architectural concern?

2. What is a trust boundary?

3. What is the difference between authentication and authorization?

4. What does least privilege mean architecturally?

5. What does Zero Trust change compared with traditional network security?

---

## Distributed Systems

6. How do you authenticate service-to-service communication?

7. How do you propagate identity across services?

8. How do you prevent one compromised service from compromising the entire system?

9. How do network segmentation and service identity complement each other?

10. How do you secure asynchronous messaging?

---

## Data

11. How do you protect sensitive data across replicas and caches?

12. How do you design tenant isolation?

13. When is field-level encryption necessary?

14. How do you audit sensitive data access?

15. How do you handle encryption key rotation?

---

## Cloud

16. How would you design workload identity in Azure?

17. How do you protect CI/CD credentials?

18. How do you secure containerized workloads?

19. How do you control cloud resource permissions?

20. How do you design cloud network boundaries?

---

## Architecture

21. How do you measure blast radius?

22. How do you design for credential compromise?

23. How do you design security failure behavior?

24. When should a system fail closed?

25. When might cached authorization be acceptable?

---

## AI

26. How do you secure an autonomous agent?

27. How should an agent receive permissions?

28. How do sandbox and network policies reduce agent risk?

29. How do you prevent prompt injection from becoming an authorization bypass?

30. Should an LLM ever directly control a production system?

31. How would you design human approval for high-risk AI actions?

32. How do you audit agent tool usage?

33. How do you limit an agent's blast radius?

---

# 87. The Senior Architect Mental Model

A junior security discussion often starts with:

```text
Which security technology should we use?
```

A senior architect starts with:

```text
What are we protecting?
```

Then:

```text
Who can access it?
```

Then:

```text
What can they do?
```

Then:

```text
What do we trust?
```

Then:

```text
What happens if that trust is violated?
```

Then:

```text
How large is the blast radius?
```

Then:

```text
How do we detect it?
```

Then:

```text
How do we contain it?
```

Then:

```text
How do we recover?
```

Finally:

```text
How do we make the security property executable and continuously verifiable?
```

This is the difference between security configuration and security architecture.

---

# 88. Security Architecture Reasoning Loop

A reusable reasoning model:

```text
Asset
  ↓
Threat
  ↓
Trust Boundary
  ↓
Identity
  ↓
Authorization
  ↓
Least Privilege
  ↓
Isolation
  ↓
Protection
  ↓
Detection
  ↓
Containment
  ↓
Recovery
  ↓
Measurement
  ↓
Architecture Decision
```

---

# 89. Security Architecture and Evidence

Security decisions should be evidence-driven.

Do not simply state:

```text
This architecture is secure.
```

Instead establish:

```text
Threat Model
       ↓
Security Invariant
       ↓
Security Control
       ↓
Automated Test
       ↓
Failure Experiment
       ↓
Observed Blast Radius
       ↓
Detection
       ↓
Recovery
       ↓
Evidence
```

Security architecture therefore fits directly into the broader Architecture Lab methodology:

```text
Problem
   ↓
Threat Model
   ↓
Architecture
   ↓
Security Invariants
   ↓
Implementation
   ↓
Security Tests
   ↓
Failure Experiment
   ↓
Measurement
   ↓
Trade-offs
   ↓
ADR
```

---

# 90. Key Takeaways

1. Security is an architectural property, not a final checklist.

2. Start with assets, threats, trust boundaries, and attack surfaces.

3. Authentication establishes identity. Authorization establishes permission.

4. Least privilege should apply to users, services, workloads, pipelines, and AI agents.

5. Zero Trust minimizes implicit trust.

6. Network location should not be the primary security boundary.

7. Service identity and workload identity are fundamental to distributed systems.

8. Secrets require lifecycle management, not simply secure storage.

9. Encryption requires proper key management.

10. Data security must follow data ownership and classification.

11. Authorization must be enforced at the actual resource boundary.

12. Egress control is as important as ingress control.

13. Auditability is different from ordinary application logging.

14. Security and resilience are interconnected.

15. Blast radius is one of the most useful architectural security concepts.

16. Isolation limits the consequences of compromise.

17. Supply chain security is part of application architecture.

18. Security controls should become executable architecture invariants wherever possible.

19. Security must be tested under failure and compromise, not only normal operation.

20. AI systems introduce new trust boundaries around models, context, memory, tools, agents, and external systems.

21. AI agents should receive explicit permissions rather than unrestricted authority.

22. Sandboxes and network policies are architectural controls for agentic systems.

23. Verification and human gates separate AI intelligence from system authority.

24. A secure architecture should make compromise containable, observable, and recoverable.

25. The central security architecture question is:

> **If this component, identity, credential, dependency, or agent is compromised, what is the maximum damage it can cause, how will we detect it, and how will we recover?**
