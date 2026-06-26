# BA-EXTRACT — Business Architecture Extraction

## 1. Metadata
- prompt_id:        BA-EXTRACT
- version:          2.0.0
- owner_layer:      BA
- role:             EXTRACT (Scout→Analyst, internally phased)
- status:           active
- governed_by:      GOV (V2-GOV) v2.0.0 → GOV-01
- confidence_model: CONFIDENCE (V2-CONF) v2.0.0 → GOV-04
- model_pin:        required (run manifest)
- consumes:         [layer1.source_code, layer1.database, layer1.config, layer1.extraction_summary]
- produces:         [ba.layer2_output, ba.10-business-documents]
- merges:           [BA-ANALYST-02, BA-ANALYST-03]   (+ folds BA-SCOUT-01 scout discipline as Phase A)
- last_updated:     2026-06-24

## 2. Purpose
Discover, analyze, and produce the Business Architecture for the project in one governed prompt — from
structural discovery through business documents — preserving the parse-first phasing of the legacy
layer2→layer3 lineage.

## 3. Inputs
- `layer1.source_code` (required), `layer1.database`, `layer1.config`, `layer1.extraction_summary`.
- Quality gate (GR-7.1): stop if `source_code` is empty.

## 4. Responsibilities  (GOV-02: BA = Owner)
- Business rules (sole owner), capabilities, processes/value streams, stakeholders/roles, the 10 BA documents.

## 5. Allowed Actions — MANDATORY PHASES (parse-first, GR-6.1)
> The merge is at the authoring surface; execution MUST remain phased. Each phase emits its artifact before the next.
- **Phase A — Structural Scout** (declaration-level): scan domains, services, states, roles; no interpretation.
- **Phase B — Analysis → JSON**: extract business rules (IF/THEN, validation, approval), entities **as DA references** (`da_entity_ref`), processes, user roles, capability candidates → emit **`layer2_output.json`** (unchanged schema). *Must emit before Phase C.*
- **Phase C — Document generation**: render `layer2_output.json` into the **10 BA documents**; mark INFERRED docs; data-model doc is a DA-sourced view.

## 6. Forbidden Actions
- MUST NOT author schema/cardinality (DA owns) — entities are DA references (GOV-08 BA Must-Not-Own).
- MUST NOT author call-flow topology/components (AA) or tech/NFR/infra (TA) — consume + cite.
- MUST NOT skip Phase B's `layer2_output.json` and jump source→documents (GR-6.1 violation).
- MUST NOT invent BR references in documents — every BR must exist in `layer2_output.json` (GR-7.3).

## 7. Outputs  (markers preserve legacy parsers)
- `ba.layer2_output` → `layer2_output.json` (JSON marker; schema unchanged; `business_entities[]` carry `da_entity_ref`).
- `ba.10-business-documents` → `ba_documents/01_capability_map.md … 10_business_roadmap.md` (DOCUMENT marker `===DOCUMENT_START:<file>===`).

## 8. Validation Rules
Per `Shared/VALIDATION.md` (GR-7). Parse-first integrity: `layer2_output.json` emitted before documents.
Exactly 10 documents, delimiter-wrapped; every BR reference resolves.

## 9. Confidence Rules
Per `Shared/CONFIDENCE.md`. Material nodes: capability, business-rule, value-stream.

## 10. Traceability Rules
`BR###` IDs sequential, never reset (GR-6.2); capability/role IDs feed FN cross-links; entity refs use DA owner IDs.

## 11. Governance Reference
Per `Shared/GOV.md` (role=EXTRACT, live_source=false, markers=JSON+DOCUMENT, audience=business).

## 12. Version Information
- 2.0.0 — 2026-06-24 — Merge of BA-ANALYST-02 + BA-ANALYST-03 into a phased EXTRACT; folds BA-SCOUT-01 discipline as Phase A; outputs (layer2_output.json + 10 docs) unchanged. — Prompt Architect
- supersedes: BA-ANALYST-02, BA-ANALYST-03 (and the unwired BA-SCOUT-01/BA-ANALYST-01)
- migration_ref: ../01_PROMPT_MERGE_REPORT.md#ba
