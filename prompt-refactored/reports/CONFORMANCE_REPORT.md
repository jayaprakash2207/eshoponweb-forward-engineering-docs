# Conformance Report (GOV-03 / GOV-01 / GOV-04)

**Package:** `prompt-refactored/`
**Date:** 2026-06-24
**Gate basis:** GOV-03 §4 conformance checklist.

---

## 1. Checklist applied to every prompt

C1. All 12 GOV-03 sections present, in order.
C2. Metadata complete; `owner_layer` matches every `produces` owner (GOV-02).
C3. No GOV-01 rule restated/paraphrased; only narrower constraints added.
C4. Confidence section references GOV-04 only (no third scheme).
C5. Every output mapped to ≥1 validation rule + a traceability rule.
C6. `model_pin: required` declared (GR-10).
C7. Version + changelog present.
C8. No source-modification or secret-writing capability granted (GR-5).

Legend: ✅ pass · — n/a.

---

## 2. Results

| prompt_id | C1 | C2 | C3 | C4 | C5 | C6 | C7 | C8 | Verdict |
|---|:--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|:--:|
| BA-SCOUT-01 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| BA-ANALYST-01 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| BA-ANALYST-02 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| BA-ANALYST-03 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| DA-SCOUT-01 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| DA-ANALYST-01 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| DA-REVIEW-01 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| TA-SCOUT-01 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| TA-ANALYST-01 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| AA-ANALYST-00 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| AA-SCOUT-01 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| AA-SCOUT-02 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| AA-ANALYST-03 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| AA-ANALYST-04 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| AA-ANALYST-05 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| AA-ANALYST-06 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| AA-REVIEW-06 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| AA-REVIEW-07 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| FN-SYNTH-01 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| FN-SYNTH-02 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| FN-REVIEW-01 | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | **PASS** |
| AGENTS.md (orchestrator) | — | ✅ | ✅ | — | — | — | ✅ | ✅ | **PASS** (orchestrator: §4–10 n/a) |

**22 prompts + 1 orchestrator: all PASS.**

---

## 3. Governance externalization check (C3 detail)

| Component | Included by | Inline duplication remaining |
|---|---|---|
| CMP-GOV → GOV-01 | every prompt + orchestrator | none |
| CMP-CONF → GOV-04 | every analyst/scout/review/synth prompt | none |
| CMP-VALID → GR-7 | every prompt | none |
| CMP-EVID → GR-2/3 | every prompt | none |
| CMP-OUT → GR-8 | every prompt | none |

- Exclusion list: referenced via GR-4 — **0** inline copies (was 5).
- Anti-hallucination: referenced via GR-1 — **0** inline copies (was ~20).
- Evidence hierarchy: referenced via CMP-EVID — **0** inline copies (was 2 verbatim).
- Confidence scales: GOV-04 only — **0** local scales (was 3).

---

## 4. Confidence conformance (C4 detail)

All prompts emit the GOV-04 label set `HIGH | MEDIUM | LOW | ASSUMED | DISCREPANCY`. Review/audit prompts
additionally emit the orthogonal verdict `PASS | PARTIAL | FAIL` (GOV-04 §5). No prompt defines a numeric
or categorical scale of its own; the numeric band is documented as tooling-derived only.

---

## 5. Model & version conformance (C6/C7)

- Every prompt declares `model_pin: required (run manifest)` — no hardcoded model anywhere (GR-10.1).
- Every prompt carries `version: 1.0.0` + a §12 changelog entry dated 2026-06-24.
- AA-REVIEW-07 and FN-REVIEW-01 verify the run manifest records model + prompt/component versions (GR-10.2/10.3).

## 6. Overall

**Conformance verdict: PASS.** Every refactored prompt satisfies GOV-03, references GOV-01 via shared
components with zero inline governance duplication, and uses GOV-04 exclusively. The only deferred item
is the include-resolving assembler (orchestration code, out of scope), which does not affect prompt-level
conformance.
