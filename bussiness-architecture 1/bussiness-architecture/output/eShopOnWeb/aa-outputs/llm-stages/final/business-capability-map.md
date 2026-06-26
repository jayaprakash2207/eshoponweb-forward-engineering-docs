# Business Capability Map (Candidate)

> Status: **CANDIDATE** — all capabilities below require architect review before being treated as authoritative. Derived from `final/architecture-pattern-report.md` and `final/architecture-violation-register.json` only.

## Candidate Capabilities

| ID | Capability | Type | Supporting Module(s) | Confidence |
|----|------------|------|------------------------|------------|
| CAP-001 | Catalog Management | Business | ApplicationCore (CatalogItem, Specifications), PublicApi (CatalogItemEndpoints) | 0.6 |
| CAP-002 | Shopping Basket Management | Business | ApplicationCore (BasketAggregate, BasketService) | 0.6 |
| CAP-003 | Order Management | Business | ApplicationCore (OrderAggregate, OrderService, CatalogItemOrdered) | 0.6 |
| CAP-004 | Buyer / Customer Profile Management | Business | ApplicationCore (BuyerAggregate) | 0.55 |
| CAP-005 | Catalog Administration | Application | Web (Pages/Admin), BlazorAdmin (CatalogItemPage) | 0.5 |
| CAP-006 | Storefront Web Experience | Application | Web | 0.5 |
| CAP-007 | Public API / Integration Layer | Application | PublicApi | 0.55 |
| CAP-008 | Admin Application Experience | Application | BlazorAdmin | 0.55 |

## Notes

- **CAP-005 (Catalog Administration)** is duplicated across two front-ends (`src/Web/Pages/Admin/EditCatalogItem.cshtml` and `src/BlazorAdmin/Pages/CatalogItemPage/Edit.razor`) — see `ARCH-VIOL-002`. This duplication must be resolved (canonical surface chosen) before this capability can be assigned to a single forward-engineering owner.
- **CAP-001/CAP-003** are linked by a deliberate (or possibly accidental) data snapshot: `CatalogItemOrdered` embeds Catalog data inside the Order aggregate (`ARCH-VIOL-001`). This relationship should be preserved as a documented bounded-context boundary, not silently removed, if Catalog and Order are later split.
- **CAP-007/CAP-008** are tightly coupled via `CachedCatalogItemServiceDecorator` → `CatalogItemEndpoints` (`ARCH-VIOL-003`). Any capability boundary drawn around "Catalog" must account for this dependency.
- No evidence was provided in this stage's input context for Identity/Authentication, Payment, or Shipping capabilities. These may exist in the legacy system but are marked `unknown` here pending review of the full inventory pack — do **not** assume they are absent.

## Open Questions

- Confirm whether Buyer/Customer capability has dedicated endpoints/services (no direct evidence in this run).
- Confirm presence/absence of Identity, Payment, and Notification capabilities against the full system inventory (`architecture-output/final/system-inventory.json`), which was not included in this stage's input context.