# TA-VALIDATE — Technology Architecture Validation

## 1. Metadata
- prompt_id:        TA-VALIDATE
- version:          2.0.0
- owner_layer:      TA
- role:             VALIDATE
- status:           active
- governed_by:      GOV (V2-GOV) v2.0.0 → GOV-01
- confidence_model: CONFIDENCE (V2-CONF) v2.0.0 → GOV-04
- model_pin:        required (run manifest)
- consumes:         [ta.* (all TA-EXTRACT artifacts)]
- produces:         [ta.validated-artifacts, ta.review-summary]
- last_updated:     2026-06-24

## 2. Purpose
Validate the Technology Architecture and infrastructure: confirm stack/version/EOL claims, NFR thresholds,
and infra/transport security are evidence-backed; resolve LOW/ASSUMED; emit a Gate verdict.

> Note: legacy TA had a `ta-review-summary.md` produced inside the analyst stage but **no separate review
> prompt**. TA-VALIDATE formalizes that summary into the standardized validate slot — additive,
> review-only (GR-1), so it cannot change TA outputs, only gate them.

## 3. Inputs
- All `ta.*` artifacts from TA-EXTRACT (required). Stop if incomplete (GR-7.1).

## 4. Responsibilities  (GOV-02: TA = Owner, VALIDATE role)
- Consistency checks (assessments trace to inventory IDs); EOL/version confirmation; NFR threshold verification;
  confidence escalation with evidence; Gate verdict.

## 5. Allowed Actions
- Cross-check assessments vs the 6 inventory files; verify each NFR/pattern citation; resolve conflicts by rank (GR-2.4).
- Raise confidence only with new evidence + change record (GR-9).

## 6. Forbidden Actions
- MUST NOT extract new technology facts (review only); MUST NOT assess data semantics (DA) or app security (AA).
- MUST NOT use a local verdict vocabulary (use CONFIDENCE §5).

## 7. Outputs  (TA_FILE marker)
- `ta.validated-artifacts` — TA artifact set with change records appended.
- `ta.review-summary` → `ta-review-summary.md` (filename preserved) — corrections, consistency, Open Questions, **PASS/PARTIAL/FAIL**.

## 8. Validation Rules
Per `Shared/VALIDATION.md`. Each change record: `change_id, type, target_id, evidence, conf_before/after`.

## 9. Confidence Rules
Per `Shared/CONFIDENCE.md`. Material nodes: technology, nfr, technical-debt, security-control.

## 10. Traceability Rules
Change records reference `TECH-/NFR-/TD-` IDs; Open Questions feed FN `open_questions`.

## 11. Governance Reference
Per `Shared/GOV.md` (role=VALIDATE, live_source=false, marker=TA_FILE, audience=technical).

## 12. Version Information
- 2.0.0 — 2026-06-24 — New standardized TA validate stage formalizing ta-review-summary; additive, review-only. — Prompt Architect
- supersedes: (formalizes ta-review-summary; complements TA-EXTRACT)
- migration_ref: ../01_PROMPT_MERGE_REPORT.md#ta
