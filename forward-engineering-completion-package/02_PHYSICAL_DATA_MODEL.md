# 02 — Physical Data Model (closes C2, part 1 of 2)

> ⚠️ **DISC-001 (verified 2026-06-25):** The `CatalogItem` columns `AvailableStock`, `RestockThreshold`,
> `MaxStockThreshold`, `OnReorder` and the `CK_CatalogItem_Stock` check constraint specified below are a
> **verified discrepancy** — those fields are absent from the real `eShopOnWeb` source. **Generate
> `CatalogItem` with only:** Id, Name, Description, Price, PictureUri, CatalogTypeId, CatalogBrandId
> (+ FKs). Omit the four stock columns and the stock check constraint. See
> [`../EVIDENCE_VERIFICATION_REPORT.md`](../EVIDENCE_VERIFICATION_REPORT.md).

**Closes:** Audit C2 — "No physical data model." Promotes the logical model (docs 05–08) to a physical
specification: types, nullability, lengths, defaults, keys, indexes, constraints, naming.
**Authority:** doc 06 (Data Dictionary), doc 07 (Data Model Spec), doc 08 (ERD), graph `data.*` (15 entities,
12 relationships, 4 aggregates).
**Scope:** the **11 implemented/persisted entities only.** The 3 aspirational entities (DATA-ENT-010 Buyer,
-011 PaymentMethod, -014 CatalogItemDetails) are **excluded** (empty attribute sets; GR-05 SKIP) and the
abstract DATA-ENT-015 BaseEntity is a supertype (no table). DDL syntax is in `03_DATABASE_DDL_SPECIFICATION.md`.

> Type choices are ⚠ **NEUTRAL DEFAULTS** derived from logical types + the recommended PostgreSQL target
> (doc 01). They are overridable per chosen DB. Lengths/defaults marked 🟦 where evidence is silent.

---

## 1. Naming standards & conventions

| Element | Convention | Example |
|---|---|---|
| Table | PascalCase singular (mirror legacy EF entity) | `CatalogItem` |
| Column | PascalCase (legacy) | `CatalogBrandId` |
| PK | `Id` | `Id` |
| FK column | `<Target>Id` | `CatalogBrandId` |
| Owned/embedded VO | `<Owner>_<Field>` flatten (legacy convention) | `ShipToAddress_City` |
| Index | `IX_<Table>_<Cols>` | `IX_CatalogItem_CatalogBrandId` |
| Unique | `UQ_<Table>_<Cols>` | `UQ_ApplicationUser_NormalizedEmail` |
| FK constraint | `FK_<Child>_<Parent>` | `FK_BasketItem_Basket` |
| Check | `CK_<Table>_<Rule>` | `CK_BasketItem_Quantity` |
| Join table | `<A><B>` (Identity convention) | `AspNetUserRoles` |

**Normalization:** target is **3NF** for transactional tables. Two deliberate denormalizations are
**DERIVED from the domain model** and preserved: (a) owned value objects flattened into parent rows
(Address into Order, CatalogItemOrdered into OrderItem — DATA-REL-006/007); (b) ordered-item **snapshot**
copies product fields into OrderItem (VO-03, must not FK to live CatalogItem — DR-06).

---

## 2. Physical tables (11 implemented entities)

Legend: PK = primary key · FK = foreign key · NN = NOT NULL · 🟦 = length/default requires confirmation.

### 2.1 `CatalogItem` (DATA-ENT-001 · aggregate root DATA-AGG-004)

| Column | Type (Postgres default) | Null | Key | Default | Source |
|---|---|---|---|---|---|
| Id | `integer GENERATED ALWAYS AS IDENTITY` | NN | PK | — | DATA-ENT-001 |
| Name | `varchar(100)` 🟦 | NN | | — | key_attr |
| Description | `text` | NULL | | — | key_attr |
| Price | `numeric(18,2)` | NN | | — | VO-05 Money (amount-only) |
| PictureUri | `varchar(1000)` 🟦 | NULL | | — | key_attr |
| CatalogTypeId | `integer` | NN | FK→CatalogType | — | DATA-REL-002 |
| CatalogBrandId | `integer` | NN | FK→CatalogBrand | — | DATA-REL-001 |
Constraints: `CK_CatalogItem_Price (Price >= 0)` (BR001).
Indexes: `IX_CatalogItem_CatalogBrandId`, `IX_CatalogItem_CatalogTypeId` (FK indexes).

### 2.2 `CatalogBrand` (DATA-ENT-002)

| Column | Type | Null | Key |
|---|---|---|---|
| Id | `integer GENERATED ALWAYS AS IDENTITY` | NN | PK |
| Brand | `varchar(100)` 🟦 | NN | |

Constraint: `CK_CatalogBrand_Id (Id <> 0)` (BR002).

### 2.3 `CatalogType` (DATA-ENT-003)

| Column | Type | Null | Key |
|---|---|---|---|
| Id | `integer GENERATED ALWAYS AS IDENTITY` | NN | PK |
| Type | `varchar(100)` 🟦 | NN | |

Constraint: `CK_CatalogType_Id (Id <> 0)` (BR003).

### 2.4 `Basket` (DATA-ENT-004 · aggregate root DATA-AGG-001)

| Column | Type | Null | Key | Note |
|---|---|---|---|---|
| Id | `integer GENERATED ALWAYS AS IDENTITY` | NN | PK | |
| BuyerId | `varchar(256)` 🟦 | NN | **soft ref** | DATA-REL-008 → ApplicationUser.Id; **no DB FK** (cross-DB); app-enforced. For anonymous baskets holds a session/cookie key 🟦 (doc 09 ASMP-FE-005) |

Index: `IX_Basket_BuyerId` (lookup by buyer).

### 2.5 `BasketItem` (DATA-ENT-005 · member of DATA-AGG-001)

| Column | Type | Null | Key |
|---|---|---|---|
| Id | `integer GENERATED ALWAYS AS IDENTITY` | NN | PK |
| BasketId | `integer` | NN | FK→Basket (ON DELETE CASCADE — within aggregate) |
| CatalogItemId | `integer` | NN | **soft ref** DATA-REL-004 (cross-context; no DB FK) |
| UnitPrice | `numeric(18,2)` | NN | |
| Quantity | `integer` | NN | |

Constraints: `CK_BasketItem_Quantity (Quantity >= 0)` (BR006/BR007); unique `UQ_BasketItem_Basket_Catalog (BasketId, CatalogItemId)` ⚠ (BR005 line-consolidation option). Index: `IX_BasketItem_BasketId`.

### 2.6 `Order` (DATA-ENT-006 · aggregate root DATA-AGG-002 · **PII**)

| Column | Type | Null | Key |
|---|---|---|---|
| Id | `integer GENERATED ALWAYS AS IDENTITY` | NN | PK |
| BuyerId | `varchar(256)` | NN | **soft ref** DATA-REL-009 (cross-DB; no FK) (BR011) |
| OrderDate | `timestamptz` | NN | default `now()` ⚠ |
| ShipToAddress_Street | `varchar(180)` 🟦 | NN | owned VO-01 (PII) |
| ShipToAddress_City | `varchar(100)` 🟦 | NN | owned VO-01 (PII) |
| ShipToAddress_State | `varchar(100)` 🟦 | NULL | owned VO-01 |
| ShipToAddress_Country | `varchar(100)` 🟦 | NN | owned VO-01 |
| ShipToAddress_ZipCode | `varchar(18)` 🟦 | NN | owned VO-01 |

> **No `Total` column** — order total is **derived** `Σ(UnitPrice×Units)` (BR010, doc 07). If a stored total
> is later required, add `Total numeric(18,2)` maintained by the application on the OrderAggregate (🟦 decision).
Index: `IX_Order_BuyerId`.

### 2.7 `OrderItem` (DATA-ENT-007 · member of DATA-AGG-002)

| Column | Type | Null | Key |
|---|---|---|---|
| Id | `integer GENERATED ALWAYS AS IDENTITY` | NN | PK |
| OrderId | `integer` | NN | FK→Order (ON DELETE CASCADE) |
| ItemOrdered_CatalogItemId | `integer` | NN | owned snapshot VO-03 (**no FK** — DR-06) |
| ItemOrdered_ProductName | `varchar(100)` 🟦 | NN | owned snapshot (BR009) |
| ItemOrdered_PictureUri | `varchar(1000)` 🟦 | NULL | owned snapshot |
| UnitPrice | `numeric(18,2)` | NN | |
| Units | `integer` | NN | `CK_OrderItem_Units (Units >= 1)` (ASMP-DD-001) |

Index: `IX_OrderItem_OrderId`.

### 2.8 `ApplicationUser` (DATA-ENT-008 · **PII** · INFERRED 0.7)

Listed attributes are evidence-backed; **the remaining standard ASP.NET Core Identity columns are
🟦 REQUIRES HUMAN DECISION / source verification** (ASMP-DD-003). Generating Identity without them breaks
lockout/2FA/concurrency.

| Column | Type | Null | Key | Source |
|---|---|---|---|---|
| Id | `varchar(450)` (Identity default) | NN | PK | DATA-ENT-008 |
| UserName | `varchar(256)` | NULL | | key_attr |
| NormalizedUserName | `varchar(256)` | NULL | UQ | Identity convention 🟦 |
| Email | `varchar(256)` | NULL | | key_attr (PII) |
| NormalizedEmail | `varchar(256)` | NULL | UQ | Identity convention 🟦 |
| EmailConfirmed | `boolean` | NN | `false` | Identity 🟦 |
| PasswordHash | `text` | NULL | | key_attr |
| SecurityStamp | `text` | NULL | | Identity 🟦 |
| ConcurrencyStamp | `text` | NULL | | Identity 🟦 |
| PhoneNumber | `varchar(50)` 🟦 | NULL | | key_attr (PII) |
| PhoneNumberConfirmed | `boolean` | NN | `false` | Identity 🟦 |
| TwoFactorEnabled | `boolean` | NN | `false` | Identity 🟦 |
| LockoutEnd | `timestamptz` | NULL | | Identity 🟦 |
| LockoutEnabled | `boolean` | NN | `true` | Identity 🟦 |
| AccessFailedCount | `integer` | NN | `0` | Identity 🟦 |

Unique: `UQ_ApplicationUser_NormalizedUserName`, `UQ_ApplicationUser_NormalizedEmail` (ASMP-DD-003).

### 2.9 `Role` (DATA-ENT-009 · INFERRED 0.7)

| Column | Type | Null | Key |
|---|---|---|---|
| Id | `varchar(450)` | NN | PK |
| Name | `varchar(256)` | NULL | |
| NormalizedName | `varchar(256)` | NULL | UQ 🟦 |
| ConcurrencyStamp | `text` | NULL | 🟦 |

Confirmed role value: `Administrators` (RC-008). Unique: `UQ_Role_NormalizedName`.

### 2.10 `AspNetUserRoles` (join — DATA-REL-010, INFERRED)

| Column | Type | Null | Key |
|---|---|---|---|
| UserId | `varchar(450)` | NN | PK, FK→ApplicationUser (ON DELETE CASCADE) |
| RoleId | `varchar(450)` | NN | PK, FK→Role (ON DELETE CASCADE) |

Composite PK `(UserId, RoleId)`. Exact columns 🟦 (verify against IdentityDb).

### 2.11 Owned types (NO independent tables)

- **CatalogItemOrdered (DATA-ENT-012, VO-03)** → flattened into `OrderItem.ItemOrdered_*` (snapshot).
- **Address (DATA-ENT-013, VO-01, PII)** → flattened into `Order.ShipToAddress_*`.
- **BaseEntity (DATA-ENT-015)** → abstract supertype; supplies `Id`; no table.

---

## 3. Primary & foreign key summary

| FK constraint | Child.Column | Parent | On delete | Type |
|---|---|---|---|---|
| FK_CatalogItem_CatalogBrand | CatalogItem.CatalogBrandId | CatalogBrand.Id | RESTRICT | hard (DATA-REL-001) |
| FK_CatalogItem_CatalogType | CatalogItem.CatalogTypeId | CatalogType.Id | RESTRICT | hard (DATA-REL-002) |
| FK_BasketItem_Basket | BasketItem.BasketId | Basket.Id | CASCADE | hard, intra-aggregate (DATA-REL-003) |
| FK_OrderItem_Order | OrderItem.OrderId | Order.Id | CASCADE | hard, intra-aggregate (DATA-REL-005) |
| FK_AspNetUserRoles_User | AspNetUserRoles.UserId | ApplicationUser.Id | CASCADE | hard (DATA-REL-010) |
| FK_AspNetUserRoles_Role | AspNetUserRoles.RoleId | Role.Id | CASCADE | hard (DATA-REL-010) |
| *(soft)* Basket.BuyerId → ApplicationUser.Id | — | — | **app-enforced, no DB FK** | DATA-REL-008 |
| *(soft)* Order.BuyerId → ApplicationUser.Id | — | — | **app-enforced, no DB FK** | DATA-REL-009 |
| *(soft)* BasketItem.CatalogItemId → CatalogItem.Id | — | — | **app-enforced** (cross-context) | DATA-REL-004 |

## 4. Sequences / identity strategy

⚠ NEUTRAL DEFAULT: integer **identity columns** (`GENERATED ALWAYS AS IDENTITY` Postgres / `IDENTITY` SQL Server / `AUTO_INCREMENT` MySQL) for catalog/basket/order tables; Identity tables use **string `Id`** (`varchar(450)`) per ASP.NET Core convention. 🟦 If GUID keys are desired (distributed-friendly), that is a human decision affecting all PKs.

## 5. Index plan

| Index | Table(Cols) | Purpose |
|---|---|---|
| IX_CatalogItem_CatalogBrandId / _CatalogTypeId | FK filters | catalog browse/filter |
| IX_BasketItem_BasketId | aggregate load | basket retrieval |
| IX_OrderItem_OrderId | aggregate load | order detail |
| IX_Basket_BuyerId / IX_Order_BuyerId | soft-ref lookup | by-buyer queries |
| UQ_ApplicationUser_NormalizedEmail/UserName, UQ_Role_NormalizedName | uniqueness | Identity |
| UQ_BasketItem_Basket_Catalog ⚠ | line consolidation (BR005) | confirm before enabling |

## 6. Schema/context boundaries (per-bounded-context split — DB-01)

| Schema / database | Tables | Bounded context | Source |
|---|---|---|---|
| `catalog` | CatalogItem, CatalogBrand, CatalogType | BC-01 | split from shared CatalogContext (RISK-SHARED-DBCTX-001) |
| `basket` | Basket, BasketItem | BC-02 | per-context (DB-01) |
| `ordering` | Order, OrderItem (+owned) | BC-03 | per-context (DB-01) |
| `identity` | ApplicationUser, Role, AspNetUserRoles | BC-04 | AppIdentityDbContext (DATA-REPO-004) |

> Legacy uses **one** `CatalogContext` spanning catalog/basket/ordering (DATA-REPO-003). The split is a
> **🟦 data-migration decision** (effort flagged G-M9): (a) single shared schema (lowest change) or
> (b) per-context schemas/databases (target DB-01). Cross-context references become soft (already are).

## 7. Outstanding 🟦 decisions captured (do not invent)

| # | Decision | Default offered |
|---|---|---|
| D1 | String/length bounds marked 🟦 | sensible defaults above; confirm vs source DTOs |
| D2 | Full Identity column set | ASP.NET Core standard set listed; verify vs IdentityDb |
| D3 | Stored vs derived order Total | derived (no column) — legacy behavior |
| D4 | Basket line-consolidation unique constraint | offered, default OFF until BR005 confirmed |
| D5 | Per-context DB split vs shared schema | per-context (DB-01 target) |
| D6 | Integer-identity vs GUID PKs | integer identity (legacy-aligned) |
| D7 | Default values (dates, flags) | ⚠ defaults above |
