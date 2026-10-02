# 01 - Monolith to Modular Monolith

## 1. Essence

**Monolith → Modular Monolith** is the evolution of an unstructured or tightly coupled monolith into a system with **explicit internal architectural boundaries** while keeping a single deployable application.

The goal is not to split the system into services.

The goal is to create **strong modules inside the monolith**.

> **One deployment unit, multiple architectural boundaries.**

---

## 2. The Problem

A traditional monolith often starts simple:

```text
┌─────────────────────────────┐
│          Monolith           │
│                             │
│ Orders ── Customers         │
│   │          │              │
│ Payments ── Shipping        │
│   │          │              │
│     Shared Database         │
└─────────────────────────────┘
```

Over time, boundaries become unclear:

- Everything can reference everything.
- Business logic leaks across features.
- Shared models spread everywhere.
- Database tables become globally accessible.
- Changes have unpredictable impact.
- Tests become slow and fragile.
- Teams interfere with each other.
- Architectural decisions become implicit.

The system may still work, but its **architecture has degraded**.

---

## 3. Intent

Transform the monolith from:

```text
Big Ball of Code
```

into:

```text
┌─────────────────────────────────────────┐
│              Application                │
│                                         │
│ ┌─────────┐ ┌─────────┐ ┌────────────┐ │
│ │ Orders  │ │Payments │ │ Customers  │ │
│ └─────────┘ └─────────┘ └────────────┘ │
│                                         │
│       Explicit Module Boundaries        │
└─────────────────────────────────────────┘
```

without introducing distributed-system complexity.

---

## 4. Core Principles

### 4.1 Modules represent business capabilities

Prefer:

```text
Orders
Payments
Customers
Inventory
Shipping
```

over technical modules such as:

```text
Controllers
Services
Repositories
Helpers
Utilities
```

The module should represent **what the system does**, not merely how the code is organized.

---

### 4.2 Explicit boundaries

A module should expose a small public surface.

```text
Orders
 ├── Public API
 ├── Application
 ├── Domain
 └── Infrastructure
```

Other modules should not directly access its internal implementation.

---

### 4.3 Control dependencies

Avoid:

```text
Orders → Customers → Payments → Orders
```

Prefer a dependency graph that is intentional and preferably acyclic.

```text
Orders ─────→ Customers
   │
   └────────→ Payments
```

Architecture tests should enforce these rules.

---

### 4.4 Own data logically

A modular monolith can still use one physical database.

But ownership should be explicit:

```text
Orders module      → Orders tables
Payments module    → Payment tables
Customers module   → Customer tables
```

Physical database sharing does not mean unrestricted logical access.

---

### 4.5 Keep communication explicit

Modules should communicate through defined contracts:

```text
Orders
   │
   ├── Command
   ├── Query
   └── Domain/Event Contract
```

Avoid directly manipulating another module's internal objects or database tables.

---

## 5. Target Architecture

A typical structure:

```text
src/
├── Orders/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   └── Public/
│
├── Payments/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   └── Public/
│
├── Customers/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   └── Public/
│
└── Host/
```

The host remains one deployable application.

---

## 6. Migration Strategy

Do not rewrite the monolith.

Use incremental extraction of boundaries.

```text
1. Understand
      ↓
2. Identify Business Boundaries
      ↓
3. Create Module
      ↓
4. Move Code
      ↓
5. Define Public Contract
      ↓
6. Restrict Dependencies
      ↓
7. Protect with Architecture Tests
      ↓
8. Repeat
```

The migration should continuously improve the architecture without requiring a big-bang rewrite.

---

## 7. Step 1: Discover the Current Architecture

Before changing code, identify:

- major business capabilities
- dependencies
- shared models
- shared services
- database access
- circular dependencies
- high-change areas
- architectural hotspots

Useful techniques:

```text
Code analysis
Dependency graphs
Runtime tracing
Database analysis
Git/change history
Team ownership analysis
```

The first goal is **visibility**, not refactoring.

---

## 8. Step 2: Identify Module Boundaries

A good module usually has:

- clear business responsibility
- cohesive behavior
- explicit data ownership
- understandable dependencies
- independent change patterns

For example:

```text
Order
 ├── Create Order
 ├── Cancel Order
 ├── Calculate Total
 └── Order Lifecycle
```

is usually a stronger boundary than:

```text
OrderController
OrderService
OrderRepository
```

---

## 9. Step 3: Introduce the Module

Create the boundary first.

```text
Orders
├── Domain
├── Application
├── Infrastructure
└── Public
```

Then gradually move existing code into it.

The important change is not the folder structure.

The important change is:

> **Who is allowed to depend on what?**

---

## 10. Step 4: Establish Public Contracts

Example:

```csharp
public interface IOrderService
{
    Task<OrderResult> CreateAsync(CreateOrder command);
}
```

The rest of the system depends on the contract rather than the internal implementation.

```text
Consumer
   │
   ▼
IOrderService
   │
   ▼
Orders Module
```

The internal domain, persistence, and implementation remain private.

---

## 11. Step 5: Establish Data Ownership

Initially:

```text
One Database
```

is perfectly acceptable.

But establish logical ownership:

```text
Orders module
    owns
Orders + OrderItems

Payments module
    owns
Payments + Transactions
```

Avoid:

```csharp
// Orders directly querying Payments tables
db.Payments.Where(...);
```

Prefer:

```text
Orders
   │
   ▼
Payments Public Contract
   │
   ▼
Payments
```

---

## 12. Step 6: Enforce Boundaries

This is where the architecture becomes executable.

Examples of rules:

```text
Orders cannot reference Payments.Infrastructure
Orders cannot access Payments.Domain
Payments cannot access Orders.Infrastructure
Modules cannot access another module's DbContext
Public APIs are the only cross-module entry points
```

These rules should be automated with architecture tests.

For example:

```csharp
[Fact]
public void Orders_Should_Not_Depend_On_Payments_Internal_Implementation()
{
    // Architecture test
}
```

The exact testing framework is less important than making the rule executable.

---

## 13. Step 7: Remove Accidental Coupling

Typical migration targets:

```text
Shared God Service
Shared Entity
Shared Repository
Shared DbContext
Shared Utility
Global Static State
Direct Table Access
Circular Dependencies
```

Replace them with:

```text
Module-owned behavior
Explicit contracts
Domain concepts
Events
Queries
Adapters
```

---

## 14. Architectural Invariants

The modular monolith should enforce:

1. Each module has a clear responsibility.
2. Module boundaries are explicit.
3. Internal implementation is not publicly accessible.
4. Dependencies between modules are intentional.
5. Circular dependencies are prohibited.
6. Modules own their business data logically.
7. Cross-module communication uses explicit contracts.
8. Direct cross-module database access is prohibited.
9. Shared infrastructure does not become shared business logic.
10. Architecture rules are automated where possible.

---

## 15. Common Failure Modes

### Folder-based modularity

```text
Modules/
  Orders/
  Payments/
```

but every module can access everything.

This is only **organizational modularity**, not architectural modularity.

---

### Shared database coupling

```text
Orders → PaymentsTable
Customers → OrdersTable
Payments → CustomersTable
```

The modules appear separated but remain tightly coupled.

---

### Shared domain model

```text
SharedCustomer
SharedOrder
SharedPayment
```

A shared domain model often becomes a hidden coupling mechanism.

---

### God module

One module becomes responsible for everything:

```text
OrderModule
 ├── Orders
 ├── Payments
 ├── Customers
 ├── Shipping
 └── Reporting
```

The name changed, but the architecture did not.

---

### Premature microservices

The team extracts every module into a service simply because boundaries now exist.

That defeats the purpose.

A modular monolith can be the **final architecture**, not merely a temporary step toward microservices.

---

## 16. Testing Strategy

Use several layers:

```text
Unit Tests
    ↓
Integration Tests
    ↓
Architecture Tests
    ↓
Contract Tests
    ↓
End-to-End Tests
```

Architecture tests should verify:

- dependency direction
- forbidden references
- module isolation
- namespace rules
- public API boundaries
- database ownership rules

---

## 17. Failure Experiment

Intentionally break the architecture.

For example:

```text
Orders → Payments.Infrastructure
```

Then run the architecture tests.

The expected result:

```text
❌ Architecture violation
```

This is important because the lab should demonstrate that architectural rules are **enforceable**, not merely documented.

---

## 18. Evolution Path

A modular monolith creates a stronger foundation for future evolution.

```text
Unstructured Monolith
        ↓
Modular Monolith
        ↓
Selective Extraction
        ↓
Independent Service
```

But:

```text
Modular Monolith
        ↓
Keep as Modular Monolith
```

is equally valid.

The architecture should evolve because there is a real architectural driver, not because microservices are considered the default destination.

---

## 19. .NET Implementation

Useful .NET mechanisms include:

- Projects and assemblies for stronger boundaries
- Internal types
- Explicit public contracts
- Dependency Injection
- EF Core
- Separate `DbContext` per module where appropriate
- Domain events
- MediatR or similar application messaging where justified
- NetArchTest / architecture testing tools
- Roslyn analyzers
- OpenTelemetry
- .NET Aspire for local distributed experimentation when needed

A particularly strong boundary is:

```text
Module
   ↓
Public Contract
   ↓
Internal Implementation
```

rather than exposing the entire module assembly.

---

## 20. Lab Experiment

Build a deliberately coupled monolith:

```text
Orders
Payments
Customers
```

Then evolve it.

### Phase 1

Create the tightly coupled monolith.

### Phase 2

Identify architectural boundaries.

### Phase 3

Create modules.

### Phase 4

Move business logic into modules.

### Phase 5

Define public contracts.

### Phase 6

Establish data ownership.

### Phase 7

Add architecture tests.

### Phase 8

Introduce an intentional violation.

### Phase 9

Verify that the architecture test fails.

### Phase 10

Fix the violation.

### Phase 11

Measure the resulting dependency graph.

---

## 21. Key Takeaways

- A modular monolith is still a monolith operationally.
- Modularity is primarily about **boundaries and ownership**.
- Business capabilities are stronger module boundaries than technical layers.
- One database can still support a modular architecture.
- Logical data ownership matters even when storage is shared.
- Public contracts should hide module internals.
- Architecture tests turn architectural principles into enforceable rules.
- A modular monolith does not automatically need to become microservices.
- The migration should be incremental.
- The objective is **controlled change**, not decomposition for its own sake.

> **Monolith to Modular Monolith is not about splitting the deployment.**
>
> **It is about splitting responsibility while keeping operational simplicity.**
