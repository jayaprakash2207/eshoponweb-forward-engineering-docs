# Application Architecture — Workflow Audit (Stage 7)

## 1. Metadata
- prompt_id: AA-REVIEW-07 | version: 1.0.0 | owner_layer: AA | role: Review | status: active
- governed_by: GOV-01 v1.0.0 | confidence_model: GOV-04 v1.0.0 | model_pin: required (run manifest)
- consumes: [AGENTS.md, aa.prompts.*, aa.outputs.*, run.manifest] | produces: [aa.architecture-workflow-audit, aa.missing-output-fixes]
- supersedes: architecture-prompts/07-workflow-audit-agent.md | last_updated: 2026-06-24

## 2. Purpose
Audit the AA extraction workflow itself for enterprise readiness and reuse — a meta-level gate over the pipeline.

## 3. Inputs
- `AGENTS.md`, all AA prompts, all AA outputs, and the run manifest (for model/version pins, GR-10).

## 4. Responsibilities (GOV-02: AA = Owner, Review role)
- Stage completeness, I/O contract validation, source-modification guard, schema validation, run-history verification, graph normalization, quality-gate verification, hardcoding detection, parser extensibility, hallucination-handling, forward-engineering usefulness.

## 5. Allowed Actions
- Inspect prompts/outputs/manifest; verify GOV-03 conformance and GOV-02 ownership were respected.

## 6. Forbidden Actions
- MUST NOT modify prompts or outputs (audit only, GR-5).
- MUST NOT use a local maturity vocabulary — map to GOV-04 §5 (ENTERPRISE READY=PASS, etc.).

## 7. Outputs (marker: AA_FILE)
- `architecture-workflow-audit.md, missing-output-fixes.md` (filenames preserved); include a score /100 and a GOV-04 verdict.

## 8. Validation Rules
{{include: CMP-VALID outputs=[architecture-workflow-audit, missing-output-fixes]}}
- Confirm model + prompt/component versions are recorded in the run manifest (GR-10.2/10.3).

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[stage-contract, schema]}}

## 10. Traceability Rules
- Each finding references the stage/prompt/output it concerns and the GOV rule applied.

## 11. Governance Reference
{{include: CMP-GOV role=Review}}
{{include: CMP-EVID live_source=false}}
{{include: CMP-OUT marker_name=AA_FILE audience=technical}}

## 12. Version Information
- 1.0.0 — 2026-06-24 — Refactor of 07-workflow-audit-agent.md to GOV-03; maturity verdict → GOV-04 §5; manifest/version checks added. — Prompt Architect
- supersedes: architecture-prompts/07-workflow-audit-agent.md | migration_ref: ../reports/MIGRATION_REPORT.md#aa-review-07
