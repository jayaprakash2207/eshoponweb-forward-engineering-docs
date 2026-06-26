# Application Architecture — Evidence Pack Builder (Stage 3)

## 1. Metadata
- prompt_id: AA-ANALYST-03 | version: 1.0.0 | owner_layer: AA | role: Analyst | status: active
- governed_by: GOV-01 v1.0.0 | confidence_model: GOV-04 v1.0.0 | model_pin: required (run manifest)
- consumes: [aa.inventory.*, aa.parsed.*] | produces: [aa.evidence-packs.*]
- supersedes: architecture-prompts/03-evidence-pack-agent.md | last_updated: 2026-06-24

## 2. Purpose
Convert inventory + parsed facts into technology-agnostic evidence packs — the intermediate representation before final synthesis (GR-6.1 parse-first discipline).

## 3. Inputs
- `aa.inventory.*` + `aa.parsed.*` (required). Stop if parsed outputs invalid/missing keys (GR-7.1).

## 4. Responsibilities (GOV-02: AA = Owner)
- System-inventory, module-boundary, component-registry, dependency, entry-point, call-flow, layering-pattern, external-boundary, frontend packs.

## 5. Allowed Actions
- Group/organize parsed evidence into packs; preserve source evidence + confidence; mark partial call flows partial (GR-6.4).

## 6. Forbidden Actions
- MUST NOT write final architecture conclusions or invent module responsibilities (Stage 4).
- MUST NOT pull in business/data/tech-owned facts as originals — reference owners if needed.

## 7. Outputs (marker: AA_FILE)
- 9 evidence packs (filenames preserved): `system-inventory-pack, module-boundary-pack, component-registry-pack, dependency-pack, entry-point-pack, call-flow-pack, layering-pattern-pack, external-boundary-pack, frontend-application-pack`.

## 8. Validation Rules
{{include: CMP-VALID outputs=[system-inventory-pack, module-boundary-pack, component-registry-pack, dependency-pack, entry-point-pack, call-flow-pack, layering-pattern-pack, external-boundary-pack, frontend-application-pack]}}
- `unknown` used where evidence insufficient (GR-1.2); partials marked.

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[module-boundary, component, call-flow]}}

## 10. Traceability Rules
- Packs preserve upstream IDs (`SYM-/RT-/DEPC-/EP-`); add `MBP-`, `CFP-`.

## 11. Governance Reference
{{include: CMP-GOV role=Analyst}}
{{include: CMP-EVID live_source=false}}
{{include: CMP-OUT marker_name=AA_FILE audience=technical}}

## 12. Version Information
- 1.0.0 — 2026-06-24 — Refactor of 03-evidence-pack-agent.md to GOV-03. — Prompt Architect
- supersedes: architecture-prompts/03-evidence-pack-agent.md | migration_ref: ../reports/MIGRATION_REPORT.md#aa-analyst-03
