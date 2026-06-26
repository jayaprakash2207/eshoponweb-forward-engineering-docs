# 03 — Traceability Audit

**Board:** TOGAF Reviewer + Application Architect + AI FE Specialist
**Date:** 2026-06-25
**Required chain:** Capability → Business Process → Use Case → Domain Entity → Application Service → API → Technology Component → Deployment Component
**Method:** Trace each hop against the knowledge graph cross_links (117) and the FE documents that render them. Report every broken/partial hop.

---

## 1. Hop-by-hop result

| # | Hop | Mechanism / source | Coverage | Status |
|---|---|---|---|---|
| 1 | Capability → Process | `capability_to_process` (17 links); doc 02/04 | 17/39 caps directly linked | ⚠ **PARTIAL** |
| 2 | Process → Use Case | doc 03 maps UCs to processes/capabilities | 10 processes → 21 UCs | ✅ COMPLETE |
| 3 | Use Case → Domain Entity | doc 03 §"Related entities"; `process_to_entity` (29) | 10/10 processes reach entities | ✅ COMPLETE |
| 4 | Entity → Application Service | `entity_to_service` (16 links) | 15/15 entities owned | ✅ COMPLETE |
| 5 | Service → API | `service_to_api` (55 links) — **binds API to physical host, not owning module** | 55/55 APIs host-bound | ⚠ **STRUCTURAL BREAK** |
| 6 | API → Technology Component | doc 11 + doc 12 (deployable_unit → container) | 55 APIs → Web/PublicApi containers | ✅ COMPLETE (via host) |
| 7 | Technology → Deployment Component | doc 12/18 (TECH-INF-001..008) | containers → compose services | ✅ COMPLETE (local); ❌ production undefined |

---

## 2. The two real breaks

### BREAK-1 (Hop 1) — Capability→Process is 44% direct
- **Evidence:** `TRACEABILITY_MATRIX §5.2` — 22 of 39 capabilities have no direct `capability_to_process` link.
- **Mitigated:** most are L1/L2 parents whose L3 children carry the link (rollup, `ASMP-RPT-002`).
- **Genuine residual gap:** `BIZ-CAP-003, -004, -005, -007, -008` (5 mid-level catalog capabilities) — no process path even through children.
- **Impact:** these 5 capabilities cannot be generated as first-class behaviors; doc 17 OQ-FE-012 asks whether they are admin/seeding sub-behaviors. **Not blocking** generation of the implemented scope, but a documented coverage hole.

### BREAK-2 (Hop 5) — Service→API binds to host, not domain module
- **Evidence:** `TRACEABILITY footnote [^svcapi]`, `§5.6`; graph `service_to_api` only has owning services `APP-SVC-006` (Web, 43), `APP-SVC-011` (PublicApi, 9), `APP-SVC-016` (BlazorAdmin, 3).
- **Consequence:** no chain *mechanically* runs `domain-module → API`; the entity-owning modules (Catalog/Identity/Basket/Order) have **zero** `service_to_api` links.
- **Resolved by design intent:** `DECISIONS.json ASMP-FE-004` distinguishes **functional ownership** (domain module) from **physical hosting** (Web/PublicApi shell). The FE docs (11, 15) re-establish the functional link narratively, so the *logical* chain is recoverable — but it is **not a single resolvable graph edge.**
- **Impact:** an AI generator must apply ASMP-FE-004 to re-attribute the 55 host-bound APIs to their functional contexts. Doc 15 GR-06 + ASMP-FE-154 handle this. **Recoverable, but it is the single most important traceability subtlety in the package.**

---

## 3. Representative full chains (functional, reconstructed)

Quoted/derived from `TRACEABILITY §6` + doc 03/11/12/18:

**Checkout:**
`BIZ-CAP-019 Checkout → BIZ-PROC-005 → UC-11/12/13/14 → DATA-ENT-006 Order (+004/007/012/013) → APP-SVC-004 Order (functional) → APP-API-035/036 (hosted by APP-SVC-006 Web) → TECH-INF-001 eshopwebmvc container → docker-compose (TECH-INF-004)`
— complete end-to-end **once ASMP-FE-004 functional re-attribution is applied**; deployment hop terminates at *local* compose (no production component).

**Catalog admin create:**
`BIZ-CAP-038 → BIZ-PROC-006 → UC-20 → DATA-ENT-001 → APP-SVC-001 Catalog (functional) → APP-API-005 (hosted by APP-SVC-011 PublicApi) → TECH-INF-002 eshoppublicapi → compose`
— complete; **but APP-API-005 carries `auth=not noted`** (TECH-SEC-010) — a security trace gap, gated VR-05.

**Authentication:**
`BIZ-CAP-031/032 → BIZ-PROC-007 → UC-16 → DATA-ENT-008/009 → APP-SVC-002 Identity → APP-API-001 (PublicApi) → TECH-INF-002 → compose`
— complete.

---

## 4. Deployment-hop (Hop 7) finding

The chain reaches a **deployment component only at the local/Docker level** (TECH-INF-001..004). **There is no production deployment component** — no IaC resource template, no K8s manifest, no release pipeline (doc 18 §18.5/§18.7; TECH-INF-007 azd is parameters-only, LOW). So Hop 7 is:
- ✅ COMPLETE for local/dev (compose services exist with HIGH confidence)
- ❌ ABSENT for production (must be authored — doc 18 §18.6 neutral options)

---

## 5. Traceability scorecard

| Hop | Coverage | Score |
|---|---|---|
| Capability → Process | 44% direct (rollup mitigated; 5 genuine gaps) | 70 |
| Process → Use Case | 100% | 95 |
| Use Case → Entity | 100% | 95 |
| Entity → Service | 100% | 95 |
| Service → API | 100% host-bound; functional via ASMP-FE-004 | 75 |
| API → Technology | 100% | 90 |
| Technology → Deployment | local 100%, production 0% | 60 |

**Traceability composite: 78 / 100** (matches doc 17 R3 = 78, independently corroborated).

**Verdict:** The end-to-end chain is **substantially traceable and recoverable for the implemented scope**, with two well-documented structural characteristics (capability→process rollup; functional-vs-physical API binding) and one real terminal gap (no production deployment component). No *broken/dangling* reference exists — the gaps are coverage and design-attribution, fully explained in the source.
