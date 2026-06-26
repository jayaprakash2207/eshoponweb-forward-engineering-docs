# Foundation / Synthesis — Canonical Model & Views

## 1. Metadata
- prompt_id:        FN-SYNTH-02
- version:          1.0.0
- owner_layer:      FN
- role:             Synthesis
- status:           active
- governed_by:      GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin:        required (run manifest)
- consumes:         [fn.enterprise-knowledge-graph]
- produces:         [fn.canonical-enterprise-model, fn.architecture-inventory, fn.traceability-matrix, fn.forward-engineering-input-map]
- supersedes:       (new — GOV-05)
- last_updated:     2026-06-24

## 2. Purpose
Render the canonical knowledge graph into read-only human-readable views: the canonical model, a flat
inventory, the traceability matrix, and the forward-engineering input map. Views only — no new facts.

## 3. Inputs
- `fn.enterprise-knowledge-graph` (required, from FN-SYNTH-01). Stop if graph invalid/incomplete (GR-7.1).

## 4. Responsibilities (GOV-02: FN = Owner)
- Canonical model generation; inventory projection; traceability-matrix construction; FE input-map projection.

## 5. Allowed Actions
- Project graph nodes/sections into markdown views; preserve confidence/status verbatim (GR-1.6).
- Build Capability→Process→Entity→Service→API chains from `cross_links`.

## 6. Forbidden Actions
- MUST NOT add, infer, or elevate any fact beyond the graph (FN-7 / GR-1).
- MUST NOT propose target design, code, or a future stack (forward-engineering artifacts are out of scope here).
- MUST NOT re-resolve conflicts (FN-SYNTH-01 owns resolution); surface residual ones as-is.

## 7. Outputs (marker: FN_FILE — filenames mirror the foundation package)
- `CANONICAL_ENTERPRISE_MODEL.md`, `ARCHITECTURE_INVENTORY.md`, `TRACEABILITY_MATRIX.md`, `FORWARD_ENGINEERING_INPUT_MAP.md`.
- Each is a **view** over the graph; the graph remains the single source of truth.

## 8. Validation Rules
{{include: CMP-VALID outputs=[canonical-enterprise-model, architecture-inventory, traceability-matrix, forward-engineering-input-map]}}
- Every view item carries its graph node ID; no item exists in a view without a graph node (GR-7.3).
- Traceability chains complete, or coverage gaps recorded as PARTIAL + Open Questions.

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[capability, entity, service, api]}}
- Confidence/status are copied verbatim from the graph; views never re-score (GR-1.6).

## 10. Traceability Rules
- Views cite graph node IDs only; the FE input map maps graph sections to FE inputs without inventing inputs.

## 11. Governance Reference
{{include: CMP-GOV role=Synthesis}}
{{include: CMP-EVID live_source=false}}
{{include: CMP-OUT marker_name=FN_FILE audience=technical}}

## 12. Version Information
- changelog:
  - 1.0.0 — 2026-06-24 — New Foundation view-generation prompt per GOV-05; produces canonical model + 3 views as graph projections. — Prompt Architect
- supersedes: (none — new)
- migration_ref: ../reports/MIGRATION_REPORT.md#fn-synth-02
