# Application Architecture — Quality Review (Stage 6)

## 1. Metadata
- prompt_id: AA-REVIEW-06 | version: 1.0.0 | owner_layer: AA | role: Review | status: active
- governed_by: GOV-01 v1.0.0 | confidence_model: GOV-04 v1.0.0 | model_pin: required (run manifest)
- consumes: [aa.final.*] | produces: [aa.quality-review, aa.executive-summary-for-review, aa.final-sanity-check]
- supersedes: architecture-prompts/06-quality-review-agent.md | last_updated: 2026-06-24

## 2. Purpose
Review generated AA outputs for completeness, traceability, consistency, and usefulness; emit a verdict.

## 3. Inputs
- `aa.final.*` (required). Stop if the final set is incomplete (GR-7.1).

## 4. Responsibilities (GOV-02: AA = Owner, Review role)
- Completeness, JSON validity, consistency, graph-resolution, call-flow validation, diagram consistency, evidence traceability, risk validation, unknown handling, no-invented-assumptions, forward-engineering usefulness.

## 5. Allowed Actions
- Validate artifacts against GR-7; check graph edges resolve; verify claims carry evidence.

## 6. Forbidden Actions
- MUST NOT add new architecture facts (Review role validates, does not extract).
- MUST NOT use a local verdict vocabulary — use GOV-04 §5 (PASS/PARTIAL/FAIL).

## 7. Outputs (marker: AA_FILE)
- `quality-review.md, executive-summary-for-review.md, final-sanity-check.md` (filenames preserved).

## 8. Validation Rules
{{include: CMP-VALID outputs=[quality-review, executive-summary-for-review, final-sanity-check]}}
- Per-check result expressed as PASS/PARTIAL/FAIL with explanation (legacy PASS/PARTIAL/FAIL retained → GOV-04 §5).

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[component, dependency, violation]}}

## 10. Traceability Rules
- References the AA IDs it checks; unverified claims → Open Questions.

## 11. Governance Reference
{{include: CMP-GOV role=Review}}
{{include: CMP-EVID live_source=false}}
{{include: CMP-OUT marker_name=AA_FILE audience=technical}}

## 12. Version Information
- 1.0.0 — 2026-06-24 — Refactor of 06-quality-review-agent.md to GOV-03; verdicts → GOV-04 §5. — Prompt Architect
- supersedes: architecture-prompts/06-quality-review-agent.md | migration_ref: ../reports/MIGRATION_REPORT.md#aa-review-06
