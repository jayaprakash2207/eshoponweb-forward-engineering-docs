# Evidence Verification Report — eShopOnWeb Ground-Truth Check

**Date:** 2026-06-25
**Verified against:** `https://github.com/dotnet-architecture/eShopOnWeb` (branch `main`)
**Method:** Direct fetch of real source files; compared to the knowledge graph + 18 FE documents.
**Status of files:** No source files deleted. Discrepancies are **annotated in place** (non-destructive),
preserving the evidence trail, consistent with the package's anti-hallucination philosophy.

---

## 1. Verified-accurate claims (✅ confirmed against real source)

| # | Claim | Real source | Result |
|---|---|---|---|
| 1 | Entity set (CatalogItem, CatalogBrand, CatalogType, Basket, BasketItem, Order, OrderItem, Buyer, PaymentMethod, BaseEntity + 3 aggregates) | `src/ApplicationCore/Entities/` | ✅ exact |
| 2 | Buyer + PaymentMethod exist, flagged **aspirational** | `BuyerAggregate/Buyer.cs`, `PaymentMethod.cs` | ✅ correct |
| 3 | CatalogItemDetails = nested record (aspirational, not a table) | nested `record struct` in `CatalogItem.cs` | ✅ correct |
| 4 | CatalogItem core props: Name, Description, Price, PictureUri, CatalogTypeId, CatalogBrandId | `CatalogItem.cs` | ✅ exact |
| 5 | Order: BuyerId, OrderDate, ShipToAddress (Address), OrderItems | `OrderAggregate/Order.cs` | ✅ exact |
| 6 | OrderItem: ItemOrdered (CatalogItemOrdered), UnitPrice, Units | `OrderAggregate/OrderItem.cs` | ✅ exact |
| 7 | Address VO: Street, City, State, Country, ZipCode (all string) | `OrderAggregate/Address.cs` | ✅ exact |
| 8 | CatalogItemOrdered VO: CatalogItemId, ProductName, PictureUri | `OrderAggregate/CatalogItemOrdered.cs` | ✅ exact |
| 9 | Basket: BuyerId, Items (BasketItem), TotalItems | `BasketAggregate/Basket.cs` | ✅ exact |
| 10 | BasketItem: CatalogItemId, Quantity, UnitPrice | `BasketAggregate/BasketItem.cs` | ✅ exact |
| 11 | PublicApi endpoint groups: Auth, CatalogBrand, CatalogType, CatalogItem | `src/PublicApi/` | ✅ exact |
| 12 | 5 CatalogItem endpoints (GetById, ListPaged, Create, Delete, Update) | `CatalogItemEndpoints/` | ✅ exact |
| 13 | AuthenticateEndpoint exists | `AuthEndpoints/AuthenticateEndpoint.cs` | ✅ exact |

**13 of 14 verified claims correct.**

---

## 2. CONFIRMED ERROR (❌ DISCREPANCY)

### DISC-001 — CatalogItem stock/reorder fields do NOT exist in real eShopOnWeb

| | |
|---|---|
| **Claimed** | `CatalogItem` has `AvailableStock`, `RestockThreshold`, `MaxStockThreshold`, `OnReorder` |
| **Real source** | `src/ApplicationCore/Entities/CatalogItem.cs` (main) declares ONLY: `Name`, `Description`, `Price`, `PictureUri`, `CatalogTypeId`, `CatalogType`, `CatalogBrandId`, `CatalogBrand`. **None of the four stock fields exist.** |
| **Verdict** | **FALSE — not in legacy evidence.** Likely contaminated from `eShopOnContainers` (the microservices sibling, whose Catalog DOES have stock fields) or an older variant. |
| **Confidence label that should have applied** | These were carried as if HIGH; correct label is **DISCREPANCY / ASSUMED** (no evidence in the named source). |

### Knock-on effects of DISC-001
- **`StockReorderTriggered` (EVT-12)** — inferred *from* these phantom fields → **invalid**; must not be generated. (Already the graph's weakest event, ASMP-FE-002.)
- Any **reorder capability/process** inferred from stock thresholds → **invalid**.
- The DDL `CK_CatalogItem_Stock` check constraint (completion-package doc 03) → **must be removed** when DISC-001 is resolved.

---

## 3. Files affected by DISC-001 (annotated in place)

| Layer | Files |
|---|---|
| **Canonical (source of truth)** | `enterprise-foundation-package/ENTERPRISE_KNOWLEDGE_GRAPH.json` (5×), `ARCHITECTURE_INVENTORY.md` |
| **FE package** | `01_BRD.md`, `04_BUSINESS_PROCESS_MODEL.md`, `05_DOMAIN_MODEL.md`, `06_DATA_DICTIONARY.md`, `07_DATA_MODEL_SPECIFICATION.md`, `08_ERD.md`, `11_API_CONTRACT_SPECIFICATION.md`, `15_FORWARD_ENGINEERING_SPECIFICATION.md`, `16_GENERATION_MANIFEST.json`, `17_FORWARD_ENGINEERING_READINESS_REPORT.md` |
| **Completion package** | `02_PHYSICAL_DATA_MODEL.md`, `03_DATABASE_DDL_SPECIFICATION.md`, `06_GENERATION_POLICY.md`, `07_IMPLEMENTATION_GUIDELINES.md` |

Each occurrence is annotated with a marker referencing **DISC-001** and this report. The knowledge graph
(JSON) carries a `metadata`-level discrepancy note (it is the authoritative artifact; structural deletion
deferred to a deliberate regeneration to avoid breaking dependents).

---

## 4. Accuracy after verification

| Metric | Value |
|---|---|
| Verified-sample accuracy | **13/14 = ~93%** |
| Structural/high-confidence facts (entities, aggregates, VOs, APIs) | **~96%** (only stock fields wrong) |
| Documents with no known error | **after annotation: all 18 carry correct guidance**; DISC-001 is flagged, not silently present |
| True whole-graph accuracy (estimate) | **~90–95%** (unmeasured items: 19 dep versions LOW, NFRs derived, Identity columns inferred — not errors, just unverified) |

---

## 5. Forward-engineering impact

- **Generation is safe for the implemented scope** (Catalog/Basket/Order/Identity) **once DISC-001 is honored**: generate `CatalogItem` with its 6 real properties only; **do NOT** generate stock fields, the `CK_CatalogItem_Stock` constraint, `StockReorderTriggered`, or any reorder workflow.
- Buyer/Payment (BC-06) remain SKIPPED (aspirational — independently confirmed present-but-unwired in the real repo).
- No other entity/API claim requires correction.

---

## 6. Unverified (consistent, not yet ground-truth-checked)

Not errors — simply not spot-checked against source: docs 09 (DFD), 12 (tech versions — already LOW),
13 (security findings), 14 (NFRs — derived by design), and the business narrative of 01/02/03. These
remain internally consistent; verifying them would require deeper source inspection (build files, configs).

*Verification performed read-only against the public repo. Corrections applied as in-place annotations only.*
