# 07 — Implementation Guidelines & Constraints

> ⚠️ **DISC-001 (verified 2026-06-25):** Add to "Unsupported / forbidden patterns" (§3): generating
> `CatalogItem` stock fields or a reorder workflow — a **verified discrepancy** absent from the real
> `eShopOnWeb` source. See [`../EVIDENCE_VERIFICATION_REPORT.md`](../EVIDENCE_VERIFICATION_REPORT.md).

**Purpose:** The mandatory/optional/unsupported rules and the non-functional targets an AI generator must
honor. Bounds the generation so output is production-grade with minimal assumptions.
**Authority:** doc 14 (NFR), doc 15 (gates), docs 01–06 of this package.

---

## 1. Mandatory rules (release-blocking — derived from doc 15 [GATE]s + this package)

| # | Mandatory rule | Source |
|---|---|---|
| M-1 | Record the target stack before generating (else halt) | doc 01 §9 / GR-08 |
| M-2 | Generate only implemented scope (BC-01..05); SKIP aspirational | GR-05 |
| M-3 | No module cycle in output (DAG) | AR-03 / VR-03 |
| M-4 | No endpoint→repository shortcut | AR-04 |
| M-5 | Deny-by-default authZ; Administrators on catalog mutations; ownership on orders/basket | doc 05 §7 |
| M-6 | AuthN enforced on protected endpoints (closes TECH-SEC-010) | SR-03 / VR-05 |
| M-7 | No secrets in source/config/image; externalized + scanned | doc 04 §7 / SR-06 |
| M-8 | Physical schema per doc 02/03; soft refs app-enforced; snapshot has no FK | doc 02/03 |
| M-9 | Preserve 55-API contract surface | API-01 / VR-04 |
| M-10 | Enforce BR001–BR012 at correct layer; server-side total recompute | doc 06 §8 |
| M-11 | TLS everywhere; security headers; DB not host-published | doc 04 §8/§9/§10 |
| M-12 | Trace tags (graph node ids) on every generated unit | GR-04 / VR-12 |
| M-13 | Coverage gates (domain 80% / app 70%) | VR-10 |
| M-14 | SAST + dependency + container + secret scanning in CI | VR-09 |

## 2. Optional rules (recommended, not blocking)

| # | Optional | Default |
|---|---|---|
| O-1 | CQRS command/query split | application-level only ⚠ (no event-sourcing) |
| O-2 | Basket line-consolidation unique constraint (BR005) | OFF until confirmed |
| O-3 | Stored order Total column | OFF (derived) |
| O-4 | Caching decorators (legacy had CachedCatalog* — APP-SVC-044/045) | optional |
| O-5 | Microservices extraction | after Modular Monolith + cycle break |

## 3. Unsupported / forbidden patterns

| Pattern | Why forbidden |
|---|---|
| Generating Buyer/Payment persistence (BC-06) | aspirational/unimplemented (RC-002, SR-09) — no PCI surface |
| Generating reorder workflow / EVT-12 | inferred from attributes only; no process/rule (ASMP-FE-002) |
| FK on ordered-item snapshot | violates snapshot semantics (DR-06/BR009) |
| Hard DB FK on cross-DB BuyerId | cross-database; app-enforced only (DATA-REL-008/009) |
| Trusting client-supplied order total | BR010 — recompute server-side |
| `AllowedHosts=*` / `TrustServerCertificate=true` | TECH-SEC-015 |
| Inventing multi-currency | VO-05 amount-only; no currency in evidence (ASMP-FE-001) |
| Treating ROUTE/CLI synthetic verbs as REST contracts | OQ-009 |

## 4. Migration constraints

- Re-architect, don't reproduce: the cycle (APP-DEP-001) and 7 endpoint→repo violations must be fixed.
- CatalogContext (DATA-REPO-003) split per bounded context is a **data-migration project** (🟦 G-M9 plan required).
- Rotate the leaked credentials before any deployment (TECH-SEC-008/009).
- Azure SQL Edge is EOL — migrate to a supported engine (doc 01 §6 / NFR-AVL-005).

## 5. Performance targets (doc 14 — ⚠ DERIVED baselines, validate post-generation)

| Target | Value | Basis |
|---|---|---|
| Catalog read p95 / p99 | ≤ 300 ms / ≤ 800 ms | NFR-PERF-001 (industry baseline) |
| Write p95 / p99 | ≤ 700 ms / ≤ 1500 ms | ASMP-FE-001 |
| Cache TTL / timeouts / max body | per doc 14 | derived |

> These are **not measured** in legacy (G-M6). Treat as initial SLOs; re-baseline with real telemetry.

## 6. Scalability targets

| Target | Value | Source |
|---|---|---|
| Horizontal scale | ≥ 5 stateless instances per service | NFR-SCAL (doc 14) |
| Statelessness | no server-side session affinity (anonymous basket via token/session key) | doc 09 |
| Resource requests/limits | define CPU/memory (legacy had none — TECH-INF-004 gap) | NFR-27 |
| Data partitioning | per-context schemas (DB-01) | doc 02 §6 |

## 7. Availability targets

| Target | Value | Source |
|---|---|---|
| Service SLO | ≥ 99.9% | NFR-AVL-001 (industry baseline) |
| DB SLO | ≥ 99.95% | NFR-AVL |
| Liveness/readiness probes | required (legacy health routes unused — NFR-26) | IR-06 |
| Readiness gating | block traffic until dependencies healthy | doc 14 |

## 8. Observability requirements

| Requirement | Detail | Source |
|---|---|---|
| Structured logging | JSON, correlation ids, log levels | doc 14 NFR-09..13 |
| Metrics | RED/USE per service | NFR-OBS (recommended) |
| Tracing | distributed trace across BC boundaries | NFR-OBS |
| Security audit log | per doc 04 §6 | TECH-SEC-017 |
| Alerting | on SLO breach + security events | NFR-OBS |

## 9. Definition of Done (per generated bounded context)

- [ ] All entities/aggregates/VOs/events for the BC generated (status flags honored).
- [ ] Repositories per aggregate; soft refs app-enforced.
- [ ] All BC APIs preserved with correct authZ (doc 05 §7).
- [ ] BR invariants enforced + unit-tested; integration tests on real provider.
- [ ] Security controls applied (authN/Z, secrets, TLS, headers, audit).
- [ ] Trace tags present; coverage gates met.
- [ ] No cycle, no endpoint→repo shortcut (DAG verified).
- [ ] Human review at every doc-15 [GATE].
