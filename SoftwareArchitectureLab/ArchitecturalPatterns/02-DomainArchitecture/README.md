# Domain Architecture

## 1. Essence

**Domain Architecture** organizes software around the **business domain, its capabilities, rules, invariants, language, and boundaries**, rather than around technical infrastructure.

The central idea is:

> **The domain defines what the system means; the architecture protects that meaning from technical concerns.**

A domain-centric architecture makes business concepts first-class architectural elements.

Typical concepts include:

- Entities
- Value Objects
- Aggregates
- Domain Services
- Domain Events
- Bounded Contexts
- Domain Policies
- Business Rules
- Invariants
- Ubiquitous Language

The goal is not to reproduce business terminology in class names.

The goal is to make the **business model explicit, coherent, and enforceable in software**.

---

## 2. Problem

As systems evolve, business logic often becomes distributed across technical layers:

```text
Controller
    ↓
Application Service
    ↓
Repository
    ↓
Database
```

Business rules may then appear in:

- controllers
- application services
- repositories
- database triggers
- validators
- utility classes
- event handlers
- frontend code

The result is a system where it becomes difficult to answer:

> **Where does the business rule actually live?**

This leads to:

- duplicated business logic
- inconsistent rules
- anemic domain models
- weak invariants
- accidental coupling
- difficult change
- unclear ownership
- infrastructure leaking into the domain

Domain Architecture addresses this by making **business concepts and rules explicit architectural boundaries**.

---

## 3. Intent

Domain Architecture aims to:

1. Make the business domain explicit in the code.
2. Protect business rules from infrastructure concerns.
3. Give business concepts clear ownership.
4. Keep invariants close to the state they protect.
5. Create a shared and precise domain language.
6. Separate business complexity from technical complexity.
7. Establish meaningful domain boundaries.
8. Make domain behavior independently testable.
9. Allow infrastructure to evolve without redefining the business model.
10. Provide a foundation for evolving systems as business complexity increases.

---

## 4. Core Principles

### 4.1 Domain First

The domain should be treated as a primary architectural concern.

Instead of starting with:

```text
Database
    ↓
Repository
    ↓
Service
    ↓
Controller
```

start with:

```text
Business Capability
        ↓
Domain Model
        ↓
Use Cases
        ↓
Infrastructure
```

The technical architecture exists to support the domain.

---

### 4.2 Ubiquitous Language

The domain model should use precise terminology shared by:

- domain experts
- developers
- architects
- product owners
- analysts
- testers

If the business distinguishes:

```text
Pending
Approved
Rejected
Settled
```

the software should not collapse all of these into:

```text
Status = 1
```

unless the distinction is genuinely irrelevant to the domain.

Language is part of the model.

---

### 4.3 Business Rules Are First-Class

Important business rules should exist explicitly in the domain model.

For example:

```text
A transaction cannot be settled twice.

A payment cannot be captured after expiration.

A withdrawal cannot exceed the permitted limit.

An account cannot become active without verification.
```

These should not exist only as comments or implicit assumptions.

---

### 4.4 Invariants Are Protected

An invariant is a condition that must remain true.

For example:

```text
Account balance >= 0
```

or:

```text
Order cannot be shipped before payment is confirmed.
```

The domain model should provide controlled operations that preserve these invariants.

Prefer:

```csharp
order.ConfirmPayment();
```

over:

```csharp
order.Status = OrderStatus.Paid;
```

when changing the state requires business rules.

---

### 4.5 Behavior Over Data

A domain model should not merely represent data.

It should represent **behavior and decisions**.

Prefer:

```csharp
transaction.Approve();
transaction.Reject(reason);
transaction.Settle();
```

over:

```csharp
transaction.Status = Approved;
transaction.Status = Rejected;
transaction.Status = Settled;
```

The object that owns the state should normally own the rules governing changes to that state.

---

### 4.6 Explicit Ownership

Every business concept should have an identifiable owner.

Ask:

> **Who owns this rule?**

and:

> **Who owns the state required to enforce this rule?**

Avoid rules that are scattered across unrelated components.

---

### 4.7 Separation of Domain and Infrastructure

The domain should not depend directly on:

- Entity Framework Core
- HTTP
- databases
- message brokers
- cloud SDKs
- file systems
- UI frameworks
- infrastructure-specific configuration

The dependency direction should protect the domain:

```text
        Infrastructure
              │
              ▼
        Application
              │
              ▼
           Domain
```

Infrastructure depends on the domain.

The domain should not depend on infrastructure.

---

## 5. Architectural Model

A domain-centric architecture can be represented as:

```text
                    Business Domain
                          │
          ┌───────────────┼───────────────┐
          │               │               │
          ▼               ▼               ▼
       Entities      Value Objects     Policies
          │               │               │
          └───────────────┼───────────────┘
                          │
                     Aggregates
                          │
                     Domain Events
                          │
                          ▼
                     Application
                          │
                          ▼
                    Infrastructure
```

The domain is not simply another layer.

It is the **source of business meaning and rules**.

---

## 6. Core Domain Building Blocks

### 6.1 Entity

An entity is a domain object whose identity remains meaningful over time.

```csharp
public sealed class Customer
{
    public CustomerId Id { get; }

    public string Name { get; private set; }

    public void Rename(string name)
    {
        // Business rules
    }
}
```

Identity is more important than object equality.

---

### 6.2 Value Object

A Value Object represents a domain concept defined by its values rather than identity.

Examples:

```text
Money
EmailAddress
Address
Currency
Percentage
DateRange
AccountNumber
```

Example:

```csharp
public readonly record struct Money(
    decimal Amount,
    Currency Currency);
```

A value object should ideally be:

- immutable
- self-validating
- conceptually meaningful
- free from infrastructure concerns

---

### 6.3 Aggregate

An Aggregate defines a consistency boundary around related domain objects.

```text
Order Aggregate
│
├── Order
├── OrderLine
├── ShippingAddress
└── PaymentState
```

The Aggregate Root controls access to the aggregate's invariants.

External code should normally interact through the root rather than manipulating internal entities directly.

---

### 6.4 Domain Service

A Domain Service represents domain behavior that does not naturally belong to a single entity or value object.

Use it carefully.

A Domain Service should represent **domain behavior**, not generic technical functionality.

Good:

```text
RiskEvaluationService
ExchangeRatePolicy
PricingPolicy
```

Not:

```text
DatabaseService
HttpService
LoggingService
JsonService
```

---

### 6.5 Domain Event

A Domain Event represents a meaningful fact that has occurred within the domain.

Examples:

```text
OrderPlaced
PaymentApproved
AccountActivated
TransactionSettled
CustomerVerified
```

An event should represent something that happened:

```text
PaymentApproved
```

rather than an instruction:

```text
ApprovePayment
```

The latter is a command.

---

### 6.6 Domain Policy

A policy represents a business decision or rule that may evolve independently.

Examples:

```text
CreditLimitPolicy
DiscountPolicy
RiskPolicy
ApprovalPolicy
FraudPolicy
```

Policies become particularly useful when business decisions are complex or frequently changing.

---

## 7. Folder Structure

A domain-centric .NET project might look like:

```text
Domain/
│
├── Entities/
├── ValueObjects/
├── Aggregates/
├── DomainServices/
├── Policies/
├── Events/
├── Exceptions/
├── Specifications/
└── Common/
```

A larger bounded context might use:

```text
OrderManagement/
│
├── Domain/
│   ├── Orders/
│   │   ├── Order.cs
│   │   ├── OrderLine.cs
│   │   ├── OrderId.cs
│   │   ├── OrderStatus.cs
│   │   ├── Events/
│   │   └── Policies/
│   │
│   ├── Customers/
│   └── Shared/
│
├── Application/
├── Infrastructure/
└── Contracts/
```

Prefer organizing around meaningful domain concepts rather than creating large technical buckets.

For example:

```text
Domain/
├── Entities/
│   ├── Order.cs
│   ├── Customer.cs
│   └── Payment.cs
```

may be less expressive than:

```text
Domain/
├── Orders/
├── Customers/
└── Payments/
```

when those concepts represent meaningful domain boundaries.

---

## 8. Dependency Rules

The domain should have minimal dependencies.

Prefer:

```text
Infrastructure
      │
      ▼
Application
      │
      ▼
Domain
```

Avoid:

```text
Domain
   │
   ├──→ EntityFrameworkCore
   ├──→ ASP.NET Core
   ├──→ Kafka
   └──→ Azure SDK
```

The domain may depend on abstractions when those abstractions represent genuine domain or application concepts.

However, an abstraction should not be introduced merely to hide a technical dependency.

---

## 9. Domain State and Invariants

State should be controlled through behavior.

Avoid unrestricted mutation:

```csharp
public OrderStatus Status { get; set; }
```

Prefer controlled transitions:

```csharp
public void ConfirmPayment()
{
    if (Status != OrderStatus.PendingPayment)
        throw new InvalidOperationException();

    Status = OrderStatus.Paid;
}
```

The exact implementation can vary, but the architectural principle is:

> **State transitions should preserve domain invariants.**

---

## 10. Aggregate Boundaries

Aggregate boundaries should be designed around **consistency requirements**, not simply object relationships.

For example:

```text
Order
 ├── OrderLine
 ├── ShippingAddress
 └── Payment
```

does not automatically mean that all of these concepts must belong to one aggregate.

Ask:

- Which state must change atomically?
- Which invariants must be enforced together?
- What consistency boundary is actually required?
- Which objects need transactional consistency?
- Which relationships can tolerate eventual consistency?

An Aggregate should not become a large object graph simply because the objects are related.

---

## 11. Main Design Questions

### Domain Discovery

- What business problem does the system solve?
- What are the core business capabilities?
- Which concepts are essential?
- Which concepts are supporting?
- Which terminology is ambiguous?

### Modeling

- What are the important entities?
- Which concepts are value objects?
- Which concepts require identity?
- Which rules are invariants?
- Which behaviors belong to which concepts?

### Ownership

- Who owns this state?
- Who owns this invariant?
- Who decides whether an operation is valid?
- Which aggregate protects the invariant?

### Boundaries

- Where does one domain concept stop and another begin?
- Which concepts belong together?
- Where are consistency boundaries?
- Where are transactional boundaries?

### Events

- What meaningful facts occur in the domain?
- Which events are internal domain events?
- Which facts need to cross a boundary?
- Which consumers need eventual consistency?

### Evolution

- Which rules are likely to change?
- Which policies should be isolated?
- Which concepts are stable?
- Which concepts are still uncertain?

---

## 12. Important Constraints

Domain Architecture does **not** mean:

- every class must be an Entity
- every method must belong to a domain object
- every system needs full DDD
- every operation requires an Aggregate
- every business event must become a message
- every repository needs an interface
- every service is a Domain Service

Avoid modeling complexity that the business does not actually have.

The domain model should be **as rich as necessary and no richer**.

---

## 13. Architectural Invariants

A domain-centric architecture should normally enforce:

1. Domain rules do not depend on infrastructure.
2. Important business invariants have explicit owners.
3. State transitions preserve invariants.
4. Aggregates protect their consistency boundaries.
5. Domain concepts use precise business terminology.
6. Domain events represent meaningful business facts.
7. Technical concerns do not define the domain model.
8. Domain objects do not expose uncontrolled mutation.
9. Business rules are testable without infrastructure.
10. Domain boundaries remain explicit as the system evolves.

---

## 14. Failure Modes

### 14.1 Anemic Domain Model

The domain contains mostly properties:

```csharp
public class Order
{
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
}
```

while business behavior lives elsewhere:

```text
OrderService
OrderValidator
OrderManager
OrderHelper
OrderProcessor
```

The model represents data but not meaningful domain behavior.

---

### 14.2 Overloaded Aggregates

An Aggregate becomes extremely large:

```text
Order
 ├── Customer
 ├── Payment
 ├── Inventory
 ├── Shipping
 ├── Pricing
 ├── Discounts
 ├── Notifications
 └── EverythingElse
```

Large aggregates increase:

- contention
- transaction size
- memory usage
- complexity
- coupling

Aggregate boundaries should follow consistency requirements.

---

### 14.3 Primitive Obsession

Important domain concepts are represented by primitive types:

```csharp
decimal amount;
string currency;
string email;
string accountNumber;
int riskScore;
```

This makes invalid states easier to represent.

Value Objects can make domain semantics and validation explicit.

---

### 14.4 Domain Leakage

Infrastructure concepts enter the domain:

```csharp
public async Task SaveAsync(DbContext context)
```

or:

```csharp
public class Order : EntityFrameworkEntity
```

The domain becomes coupled to implementation technology.

---

### 14.5 Service Explosion

Every piece of behavior becomes a service:

```text
OrderService
OrderValidationService
OrderCalculationService
OrderStateService
OrderHelperService
OrderManagerService
```

This can result in procedural code disguised as object-oriented architecture.

---

### 14.6 False Domain Modeling

Technical concepts are presented as domain concepts:

```text
Repository
Database
HTTP Client
Message Handler
Controller
```

These may be important architectural components, but they are not automatically domain concepts.

---

## 15. Common Misuse

### Using DDD Vocabulary Without DDD Meaning

Creating:

```text
Aggregate
Entity
Repository
DomainService
```

does not create a domain model.

The concepts must represent real business semantics.

---

### Modeling the Database Instead of the Domain

Avoid:

```text
CustomerTable
OrderTable
OrderItemTable
```

as the primary model of the business.

The database schema is a persistence model.

The domain model represents business meaning.

---

### Exposing Mutable State

Avoid making every domain property publicly mutable.

```csharp
public decimal Balance { get; set; }
```

when changing the balance requires business rules.

Prefer behavior that controls state transitions.

---

### Excessive Abstraction

Do not introduce abstractions solely because "architecture requires interfaces."

An abstraction should represent a meaningful boundary or variation point.

---

## 16. Testing Strategy

Domain testing should be fast and mostly independent of infrastructure.

### Entity Tests

Test business behavior and invariants.

### Value Object Tests

Test:

- validation
- equality
- normalization
- domain semantics

### Aggregate Tests

Test consistency boundaries and state transitions.

### Domain Service Tests

Test complex domain decisions.

### Domain Event Tests

Verify that important business facts are emitted correctly.

### Property-Based Testing

For complex domain rules, property-based testing can verify invariants across many generated inputs.

For example:

```text
For every valid transaction:

balance_after =
    balance_before + credit - debit
```

while preserving:

```text
balance_after >= minimum_allowed_balance
```

The exact properties depend on the domain.

---

## 17. Observability

The domain itself should remain free from infrastructure-specific logging concerns where possible.

Instead, application and infrastructure layers can observe domain activity.

Useful telemetry concepts include:

```text
aggregate
command
domain-event
business-operation
outcome
duration
correlation-id
```

For example:

```text
aggregate=Transaction
operation=Approve
event=TransactionApproved
```

Business events can become useful observability signals without coupling the domain to a particular telemetry provider.

---

## 18. Scalability Considerations

Domain Architecture supports scalability primarily by controlling **business complexity and consistency**.

Important questions include:

- Which aggregates can operate independently?
- Which operations require strong consistency?
- Which relationships can use eventual consistency?
- Which domain events can be processed asynchronously?
- Which domain policies change frequently?
- Which bounded contexts can evolve independently?

Avoid introducing distributed consistency merely because the domain model contains multiple concepts.

---

## 19. Security Considerations

Security-related business rules can be part of the domain when they represent genuine business policies.

Examples:

```text
Approval limits
Transaction authorization rules
Customer eligibility
Risk thresholds
Access-related business decisions
```

Distinguish these from technical security mechanisms such as:

```text
JWT validation
TLS
OAuth token handling
Password hashing
Network policies
```

The latter belong to application or infrastructure concerns.

The domain should express **business authorization rules**, not implement transport-level security.

---

## 20. Evolution & Migration

A useful progression for a domain model is:

```text
Business Understanding
        ↓
Domain Concepts
        ↓
Ubiquitous Language
        ↓
Entities / Value Objects
        ↓
Invariants
        ↓
Aggregates
        ↓
Domain Events
        ↓
Bounded Contexts
        ↓
Evolving Domain Model
```

The model should evolve as understanding of the business improves.

Do not treat the first domain model as permanently correct.

Domain modeling is an iterative engineering activity.

---

## 21. .NET Implementation Notes

Modern C# provides useful language features for expressive domain models.

### Records and Value Objects

`record` and `record struct` can be useful for immutable value semantics.

```csharp
public readonly record struct Money(
    decimal Amount,
    Currency Currency);
```

### Encapsulation

Use:

```csharp
private set;
private;
internal;
```

to control state and implementation exposure.

### Immutable Collections

Use immutable collections when they help preserve domain invariants and prevent accidental mutation.

### Strongly Typed IDs

Instead of:

```csharp
int customerId;
Guid orderId;
string accountId;
```

consider strongly typed identifiers:

```csharp
public readonly record struct CustomerId(Guid Value);
public readonly record struct OrderId(Guid Value);
```

This prevents accidental mixing of semantically different identifiers.

### Pattern Matching

Modern C# pattern matching can make domain rules expressive:

```csharp
return transaction switch
{
    { Status: TransactionStatus.Pending } => Approve(),
    { Status: TransactionStatus.Approved } => RejectAlreadyApproved(),
    _ => RejectInvalidState()
};
```

Use it when it improves domain clarity rather than simply because the language supports it.

### Domain Events

Keep event definitions domain-oriented:

```csharp
public sealed record TransactionApproved(
    TransactionId TransactionId,
    DateTimeOffset ApprovedAt);
```

Avoid coupling domain events directly to:

- Kafka
- RabbitMQ
- Azure Service Bus
- EF Core
- HTTP

The infrastructure decides how and where events are delivered.

---

## 22. Production Checklist

Before considering a domain architecture healthy, verify:

```text
[ ] Core business concepts are explicit
[ ] Ubiquitous language is defined
[ ] Important invariants have clear owners
[ ] Entities contain meaningful behavior
[ ] Value Objects represent important value semantics
[ ] Aggregate boundaries are intentional
[ ] Aggregates protect consistency boundaries
[ ] Domain events represent meaningful business facts
[ ] Domain Services contain genuine domain behavior
[ ] Business policies are explicit where necessary
[ ] Domain code is independent of infrastructure
[ ] Mutable state is controlled
[ ] Strongly typed concepts are used where valuable
[ ] Domain rules are independently testable
[ ] Architecture tests protect domain dependencies
[ ] Domain terminology remains consistent
[ ] The model evolves with business understanding
```

---

## 23. Key Takeaways

1. **The domain defines business meaning; architecture protects it.**

2. **Business rules should be explicit and have clear ownership.**

3. **Invariants belong close to the state they protect.**

4. **Entities should model behavior, not merely data.**

5. **Value Objects make important domain concepts explicit.**

6. **Aggregates define consistency boundaries, not simply object hierarchies.**

7. **Domain Events represent meaningful facts that have occurred.**

8. **Domain Services should contain genuine domain behavior, not technical operations.**

9. **Infrastructure should support the domain without defining it.**

10. **A good domain model is not the most sophisticated model. It is the smallest model that expresses the important business complexity clearly and safely.**
