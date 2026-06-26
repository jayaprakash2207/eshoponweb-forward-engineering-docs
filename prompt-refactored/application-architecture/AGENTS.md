# Application Architecture — Orchestrator Manifest

> Orchestrator (not a prompt). Drives the AA staged workflow. Refactor of legacy `AGENTS.md`.
> **Governance is NOT restated here.** All rules: GOV-01 via `_shared/CMP-GOV.md`.

## Metadata
- doc_id:       AA-ORCH
- version:      1.0.0
- owner_layer:  AA
- governed_by:  GOV-01 v1.0.0
- last_updated: 2026-06-24
- supersedes:   application-architecture/AGENTS.md

## Governance Reference
{{include: CMP-GOV role=Analyst}}
> The legacy "Golden Rules" block (previously restated verbatim here and in the master prompt and
> `00-global-rules.md`) is **removed**. It now lives once in GOV-01. See `_archived/AA_00-global-rules.POINTER.md`.

## Stage sequence (parse-first → reason — GR-6.1)

| Order | prompt_id | Role | Reads | Produces |
|---|---|---|---|---|
| 1 | AA-SCOUT-01 | Scout | repo | inventory/* |
| 2 | AA-SCOUT-02 | Scout | inventory/* | parsed/* |
| 3 | AA-ANALYST-03 | Analyst | inventory/* + parsed/* | evidence-packs/* |
| 4 | AA-ANALYST-04 | Analyst | evidence-packs/* | final/* |
| 5 | AA-ANALYST-05 | Analyst | final/* | forward-eng inputs (see relocations) |
| 5b | AA-ANALYST-06 | Analyst | final/* + parsed/* | application/data-level security assessment |
| 6 | AA-REVIEW-06 | Review | final/* | quality-review, exec-summary, sanity-check |
| 7 | AA-REVIEW-07 | Review | AGENTS.md + prompts + outputs | workflow audit |

## Acceptance standards
- Source-backed (GR-2), machine+human-readable, diagrammed, no source modification (GR-5).
- Each stage emits a verdict (GOV-04 §5). A stage producing a non-owned artifact is a release blocker (GOV-02).

## Relocations applied (GOV-02 / GOV-08)
- `business-capability-map.*` → **BA** (consumed here as forward-engineering input, cited).
- `data-ownership-map.md` → **DA** (consumed here, cited).
- Application/data-level security → **AA-ANALYST-06** (received from legacy TA OUTPUT 5).

## Version Information
- 1.0.0 — 2026-06-24 — Inline golden rules removed → GOV-01 pointer; stage prompt_ids assigned; relocations recorded. — Prompt Architect
