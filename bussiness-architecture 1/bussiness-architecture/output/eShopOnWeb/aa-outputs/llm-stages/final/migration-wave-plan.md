# Migration Wave Plan (Candidate)

> This plan sequences forward-engineering activities based on **evidence-backed risk and coupling**, not a chosen target architecture. Each wave should be re-validated against the full inventory before execution.

## Wave 0 — Clarify Open Questions (Pre-requisite, no code changes)

- Resolve ARCH-VIOL-002: confirm canonical Admin UI (`src/Web/Pages/Admin` vs `src/BlazorAdmin/Pages/CatalogItemPage`).
- Resolve ARCH-VIOL-001: confirm whether `CatalogItemOrdered` is an intentional snapshot/anti-corruption boundary.
- Obtain full module/component inventory and test/runtime evidence packs (not present in this stage's input context) to validate capability and data-ownership maps.
- **Evidence:** ARCH-VIOL-001, ARCH-VIOL-002 (architecture-violation-register.json)
- **Confidence:** 0.6 — this wave is procedural and low-risk regardless of eventual target architecture.

## Wave 1 — Admin Surface Consolidation

- Once Wave 0 confirms the canonical Admin UI, plan retirement/redesign of the non-canonical surface (`MCC-001`).
- Depends on usage evidence (not yet available) before any retirement — per global rules, nothing is marked retire without usage evidence.
- **Related:** ARCH-VIOL-002, CAP-005, MCC-001
- **Confidence:** 0.5

## Wave 2 — Formalize PublicApi ↔ BlazorAdmin Contract

- Treat `CatalogItemEndpoints` request/response shapes consumed by `CachedCatalogItemServiceDecorator` as a versioned, documented contract (see `api-contract-preservation-map.json`).
- This reduces coupling risk identified in ARCH-VIOL-003 ahead of any service-boundary changes.
- **Related:** ARCH-VIOL-003, CAP-007, CAP-008, MCC-003
- **Confidence:** 0.5

## Wave 3 — Domain Boundary Validation (ApplicationCore)

- Validate aggregate boundaries (`BasketAggregate`, `OrderAggregate`, `BuyerAggregate`, Catalog entities) against actual usage/test evidence (currently `unknown` — see `test-runtime-evidence-map.md`).
- Document the `CatalogItemOrdered` snapshot explicitly as a deliberate boundary artifact if confirmed in Wave 0.
- **Related:** ARCH-VIOL-001, CAP-001, CAP-002, CAP-003, CAP-004, MCC-002
- **Confidence:** 0.45 — depends heavily on evidence not available in this run.

## Wave 4 — Service Boundary Decision (Architect-Led)

- Using outputs of Waves 0–3 plus `service-boundary-options.md` and `data-ownership-map.md`, select a target boundary option (A/B/C or a hybrid).
- This wave is explicitly **not** pre-decided by this stage.
- **Confidence:** unknown — intentionally deferred to architects.

## Sequencing Rationale

Waves are ordered by **evidence dependency**: items with direct, file-level evidence (Admin duplication, API contract coupling) are sequenced before items that require additional evidence (domain boundary validation, service split) that was not available in this stage's input context.