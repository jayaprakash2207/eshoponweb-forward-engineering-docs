# Data Ownership Map (Candidate)

> This map reflects entity/aggregate-level ownership as visible in the domain model evidence provided. **Physical database/schema ownership is `unknown`** — no infrastructure/persistence evidence (connection strings, DbContext mappings, schema files) was included in this stage's input context.

## Candidate Data Ownership by Aggregate

| Data / Entity | Candidate Owning Module | Evidence | Notes | Confidence |
|---------------|--------------------------|----------|-------|------------|
| `CatalogItem` (and implied Brand/Type entities) | Catalog (CAP-001) | `src/ApplicationCore/Entities/CatalogItem.cs` | Brand/Type entity files not directly evidenced in this stage's input; assumed to exist alongside CatalogItem per typical Catalog aggregate composition — confirm against inventory. | 0.55 |
| `Basket`, basket items (BasketAggregate) | Shopping Basket (CAP-002) | `src/ApplicationCore/Entities/BasketAggregate`, `BasketWithItemsSpecification` | — | 0.6 |
| `Order`, `OrderItem` (OrderAggregate) | Order Management (CAP-003) | `src/ApplicationCore/Entities/OrderAggregate`, `CustomerOrdersWithItemsSpecification`, `OrderWithItemsByIdSpec` | — | 0.6 |
| `CatalogItemOrdered` (value object, embedded in OrderItem) | Order Management (CAP-003), **contains a denormalized snapshot of Catalog data** | `src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs` (ARCH-VIOL-001) | This is a cross-module data dependency: Order owns the snapshot record, but its *origin* is Catalog data at order time. Document as a deliberate boundary if confirmed (see Wave 0 in migration-wave-plan.md). | 0.55 |
| `Buyer`, `Address` (BuyerAggregate) | Buyer / Customer Profile (CAP-004) | `src/ApplicationCore/Entities/BuyerAggregate` | Address sub-entity inferred from typical BuyerAggregate composition, not directly evidenced — confirm against inventory. | 0.5 |

## Open Questions / Gaps

- **Physical database/schema ownership**: `unknown`. No `Infrastructure`/EF Core `DbContext` or migration evidence was provided in this stage's input context. A single shared database vs. per-aggregate schemas cannot be determined here.
- **Cross-module data dependency** (`CatalogItemOrdered` snapshot): flagged for explicit review in `preserve-redesign-retire-map.md` and `migration-wave-plan.md` Wave 0/3.
- **Brand/Type and Address sub-entities**: presence inferred from standard aggregate composition referenced in architecture-pattern-report.md, but no direct file evidence — verify against `architecture-output/final/component-catalog.json` (not provided to this stage).
- **Read-models / caching**: `BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs` implies a caching layer over Catalog data in the Admin app — ownership of this cache (and its invalidation) is `unknown`.