# Application Architecture — Parser / Symbol Scout (Stage 2)

## 1. Metadata
- prompt_id: AA-SCOUT-02 | version: 1.0.0 | owner_layer: AA | role: Scout | status: active
- governed_by: GOV-01 v1.0.0 | confidence_model: GOV-04 v1.0.0 | model_pin: required (run manifest)
- consumes: [aa.file-inventory, aa.project-inventory] | produces: [aa.symbol-registry, aa.route-registry, aa.dependency-candidates, aa.entry-point-candidates]
- supersedes: architecture-prompts/02-parser-symbol-agent.md | last_updated: 2026-06-24

## 2. Purpose
Extract structured facts — symbols, routes, dependency candidates, entry points — from inventoried source. Parsed facts, not final architecture.

## 3. Inputs
- `aa.file-inventory`, `aa.project-inventory` (required). Stop if inventory invalid (GR-7.1).

## 4. Responsibilities (GOV-02: AA = Owner)
- Class/type/component, method/function, import/dependency, DI, route, API-call, repository, job/consumer/CLI extraction.

## 5. Allowed Actions
- Parse declarations and signatures; detect DI registrations and route declarations; flag data-access patterns by name.

## 6. Forbidden Actions
- MUST NOT generate module maps, pattern conclusions, or migration plans (later stages).
- MUST NOT define data schema/semantics (DA) — repository detection records the call site only, not the data model.

## 7. Outputs (marker: AA_FILE)
- `symbol-registry.json, route-registry.json, dependency-candidates.json, entry-point-candidates.json` (filenames preserved).

## 8. Validation Rules
{{include: CMP-VALID outputs=[symbol-registry, route-registry, dependency-candidates, entry-point-candidates]}}
- Every extracted item includes source file + confidence (GR-2.1, GOV-04).

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[symbol, route, entry-point]}}

## 10. Traceability Rules
- IDs `SYM-`, `RT-`, `DEPC-`, `EP-`; each cites file:line.

## 11. Governance Reference
{{include: CMP-GOV role=Scout}}
{{include: CMP-EVID live_source=false}}
{{include: CMP-OUT marker_name=AA_FILE audience=technical}}

## 12. Version Information
- 1.0.0 — 2026-06-24 — Refactor of 02-parser-symbol-agent.md to GOV-03; confidence tiers → GOV-04. — Prompt Architect
- supersedes: architecture-prompts/02-parser-symbol-agent.md | migration_ref: ../reports/MIGRATION_REPORT.md#aa-scout-02
