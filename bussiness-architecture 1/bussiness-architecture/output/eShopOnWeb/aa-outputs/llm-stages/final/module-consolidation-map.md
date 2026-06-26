# Module Consolidation Map (Candidate)

> Status: **CANDIDATE** — none of these represent committed consolidation decisions. They are inputs for architect review.

## MCC-001: Unify Catalog Administration UI

- **Modules involved:** `Web` (Pages/Admin), `BlazorAdmin` (Pages/CatalogItemPage)
- **Evidence:** `src/Web/Pages/Admin/EditCatalogItem.cshtml`, `src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor`
- **Related violation:** ARCH-VIOL-002 (severity: Medium)
- **Recommendation:** Do not merge until the team confirms which UI is canonical (legacy vs actively maintained). Carrying both forward duplicates effort and risks divergent business rules.
- **Confidence:** 0.5

## MCC-002: Preserve ApplicationCore as a Single Domain/Application Module

- **Modules involved:** `ApplicationCore` (Basket/Order/Buyer aggregates, CatalogItem, Specifications, Services)
- **Evidence:** `src/ApplicationCore/Entities/BasketAggregate`, `src/ApplicationCore/Entities/OrderAggregate`, `src/ApplicationCore/Entities/BuyerAggregate`, `src/ApplicationCore/Entities/CatalogItem.cs`
- **Related violation:** none
- **Recommendation:** Keep as a single cohesive unit for now; it is the likely anchor for any future aggregate-aligned service split (see `service-boundary-options.md`).
- **Confidence:** 0.6

## MCC-003: Review Contract Coupling Between PublicApi and BlazorAdmin Catalog Services

- **Modules involved:** `PublicApi` (CatalogItemEndpoints), `BlazorAdmin` (CachedCatalogItemServiceDecorator)
- **Evidence:** `src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs`, `src/PublicApi/CatalogItemEndpoints/CreateCatalogItemEndpoint.cs`
- **Related violation:** ARCH-VIOL-003 (severity: Medium)
- **Recommendation:** Not a code-merge candidate — a contract-formalization task. See `api-contract-preservation-map.json`.
- **Confidence:** 0.5

## Out of Scope for This Stage

- No consolidation candidates were identified that involve `Infrastructure`, since no violations or pattern-report evidence directly implicated it beyond its role as a dependency target.
- Module-level identifiers (`MOD-xxx`) for `ApplicationCore`, `Web`, `PublicApi`, and `BlazorAdmin` were not present in the input context provided to this stage (only `MOD-003` and `MOD-006` appear, via the violation register). Cross-reference against `architecture-output/final/module-catalog.json` (not included in this run) to attach full module IDs.