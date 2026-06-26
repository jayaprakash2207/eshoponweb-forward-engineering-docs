# Architecture Decision Inputs

> Compiled questions and decision points for architects, derived from this stage's analysis. None of these are pre-decided.

## Decisions Stemming from Confirmed Violations

### AD-001: Canonical Catalog Admin UI (ARCH-VIOL-002, severity Medium)
- **Question:** Is `src/Web/Pages/Admin/EditCatalogItem.cshtml` (Razor Pages) or `src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor` (Blazor) the canonical/actively-maintained Catalog Administration surface?
- **Why it matters:** Drives MCC-001 (module-consolidation-map.json), CAP-005 ownership, and Wave 1 of migration-wave-plan.md.
- **Evidence:** Both files exist and implement overlapping functionality; no usage data available to distinguish.
- **Confidence of question's validity:** 0.5

### AD-002: Intent of `CatalogItemOrdered` Snapshot (ARCH-VIOL-001, severity Low)
- **Question:** Was the `CatalogItemOrdered` value object (embedding Catalog data inside `OrderItem`) designed as a deliberate anti-corruption/historical-record pattern, or is it accidental coupling that arose organically?
- **Why it matters:** If deliberate, it should be documented as a bounded-context boundary and explicitly preserved in any Catalog/Order service split (per the violation register's own migration_impact note). If accidental, it may warrant redesign.
- **Evidence:** `src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs`
- **Confidence of question's validity:** 0.55

### AD-003: PublicApi Contract Formalization (ARCH-VIOL-003, severity Medium)
- **Question:** Should the `CatalogItemEndpoints` consumed by `BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs` be formally versioned/published as a contract, and who owns changes to it?
- **Why it matters:** Determines how much coordination overhead is needed for future Catalog changes, and informs Wave 2 of migration-wave-plan.md.
- **Evidence:** `src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs`, `src/PublicApi/CatalogItemEndpoints/CreateCatalogItemEndpoint.cs`
- **Confidence of question's validity:** 0.5

## Decisions Requiring Additional Evidence (Not Resolvable This Stage)

### AD-004: Service Boundary Option Selection
- **Question:** Which of Option A (status quo), Option B (aggregate-aligned services), or Option C (front-end decoupling only) — or a hybrid — should guide forward engineering?
- **Blocking gaps:** Database/persistence topology (`data-ownership-map.md` marks this `unknown`), test/runtime evidence (`test-runtime-evidence-map.md`).

### AD-005: Existence and Scope of Non-Evidenced Capabilities
- **Question:** Does the system include Identity/Authentication, Payment, Shipping/Notification, or other capabilities not referenced in `architecture-pattern-report.md` or `architecture-violation-register.json`?
- **Why it matters:** Forward engineering backlog and capability map (`business-capability-map.json`) would be incomplete if these exist.
- **Recommended action:** Re-run capability extraction once full `architecture-output/final/system-inventory.json` and component catalog are available.

### AD-006: Database/Schema Ownership Per Aggregate
- **Question:** Is there a single shared database, or per-module/aggregate schema separation?
- **Why it matters:** Directly gates feasibility of service-boundary Option B.
- **Blocking gap:** No `Infrastructure`/persistence evidence was provided to this stage.

## Summary

| ID | Decision | Owner | Blocking |
|----|----------|-------|----------|
| AD-001 | Canonical Admin UI | Architecture team | None — can decide now |
| AD-002 | CatalogItemOrdered intent | Architecture/Domain team | None — can decide now |
| AD-003 | PublicApi contract formalization | Architecture/API team | None — can decide now |
| AD-004 | Service boundary option | Architecture team | AD-006, test/runtime evidence |
| AD-005 | Non-evidenced capability scope | Architecture team | Full inventory pack |
| AD-006 | Database/schema ownership | Architecture/Infra team | Infrastructure evidence pack |