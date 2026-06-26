# Regression Risk Report

**Date:** 2026-06-24
**Purpose:** Score the behavioral-regression risk of the cutover and define go/no-go gates.

Risk = Likelihood × Impact on (a) functional behavior, (b) output compatibility, (c) governance conformance.

---

## 1. Critical risks (must clear before cutover completes)

### R-CRIT-1 — Unresolved `{{include: CMP-*}}` directives 🔴 BLOCKING
- **What:** Governed prompts reference governance/confidence/validation via `{{include: CMP-GOV}}` etc.
  The legacy runners load a prompt file and send its text to the model verbatim. If a governed prompt is
  loaded **without resolving includes**, the model receives the literal string `{{include: CMP-GOV}}`
  instead of the actual rules → **the model loses all governance text the legacy prompt used to contain**
  → severe behavioral regression (hallucination guards, exclusion lists, evidence rules all missing).
- **Likelihood:** High if cutover is a naive file swap. **Impact:** Severe.
- **Mitigation / gate:** Cutover MUST use the **assembled** prompt — includes resolved into full text —
  before the runner sends it. Two acceptable paths:
  1. Build the include-resolver (deferred code) and load through it; **or**
  2. Pre-assemble static "resolved" copies of each governed prompt (concatenate the `_shared/CMP-*.md`
     bodies in place of each include) and point the runner at the resolved copy.
- **Status:** **GATE OPEN — cutover not complete until includes are resolved.** This is the single
  highest-risk item and the reason cutover is parallel-primary, not a destructive swap.

### R-CRIT-2 — Ownership-relocation file-path breakage (R-09, R-17) 🔴
- **What:** TA OUTPUT 4/5 and AA Stage-05 capability/data-ownership maps move folders. Consumers that
  hardcode `ta-outputs/data-architecture-assessment.md`, `ta-outputs/security-architecture-assessment.md`,
  `aa-outputs/final/business-capability-map.*`, or `aa-outputs/final/data-ownership-map.md` will not find them.
- **Likelihood:** Medium (depends on whether any consumer globs those exact paths). **Impact:** Medium —
  missing-file, not wrong-content.
- **Mitigation:** (a) Repoint consumers per COMPATIBILITY_REPORT C-09/C-17; (b) Foundation cross-links
  resolve provenance so the canonical graph is unaffected; (c) optional transitional symlink/pointer
  files at old paths during a deprecation window.
- **Status:** GATE — repoint consumers or accept the deprecation-window pointers before relying on the
  relocated artifacts.

---

## 2. Risk register (per replacement)

| Ref | Replacement | Behavioral risk | Output risk | Governance risk | Overall |
|---|---|---|---|---|---|
| R-01 | BA-SCOUT-01 | entity-ref now consume-cite | low (table shape same) | none | 🟡 |
| R-02 | BA-ANALYST-01 | consumes DA/AA facts | low | none | 🟡 |
| R-03 | (archive P3) | none (never executed) | none | none | 🟢 |
| R-04 | BA-ANALYST-02 | sole owner business rules; entity shape | ⚠️ entity element shape | none | 🟡 |
| R-05 | BA-ANALYST-03 | data-model = DA view | none | none | 🟢 |
| R-06 | DA-SCOUT-01 + DA-ANALYST-01 | single prompt → two phases | ✅ 13 files + 2 | none | 🟡 |
| R-07 | DA-REVIEW-01 | verdict vocab → GOV-04 | none | none | 🟢 |
| R-08 | TA-SCOUT-01 | none material | none | none | 🟢 |
| R-09 | TA-ANALYST-01 | **OUT4/5 relocate** | 🔴 path change | none (closes violation) | 🔴 |
| R-10 | AGENTS.md | rules → pointer | none | none | 🟢 |
| R-11 | AA-ANALYST-00 | tech-stack → consume TA | low | none | 🟡 |
| R-12 | (demote 00-global-rules) | rules → GOV-01 | none | **removes dup** | 🟡 |
| R-13..16 | AA-SCOUT-01/02, AA-ANALYST-03/04 | governance externalized | none | none | 🟢 |
| R-17 | AA-ANALYST-05 | **capability/data-ownership relocate** | 🔴 path change | none (closes violation) | 🔴 |
| R-18 | AA-ANALYST-06 | new app-security stage | 🟡 additive | none | 🟡 |
| R-19 | AA-REVIEW-06 | verdict → GOV-04 | none | none | 🟢 |
| R-20 | AA-REVIEW-07 | + manifest checks | none | none | 🟢 |
| R-22..24 | FN-SYNTH/REVIEW | new layer (additive) | ✅ additive | none | 🟡 |

**Distribution:** 🟢 11 · 🟡 9 · 🔴 2 (+ R-CRIT-1 cross-cutting blocker).

---

## 3. Risk by category (governance validation targets)

| Validation invariant | Residual risk after mitigation |
|---|---|
| No ownership violations | **None** — verified in CUTOVER_VALIDATION §2 (4 closed). |
| No boundary violations | **None** — verified §3. |
| No duplicated extraction logic | **None** — single owners; others consume-cite. |
| No duplicated governance blocks | **None** *after R-CRIT-1 resolved* — includes must materialize to one source. |
| No confidence-model drift | **None** — CMP-CONF only; GOV-04 labels; legacy numeric → band. |

---

## 4. Go / No-Go gates

| Gate | Condition | State |
|---|---|---|
| G1 | `{{include}}` directives resolved (R-CRIT-1) | ⛔ OPEN — required |
| G2 | Relocated-file consumers repointed or pointers placed (R-CRIT-2) | ⛔ OPEN — required |
| G3 | A pilot run on eShopOnWeb produces the preserved artifact set (diff vs current `output/`) | ⛔ OPEN — recommended before legacy retirement |
| G4 | Conformance = PASS (FINAL_PROMPT_CONFORMANCE_REPORT) | ✅ MET (98/100) |
| G5 | Legacy files retained read-only for rollback | ✅ MET (no deletion) |

**Cutover recommendation:** Proceed with **parallel-primary** designation now (governed prompts are
canonical, conformance met, legacy retained). **Do NOT retire legacy prompts** until G1–G3 close. The
two 🔴 items are path relocations (the intended fix), not content regressions; G1 (includes) is the only
risk that could silently degrade model behavior and is therefore the hard blocker.

---

## 5. Rollback plan

- Legacy prompts are **retained read-only**; rollback = repoint each runner constant back to the legacy
  `.md` (one-line revert per runner; enumerated in `CHANGED_FILES.md` §4).
- No generated artifacts were modified, so rollback requires no data restoration.
- Foundation prompts are additive; rolling back simply means not invoking the (deferred) FN runner.
