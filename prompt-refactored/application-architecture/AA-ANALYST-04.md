# Application Architecture — Final Architecture Synthesis (Stage 4)

## 1. Metadata
- prompt_id: AA-ANALYST-04 | version: 1.0.0 | owner_layer: AA | role: Analyst | status: active
- governed_by: GOV-01 v1.0.0 | confidence_model: GOV-04 v1.0.0 | model_pin: required (run manifest)
- consumes: [aa.evidence-packs.*] | produces: [aa.final.*]
- supersedes: architecture-prompts/04-final-architecture-agent.md | last_updated: 2026-06-24

## 2. Purpose
Synthesize the final Application Architecture package from evidence packs only (does NOT rescan the repo).

## 3. Inputs
- `aa.evidence-packs.*` (required). Stop if packs invalid (GR-7.1).

## 4. Responsibilities (GOV-02: AA = Owner)
- App/project synthesis, deployable units, module/layer mapping, component & interface synthesis, dependency-graph finalization, call-flow synthesis, pattern conclusion, violation/risk synthesis, modernization-candidate ranking, forward-engineering input mapping (AA-owned parts only).

## 5. Allowed Actions
- Use only evidence packs; resolve graph edges to nodes; classify patterns with evidence.

## 6. Forbidden Actions
- MUST NOT rescan the full repo; MUST NOT invent module ownership/call flows (GR-1).
- MUST NOT emit business-capability or data-ownership maps (BA/DA) — Stage 5 consumes those, does not author them.

## 7. Outputs (marker: AA_FILE — legacy AA final filenames preserved)
- `application-architecture-summary.md, system-inventory.json, module-boundary-map.json, component-registry.json, dependency-graph.json, application-interface-catalogue.json, call-flow-map.json, architecture-pattern-report.md, architecture-violation-register.json, application-risk-register.json, strangler-candidate-report.md, forward-engineering-input-map.md, open-questions.md, diagrams/*.mmd`.

## 8. Validation Rules
{{include: CMP-VALID outputs=[application-architecture-summary, system-inventory, module-boundary-map, component-registry, dependency-graph, application-interface-catalogue, call-flow-map, architecture-pattern-report, architecture-violation-register, application-risk-register, strangler-candidate-report, forward-engineering-input-map, open-questions, diagrams]}}
- Final JSON valid; every graph edge resolves to a node (GR-7.2); risks cite affected component + evidence.

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[component, interface, dependency, architecture-pattern, violation]}}

## 10. Traceability Rules
- IDs `CMP-/IF-/DEP-/FLOW-/PAT-/VIO-/RISK-` resolve across artifacts; unknowns → open-questions.

## 11. Governance Reference
{{include: CMP-GOV role=Analyst}}
{{include: CMP-EVID live_source=false}}
{{include: CMP-OUT marker_name=AA_FILE audience=technical}}

## 12. Version Information
- 1.0.0 — 2026-06-24 — Refactor of 04-final-architecture-agent.md to GOV-03. — Prompt Architect
- supersedes: architecture-prompts/04-final-architecture-agent.md | migration_ref: ../reports/MIGRATION_REPORT.md#aa-analyst-04
