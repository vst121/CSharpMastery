# Modular Architecture

## 1. Essence

**Modular Architecture** structures a software system as a set of **explicit, cohesive, and independently understandable modules**, each responsible for a well-defined capability or domain area.

A module owns its internal implementation and exposes only the contracts required by other modules.

The fundamental idea is:

> **Strong internal cohesion, explicit external contracts, controlled dependencies.**

Modularity is primarily about **managing complexity and change**, not about deployment topology.

A modular system may exist within:

- a single process
- a modular monolith
- multiple services
- a distributed system

The architectural boundary is the important part. The deployment boundary is a separate decision.

---

## 2. Problem

As software grows, dependencies tend to spread across the system.

Without explicit boundaries:

```text
Feature A ──→ Service B ──→ Repository C
     │             │
     └──────→ Database D
                   │
Feature E ─────────┘
```

````

The result is increasing:

- coupling
- cognitive load
- change amplification
- regression risk
- testing complexity
- ownership ambiguity
- architectural erosion

Modular Architecture addresses this by creating **intentional boundaries around change and responsibility**.

---

## 3. Intent

Modular Architecture aims to:

1. Localize business responsibilities.
2. Reduce unnecessary coupling.
3. Increase cohesion.
4. Make dependencies explicit.
5. Protect internal implementation details.
6. Enable independent testing.
7. Make architectural rules enforceable.
8. Allow the system to evolve without uncontrolled dependency growth.

---

## 4. Core Principles

### 4.1 Explicit Boundaries

Every module has a defined boundary.

```text
┌───────────────────────┐
│       Module A        │
│                       │
│  Internal Components  │
│                       │
│      ┌─────────┐      │
│      │ Contract│      │
│      └────┬────┘      │
└───────────┼───────────┘
            │
            ▼
       Other Modules
```

The boundary should be understandable by developers and enforceable by tooling.

---

### 4.2 High Cohesion

Code that changes for the same reason should live close together.

A module should represent a meaningful:

- business capability
- bounded responsibility
- domain area
- architectural responsibility

It should not merely represent a technical layer.

---

### 4.3 Controlled Coupling

Modules should depend on **stable contracts**, not implementation details.

```text
Module A
   │
   ▼
Public Contract
   │
   ▼
Module B
```

Not:

```text
Module A
   │
   └──────→ Module B.Internal.Service
```

The goal is not to eliminate coupling.

The goal is to make coupling:

- intentional
- visible
- minimal
- directional
- enforceable

---

### 4.4 Information Hiding

A module should hide decisions that other modules do not need to know.

Typically internal:

```text
Domain model
Database implementation
Internal services
Algorithms
Infrastructure
Persistence details
Caching implementation
```

Typically exposed:

```text
Commands
Queries
DTOs
Integration events
Explicit interfaces
Module contracts
```

The public surface should remain significantly smaller and more stable than the implementation behind it.

---

### 4.5 Ownership

Every important piece of behavior and state should have an identifiable owner.

A fundamental question is:

> **Which module is responsible for maintaining this invariant?**

Ownership should not be ambiguous.

---

### 4.6 Dependency Direction

Dependencies should follow intentional architectural rules.

For example:

```text
API
 │
 ▼
Application
 │
 ▼
Domain
```

Infrastructure implements abstractions rather than becoming part of the domain model.

At the module level:

```text
Module A ──→ Module B.Contracts
Module B ──→ Module C.Contracts
Module C ──→ Module A.Contracts
```

Circular dependencies should normally be treated as an architectural smell.

---

## 5. Architectural Model

A modular system can be represented as:

```text
                    System
                      │
        ┌─────────────┼─────────────┐
        │             │             │
        ▼             ▼             ▼
    Module A       Module B       Module C
        │             │             │
   ┌────┴────┐   ┌────┴────┐   ┌────┴────┐
   │ Internal│   │ Internal│   │ Internal│
   │  State  │   │  State  │   │  State  │
   └────┬────┘   └────┬────┘   └────┬────┘
        │             │             │
        └──── Contracts / Events ───┘
```

The system is therefore not merely a collection of namespaces or projects.

It is a **graph of controlled dependencies**.

A useful mental model is:

```text
Module = Responsibility + Ownership + Boundary + Contract
```

---

## 6. Module Anatomy

A practical .NET module can be organized as:

```text
Module/
├── Domain/
├── Application/
├── Infrastructure/
├── Contracts/
└── Tests/
```

### Domain

Contains:

- entities
- value objects
- aggregates
- domain services
- domain events
- business invariants

The domain should not depend on infrastructure concerns.

### Application

Coordinates use cases.

Typical responsibilities:

- commands
- queries
- handlers
- application services
- validation
- orchestration

### Infrastructure

Contains technical implementations:

- database access
- external APIs
- messaging
- filesystem
- caching
- infrastructure services

### Contracts

Defines what the module intentionally exposes.

Examples:

```text
Commands
Queries
DTOs
Integration events
Public interfaces
```

Contracts should remain smaller and more stable than the implementation behind them.

### Tests

Tests the module's behavior and architectural boundaries.

---

## 7. Folder Structure

A larger modular system might look like:

```text
Modules/
│
├── Transactions/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   ├── Contracts/
│   └── Tests/
│
├── Accounts/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   ├── Contracts/
│   └── Tests/
│
├── Risk/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   ├── Contracts/
│   └── Tests/
│
└── Notifications/
    ├── Domain/
    ├── Application/
    ├── Infrastructure/
    ├── Contracts/
    └── Tests/
```

The exact physical structure can vary.

The important requirement is that the **physical structure reinforces the logical boundary**.

A namespace or folder named `Transactions` is not sufficient by itself. The architecture must prevent unrelated code from freely reaching into its internals.

---

## 8. Dependency Rules

A module should distinguish between different dependency levels.

### Allowed

```text
Module A
   │
   ▼
Module B.Contracts
```

### Discouraged

```text
Module A
   │
   ▼
Module B.Application
```

### Forbidden

```text
Module A
   │
   ▼
Module B.Infrastructure
```

And especially:

```text
Module A
   │
   ▼
Module B
   └── InternalImplementation
```

The objective is not to eliminate dependencies.

The objective is to make dependencies **intentional, visible, and enforceable**.

---

## 9. Communication

Modules generally communicate through three architectural mechanisms.

### 9.1 Direct Contract

```text
A ──→ B.Contract
```

Useful when immediate interaction is required.

The caller depends on a stable contract rather than the implementation.

---

### 9.2 Event-Based Communication

```text
A
 │
 └── publishes event
          │
          ▼
       Event Bus
          │
      ┌───┴───┐
      ▼       ▼
      B       C
```

Useful when consumers should not depend directly on the producer's implementation.

Examples:

- domain events
- integration events
- in-process events
- message broker events

The event contract should represent a meaningful fact rather than expose internal implementation details.

---

### 9.3 Shared Infrastructure

Examples:

- message broker
- cache
- telemetry infrastructure
- configuration infrastructure

Shared infrastructure should not become a backdoor for bypassing module boundaries.

For example:

```text
Module A ──→ SharedDatabase ──→ Module B
```

does not automatically constitute modular communication.

If modules directly manipulate each other's state, the architectural boundary has effectively been weakened.

---

## 10. State & Ownership

One of the most important questions in modular architecture is:

> **Who owns the state?**

A module should normally own the data required to maintain its business invariants.

Other modules should not directly manipulate that state.

Conceptually:

```text
Transactions
    │
    └── owns Transaction state

Risk
    │
    └── owns Risk state

Notifications
    │
    └── owns Notification state
```

Avoid:

```text
Risk ──→ Transactions.Tables
```

when the Transactions module owns those tables.

Data ownership is an architectural boundary, not merely a database decision.

---

## 11. Main Design Questions

Before creating a module, ask:

### Responsibility

- What responsibility does this module own?
- What business capability does it represent?
- Which invariants does it protect?
- What should change inside this module without affecting others?

### Boundary

- What does the module expose?
- What must remain private?
- What is explicitly forbidden from crossing the boundary?
- Is the boundary based on business responsibility or technical convenience?

### Ownership

- Which state does the module own?
- Which module owns each business invariant?
- Who is responsible for changing that state?

### Dependencies

- Which modules does it depend on?
- Why does each dependency exist?
- Is the dependency stable?
- Can the dependency be inverted or removed?

### Communication

- Should communication be direct or event-based?
- Is synchronous interaction actually required?
- What contract crosses the boundary?
- Who owns the contract?

### Evolution

- Can the module evolve independently?
- Can its implementation change without affecting consumers?
- Could the module eventually require independent deployment or scaling?
- Is the boundary stable enough to survive future changes?

---

## 12. Important Constraints

Modularity does **not** automatically mean:

- microservices
- separate databases
- separate deployments
- asynchronous communication
- independent teams
- independent scaling

A module can remain inside the same process and still have a strong architectural boundary.

Likewise, physically separating projects does not automatically create modularity.

```text
Five projects
     ≠
Five modules
```

A real module requires **behavioral and dependency boundaries**, not merely folders or `.csproj` files.

---

## 13. Architectural Invariants

The following invariants should normally be explicit:

1. Every module has a clearly defined responsibility.
2. Every important business capability has an identifiable owner.
3. Module internals are not accessed directly by other modules.
4. Cross-module dependencies use explicit contracts.
5. Business invariants remain inside the owning module.
6. Module boundaries are enforced by automated architecture tests.
7. Circular dependencies are prohibited unless explicitly justified.
8. Shared code must not become an uncontrolled coupling mechanism.
9. Data ownership is explicit.
10. Architectural boundaries should survive refactoring.

These invariants should be treated as **executable architecture**, not merely documentation.

---

## 14. Failure Modes

### 14.1 Shared Kernel Explosion

A supposedly shared library becomes a dumping ground:

```text
Common/
├── Utilities/
├── Helpers/
├── Services/
├── Models/
├── Repositories/
└── EverythingElse/
```

The shared dependency becomes a coupling hub.

A shared component should exist only when the shared concept is genuinely stable and intentionally owned.

---

### 14.2 Boundary Leakage

Internal implementation becomes accessible:

```text
Module A
   │
   └──→ Module B.Infrastructure
```

The boundary exists physically but not architecturally.

---

### 14.3 Distributed Monolith

Services or modules become highly dependent on each other:

```text
A → B → C → D → A
```

The system may be physically distributed while remaining tightly coupled logically.

---

### 14.4 Anemic Modules

A module becomes little more than:

```text
Controller
   ↓
Service
   ↓
Repository
```

with business rules scattered across layers.

The module boundary exists, but meaningful domain ownership does not.

---

### 14.5 Shared Database Coupling

Multiple modules directly manipulate the same tables.

This makes:

- schema evolution
- ownership
- invariants
- testing
- independent changes

difficult to control.

---

### 14.6 Circular Dependencies

For example:

```text
A → B
↑   ↓
└── C
```

Circular dependencies increase change amplification and make module ownership unclear.

They should require explicit architectural justification if allowed at all.

---

## 15. Common Misuse

Avoid creating modules based only on technical categories:

```text
UserModule
DatabaseModule
RepositoryModule
ServiceModule
ControllerModule
```

when the actual business boundaries are elsewhere.

Prefer boundaries around meaningful responsibilities:

```text
Orders
Payments
Risk
Inventory
Shipping
```

Also avoid creating a module for every class.

> **Modularity is not fragmentation.**

Another common mistake is creating abstractions purely to make the architecture look modular.

An interface does not create a boundary if the underlying dependency remains tightly coupled.

---

## 16. Testing Strategy

A modular architecture should have several levels of tests.

### Behavioral Tests

Verify module behavior and business rules.

### Integration Tests

Verify infrastructure interactions.

### Contract Tests

Verify exposed contracts.

### Architecture Tests

Verify dependency rules.

Examples:

```text
Transactions must not reference Risk.Infrastructure.

Risk must not access Transactions.Domain internals.

Modules must not depend on each other circularly.

Infrastructure must not be referenced by Domain.

Public module contracts must not depend on internal implementation types.
```

Architecture tests transform architectural intent into executable constraints.

---

## 17. Observability

Module boundaries should also be visible at runtime.

Useful telemetry dimensions include:

```text
module
operation
dependency
event
duration
error
correlation-id
trace-id
```

For example:

```text
module=Risk
operation=EvaluateTransaction
duration=42ms
```

This becomes particularly important when logical module boundaries evolve toward process or service boundaries.

A useful principle is:

> **If an architectural boundary matters to the design, it should be observable when practical.**

---

## 18. Scalability Considerations

Modularity should support several forms of scalability.

### Codebase Scalability

More features without uncontrolled coupling.

### Team Scalability

Teams can work within bounded areas with fewer accidental dependencies.

### Runtime Scalability

Modules with different workloads can potentially evolve toward independent scaling.

### Organizational Scalability

Ownership can align with business capabilities.

The key principle:

> **First establish logical boundaries. Then decide whether those boundaries need physical separation.**

---

## 19. Security Considerations

Module boundaries can provide useful security boundaries.

Consider:

- authorization ownership
- sensitive data ownership
- access policies
- secrets
- audit requirements
- data minimization
- cross-module access

A module should not expose sensitive internal state merely because another module can technically access its assembly or database.

Security boundaries should be aligned with actual data and capability ownership.

---

## 20. Evolution & Migration

A useful modular architecture allows boundaries to evolve.

A possible evolution path is:

```text
Monolithic Codebase
       ↓
Logical Modules
       ↓
Enforced Module Boundaries
       ↓
Independent Data Ownership
       ↓
Explicit Module Communication
       ↓
Selective Physical Separation
```

The important principle is:

> **Do not introduce distributed-system complexity before the business and architectural boundary requires it.**

Modularity should make future architectural change easier, not force a particular deployment model today.

---

## 21. .NET Implementation Notes

Modern .NET provides several mechanisms for enforcing modularity.

Useful techniques include:

- separate projects and assemblies
- `internal` visibility
- explicit public contracts
- dependency injection
- module-specific composition roots
- architecture tests
- Roslyn analyzers
- source generators where appropriate
- namespace conventions
- module-specific integration tests

For stronger boundaries, prefer **compile-time enforcement** over conventions whenever practical.

For example:

```text
Transactions.Domain
       │
       ├── public → intentional API
       │
       └── internal → implementation
```

`internal` is particularly useful for preventing accidental exposure of implementation details.

### Assembly Boundaries

Separate assemblies can provide stronger enforcement than namespaces alone.

```text
Transactions.Domain.dll
Transactions.Application.dll
Transactions.Infrastructure.dll
Transactions.Contracts.dll
```

However, assemblies are an implementation mechanism, not the architectural boundary itself.

### Dependency Injection

Dependency Injection should compose modules without allowing the host to bypass their internal boundaries.

Prefer:

```text
Host
  │
  ├── AddTransactions()
  ├── AddRisk()
  └── AddNotifications()
```

over exposing every internal implementation to the host.

### Architecture Tests

Use automated rules to enforce:

- dependency direction
- forbidden references
- namespace boundaries
- public API constraints
- module isolation
- absence of cycles

The architecture should fail the build when a critical invariant is violated.

---

## 22. Production Checklist

Before considering a modular architecture healthy, verify:

```text
[ ] Module responsibilities are explicit
[ ] Module ownership is clear
[ ] Boundaries are documented
[ ] Internal implementation is hidden
[ ] Public contracts are intentional
[ ] Dependencies are directional
[ ] Circular dependencies are prevented
[ ] Data ownership is explicit
[ ] Cross-module communication is intentional
[ ] Architecture tests exist
[ ] Module behavior is independently testable
[ ] Shared code is controlled
[ ] Observability identifies important module boundaries
[ ] Security boundaries are considered
[ ] Evolution paths are understood
[ ] Architectural invariants are executable
```

---

## 23. Key Takeaways

1. **Modularity is about boundaries, not folders.**

2. **A module should own a meaningful responsibility.**

3. **High cohesion and controlled coupling are the fundamental goals.**

4. **Internal implementation should remain private.**

5. **Contracts should define intentional dependencies.**

6. **Data ownership is part of the architecture.**

7. **Architectural boundaries should be executable through tests and tooling.**

8. **A modular system does not need to be distributed.**

9. **Modularity should reduce the cost of change.**

10. **The strongest boundary is one that is understandable, enforceable, observable, and evolvable.**

```
````
