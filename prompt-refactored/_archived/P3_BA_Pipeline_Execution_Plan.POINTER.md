# ARCHIVED — BA_Pipeline_Execution_Plan.md (superseded)

**Status:** Superseded / archived (audit finding F7).
**Date:** 2026-06-24
**Authority:** `../../prompt-governance/06_PROMPT_REFACTORING_PLAN.md` (P3), `10_PROMPT_ARCHITECTURE_TARGET_STATE.md` §3 Wave 1.

## Why archived
The legacy `BA_Pipeline_Execution_Plan.md` (1,566 lines) described an aspirational SLM + vector-DB
architecture (Phi-3/Mistral, embeddings) that **no runner implements** — the actual pipeline calls the
`claude` CLI directly. The audit found ~55% of its content re-describes Layer 2 / Layer 3 tasks and it
also leaks DA (DDL/stored procs) and TA (infra/cloud) work into a BA document.

## Replacement
- Executable business analysis: **BA-ANALYST-02** (`../business-architecture/BA-ANALYST-02.md`) and **BA-ANALYST-03** (`../business-architecture/BA-ANALYST-03.md`).
- Structural inventory: **BA-SCOUT-01**, deep analysis: **BA-ANALYST-01**.
- Pipeline narrative & governance: this `prompt-refactored/` package + `../../prompt-governance/`.

## Action
No behavior is lost (nothing executed this plan). Retain the original file read-only for historical
reference; do not consume it in any run. Cross-track reconciliation it gestured at is now owned by the
Foundation layer (`../foundation/`).
