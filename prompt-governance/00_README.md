# Prompt Governance Package

**Project:** Forward-/Reverse-Engineering Enterprise Pipeline (`frwd engg - op's`)
**Package type:** Enterprise Prompt Architecture — Governance, Standards & Migration
**Created:** 2026-06-24
**Authority:** Derived from and bound by [`../PROMPT_AUDIT_REPORT.md`](../PROMPT_AUDIT_REPORT.md) (authoritative)
**Status:** Design artifacts only. **No source prompts modified. No code generated.**

---

## Purpose

This package refactors the pipeline's prompt architecture into a standardized, TOGAF-aligned,
AI-governance-aligned enterprise architecture. It eliminates the defects the audit confirmed —
duplication, cross-layer ownership violations, governance fragmentation, confidence-scheme
inconsistency, and repeated extraction logic — **without changing functional behavior**.

These documents are **specifications and plans**. They do not rewrite the prompt files; they define the
target state and the controlled migration toward it (see `06_PROMPT_REFACTORING_PLAN.md`).

## Authoritative inputs

| Input | Role in this package |
|---|---|
| [`../PROMPT_AUDIT_REPORT.md`](../PROMPT_AUDIT_REPORT.md) | **Authoritative findings.** All defects treated as factual. |
| `ENTERPRISE_KNOWLEDGE_GRAPH.json` | Canonical model schema the Foundation layer must produce. |
| `CANONICAL_ENTERPRISE_MODEL.md` | Human-readable canonical model (Foundation output reference). |
| `TRACEABILITY_MATRIX.md` | Capability→Process→Entity→Service→API traceability target. |
| `ARCHITECTURE_INVENTORY.md` | Flat node inventory (Foundation output reference). |
| `FORWARD_ENGINEERING_INPUT_MAP.md` | Confirms graph sections: `metadata, business, data, application, technology, cross_links, assumptions, normalization_log, open_questions`. |

## Document index

| # | Document | Addresses audit findings |
|---|---|---|
| 01 | [`01_GLOBAL_PROMPT_RULES.md`](01_GLOBAL_PROMPT_RULES.md) | F4 (governance duplication), F6 |
| 02 | [`02_PROMPT_OWNERSHIP_MATRIX.md`](02_PROMPT_OWNERSHIP_MATRIX.md) | F2, F3 (ownership), finding 7 |
| 03 | [`03_PROMPT_STANDARD.md`](03_PROMPT_STANDARD.md) | F1 (paradigms), finding 9 (metadata) |
| 04 | [`04_CONFIDENCE_STANDARD.md`](04_CONFIDENCE_STANDARD.md) | F1/§3.2 (3 confidence schemes) |
| 05 | [`05_FOUNDATION_LAYER_SPECIFICATION.md`](05_FOUNDATION_LAYER_SPECIFICATION.md) | finding 10 (no synthesis layer) |
| 06 | [`06_PROMPT_REFACTORING_PLAN.md`](06_PROMPT_REFACTORING_PLAN.md) | all findings (migration) |
| 07 | [`07_PROMPT_DEPENDENCY_MODEL.md`](07_PROMPT_DEPENDENCY_MODEL.md) | §5 (dependencies) |
| 08 | [`08_LAYER_BOUNDARY_SPECIFICATION.md`](08_LAYER_BOUNDARY_SPECIFICATION.md) | §6 (boundary validation) |
| 09 | [`09_REUSABLE_PROMPT_COMPONENTS.md`](09_REUSABLE_PROMPT_COMPONENTS.md) | §8 (reusability) |
| 10 | [`10_PROMPT_ARCHITECTURE_TARGET_STATE.md`](10_PROMPT_ARCHITECTURE_TARGET_STATE.md) | end-state + readiness |

## Reading order

Governance first (01–04), then the new layer and structure (05, 07, 08), then reusable parts (09),
then the migration and end-state (06, 10).

## Refactoring rules honored by this package

- ❌ No code generated.
- ❌ No source prompts modified or rewritten.
- ✅ Only architecture, governance, standards, and migration plans produced.
- ✅ Audit report treated as authoritative.
- ✅ Targets: TOGAF-aligned · Enterprise-Architecture-aligned · AI-Governance-aligned · Forward-Engineering-ready.
