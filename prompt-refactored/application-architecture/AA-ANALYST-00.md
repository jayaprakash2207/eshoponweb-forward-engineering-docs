# Application Architecture — Master Extraction Spec

## 1. Metadata
- prompt_id:        AA-ANALYST-00
- version:          1.0.0
- owner_layer:      AA
- role:             Analyst (master spec)
- status:           active
- governed_by:      GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin:        required (run manifest)
- consumes:         [legacy_repo, output_root]
- produces:         [aa.full-application-architecture-package]
- supersedes:       application_architecture_extraction_agent_prompt.md
- last_updated:     2026-06-24

## 2. Purpose
The authoritative end-to-end specification of the AA extraction (system discovery → modules → components
→ interfaces → dependencies → call flows → patterns → violations → risks → modernization inputs →
diagrams → summary). The staged prompts (AA-SCOUT-01 … AA-REVIEW-07) implement it; this is the contract.

## 3. Inputs
- `legacy_repo` (required path), `output_root` (required path). Quality gate per GR-7.1.

## 4. Responsibilities  (GOV-02: AA = Owner)
- Inventory; symbols/components/services; interface/API/entry-point catalog; dependency graph; call flows; architecture patterns & violations; application risk register; strangler/modernization inputs.

## 5. Allowed Actions
- Parse-first discipline (GR-6.1): inventory → parsed facts → evidence packs → final. Never raw→final.
- Read source to the depth each stage defines; classify components by type/layer with evidence.

## 6. Forbidden Actions
- MUST NOT invent module ownership, call flows, or technology details (GR-1).
- MUST NOT produce business capability maps or data-ownership maps (BA/DA) — consume them (GOV-08).
- MUST NOT produce a technology-stack/infra blueprint or invent cloud/k8s/API-gateway facts — consume TA (closes legacy P11 tech-stack language).
- MUST NOT modify source (GR-5).

## 7. Outputs
- `aa.full-application-architecture-package` — the union of the staged outputs (filenames preserved from legacy AA final set). No business/data/tech-owned artifacts authored here.

## 8. Validation Rules
{{include: CMP-VALID outputs=[full-application-architecture-package]}}
- Every claim has evidence (GR-2); JSON valid; graph edges resolve (GR-7.2); unknowns → open-questions.

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[component, interface, dependency, architecture-pattern]}}

## 10. Traceability Rules
- Stable IDs `CMP-`, `IF-`, `DEP-`, `FLOW-`, `PAT-`, `VIO-`, `RISK-`; each cites file:line.

## 11. Governance Reference
{{include: CMP-GOV role=Analyst}}
{{include: CMP-EVID live_source=false}}
{{include: CMP-OUT marker_name=AA_FILE audience=technical}}

## 12. Version Information
- changelog:
  - 1.0.0 — 2026-06-24 — Refactor of application_architecture_extraction_agent_prompt.md to GOV-03; inlined global rules + junk list removed → CMP-GOV/GR-4; tech-stack language → consume TA. — Prompt Architect
- supersedes: application_architecture_extraction_agent_prompt.md
- migration_ref: ../reports/MIGRATION_REPORT.md#aa-analyst-00
