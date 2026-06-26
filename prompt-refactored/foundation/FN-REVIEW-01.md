# Foundation / Synthesis — Reconciliation Review & Gate

## 1. Metadata
- prompt_id:        FN-REVIEW-01
- version:          1.0.0
- owner_layer:      FN
- role:             Review
- status:           active
- governed_by:      GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin:        required (run manifest)
- consumes:         [fn.enterprise-knowledge-graph, fn.canonical-enterprise-model, fn.traceability-matrix, fn.architecture-inventory, fn.forward-engineering-input-map]
- produces:         [fn.reconciliation-report]
- supersedes:       (new — GOV-05)
- last_updated:     2026-06-24

## 2. Purpose
Validate the synthesized graph and views for ownership correctness, traceability coverage, confidence
integrity, and determinism; emit the reconciliation report and a Gate verdict before forward-engineering.

## 3. Inputs
- `fn.enterprise-knowledge-graph` + the four views (required). Stop if any is missing/invalid (GR-7.1).

## 4. Responsibilities (GOV-02: FN = Owner, Review role)
- Schema completeness, owner correctness, orphan-link detection, confidence integrity, traceability coverage, determinism check, conflict cataloging.

## 5. Allowed Actions
- Cross-check views against the graph; verify each node's `owner_layer` = GOV-02 owner; list residual DISCREPANCYs.
- Confirm the run manifest pins model + prompt/component versions (GR-10).

## 6. Forbidden Actions
- MUST NOT modify the graph or views (Review role).
- MUST NOT resolve conflicts itself (that is FN-SYNTH-01); only report them.
- MUST NOT introduce new facts (GR-1).

## 7. Outputs (marker: FN_FILE)
- `reconciliation-report.md` — conflicts + resolutions, residual DISCREPANCYs, ownership-mismatch flags, traceability coverage, and a **Gate verdict (PASS/PARTIAL/FAIL — GOV-04 §5)** gating the forward-engineering stage.

## 8. Validation Rules
{{include: CMP-VALID outputs=[reconciliation-report]}}
- All 9 graph sections present; no orphan `cross_links`; no node with owner ≠ GOV-02 owner unflagged.
- Confidence never silently raised across synthesis (GR-1.6 / FN-5); determinism asserted (GR-10.3).

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[capability, entity, component, technology, security-control]}}

## 10. Traceability Rules
- Each reported item references graph node IDs and the GOV rule applied; unresolved items remain in `open_questions`.

## 11. Governance Reference
{{include: CMP-GOV role=Review}}
{{include: CMP-EVID live_source=false}}
{{include: CMP-OUT marker_name=FN_FILE audience=technical}}

## 12. Version Information
- changelog:
  - 1.0.0 — 2026-06-24 — New Foundation review/gate prompt per GOV-05; gates forward-engineering on reconciled, owner-correct, traceable canonical model. — Prompt Architect
- supersedes: (none — new)
- migration_ref: ../reports/MIGRATION_REPORT.md#fn-review-01
