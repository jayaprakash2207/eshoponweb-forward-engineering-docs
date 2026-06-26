# 05 — Authorization Model (closes C4)

**Closes:** Audit C4 — "Authorization verification." Defines the complete RBAC model: actors, roles,
permissions, resources, operations, and the API/business-rule authorization matrices.
**Authority:** graph `business.actors` (5), `Administrators` role (DATA-ENT-009, RC-008), 55 APIs
(`application.apis`), 12 business rules (BR001–BR012), doc 13/15 SR-02.
**Nature:** Authorization specification (no code). Where the legacy role model is silent, items are
🟦 REQUIRES HUMAN DECISION — not invented.

---

## 1. Actors (DERIVED — `business.actors`)

| Actor | Node | Type | Authn |
|---|---|---|---|
| Customer / Buyer | BIZ-ACT-001 | human | authenticated |
| Anonymous Shopper | BIZ-ACT-002 | human | unauthenticated (inferred †) |
| Administrator | BIZ-ACT-003 | human | authenticated + `Administrators` role |
| System / Service Account | BIZ-ACT-004 | system | service credentials (seeding) |
| Notification Recipients | BIZ-ACT-005 | external | n/a (not yet implemented) |

## 2. Roles

| Role | Status | Source | Maps to actor |
|---|---|---|---|
| **Administrators** | ✅ DERIVED (confirmed, RC-008) | DATA-ENT-009 seed | BIZ-ACT-003 |
| **Customer** | ⚠ NEUTRAL DEFAULT (authenticated tier) | inferred from BIZ-ACT-001 + user-facing routes | BIZ-ACT-001 |
| **Anonymous** | ⚠ implicit (no role; unauthenticated) | BIZ-ACT-002 † | BIZ-ACT-002 |
| **ServiceAccount** | ⚠ NEUTRAL DEFAULT (client-credentials) | BIZ-ACT-004 seeding | BIZ-ACT-004 |

> 🟦 **S5 decision (from doc 04):** is `Customer` an **explicit role** or simply "any authenticated user"?
> Only `Administrators` is evidenced. Default: treat Customer as the authenticated tier (no explicit role
> row), Administrators as the sole explicit role. Recorded as a decision, not asserted as legacy fact.

## 3. Role hierarchy

```
Administrators   (full catalog admin + all Customer capabilities)
      │  inherits
   Customer       (authenticated: own basket, own orders, account mgmt)
      │  inherits
  Anonymous       (browse catalog, anonymous basket only)

ServiceAccount    (separate principal: seeding only — not in the user hierarchy)
```

## 4. Resources & operations

| Resource | Operations | Owning BC |
|---|---|---|
| Catalog item | read, create, update, delete | BC-01 |
| Catalog brand/type | read (+ admin maintenance) | BC-01 |
| Basket (own) | read, add-item, adjust, checkout | BC-02 |
| Order (own) | place, read-own, read-detail-own | BC-03 |
| Account / identity (own) | login, logout, manage profile, 2FA, password | BC-04 |
| Admin catalog UI | list, create, delete, cache-refresh | BC-05 |

## 5. Permission catalog

| Permission | Granted to | Notes |
|---|---|---|
| `catalog:read` | Anonymous, Customer, Administrators | public read |
| `catalog:write` (create/update/delete) | **Administrators only** | closes TECH-SEC-010 on APP-API-005/006/007 |
| `brandtype:read` | all | reference data |
| `brandtype:manage` | Administrators 🟦 (no CRUD API in evidence — ASMP-FE-011) | only if added |
| `basket:own:*` | Customer, Anonymous (session) | row-level: BuyerId/session match |
| `order:place` | Customer | BR011 requires buyer id |
| `order:read:own` | Customer (own), Administrators (all 🟦) | row-level ownership |
| `account:own:*` | Customer | own profile only |
| `admin:catalog:*` | Administrators | BC-05 admin SPA |
| `seed:execute` | ServiceAccount | BIZ-PROC-009/010 |

## 6. Permission matrix (role × permission)

| Permission | Anonymous | Customer | Administrators | ServiceAccount |
|---|:--:|:--:|:--:|:--:|
| catalog:read | ✅ | ✅ | ✅ | — |
| catalog:write | ❌ | ❌ | ✅ | — |
| brandtype:read | ✅ | ✅ | ✅ | — |
| basket:own | ✅ (session) | ✅ | ✅ | — |
| order:place | ❌ | ✅ | ✅ | — |
| order:read:own | ❌ | ✅ (own) | ✅ (all 🟦) | — |
| account:own | ❌ | ✅ | ✅ | — |
| admin:catalog | ❌ | ❌ | ✅ | — |
| seed:execute | ❌ | ❌ | ❌ | ✅ |

## 7. API Authorization Matrix (all 55 APIs)

Mapped from `service_to_api` + auth notes + SR-02. **Catalog mutations are the audit's key C4 gap.**

| API(s) | Method/Path | Required authorization | Evidence |
|---|---|---|---|
| APP-API-001 | POST /api/authenticate | **anonymous** (issues JWT) | DERIVED (issues JWT) |
| APP-API-002/003/004/008 | GET /api/catalog-* | **anonymous** (public read) | auth=not noted → read-only OK |
| **APP-API-005** | POST /api/catalog-items | **Administrators** | TECH-SEC-010 gap → enforce (VR-05) |
| **APP-API-006** | DELETE /api/catalog-items/{id} | **Administrators** | TECH-SEC-010 gap → enforce |
| **APP-API-007** | PUT /api/catalog-items | **Administrators** | TECH-SEC-010 gap → enforce |
| APP-API-012/013 | GET health checks | **anonymous** | infra |
| APP-API-014..034 | /Manage/* | **Customer (authenticated, own)** | user_facing identity |
| APP-API-035/036 | /Order/MyOrders, /Order/Detail/{id} | **Customer (own) + ownership check** | user_facing |
| APP-API-037/038 | /User, /User/Logout | **Customer (authenticated)** | user_facing identity |
| APP-API-039/040 | /logout, /admin (BlazorAdmin) | **Administrators** (admin SPA) | user_facing admin |
| APP-API-041..044 | /Account/* (login/register/confirm) | **anonymous** (pre-auth flows) | user_facing identity |
| APP-API-045/046/047 | /Error, /, /Privacy | **anonymous** | public |
| APP-API-048/049 | /Admin/* (edit catalog) | **Administrators** | user_facing admin |
| APP-API-050/051/052 | /Basket/*, /Basket/Checkout | **Customer or Anonymous (session)**; checkout needs buyer id | BR011/BR012 |
| APP-API-009/010/011/053/054/055 | synthetic ROUTE/CLI | **n/a** (host/bootstrap, not REST) | OQ-009 |

**Coverage: all 55 APIs assigned an authorization rule.** The 3 catalog mutations and the 5 admin routes
are the privilege-escalation surface — all now require `Administrators` (closes C4 / TECH-SEC-010).

## 8. Business-rule authorization (BR → authz)

| BR | Rule | Authorization implication |
|---|---|---|
| BR011 | Order requires buyer id | order:place requires authenticated Customer with `sub`→BuyerId |
| BR012 | Block empty-basket checkout | enforced after authn, before order creation |
| BR005/006/007 | Basket quantity rules | basket:own — actor may only mutate own/session basket |
| BR001..004 | Catalog item/brand/type validation | catalog:write (Administrators) — validation server-side |
| BR008 | Buyer creation needs valid identity | aspirational (BC-06) — SR-09 no payment surface |

## 9. Row-level (ownership) authorization

Because BuyerId is a **soft cross-DB reference** (DATA-REL-008/009), ownership **must be enforced in the
application**, not the DB:

| Resource | Ownership rule |
|---|---|
| Basket | `Basket.BuyerId == token.sub` (or anonymous session key) |
| Order | `Order.BuyerId == token.sub` (Customer); Administrators may read all 🟦 |
| Account/Manage | operate only on `token.sub` user |

## 10. Administrative permissions

`Administrators` (RC-008) may: create/update/delete catalog items (APP-API-005/006/007), access the admin
SPA (APP-API-039/040/048/049), trigger cache refresh (BIZ-CAP-039). Admin actions are **audit-logged**
(doc 04 §6). 🟦 Fine-grained admin sub-roles (e.g. CatalogManager vs SuperAdmin) are **not in evidence** —
single `Administrators` role unless an owner decides to split it.

## 11. 🟦 Authorization decisions requiring human input

| # | Decision | Default |
|---|---|---|
| A1 | Is `Customer` an explicit role or implicit-authenticated? | implicit-authenticated ⚠ |
| A2 | May Administrators read **all** orders? | yes ⚠ (typical), confirm |
| A3 | Split `Administrators` into finer admin roles? | no — single role (evidence) |
| A4 | brand/type CRUD permissions (no API in evidence) | none until API added (ASMP-FE-011) |
| A5 | Anonymous basket session-key authz mechanism | session/cookie key (doc 09 ASMP-FE-005) |
