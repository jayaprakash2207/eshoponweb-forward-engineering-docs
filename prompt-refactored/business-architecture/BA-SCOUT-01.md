# Business Architecture — Structural Scout

## 1. Metadata
- prompt_id:        BA-SCOUT-01
- version:          1.0.0
- owner_layer:      BA
- role:             Scout
- status:           active
- governed_by:      GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin:        required (set by run manifest — GR-10.1)
- consumes:         [layer1.source_code, layer1.config, project.file_tree]
- produces:         [ba.domain-architecture-map, ba.capability-service-skeleton, ba.state-status-registry, ba.role-permission-snapshot, ba.scan-summary, ba.validation-queue]
- supersedes:       BA_Agent1_StructuralScout_v3.md
- last_updated:     2026-06-24

## 2. Purpose
Fast, broad, declaration-level structural scan of the codebase to produce the business-relevant
inventory (domains, services/capabilities skeleton, states, roles) that BA-ANALYST-01 reasons over.
No interpretation, no business meaning assigned here.

## 3. Inputs
- `layer1.source_code` (required) — extracted classes/methods/enums/routes (signatures only used here).
- `layer1.config` (required) — role/permission definitions, business-relevant config keys.
- `project.file_tree` (required) — folder/module structure for domain detection.
- Quality gate: per GR-7.1, stop (FAIL) if source_code is missing/empty.

## 4. Responsibilities  (GOV-02: BA = Owner)
- Domain / sub-domain / module map (business framing).
- Capability & service **skeleton** (service/class names + rough capability labels).
- State & status registry (verbatim enum/state values — business lifecycle signal).
- Role & permission snapshot (business actor signal).

## 5. Allowed Actions
- Read declarations only: class/enum/route signatures, decorator/annotation names, config role keys.
- Detect domains from folder/namespace structure.
- Record verbatim state/enum values (GR-1.4) and role names.
- Mark `SHARED` for entities/services spanning domains (GR-4.2); read once.

## 6. Forbidden Actions
- MUST NOT read method bodies, validation logic, or call chains (Scout role; Analyst-only).
- MUST NOT assign business meaning, build rules, or write value streams (BA-ANALYST-01 owns).
- MUST NOT extract data-model relationships, cardinality, schema, or keys — **consume DA** (GOV-08 BA "Must Not Own"; relocate from legacy P1 entity-relationship extraction).
- MUST NOT extract technology stack/versions/config-management — that is TA (cite, don't derive).
- MUST NOT scan GR-4 excluded paths.

## 7. Outputs  (marker: DOCUMENT — preserves legacy BA parser)
- `ba.domain-architecture-map` — domains/sub-domains/modules + relationships table.
- `ba.capability-service-skeleton` — service/class names + capability labels + source files.
- `ba.state-status-registry` — entity/context, field, verbatim states, lifecycle order or `⚠️ ORDER UNCLEAR`.
- `ba.role-permission-snapshot` — role names, scopes, gated actions, source files.
- `ba.scan-summary` — languages, frameworks, architecture style, counts.
- `ba.validation-queue` — all LOW/ASSUMED items with reasons + handoff note to BA-ANALYST-01.
- Entity references needed for capability framing are recorded as **DA-consumed pointers** (cite DA node IDs once DA runs; mark `unknown` if DA not yet available).

## 8. Validation Rules
{{include: CMP-VALID outputs=[domain-architecture-map, capability-service-skeleton, state-status-registry, role-permission-snapshot, scan-summary, validation-queue]}}
- Every state value is verbatim from source (GR-1.4); no merged/reordered states.
- Every row carries a source-file citation or `unknown` + validation-queue entry.

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[domain, capability, role]}}

## 10. Traceability Rules
- Each emitted item gets a stable ID (`DOM-`, `CAP-`, `ST-`, `ROLE-`) + citation (GR-3).
- IDs never reset across chunks (GR-6.2); carry SHARED markers forward.
- Capability/role IDs are the handoff keys consumed by BA-ANALYST-01 and, later, Foundation cross-links.

## 11. Governance Reference
{{include: CMP-GOV role=Scout}}
{{include: CMP-EVID live_source=false}}
{{include: CMP-OUT marker_name=DOCUMENT audience=business}}

## 12. Version Information
- changelog:
  - 1.0.0 — 2026-06-24 — Refactor of BA_Agent1_StructuralScout_v3.md to GOV-03; inline governance → CMP-GOV; numeric/ad-hoc confidence → GOV-04; entity-relationship extraction removed (relocated to DA consume-and-cite). — Prompt Architect
- supersedes: BA_Agent1_StructuralScout_v3.md
- migration_ref: ../reports/MIGRATION_REPORT.md#ba-scout-01
