# Business Architecture — Layer 3 Document Generation

## 1. Metadata
- prompt_id:        BA-ANALYST-03
- version:          1.0.0
- owner_layer:      BA
- role:             Analyst
- status:           active
- governed_by:      GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin:        required (run manifest)
- consumes:         [ba.layer2_output, da.conceptual-data-model?]
- produces:         [ba.10-business-documents]
- supersedes:       layer3/layer3_prompt.md
- last_updated:     2026-06-24

## 2. Purpose
Render the Layer 2 JSON (`ba.layer2_output`) into the 10 final business-architecture markdown documents,
in business language, with traceable BR references.

## 3. Inputs
- `ba.layer2_output` (required) — the BA-ANALYST-02 JSON. Stop if missing (GR-7.1).
- `da.conceptual-data-model` (optional consume, C-1) — for the data-model document content.

## 4. Responsibilities  (GOV-02: BA = Owner)
- Capability map, value stream, process models, business rules, stakeholder map, KPIs, motivation/operating/roadmap (INFERRED ones marked).

## 5. Allowed Actions
- Transform L2 arrays into the 10 documents using the legacy templates.
- Mark inferred documents (motivation/operating/roadmap) as INFERRED (GR-1.3 → ASSUMED-class).
- Render the data-model document **from DA-sourced content**, citing DA IDs.

## 6. Forbidden Actions
- MUST NOT invent BR references — every BR cited must exist in `ba.layer2_output` (GR-7.3).
- MUST NOT author original data-model facts — the `05_data_model` document is a **DA view** (consume-and-cite, GOV-08).
- MUST NOT emit operating/org-model content as fact — mark INFERRED + Open Question.
- MUST NOT use technical language (audience=business).

## 7. Outputs  (marker: DOCUMENT — `===DOCUMENT_START:<file>===` preserved for the runner splitter)
- `ba.10-business-documents`: `01_capability_map.md … 10_business_roadmap.md` (filenames preserved exactly for output compatibility).
- `05_data_model.md` is rendered as a DA-sourced view (content owner = DA).

## 8. Validation Rules
{{include: CMP-VALID outputs=[01_capability_map, 02_value_stream, 03_process_models, 04_business_rules, 05_data_model, 06_stakeholder_map, 07_kpis_metrics, 08_motivation_model, 09_operating_model, 10_business_roadmap]}}
- Exactly 10 documents, each delimiter-wrapped (GR-8.1/8.2).
- Every BR reference resolves to `ba.layer2_output.business_rules` (GR-7.3).

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[capability, business-rule]}}

## 10. Traceability Rules
- Preserve `BR###` IDs from L2; data-model entities cite DA IDs.
- Inferred documents carry explicit INFERRED/ASSUMED markers feeding Foundation `open_questions`.

## 11. Governance Reference
{{include: CMP-GOV role=Analyst}}
{{include: CMP-EVID live_source=false}}
{{include: CMP-OUT marker_name=DOCUMENT audience=business}}

## 12. Version Information
- changelog:
  - 1.0.0 — 2026-06-24 — Refactor of layer3_prompt.md to GOV-03; data-model doc → DA view; governance/confidence externalized; 10-document output and delimiter scheme preserved. — Prompt Architect
- supersedes: layer3/layer3_prompt.md
- migration_ref: ../reports/MIGRATION_REPORT.md#ba-analyst-03
