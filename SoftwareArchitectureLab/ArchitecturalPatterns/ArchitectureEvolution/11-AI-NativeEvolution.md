# 11 - AI-Native Evolution

## 1. Essence

**AI-Native Evolution** is an architecture evolution strategy for progressively introducing AI capabilities into an existing software system while preserving control, reliability, security, observability, and business continuity.

The evolution is not:

```text
Traditional Application
        ↓
Add LLM
        ↓
AI-Native System
```

It is a controlled architectural transition:

```text
Traditional Application
        ↓
AI-Assisted Capability
        ↓
AI Decision Support
        ↓
Tool-Using AI
        ↓
Bounded Agent
        ↓
Workflow Agent
        ↓
Multi-Agent / Autonomous Capability
```

Each stage introduces additional architectural responsibilities.

> **AI-native evolution is the controlled transfer of intelligence from deterministic software components toward probabilistic AI components without transferring control of the system itself.**

---

## 2. The Problem

Existing software systems were usually designed around deterministic behavior:

```text
Input
  ↓
Business Logic
  ↓
Database
  ↓
Output
```

AI introduces new architectural characteristics:

```text
Probabilistic Output
Context
Model Selection
Prompt / Instructions
Tool Use
Memory
Retrieval
Evaluation
Non-Determinism
Policy
Human Oversight
```

Simply inserting an LLM into an existing system does not make the architecture AI-native.

The architecture must evolve around the new properties of AI.

---

## 3. Why AI-Native Evolution Is Different

Traditional architecture evolution usually changes:

```text
Deployment
Data
Communication
Boundaries
Infrastructure
```

AI-native evolution also changes:

```text
Decision Making
Context
Execution
Verification
Human Interaction
Evaluation
Runtime Adaptation
```

The system therefore needs explicit boundaries around probabilistic behavior.

---

## 4. Intent

Introduce AI capabilities incrementally while preserving deterministic control.

The fundamental principle is:

```text
Application owns:
    State
    Policy
    Control
    Permissions
    Security
    Business Invariants

AI provides:
    Intelligence
    Interpretation
    Generation
    Reasoning
    Recommendations
```

The AI should not silently become the system of record or the authority for unrestricted side effects.

---

## 5. Core Principles

### 5.1 AI Provides Intelligence, Not Application Control

Prefer:

```text
Application
    ↓
AI Capability
    ↓
Structured Result
    ↓
Application Validation
    ↓
Action
```

over:

```text
Application
    ↓
LLM
    ↓
Anything the LLM decides
```

---

### 5.2 Application Owns State

Business state should remain in deterministic application components.

For example:

```text
Order.Status
Payment.Status
User.Role
Workflow.State
Transaction.State
```

should not exist only inside an LLM conversation.

AI may interpret or recommend state transitions.

The application validates and commits them.

---

### 5.3 Introduce AI at the Smallest Useful Boundary

Do not start with:

```text
"Let's make the whole application autonomous."
```

Start with:

```text
One Capability
     ↓
One AI Boundary
     ↓
Measurable Outcome
```

Then expand only when evidence justifies it.

---

### 5.4 Deterministic Control Around Probabilistic Components

A useful architectural structure is:

```text
Deterministic Control
        ↓
Probabilistic Intelligence
        ↓
Deterministic Verification
        ↓
Deterministic Action
```

This creates a bounded execution model.

---

## 6. Evolution Model

A practical maturity path is:

```text
Level 0
Traditional Software
        ↓
Level 1
AI-Assisted Feature
        ↓
Level 2
AI Decision Support
        ↓
Level 3
Tool-Using AI
        ↓
Level 4
Bounded Agent
        ↓
Level 5
Workflow Agent
        ↓
Level 6
Multi-Agent Capability
```

The levels are architectural stages, not mandatory steps.

A system may remain at an earlier stage when that provides sufficient business value and lower operational risk.

---

# 7. Level 0 - Traditional Application

Architecture:

```text
User
 ↓
Application
 ↓
Business Logic
 ↓
Database
```

Characteristics:

- deterministic workflows
- explicit state
- explicit business rules
- conventional APIs
- conventional persistence

AI is not yet part of the architecture.

---

# 8. Level 1 - AI-Assisted Feature

Introduce AI for a bounded capability.

Examples:

```text
Text Summarization
Classification
Extraction
Translation
Speech-to-Text
Document Understanding
Semantic Search
Code Assistance
```

Architecture:

```text
Application
     │
     ▼
AI Service
     │
     ▼
Structured Result
     │
     ▼
Application
```

The AI does not directly control business state.

Example:

```text
Document
   ↓
LLM
   ↓
Extracted Invoice Data
   ↓
Schema Validation
   ↓
Application
```

---

## 9. Level 2 - AI Decision Support

AI begins providing recommendations.

```text
Business State
      ↓
Context Builder
      ↓
AI
      ↓
Recommendation
      ↓
Application / Human
      ↓
Decision
```

Examples:

- fraud recommendation
- pricing recommendation
- support classification
- incident diagnosis
- candidate matching
- demand forecasting
- risk analysis

The critical architectural distinction is:

```text
Recommendation ≠ Authorization
```

The application or authorized human remains responsible for the final action.

---

## 10. Level 3 - Tool-Using AI

AI can now invoke controlled tools.

```text
                ┌── Tool A
                │
Application → AI ├── Tool B
                │
                └── Tool C
```

Tools may include:

```text
Database Query
Search
Calculator
API
File System
Business Service
Workflow
```

Tool access must be explicit.

The model should not receive unrestricted access to application infrastructure.

---

## 11. Tool Boundary

A tool should expose a controlled capability rather than an unrestricted implementation.

Prefer:

```text
get_customer(customer_id)
```

over:

```text
execute_arbitrary_sql(sql)
```

Prefer:

```text
create_refund(order_id, amount)
```

over:

```text
call_payment_database(...)
```

Tools should have:

```text
Schema
Authorization
Validation
Timeout
Audit
Rate Limit
Side-Effect Classification
```

---

## 12. Level 4 - Bounded Agent

The system now allows an AI component to perform multiple reasoning and tool-use steps.

```text
Goal
 ↓
Agent
 ↓
Reason
 ↓
Tool
 ↓
Observe
 ↓
Reason
 ↓
Tool
 ↓
Verify
 ↓
Result
```

The loop must be bounded.

Define:

```text
Maximum Steps
Maximum Time
Maximum Cost
Maximum Tool Calls
Allowed Tools
Allowed Resources
Allowed Network
```

An agent should not have unlimited execution authority.

---

## 13. Agent Loop Engineering

A bounded agent loop can be modeled as:

```text
Observe
   ↓
Context
   ↓
Reason
   ↓
Plan
   ↓
Act
   ↓
Observe
   ↓
Verify
   ↓
Continue / Stop
```

The application owns:

```text
Loop Limits
Termination
State
Permissions
Tool Availability
Failure Handling
```

The model contributes reasoning.

---

## 14. Level 5 - Workflow Agent

A workflow agent operates inside an explicit business process.

Example:

```text
Customer Request
       ↓
Classification
       ↓
Retrieve Customer Context
       ↓
Analyze Problem
       ↓
Generate Resolution
       ↓
Verification
       ↓
Human Gate
       ↓
Execute Action
       ↓
Audit
```

The workflow should remain explicit.

Do not hide the entire business process inside a prompt.

---

## 15. Graph Engineering

Complex agent workflows can be represented as an explicit graph:

```text
              ┌── Retrieve
              │
Start → Classify → Analyze → Verify
              │                 │
              └── Escalate ←────┘
                                  ↓
                              Human Gate
                                  ↓
                                Action
```

Graph nodes may represent:

- agent execution
- deterministic functions
- retrieval
- validation
- tool invocation
- human approval
- policy checks
- evaluation
- compensation

The graph provides control around probabilistic components.

---

## 16. Level 6 - Multi-Agent Capability

Multiple specialized agents may collaborate.

```text
                 Orchestrator
                 /     |      \
                /      |       \
          Research   Analysis   Execution
             Agent      Agent      Agent
                \        |        /
                 \       |       /
                    Verification
```

Agents should have explicit responsibilities.

Avoid:

```text
Agent
  ↕
Agent
  ↕
Agent
  ↕
Agent
  ↕
Agent
```

without a clear coordination model.

Multi-agent architecture introduces:

```text
Coordination
Context Sharing
Permission Boundaries
Failure Isolation
Cost Control
Observability
```

---

## 17. Context Engineering

As AI becomes more capable, context becomes an architectural resource.

Context may include:

```text
User Context
Business State
Retrieved Knowledge
Conversation History
Tool Results
Policies
System Instructions
Agent State
Memory
```

Do not simply put everything into the prompt.

Instead define:

```text
What context is needed?
Who owns it?
Where is it stored?
How is it retrieved?
How long is it valid?
Who can access it?
```

---

## 18. RAG Evolution

A basic AI feature may use:

```text
Query
 ↓
Vector Search
 ↓
Documents
 ↓
LLM
```

More mature architecture may evolve toward:

```text
Query
 ↓
Intent / Query Analysis
 ↓
Hybrid Retrieval
 ↓
Filtering
 ↓
Ranking
 ↓
Context Construction
 ↓
Model
 ↓
Verification
```

Retrieval should be evaluated independently.

Measure:

```text
Retrieval Recall
Precision
Ranking Quality
Context Relevance
Answer Grounding
Citation Accuracy
```

---

## 19. Memory Architecture

Agentic systems may require different memory layers:

```text
Working Context
      ↓
Conversation Memory
      ↓
Task Memory
      ↓
User Memory
      ↓
Long-Term Knowledge
```

Do not treat all information as "memory".

Define:

```text
Ownership
Lifetime
Storage
Retrieval
Privacy
Deletion
Consistency
```

Application state should remain distinct from model memory.

---

## 20. MCP and Tool Connectivity

As AI capabilities grow, tool integration may evolve toward standardized interfaces such as MCP.

Architecture:

```text
Agent
  ↓
MCP Client
  ↓
MCP Server
  ↓
Controlled Capability
```

MCP can standardize access to tools and resources.

It does not automatically provide:

```text
Authorization
Isolation
Safety
Business Validation
Auditability
```

Those remain architectural responsibilities.

---

## 21. Permission Policy

AI-native evolution requires increasingly explicit permissions.

Define:

```text
Who
  ↓
Can invoke what
  ↓
On which resource
  ↓
Under which conditions
```

Example:

```text
Research Agent
    ✓ Search
    ✓ Read Documents
    ✓ Retrieve Knowledge
    ✗ Send Email
    ✗ Modify Payments
    ✗ Delete Data
```

Permissions should be enforced outside the model.

---

## 22. Sandbox

Agents performing potentially unsafe operations should operate inside controlled execution environments.

Possible boundaries:

```text
Agent
 ↓
Sandbox
 ├── Filesystem
 ├── Process
 ├── Network
 └── Resources
```

Define:

```text
Filesystem Access
Network Access
CPU
Memory
Execution Time
Processes
Secrets
```

The sandbox limits blast radius.

---

## 23. Network Policy

AI agents may access external systems through tools.

Define:

```text
Allowed Destinations
Allowed Protocols
Allowed Ports
Authentication
Rate Limits
Data Transfer Rules
```

Do not assume that tool-level permissions are sufficient network control.

---

## 24. Verification

AI output should be verified before important actions.

Verification may include:

```text
Schema Validation
Business Rules
Policy Validation
Permission Check
Consistency Check
Grounding Check
Confidence Threshold
Deterministic Calculation
Human Review
```

Architecture:

```text
AI
 ↓
Candidate Action
 ↓
Verification
 ↓
Policy
 ↓
Action
```

---

## 25. Human Gate

Some actions should require explicit human approval.

```text
AI
 ↓
Recommendation
 ↓
Human Gate
 ↓
Action
```

Typical candidates:

```text
Financial Transactions
Production Changes
Data Deletion
External Commitments
Security Changes
High-Impact Decisions
```

Human approval should be explicit and auditable.

---

## 26. Evaluation-Driven Evolution

Traditional software often relies heavily on deterministic tests.

AI systems require additional evaluation layers:

```text
Unit Tests
      ↓
Integration Tests
      ↓
AI Evaluations
      ↓
Regression Evaluations
      ↓
Production Monitoring
```

Evaluate:

```text
Correctness
Grounding
Tool Selection
Tool Arguments
Policy Compliance
Safety
Latency
Cost
Consistency
```

---

## 27. Evals as Architecture

Evaluation should not be treated as a final QA activity.

Architecture can define:

```text
Evaluation Dataset
Evaluation Criteria
Quality Thresholds
Regression Rules
Release Gates
Production Monitoring
```

For example:

```text
New Model
   ↓
Evaluation Suite
   ↓
Threshold Check
   ↓
Pass → Deploy
Fail → Reject
```

This makes AI quality part of the delivery architecture.

---

## 28. AI Gateway and Model Routing

As AI usage grows, introduce an AI gateway where justified.

```text
Application / Agent
        ↓
AI Gateway
   ┌────┼─────┐
   ↓    ↓     ↓
Model A Model B Model C
```

Possible responsibilities:

```text
Model Routing
Authentication
Rate Limiting
Token Tracking
Cost Tracking
Caching
Fallback
Observability
Policy Enforcement
```

Avoid turning the gateway into an unbounded central dependency.

---

## 29. Model Replaceability

Models should be replaceable architectural components.

Prefer:

```text
AI Capability
     ↓
Model Abstraction / Gateway
     ↓
Model
```

rather than embedding model-specific assumptions everywhere.

Track:

```text
Model Version
Provider
Latency
Cost
Quality
Context Window
Capabilities
Failure Rate
```

Model replacement should be validated through evaluation rather than assumed to be behaviorally equivalent.

---

## 30. Observability

AI-native systems require observability beyond conventional request tracing.

Track:

```text
TraceId
SpanId
AgentId
RunId
Model
Model Version
Prompt Version
Tool
Tool Arguments
Tool Result
Token Usage
Latency
Cost
Evaluation Result
Policy Decision
Human Approval
```

A useful execution trace is:

```text
Request
  ↓
Agent Run
  ↓
Model Call
  ↓
Tool Call
  ↓
Tool Result
  ↓
Model Call
  ↓
Verification
  ↓
Action
```

---

## 31. Cost Architecture

AI introduces variable computational cost.

Measure:

```text
Input Tokens
Output Tokens
Model Calls
Tool Calls
Agent Steps
Execution Time
GPU / CPU Usage
Retrieval Cost
Storage Cost
```

Cost controls may include:

```text
Model Routing
Token Budgets
Context Limits
Caching
Batching
Step Limits
Rate Limits
Smaller Models
Early Termination
```

Cost should be treated as an architectural constraint.

---

## 32. Failure Modes

### LLM as System of Record

Business state exists only in conversations or model context.

---

### Prompt as Business Logic

Critical business rules exist only in prompts.

---

### Unbounded Agent Loop

The agent can continue indefinitely.

---

### Excessive Agency

The AI receives more authority than required.

---

### Tool Explosion

The agent has too many overlapping or poorly defined tools.

---

### Context Overload

Too much irrelevant information is supplied to the model.

---

### Memory Confusion

Conversation history, business state, and long-term knowledge are mixed together.

---

### Hidden Side Effects

The agent performs actions without explicit control boundaries.

---

### No Verification

AI output is executed directly.

---

### No Evaluation

Model or prompt changes are deployed without regression measurement.

---

### Model Lock-In

Application behavior depends heavily on one provider or model.

---

### Agentic Distributed Monolith

Multiple agents become tightly coupled through synchronous calls and shared state.

---

### Observability Blindness

The system cannot explain:

```text
Which model?
Which prompt?
Which tools?
Which context?
Which decision?
Which cost?
```

---

### Cost Explosion

Agent loops and model calls grow without effective budgets.

---

## 33. Architectural Invariants

An AI-native evolution should enforce:

1. Application owns authoritative business state.
2. AI outputs are bounded by explicit contracts.
3. AI does not bypass authorization.
4. Tool access is explicit.
5. Side effects have deterministic control boundaries.
6. Agent loops have limits.
7. Important AI outputs are verifiable.
8. High-impact actions can require human approval.
9. Context ownership is explicit.
10. Memory is separated from authoritative application state.
11. Model versions are observable.
12. AI behavior is evaluated.
13. AI quality has measurable thresholds.
14. Sensitive operations are auditable.
15. Network access is controlled.
16. Agent execution has cost limits.
17. Failure isolation limits blast radius.
18. Temporary AI migration mechanisms have removal criteria.
19. Model replacement is possible through controlled interfaces.
20. The architecture remains understandable without relying on model behavior alone.

---

## 34. Testing Strategy

AI-native evolution requires multiple testing dimensions.

```text
Unit Tests
    ↓
Schema Tests
    ↓
Architecture Tests
    ↓
Integration Tests
    ↓
Tool Tests
    ↓
Agent Tests
    ↓
Evaluation Tests
    ↓
Regression Tests
    ↓
Security Tests
    ↓
Failure Tests
```

Important test categories:

### Tool Tests

Verify:

```text
Schema
Authorization
Arguments
Errors
Side Effects
Idempotency
```

### Agent Tests

Verify:

```text
Goal Completion
Tool Selection
Loop Termination
Policy Compliance
Failure Handling
```

### Evaluation Tests

Measure:

```text
Accuracy
Grounding
Relevance
Consistency
Safety
Cost
Latency
```

---

## 35. Migration Strategy

A practical migration sequence is:

```text
1. Identify AI Opportunity
        ↓
2. Define Business Outcome
        ↓
3. Establish Baseline
        ↓
4. Introduce AI Boundary
        ↓
5. Add Structured Output
        ↓
6. Add Evaluation
        ↓
7. Add Retrieval / Context
        ↓
8. Add Controlled Tools
        ↓
9. Add Verification
        ↓
10. Add Bounded Agent Loop
        ↓
11. Add Human Gates Where Required
        ↓
12. Expand Automation
        ↓
13. Remove Obsolete Deterministic Path
```

Not every capability needs to reach the final stages.

---

## 36. Parallel AI and Legacy Logic

During migration:

```text
Request
   │
   ├──→ Existing Logic
   │
   └──→ AI Capability
```

Compare results.

For example:

```text
Existing Classification
        ↕
AI Classification
```

Track:

```text
Agreement
Disagreement
Accuracy
False Positives
False Negatives
Latency
Cost
```

The AI path should initially be informational when the consequences of errors are significant.

---

## 37. AI Feature Flags

AI capabilities should be independently controllable.

Example:

```text
AI_FEATURE_ENABLED
AI_MODEL
AI_VERSION
AI_AGENT_ENABLED
AI_TOOLS_ENABLED
AI_AUTONOMOUS_ACTIONS_ENABLED
HUMAN_APPROVAL_REQUIRED
```

Feature flags can control:

```text
Capability
Model
Agent
Tool
Tenant
User
Percentage of Traffic
Environment
```

Do not allow feature flags to become permanent architectural complexity.

---

## 38. Rollback

AI rollback differs from traditional deployment rollback.

A model can change behavior without changing the application binary.

Therefore rollback may require:

```text
Model Version
Prompt Version
Tool Version
Retrieval Version
Evaluation Version
Configuration Version
```

A useful rollback unit is:

```text
AI Capability Version
```

not only:

```text
Application Version
```

---

## 39. Security Evolution

As AI gains more capabilities, the security boundary must evolve.

```text
AI-Assisted
    ↓
Tool Access
    ↓
Write Access
    ↓
Autonomous Execution
```

Each step increases potential blast radius.

Security architecture should therefore define:

```text
Identity
Permissions
Tool Access
Network Policy
Sandbox
Secrets
Data Access
Audit
Human Approval
```

Prompt injection and indirect instruction attacks should be treated as architectural threats, particularly when retrieved or external content can influence tool execution.

---

## 40. AI Supply Chain

AI-native systems introduce additional dependencies:

```text
Model Provider
Model
Embedding Model
Vector Store
Prompt
Dataset
Evaluation Dataset
Agent Framework
MCP Server
Tool
External API
```

Track:

```text
Version
Origin
License
Security
Availability
Quality
Cost
Dependencies
```

The AI supply chain should be observable and governable.

---

## 41. Migration State Machine

Represent AI capability evolution explicitly:

```text
Identified
    ↓
BaselineEstablished
    ↓
AIPrototype
    ↓
AIValidated
    ↓
AIProduction
    ↓
ToolEnabled
    ↓
VerifiedAutomation
    ↓
BoundedAgent
    ↓
WorkflowAgent
    ↓
ExpandedAutomation
```

Possible failure state:

```text
AI Evaluation Failed
        ↓
Rollback / Rework
        ↓
Previous State
```

---

## 42. Production Checklist

### AI Opportunity

- [ ] Business outcome defined.
- [ ] Existing baseline measured.
- [ ] AI capability clearly bounded.
- [ ] Failure impact understood.

### Architecture

- [ ] Application owns state.
- [ ] AI boundary explicit.
- [ ] Structured outputs defined.
- [ ] Model dependency isolated.
- [ ] Context ownership defined.
- [ ] Tool boundaries defined.

### Safety

- [ ] Permissions enforced outside the model.
- [ ] Network access controlled.
- [ ] Sandbox used where required.
- [ ] Side effects verified.
- [ ] Human gate defined where appropriate.
- [ ] Blast radius understood.

### Evaluation

- [ ] Evaluation dataset exists.
- [ ] Quality criteria defined.
- [ ] Regression evaluation exists.
- [ ] Release thresholds defined.
- [ ] Production quality monitored.

### Operations

- [ ] Model version tracked.
- [ ] Prompt version tracked.
- [ ] Tool calls observable.
- [ ] Token usage measured.
- [ ] Cost measured.
- [ ] Latency measured.
- [ ] Agent steps bounded.
- [ ] Rollback defined.

### Evolution

- [ ] Legacy path identified.
- [ ] Migration boundary established.
- [ ] AI usage measurable.
- [ ] Legacy usage measurable.
- [ ] Compatibility layer has removal criteria.
- [ ] Final architecture does not depend unnecessarily on legacy AI/non-AI paths.

---

## 43. Lab Experiment

Build an existing application:

```text
Customer Support System
        │
        ├── Customer Database
        ├── Ticket Service
        ├── Knowledge Base
        └── Support Workflow
```

### Phase 1 - Traditional

Implement:

```text
Ticket
  ↓
Rule-Based Classification
  ↓
Support Queue
```

Measure:

```text
Accuracy
Latency
Processing Time
```

### Phase 2 - AI-Assisted

Add:

```text
Ticket
  ↓
LLM Classification
  ↓
Structured Result
  ↓
Existing Workflow
```

Add evaluation.

### Phase 3 - Decision Support

Add:

```text
Ticket
  ↓
AI Analysis
  ↓
Recommended Resolution
  ↓
Human
```

### Phase 4 - RAG

Add:

```text
Ticket
  ↓
Query Analysis
  ↓
Knowledge Retrieval
  ↓
Context
  ↓
AI
```

Measure retrieval and answer quality separately.

### Phase 5 - Tool Use

Add controlled tools:

```text
get_customer()
search_knowledge()
get_order()
```

Define permissions.

### Phase 6 - Verification

Introduce:

```text
AI
 ↓
Recommendation
 ↓
Schema Validation
 ↓
Business Policy
 ↓
Human Gate
```

### Phase 7 - Bounded Agent

Allow:

```text
Analyze
 → Retrieve
 → Inspect Customer
 → Inspect Order
 → Generate Resolution
 → Verify
```

Set:

```text
Max Steps
Max Cost
Max Runtime
Allowed Tools
```

### Phase 8 - Controlled Automation

Allow low-risk actions to execute automatically.

Keep high-impact operations behind human approval.

### Phase 9 - Failure Experiments

Test:

```text
Prompt Injection
Wrong Retrieval
Wrong Tool Selection
Tool Failure
Model Timeout
Model Hallucination
Infinite Loop
Budget Exhaustion
Permission Denial
Network Failure
Human Rejection
```

### Phase 10 - Measure

Compare:

```text
Traditional
    vs
AI-Assisted
    vs
AI Decision Support
    vs
Bounded Agent
```

Measure:

```text
Quality
Speed
Cost
Safety
Human Effort
Failure Rate
```

---

## 44. Evolution Decision Questions

Before increasing AI autonomy, ask:

```text
Does AI provide measurable value?

What is the cost of an incorrect result?

Can the result be verified?

Can the action be reversed?

What state must remain deterministic?

What tools are actually required?

What permissions are required?

What is the maximum blast radius?

What happens when the model fails?

What happens when retrieval is wrong?

What happens when the agent loops?

Can a human intervene?

Can the AI capability be disabled quickly?

How will quality be evaluated after model changes?
```

These questions should determine the architecture level.

---

## 45. Key Takeaways

- AI-native evolution is a migration strategy, not simply adding an LLM.
- Start with bounded AI capabilities.
- Preserve deterministic ownership of application state and business invariants.
- Separate intelligence from control.
- Introduce tools only through explicit boundaries.
- Bound agent loops by time, steps, cost, permissions, and resources.
- Treat context as an architectural resource.
- Separate memory from authoritative application state.
- Use evaluation as part of the architecture and delivery pipeline.
- Introduce verification before allowing important side effects.
- Use human gates for actions where automated failure has unacceptable consequences.
- Make model, prompt, tool, and retrieval versions observable.
- Treat AI cost as an architectural constraint.
- Control network access and sandbox potentially dangerous execution.
- Use gradual migration and shadow execution when replacing deterministic paths.
- Do not introduce autonomy simply because it is technically possible.
- The target is not maximum autonomy. The target is the appropriate level of reliable intelligence for the business capability.

> **AI-native evolution is the gradual movement from deterministic software that uses AI toward systems where intelligence is embedded into the architecture, while control, state, policy, verification, and safety remain explicit.**
