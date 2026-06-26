# Business Architecture — Layer 2 Business Analysis (JSON contract)

## 1. Metadata
- prompt_id:        BA-ANALYST-02
- version:          1.0.0
- owner_layer:      BA
- role:             Analyst
- status:           active
- governed_by:      GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin:        required (run manifest)
- consumes:         [layer1.source_code, layer1.database, layer1.config, layer1.extraction_summary]
- produces:         [ba.layer2_output]   # business_rules, business_entities(ref), process_sequences, user_roles, capability_candidates
- supersedes:       layer2/layer2_prompt.md
- last_updated:     2026-06-24

## 2. Purpose
Produce the structured JSON business-analysis object (`layer2_output.json`) from Layer 1 extracts —
the machine-readable feed BA-ANALYST-03 renders into business documents.

## 3. Inputs
- `layer1.source_code` (required) — methods flagged `is_business_artifact: true` (Layer 1 heuristic = hint only, not authority).
- `layer1.database` (required) — used only to **reference** entities (see Forbidden Actions).
- `layer1.config` (required) — business params, feature flags, role definitions.
- `layer1.extraction_summary` (required).
- Quality gate (GR-7.1): stop if `source_code` is empty.

## 4. Responsibilities  (GOV-02: BA = Owner)
- Business rules (semantic) — BA is the single owner (GOV-02 §3; removes the ×4 duplication).
- Process sequences, user-role profiles, capability candidates (business framing).

## 5. Allowed Actions
- Extract IF/THEN, validation, calculation, approval logic from business methods → business statements.
- Group methods/classes into capability candidates.
- Reference entities and config keys for context (by name/ID), citing the owner.

## 6. Forbidden Actions
- MUST NOT author data-model relationships/cardinality — emit `business_entities` as **references to DA** entity IDs (consume-and-cite, GOV-08); do not define schema here.
- MUST NOT classify technology/config-management as a deliverable — config keys are referenced, owned by TA.
- MUST NOT translate controller/module access into an application-architecture artifact — that is AA; record only the business "what a role can do".
- MUST NOT define a local confidence scale.

## 7. Outputs  (JSON — exact schema preserved from legacy P4 for runner compatibility)
- `ba.layer2_output` = `{ analysis_metadata, business_rules[], business_entities[](references), process_sequences[], user_roles[], capability_candidates[] }`.
- `business_rules[]` keep fields: `rule_id (BR###), rule_name, business_statement, category, priority, source_method, source_file, config_driven, config_key`.
- `business_entities[]` now carry `da_entity_ref` + `confidence` instead of locally-invented relationships.

## 8. Validation Rules
{{include: CMP-VALID outputs=[layer2_output]}}
- Output JSON parses (GR-7.2); every `business_statement` is plain English; every rule cites `source_file`.
- `business_entities[]` either cite a DA id or are marked `unknown` + Open Question if DA unavailable.

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[business-rule, capability_candidate]}}

## 10. Traceability Rules
- `BR###` IDs stable and sequential (GR-6.2); reused by BA-ANALYST-03 and Foundation.
- Entity references use DA owner IDs (GR-3.4), enabling Foundation cross-links.

## 11. Governance Reference
{{include: CMP-GOV role=Analyst}}
{{include: CMP-EVID live_source=false}}
{{include: CMP-OUT marker_name=JSON audience=business}}

## 12. Version Information
- changelog:
  - 1.0.0 — 2026-06-24 — Refactor of layer2_prompt.md to GOV-03; business_entities relationships → DA references; sole owner of business-rule extraction; governance/confidence externalized. — Prompt Architect
- supersedes: layer2/layer2_prompt.md
- migration_ref: ../reports/MIGRATION_REPORT.md#ba-analyst-02
