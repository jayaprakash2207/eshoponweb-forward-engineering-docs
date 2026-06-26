# 03 — Database DDL Specification (closes C2, part 2 of 2)

> ⚠️ **DISC-001 (verified 2026-06-25):** In the `CREATE TABLE CatalogItem` DDL below, the four stock
> columns (`AvailableStock`, `RestockThreshold`, `MaxStockThreshold`, `OnReorder`) and the
> `CK_CatalogItem_Stock` constraint are a **verified discrepancy** — absent from the real `eShopOnWeb`
> source. **Omit them** when emitting DDL. See
> [`../EVIDENCE_VERIFICATION_REPORT.md`](../EVIDENCE_VERIFICATION_REPORT.md).

**Closes:** Audit C2 — provides technology-neutral DDL, migration ordering, dependency ordering, and
referential-integrity rules from the physical model (`02_PHYSICAL_DATA_MODEL.md`).
**Nature:** A **specification** an AI generator emits per chosen DB. It is **not** the application's data
layer (no code). Type tokens map per target (PostgreSQL / SQL Server / MySQL).

---

## 1. Type mapping (logical → physical per target)

| Logical | PostgreSQL | SQL Server | MySQL |
|---|---|---|---|
| Identifier (int identity) | `integer GENERATED ALWAYS AS IDENTITY` | `INT IDENTITY(1,1)` | `INT AUTO_INCREMENT` |
| Identity-string Id | `varchar(450)` | `NVARCHAR(450)` | `VARCHAR(450)` |
| ShortText(n) | `varchar(n)` | `NVARCHAR(n)` | `VARCHAR(n)` |
| LongText | `text` | `NVARCHAR(MAX)` | `TEXT` |
| Money | `numeric(18,2)` | `DECIMAL(18,2)` | `DECIMAL(18,2)` |
| Integer | `integer` | `INT` | `INT` |
| Boolean | `boolean` | `BIT` | `TINYINT(1)` |
| Timestamp | `timestamptz` | `DATETIME2` | `DATETIME` |

## 2. Migration / dependency ordering (topological — parents before children)

```
Wave 1 (no FK deps):   CatalogBrand · CatalogType · ApplicationUser · Role · Basket
Wave 2 (FK to Wave 1): CatalogItem (→Brand,→Type) · BasketItem (→Basket) · AspNetUserRoles (→User,→Role)
Wave 3 (FK to Wave 1): Order (root) 
Wave 4 (FK to Wave 3): OrderItem (→Order)
```

Owned types (Address, CatalogItemOrdered) require **no** wave — they are columns on Order/OrderItem.
Soft references (Basket.BuyerId, Order.BuyerId, BasketItem.CatalogItemId) create **no FK** — no ordering
constraint, enforced in application.

**Per-context split ordering:** generate `identity` schema first (BC-04, prerequisite — GR-01 priority 1),
then `catalog` (BC-01), `basket` (BC-02), `ordering` (BC-03).

## 3. Technology-neutral DDL (PostgreSQL shown; tokens swap per §1)

```sql
-- ============ Wave 1 ============
CREATE TABLE CatalogBrand (
    Id    integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    Brand varchar(100) NOT NULL,
    CONSTRAINT CK_CatalogBrand_Id CHECK (Id <> 0)          -- BR002
);

CREATE TABLE CatalogType (
    Id   integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    Type varchar(100) NOT NULL,
    CONSTRAINT CK_CatalogType_Id CHECK (Id <> 0)           -- BR003
);

CREATE TABLE ApplicationUser (              -- PII; INFERRED columns 🟦 verify vs IdentityDb
    Id                   varchar(450) PRIMARY KEY,
    UserName             varchar(256),
    NormalizedUserName   varchar(256),
    Email                varchar(256),
    NormalizedEmail      varchar(256),
    EmailConfirmed       boolean NOT NULL DEFAULT false,
    PasswordHash         text,
    SecurityStamp        text,
    ConcurrencyStamp     text,
    PhoneNumber          varchar(50),
    PhoneNumberConfirmed boolean NOT NULL DEFAULT false,
    TwoFactorEnabled     boolean NOT NULL DEFAULT false,
    LockoutEnd           timestamptz,
    LockoutEnabled       boolean NOT NULL DEFAULT true,
    AccessFailedCount    integer NOT NULL DEFAULT 0
);
CREATE UNIQUE INDEX UQ_ApplicationUser_NormalizedUserName ON ApplicationUser(NormalizedUserName);
CREATE UNIQUE INDEX UQ_ApplicationUser_NormalizedEmail    ON ApplicationUser(NormalizedEmail);

CREATE TABLE Role (
    Id               varchar(450) PRIMARY KEY,
    Name             varchar(256),
    NormalizedName   varchar(256),
    ConcurrencyStamp text
);
CREATE UNIQUE INDEX UQ_Role_NormalizedName ON Role(NormalizedName);

CREATE TABLE Basket (
    Id      integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    BuyerId varchar(256) NOT NULL          -- soft ref → ApplicationUser.Id (no DB FK, cross-DB)
);
CREATE INDEX IX_Basket_BuyerId ON Basket(BuyerId);

-- ============ Wave 2 ============
CREATE TABLE CatalogItem (
    Id                integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    Name              varchar(100) NOT NULL,
    Description       text,
    Price             numeric(18,2) NOT NULL,
    PictureUri        varchar(1000),
    CatalogTypeId     integer NOT NULL,
    CatalogBrandId    integer NOT NULL,
    CONSTRAINT FK_CatalogItem_CatalogBrand FOREIGN KEY (CatalogBrandId) REFERENCES CatalogBrand(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_CatalogItem_CatalogType  FOREIGN KEY (CatalogTypeId)  REFERENCES CatalogType(Id)  ON DELETE RESTRICT,
    CONSTRAINT CK_CatalogItem_Price CHECK (Price >= 0)                          -- BR001
);
CREATE INDEX IX_CatalogItem_CatalogBrandId ON CatalogItem(CatalogBrandId);
CREATE INDEX IX_CatalogItem_CatalogTypeId  ON CatalogItem(CatalogTypeId);

CREATE TABLE BasketItem (
    Id            integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    BasketId      integer NOT NULL,
    CatalogItemId integer NOT NULL,         -- soft ref (cross-context, no DB FK)
    UnitPrice     numeric(18,2) NOT NULL,
    Quantity      integer NOT NULL,
    CONSTRAINT FK_BasketItem_Basket FOREIGN KEY (BasketId) REFERENCES Basket(Id) ON DELETE CASCADE,
    CONSTRAINT CK_BasketItem_Quantity CHECK (Quantity >= 0)                    -- BR006/BR007
    -- , CONSTRAINT UQ_BasketItem_Basket_Catalog UNIQUE (BasketId, CatalogItemId)  -- BR005 🟦 enable if confirmed
);
CREATE INDEX IX_BasketItem_BasketId ON BasketItem(BasketId);

CREATE TABLE AspNetUserRoles (
    UserId varchar(450) NOT NULL,
    RoleId varchar(450) NOT NULL,
    CONSTRAINT PK_AspNetUserRoles PRIMARY KEY (UserId, RoleId),
    CONSTRAINT FK_AspNetUserRoles_User FOREIGN KEY (UserId) REFERENCES ApplicationUser(Id) ON DELETE CASCADE,
    CONSTRAINT FK_AspNetUserRoles_Role FOREIGN KEY (RoleId) REFERENCES Role(Id)            ON DELETE CASCADE
);

-- ============ Wave 3 ============
CREATE TABLE "Order" (                      -- PII; quote reserved word per dialect
    Id                     integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    BuyerId                varchar(256) NOT NULL,      -- soft ref (BR011), no DB FK
    OrderDate              timestamptz NOT NULL DEFAULT now(),
    ShipToAddress_Street   varchar(180) NOT NULL,      -- owned VO-01 (PII)
    ShipToAddress_City     varchar(100) NOT NULL,
    ShipToAddress_State    varchar(100),
    ShipToAddress_Country  varchar(100) NOT NULL,
    ShipToAddress_ZipCode  varchar(18)  NOT NULL
);
CREATE INDEX IX_Order_BuyerId ON "Order"(BuyerId);

-- ============ Wave 4 ============
CREATE TABLE OrderItem (
    Id                        integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    OrderId                   integer NOT NULL,
    ItemOrdered_CatalogItemId integer NOT NULL,        -- snapshot VO-03 (NO FK — DR-06)
    ItemOrdered_ProductName   varchar(100) NOT NULL,   -- BR009
    ItemOrdered_PictureUri    varchar(1000),
    UnitPrice                 numeric(18,2) NOT NULL,
    Units                     integer NOT NULL,
    CONSTRAINT FK_OrderItem_Order FOREIGN KEY (OrderId) REFERENCES "Order"(Id) ON DELETE CASCADE,
    CONSTRAINT CK_OrderItem_Units CHECK (Units >= 1)                           -- ASMP-DD-001
);
CREATE INDEX IX_OrderItem_OrderId ON OrderItem(OrderId);
```

## 4. Referential-integrity rules

| Rule | Enforcement |
|---|---|
| Intra-aggregate FKs (BasketItem→Basket, OrderItem→Order) | DB FK + `ON DELETE CASCADE` (aggregate lifecycle) |
| Reference-data FKs (CatalogItem→Brand/Type) | DB FK + `ON DELETE RESTRICT` (protect referenced data) |
| Identity join FKs | DB FK + CASCADE |
| **Soft references** (Basket/Order.BuyerId; BasketItem.CatalogItemId) | **Application-enforced ONLY** — no DB FK (cross-DB / cross-context). Generator MUST emit application-layer existence checks + document the invariant (DATA-REL-004/008/009) |
| **Ordered-item snapshot** (OrderItem.ItemOrdered_*) | **NO FK by design** — historical snapshot, must not change if CatalogItem changes (DR-06/BR009) |
| Empty-basket checkout block (BR012) | Application/domain invariant (cross-aggregate) — not a DB constraint |

## 5. Migration tooling note (⚠ neutral)

Use the chosen stack's migration framework (EF Core Migrations / Flyway / Liquibase / Alembic / Prisma
Migrate). Migrations MUST follow the Wave 1→4 order in §2 and the per-context split order in GR-01. Seed
data (catalog brands/types, `Administrators` role) runs **after** schema creation (BIZ-PROC-009/010 —
seeding processes; note: 0-step in evidence, content 🟦).

## 6. What this spec deliberately does NOT decide (🟦)

- Exact string lengths marked 🟦 in doc 02 (confirm vs source).
- Full Identity column set (verify vs IdentityDb).
- Stored-total column (default: none — derived).
- BR005 unique constraint (default: OFF).
- GUID vs integer PKs (default: integer).
- Per-context physical split vs single shared schema (default per DB-01: per-context).
