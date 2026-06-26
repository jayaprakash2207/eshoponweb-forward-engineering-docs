I'd like you to review and enrich a set of data-architecture documents that
were produced in an earlier analysis pass. All 13 files from that pass are
included below as context sections. Please rely on your own knowledge for
any SQL verification queries.

The 13 files live on disk at C:/Users/BrianRoyS/Downloads/bussiness-architecture 1/bussiness-architecture/output/eShopOnWeb/da-outputs/. For any file you make changes to
(including change records), please update it directly on disk at
C:/Users/BrianRoyS/Downloads/bussiness-architecture 1/bussiness-architecture/output/eShopOnWeb/da-outputs/<filename> using your file tools, as soon as it's ready — don't
print updated file contents in your reply. If a file needs no changes, leave
it untouched.

Then write a `review-summary.md` summarizing the full review to
C:/Users/BrianRoyS/Downloads/bussiness-architecture 1/bussiness-architecture/output/eShopOnWeb/da-outputs/review-summary.md using your file tools.

Once done, reply with a short checklist of which files you updated, plus
✅ review-summary.md written (or ❌ with a reason).

--- FILES FROM THE EARLIER ANALYSIS PASS ---

### schema-catalogue.json
```
{
  "db_connection": "CODE-ONLY — db_connection_results was empty in the supplied extraction and no live DB CLI session was available in this analysis context. Connection strings for CatalogConnection and IdentityConnection were present in appsettings.json / appsettings.Docker.json but redacted in the extract. No psql/sqlcmd run was attempted because no extraction artifact recorded one.",
  "databases": [
    {
      "logical_name": "CatalogDb",
      "context_class": "CatalogContext",
      "connection_string_key": "ConnectionStrings.CatalogConnection",
      "engine": "SQL Server (azure-sql-edge in docker-compose)",
      "confidence": 0.8,
      "tables": [
        {
          "table_name": "CatalogBrands",
          "entity": "CatalogBrand",
          "source_file": "src/ApplicationCore/Entities/CatalogBrand.cs",
          "columns": [
            { "name": "Id", "type": "int", "pk": true, "confidence": 0.8 },
            { "name": "Brand", "type": "string", "nullable": false, "confidence": 0.8 }
          ],
          "confidence": 0.8
        },
        {
          "table_name": "CatalogTypes",
          "entity": "CatalogType",
          "source_file": "src/ApplicationCore/Entities/CatalogType.cs",
          "columns": [
            { "name": "Id", "type": "int", "pk": true, "confidence": 0.8 },
            { "name": "Type", "type": "string", "nullable": false, "confidence": 0.8 }
          ],
          "confidence": 0.8
        },
        {
          "table_name": "Catalog",
          "entity": "CatalogItem",
          "source_file": "src/ApplicationCore/Entities/CatalogItem.cs",
          "columns": [
            { "name": "Id", "type": "int", "pk": true, "confidence": 0.8 },
            { "name": "Name", "type": "string", "nullable": false, "confidence": 0.8 },
            { "name": "Description", "type": "string", "nullable": true, "confidence": 0.75 },
            { "name": "Price", "type": "decimal", "nullable": false, "confidence": 0.8 },
            { "name": "PictureUri", "type": "string", "nullable": true, "confidence": 0.75 },
            { "name": "CatalogTypeId", "type": "int", "fk": "CatalogTypes.Id", "confidence": 0.8 },
            { "name": "CatalogBrandId", "type": "int", "fk": "CatalogBrands.Id", "confidence": 0.8 },
            { "name": "AvailableStock", "type": "int", "nullable": false, "confidence": 0.75 },
            { "name": "RestockThreshold", "type": "int", "nullable": false, "confidence": 0.75 },
            { "name": "MaxStockThreshold", "type": "int", "nullable": false, "confidence": 0.75 },
            { "name": "OnReorder", "type": "bool", "nullable": false, "confidence": 0.75 }
          ],
          "confidence": 0.8,
          "notes": "CatalogItemDetails struct exists alongside CatalogItem (src/ApplicationCore/Entities/CatalogItem.cs) — likely a DTO/value object, not its own table. INFERRED, confidence 0.7."
        },
        {
          "table_name": "Baskets",
          "entity": "Basket",
         
... [truncated]
```

### erd.md
```
# Entity Relationship Diagram — eShopOnWeb

`db_connection: CODE-ONLY — no live DB session in extraction (db_connection_results: [])`

## CatalogDb

```
CatalogBrand (CatalogBrands)
  Id (PK)
  Brand
        ▲
        │ 1
        │
        │ *
CatalogItem (Catalog) ───────────────* CatalogType (CatalogTypes)
  Id (PK)                                Id (PK)
  Name                                   Type
  Description
  Price
  PictureUri
  CatalogTypeId (FK -> CatalogTypes.Id)
  CatalogBrandId (FK -> CatalogBrands.Id)
  AvailableStock
  RestockThreshold
  MaxStockThreshold
  OnReorder

Basket (Baskets)
  Id (PK)
  BuyerId  ~~~> AspNetUsers.Id   [SOFT REFERENCE — see WARNINGS]
        │ 1
        │
        │ *
BasketItem (BasketItems)
  Id (PK)
  BasketId (FK -> Baskets.Id)
  CatalogItemId (FK -> Catalog.Id)
  UnitPrice
  Quantity

Order (Orders)
  Id (PK)
  BuyerId  ~~~> AspNetUsers.Id   [SOFT REFERENCE — see WARNINGS]
  OrderDate
  ShipToAddress_* (owned type Address, flattened)
        │ 1
        │
        │ *
OrderItem (OrderItems)
  Id (PK)
  OrderId (FK -> Orders.Id)
  ItemOrdered_* (owned type CatalogItemOrdered — denormalized snapshot of CatalogItem)
  UnitPrice
  Units
```

## IdentityDb (INFERRED — standard ASP.NET Core Identity, confidence 0.7)

```
AspNetUsers
  Id (PK, string/GUID)
  UserName
  Email
  PasswordHash
  ... standard Identity columns
        │ *           │ *
        │             │
AspNetUserRoles ──── AspNetRoles
  UserId (FK)           Id (PK)
  RoleId (FK)           Name (e.g. "Administrators")
```

## ⚠️ WARNINGS — Soft / Unenforced References

1. **`Baskets.BuyerId` → `AspNetUsers.Id`** — cross-database reference (CatalogDb → IdentityDb). No DB-level FK possible since these are two separate databases/contexts. Enforced only in application code (BasketService / OrderService). confidence 0.8.
2. **`Orders.BuyerId` → `AspNetUsers.Id`** — same cross-database soft reference as above. confidence 0.8.
3. **`OrderItems.ItemOrdered_*` (CatalogItemOrdered)** — denormalized/duplicated snapshot of `Catalog` columns (Id, ProductName, PictureUri) at time of order. Not a live FK to `Catalog.Id` — by design, to preserve order history if catalog item changes/is removed later. This is an intentional historical-snapshot pattern, not a data-quality defect. confidence 0.8 — see redundancy-analysis.json.
4. **`Buyer` / `PaymentMethod` entities** (src/ApplicationCore/Entities/BuyerAggregate/) appear in the source-code entity list but were **not** present in the supplied `layer1_db_artifacts.ef_entities` array (which lists only CatalogContext's 7 DbSet entities). UNKNOWN whether these are mapped via `CatalogContext` with a missing DbSet entry in the extract, mapped elsewhere, or are currently dead/unused code. confidence 0.7 — flagged for Agent 2 / redundancy-analysis.json as a possible shadow entity.

```

### data-source-inventory.json
```
{
  "db_connection": "CODE-ONLY — db_connection_results empty in extraction; no live CLI session recorded.",
  "data_sources": [
    {
      "name": "CatalogDb",
      "type": "relational_database",
      "engine": "SQL Server / Azure SQL Edge",
      "connection_string_key": "ConnectionStrings.CatalogConnection",
      "found_in": [
        "src/PublicApi/appsettings.json",
        "src/PublicApi/appsettings.Docker.json",
        "src/Web/appsettings.Docker.json"
      ],
      "context_class": "CatalogContext (src/Infrastructure/Data/CatalogContext.cs)",
      "tables_or_entities": ["CatalogBrand", "CatalogType", "CatalogItem", "Basket", "BasketItem", "Order", "OrderItem"],
      "confidence": 0.8
    },
    {
      "name": "IdentityDb",
      "type": "relational_database",
      "engine": "SQL Server / Azure SQL Edge",
      "connection_string_key": "ConnectionStrings.IdentityConnection",
      "found_in": [
        "src/PublicApi/appsettings.json",
        "src/PublicApi/appsettings.Docker.json",
        "src/Web/appsettings.Docker.json"
      ],
      "context_class": "AppIdentityDbContext (INFERRED, not in supplied ef_entities)",
      "tables_or_entities": ["AspNetUsers", "AspNetRoles", "AspNetUserRoles", "AspNetUserClaims", "AspNetUserLogins", "AspNetUserTokens", "AspNetRoleClaims"],
      "confidence": 0.7
    },
    {
      "name": "In-memory / browser-side cache (BlazorAdmin)",
      "type": "application_cache",
      "engine": "UNKNOWN — likely IMemoryCache or simple in-process dictionary",
      "evidence": [
        "src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs",
        "src/BlazorAdmin/Services/CachedCatalogLookupDataServiceDecorator .cs",
        "src/BlazorAdmin/Services/CacheEntry.cs"
      ],
      "notes": "Decorator pattern wraps CatalogItemService / CatalogLookupDataService with a caching layer (CacheEntry.cs). TTL/eviction policy not visible in supplied extraction — UNKNOWN, confidence < 0.70. See storage-pattern-analysis.md.",
      "confidence": 0.75
    },
    {
      "name": "Browser cookies (BlazorAdmin auth)",
      "type": "client_storage",
      "engine": "Browser cookie store via JS interop",
      "evidence": ["src/BlazorAdmin/JavaScript/Cookies.cs", "src/BlazorAdmin/CustomAuthStateProvider.cs"],
      "notes": "Used for auth token persistence on the Blazor admin client. confidence 0.75",
      "confidence": 0.75
    },
    {
      "name": "Redis (infra abbreviations only — not confirmed in use)",
      "type": "cache",
      "engine": "Redis",
      "evidence": ["infra/abbreviations.json: cacheRedis = 'redis-'"],
      "notes": "Only an Azure naming-convention abbreviation entry found; no Redis connection string, client registration, or usage found in the supplied entity/config extraction. UNKNOWN whether actually used — likely scaffolding left over from the azd template. confidence < 0.70 — marked UNKNOWN.",
      "confidence": 0.5
    }
  ],
  "external_integrations": [
    {
      "name": "Az
... [truncated]
```

### data-flow-map.md
```
# Data Flow Map — eShopOnWeb

## Major Flows

### 1. Catalog Browsing (Read)
`Web (MVC/Razor) / PublicApi` → `IRepository<CatalogItem>` (CatalogContext) → `CatalogDb.Catalog` (joined with `CatalogBrands`, `CatalogTypes`)
- Read-heavy. CachedCatalogItemServiceDecorator and CachedCatalogLookupDataServiceDecorator (BlazorAdmin) sit in front of catalog lookups to reduce DB round-trips. confidence 0.75

### 2. Basket Management
`Web/PublicApi` → `IBasketService` (BasketService.cs) → `IRepository<Basket>` → `CatalogDb.Baskets` / `BasketItems`
- `BasketService` enforces guard clauses (`BasketGuards` in GuardExtensions.cs) — e.g. throwing `BasketNotFoundException` if basket id invalid. confidence 0.8
- Basket is keyed by `BuyerId`, which is a soft reference to `AspNetUsers.Id` in IdentityDb (cross-DB, app-enforced only). confidence 0.8

### 3. Checkout / Order Creation
`Web/PublicApi` (checkout endpoint) → `IOrderService` (OrderService.cs) →
1. Reads `Basket` + `BasketItems` for the buyer
2. Validates basket is non-empty — throws `EmptyBasketOnCheckoutException` if empty (src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs)
3. Snapshots each `CatalogItem` into a `CatalogItemOrdered` owned-type record (denormalization for historical accuracy)
4. Writes `Order` + `OrderItems` to `CatalogDb`
5. (Inferred) clears/empties the `Basket` after order creation — not directly observable in supplied extraction, confidence 0.7

### 4. Authentication / Identity
`Web/PublicApi` → ASP.NET Core Identity middleware → `IdentityDb.AspNetUsers` / `AspNetRoles` / `AspNetUserRoles`
- `ITokenClaimsService` (src/ApplicationCore/Interfaces/ITokenClaimsService.cs) issues claims/tokens for API auth (likely JWT for PublicApi — confidence 0.7, UNKNOWN exact mechanism without reading the implementation).
- `AuthorizationConstants` (src/ApplicationCore/Constants/AuthorizationConstants.cs) and `Roles`/`Constants` (src/BlazorShared/Authorization/Constants.cs) define role names referenced by `[Authorize]` attributes — see access-control-matrix.md.

### 5. BlazorAdmin Management UI
`BlazorAdmin` (WASM client) → `HttpService` → `PublicApi` (HTTP/JSON) → `CatalogContext` → `CatalogDb`
- `CatalogItemService` / `CatalogLookupDataService` call PublicApi endpoints to list/update catalog items, brands, types.
- Decorated by cache layer (see storage-pattern-analysis.md) before hitting the network/API.
- `RefreshBroadcast` (src/BlazorAdmin/Helpers/RefreshBroadcast.cs) appears to be a pub/sub mechanism to invalidate cached data across components after a write — confidence 0.7, INFERRED from naming.

### 6. Notifications
- `IEmailSender` interface (src/ApplicationCore/Interfaces/IEmailSender.cs) — used for order confirmation or account emails. Implementation/trigger point not visible in supplied extraction. UNKNOWN, confidence < 0.7.

## Cross-Database Boundary

```
┌────────────────┐        soft FK (BuyerId)        ┌──────────────────┐
│   CatalogDb     │  <─────────────────────────────
... [truncated]
```

### pii-inventory.json
```
{
  "db_connection": "CODE-ONLY — db_connection_results empty in extraction. All classifications below are INFERRED from entity/field naming conventions typical of eShopOnWeb-style apps; not confirmed against live data.",
  "pii_fields": [
    {
      "table": "AspNetUsers",
      "field": "Email",
      "classification": "PII - direct identifier",
      "sensitivity": "HIGH",
      "status": "INFERRED",
      "confidence": 0.7,
      "notes": "Standard ASP.NET Identity column. Used as login/username in eShopOnWeb."
    },
    {
      "table": "AspNetUsers",
      "field": "UserName",
      "classification": "PII - direct identifier",
      "sensitivity": "HIGH",
      "status": "INFERRED",
      "confidence": 0.7,
      "notes": "Typically equals Email in this app's Identity setup."
    },
    {
      "table": "AspNetUsers",
      "field": "PasswordHash",
      "classification": "Sensitive credential (not PII per se, but high-sensitivity secret)",
      "sensitivity": "CRITICAL",
      "status": "INFERRED",
      "confidence": 0.7
    },
    {
      "table": "AspNetUsers",
      "field": "PhoneNumber",
      "classification": "PII - direct identifier",
      "sensitivity": "MEDIUM",
      "status": "INFERRED",
      "confidence": 0.65,
      "notes": "Standard Identity column, may be unused/null in this app. Confidence below 0.70 threshold — UNKNOWN whether populated."
    },
    {
      "table": "Orders",
      "field": "BuyerId",
      "classification": "PII - pseudo-identifier (links to AspNetUsers.Id)",
      "sensitivity": "MEDIUM",
      "status": "CONFIRMED via entity (Order.cs)",
      "confidence": 0.8
    },
    {
      "table": "Orders",
      "field": "ShipToAddress_Street / _City / _State / _Country / _ZipCode",
      "classification": "PII - physical address (shipping address)",
      "sensitivity": "HIGH",
      "status": "CONFIRMED via entity (Address.cs, owned type)",
      "confidence": 0.8
    },
    {
      "table": "Baskets",
      "field": "BuyerId",
      "classification": "PII - pseudo-identifier",
      "sensitivity": "MEDIUM",
      "status": "CONFIRMED via entity (Basket.cs)",
      "confidence": 0.8
    },
    {
      "table": "PaymentMethod (Buyer/PaymentMethod entities — table not confirmed)",
      "field": "ALL — likely cardholder name, masked card number / payment token, expiry",
      "classification": "PII / Payment data — potential PCI-DSS scope",
      "sensitivity": "CRITICAL",
      "status": "INFERRED — entity exists in source (src/ApplicationCore/Entities/BuyerAggregate/PaymentMethod.cs) but was NOT present in the supplied layer1_db_artifacts.ef_entities list, so its mapped table/columns could not be confirmed.",
      "confidence": 0.65,
      "action_required": "Agent 2 should confirm whether PaymentMethod is actually persisted, and if so, audit for PCI-DSS scope (raw PAN storage would be a major finding)."
    },
    {
      "table": "OrderItems",
      "field": "ItemOrdered_* (CatalogItemOrdered snaps
... [truncated]
```

### data-quality-report.md
```
# Data Quality Report — eShopOnWeb

`db_connection: CODE-ONLY — db_connection_results empty in extraction; no row counts or live constraint checks available. All findings below are derived from EF entity/code structure only.`

## 1. Volume
```
"volume": "UNKNOWN",
"volume_detail": "DB not connected — db_connection_results was an empty array in the supplied extraction. No psql/sqlcmd command was recorded as attempted."
```
All tables (Catalog, CatalogBrands, CatalogTypes, Baskets, BasketItems, Orders, OrderItems, AspNetUsers, etc.): row counts UNKNOWN.

## 2. Referential Integrity Risks (code-derived)

| Issue | Severity | Confidence | Detail |
|---|---|---|---|
| `Baskets.BuyerId` / `Orders.BuyerId` are unenforced cross-database soft references to `AspNetUsers.Id` | MEDIUM | 0.8 | No FK constraint possible since CatalogDb and IdentityDb are separate databases/contexts. Orphaned baskets/orders possible if a user is deleted from IdentityDb without cleanup in CatalogDb. |
| FK delete behavior for `CatalogItem` ← `BasketItem`/`OrderItem` not visible | UNKNOWN | <0.7 | If `ON DELETE NO ACTION` (typical EF Core default for non-owned FK to avoid multiple cascade paths), deleting a `CatalogItem` referenced by an existing `BasketItem` would fail with an FK violation error. Needs migration-file or live-DB confirmation. |
| `OrderItem.ItemOrdered_CatalogItemId` is a denormalized snapshot, not an enforced FK | LOW (by design) | 0.8 | This is intentional (preserves order history even if catalog item is later deleted) — not a defect, but should be documented so engineers don't "fix" it by adding an FK. |

## 3. Nullability / Validation Gaps
- `CatalogItem.Description`, `PictureUri` — INFERRED nullable (confidence 0.75). Not confirmed.
- `Address` fields on `Orders` — all INFERRED nullable string columns via EF owned-type convention; actual `[Required]` annotations not visible in supplied extraction (confidence 0.75).
- 7 validation-category business artifacts were reported in extraction summary (`"validation": 7`) but their specific rules were not included in the entity excerpts — UNKNOWN which fields they apply to. Flagged for Phase 1 deep-read by Agent 2.

## 4. Soft-Delete / Audit Columns
- `BaseEntity` (src/ApplicationCore/Entities/BaseEntity.cs) provides only `Id` based on its one-line content summary — no `IsDeleted`, `CreatedAt`, `ModifiedAt`, or audit columns were observable in the supplied extraction.
- **Finding**: No soft-delete pattern detected anywhere in the supplied entity list. If retention/audit requirements exist, this is a gap. confidence 0.75 (absence-based, could be hidden in a partial class or interceptor not in extraction).

## 5. Shadow / Unconfirmed Entities
- `Buyer` and `PaymentMethod` (BuyerAggregate) exist as source files but do not appear in the 7-entity `layer1_db_artifacts.ef_entities` list (which only lists CatalogContext's `OnModelCreating`-configured types). This could mean:
  - (a) They are configured via `modelBuilder.Entit
... [truncated]
```

### migration-complexity.json
```
{
  "db_connection": "CODE-ONLY — db_connection_results empty in extraction. Complexity scoring below is based on code structure (entity count, cross-DB references, owned types) — no live schema diff was possible.",
  "overall_complexity": "MEDIUM",
  "rationale": "Small entity surface (7 EF entities in CatalogContext + standard ASP.NET Identity schema), but two separate databases with an unenforced cross-database FK (BuyerId) increase migration risk — any migration tooling that assumes a single schema/transaction boundary will not be able to enforce referential integrity across CatalogDb and IdentityDb.",
  "per_table_complexity": [
    { "table": "CatalogBrands", "complexity": "LOW", "reason": "Simple lookup table, 2 columns, no FK dependents besides Catalog.", "confidence": 0.8 },
    { "table": "CatalogTypes", "complexity": "LOW", "reason": "Simple lookup table, 2 columns.", "confidence": 0.8 },
    { "table": "Catalog", "complexity": "MEDIUM", "reason": "Referenced by BasketItems and OrderItems; FK delete-behavior unknown (UNKNOWN, see data-quality-report.md). Migrating this table requires checking for orphaned-row handling.", "confidence": 0.75 },
    { "table": "Baskets", "complexity": "MEDIUM", "reason": "Cross-database soft FK (BuyerId -> IdentityDb.AspNetUsers.Id) cannot be validated by migration tooling automatically.", "confidence": 0.8 },
    { "table": "BasketItems", "complexity": "LOW", "reason": "Straightforward child table with two FKs into CatalogDb only.", "confidence": 0.8 },
    { "table": "Orders", "complexity": "MEDIUM", "reason": "Owned-type Address flattened into columns (5 extra columns) + cross-database soft FK BuyerId.", "confidence": 0.8 },
    { "table": "OrderItems", "complexity": "MEDIUM", "reason": "Owned-type CatalogItemOrdered flattened into columns; denormalized snapshot data needs careful handling during schema migration (don't accidentally re-link to live Catalog).", "confidence": 0.8 },
    { "table": "AspNetUsers / AspNetRoles / Identity tables", "complexity": "LOW", "reason": "Standard ASP.NET Core Identity schema — well-documented, tooling (dotnet ef) handles migrations automatically. INFERRED, confidence 0.7." }
  ],
  "cross_database_concerns": [
    {
      "concern": "BuyerId soft references (Baskets.BuyerId, Orders.BuyerId -> AspNetUsers.Id)",
      "impact": "Any migration that changes AspNetUsers.Id type/format (e.g. switching from string GUID to int) would silently break Basket/Order ownership with no DB-level error.",
      "confidence": 0.8
    }
  ],
  "unresolved_items": [
    {
      "item": "Buyer / PaymentMethod entities not present in supplied ef_entities list",
      "impact": "If these are actually mapped tables, migration plan is incomplete until live schema is confirmed.",
      "confidence": 0.6
    }
  ],
  "estimated_effort": {
    "schema_migration": "LOW-MEDIUM — 9 confirmed tables (7 CatalogDb + ~6-7 Identity tables, standard scaffolding)",
    "data_migration": "UNKNOWN — row co
... [truncated]
```

### hidden-business-rules.json
```
{
  "db_connection": "N/A — this file is derived from Phase 1 code structure, not DB state.",
  "hidden_business_rules": [
    {
      "rule": "A basket cannot be checked out if it has no items.",
      "evidence": "EmptyBasketOnCheckoutException (src/ApplicationCore/Exceptions/EmptyBasketOnCheckoutException.cs), used presumably by OrderService.cs",
      "confidence": 0.8,
      "category": "validation"
    },
    {
      "rule": "Basket lookups by id must succeed or the operation fails fast with a typed exception (not a generic null-ref).",
      "evidence": "BasketNotFoundException (src/ApplicationCore/Exceptions/BasketNotFoundException.cs)",
      "confidence": 0.8,
      "category": "validation"
    },
    {
      "rule": "Certain entities (likely CatalogBrand/CatalogType by name, or Buyer by BuyerId) must be unique — duplicate creation is explicitly guarded against rather than relying on a DB unique constraint error bubbling up.",
      "evidence": "DuplicateException (src/ApplicationCore/Exceptions/DuplicateException.cs)",
      "confidence": 0.7,
      "category": "validation",
      "notes": "UNKNOWN which specific entity/field this guards — not visible in supplied extraction."
    },
    {
      "rule": "Order line items must capture a point-in-time snapshot of product name/price/image rather than a live reference, so historical orders display correctly even if the catalog item is later changed or deleted.",
      "evidence": "CatalogItemOrdered owned type (src/ApplicationCore/Entities/OrderAggregate/CatalogItemOrdered.cs) embedded in OrderItem",
      "confidence": 0.85,
      "category": "data_integrity / business_rule"
    },
    {
      "rule": "Inventory reordering: items have AvailableStock, RestockThreshold, MaxStockThreshold and an OnReorder flag — implies an automatic/manual reorder workflow triggers when AvailableStock <= RestockThreshold.",
      "evidence": "CatalogItem entity fields (INFERRED from naming, src/ApplicationCore/Entities/CatalogItem.cs)",
      "confidence": 0.7,
      "category": "calculation / process",
      "notes": "Actual trigger logic (5 calculation-category artifacts reported in extraction summary) not visible in supplied excerpts — Agent 2 should locate the reorder calculation method."
    },
    {
      "rule": "Role-based access control distinguishes at least an 'Administrators' role from regular buyers, gating BlazorAdmin and certain PublicApi/Web endpoints.",
      "evidence": "Roles class (src/BlazorShared/Authorization/Constants.cs), AuthorizationConstants (src/ApplicationCore/Constants/AuthorizationConstants.cs)",
      "confidence": 0.75,
      "category": "authentication / authorization"
    },
    {
      "rule": "Frontend caching layer for catalog lookups (brands/types/items) has a TTL or invalidation policy that must be respected — stale data risk if TTL too long, perf risk if too short.",
      "evidence": "CacheEntry.cs, CachedCatalogItemServiceDecorator.cs, CachedCatalogLookupDataService
... [truncated]
```

### storage-pattern-analysis.md
```
# Storage Pattern Analysis — eShopOnWeb

## 1. Relational Storage (Primary)
- **CatalogDb** (SQL Server / Azure SQL Edge in docker-compose) — accessed via EF Core `CatalogContext`, repository pattern (`IRepository<T>`, `IReadRepository<T>` in src/ApplicationCore/Interfaces/). confidence 0.85
- **IdentityDb** (SQL Server / Azure SQL Edge) — ASP.NET Core Identity, standard `UserManager`/`SignInManager` access patterns. confidence 0.7 (INFERRED, not directly observed)

## 2. Caching Layer ⚠️ (mandatory section per Phase 1 step 5)

| Component | File | Pattern | TTL / Eviction |
|---|---|---|---|
| `CachedCatalogItemServiceDecorator` | src/BlazorAdmin/Services/CachedCatalogItemServiceDecorator.cs | Decorator over `ICatalogItemService` — wraps PublicApi calls for catalog item CRUD | UNKNOWN — confidence < 0.7, not visible in supplied extraction |
| `CachedCatalogLookupDataServiceDecorator` | src/BlazorAdmin/Services/CachedCatalogLookupDataServiceDecorator .cs | Decorator over `ICatalogLookupDataService` — caches brand/type lookup lists | UNKNOWN |
| `CacheEntry` | src/BlazorAdmin/Services/CacheEntry.cs | Likely a simple `{ Value, ExpiresAt }` wrapper used by the two decorators above | UNKNOWN exact expiry duration |

**Caching as a business rule**: see hidden-business-rules.json — the TTL/invalidation policy for these decorators directly affects whether BlazorAdmin users see stale catalog data after an edit. This should be documented explicitly once the TTL value is located (Agent 2 action item).

`RefreshBroadcast.cs` (src/BlazorAdmin/Helpers/) appears to be the invalidation-signal mechanism that complements this cache — likely a simple pub/sub (C# event or similar) notifying components to re-fetch after a write. confidence 0.65, UNKNOWN exact implementation.

## 3. Client-Side Storage
- **Browser cookies** via `Cookies.cs` (src/BlazorAdmin/JavaScript/) + `CustomAuthStateProvider.cs` — used to persist auth tokens for the Blazor WASM admin client. confidence 0.75

## 4. Background / Async Writers
- No background DB writer classes (`*/Background*`) were present in the supplied entity extraction. **Absence noted** — if eShopOnWeb has a `*HostedService` for things like stock-reorder processing, it was not captured. UNKNOWN, confidence < 0.7.

## 5. Health Checks
- No `*/HealthChecks*` files were present in the supplied entity extraction. **Absence noted** — typical eShopOnWeb has `AddHealthChecks().AddDbContextCheck<CatalogContext>()` etc. in `Program.cs`/`Startup.cs`, which were not part of this extraction. UNKNOWN, confidence < 0.7.

## 6. Configuration-Driven Storage Behavior
- `CatalogSettings` class (src/ApplicationCore/CatalogSettings.cs) — likely binds to an `appsettings` section controlling things like `CatalogBaseUrl` for product images (used by `IUriComposer`/`UriComposer.cs`). confidence 0.75

## Summary
The codebase follows a clean **Repository + Specification pattern** (IRepository/IReadRepository) over two SQL databases, with a thin **decor
... [truncated]
```

### redundancy-analysis.json
```
{
  "db_connection": "N/A — derived from Phase 1 code review (entity list + cross-reference of CatalogContext ef_entities vs. full entity source list).",
  "canonical_shadow_check_performed": true,
  "findings": [
    {
      "concept": "Customer / Buyer",
      "representations": [
        {
          "representation": "AspNetUsers (IdentityDb)",
          "role": "CANONICAL — source of truth for identity, login, email, roles.",
          "confidence": 0.8
        },
        {
          "representation": "Buyer (src/ApplicationCore/Entities/BuyerAggregate/Buyer.cs)",
          "role": "POSSIBLE SHADOW — exists as a class but was NOT present in the supplied layer1_db_artifacts.ef_entities list (only 7 CatalogContext entities listed: CatalogType, CatalogBrand, Order, OrderItem, BasketItem, Basket, CatalogItem). This is the canonical/shadow check trigger: a representation that is defined but whose mapping/instantiation could not be confirmed.",
          "confidence": 0.6,
          "action": "Agent 2 should grep for `modelBuilder.Entity<Buyer>` and any `_buyerRepository`/`IRepository<Buyer>` usage to confirm whether Buyer is actually persisted and queried, or is dead/aspirational code."
        },
        {
          "representation": "Basket.BuyerId / Order.BuyerId (string)",
          "role": "SHADOW / loose reference — a string foreign key into AspNetUsers.Id, duplicated across two tables, with no enforced FK. Classic 'loose identifier' pattern called out in the skill's canonical/shadow guidance.",
          "confidence": 0.8
        }
      ],
      "recommendation": "If Buyer is confirmed unused (dead code), BuyerId string fields are the de-facto canonical link and should be documented as such with an explicit note that referential integrity is app-enforced only. If Buyer IS used, clarify the relationship between Buyer.Id and AspNetUsers.Id / BuyerId strings — there may be three overlapping identifiers for one customer."
    },
    {
      "concept": "Catalog Product",
      "representations": [
        {
          "representation": "CatalogItem (Catalog table)",
          "role": "CANONICAL — live product data, source of truth for price/stock/description.",
          "confidence": 0.85
        },
        {
          "representation": "CatalogItemOrdered (owned type, embedded in OrderItems)",
          "role": "INTENTIONAL DUPLICATE / historical snapshot — not a shadow entity in the negative sense. Captures product name/price/image at time of order for audit/history purposes.",
          "confidence": 0.85
        },
        {
          "representation": "CatalogItemDetails (struct, src/ApplicationCore/Entities/CatalogItem.cs)",
          "role": "POSSIBLE SHADOW / DTO — a struct co-located with CatalogItem class. Likely a read-model/value-object for API responses (e.g. combining CatalogItem with brand/type names), not a persisted duplicate.",
          "confidence": 0.7,
          "action": "Confirm CatalogItemDetails is used only as a proje
... [truncated]
```

### data-dictionary.md
```
# Data Dictionary — eShopOnWeb

`db_connection: CODE-ONLY — definitions below derived from EF entity structure and naming conventions; values not confirmed against live data.`

## CatalogDb

### CatalogBrands
| Column | Type | Description | Confidence |
|---|---|---|---|
| Id | int | Primary key | 0.8 |
| Brand | string | Display name of the product brand (e.g. "Adventure Works") | 0.8 |

### CatalogTypes
| Column | Type | Description | Confidence |
|---|---|---|---|
| Id | int | Primary key | 0.8 |
| Type | string | Product category name (e.g. "Mugs", "T-Shirts") | 0.8 |

### Catalog (CatalogItem)
| Column | Type | Description | Confidence |
|---|---|---|---|
| Id | int | Primary key | 0.8 |
| Name | string | Product display name | 0.8 |
| Description | string | Long-form product description shown on detail page | 0.75 |
| Price | decimal | Unit sale price | 0.8 |
| PictureUri | string | Path/URL to product image, composed via IUriComposer | 0.75 |
| CatalogTypeId | int (FK) | Category classification → CatalogTypes.Id | 0.8 |
| CatalogBrandId | int (FK) | Brand classification → CatalogBrands.Id | 0.8 |
| AvailableStock | int | Current units available for sale | 0.75 |
| RestockThreshold | int | Stock level below which reorder is triggered | 0.75 |
| MaxStockThreshold | int | Maximum stock level the warehouse will hold | 0.75 |
| OnReorder | bool | Flag indicating a reorder is currently in progress | 0.75 |

### Baskets (Basket)
| Column | Type | Description | Confidence |
|---|---|---|---|
| Id | int | Primary key | 0.8 |
| BuyerId | string | Soft reference to AspNetUsers.Id (IdentityDb) — identifies the basket's owner | 0.8 |

### BasketItems (BasketItem)
| Column | Type | Description | Confidence |
|---|---|---|---|
| Id | int | Primary key | 0.8 |
| BasketId | int (FK) | → Baskets.Id | 0.8 |
| CatalogItemId | int (FK) | → Catalog.Id | 0.8 |
| UnitPrice | decimal | Price captured at time of adding to basket | 0.75 |
| Quantity | int | Number of units of this item in the basket | 0.75 |

### Orders (Order)
| Column | Type | Description | Confidence |
|---|---|---|---|
| Id | int | Primary key | 0.8 |
| BuyerId | string | Soft reference to AspNetUsers.Id (IdentityDb) — who placed the order | 0.8 |
| OrderDate | datetime | Date/time the order was placed | 0.8 |
| ShipToAddress_Street | string | Shipping address — street (owned type Address, flattened) | 0.75 |
| ShipToAddress_City | string | Shipping address — city | 0.75 |
| ShipToAddress_State | string | Shipping address — state/province | 0.75 |
| ShipToAddress_Country | string | Shipping address — country | 0.75 |
| ShipToAddress_ZipCode | string | Shipping address — postal code | 0.75 |

### OrderItems (OrderItem)
| Column | Type | Description | Confidence |
|---|---|---|---|
| Id | int | Primary key | 0.8 |
| OrderId | int (FK) | → Orders.Id | 0.8 |
| ItemOrdered_CatalogItemId | int | Snapshot of the catalog item id at time of order (not a live FK) | 0.75 |
| ItemOrdered_ProductName | strin
... [truncated]
```

### conceptual-data-model.md
```
# Conceptual Data Model — eShopOnWeb

*Business language only — no table names, data types, or FK syntax.*

## Core Business Concepts

**Customer**
A person who can log in, browse products, hold a shopping basket, and place orders. Customers have an identity (login credentials, email, roles such as Administrator) and may store one or more payment methods for checkout.

**Product**
An item offered for sale. Each product belongs to a Brand and a Category (Type), has a name, description, price, and an image. Each product also tracks how many units are currently available, and the warehouse rules for when to reorder more stock (a low-stock threshold, a maximum stock level, and whether a reorder is currently underway).

**Brand**
A manufacturer or label that groups products together (e.g. a clothing brand).

**Category**
A classification grouping products by type (e.g. mugs, t-shirts, stickers).

**Shopping Basket**
A temporary collection of products a customer intends to purchase, along with the quantity and price of each. A basket belongs to exactly one customer and exists until checkout (or abandonment).

**Order**
A confirmed purchase made by a customer. An order records when it was placed, where it should be shipped (a shipping address consisting of street, city, state, country, and postal code), and a list of the products purchased — each captured as it appeared *at the time of purchase* (name, price, image), so that historical orders remain accurate even if the product catalog changes later.

**Payment Method**
A way for a customer to pay for an order. Associated with a customer's account.

**Role / Permission**
Customers (and staff) are assigned roles (e.g. "Administrator") that determine what parts of the system they can access — notably the catalog-management admin tools.

## Relationships (in business terms)

- A **Customer** has **one Shopping Basket** at a time.
- A **Shopping Basket** contains **many Basket Lines**, each referring to one **Product** with a quantity and price.
- A **Customer** can place **many Orders** over time.
- An **Order** contains **many Order Lines**, each describing one **Product** as it was at the time of purchase, with quantity and price paid.
- A **Product** belongs to **one Brand** and **one Category**.
- A **Brand** can have **many Products**.
- A **Category** can have **many Products**.
- A **Customer** may have **one or more Payment Methods**. *(Existence/usage of this concept in current system is unconfirmed — see data-quality-report.md.)*
- A **Customer** has **one or more Roles** governing system access.

## Business Lifecycle Notes
- Products have a **stock replenishment lifecycle**: available stock depletes as orders are placed; when it falls below a restock threshold, a reorder process is signaled (`OnReorder`); stock is replenished up to a maximum threshold.
- An **Order cannot be created from an empty Basket** — this is an enforced business rule, not just a UI restriction.
- Once an **Order** is pla
... [truncated]
```

### access-control-matrix.md
```
# Access Control Matrix — eShopOnWeb

`db_connection: CODE-ONLY — db_connection_results empty; no live role/permission grants checked. Roles below are derived from class/source names found in extraction (AuthorizationConstants, BlazorShared.Authorization.Constants/Roles).`

## Roles Identified

| Role | Source Evidence | Confidence |
|---|---|---|
| Administrators (or "Administrator") | `Roles` class (src/BlazorShared/Authorization/Constants.cs); `AuthorizationConstants` (src/ApplicationCore/Constants/AuthorizationConstants.cs) | 0.75 |
| Authenticated user (default, no explicit role name found) | Standard ASP.NET Identity — any logged-in AspNetUsers row | 0.7 |
| Anonymous / Guest | Implicit — catalog browsing and basket creation typically allowed without login in eShopOnWeb | 0.7 |

> Exact role name strings (e.g. whether it's literally `"Administrators"`) could not be confirmed — the supplied extraction included the *file names* containing these constants but not their string values. confidence 0.75. Agent 2 should open `AuthorizationConstants.cs` and `BlazorShared/Authorization/Constants.cs` to extract literal role names.

## Access Matrix (by data domain)

| Data Domain | Anonymous | Authenticated Customer | Administrator |
|---|---|---|---|
| Catalog (browse products/brands/types) | ✅ Read | ✅ Read | ✅ Read/Write (via BlazorAdmin + PublicApi) |
| Basket (own) | ✅ Create/Read/Update (session or guest-id based) | ✅ Full CRUD on own basket | ✅ (own basket only, no special elevation expected) |
| Basket (other users') | ❌ | ❌ | UNKNOWN — confidence < 0.7, not confirmed whether admins can view other baskets |
| Orders (own) | ❌ (must authenticate to checkout) | ✅ Create + Read own orders | UNKNOWN whether admins can view all orders — INFERRED ✅ likely yes (typical admin capability), confidence 0.65 |
| Orders (all customers) | ❌ | ❌ | INFERRED ✅, confidence 0.65 — not confirmed in supplied extraction |
| Identity (AspNetUsers/Roles) | ❌ | Self only (profile/password) | INFERRED ✅ user management, confidence 0.6 — ASP.NET Identity admin UI not confirmed present |
| Payment Methods | ❌ | Self only (if feature exists — see redundancy-analysis.json) | ❌ |
| BlazorAdmin UI (catalog management) | ❌ | ❌ | ✅ — gated via `[Authorize]`-style attributes referencing the Administrator role, per `Roles`/`AuthorizationConstants` classes |

## Authentication Mechanism
- `ITokenClaimsService` (src/ApplicationCore/Interfaces/ITokenClaimsService.cs) — issues claims, likely backing JWT bearer auth for PublicApi consumed by BlazorAdmin/mobile clients. confidence 0.7.
- `CustomAuthStateProvider` (src/BlazorAdmin/CustomAuthStateProvider.cs) — Blazor WASM client-side auth state, backed by cookie storage (`Cookies.cs`). confidence 0.75.

## Gaps / Action Items for Agent 2
1. Extract literal role name strings from `AuthorizationConstants.cs` and `BlazorShared/Authorization/Constants.cs`.
2. Confirm whether Administrators can view/modify other customers' orders or baskets
... [truncated]
```

---

---
name: da-reviewer
description: Data architecture review and enrichment. Use when user says "review the analysis", 
  "run Agent 2", "enrich the findings", "validate the DA outputs", or after DA Agent 1 has 
  produced its 13 output files. Do NOT use before DA Agent 1 has completed. Do NOT re-run 
  the full extraction — only review and enrich what exists.
---

# DA Agent 2 — Data Architecture Reviewer
> Pair with: `DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md` | Version: June 2026 v2

---

## When to Activate

- DA Agent 1 has produced files in `da-outputs/`
- User says "review the analysis", "run Agent 2", "enrich the findings", "check what was missed"
- User wants to prepare for a Gate G1 stakeholder review meeting
- User wants to raise confidence scores before sharing the documents

---

## What This Skill Does NOT Do

- Does not re-run the full extraction — only updates what new evidence changes
- Does not add findings without citing evidence (file:line or SQL result)
- Does not escalate questions to Gate G1 that can be answered by reading more code
- Does not merge the review summary into the 13 output files — `review-summary.md` is always separate
- Does not raise a confidence score without a specific test, query result, or document line as proof

---

## Pre-Flight Check — Run Before Anything Else

Read `da-outputs/schema-catalogue.json`. Check the `db_connection` field.

| If `db_connection` is | Then |
|---|---|
| `"CONNECTED"` | Proceed to Phase 1 |
| `"CODE-ONLY"` or missing | Connect to the database NOW (find the CLI tool on this machine, run a connection test, then row counts / schema / FK / index / DQ checks). Update all 13 files with live data first. Do not begin Phase 1 until done. |

---

## Steps

**Phase 1 — Test File Evidence**
1. Find all test files — record total count by type and framework used
2. Read tests in priority order:
   - Priority 1: Business rule tests (`*Entities*`, `*Domain*`, `*Services*` — price, quantity, validation, transfer)
   - Priority 2: Repository / integration tests — look for comments about InMemory vs SQL DB differences
   - Priority 3: Functional / E2E tests — confirm hardcoded passwords, addresses, emails
   - Priority 4: Builders / factories (`*Builder*`, `*Factory*`, `*Fixture*`) — reveal valid domain data shapes
3. For each finding a test changes — write a change record (see format below) and update the output file

**Phase 2 — Documentation Review**
1. Read `README.md` and all files in `docs/`
2. Extract: stated purpose (demo vs production), deployment model, external system references, known limitations, demo credentials
3. Apply findings to relevant output files — record each as a change record

**Phase 3 — Database Verification** *(if DB was connected in Agent 1)*
1. Run targeted confirmation queries for findings from Phases 1–2
2. Update any finding where live data contradicts or confirms Agent 1's assumption
3. **Conflict resolution** — when a test, doc, or DB result disagrees with Agent 1's code-based finding, don't average the two scores. Rank both against this Evidence Strength Hierarchy (highest wins): live DB > migration files > entity/ORM code > tests > repository code > naming convention > docs/git history (docs/git only win if they cite a hard constraint). The higher-ranked source wins; record both in the change record's `evidence_detail`.

**Phase 4 — Spot Check of Unreferenced Files**
1. List all source files referenced in the 13 output files
2. Open files from directories NOT yet covered: `*/Config*` `*/Extensions*` `*/HealthChecks*` `*/Cache*` `*/Background*` `*/Events*`
3. For each new file — check for missed caching layers, feature flags, background DB writers, health check dependencies
4. Record every new file read and whether it produced a finding

**Phase 5 — Cross-File Consistency Check**

| Check | Files |
|---|---|
| Same table count | `schema-catalogue.json` ↔ `erd.md` |
| PII columns match | `pii-inventory.json` ↔ `schema-catalogue.json` |
| Row counts match | `schema-catalogue.json` ↔ `migration-complexity.json` |
| Business rules in flow map | `hidden-business-rules.json` ↔ `data-flow-map.md` |
| Cache in both places | `data-source-inventory.json` ↔ `storage-pattern-analysis.md` |
| FK delete rules consistent | `schema-catalogue.json` ↔ `migration-complexity.json` |
| Canonical entity claims match actual table/usage evidence | `redundancy-analysis.json` ↔ `schema-catalogue.json` |
| Every table/column has a dictionary entry, and none invent meanings absent from code | `data-dictionary.md` ↔ `schema-catalogue.json` |
| Every concept in the conceptual model traces to a real aggregate root | `conceptual-data-model.md` ↔ `schema-catalogue.json` |
| Every PII table/column appears in the access matrix with cited evidence | `access-control-matrix.md` ↔ `pii-inventory.json` |

Fix every contradiction found. Record each as a CORRECTED change record.

**Phase 6 — Write `da-outputs/review-summary.md`**

Include these 6 sections:
1. **Overview** — files reviewed (13 of 13), total change count by type (ADDED/CORRECTED/ENRICHED)
2. **Quality scores** — overall confidence before vs. after review
3. **Key corrections** — the most significant CORRECTED change records, with file + finding
4. **Cross-file consistency results** — outcome of each Phase 5 check (pass/fixed)
5. **Open questions for Gate G1** — anything requiring business intent, legal input, or infrastructure sizing that code-reading alone can't resolve
6. **Gate G1 recommendation** — READY or NOT READY, with reason if not ready

---

## Output Format

**Change record (add to every updated output file):**
```json
{
  "change_id": "RC-007",
  "type": "CORRECTED",
  "finding_id": "storage-pattern-analysis.md — Caching",
  "what": "Original said 'Cache Type: None'. IMemoryCache IS active via CachedCatalogViewModelService (30s sliding TTL).",
  "evidence_source": "spot check",
  "evidence_detail": "src/Web/Services/CachedCatalogViewModelService.cs + ConfigureWebServices.cs:17",
  "confidence_before": 0.0,
  "confidence_after": 1.0,
  "phase_found": "Phase 4 spot check"
}
```

**Change types:**

| Type | Meaning |
|---|---|
| ADDED | New finding not in Agent 1 |
| CORRECTED | Agent 1 was wrong — now fixed |
| ENRICHED | Agent 1 was correct — now has more evidence |

---

## Error Handling

| If | Then |
|---|---|
| `da-outputs/` folder missing or has fewer than 10 files | Stop — ask user to run DA Agent 1 first |
| `schema-catalogue.json` is empty or has 0 tables | Stop — Agent 1 may not have completed |
| Spot check reveals fundamentally different architecture (event sourcing, CQRS read DB, multi-tenancy) | Stop — ask user if Agent 1 should re-run with this knowledge |
| Test contradicts an Agent 1 finding | Mark CORRECTED — never silently update without recording the change |
| Open question can be answered by reading more code | Read the code and answer it — do not escalate to Gate G1 |
| Open question requires business intent, legal input, or infrastructure sizing | Add to Gate G1 list with role assigned |

---

## Final Report to User

```
## 📋 DA Agent 2 — Review Complete

Files reviewed:   13 of 13
Changes made:     [N] ADDED, [N] CORRECTED, [N] ENRICHED

Quality scores:
  Before review:  [X.XX] overall
  After review:   [X.XX] overall

Key corrections:
  - [most important correction]
  - [second most important]

Open questions for Gate G1:  [N]
  → See da-outputs/review-summary.md

Gate G1 recommendation:  READY | NOT READY ([reason])
```

---

*DA Reverse Engineering System — Agent 2 of 2 | v2 | June 2026*

---

## Reminder on output

Apply all review phases (including Phase 5 cross-file consistency checks)
across the 13 files. Update any file you change directly at
C:/Users/BrianRoyS/Downloads/bussiness-architecture 1/bussiness-architecture/output/eShopOnWeb/da-outputs/<filename> using your file tools, then write
C:/Users/BrianRoyS/Downloads/bussiness-architecture 1/bussiness-architecture/output/eShopOnWeb/da-outputs/review-summary.md (required), then reply with the checklist.
