# 04 — Governance Compliance Report

**Pilot:** eShopOnWeb · **Date:** 2026-06-24
**Validates the resolved pipeline against:** GOV-01, GOV-02, GOV-03, GOV-04, GOV-07, GOV-08.
**Evidence base:** `../FINAL_PROMPT_CONFORMANCE_REPORT.md` (98/100, all PASS), `../prompt-resolved/INCLUDE_RESOLUTION_REPORT.md` (0 unresolved), and direct checks on `prompt-resolved/`.

---

## 1. GOV-01 — Global Rules (single source of truth)

| Check | Result |
|---|---|
| Every prompt references GOV-01 (CMP-GOV resolved inline) | ✅ 22/22 (1 governance block each) |
| Zero inline duplicated rules (exclusion list, anti-hallucination, evidence hierarchy) | ✅ 0 duplicates (collapsed from 5/~20/2) |
| No unresolved `{{include}}` reaching the model | ✅ 0 across 22 resolved prompts |

**GOV-01: COMPLIANT (100%).**

## 2. GOV-02 — Ownership Matrix

| Responsibility | Sole owner (resolved) | Legacy duplicate producers removed | Status |
|---|---|---|---|
| Data-store / entity / schema | DA | layer2, BA-P1, TA-P8, layer1 | ✅ |
| Business rules (semantic) | BA | layer2, DA hidden-rules, layer1 | ✅ |
| Tech-stack | TA | layer1, AA inventory | ✅ |
| Components | AA | TA, P9 | ✅ |
| Capability map | BA | AA Stage 05 | ✅ |
| Data-ownership map | DA | AA Stage 05 | ✅ |
| Data-store transaction/consistency | DA | TA OUTPUT 4 | ✅ |
| App/data-level security | AA | TA OUTPUT 5 | ✅ |

**Ownership violations: 0. Duplicate-extraction tasks: 0** (5→1, 4→1, 3→1, 3→1, 2→1 all collapsed).
**GOV-02: COMPLIANT (100%).**

## 3. GOV-03 — Prompt Standard

| Check | Result |
|---|---|
| 12 canonical sections, in order | ✅ 22/22 |
| Metadata complete (prompt_id, version, owner_layer, role, model_pin, consumes, produces, …) | ✅ 22/22 |
| Version + changelog (§12) | ✅ 22/22 |
| Carried from `FINAL_PROMPT_CONFORMANCE_REPORT` (98/100) | ✅ structure unchanged by resolution |

**GOV-03: COMPLIANT (100% structural; carries the 98/100 quality score with 3 non-blocking clarity warnings).**

## 4. GOV-04 — Confidence Model

| Check | Result |
|---|---|
| CMP-CONF (5-label model) resolved in every prompt | ✅ 1 block/prompt |
| No local numeric/categorical scale | ✅ 0 legacy-scale leaks detected |
| Verdicts use PASS/PARTIAL/FAIL | ✅ Review prompts |
| Legacy numeric (1.0…0.70…UNKNOWN) mapped to labels (band derived) | ✅ no drift |

**Confidence drift: 0. GOV-04: COMPLIANT (100%).**

## 5. GOV-07 — Dependency Model

| Check | Result |
|---|---|
| DAG terminates at Foundation | ✅ BA/DA/AA/TA → FN-SYNTH-01 → FN-SYNTH-02 → FN-REVIEW-01 |
| No extraction layer consumes FN output | ✅ |
| Cross-layer reads = consume-and-cite under contracts C-1…C-6 | ✅ (e.g. AA-ANALYST-05 cites BA/DA owner IDs) |
| No forbidden edges / cycles | ✅ |
| No unresolved references | ✅ 0 |

**GOV-07: COMPLIANT (100%).**

## 6. GOV-08 — Layer Boundaries

| Boundary | Result |
|---|---|
| BA produces nothing in its Must-Not list (no schema/DDL) | ✅ |
| DA produces nothing in its Must-Not list | ✅ |
| AA does not author capability-map or data-ownership-map | ✅ (relocated; consumed) |
| TA does not produce data-arch assessment or app-level security | ✅ (relocated) |
| FN performs no primary extraction (FN-1) | ✅ |

**Boundary violations: 0. GOV-08: COMPLIANT (100%).**

## 7. Compliance roll-up

| Standard | Score |
|---|---|
| GOV-01 | 100% |
| GOV-02 | 100% |
| GOV-03 | 100% (structural) |
| GOV-04 | 100% |
| GOV-07 | 100% |
| GOV-08 | 100% |
| **Governance Compliance** | **100%** |

| Success-criterion gate | Required | Actual | Met? |
|---|---|---|---|
| Governance Compliance | 100% | 100% | ✅ |
| Ownership Violations | 0 | 0 | ✅ |
| Boundary Violations | 0 | 0 | ✅ |
| Unresolved References | 0 | 0 | ✅ |
| Knowledge Graph Integrity | PASS | PASS | ✅ |

**Governance Compliance: 100% — all six standards COMPLIANT; all hard governance gates met.**
