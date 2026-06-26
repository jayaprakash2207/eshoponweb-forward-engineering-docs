# DA-EXTRACT — Data Architecture Extraction

## 1. Metadata
- prompt_id:        DA-EXTRACT
- version:          2.0.0
- owner_layer:      DA
- role:             EXTRACT (Scout→Analyst, internally phased)
- status:           active
- governed_by:      GOV (V2-GOV) v2.0.0 → GOV-01
- confidence_model: CONFIDENCE (V2-CONF) v2.0.0 → GOV-04
- model_pin:        required (run manifest)
- consumes:         [layer1.database, layer1.config, layer1.source_code, live.database?]
- produces:         [da.schema-catalogue, da.data-source-inventory, da.pii-inventory, da.migration-complexity, da.conceptual-data-model, da.erd, da.data-dictionary, da.data-flow-map, da.data-quality-report, da.storage-pattern-analysis, da.redundancy-analysis, da.access-control-matrix, da.hidden-business-rules, da.data-ownership-map, da.datastore-transaction-consistency-assessment]
- merges:           [DA-SCOUT-01, DA-ANALYST-01]   (restores original single-prompt contract)
- last_updated:     2026-06-24

## 2. Purpose
Discover entities, analyze relationships, and produce the Data Architecture in one governed prompt —
restoring the original single-prompt/single-runner contract (`DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md`).

## 3. Inputs
- `layer1.database`, `layer1.config`, `layer1.source_code` (required); `live.database` (optional, rank-1).
- Quality gate (GR-7.1): stop if no entity/migration/schema evidence.

## 4. Responsibilities  (GOV-02: DA = sole owner of data-store/entity/schema discovery)
- Schema/source/PII/migration inventory; conceptual model, ERD, dictionary, data-flow, quality, storage,
  redundancy, access-control, hidden data-layer rules (tagged for BA); **data-ownership-map**;
  **datastore-transaction-consistency-assessment**.

## 5. Allowed Actions — MANDATORY PHASES (parse-first)
- **Phase A — Discovery (Scout)**: entity/model classes, migrations, repositories → schema-catalogue, data-source-inventory, pii-inventory, migration-complexity. Attempt live DB (live_source=true); on failure record exact error+command.
- **Phase B — Analysis (Analyst)**: relationships, transaction/consistency, ownership, quality → the remaining deliverables. *Phase A artifacts emitted first.*

## 6. Forbidden Actions
- MUST NOT assign business meaning/own business rules (BA) — tag data-layer rules `for_ba` and hand off.
- MUST NOT assess application components (AA) or tech-stack/infra as deliverables (TA) — engine/version is a data-source attribute only.
- MUST NOT capture secret values (GR-5.4); MUST NOT skip Phase A inventory (GR-6.1).

## 7. Outputs  (DA_FILE marker; legacy filenames preserved)
- 13 legacy files: `schema-catalogue.json, data-source-inventory.json, pii-inventory.json, migration-complexity.json, conceptual-data-model.md, erd.md, data-dictionary.md, data-flow-map.md, data-quality-report.md, storage-pattern-analysis.md, redundancy-analysis.json, access-control-matrix.md, hidden-business-rules.json`
- + relocated-in: `data-ownership-map.md`, `datastore-transaction-consistency-assessment.md`.

## 8. Validation Rules
Per `Shared/VALIDATION.md`. ERD entities resolve to schema IDs; live-confirmed=HIGH, naming-only=LOW.

## 9. Confidence Rules
Per `Shared/CONFIDENCE.md`. Material nodes: entity, data-store, data-owner, pii-field.

## 10. Traceability Rules
Stable IDs `TBL-/ENT-/DS-/PII-/OWN-/DR-`, each cited; consumed by DA-VALIDATE, BA, AA, TA, FN.

## 11. Governance Reference
Per `Shared/GOV.md` (role=EXTRACT, live_source=true, marker=DA_FILE, audience=technical).

## 12. Version Information
- 2.0.0 — 2026-06-24 — Merge of DA-SCOUT-01 + DA-ANALYST-01 (restores original 1-prompt contract). — Prompt Architect
- supersedes: DA-SCOUT-01, DA-ANALYST-01
- migration_ref: ../01_PROMPT_MERGE_REPORT.md#da
