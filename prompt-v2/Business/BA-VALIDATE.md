# BA-VALIDATE — Business Architecture Validation

## 1. Metadata
- prompt_id:        BA-VALIDATE
- version:          2.0.0
- owner_layer:      BA
- role:             VALIDATE
- status:           active
- governed_by:      GOV (V2-GOV) v2.0.0 → GOV-01
- confidence_model: CONFIDENCE (V2-CONF) v2.0.0 → GOV-04
- model_pin:        required (run manifest)
- consumes:         [ba.layer2_output, ba.10-business-documents]
- produces:         [ba.validated-documents, ba.business-review-summary]
- last_updated:     2026-06-24

## 2. Purpose
Validate, review, and finalize the Business Architecture: confirm rules/capabilities are evidence-backed,
documents are internally consistent, INFERRED content is flagged, and emit a Gate verdict.

> Note: the legacy wired BA pipeline had **no separate BA review prompt**. BA-VALIDATE adds the
> standardized validate stage required by the two-prompt-per-layer model. It is **additive and
> non-destructive** — it reviews, never re-extracts (GR-1), so it cannot change BA outputs, only gate them.

## 3. Inputs
- `ba.layer2_output` + `ba.10-business-documents` (required). Stop if missing (GR-7.1).

## 4. Responsibilities  (GOV-02: BA = Owner, VALIDATE role)
- Consistency checks (BR references resolve; capabilities trace to services); INFERRED-content flagging;
  confidence escalation with evidence (change records); Gate verdict.

## 5. Allowed Actions
- Cross-check documents vs `layer2_output.json`; verify every BR cited exists; verify capability↔service links.
- Raise confidence only with new evidence + change record (GR-9).

## 6. Forbidden Actions
- MUST NOT extract new business facts (VALIDATE role; review only).
- MUST NOT modify DA/AA/TA artifacts; MUST NOT use a local verdict vocabulary (use CONFIDENCE §5).

## 7. Outputs
- `ba.validated-documents` — the 10 docs + change records appended (DOCUMENT marker; filenames unchanged).
- `ba.business-review-summary` → `ba-review-summary.md` — corrections, consistency results, Open Questions, **PASS/PARTIAL/FAIL**.

## 8. Validation Rules
Per `Shared/VALIDATION.md`. Each change record: `change_id, type(ADDED/CORRECTED/ENRICHED), target_id, evidence, conf_before/after`.

## 9. Confidence Rules
Per `Shared/CONFIDENCE.md`. Material nodes: capability, business-rule.

## 10. Traceability Rules
Change records reference BR/capability IDs; Open Questions carry IDs feeding FN `open_questions`.

## 11. Governance Reference
Per `Shared/GOV.md` (role=VALIDATE, live_source=false, marker=DOCUMENT, audience=business).

## 12. Version Information
- 2.0.0 — 2026-06-24 — New standardized BA validate stage (two-prompt-per-layer); additive, review-only. — Prompt Architect
- supersedes: (none — additive; complements BA-EXTRACT)
- migration_ref: ../01_PROMPT_MERGE_REPORT.md#ba
