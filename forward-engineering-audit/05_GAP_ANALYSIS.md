# 05 — Gap Analysis

**Board:** All members
**Date:** 2026-06-25
**Definition:** A gap = information an AI generator needs that is absent, inferred, or unresolved in the package. Each gap is evidence-cited and severity-rated. **Gaps are reported, never filled by invention.**

Severity: 🔴 Critical (blocks generation/release) · 🟠 Major (blocks a stack or a layer) · 🟡 Minor (degrades quality, workaround exists).

---

## 1. Critical gaps (🔴) — must resolve before generation/release

| ID | Gap | Evidence | Why critical |
|---|---|---|---|
| **G-C1** | **No target stack chosen** (target_stack = 0 nodes; all targets neutral) | doc 12 §1; doc 15 GR-08; graph empty_target_stack=true | Generation cannot begin; GR-08 halts the agent until a human selects backend+frontend+DB+runtime |
| **G-C2** | **Physical data model absent** — no column types, nullability, lengths, defaults, indexes-as-DDL, referential actions | doc 06 line 5; doc 07 §4.1/§4.3 | DDL **cannot be generated** from the logical model; every target RDBMS needs concrete types |
| **G-C3** | **2 CRITICAL security findings open** — hardcoded credentials in source/compose | TECH-SEC-008/009; doc 13 §13.7 | Must be removed/rotated/externalized before any non-dev deployment; gated SR-06/VR-09 |
| **G-C4** | **Auth enforcement unverified** on 3 catalog mutation APIs (`auth=not noted`) | APP-API-005/006/007; TECH-SEC-010/011; OQ-005 | Release gate VR-05; generating without enforcement reproduces the vulnerability |

## 2. Major gaps (🟠) — block a stack, layer, or correctness guarantee

| ID | Gap | Evidence | Impact |
|---|---|---|---|
| **G-M1** | **Module dependency cycle reality unknown** (real runtime vs static artifact) | APP-DEP-001; OQ-004 | AR-03/VR-03 enforce DAG regardless, but risk/effort planning needs the answer |
| **G-M2** | **19 of 26 dependency versions undeclared** (LOW) | doc 12 §2.2; FND-06 | Cannot pin dependencies (ASMP-FE-016); no asserted upgrade/CVE baseline |
| **G-M3** | **3 aspirational entities with empty attribute sets** (Buyer, PaymentMethod, CatalogItemDetails) | DATA-ENT-010/011/014; docs 05/06 | Cannot generate persistence; correctly gated SKIP (GR-05) — a gap only if activation is desired |
| **G-M4** | **Identity schema inferred (0.7)** — ~10 standard ASP.NET Identity columns unknown | DATA-ENT-008/009; ASMP-DD-003 | Generated Identity may miss lockout/2FA/security-stamp/concurrency columns |
| **G-M5** | **40 UI/page-route DTO shapes inferred**, not from evidence | doc 11 §11.4; ASMP-FE-105 | Request/response models must be verified against source before implementation |
| **G-M6** | **NFRs are DERIVED, not measured** — no real latency/throughput/availability baselines | doc 14; ASMP-FE-001..019 | Can scaffold but cannot validate performance/SLA acceptance |
| **G-M7** | **No production deployment definition** — no IaC/K8s/release automation | doc 18 §18.5/§18.7; TECH-INF-007 azd params-only | Hop-7 production deployment component absent; must be authored |
| **G-M8** | **5 mid-level capabilities lack process path** (BIZ-CAP-003/004/005/007/008) | TRACEABILITY §5.2; OQ-FE-012 | Cannot generate as first-class behaviors; classify as admin/seeding sub-behaviors |
| **G-M9** | **Shared CatalogContext split is a data-migration project** (spans BC-01/02/03) | RISK-SHARED-DBCTX-001; doc 15 DB-01/MR-02 | Per-context DB split scope unspecified — significant effort |
| **G-M10** | **No at-rest encryption / audit-logging evidence** for PII | TECH-SEC-017; doc 13; doc 14 | Compliance gap; must be added as new NFR/control |

## 3. Minor gaps (🟡) — quality, workaround exists

| ID | Gap | Evidence |
|---|---|---|
| G-m1 | 3 zero-step processes (BIZ-PROC-008/009/010) — trigger→outcome only | doc 04; ASMP-FE-101..103 |
| G-m2 | Order total derived, not stored (no column / maintenance rule) | doc 07 BR010 |
| G-m3 | Cross-DB soft refs (Basket/Order→ApplicationUser) unenforced at DB | DATA-REL-008/009 |
| G-m4 | Anonymous-basket session-key mechanism unspecified | doc 09 ASMP-FE-005 |
| G-m5 | 8 synthetic ROUTE/CLI API verbs (method=unknown) | OQ-009; APP-API-009/010/011/039/040/053/054/055 |
| G-m6 | Email/notification flow (IEmailSender) has no implementer | doc 09; APP-IF-008 |
| G-m7 | OQ-001 Admin vs BlazorAdmin merge unresolved (affects BC-05 packaging) | OQ-001 |
| G-m8 | Azure SQL Edge EOL / untagged image | TECH-CUR-020; NFR-AVL-005 |
| G-m9 | Multi-currency not derivable (Money VO amount-only) | VO-05; ASMP-FE-001 |
| G-m10 | EVT-12 reorder event inferred from attributes only | ASMP-FE-002 |

---

## 4. Gap heat map by layer

| Layer | Critical | Major | Minor | Net readiness |
|---|---|---|---|---|
| Business | 0 | 1 (G-M8) | 4 | High |
| Data | 1 (G-C2) | 3 (G-M3/M4/M9) | 3 | **Medium — physical model blocks DDL** |
| Application | 1 (G-C4) | 1 (G-M5) | 2 | Medium-High |
| Technology | 1 (G-C3) | 4 (G-M2/M6/M7/M10) | 2 | **Medium — versions, NFRs, prod deploy** |
| Foundation/process | 1 (G-C1) | 1 (G-M1) | 1 | High (governance strong) |

---

## 5. What is NOT a gap (explicitly cleared)

- **Generation manifest** — present, valid, graph-consistent (NOT missing, despite one reviewer's partial-set flag).
- **Empty target stack** — by design/mandate, not an omission.
- **Aspirational skips** — correctly gated (GR-05/VR-07); skipping them is correct behavior, not a defect.
- **Count integrity** — 274 nodes verified; zero count gaps.

---

## 6. Gap summary

**4 Critical, 10 Major, 10 Minor.** The Critical gaps are the classic FE pre-conditions: **stack choice (G-C1), physical data model (G-C2), credential remediation (G-C3), auth verification (G-C4).** None indicates a *defective* package — all are either human-decision points or evidence-bounded unknowns that the package **correctly surfaces rather than fabricates.** Remediation paths are specified in the package itself (doc 15 gates, doc 17 §11 recommendations).
