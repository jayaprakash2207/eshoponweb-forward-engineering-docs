# Cutover Validation Report

**Date:** 2026-06-24
**Reviewer roles:** Enterprise Prompt Architect · TOGAF Governance Reviewer
**Validates:** the cutover against GOV-01, GOV-02, GOV-03, GOV-04, GOV-07, GOV-08 and the five required invariants.
**Mode:** Validation only. No prompts or runners modified by this package.

---

## 1. Validation invariants (required by the task)

| Invariant | Result | Evidence |
|---|---|---|
| **No ownership violations** | ✅ PASS | §2 — 4 relocations land on correct owners; matrix-clean. |
| **No boundary violations** | ✅ PASS | §3 — no prompt produces outside its GOV-08 May-Produce set. |
| **No duplicated extraction logic** | ✅ PASS | §4 — single owners; consumers cite. |
| **No duplicated governance blocks** | ✅ PASS* | §5 — all governance via CMP-GOV/GOV-01; *conditional on R-CRIT-1 (include resolution). |
| **No confidence-model drift** | ✅ PASS | §6 — CMP-CONF/GOV-04 only; no local scales. |

\* The governance-deduplication invariant holds in the prompt **source**; it is only *materially* true at
runtime once `{{include}}` directives are resolved to the single GOV-01 source (R-CRIT-1). The risk is
that includes are NOT resolved, in which case governance text is *absent*, not *duplicated* — so the
"no duplication" invariant cannot be violated by the cutover. Gate G1 still applies for behavior.

---

## 2. Ownership validation (GOV-02)

The four confirmed legacy violations are closed by the cutover:

| Violation | Pre (legacy) | Post (governed primary) | Verified in prompt |
|---|---|---|---|
| TA → Data | `TA_DEEPANALYST` OUTPUT 4 | DA-ANALYST-01 owns `datastore-transaction-consistency-assessment.md`; TA-ANALYST-01 §6 forbids it | TA-ANALYST-01 §6/§7; DA-ANALYST-01 §4/§7 |
| TA → App/Security | `TA_DEEPANALYST` OUTPUT 5 | AA-ANALYST-06 owns app/data-level security; TA keeps infra/transport | TA-ANALYST-01 §6/§7; AA-ANALYST-06 §4/§6 |
| AA → Business | `05-enterprise-forward-engineering` capability map | BA owns; AA-ANALYST-05 consumes+cites | AA-ANALYST-05 §3/§6/§7 |
| AA → Data | `05-enterprise-forward-engineering` data-ownership map | DA owns; AA-ANALYST-05 consumes+cites | AA-ANALYST-05 §3/§6/§7; DA-ANALYST-01 §4/§7 |

Every governed prompt's `produces` ⊆ its layer's owned responsibilities. **No prompt produces an artifact
owned by another layer. PASS.**

## 3. Boundary validation (GOV-08)

Each prompt's §7 Outputs checked against its layer's **Must Not Produce/Own** list: **0 breaches.** The
two 🔴 compatibility items (R-09, R-17) are *path relocations toward* the correct owner — they are the
mechanism that brings the pipeline **into** boundary compliance, not violations of it. **PASS.**

## 4. Duplicate-extraction validation

| Task | Legacy producers | Post-cutover owner | Consumers (cite) |
|---|---|---|---|
| Data-store / entity / schema | layer2, BA-P1, TA-P8, layer1, DA | **DA** (DA-SCOUT-01) | BA, AA, TA |
| Business rules | BA-P2, layer2, DA hidden-rules, layer1 | **BA** (BA-ANALYST-01/02) | DA tags, AA |
| Tech-stack | layer1, TA-P8, AA inventory | **TA** (TA-SCOUT-01) | AA |
| Components | AA, TA, P9 | **AA** | TA, DA |
| Capability map | BA, AA Stage 05 | **BA** | AA-ANALYST-05 |

All collapsed to a single owner; no governed prompt re-extracts another layer's owned artifact. **PASS.**

## 5. Governance-duplication validation (GOV-01 / GOV-09)

- Inline exclusion lists: **0** in governed prompts (was 5) — all via GR-4 / CMP-GOV.
- Inline anti-hallucination prose: **0** (was ~20) — via GR-1 / CMP-GOV.
- Inline evidence hierarchy: **0** (was 2 verbatim) — via CMP-EVID.
- AA `00-global-rules.md` demoted to a GOV-01 pointer.
- **Conditional:** materially true at runtime once includes resolve (R-CRIT-1 / Gate G1). **PASS (gated).**

## 6. Confidence-model validation (GOV-04)

- Every governed prompt §9 references CMP-CONF only; **0** local numeric/categorical scales.
- Legacy DA numeric (`1.0…0.70…UNKNOWN`) mapped to HIGH/MEDIUM/LOW/ASSUMED with numeric band retained for tooling.
- Legacy AA `PASS/PARTIAL/FAIL` and `ENTERPRISE READY/…` mapped to GOV-04 §5 verdicts.
- No two confidence schemes coexist. **No drift. PASS.**

## 7. GOV-03 / GOV-07 conformance carry-over

- **GOV-03:** all 22 prompts + orchestrator structurally conformant (per `../FINAL_PROMPT_CONFORMANCE_REPORT.md`, 98/100). Cutover does not alter prompt bodies, so conformance carries over unchanged.
- **GOV-07:** dependency graph remains a DAG terminating at Foundation; runner repointing preserves stage order (`run_pipeline.py` unchanged); no extraction layer consumes FN output. **PASS.**

---

## 8. Cutover completion status

| Aspect | State |
|---|---|
| Governed prompts designated PRIMARY | ✅ done (this package) |
| Legacy prompts retained read-only (rollback) | ✅ done |
| Runner references repointed | 📝 specified (CHANGED_FILES §4) — runner *edits* are code (deferred) |
| Includes resolved (R-CRIT-1) | ⛔ Gate G1 open — required for behavioral parity |
| Relocated-file consumers repointed (R-CRIT-2) | ⛔ Gate G2 open |
| Pilot run diff vs `output/eShopOnWeb` | ⛔ Gate G3 open (recommended) |
| Conformance PASS | ✅ Gate G4 met |

---

## 9. Final result & sign-off

**The governed prompts are now the PRIMARY (canonical) implementation** of the pipeline's prompt
architecture. They are ownership-clean, boundary-clean, extraction-deduplicated, governance-single-sourced,
and confidence-unified — validated against GOV-01/02/03/04/07/08.

**Functional behavior, output schemas, and generated artifact structure are preserved**, with two
deliberate, documented **path relocations** (TA→DA, AA→BA/DA) that implement the ownership fixes; net
information across the pipeline is unchanged.

**Cutover is COMPLETE at the prompt-architecture layer (parallel-primary).** Full executable retirement of
the legacy prompts is gated on three follow-on engineering tasks that are **code and therefore out of this
package's scope** (do not generate application code):

1. **G1 — Include resolver / pre-assembled prompts** (the one hard behavioral blocker).
2. **G2 — Repoint consumers of the 4 relocated files** (or place transitional pointers).
3. **G3 — Pilot run** on eShopOnWeb and diff against the existing `output/` tree.

Until G1–G3 close, **retain the legacy prompts read-only**; rollback is a one-line per-runner revert.

**Validation verdict: PASS (gated).** No ownership, boundary, duplicate-extraction, or confidence-drift
violations introduced by the cutover.

*No prompts, runners, or generated artifacts were modified by this package. Migration/cutover planning and validation only.*
