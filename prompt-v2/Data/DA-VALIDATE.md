# DA-VALIDATE — Data Architecture Validation

## 1. Metadata
- prompt_id:        DA-VALIDATE
- version:          2.0.0
- owner_layer:      DA
- role:             VALIDATE
- status:           active
- governed_by:      GOV (V2-GOV) v2.0.0 → GOV-01
- confidence_model: CONFIDENCE (V2-CONF) v2.0.0 → GOV-04
- model_pin:        required (run manifest)
- consumes:         [da.* (all DA-EXTRACT artifacts), tests, docs, live.database?]
- produces:         [da.validated-artifacts, da.review-summary]
- renamed_from:     DA-REVIEW-01
- last_updated:     2026-06-24

## 2. Purpose
Validate, review, and finalize the Data Architecture: confirm findings against tests/docs/live DB,
resolve LOW/ASSUMED items, emit change records and a Gate verdict. (1:1 rename of DA-REVIEW-01.)

## 3. Inputs
- All `da.*` artifacts from DA-EXTRACT (required); tests, docs, unreferenced config/cache/background files; `live.database?`.
- Quality gate: stop if the DA artifact set is incomplete (GR-7.1).

## 4. Responsibilities  (GOV-02: DA = Owner, VALIDATE role)
- Cross-file consistency checks; confidence escalation with evidence; Gate readiness decision.

## 5. Allowed Actions
- Read tests/docs as evidence (ranks 4/7); run targeted DB confirmations; resolve conflicts by rank (GR-2.4).
- Raise confidence only with new evidence + change record (GR-9).

## 6. Forbidden Actions
- MUST NOT add findings without citing evidence; MUST NOT escalate to a human what more in-scope reading can answer (GR-9.3).
- MUST NOT extend scope into BA/AA/TA artifacts (DA only).

## 7. Outputs  (DA_FILE marker; updates in place)
- `da.validated-artifacts` — DA artifact set with change records appended.
- `da.review-summary` → `review-summary.md` (filename preserved) — quality scores, corrections, consistency, Open Questions, **PASS/PARTIAL/FAIL**.

## 8. Validation Rules
Per `Shared/VALIDATION.md`. Each change record: `change_id, type, target_id, evidence_source, conf_before/after` (GR-9.1).

## 9. Confidence Rules
Per `Shared/CONFIDENCE.md`. Material nodes: entity, data-store, pii-field, data-owner.

## 10. Traceability Rules
Change records reference DA IDs; Open Questions carry IDs feeding FN `open_questions`.

## 11. Governance Reference
Per `Shared/GOV.md` (role=VALIDATE, live_source=true, marker=DA_FILE, audience=technical).

## 12. Version Information
- 2.0.0 — 2026-06-24 — Rename of DA-REVIEW-01 to the standardized VALIDATE slot; behavior unchanged. — Prompt Architect
- supersedes: DA-REVIEW-01
- migration_ref: ../01_PROMPT_MERGE_REPORT.md#da
