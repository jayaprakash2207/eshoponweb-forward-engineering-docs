# FN-VALIDATE — Foundation Validation

## 1. Metadata
- prompt_id:        FN-VALIDATE
- version:          2.0.0
- owner_layer:      FN
- role:             VALIDATE
- status:           active
- governed_by:      GOV (V2-GOV) v2.0.0 → GOV-01
- confidence_model: CONFIDENCE (V2-CONF) v2.0.0 → GOV-04
- model_pin:        required (run manifest)
- consumes:         [fn.enterprise-knowledge-graph, fn.canonical-enterprise-model, fn.architecture-inventory, fn.traceability-matrix, fn.forward-engineering-input-map, run.manifest]
- produces:         [fn.reconciliation-report]
- renamed_from:     FN-REVIEW-01
- last_updated:     2026-06-24

## 2. Purpose
Validate the knowledge graph, traceability, cross-layer links, and integrity; produce the final Enterprise
Architecture sign-off (the Gate before Forward Engineering). (1:1 rename of FN-REVIEW-01.)

## 3. Inputs
- The graph + 4 views from FN-SYNTHESIZE + `run.manifest` (required). Stop if any missing/invalid (GR-7.1).

## 4. Responsibilities  (GOV-02: FN = Owner, VALIDATE role)
- Schema completeness (9 sections); owner correctness; orphan-link detection; confidence integrity;
  traceability coverage; determinism check; conflict cataloging; model/version-pin check (GR-10).

## 5. Allowed Actions
- Cross-check views vs graph; verify each node's `owner_layer` = GOV-02 owner; list residual DISCREPANCYs.
- Verify Capability→Process→Entity→Service→API chains complete; confirm manifest pins (GR-10).

## 6. Forbidden Actions
- MUST NOT modify the graph or views (Review role); MUST NOT resolve conflicts (FN-SYNTHESIZE owns that) — only report.
- MUST NOT introduce new facts (GR-1).

## 7. Outputs  (FN_FILE marker)
- `reconciliation-report.md` — conflicts + resolutions, residual DISCREPANCYs, ownership-mismatch flags,
  traceability coverage, and a **Gate verdict (PASS/PARTIAL/FAIL — CONFIDENCE §5)** gating Forward Engineering.

## 8. Validation Rules
Per `Shared/VALIDATION.md`. All 9 sections; no orphan cross-links; no node with owner ≠ GOV-02 owner unflagged; confidence never silently raised (FN-5); determinism (GR-10.3).

## 9. Confidence Rules
Per `Shared/CONFIDENCE.md`. Material nodes: capability, entity, component, technology, security-control.

## 10. Traceability Rules
Each reported item references graph node IDs and the GOV rule applied; unresolved items remain in `open_questions`.

## 11. Governance Reference
Per `Shared/GOV.md` (role=VALIDATE, live_source=false, marker=FN_FILE, audience=technical).

## 12. Version Information
- 2.0.0 — 2026-06-24 — Rename of FN-REVIEW-01 to the standardized VALIDATE slot; behavior unchanged. — Prompt Architect
- supersedes: FN-REVIEW-01
- migration_ref: ../01_PROMPT_MERGE_REPORT.md#fn
