# 07 — Migration Plan (Current → Optimized)

**Date:** 2026-06-24 · No code generated here; this is the plan + rollback. Prompt edits happen in `prompt-refactored/` source, then re-resolve into `prompt-resolved/`.

---

## 1. Change set

| # | Change | Type | Files |
|---|---|---|---|
| M1 | Fold BA-SCOUT-01 structural-scout discipline into BA-ANALYST-02 as "Phase A" | prompt edit | `prompt-refactored/business-architecture/BA-ANALYST-02.md` (+ re-resolve) |
| M2 | Retire BA-SCOUT-01 + BA-ANALYST-01 (unwired) → move to `_archived/` as pointers | archive | 2 files |
| M3 | Merge DA-SCOUT-01 + DA-ANALYST-01 → `DA-EXTRACT-01` | prompt edit | new `DA-EXTRACT-01.md`; archive the two |
| M4 | Update DA runner load to single prompt (restore original 1:1) | code (deferred) | `da_agent1_runner.py` PROMPT_FILE constant |
| M5 | Re-resolve affected prompts into `prompt-resolved/` | regenerate | BA-ANALYST-02, DA-EXTRACT-01 |
| M6 | Update governance docs' inventory counts (21→18) | doc | GOV-02/06/07 cross-refs (informational) |

> AA / TA / FN prompts: **no change.**

## 2. Wave sequence

```
Wave 0  Snapshot current prompt-refactored/ + prompt-resolved/ (rollback baseline).        [reversible]
Wave 1  M3: author DA-EXTRACT-01 (Phase A scout + Phase B analyst, single CMP-* set).      [source only]
Wave 2  M1: add BA-ANALYST-02 Phase A (structural scout); M2: archive BA-SCOUT/ANALYST-01. [source only]
Wave 3  M5: re-resolve BA-ANALYST-02 + DA-EXTRACT-01 into prompt-resolved/.                [generated]
Wave 4  M4: repoint da_agent1_runner to DA-EXTRACT-01 (single load).                        [code, deferred]
Wave 5  Pilot re-validate on eShopOnWeb: confirm 13+2 DA files + layer2_output.json + graph parity.
Wave 6  Update doc counts (M6); mark optimization complete.
```

**Behavioral-equivalence guarantee:** Waves 1–3, 6 are source/generated docs only. Wave 4 changes *how*
DA loads its prompt (single vs two-phase) but **not what it produces** (same 13+2 files). The eliminated
BA prompts never executed, so removing them cannot change runtime output.

## 3. Validation gates (must pass before legacy prompt deletion)

| Gate | Condition |
|---|---|
| O1 | DA-EXTRACT-01 produces all 13 legacy files + 2 relocated-in (filenames + DA_FILE markers) |
| O2 | BA-ANALYST-02 still emits `layer2_output.json` with unchanged top-level schema |
| O3 | Knowledge graph still 9 sections / 274 nodes / 117 links (FN-REVIEW-01 = PASS) |
| O4 | Governance re-check: GOV-01/02/03/04/07/08 still 100% (no inline dup, single owner) |
| O5 | `grep '{{'` on re-resolved prompts = 0 |

## 4. Rollback

| Trigger | Rollback action | Cost |
|---|---|---|
| O1/O2 fail (DA or BA output drift) | Revert `da_agent1_runner` PROMPT_FILE to the split pair / restore BA-ANALYST-02 from Wave 0 snapshot; re-resolve | minutes |
| O3 fail (graph regression) | Revert merged prompts; FN inputs unchanged → graph restored | minutes |
| Any consumer breaks | Restore archived BA-SCOUT-01/ANALYST-01 + DA split from `_archived/` | minutes |

- All retired prompts are **archived, not deleted** (pointers in `_archived/`), so rollback is a file
  restore + runner-constant revert. No generated artifacts are modified during migration.
- Because the optimization is **within-layer, within-owner**, rollback never touches AA/TA/FN/governance.

## 5. Sequencing dependency note

M4 (runner code) is the only **executable** change and is **deferred** (no code in this task). Until M4
lands, DA-EXTRACT-01 can be loaded by concatenation in the existing runner to reproduce single-pass
behavior, so the source optimization (M1–M3, M5) can proceed and be validated independently.
