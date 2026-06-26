# Cutover Readiness Report

**Package:** `prompt-resolved/`
**Date:** 2026-06-24
**Purpose:** State which cutover gates this resolved package closes, and what remains before legacy-prompt retirement.
**References:** `../migration-output/REGRESSION_RISK_REPORT.md`, `../migration-output/CUTOVER_VALIDATION_REPORT.md`, `../FINAL_PROMPT_CONFORMANCE_REPORT.md`

---

## 1. Gate status (from the cutover plan)

| Gate | Condition | Before | Now |
|---|---|---|---|
| **G1** | `{{include}}` directives resolved (R-CRIT-1) | ⛔ OPEN | ✅ **CLOSED by this package** |
| G2 | Relocated-file consumers repointed (R-CRIT-2) | ⛔ OPEN | ⛔ OPEN (code/config — out of scope) |
| G3 | Pilot run on eShopOnWeb diffs clean vs `output/` | ⛔ OPEN | ⛔ OPEN (requires a run) |
| G4 | Conformance = PASS | ✅ MET | ✅ MET (98/100) |
| G5 | Legacy prompts retained read-only (rollback) | ✅ MET | ✅ MET |

**This package closes the single hard behavioral blocker (G1).** The resolved prompts can now be loaded
by runners and sent to the model with the full governance/confidence/validation text inline — no resolver
needed in the execution path.

## 2. What "production-ready, runner-executable" means here

A runner's prompt-load step becomes a plain file read of the resolved `.md`:

| Runner | Loads (resolved) | Marker the runner parses | Match |
|---|---|---|---|
| `layer2_runner.py` | `business-architecture/BA-ANALYST-02.md` | JSON / `layer2_output.json` | ✅ |
| `layer3_runner.py` | `business-architecture/BA-ANALYST-03.md` | `===DOCUMENT_START:===` | ✅ |
| `da_agent1_runner.py` | `data-architecture/DA-SCOUT-01.md` + `DA-ANALYST-01.md` | DA_FILE / 13-file set | ✅ (two-phase load — see §4) |
| `da_agent2_runner.py` | `data-architecture/DA-REVIEW-01.md` | DA_FILE / `review-summary.md` | ✅ |
| `ta_agent1_runner.py` | `technology-architecture/TA-SCOUT-01.md` | `===TA_FILE_START===` | ✅ |
| `ta_agent2_runner.py` | `technology-architecture/TA-ANALYST-01.md` | `===TA_FILE_START===` | ✅ |
| `aa_runner.py` | governed AA stages + `AGENTS.md` + `AA-ANALYST-00.md` | `===AA_FILE_START===` | ✅ |
| (new FN runner — deferred) | `foundation/FN-SYNTH-01/02`, `FN-REVIEW-01` | `===FN_FILE_START===` | ✅ (additive) |

Every resolved prompt's CMP-OUT block carries the **exact marker the existing runner already parses**, so
output capture and downstream artifact structure are unchanged.

## 3. Readiness validation summary

| Invariant | Result |
|---|---|
| No unresolved include directives | ✅ 0 across 22 files |
| No duplicated governance blocks | ✅ 1 GOV-01 block per file |
| No ownership violations | ✅ preserved from governed source (4 relocations intact) |
| No boundary violations | ✅ preserved (§6/§7 unchanged) |
| No confidence drift | ✅ CMP-CONF / GOV-04 only; no legacy scale |
| Conformance (GOV-03) | ✅ structure unchanged; 98/100 carries over |
| Output/marker compatibility | ✅ legacy markers preserved per prompt |

## 4. Remaining work before legacy retirement (NOT in scope — no code generated)

| Item | Type | Gate | Note |
|---|---|---|---|
| Repoint runner prompt-path constants to `prompt-resolved/` | code (1 line/runner) | enables G-load | enumerated in `../migration-output/CHANGED_FILES.md` §4 |
| `da_agent1_runner.py` two-phase load (SCOUT then ANALYST) | code | — | or concatenate the two resolved DA prompts for single-pass parity |
| `aa_runner.py` new stage 05b (AA-ANALYST-06) + FN runner | code | additive | does not block legacy parity |
| Repoint consumers of 4 relocated files (TA→DA, AA→BA/DA) | code/config | **G2** | see `../migration-output/COMPATIBILITY_REPORT.md` C-09/C-17 |
| Pilot run + artifact diff vs `output/eShopOnWeb` | run | **G3** | recommended before retiring legacy |
| Model pin in run manifest (GR-10) | code | — | `model_pin: required` declared in every prompt |

## 5. Recommendation

**G1 closed → proceed to repoint runners at `prompt-resolved/` and run the G3 pilot.** Keep legacy prompts
read-only until G2 and G3 close; rollback remains a one-line per-runner revert. The resolved prompts are
the production artifacts the runners should load; the unresolved governed prompts in `prompt-refactored/`
remain the **editable source** (edit there, re-resolve, never hand-edit `prompt-resolved/`).

## 6. Maintenance note

`prompt-resolved/` is a **generated** package. The editing workflow is:
1. edit the governed source in `prompt-refactored/` (or a component in `_shared/`),
2. re-run resolution,
3. never hand-edit files in `prompt-resolved/` (they would drift from source).

When the include-resolver (deferred code) is built, this package becomes its build output and can be
regenerated on demand.
