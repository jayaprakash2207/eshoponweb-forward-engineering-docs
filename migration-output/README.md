# Migration Output — Prompt Cutover to Governed Architecture

**Project:** Forward-/Reverse-Engineering Enterprise Pipeline (`frwd engg - op's`)
**Date:** 2026-06-24
**Authority:** `../prompt-governance/` (GOV-01…GOV-10), `../prompt-refactored/`, `../FINAL_PROMPT_CONFORMANCE_REPORT.md`
**Scope:** Prompt migration & cutover ONLY. No application code. No forward-engineering artifacts.

---

## What this package is

The cutover plan and validation that makes the **governed prompts** (`prompt-refactored/`) the **primary
implementation** for the pipeline, while preserving existing outputs and behavior. It maps every legacy
prompt to its governed replacement, identifies functional differences, certifies output compatibility,
scores regression risk, and validates the cutover against GOV-01/02/03/04/07/08.

## Documents

| File | Purpose |
|---|---|
| `PROMPT_REPLACEMENT_MATRIX.md` | Per-prompt: original → replacement, functional diff, output-compat, risk |
| `CHANGED_FILES.md` | Exact files changed/added/archived + runner reference updates |
| `COMPATIBILITY_REPORT.md` | Output schema/filename/marker compatibility vs downstream consumers |
| `REGRESSION_RISK_REPORT.md` | Risk per replacement + mitigations + go/no-go gates |
| `CUTOVER_VALIDATION_REPORT.md` | Final validation against the 5 governance invariants + sign-off |

## Cutover model (important)

This is a **non-git, no-backup** workspace and the legacy prompts are production files. The cutover is
therefore executed as a **parallel-primary** swap, not a destructive overwrite:

1. The governed prompts in `prompt-refactored/` become the **primary** (canonical) implementation.
2. Each runner's prompt reference is **repointed** to the governed prompt (see `CHANGED_FILES.md`).
3. Legacy prompt files are **retained read-only** as rollback until a green production run is observed.
4. **Blocking dependency:** the governed prompts use `{{include: CMP-*}}` directives. These must be
   **resolved** (governance text materialized into the model-facing prompt) at load time. Until the
   include-resolver exists, cutover uses the **assembled** form — see `REGRESSION_RISK_REPORT.md`
   R-CRIT-1. Deploying an unresolved `{{include}}` prompt would strip the governance text the model
   reads and regress behavior; this package gates the cutover on resolution.

No legacy file is deleted by this package; cutover = repoint + retain-for-rollback.
