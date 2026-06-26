# Service Boundary Options (Candidate)

> **These are options for discussion, not final service boundaries.** Per stage rules, no future technology stack or final boundary is asserted here.

## Context

The current system is a Modular Monolith (confidence 0.7, per `architecture-pattern-report.md`):
- `ApplicationCore` — shared domain/application layer (Basket, Order, Buyer, Catalog aggregates + specifications)
- `Infrastructure` — implementation of repositories etc., depended on by presentation/API projects
- `Web` — Razor Pages/MVC storefront + admin pages
- `PublicApi` — Ardalis.ApiEndpoints minimal API (consumed by BlazorAdmin)
- `BlazorAdmin` — Blazor WASM/Server admin app, calls `PublicApi`

There is **no evidence** of independent data stores per module, message brokers, or service-to-service network contracts between `Web` and `PublicApi` (per `architecture-pattern-report.md`). Any service boundary option below that assumes independent databases or async messaging is **speculative** and flagged accordingly.

---

## Option A — Status Quo / Strangler-Friendly (Modular Monolith Retained)

Keep `ApplicationCore` + `Infrastructure` as a single deployable "Core Commerce" module. `Web`, `PublicApi`, and `BlazorAdmin` remain separate front-ends/deployables as they are today.

- **Pros:** Lowest risk; matches current dependency direction (pattern-report evidence); no data-ownership questions to resolve immediately.
- **Cons:** Does not resolve ARCH-VIOL-002 (duplicated Admin UI) or ARCH-VIOL-003 (contract coupling) — these would need to be addressed within the monolith regardless.
- **Evidence:** `architecture-pattern-report.md` (Modular Monolith determination, confidence 0.7).
- **Confidence:** 0.6 (as a *viable starting point*, not as a final decision).

## Option B — Aggregate-Aligned Service Candidates

Split `ApplicationCore` along its existing aggregate boundaries into candidate services:
- **Catalog Service** — `CatalogItem`, Catalog specifications, `PublicApi/CatalogItemEndpoints`
- **Ordering Service** — `OrderAggregate`, `BasketAggregate`, `BuyerAggregate`, order/basket specifications
- **Admin/BFF** — consolidated admin surface (pending resolution of ARCH-VIOL-002)

- **Pros:** Aligns with existing DDD aggregate boundaries already visible in code (Basket/Buyer/Order as `IAggregateRoot`).
- **Cons:** The `CatalogItemOrdered` snapshot (ARCH-VIOL-001) creates a direct data dependency between Catalog and Ordering — per the violation register's own migration_impact note, this snapshot is "actually beneficial" if these become separate services and should be **preserved as an anti-corruption pattern**, not eliminated.
- **Open question:** No evidence of per-aggregate database ownership was provided — confirm via `data-ownership-map.md` and the (not-yet-seen) full inventory before treating this as feasible.
- **Confidence:** 0.45 (directional only; database/transaction-boundary evidence is missing).

## Option C — Front-End Decoupling Only

Leave the domain/data layer (`ApplicationCore` + `Infrastructure`) as one deployable, but formally separate the front-ends:
- Resolve ARCH-VIOL-002 by choosing one canonical Admin UI (`Web/Pages/Admin` or `BlazorAdmin`).
- Formalize the `PublicApi` ↔ `BlazorAdmin` contract (ARCH-VIOL-003) as a versioned API contract rather than an internal coupling.

- **Pros:** Addresses both Medium-severity violations without requiring a domain-model split or database ownership decisions.
- **Cons:** Does not reduce the size/complexity of the core domain module.
- **Confidence:** 0.55.

---

## Cross-Cutting Open Questions for All Options

1. Which Admin UI (`Web/Pages/Admin` vs `BlazorAdmin`) is canonical? (ARCH-VIOL-002)
2. Is the `CatalogItemOrdered` snapshot intentional (anti-corruption/historical record) or accidental coupling? (ARCH-VIOL-001)
3. What is the database topology (single shared DB vs per-module schemas)? Not addressed in evidence provided to this stage — see `data-ownership-map.md`.
4. Is there an Identity/Authentication module, and does it have its own boundary? Not present in evidence provided to this stage.