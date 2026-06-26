# Architecture Pattern Report

## Detected Pattern

**Primary: Clean Architecture / Onion Architecture, organized as a Modular Monolith with N-tier separation.**

Confidence: **0.7**

## Evidence

- `src/ApplicationCore/` contains domain entities (`Entities/BasketAggregate`, `Entities/BuyerAggregate`, `Entities/OrderAggregate`, `Entities/CatalogItem.cs`, etc.), interfaces (`Interfaces/IRepository.cs`, `IReadRepository.cs`, `IAggregateRoot.cs`), application services (`Services/BasketService.cs`, `Services/OrderService.cs`), and specifications (`Specifications/*`) — a textbook Domain + Application layer separated from infrastructure and presentation, consistent with Clean/Onion Architecture.
- `src/Infrastructure/` is referenced as a project dependency from `src/PublicApi/PublicApi.csproj` (system-inventory-pack), consistent with the "Infrastructure depends inward on ApplicationCore, outer layers depend on Infrastructure for composition" pattern.
- Two separate presentation/API projects (`src/Web` — Razor Pages/MVC, `src/PublicApi` — Ardalis.ApiEndpoints minimal API) both sit at the outer ring and (per evidence and convention) depend on `ApplicationCore` and `Infrastructure`, rather than on each other — consistent with a **Modular Monolith** where multiple front-ends share one core domain library.
- A third UI, `src/BlazorAdmin` (Blazor WASM/Server admin), consumes `src/PublicApi` endpoints (`src/PublicApi/CatalogItemEndpoints/*`) rather than `ApplicationCore` directly, indicating an N-tier client/API split for the admin surface.
- Aggregate roots (`Basket`, `Order`, `Buyer`) implementing `IAggregateRoot` and using `Specifications` for queries (`BasketWithItemsSpecification`, `CustomerOrdersWithItemsSpecification`, `OrderWithItemsByIdSpec`) reflect DDD-influenced patterns (Specification pattern, Aggregate Root, Repository).

## Why This Pattern Was Selected

The folder/namespace structure cleanly separates Domain+Application concerns (`ApplicationCore`) from Infrastructure and from multiple presentation/API projects, with dependency direction (PublicApi → ApplicationCore, PublicApi → Infrastructure) flowing inward as Clean Architecture prescribes. The presence of multiple deployable front-ends (`Web`, `PublicApi`, `BlazorAdmin`) sharing a single `ApplicationCore`/`Infrastructure` core is the defining trait of a **Modular Monolith** rather than independently deployable microservices — there is no evidence of independent data stores per module, message brokers, or service-to-service network contracts between `Web` and `PublicApi`.

## Competing Possible Patterns

1. **Layered Monolith / N-tier Architecture** (confidence 0.5) — If `ApplicationCore` services (`BasketService`, `OrderService`) are thin pass-throughs over repositories with most logic in controllers/pages, this would look more like a traditional layered (UI → Service → Data) architecture than true Clean Architecture. Evidence pack truncation prevents confirming whether business rules live in the entities (rich domain model) or in the services/controllers (anemic domain model + layered).
2. **Anemic Domain Model** (confidence 0.45) — Entities such as `Order`, `Basket`, `CatalogItem` appear in the evidence primarily as data holders (`BaseEntity`-derived classes); whether they contain behavior/invariants (rich domain model) or are pure DTtype POCOs with logic in `OrderService`/`BasketService` could not be confirmed from the truncated evidence.
3. **Big Ball of Mud (localized)** (confidence 0.3) — The duplicated admin catalog-editing surfaces (`src/Web/Pages/Admin/EditCatalogItem.cshtml` vs `src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor`) and the malformed filename `CachedCatalogLookupDataServiceDecorator .cs` (trailing space) are localized signs of inconsistent maintenance discipline, though not sufficient to characterize the whole system this way.

## Architecture Violations (Summary — see `architecture-violation-register.json` for details)

- Possible duplicated admin UI ownership for Catalog item editing (Web Razor Pages vs. BlazorAdmin).
- Order aggregate (`CatalogItemOrdered`) embeds a denormalized copy of Catalog module data — a deliberate but cross-module data leakage pattern common in DDD bounded-context snapshots; flagged for review rather than as a defect.
- File-naming hygiene issue (`CachedCatalogLookupDataServiceDecorator .cs`).
- Unclear ownership of `Buyer`/`PaymentMethod` — no service or interface evidence found referencing them directly.

## Forward Engineering Implications

- The Clean Architecture boundary between `ApplicationCore` and the three presentation projects (`Web`, `PublicApi`, `BlazorAdmin`) is the most promising seam for **extracting modules as independent services** (see `strangler-candidate-report.md`), provided per-module repository/data ownership can be untangled from the shared `ApplicationCore`/`Infrastructure` libraries.
- Before any extraction, the **rich vs. anemic domain model question** must be resolved — if business logic lives in `OrderService`/`BasketService` rather than in `Order`/`Basket` entities, a service extraction can move logic wholesale; if it's split across both, extraction risk is higher.
- The dual admin UI (Web Pages/Admin vs BlazorAdmin) should be resolved (pick one) before forward engineering an Admin module/service, to avoid carrying forward duplicated functionality.