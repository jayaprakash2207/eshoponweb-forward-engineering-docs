# 05 — Cutover Decision Report

**Pilot:** eShopOnWeb · **Date:** 2026-06-24
**Decision roles:** Enterprise Architect · TOGAF Reviewer · QA Lead · Migration Validation Specialist

---

## 1. Success-criteria scorecard

| Criterion | Threshold | Actual | Met? |
|---|---|---|---|
| Output Compatibility | ≥ 95% | **98.7%** | ✅ |
| Knowledge Preservation | ≥ 95% | **100%** | ✅ |
| Governance Compliance | = 100% | **100%** | ✅ |
| Ownership Violations | 0 | **0** | ✅ |
| Boundary Violations | 0 | **0** | ✅ |
| Unresolved References | 0 | **0** | ✅ |
| Knowledge Graph Integrity | PASS | **PASS** | ✅ |

**All seven PASS gates are met.**

## 2. Decision

# ✅ PASS WITH CONDITIONS

The resolved governed prompt architecture **can replace** the legacy prompts without regression of
content, knowledge, ownership, boundaries, governance, or graph integrity. It is approved subject to two
operational conditions that are **execution/code** items, not prompt defects.

**Why "WITH CONDITIONS" and not unconditional PASS:** this pilot is a **contract + real-artifact**
validation (the pipeline could not be live-executed here — see `00_README_AND_METHOD.md`). Two
conditions must close before legacy retirement:

1. **C1 — Live G3 run.** Execute one end-to-end run on eShopOnWeb with the pinned model and diff
   structural artifacts (IDs, counts, 274-node graph, 117 links) against the legacy baseline. Expect
   structural parity; accept prose-wording deltas (RG-5) if structure matches.
2. **C2 — Consumer repoint (G2).** Repoint anything globbing the four relocated paths
   (`aa-outputs/business-capability-map.*`, `aa-outputs/data-ownership-map.md`,
   `ta-outputs/data-architecture-assessment.md`, `ta-outputs/security-architecture-assessment.md`), or
   place transitional pointer files for a deprecation window.

Both are documented in `../migration-output/` (Gates G2/G3) and are **code/config**, explicitly outside
this validation's scope.

## 3. Go / No-Go recommendation

| Dimension | Recommendation |
|---|---|
| **Prompt architecture** | **GO** — governed/resolved prompts are conformant, compatible, and knowledge-preserving. |
| **Designate resolved set as PRIMARY** | **GO** — already validated; make `prompt-resolved/` the runner-loaded set. |
| **Retire legacy prompts** | **NO-GO until C1 + C2 close** — keep legacy read-only as rollback. |

## 4. Deployment recommendation

- **Phase 1 (now):** Repoint runner prompt-path constants to `prompt-resolved/` (one line per runner;
  enumerated in `../migration-output/CHANGED_FILES.md` §4). Keep legacy files in place.
- **Phase 2 (C1):** Run the live G3 pilot on eShopOnWeb; capture structural diff; sign off on parity.
- **Phase 3 (C2):** Repoint relocated-file consumers / place deprecation pointers.
- **Phase 4:** After two consecutive green runs, retire legacy prompts.
- Foundation prompts (FN-*) and AA-ANALYST-06 require their runner wiring (code, deferred) — additive,
  do not block Phase 1.

## 5. Rollback recommendation

- **Trigger:** any structural diff failure in C1 (missing artifact, node loss, dangling link, ownership
  mismatch) or a downstream consumer break in C2.
- **Action:** revert each runner's prompt-path constant to the legacy `.md` (one-line per-runner revert);
  no generated-artifact restoration needed (none were modified).
- **Cost:** minutes; fully reversible. Legacy set retained read-only specifically for this.
- **Risk of needing rollback:** Low — all hard gates already pass; rollback chiefly guards the
  unverified live-prose dimension (RG-5).

## 6. Conditions ledger

| ID | Condition | Owner | Blocks legacy retirement? |
|---|---|---|---|
| C1 | Live G3 structural-diff run = parity | runner/ops | Yes |
| C2 | Relocated-file consumers repointed | runner/ops | Yes |
| (info) | DA two-phase load + FN/AA-06 runner wiring | engineering | No (additive) |

**Decision: PASS WITH CONDITIONS — designate resolved prompts PRIMARY now; retire legacy only after C1 + C2.**
