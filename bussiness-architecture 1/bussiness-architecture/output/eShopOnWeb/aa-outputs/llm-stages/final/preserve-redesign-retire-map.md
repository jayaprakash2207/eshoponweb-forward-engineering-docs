# Preserve / Redesign / Review / Retire Map (Candidate)

> Per stage rules, decisions are conservative. **Nothing is marked "Retire" without usage evidence**, and no such evidence was available in this stage's input context.

| Item | Decision | Rationale | Evidence | Confidence |
|------|----------|-----------|----------|------------|
| ApplicationCore domain model (Basket/Order/Buyer aggregates, Specifications, IAggregateRoot, IRepository/IReadRepository) | **Preserve** | Cohesive Domain+Application separation; consistent dependency direction (Clean/Onion pattern, confidence 0.7) | `src/ApplicationCore/Entities/*`, `src/ApplicationCore/Interfaces/*`, `src/ApplicationCore/Specifications/*` (architecture-pattern-report.md) | 0.65 |
| `CatalogItemOrdered` snapshot in Order aggregate | **Preserve (pending Wave 0 confirmation)** | Violation register's own migration_impact notes this snapshot is "actually beneficial" if Catalog/Order become separate services — avoids runtime cross-service calls for historical order data | `src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs` (ARCH-VIOL-001) | 0.55 |
| Duplicated Catalog Admin UI (`Web/Pages/Admin` vs `BlazorAdmin/Pages/CatalogItemPage`) | **Review** | Overlapping ownership of Admin responsibility; canonical surface unconfirmed | `src/Web/Pages/Admin/EditCatalogItem.cshtml`, `src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor` (ARCH-VIOL-002) | 0.5 |
| PublicApi ↔ BlazorAdmin catalog contract coupling (`CachedCatalogItemServiceDecorator` ↔ `CatalogItemEndpoints`) | **Review** | Contract coupling is functional but undocumented; formalize rather than restructure first | `src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs`, `src/PublicApi/CatalogItemEndpoints/CreateCatalogItemEndpoint.cs` (ARCH-VIOL-003) | 0.5 |
| Two-presentation-project structure (`Web` + `PublicApi`, both depending on `ApplicationCore`/`Infrastructure`) | **Preserve** | Defining trait of the current Modular Monolith; no evidence of need to collapse | architecture-pattern-report.md | 0.6 |
| Any component not covered by the two evidence files provided to this stage | **Review (unknown)** | No claim can be made without evidence | n/a | unknown |

## Explicitly NOT Decided in This Stage

- Nothing is marked **Retire**. The violation register provided contains no usage/runtime evidence (e.g., traffic, call counts, deprecation markers) that would justify retirement of either Admin surface or any other component.
- Database/queue ownership decisions are out of scope here — see `data-ownership-map.md`.