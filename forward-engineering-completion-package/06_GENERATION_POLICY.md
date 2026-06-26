# 06 — Generation Policy

> ⚠️ **DISC-001 (verified 2026-06-25):** Add to the status-flag policy (§12): SKIP the `CatalogItem` stock
> fields and the reorder event/capability (EVT-12) — a **verified discrepancy** absent from the real
> `eShopOnWeb` source. See [`../EVIDENCE_VERIFICATION_REPORT.md`](../EVIDENCE_VERIFICATION_REPORT.md).

**Purpose:** Consolidate the generation rules an AI agent must follow, binding the new decisions (docs
01–05) to the existing master spec (doc 15, 89 rules / 68 gates). This **does not regenerate** doc 15 — it
references it and adds the policy hooks the four remediations introduce.
**Authority:** doc 15 (FE Specification), docs 01–05 of this package.

---

## 1. Architecture rules (ref doc 15 AR-01..08)

- Onion/Clean layering: Domain → Application → Adapters → Host; dependencies inward only (AR-01/02).
- **DAG [GATE]:** the legacy cycle APP-DEP-001 MUST NOT be reproduced (AR-03 / VR-03).
- **No endpoint→repository shortcut [GATE]:** handler → application service → repository port (AR-04).
- Bounded-context isolation; cross-context via API/events only (AR-06).
- **Target style:** Modular Monolith first (doc 01 §7); microservices extraction BC-04→BC-03 only after cycle + CatalogContext split.

## 2. Coding rules

- Language/framework per the recorded stack decision (doc 01 §9); **halt if unset (GR-08)**.
- Domain layer is framework-free (pure model + invariants).
- Every generated unit carries graph node-id trace tags (GR-04 / VR-12).

## 3. DDD rules (ref doc 15 DR-01..08)

- 4 aggregates: Basket (DATA-AGG-001), Order (DATA-AGG-002), CatalogItem (DATA-AGG-004); Buyer (DATA-AGG-003) **SKIP — aspirational**.
- One repository per aggregate root (DR-02); no generic god-repo.
- Aggregate invariants enforced in domain (BR005/006/007 basket; BR009/010/011/012 order).
- Value objects immutable (VO-01..06); Money is amount-only (VO-05; multi-currency = new design).
- Cross-aggregate references **by id only**; ordered-item is a **snapshot copy** (DR-06).
- Domain events EVT-01..10 emitted; **EVT-11 (Buyer) and EVT-12 (reorder) NOT generated** (aspirational/weakest).

## 4. Clean Architecture rules

- Use-case (application) services orchestrate; no business logic in controllers/endpoints.
- Persistence behind ports; ORM is an adapter detail.
- DTOs at the boundary; domain entities never leave the application layer directly.

## 5. CQRS rules (⚠ neutral — optional)

- 🟦 CQRS is **optional**, not mandated by evidence. Legacy uses MediatR (TECH-CUR-011) for some flows.
  Default ⚠: command/query separation at the application-service level **without** separate read/write
  stores. Full CQRS+event-sourcing is a human decision (over-engineering risk for this scope).

## 6. Repository rules

- One repository interface per aggregate (DR-02); specialize the generic `IRepository<T>`/`IReadRepository<T>`.
- Soft references (BuyerId, CatalogItemId) resolved **in application services**, not via DB FK (doc 02/03).
- Per-context persistence (DB-01): catalog/basket/ordering/identity schemas.

## 7. API standards (ref doc 15 API-01..10)

- **Preserve the 55-API contract surface** (API-01 [GATE]); 8 REST endpoints have full contracts (doc 11).
- Authorization per `05_AUTHORIZATION_MODEL.md` §7 (deny-by-default; Administrators on catalog mutations).
- RFC 9457 problem-detail error envelope (API-04); pagination on collections (API-06).
- 🟦 Verify the 40 UI-route DTO shapes against source before implementing (ASMP-FE-105).

## 8. Validation rules

- Enforce BR001–BR012 at the correct layer (domain invariants for aggregate rules; input validation at boundary).
- Server-side recompute of order total (BR010) — never trust client total.
- Reject on violation with problem-detail; no silent coercion.

## 9. Testing rules (ref doc 15 TR-01..07)

- Unit-test aggregate invariants (BR005..012); integration-test against a **real** provider (not InMemory).
- Contract-test all preserve APIs; E2E for BIZ-PROC-002/003/005/006/007; saga test for checkout.
- Coverage gates: domain ≥80%, application ≥70% (VR-10).

## 10. Logging rules

- Structured logging; security audit events per `04_SECURITY_MODERNIZATION.md` §6.
- No PII (Order/User/Address) or secrets in logs; correlation ids on every request.

## 11. Deployment rules (ref doc 18 + doc 15 IR-01..07)

- Containerized, 12-factor, externalized config + secrets (IR-02/03/04).
- DB not published to host (IR-05 / TECH-SEC-013); health probes (IR-06).
- 🟦 Author production IaC/K8s (gap G-M7); CI gates: SAST + dependency + container + secret scan (VR-09).

## 12. Status-flag policy (GR-05 — non-negotiable)

- **SKIP aspirational:** BC-06, DATA-ENT-010/011/014, DATA-AGG-003, EVT-11, reorder/EVT-12.
- **No payment/PCI surface** (SR-09) — Buyer/PaymentMethod unimplemented.
- Honor every confidence flag; never elevate inferred→implemented.
