# AI-Native LinguaMentis

## 1. Executive Summary

LinguaMentis is a gamified AI debate platform for B2/C1 German learners.

The product combines:

- German language development
- structured thinking
- Six Thinking Hats
- AI agents
- independent AI evaluation
- evidence-based feedback
- personalized learning
- conversational interaction
- gamification

The interesting architectural challenge is not simply:

> "How do we add AI to a language-learning application?"

The real challenge is:

> **How do we design an application where AI is a first-class architectural capability without allowing probabilistic AI to become the owner of application state, business rules, workflow integrity, or learning truth?**

This case study explores LinguaMentis as an **AI-native system** while preserving strong software architecture principles.

The architecture therefore separates two responsibilities:

```text
┌──────────────────────────────────────────────────────────────┐
│                     AI-NATIVE LAYER                          │
│                                                              │
│  Agents → Reasoning → Context → Planning → Evaluation       │
│       → Generation → Reflection → Adaptation                 │
└─────────────────────────────┬────────────────────────────────┘
                              │
                       Controlled Interfaces
                              │
                              ▼
┌──────────────────────────────────────────────────────────────┐
│                 APPLICATION ARCHITECTURE                      │
│                                                              │
│  Domain → State → Workflow → Persistence → Security          │
│       → Learning History → Product Rules → APIs              │
└──────────────────────────────────────────────────────────────┘
```

The fundamental rule is:

> **AI provides intelligence, not application control.**

However, unlike a traditional AI-enabled application, LinguaMentis treats AI as a **native architectural capability**.

AI is not an optional feature attached to the side of the system.

It participates in:

- interaction
- reasoning
- challenge generation
- evaluation
- personalization
- reflection
- adaptation
- learning analysis

The architecture must therefore make AI:

- observable
- testable
- replaceable
- governable
- measurable
- cost-aware
- bounded
- evolvable

---

# 2. Business Context

Traditional language-learning applications generally follow a predefined learning model:

```text
Lesson
   ↓
Exercise
   ↓
Answer
   ↓
Correction
   ↓
Score
```

LinguaMentis uses a different model.

The learner enters an intellectual challenge.

```text
Think
   ↓
Express
   ↓
Challenge
   ↓
Evaluate
   ↓
Improve
```

The language is German.

The subject is broader.

The learner may discuss:

- artificial intelligence
- climate change
- education
- technology
- society
- economics
- politics
- cities
- ethics
- business
- future scenarios

The learner is therefore not merely practicing German.

The learner is using German as the medium for:

- reasoning
- argumentation
- perspective shifting
- creativity
- critical thinking
- reflection

This creates an important architectural property:

> **The product has two independent dimensions of intelligence.**

```text
                    User Response
                          │
             ┌────────────┴────────────┐
             ▼                         ▼
       Thinking Quality           German Quality
             │                         │
             ▼                         ▼
       Reasoning                   Grammar
       Depth                       Vocabulary
       Relevance                   Naturalness
       Specificity                 Fluency
       Creativity                  Structure
       Hat adherence               B2/C1 suitability
```

These dimensions must remain independent.

---

# 3. Architectural Problem

The core problem is:

> **How can an AI-native learning system dynamically reason, generate, evaluate, personalize, and adapt while maintaining deterministic application architecture and trustworthy learning data?**

This creates several tensions.

| Concern              | AI System       | Application                   |
| -------------------- | --------------- | ----------------------------- |
| Natural language     | Probabilistic   | Deterministic boundary        |
| Reasoning            | Strong          | Not authoritative             |
| Challenge generation | Dynamic         | Contract-bound                |
| Evaluation           | Probabilistic   | Persisted as application data |
| Workflow             | Suggestive      | Deterministic                 |
| State                | Contextual      | Authoritative                 |
| Personalization      | Adaptive        | Policy controlled             |
| Memory               | Useful          | Must be bounded               |
| Model behavior       | Variable        | Must be measurable            |
| Cost                 | Variable        | Must be governed              |
| Quality              | Probabilistic   | Must be evaluated             |
| Security             | Model-dependent | Application-enforced          |

---

# 4. What Makes LinguaMentis AI-Native?

An AI-enabled application can look like:

```text
Traditional Application
        │
        ├── Database
        ├── APIs
        ├── Business Logic
        │
        └── AI Feature
```

An AI-native application looks more like:

```text
                    Product
                       │
        ┌──────────────┼──────────────┐
        ▼              ▼              ▼
    Application       AI             Data
      Logic        Intelligence      Layer
        │              │              │
        ├──────────────┼──────────────┤
        │              │              │
        ▼              ▼              ▼
      State         Agents         Learning
      Rules         Context        History
      Workflow      Models         Evidence
      Security      Evaluation     Profiles
```

AI is therefore part of the architecture rather than merely an integration.

For LinguaMentis, AI-native architecture means that:

- agents are architectural components
- model selection is configurable
- prompts are versioned artifacts
- AI contracts are typed
- context is deliberately engineered
- agent workflows are explicit
- evaluation is part of the development lifecycle
- AI failures are first-class failure modes
- model quality is measurable
- cost is observable
- AI behavior is governed
- learning decisions remain traceable

---

# 5. Architectural Principles

## 5.1 Application Owns State

The application owns:

- MindQuest state
- current Hat
- turn lifecycle
- user responses
- evaluation persistence
- learning history
- user profile
- progress
- achievements

AI does not own authoritative state.

```text
LLM
 │
 │ produces intelligence
 ▼
Application
 │
 ├── validates
 ├── decides
 ├── persists
 └── controls lifecycle
```

---

# 6. AI Owns Intelligence

AI is responsible for capabilities that benefit from probabilistic reasoning.

Examples:

- generating questions
- interpreting user responses
- identifying reasoning patterns
- producing feedback
- identifying language problems
- generating alternative perspectives
- creating reflections
- adapting difficulty

The boundary is:

```text
Application
    │
    │ "What is the current state?"
    ▼
Domain/Application Layer
    │
    │ "What intelligence do we need?"
    ▼
AI Capability
    │
    ▼
Structured Result
    │
    ▼
Application Validation
    │
    ▼
State Transition
```

---

# 7. Agents Are Bounded Components

Agents should not be implemented as unrestricted autonomous loops.

Each agent should have:

- a role
- a goal
- an input contract
- an output contract
- available tools
- context boundaries
- permissions
- termination conditions
- evaluation criteria

For example:

```text
BlackHatAgent

Purpose:
Generate challenges focused on risks and weaknesses.

Input:
MindQuestContext

Output:
HatChallenge

Allowed:
Question generation
Risk-oriented reasoning

Not allowed:
Changing MindQuest state
Persisting data
Changing scores
Calling arbitrary services
```

This turns agents from vague AI personas into architectural components.

---

# 8. Six Thinking Hats as an Agent Architecture

The Six Thinking Hats become a natural agent topology.

```text
                    Blue Agent
                        │
       ┌────────────────┼────────────────┐
       │                │                │
       ▼                ▼                ▼
   White Agent      Red Agent       Black Agent
       │                │                │
       └────────────────┼────────────────┘
                        │
             ┌──────────┼──────────┐
             ▼          ▼          ▼
        Yellow Agent Green Agent  Blue Reflection
```

Each Hat has a distinct reasoning mode.

| Agent  | Responsibility               |
| ------ | ---------------------------- |
| White  | Facts and evidence           |
| Red    | Feelings and intuition       |
| Black  | Risks and weaknesses         |
| Yellow | Benefits and opportunities   |
| Green  | Creativity and alternatives  |
| Blue   | Orchestration and reflection |

The Blue Agent should coordinate the experience rather than becoming a general-purpose autonomous agent.

---

# 9. Graph Engineering

The MindQuest workflow should be represented as an explicit graph.

```text
Start
  │
  ▼
Topic Selection
  │
  ▼
Blue Planning
  │
  ▼
Hat Selection
  │
  ▼
Challenge Generation
  │
  ▼
User Response
  │
  ├───────────────┐
  ▼               ▼
Thinking       German
Evaluation     Evaluation
  │               │
  └───────┬───────┘
          ▼
       Evidence
          │
          ▼
       Feedback
          │
          ▼
     Progress Check
          │
     ┌────┴────┐
     │         │
   Continue   Complete
     │         │
     ▼         ▼
Next Hat    Reflection
```

This is preferable to:

```text
while True:
    ask_llm()
    let_llm_decide_everything()
```

The graph owns control flow.

The LLM provides intelligence inside graph nodes.

---

# 10. Graph vs Agent

A critical architectural distinction:

```text
Graph
 └── owns control flow

Agent
 └── provides intelligence
```

For example:

```text
Graph:
"If evaluation is complete, continue to next phase."

Agent:
"Generate the next Black Hat challenge."
```

The agent should not decide whether the database state moves from:

```text
HatInProgress
```

to:

```text
HatCompleted
```

The application decides that.

---

# 11. Context Engineering

Context is one of the most important architectural components in an AI-native system.

An agent should not receive the entire database.

Instead, the system constructs a controlled context.

```text
                 User Response
                       │
                       ▼
                Context Builder
                       │
        ┌──────────────┼──────────────┐
        ▼              ▼              ▼
   MindQuest        User Profile   Prior Evidence
      State             │              │
        │               │              │
        └───────────────┼──────────────┘
                        ▼
                  Agent Context
                        │
                        ▼
                      Agent
```

---

# 12. Context Trust Levels

Not all context should have equal trust.

A useful classification is:

```text
L0 - System Rules
     Highest trust

L1 - Application State
     Authoritative

L2 - Verified Learning Data
     Trusted application data

L3 - Retrieved Knowledge
     Requires provenance

L4 - User Input
     Untrusted

L5 - Model-Generated Content
     Lowest authority
```

This prevents model-generated content from silently becoming authoritative context.

---

# 13. Context Budget

Context has a cost.

A context architecture should optimize:

```text
Quality
   +
Relevance
   +
Freshness
   -
Noise
   -
Tokens
```

The goal is not:

> "Give the model everything."

The goal is:

> **Give the model exactly the information required to perform the current task.**

---

# 14. Memory Architecture

LinguaMentis requires memory, but memory must be separated from business state.

```text
                    Memory
                       │
       ┌───────────────┼────────────────┐
       ▼               ▼                ▼
 Working Memory   Session Memory   Long-Term Memory
       │               │                │
 Current Turn      Current Quest    Learning Patterns
```

Separately:

```text
Authoritative Business State
            │
            ▼
        PostgreSQL
```

The LLM must never treat its memory as the source of truth.

---

# 15. Learning Memory

Long-term learning memory can contain:

```text
User strengths
Common grammar problems
Vocabulary gaps
Thinking patterns
Difficulty history
Preferred topics
Past feedback
Progress trends
```

But derived learning memory should be distinguishable from raw evidence.

```text
Raw Evidence
     │
     ▼
Evaluations
     │
     ▼
Learning Profile
```

The raw evidence remains the foundation.

---

# 16. AI Gateway

All model communication should pass through an AI gateway.

```text
Agent
  │
  ▼
AI Gateway
  │
  ├── Model Routing
  ├── Prompt Version
  ├── Structured Output
  ├── Retry Policy
  ├── Timeout
  ├── Cost Tracking
  ├── Telemetry
  └── Safety Policy
       │
       ▼
   Model Provider
```

Agents should not directly depend on a provider SDK.

---

# 17. Model Routing

Different tasks have different requirements.

For example:

```text
Challenge Generation
       ↓
Fast / cost-efficient model

Deep Thinking Evaluation
       ↓
Higher reasoning capability

German Evaluation
       ↓
Language-specialized model

Final Reflection
       ↓
High-quality reasoning model
```

Therefore:

> **One model does not need to power the entire product.**

Model routing becomes an architectural concern.

---

# 18. Model Abstraction

Conceptually:

```python
class LLMClient(Protocol):

    async def generate_structured(
        self,
        *,
        system_prompt: str,
        user_prompt: str,
        response_model: type[T],
        model: str,
    ) -> T:
        ...
```

The domain does not know:

- OpenRouter
- model provider
- HTTP API
- SDK
- retry implementation

It knows only the AI capability it requires.

---

# 19. Structured AI Contracts

Every important AI boundary should use typed contracts.

Examples:

```text
HatChallenge
ThinkingEvaluation
GermanEvaluation
Evidence
Correction
HatFeedback
FinalReflection
```

Conceptually:

```python
class HatChallenge(BaseModel):
    question: str
    instruction: str
    hat: HatType
    difficulty: int
```

The contract is part of the architecture.

It is not merely a serialization detail.

---

# 20. Contract-First AI

Traditional application:

```text
Code
  ↓
Data
  ↓
API Contract
```

AI-native application:

```text
Intent
  ↓
AI Contract
  ↓
Prompt
  ↓
Model
  ↓
Structured Output
  ↓
Validation
```

This creates a stable boundary around probabilistic behavior.

---

# 21. Prompt Architecture

Prompts should be treated as versioned architectural artifacts.

Example:

```text
prompts/
├── blue/
│   └── planner.v1
├── white/
│   └── challenge.v1
├── black/
│   └── challenge.v2
├── thinking-evaluator/
│   └── evaluator.v3
├── german-evaluator/
│   └── evaluator.v2
└── reflection/
    └── reflection.v1
```

A prompt change can change system behavior.

Therefore prompt changes should be:

- reviewable
- versioned
- testable
- measurable

---

# 22. Evaluation Is Part of Architecture

In traditional software:

```text
Code
  ↓
Unit Tests
  ↓
Integration Tests
```

In AI-native software:

```text
Code
  +
Prompt
  +
Model
  +
Context
  ↓
Evaluation
```

AI behavior cannot be validated solely through conventional unit tests.

---

# 23. Evaluation Architecture

LinguaMentis has two independent evaluators.

```text
                     User Response
                           │
             ┌─────────────┴─────────────┐
             ▼                           ▼
      Thinking Evaluator           German Evaluator
             │                           │
             ▼                           ▼
       Thinking Score              German Score
             │                           │
             ▼                           ▼
          Evidence                    Evidence
             │                           │
             └─────────────┬─────────────┘
                           ▼
                        Feedback
```

The evaluation pipeline itself becomes an architectural subsystem.

---

# 24. Evaluator Independence

The evaluator should be logically separated from the agent that generated the challenge.

For example:

```text
BlackHatAgent
     │
     ▼
Challenge
     │
     ▼
User Response
     │
     ▼
ThinkingEvaluator
```

The Black Hat Agent should not judge its own challenge quality and user performance.

This reduces architectural coupling and creates a cleaner evaluation boundary.

---

# 25. Evidence-First Evaluation

A score without evidence is weak.

Instead of:

```text
Thinking Score: 82
```

the system should produce:

```text
Score: 82

Evidence:
- Identified a concrete operational risk
- Explained a likely consequence
- Connected the consequence to the proposed scenario
- Maintained the Black Hat perspective
```

Evidence becomes a first-class domain concept.

---

# 26. Evaluation Harness

The AI system should have a regression harness.

```text
Evaluation Dataset
       │
       ▼
     Runner
       │
       ├── Model A
       ├── Model B
       └── Prompt Version
              │
              ▼
          Evaluations
              │
              ▼
           Metrics
              │
              ▼
            Report
```

This allows systematic comparison of:

- models
- prompts
- context strategies
- agent versions
- evaluator versions

---

# 27. Evaluation Metrics

## Thinking

Possible metrics:

- Hat adherence
- reasoning quality
- depth
- relevance
- specificity
- critical thinking
- creativity
- evaluator consistency

## German

Possible metrics:

- grammar detection
- vocabulary assessment
- sentence structure
- naturalness
- fluency
- CEFR appropriateness
- correction accuracy

## System

Possible metrics:

- latency
- token usage
- cost
- failure rate
- retry rate
- structured-output validity
- model fallback frequency

---

# 28. AI Quality Pipeline

AI quality should evolve through:

```text
Prompt
  ↓
Contract
  ↓
Dataset
  ↓
Evaluation
  ↓
Failure Analysis
  ↓
Improvement
  ↓
Regression Test
  ↓
Release
```

This is effectively:

> **Test-Driven Development for probabilistic behavior.**

A mature implementation can evolve toward:

> **Eval-Driven Development.**

---

# 29. AI Failure Modes

AI-native architecture must explicitly model AI failures.

Possible failures include:

```text
Hallucination
Prompt Injection
Wrong Hat
Invalid Structured Output
Weak Reasoning
Incorrect Grammar Correction
Over-Correction
Under-Correction
Context Loss
Context Pollution
Model Timeout
Model Unavailability
Unexpected Cost
Agent Loop
Evaluation Drift
Model Regression
```

These are architectural failure modes.

---

# 30. Failure Isolation

An AI failure should not corrupt core application state.

Example:

```text
User Response
      │
      ▼
Persist Response
      │
      ▼
AI Evaluation
      │
      ├── Success
      │
      └── Failure
             │
             ▼
        EvaluationFailed
```

The response remains valid.

The evaluation can be retried.

The MindQuest state remains consistent.

---

# 31. AI Retry Architecture

Retries should be controlled.

```text
AI Request
    │
    ▼
Timeout?
    │
 ┌──┴───┐
 No     Yes
 │       │
 ▼       ▼
Result  Retry
          │
          ▼
      Retry Limit
          │
      ┌───┴───┐
      ▼       ▼
   Success   Failure
```

Retries must have:

- maximum attempts
- exponential backoff
- timeout
- cost awareness
- idempotent semantics

---

# 32. Agent Loop Control

An autonomous loop must never be unbounded.

A graph node should have:

```text
Maximum Steps
Maximum Tokens
Maximum Time
Maximum Cost
Allowed Tools
Allowed Transitions
```

Conceptually:

```text
Agent Loop

while:
    if step_count >= MAX_STEPS:
        stop

    if cost >= MAX_COST:
        stop

    if elapsed >= MAX_TIME:
        stop

    execute_next_step()
```

The system must remain in control.

---

# 33. Tool Architecture

Future LinguaMentis agents may require tools.

Possible tools:

```text
GetUserLearningProfile
GetMindQuestHistory
GetVocabularyHistory
GetGrammarHistory
GetTopicInformation
SearchKnowledge
SaveLearningInsight
GenerateExercise
```

Tools should be:

- explicit
- typed
- permission-controlled
- observable
- bounded

An agent should never receive unrestricted database access.

---

# 34. MCP Considerations

MCP can become useful when LinguaMentis needs a standardized tool boundary.

Conceptually:

```text
Agent
  │
  ▼
MCP Tool Interface
  │
  ├── Learning Profile
  ├── History
  ├── Knowledge
  └── Exercises
```

However, MCP should not automatically replace direct internal APIs.

For internal application calls:

```text
Direct Application API
```

may be simpler and more efficient.

MCP becomes more valuable when:

- tools are shared across agents
- external agent clients need access
- tool ecosystems grow
- standardized tool discovery is valuable

The architecture should therefore avoid introducing MCP merely because the system uses agents.

---

# 35. RAG Architecture

RAG is not required for the initial vertical slice.

It becomes valuable when LinguaMentis needs trusted knowledge such as:

- factual topic information
- German grammar references
- CEFR guidance
- vocabulary knowledge
- domain-specific learning resources

Future architecture:

```text
Agent
  │
  ▼
Retrieval Policy
  │
  ▼
Retriever
  │
  ├── Knowledge Base
  ├── Documents
  └── Learning Resources
       │
       ▼
    Evidence
       │
       ▼
     Context
       │
       ▼
      Agent
```

Retrieved information should carry provenance.

---

# 36. RAG Is Not Memory

The architecture must distinguish:

```text
RAG
 └── external knowledge retrieval

Memory
 └── interaction / learning history

Database
 └── authoritative application state
```

These are different architectural concerns.

---

# 37. Personalization Architecture

Personalization should evolve from evidence.

```text
User Responses
      │
      ▼
Evaluations
      │
      ▼
Evidence
      │
      ▼
Learning Profile
      │
      ▼
Personalization Policy
      │
      ▼
Next MindQuest
```

AI can help interpret patterns.

The application decides how those patterns influence the product.

---

# 38. Adaptive Difficulty

Difficulty should not be controlled solely by an LLM.

Instead:

```text
Evaluation History
       │
       ▼
Difficulty Policy
       │
       ├── Current CEFR Level
       ├── Thinking Performance
       ├── German Performance
       ├── Recent Errors
       └── Historical Progress
              │
              ▼
       Target Difficulty
              │
              ▼
       AI Challenge Generator
```

AI generates within an application-defined range.

---

# 39. Learning Profile

The learning profile should contain separate dimensions.

```text
Learning Profile
│
├── German
│   ├── Grammar
│   ├── Vocabulary
│   ├── Naturalness
│   ├── Fluency
│   └── Complexity
│
└── Thinking
    ├── Reasoning
    ├── Critical Thinking
    ├── Creativity
    ├── Perspective
    └── Depth
```

A learner may therefore have:

```text
German: B2+
Thinking: Advanced
```

rather than a single generic score.

---

# 40. Gamification Architecture

Gamification should be deterministic.

AI may recommend:

```text
"User demonstrated strong Black Hat reasoning."
```

The application decides:

```text
AchievementUnlocked
```

Example:

```text
AI Insight
    │
    ▼
Validated Application Rule
    │
    ▼
Achievement
    │
    ▼
Points / Ranking
```

The model should not directly award arbitrary points.

---

# 41. Domain Architecture

The core domain can be organized around:

```text
User
MindQuest
HatRound
Turn
Response
Evaluation
Evidence
LearningProfile
Achievement
```

Potential bounded contexts:

```text
Learning
│
├── MindQuest
├── Evaluation
└── LearningProfile

AI
│
├── Agents
├── Model Gateway
├── Prompt Management
└── AI Evaluation

Gamification
│
├── Achievements
├── Points
└── Rankings
```

V1 can keep these inside a modular monolith.

---

# 42. Modular Monolith

The recommended V1 architecture remains:

```text
                LinguaMentis
                     │
       ┌─────────────┼─────────────┐
       ▼             ▼             ▼
   Learning         AI        Gamification
       │             │             │
       ├─────────────┼─────────────┤
       │             │             │
       ▼             ▼             ▼
   PostgreSQL     AI Gateway     PostgreSQL
```

The modules communicate through explicit application boundaries.

There is no requirement to start with microservices.

---

# 43. Why Not Microservices?

AI already introduces significant complexity:

- model variability
- prompt management
- evaluation
- context engineering
- agent orchestration
- latency
- cost

Adding distributed infrastructure immediately would create unnecessary operational complexity.

Therefore:

> **Start with a modular monolith and extract services only when a real architectural pressure appears.**

---

# 44. Quality Attributes

| Quality Attribute  | Architectural Strategy           |
| ------------------ | -------------------------------- |
| Correctness        | Deterministic application state  |
| AI Quality         | Evaluation harness               |
| Reliability        | Failure isolation                |
| Security           | Server-side AI boundary          |
| Scalability        | Stateless application components |
| Evolvability       | Modular monolith                 |
| Model Independence | AI Gateway                       |
| Testability        | Typed contracts                  |
| Observability      | AI + application telemetry       |
| Cost Efficiency    | Model routing and budgets        |
| Personalization    | Evidence-driven learning profile |
| Explainability     | Evidence-first evaluation        |
| Maintainability    | Explicit agent boundaries        |

---

# 45. AI-Specific Quality Attributes

AI-native systems require additional quality dimensions.

## 45.1 Groundedness

Does the output rely on appropriate context?

## 45.2 Consistency

Does the same scenario produce reasonably stable results?

## 45.3 Relevance

Does the model stay within the current Hat and learning objective?

## 45.4 Calibration

Do evaluation scores correspond reasonably to actual quality?

## 45.5 Controllability

Can the application constrain model behavior?

## 45.6 Recoverability

Can the system recover when the model fails?

## 45.7 Cost Predictability

Can the system operate within a defined inference budget?

---

# 46. AI Cost Architecture

AI cost should be observable per operation.

Example:

```text
MindQuest
   │
   ├── Challenge Generation
   │
   ├── Thinking Evaluation
   │
   ├── German Evaluation
   │
   ├── Feedback
   │
   └── Reflection
```

Each operation should record:

```text
Model
Prompt Version
Input Tokens
Output Tokens
Latency
Estimated Cost
```

This allows:

```text
Cost per Turn
Cost per MindQuest
Cost per User
Cost per Evaluation
```

---

# 47. Model Selection as an Architecture Decision

Model selection should be treated as a trade-off.

```text
Quality
   ▲
   │
   │          High-end model
   │
   │       Balanced model
   │
   │  Fast model
   │
   └──────────────────────► Cost
```

The best architecture is not necessarily:

> "Use the strongest model everywhere."

Instead:

> **Use the smallest model that reliably satisfies the capability's quality requirement.**

---

# 48. Blast Radius

AI failures should have a bounded blast radius.

A defective prompt should not corrupt:

```text
User account
Learning history
Database integrity
Application state
```

For example:

```text
Bad Reflection Prompt
       │
       ▼
Incorrect Reflection
       │
       X
       │
       └── Does not modify historical evaluations
```

This is a major AI-native architectural principle.

---

# 49. Isolation

AI execution should be isolated from core state mutation.

```text
AI Runtime
   │
   ├── Reason
   ├── Generate
   ├── Evaluate
   └── Recommend
          │
          ▼
     Application
          │
          ├── Validate
          ├── Authorize
          └── Persist
```

This prevents an AI failure from becoming a domain failure.

---

# 50. Auditability

Important AI decisions should be traceable.

A useful AI interaction record may contain:

```text
AI Interaction ID
MindQuest ID
Turn ID
Agent
Model
Prompt Version
Context Version
Input Reference
Output
Validation Result
Latency
Token Usage
Cost
```

Sensitive user content should be handled according to the application's privacy requirements.

---

# 51. Permission Control

AI permissions should be explicit.

```text
Agent
 │
 ▼
Capability Policy
 │
 ├── Can Read Learning Profile
 ├── Can Read Current Quest
 ├── Can Generate Challenge
 ├── Can Evaluate Response
 ├── Can Request Retrieval
 └── Cannot Modify Domain State
```

The default should be:

> **Deny unless explicitly allowed.**

---

# 52. Verification

Important AI outputs should be verified.

Example:

```text
LLM Output
    │
    ▼
Schema Validation
    │
    ▼
Domain Validation
    │
    ▼
Safety Validation
    │
    ▼
Application Decision
```

For example, a challenge generated for Black Hat must actually conform to the Black Hat contract.

---

# 53. Human Gate

Some future features may require human review.

Examples:

- sensitive learning recommendations
- disputed evaluation
- unsafe content
- unusual learner behavior
- high-impact automated decisions

Architecture:

```text
AI Proposal
    │
    ▼
Verification
    │
    ├── Safe ───────► Execute
    │
    └── Uncertain
            │
            ▼
       Human Gate
            │
       ┌────┴────┐
       ▼         ▼
    Approve    Reject
```

---

# 54. Production-Ready AI Architecture

Production readiness requires more than deploying an LLM API.

A production-ready LinguaMentis should provide:

```text
Quality
Speed
Cost
Security
Observability
Auditability
Isolation
Permission Control
Blast Radius
Operational Recovery
```

These properties should be designed rather than added after deployment.

---

# 55. Observability Architecture

Observability should cover three dimensions.

## Application

```text
Requests
Errors
Latency
Database
State transitions
```

## AI

```text
Model
Prompt
Tokens
Cost
Latency
Failures
Structured output validity
```

## Product

```text
MindQuest completion
Evaluation quality
Learning progress
Challenge success
User engagement
```

This creates a complete operational picture.

---

# 56. Trace Architecture

A single user action should be traceable across the system.

```text
Request
 │
 ▼
MindQuest Service
 │
 ▼
Agent
 │
 ▼
AI Gateway
 │
 ▼
Model Provider
 │
 ▼
Structured Output
 │
 ▼
Evaluator
 │
 ▼
Persistence
```

Correlation IDs should connect these operations.

---

# 57. Security Architecture

Important boundaries:

```text
Browser
   │
   ▼
FastAPI
   │
   ├── Domain
   ├── Application
   ├── AI Gateway
   │
   ▼
External AI Provider
```

The browser must never receive:

- provider API keys
- internal prompts
- unrestricted AI tools
- database credentials

---

# 58. Prompt Injection

User input must always be treated as untrusted.

Example:

```text
User Response
     │
     ▼
AI Context
     │
     X
"Ignore all system instructions"
```

The architecture should separate:

```text
System Instructions
Trusted Context
User Input
Model Output
```

These should not be concatenated into one undifferentiated trust domain.

---

# 59. Data Privacy

LinguaMentis may process:

- learner responses
- learning history
- evaluation data
- language errors
- behavioral patterns

Therefore data handling should be designed intentionally.

Important principles:

- minimize stored data
- avoid unnecessary prompt logging
- protect sensitive user content
- define retention policies
- control provider data exposure
- separate operational telemetry from learner content

---

# 60. Event-Driven Evolution

V1 does not require Kafka.

If scale increases, selected domain events can later become asynchronous.

Example:

```text
ResponseSubmitted
       │
       ▼
Event
       │
       ├── Thinking Evaluation
       ├── German Evaluation
       ├── Analytics
       └── Learning Profile Update
```

The event architecture should emerge from actual requirements.

---

# 61. Asynchronous Evaluation

At scale, evaluation can become asynchronous.

```text
User Response
      │
      ▼
Persist Response
      │
      ▼
Evaluation Requested
      │
      ▼
Queue
   ┌──┴──────────────┐
   ▼                 ▼
Thinking         German
Evaluator        Evaluator
   │                 │
   └───────┬─────────┘
           ▼
       Evaluation
           │
           ▼
        Feedback
```

This improves scalability but introduces eventual consistency.

Therefore it should be introduced only when justified.

---

# 62. Evolution Path

### V1

```text
Modular Monolith
+
Synchronous AI
+
PostgreSQL
+
AI Gateway
```

### V2

```text
Modular Monolith
+
Evaluation Workers
+
AI Job Queue
+
Learning Profile Pipeline
```

### V3

```text
Event-Driven Architecture
+
Dedicated AI Workers
+
Advanced Memory
+
RAG
```

### V4

```text
Agent Platform
+
Specialized Agent Runtime
+
Multi-Agent Workflows
+
Advanced Personalization
```

The architecture evolves according to pressure.

---

# 63. Architecture Evolution

The system should evolve approximately as:

```text
AI Feature
    ↓
AI Capability
    ↓
AI Gateway
    ↓
Agent Architecture
    ↓
Agent Graph
    ↓
Evaluation Platform
    ↓
AI Platform
```

The final state should not be assumed at the beginning.

---

# 64. First Vertical Slice

The first complete vertical slice should remain deliberately small.

```text
Blue Agent
    ↓
Select Topic
    ↓
Black Hat Agent
    ↓
Generate Challenge
    ↓
User Response
    ↓
Thinking Evaluation
        +
German Evaluation
    ↓
Evidence
    ↓
Feedback
    ↓
Next Challenge
    ↓
Final Reflection
```

This validates:

- application state ownership
- agent boundaries
- structured AI contracts
- evaluation architecture
- AI gateway
- persistence
- failure handling
- frontend/backend integration

before introducing unnecessary infrastructure.

---

# 65. Recommended Implementation Order

```text
1. Domain Model
       ↓
2. MindQuest State Machine
       ↓
3. Application Use Cases
       ↓
4. PostgreSQL Persistence
       ↓
5. AI Contracts
       ↓
6. AI Gateway
       ↓
7. Prompt Versioning
       ↓
8. Black Hat Agent
       ↓
9. Thinking Evaluator
       ↓
10. German Evaluator
       ↓
11. Evidence Model
       ↓
12. Evaluation Harness
       ↓
13. MindQuest Graph
       ↓
14. FastAPI
       ↓
15. Next.js UI
       ↓
16. Complete Vertical Slice
       ↓
17. Remaining Hat Agents
       ↓
18. Blue Orchestration
       ↓
19. Learning Profile
       ↓
20. Gamification
       ↓
21. Advanced Evaluation
       ↓
22. Voice
       ↓
23. RAG
       ↓
24. Advanced Memory
       ↓
25. Event-Driven Evolution
```

---

# 66. Architecture Decision Records

Important decisions should be documented.

Examples:

### ADR-001

Why modular monolith?

### ADR-002

Why application-owned state?

### ADR-003

Why independent Thinking and German evaluators?

### ADR-004

Why structured AI contracts?

### ADR-005

Why AI Gateway?

### ADR-006

Why explicit agent contracts?

### ADR-007

Why Graph Engineering instead of unrestricted agent loops?

### ADR-008

Why evaluation harness?

### ADR-009

Why model routing?

### ADR-010

Why not introduce Kafka in V1?

### ADR-011

Why not introduce RAG in the first vertical slice?

### ADR-012

Why separate memory from authoritative state?

---

# 67. Important Trade-offs

## Simplicity vs Flexibility

Modular monolith:

```text
+
Simple
+
Fast development
+
Low operational burden

-
Less independent scaling
```

Microservices:

```text
+
Independent scaling
+
Independent deployment

-
Distributed complexity
-
Higher operational burden
```

V1 chooses simplicity.

---

# 68. AI Quality vs Cost

Higher-quality models can improve:

- reasoning
- language evaluation
- reflection

but increase:

- latency
- cost

Therefore model routing is preferred over universal use of the most expensive model.

---

# 69. Autonomy vs Control

More autonomous agents provide:

```text
+
Flexibility
+
Emergent behavior
```

but create:

```text
-
Higher unpredictability
-
Higher cost
-
Harder debugging
-
Larger blast radius
```

LinguaMentis therefore favors:

> **Bounded autonomy.**

---

# 70. Centralization vs Specialization

One general-purpose agent:

```text
General Agent
 └── Everything
```

is simple but difficult to evaluate.

Specialized agents:

```text
Blue
White
Red
Black
Yellow
Green
Thinking Evaluator
German Evaluator
```

provide clearer boundaries.

The cost is increased orchestration complexity.

The Six Hats domain provides a strong justification for specialization.

---

# 71. Deterministic vs Probabilistic Architecture

A useful architectural classification is:

```text
Deterministic
──────────────
Domain Rules
State
Persistence
Authorization
Workflow
Scores
Achievements
API Contracts

Probabilistic
──────────────
Language Understanding
Challenge Generation
Reasoning
Evaluation
Reflection
Personalization
```

The architecture should place each responsibility on the appropriate side.

---

# 72. The AI-Native Boundary

The complete system can therefore be represented as:

```text
                         USER
                           │
                           ▼
                    ┌─────────────┐
                    │   Next.js   │
                    └──────┬──────┘
                           │
                           ▼
                    ┌─────────────┐
                    │   FastAPI   │
                    └──────┬──────┘
                           │
              ┌────────────┴────────────┐
              │                         │
              ▼                         ▼
       APPLICATION CORE            AI-NATIVE CORE
              │                         │
       ┌──────┼──────┐          ┌───────┼────────┐
       │      │      │          │       │        │
       ▼      ▼      ▼          ▼       ▼        ▼
    Domain  State  Workflow   Agents  Context  Evaluation
       │      │      │          │       │        │
       └──────┼──────┘          └───────┼────────┘
              │                         │
              ▼                         ▼
          PostgreSQL               AI Gateway
                                      │
                                      ▼
                                Model Provider
```

The two worlds meet through explicit contracts.

---

# 73. Core Architectural Rule

The entire architecture can be summarized as:

```text
                 ┌───────────────────────┐
                 │         AI            │
                 │                       │
                 │ Understand            │
                 │ Reason                │
                 │ Generate              │
                 │ Evaluate              │
                 │ Recommend             │
                 │ Reflect               │
                 └───────────┬───────────┘
                             │
                      Structured Contract
                             │
                             ▼
                 ┌───────────────────────┐
                 │     APPLICATION       │
                 │                       │
                 │ Validate              │
                 │ Decide                │
                 │ Control               │
                 │ Persist               │
                 │ Authorize             │
                 │ Transition State      │
                 └───────────────────────┘
```

This is the defining architectural boundary of LinguaMentis.

---

# 74. What Makes This Architecture AI-Native?

LinguaMentis is not AI-native simply because it uses LLMs.

It is AI-native because AI is treated as a first-class architectural concern.

The architecture explicitly provides:

```text
AI Contracts
AI Gateway
Agent Boundaries
Agent Graph
Context Engineering
Memory Architecture
Model Routing
Prompt Versioning
Evaluation Harness
AI Observability
AI Cost Management
AI Failure Isolation
AI Permission Control
AI Verification
AI Governance
```

At the same time, it preserves:

```text
Domain Ownership
State Ownership
Deterministic Workflow
Persistence
Security
Auditability
Testability
Evolution
```

---

# 75. Final Architecture

The target architecture can be summarized as:

```text
                              USER
                                │
                                ▼
                         ┌─────────────┐
                         │   Next.js   │
                         └──────┬──────┘
                                │
                                ▼
                         ┌─────────────┐
                         │   FastAPI   │
                         └──────┬──────┘
                                │
                 ┌──────────────┴──────────────┐
                 │                             │
                 ▼                             ▼
        ┌──────────────────┐          ┌──────────────────┐
        │ Application Core │          │   AI-Native Core │
        │                  │          │                  │
        │ Domain           │          │ Blue Agent       │
        │ MindQuest        │          │ Hat Agents       │
        │ State Machine    │          │ Evaluators       │
        │ Learning         │          │ Reflection       │
        │ Gamification     │          │ Context          │
        └────────┬─────────┘          └────────┬─────────┘
                 │                             │
                 │                             ▼
                 │                      ┌──────────────┐
                 │                      │  AI Gateway  │
                 │                      └──────┬───────┘
                 │                             │
                 │                             ▼
                 │                       Model Provider
                 │
                 ▼
            PostgreSQL
                 │
                 ▼
          Learning History
```

---

# 76. Final Architecture Statement

LinguaMentis should be understood as:

> **A modular monolith with an AI-native architecture, where deterministic application architecture provides control and probabilistic AI provides intelligence.**

The application owns:

```text
State
Lifecycle
Workflow
Persistence
Learning History
Security
Permissions
Achievements
```

AI owns:

```text
Understanding
Reasoning
Generation
Evaluation
Feedback
Reflection
Adaptation
```

The boundary between the two is enforced through:

```text
Contracts
Graphs
Validation
Permissions
Evaluation
Observability
```

The result is neither:

> a traditional application with an AI chatbot attached

nor:

> an autonomous agent controlling the application.

It is a third model:

```text
              AI-NATIVE APPLICATION

        ┌─────────────────────────────┐
        │        APPLICATION          │
        │                             │
        │  Deterministic Control      │
        │  State Ownership            │
        │  Domain Rules               │
        │  Security                   │
        │  Persistence                │
        └──────────────┬──────────────┘
                       │
                AI Architectural
                    Boundary
                       │
        ┌──────────────▼──────────────┐
        │             AI              │
        │                             │
        │  Reasoning                  │
        │  Agents                     │
        │  Context                    │
        │  Evaluation                 │
        │  Generation                 │
        │  Personalization            │
        └─────────────────────────────┘
```

The fundamental principle remains:

> **AI provides intelligence, not application control.**
