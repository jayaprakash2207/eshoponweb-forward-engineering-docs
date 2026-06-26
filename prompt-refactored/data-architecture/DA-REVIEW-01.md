# Data Architecture — Review & Gate

## 1. Metadata
- prompt_id:        DA-REVIEW-01
- version:          1.0.0
- owner_layer:      DA
- role:             Review
- status:           active
- governed_by:      GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin:        required (run manifest)
- consumes:         [DA-SCOUT-01.*, DA-ANALYST-01.*, tests, docs, live.database?]
- produces:         [da.reviewed-artifacts, da.review-summary]
- supersedes:       DA_REVIEW_PROMPT.md
- last_updated:     2026-06-24

## 2. Purpose
Validate and enrich the DA artifacts against tests, docs, and the live DB; resolve LOW/ASSUMED items;
emit change records and a Gate verdict.

## 3. Inputs
- `DA-SCOUT-01.*` + `DA-ANALYST-01.*` (required).
- Test files, documentation, unreferenced Config/Extensions/HealthChecks/Cache/Background files.
- `live.database` (optional) — confirmation queries.
- Quality gate: stop if the DA artifact set is incomplete (GR-7.1).

## 4. Responsibilities  (GOV-02: DA = Owner, Review role)
- Cross-file consistency checks; confidence escalation with evidence; Gate readiness decision.

## 5. Allowed Actions
- Read tests/docs as evidence (ranks 4 / 7 per CMP-EVID); run targeted DB confirmations.
- Raise confidence only with new evidence, recording a change record (GR-9.1/9.2).
- Resolve conflicts by evidence rank (GR-2.4); never average.

## 6. Forbidden Actions
- MUST NOT add findings without citing evidence (GR-2.1).
- MUST NOT escalate to a human a question answerable by reading more in-scope evidence (GR-9.3).
- MUST NOT extend scope into BA/AA/TA artifacts (review DA only).

## 7. Outputs  (marker: DA_FILE; updates in place)
- `da.reviewed-artifacts` — the DA artifact set with change records appended.
- `da.review-summary` (`review-summary.md`, filename preserved) — overview, before/after scores, corrections, consistency results, Open Questions, **Gate verdict (PASS/PARTIAL/FAIL — GOV-04 §5)**.

## 8. Validation Rules
{{include: CMP-VALID outputs=[reviewed-artifacts, review-summary]}}
- Each change record has `change_id, type(ADDED/CORRECTED/ENRICHED), target_id, evidence_source, confidence_before, confidence_after` (GR-9.1).

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[entity, data-store, pii-field, data-owner]}}

## 10. Traceability Rules
- Change records reference the DA IDs they modify; verdict references the artifact set.
- Open Questions carry IDs feeding Foundation `open_questions`.

## 11. Governance Reference
{{include: CMP-GOV role=Review}}
{{include: CMP-EVID live_source=true}}
{{include: CMP-OUT marker_name=DA_FILE audience=technical}}

## 12. Version Information
- changelog:
  - 1.0.0 — 2026-06-24 — Refactor of DA_REVIEW_PROMPT.md to GOV-03; change records → GR-9 reference; Gate G1 verdict → GOV-04 PASS/PARTIAL/FAIL; evidence hierarchy → CMP-EVID. — Prompt Architect
- supersedes: DA_REVIEW_PROMPT.md
- migration_ref: ../reports/MIGRATION_REPORT.md#da-review-01
