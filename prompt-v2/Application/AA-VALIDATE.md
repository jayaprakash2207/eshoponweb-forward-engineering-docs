# AA-VALIDATE — Application Architecture Validation

## 1. Metadata
- prompt_id:        AA-VALIDATE
- version:          2.0.0
- owner_layer:      AA
- role:             VALIDATE
- status:           active
- governed_by:      GOV (V2-GOV) v2.0.0 → GOV-01
- confidence_model: CONFIDENCE (V2-CONF) v2.0.0 → GOV-04
- model_pin:        required (run manifest)
- consumes:         [aa.final, aa.forward-eng-inputs, aa.application-security-assessment, aa.prompts, run.manifest]
- produces:         [aa.quality-review, aa.executive-summary, aa.final-sanity-check, aa.workflow-audit, aa.missing-output-fixes]
- merges:           [AA-REVIEW-06, AA-REVIEW-07]
- last_updated:     2026-06-24

## 2. Purpose
Validate architecture quality, validate the workflow/process, validate completeness, and produce the
final Application Architecture sign-off — merging product QA (was AA-REVIEW-06) and process QA
(was AA-REVIEW-07) into one validate prompt with two clearly-separated sections.

> Merge rationale: both are AA-owned, both are Review-role, both run after `aa.final`, neither extracts
> new facts. The optimization step kept them separate to avoid conflating product vs process QA; here the
> two-prompt-per-layer model unifies them **as two labeled sections within one prompt**, preserving the
> distinction in structure while reducing prompt count. Inputs/outputs of both are retained.

## 3. Inputs
- `aa.final` + `aa.forward-eng-inputs` + `aa.application-security-assessment` (required, for §A product QA).
- `aa.prompts` + `run.manifest` (required, for §B process QA). Stop if final set incomplete (GR-7.1).

## 4. Responsibilities  (GOV-02: AA = Owner, VALIDATE role)
- **§A Product QA**: completeness, JSON validity, graph resolution, call-flow/diagram consistency, evidence traceability, risk validation, unknown handling.
- **§B Process QA**: stage completeness, I/O contract validation, source-modification guard, schema validation, hardcoding detection, parser extensibility, hallucination-handling, model/version pin check (GR-10).

## 5. Allowed Actions
- §A: validate artifacts against GR-7; verify graph edges resolve; verify claims carry evidence.
- §B: inspect prompts/outputs/manifest; verify GOV-03 conformance, GOV-02 ownership, and model/version pinning were respected.

## 6. Forbidden Actions
- MUST NOT add new architecture facts (review only); MUST NOT modify prompts/outputs (audit only, GR-5).
- MUST NOT use a local verdict/maturity vocabulary — use CONFIDENCE §5 (PASS/PARTIAL/FAIL).

## 7. Outputs  (AA_FILE marker; all legacy review filenames preserved)
- §A: `quality-review.md`, `executive-summary-for-review.md`, `final-sanity-check.md`.
- §B: `architecture-workflow-audit.md`, `missing-output-fixes.md`.
- Each carries a section verdict; the prompt emits an overall **PASS/PARTIAL/FAIL**.

## 8. Validation Rules
Per `Shared/VALIDATION.md`. §B confirms model + prompt/component versions recorded in the run manifest (GR-10.2/10.3).

## 9. Confidence Rules
Per `Shared/CONFIDENCE.md`. Material nodes: component, dependency, violation, stage-contract, schema.

## 10. Traceability Rules
Each finding references the artifact/stage/prompt it concerns and the GOV rule applied.

## 11. Governance Reference
Per `Shared/GOV.md` (role=VALIDATE, live_source=false, marker=AA_FILE, audience=technical).

## 12. Version Information
- 2.0.0 — 2026-06-24 — Merge of AA-REVIEW-06 (product QA) + AA-REVIEW-07 (process QA) into one VALIDATE prompt with two sections; all 5 review outputs retained. — Prompt Architect
- supersedes: AA-REVIEW-06, AA-REVIEW-07
- migration_ref: ../01_PROMPT_MERGE_REPORT.md#aa
