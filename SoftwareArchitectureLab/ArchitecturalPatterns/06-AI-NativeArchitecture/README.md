# AI-Native Architecture

## 1. Essence

AI-Native Architecture is an architectural approach in which AI capabilities are treated as **first-class system components**, rather than as isolated features added to conventional software.

The architecture is designed around the characteristics of AI systems:

- probabilistic behavior
- model-driven reasoning
- context dependency
- tool use
- agentic execution
- non-deterministic outputs
- evaluation
- memory
- retrieval
- human oversight
- policy enforcement
- model selection
- runtime adaptation
- continuous improvement

A traditional application often follows:

```text
Input
  ↓
Deterministic Business Logic
  ↓
Database
  ↓
Output
```

An AI-native application may look more like:

```text
User Intent
     ↓
Context Engineering
     ↓
AI Model
     ↓
Reasoning / Decision
     ↓
Tool Selection
     ↓
Tool Execution
     ↓
Observation
     ↓
Verification
     ↓
Next Action
     ↓
Final Result
```

The architecture therefore must control not only **data and computation**, but also **context, actions, permissions, uncertainty, and verification**.

Core principle:

> **AI provides intelligence, while the application remains responsible for control, state, policy, and safety.**

---

## 2. Problem

Traditional software architecture assumes that important behavior can be expressed deterministically in code.

AI-native systems introduce a different computational model.

The system may need to:

- interpret natural language
- reason over incomplete information
- retrieve relevant knowledge
- select tools dynamically
- plan multi-step tasks
- generate structured outputs
- interact with external systems
- adapt execution based on observations
- remember previous interactions
- evaluate its own results
- request human approval
- recover from failure

This creates architectural problems that conventional application design does not fully address.

Examples include:

- hallucination
- context overflow
- incorrect tool selection
- invalid tool parameters
- prompt injection
- excessive permissions
- uncontrolled agent loops
- non-deterministic behavior
- model drift
- changing model behavior
- evaluation difficulty
- hidden state
- expensive inference
- latency
- token consumption
- cascading agent failures

AI therefore cannot simply be inserted between an API and a database.

The architecture must explicitly control the AI runtime.

---

## 3. Intent

AI-Native Architecture aims to create systems where AI capabilities can operate within controlled architectural boundaries.

The architecture should enable:

- reasoning
- retrieval
- tool use
- agentic workflows
- autonomous execution
- contextual decision making
- structured outputs
- verification
- human oversight
- policy enforcement
- observability
- evaluation
- model independence
- controlled cost
- controlled risk
- continuous improvement

The objective is not maximum autonomy.

The objective is:

> **The maximum useful autonomy that can operate within explicit technical and business constraints.**

---

## 4. Core Principles

### 4.1 Application Owns State

The AI model should not become the system of record.

Prefer:

```text
Application
    |
    +-- State
    +-- Business Rules
    +-- Permissions
    +-- Workflow State
    +-- Policies
    +-- Audit
    |
    v
AI
    |
    +-- Reasoning
    +-- Interpretation
    +-- Planning
    +-- Generation
```

The model can reason about state.

It should not silently own authoritative application state.

---

### 4.2 AI Provides Intelligence, Not Control

AI can recommend:

```text
"Create a refund."
```

The application decides whether that action is:

- allowed
- valid
- within policy
- authorized
- safe
- properly parameterized

Therefore:

```text
AI Decision
    ↓
Policy
    ↓
Authorization
    ↓
Validation
    ↓
Tool Execution
```

---

### 4.3 Context Is an Architectural Resource

AI behavior depends heavily on context.

Context may include:

- user input
- system instructions
- conversation history
- retrieved documents
- user profile
- application state
- tool descriptions
- previous observations
- memory
- policies
- environmental information

Context should therefore be treated as a controlled architectural resource.

Important questions include:

- What enters the context?
- Who controls it?
- How is it prioritized?
- What is removed?
- What is trusted?
- What is untrusted?
- How much context is required?
- What is persisted?

---

### 4.4 Models Are Replaceable Components

The architecture should avoid unnecessarily coupling business logic to a specific model.

Prefer:

```text
Application
    |
AI Gateway / Model Abstraction
    |
    +-- Model A
    +-- Model B
    +-- Model C
    +-- Local Model
```

The architecture should support model selection based on:

- quality
- latency
- cost
- capability
- privacy
- context window
- tool support
- availability

---

### 4.5 Deterministic Control Around Probabilistic Components

AI is probabilistic.

Critical system behavior should therefore be surrounded by deterministic controls.

Example:

```text
User Request
     ↓
LLM
     ↓
Structured Output
     ↓
Schema Validation
     ↓
Policy Validation
     ↓
Authorization
     ↓
Tool Execution
```

The model proposes.

The system verifies.

---

### 4.6 Every Action Has a Boundary

An AI agent should not automatically receive unrestricted access to the environment.

Actions should be constrained by:

- permissions
- tool policies
- network policies
- sandboxing
- resource limits
- identity
- scope
- approval requirements

This creates an explicit action boundary.

---

## 5. Architectural Model

A mature AI-native architecture can be represented as:

```text
                         ┌──────────────────┐
                         │      User        │
                         └────────┬─────────┘
                                  |
                                  v
                         ┌──────────────────┐
                         │ Intent / API     │
                         │ Boundary         │
                         └────────┬─────────┘
                                  |
                                  v
                         ┌──────────────────┐
                         │ Context          │
                         │ Engineering      │
                         └────────┬─────────┘
                                  |
                                  v
                    ┌──────────────────────────┐
                    │ Agent / AI Runtime       │
                    │                          │
                    │ Planning                 │
                    │ Reasoning                │
                    │ Tool Selection           │
                    │ Loop / Graph Execution   │
                    └────────────┬─────────────┘
                                 |
                    ┌────────────┼────────────┐
                    |            |            |
                    v            v            v
               Retrieval      Tools        Memory
                    |            |            |
                    |            v            |
                    |       ┌──────────┐      |
                    |       │ Policy   │      |
                    |       │ Engine   │      |
                    |       └────┬─────┘      |
                    |            |            |
                    └────────────┼────────────┘
                                 |
                                 v
                         ┌──────────────────┐
                         │ Verification /   │
                         │ Evaluation       │
                         └────────┬─────────┘
                                  |
                     ┌────────────┴────────────┐
                     |                         |
                     v                         v
               Human Gate                Final Result
```

This architecture separates:

- intelligence
- context
- state
- action
- policy
- verification
- human control

---

## 6. AI-Native Building Blocks

A production AI-native architecture commonly contains:

### Model Layer

- foundation models
- reasoning models
- embedding models
- speech models
- vision models
- specialized models
- local models

### AI Runtime

- agent runtime
- workflow engine
- graph execution
- loop execution
- model routing
- structured output handling

### Context Layer

- prompt construction
- context assembly
- context compression
- retrieval
- memory
- user context
- system context

### Tool Layer

- APIs
- functions
- databases
- MCP servers
- file systems
- search
- external services

### Control Layer

- permissions
- policy
- sandbox
- network policy
- rate limits
- resource limits
- human gates

### Verification Layer

- schema validation
- rule validation
- evaluators
- fact verification
- policy verification
- confidence checks
- human approval

### Observability Layer

- traces
- prompts
- model calls
- tool calls
- token usage
- latency
- cost
- errors
- evaluations

---

## 7. Context Engineering

Context Engineering is the systematic construction and management of the information supplied to an AI system at execution time.

Context may contain:

```text
System Instructions
+
User Intent
+
Application State
+
Relevant Knowledge
+
Retrieved Documents
+
Tool Definitions
+
Memory
+
Policies
+
Previous Observations
```

Context should be:

- relevant
- minimal
- trustworthy
- structured
- prioritized
- bounded
- observable

A larger context is not automatically better.

Poor context can cause:

- distraction
- conflicting instructions
- increased cost
- increased latency
- lower reasoning quality
- prompt injection exposure

Therefore:

> **Context quality is an architectural concern, not merely a prompt-engineering concern.**

---

## 8. Retrieval Architecture

AI-native systems frequently require external knowledge.

A retrieval architecture may contain:

```text
Documents
    ↓
Parsing
    ↓
Chunking
    ↓
Embedding
    ↓
Index
    ↓
Retrieval
    ↓
Ranking
    ↓
Context Assembly
    ↓
LLM
```

Modern retrieval systems may include:

- keyword search
- vector search
- hybrid search
- metadata filtering
- reranking
- graph retrieval
- query rewriting
- multi-query retrieval
- contextual retrieval

Retrieval should be evaluated independently from generation.

Important metrics include:

- recall
- precision
- retrieval relevance
- ranking quality
- citation accuracy
- answer grounding

---

## 9. RAG Architecture

Retrieval-Augmented Generation separates knowledge acquisition from language generation.

```text
User Query
    |
    v
Query Understanding
    |
    v
Retriever
    |
    v
Relevant Context
    |
    v
LLM
    |
    v
Generated Answer
```

A production RAG architecture should consider:

- document ingestion
- metadata
- chunking
- embeddings
- hybrid retrieval
- reranking
- access control
- freshness
- citations
- evaluation
- hallucination detection

The retrieval layer must respect authorization.

A document should not become accessible merely because the vector database can retrieve it.

---

## 10. Agent Architecture

An agent is an AI-driven runtime capable of selecting actions and iterating toward a goal.

A simple agent loop is:

```text
Observe
   ↓
Reason
   ↓
Plan
   ↓
Act
   ↓
Observe
   ↓
Reason
   ↓
...
```

The loop must be bounded.

Typical controls include:

- maximum iterations
- timeout
- token budget
- tool budget
- cost budget
- action permissions
- network restrictions
- human approval
- termination conditions

---

## 11. Loop Engineering

Agent loops should be treated as explicit architectural components.

Example:

```text
while not goal_reached:
    observe()
    reason()
    choose_action()
    validate_action()
    execute_action()
```

The architecture must define:

- loop state
- termination conditions
- retry behavior
- failure handling
- maximum iterations
- budget limits
- verification
- recovery

Unbounded agent loops are an operational risk.

---

## 12. Graph Engineering

Complex agentic systems may be better represented as explicit execution graphs.

Example:

```text
             ┌──────────────┐
             │   Analyze    │
             └──────┬───────┘
                    |
                    v
             ┌──────────────┐
             │   Retrieve   │
             └──────┬───────┘
                    |
                    v
             ┌──────────────┐
             │   Decide     │
             └───┬──────┬───┘
                 |      |
              success   retry
                 |      |
                 v      └───────┐
             ┌──────────────┐   |
             │   Execute    │<──┘
             └──────┬───────┘
                    |
                    v
             ┌──────────────┐
             │  Verify     │
             └──────────────┘
```

Graph Engineering makes:

- state transitions
- branching
- retries
- verification
- human gates
- error paths

explicit.

This is especially valuable for production workflows.

---

## 13. Tool Use and Function Calling

Tools provide agents with capabilities beyond text generation.

Examples:

```text
Search
Database Query
File Access
API Call
Code Execution
Calendar
Email
Payment
Workflow
```

A tool should expose:

- name
- description
- schema
- authorization requirements
- side effects
- timeout
- resource limits
- audit information

The model should not directly execute arbitrary operations.

Prefer:

```text
LLM
 ↓
Tool Selection
 ↓
Schema Validation
 ↓
Permission Policy
 ↓
Human Gate if required
 ↓
Tool Execution
 ↓
Observation
```

---

## 14. MCP and Tool Connectivity

Model Context Protocol can provide a standardized mechanism for exposing tools and resources to AI systems.

Conceptually:

```text
Agent
  |
  v
MCP Client
  |
  +---- MCP Server A
  |
  +---- MCP Server B
  |
  +---- MCP Server C
```

The architecture should still control:

- tool discovery
- authentication
- authorization
- network access
- tool permissions
- data boundaries
- auditability
- failure handling

A protocol does not replace architectural governance.

---

## 15. Stateless and Stateful AI Systems

AI-native architectures should explicitly distinguish:

### Stateless Execution

Each execution receives all required context.

```text
Request
  ↓
Context
  ↓
Agent
  ↓
Result
```

Useful for:

- APIs
- scalable workers
- deterministic workflows
- isolated executions

### Stateful Execution

Execution state persists across interactions.

```text
User
 |
 v
Agent
 |
 +-- Conversation State
 +-- Task State
 +-- Memory
 +-- Workflow State
```

State should normally be owned by the application or explicit state-management layer rather than hidden inside the model.

---

## 16. Memory Architecture

AI memory can exist at multiple levels.

### Working Memory

Current execution context.

### Conversation Memory

Previous interactions.

### Semantic Memory

Long-term knowledge about entities, concepts, or user context.

### Episodic Memory

Previous experiences or events.

### Application State

Authoritative business state.

These should not be treated as interchangeable.

For example:

```text
Memory:
"User usually prefers X."

Application State:
"User selected X."
```

The second is authoritative.

Memory may influence reasoning but should not silently override business state.

---

## 17. Permission Policy

AI agents require explicit authorization boundaries.

A policy may define:

```text
Agent
   |
   +-- Can read customer data
   +-- Can search documents
   +-- Can create drafts
   +-- Cannot send email
   +-- Cannot delete data
   +-- Cannot access production database
```

Policies should consider:

- identity
- role
- resource
- action
- environment
- sensitivity
- time
- scope
- approval level

Permission decisions should be deterministic where possible.

---

## 18. Sandbox Architecture

Agents executing code or interacting with files require isolation.

A sandbox can restrict:

- filesystem
- processes
- network
- environment variables
- credentials
- CPU
- memory
- execution time

Conceptually:

```text
Agent
  |
  v
Sandbox
  |
  +-- Restricted Filesystem
  +-- Restricted Network
  +-- Limited CPU
  +-- Limited Memory
  +-- Limited Runtime
```

The sandbox reduces blast radius.

It does not replace application-level authorization.

---

## 19. Network Policy

AI agents should not automatically receive unrestricted network access.

A network policy may define:

```text
Allowed:
    api.example.com
    search.example.com

Denied:
    internal-database
    cloud-metadata
    private-network
```

Network access should be:

- explicit
- observable
- restricted
- auditable

This is especially important for autonomous agents.

---

## 20. Verification

AI output should be verified according to its risk.

Possible verification layers include:

```text
LLM Output
    ↓
Schema Validation
    ↓
Business Rules
    ↓
Policy Validation
    ↓
Evidence Verification
    ↓
Secondary Evaluator
    ↓
Human Gate
```

Different tasks require different verification strategies.

For example:

- low-risk text generation may require schema validation
- financial operations may require deterministic business validation and human approval
- production code changes may require tests and review
- security-sensitive actions may require explicit authorization

---

## 21. Human Gate

Human involvement should be treated as an architectural control.

A human gate may be required when:

- risk is high
- confidence is low
- an irreversible action is requested
- policy requires approval
- financial impact exceeds a threshold
- external communication is involved
- production changes are proposed

Example:

```text
Agent
  ↓
Plan
  ↓
Verification
  ↓
Human Approval
  ↓
Execution
```

The human gate should be explicit rather than hidden inside a conversational interface.

---

## 22. Eval-Driven Architecture

Traditional software testing is insufficient for many AI behaviors.

AI-native systems require continuous evaluation.

An evaluation architecture may contain:

```text
Input Dataset
     ↓
Agent
     ↓
Output
     ↓
Evaluator
     ↓
Metrics
     ↓
Regression Analysis
```

Evaluation dimensions may include:

- correctness
- relevance
- groundedness
- tool selection
- tool arguments
- instruction following
- safety
- latency
- cost
- task completion
- consistency

Important principle:

> **If AI behavior matters in production, it needs an evaluation strategy.**

---

## 23. Observability

AI observability extends traditional application observability.

Capture appropriate information about:

### Model

- model identifier
- provider
- temperature or relevant sampling configuration
- token usage
- latency
- cost

### Context

- context sources
- retrieval results
- context size
- truncation
- memory references

### Agent

- steps
- decisions
- tool selections
- loop iterations
- termination reason

### Tools

- tool name
- arguments
- execution time
- result
- error
- authorization decision

### Evaluation

- evaluator
- score
- failure category
- regression information

Sensitive prompts, outputs, and user data must be handled according to privacy and security requirements.

---

## 24. AI Gateway and Model Routing

A centralized AI gateway can provide:

```text
Application
    |
    v
AI Gateway
    |
    +-- Model A
    +-- Model B
    +-- Model C
    +-- Local Model
```

Possible responsibilities:

- model routing
- authentication
- rate limiting
- quotas
- cost tracking
- caching
- fallback
- observability
- policy enforcement
- provider abstraction

Routing decisions may consider:

```text
Quality
Speed
Cost
Privacy
Capability
Availability
```

The gateway should not become a business-logic bottleneck.

---

## 25. Cost and Resource Architecture

AI systems have a unique resource dimension: inference cost.

Cost may depend on:

- input tokens
- output tokens
- model selection
- context size
- number of agent iterations
- number of tool calls
- retrieval operations
- multimodal processing

A simple cost model is:

```text
Total Cost
    =
Model Cost
+
Retrieval Cost
+
Tool Cost
+
Infrastructure Cost
```

Agentic systems can multiply costs because one user request may trigger many model calls.

Therefore the architecture should enforce:

- token budgets
- iteration budgets
- tool budgets
- model routing
- caching
- context optimization
- timeout limits

---

## 26. Quality, Speed, and Cost

AI-native systems should explicitly optimize three dimensions:

```text
          Quality
            /\
           /  \
          /    \
         /      \
      Speed ---- Cost
```

Improving one dimension may affect another.

Architecture experiments should measure:

- quality
- latency
- token usage
- infrastructure cost
- task completion
- failure rate

Do not optimize only model quality.

A slightly weaker model with substantially lower latency and cost may be appropriate for some workloads.

The architectural decision should be based on measured requirements.

---

## 27. Blast Radius and Isolation

Autonomous AI introduces a new architectural concern:

> What happens if the AI makes the wrong decision?

The answer defines the system's blast radius.

Isolation mechanisms include:

- sandboxing
- permission boundaries
- network policies
- resource limits
- transaction boundaries
- human gates
- read-only tools
- staged execution
- reversible operations
- environment separation

Example:

```text
Agent
 |
 +-- Read Tools
 |      |
 |      +-- broad access
 |
 +-- Write Tools
        |
        +-- restricted access
        |
        +-- approval required
```

The more consequential the action, the stronger the isolation should be.

---

## 28. Auditability

AI-native systems must be able to answer:

```text
What happened?

Which model acted?

What context was provided?

Which tools were selected?

Which data was retrieved?

Which policies were evaluated?

Which actions were executed?

Who approved the action?

What was the final result?
```

An audit trail may include:

```text
ExecutionId
TraceId
AgentId
ModelId
ToolCallId
UserId
PolicyDecision
Approval
Timestamp
Result
```

Auditability is particularly important for regulated or high-impact systems.

---

## 29. Security and Threat Model

AI-native systems introduce additional attack surfaces.

Important threats include:

- prompt injection
- indirect prompt injection
- data exfiltration
- tool abuse
- excessive agency
- insecure tool implementation
- malicious retrieved content
- credential exposure
- model manipulation
- context poisoning
- unauthorized memory access
- supply-chain risks
- insecure MCP servers

Security controls should include:

- least privilege
- input validation
- output validation
- tool authorization
- sandboxing
- network policies
- secret isolation
- content trust boundaries
- audit logging
- human approval for high-risk actions

---

## 30. AI Supply Chain

AI-native systems depend on more than application code.

The supply chain may include:

```text
Foundation Model
Embedding Model
Prompt
System Instructions
Agent Framework
MCP Server
Tool
Vector Database
Dataset
Evaluator
Model Provider
```

Each component introduces risk.

The architecture should therefore consider:

- provenance
- versioning
- dependency management
- model changes
- prompt changes
- dataset changes
- evaluator changes
- security scanning
- reproducibility

---

## 31. Architecture Tests and Invariants

AI-native architecture should enforce explicit invariants.

### State

- Application owns authoritative business state.
- AI cannot silently mutate business state.
- Memory is not treated as authoritative state.

### Tools

- Every tool has an explicit schema.
- Every tool has an authorization boundary.
- Side effects are explicit.
- High-risk tools require stronger controls.

### Agents

- Agent loops are bounded.
- Execution has resource limits.
- Termination conditions are explicit.
- Failures are observable.

### Models

- Model dependencies are isolated where practical.
- Model changes can be evaluated.
- Model calls are observable.

### Context

- Context sources are identifiable.
- Sensitive data is controlled.
- Retrieval respects authorization.
- Context size is bounded.

### Security

- Agents operate with least privilege.
- Network access is restricted.
- Secrets are not exposed unnecessarily.
- Sandboxes isolate high-risk execution.

### Verification

- Structured outputs are validated.
- Critical actions require deterministic validation.
- High-risk actions can require human approval.

These invariants should become automated tests where possible.

---

## 32. Failure Modes and Architectural Smells

### LLM as System of Record

The model becomes responsible for authoritative state.

### Prompt as Business Logic

Critical business rules exist only inside prompts.

### Unbounded Agent Loop

The agent can continue indefinitely.

### Excessive Agency

The agent has more permissions than necessary.

### Tool Explosion

An agent has access to too many tools, making tool selection unreliable and increasing risk.

### Context Overload

Large amounts of irrelevant information are continuously added to the context.

### Memory Confusion

Retrieved memories are treated as authoritative facts.

### Hidden Side Effects

A tool appears informational but performs mutations.

### No Verification

AI output directly triggers consequential actions.

### No Evaluation

The system is deployed without measurable quality criteria.

### Model Lock-In

Business logic becomes tightly coupled to one model provider.

### AI Gateway Monolith

All AI logic is pushed into a centralized gateway.

### Agentic Distributed Monolith

Multiple agents depend synchronously on each other in long chains.

### Observability Blindness

The system records application logs but cannot reconstruct AI decisions and tool execution.

### Cost Explosion

Agent loops, context growth, and tool calls create uncontrolled inference costs.

---

## 33. Testing Strategy

AI-native systems require multiple testing layers.

### Unit Tests

Test deterministic application logic.

### Schema Tests

Verify structured AI outputs.

### Prompt / Context Tests

Verify context construction and instruction behavior.

### Retrieval Tests

Measure:

- recall
- precision
- ranking
- grounding

### Tool Tests

Verify:

- schema
- authorization
- error handling
- idempotency
- side effects

### Agent Tests

Test:

- planning
- tool selection
- termination
- recovery
- state transitions

### Evaluation Tests

Measure AI behavior against representative datasets.

### Regression Tests

Run evaluation suites whenever changing:

- model
- prompt
- context strategy
- retrieval
- tools
- agent graph

### Security Tests

Test:

- prompt injection
- unauthorized tool access
- data leakage
- privilege escalation
- sandbox escape attempts

### Failure Tests

Inject:

- model timeout
- model failure
- tool failure
- retrieval failure
- malformed output
- unavailable MCP server
- excessive context
- budget exhaustion

---

## 34. Evolution and Migration

AI-native architecture should evolve incrementally.

A traditional application can introduce AI capabilities through stages:

```text
Traditional Application
        |
        v
AI-Assisted Feature
        |
        v
AI Decision Support
        |
        v
Tool-Using AI
        |
        v
Bounded Agent
        |
        v
Workflow Agent
        |
        v
Multi-Agent System
```

Each stage increases autonomy and therefore requires stronger controls.

A useful evolution strategy is:

```text
Capability
   ↓
Observe
   ↓
Assist
   ↓
Recommend
   ↓
Verify
   ↓
Execute with Approval
   ↓
Execute within Policy
   ↓
Autonomous Execution
```

Autonomy should increase only when reliability, observability, and controls justify it.

---

## 35. Production Architecture

A production AI-native platform may look like:

```text
                         ┌──────────────┐
                         │    Users     │
                         └──────┬───────┘
                                |
                                v
                       ┌─────────────────┐
                       │ API / UI Layer  │
                       └────────┬────────┘
                                |
                                v
                       ┌─────────────────┐
                       │ Agent Runtime   │
                       └────────┬────────┘
                                |
              ┌─────────────────┼──────────────────┐
              |                 |                  |
              v                 v                  v
        Context Engine      Tool Layer        Memory Layer
              |                 |                  |
              v                 v                  v
         RAG / Search       MCP / APIs        State Store
              |                 |
              └────────┬────────┘
                       |
                       v
                 ┌──────────────┐
                 │ AI Gateway   │
                 └──────┬───────┘
                        |
              ┌─────────┼─────────┐
              v         v         v
            Model A   Model B   Local Model

              ┌─────────────────────────┐
              │ Policy / Verification   │
              └────────────┬────────────┘
                           |
                           v
                    Human Gate

              ┌─────────────────────────┐
              │ Observability / Evals   │
              └─────────────────────────┘
```

---

## 36. .NET Implementation Notes

Modern .NET can provide the deterministic application foundation around AI components.

Useful building blocks include:

### ASP.NET Core

API and application boundary.

### Dependency Injection

Explicitly manage:

- model clients
- tool providers
- retrieval services
- policy engines
- evaluators

### HttpClientFactory

For model providers and external APIs.

### BackgroundService

For:

- asynchronous agent jobs
- queue consumers
- evaluation pipelines
- ingestion
- indexing

### OpenTelemetry

For distributed AI execution tracing.

### Aspire

Useful for orchestrating local AI-native distributed systems.

Example:

```text
AppHost
 |
 +-- API
 |
 +-- Agent Runtime
 |
 +-- PostgreSQL
 |
 +-- Redis
 |
 +-- Vector Store
 |
 +-- Message Broker
 |
 +-- MCP Servers
 |
 +-- Evaluation Worker
 |
 +-- Observability
```

### Strongly Typed Outputs

Prefer schemas such as:

```csharp
public sealed record AgentDecision(
    string Action,
    string Reason,
    IReadOnlyList<string> Evidence);
```

The application can then validate the output before execution.

### Policy Abstractions

Policies should be represented as explicit application components rather than hidden inside prompts.

---

## 37. Production Checklist and Architecture Experiments

### AI Runtime

- [ ] Agent execution is bounded
- [ ] State ownership is explicit
- [ ] Model calls are observable
- [ ] Model routing is defined
- [ ] Failure handling is implemented

### Context

- [ ] Context sources are identified
- [ ] Context is bounded
- [ ] Retrieval is evaluated
- [ ] Sensitive data is controlled
- [ ] Memory boundaries are explicit

### Tools

- [ ] Tools have schemas
- [ ] Tool permissions are explicit
- [ ] Tool side effects are documented
- [ ] Tool calls are audited
- [ ] High-risk tools require stronger controls

### Security

- [ ] Least privilege
- [ ] Sandbox
- [ ] Network policy
- [ ] Secret isolation
- [ ] Prompt injection defenses
- [ ] Data access controls

### Verification

- [ ] Structured outputs validated
- [ ] Business rules validated
- [ ] Critical actions verified
- [ ] Human gates defined
- [ ] Evaluation datasets exist

### Observability

- [ ] Distributed tracing
- [ ] Model metrics
- [ ] Token usage
- [ ] Cost tracking
- [ ] Tool execution traces
- [ ] Agent loop traces
- [ ] Evaluation metrics

### Operations

- [ ] Timeouts
- [ ] Resource limits
- [ ] Token budgets
- [ ] Tool budgets
- [ ] Cost budgets
- [ ] Graceful degradation
- [ ] Fallback strategies

### Recommended Lab Experiments

1. Build a simple AI-assisted API.
2. Add structured model output.
3. Add schema validation.
4. Add RAG.
5. Measure retrieval quality.
6. Add one tool.
7. Add tool authorization.
8. Add a bounded agent loop.
9. Add explicit application state.
10. Add memory.
11. Add a graph-based agent workflow.
12. Add MCP tools.
13. Add a sandbox.
14. Add network policy.
15. Add a human gate.
16. Add an evaluation dataset.
17. Build an automated evaluation harness.
18. Add distributed tracing.
19. Add model routing.
20. Measure quality, speed, and cost.
21. Inject tool failures.
22. Inject model failures.
23. Test prompt injection.
24. Test unauthorized tool access.
25. Run a cost explosion experiment.
26. Measure agent blast radius.
27. Compare single-agent and multi-agent execution.
28. Build a production readiness checklist.

Recommended experiment workflow:

```text
Problem
   ↓
AI Capability
   ↓
Context
   ↓
Model
   ↓
Tool / Retrieval
   ↓
Policy
   ↓
Verification
   ↓
Architecture Tests
   ↓
Evaluation
   ↓
Failure Experiment
   ↓
Observability
   ↓
Quality / Speed / Cost
   ↓
Trade-offs
   ↓
ADR
```

---

## 38. Key Takeaways

AI-Native Architecture introduces a different architectural model from conventional deterministic software.

The important architectural concerns are:

- Intelligence
- Context
- State
- Tools
- Memory
- Retrieval
- Agents
- Policies
- Verification
- Human oversight
- Evaluation
- Observability
- Security
- Cost
- Model lifecycle

The central architectural principle is:

> **AI can reason and propose actions, but the application must remain responsible for state, policy, authorization, and control.**

AI-native systems should therefore separate:

```text
Intelligence
      +
Context
      +
Action
      +
State
      +
Policy
      +
Verification
      +
Observation
```

The architecture must also recognize that AI introduces a new kind of uncertainty.

Traditional software asks:

```text
Does the program execute correctly?
```

AI-native architecture must additionally ask:

```text
Did the model understand the task?

Did it receive the right context?

Did it retrieve the right evidence?

Did it choose the right tool?

Was the action authorized?

Was the result correct?

Can we verify it?

What happens when it is wrong?

What is the blast radius?

How much did it cost?

Can we reproduce and evaluate the behavior?
```

A production AI system is therefore not simply:

```text
Application + LLM
```

It is closer to:

```text
Application
    +
AI Runtime
    +
Context Engineering
    +
Tools
    +
Retrieval
    +
Memory
    +
Policy
    +
Verification
    +
Evaluation
    +
Observability
    +
Security
    +
Human Control
```

The ultimate architectural objective is not maximum autonomy.

It is:

> **Reliable, observable, bounded, verifiable, and economically sustainable intelligence embedded into software systems.**

Architecture becomes executable when AI behavior is governed through:

- deterministic state
- explicit contracts
- permission policies
- bounded agent loops
- sandboxing
- network controls
- structured outputs
- verification
- human gates
- evaluation suites
- failure experiments
- observability
- measurable quality, speed, and cost

**AI-native architecture is not about putting AI everywhere. It is about designing systems where intelligence becomes a controlled architectural capability.**
