# Compatibility Report

**Date:** 2026-06-24
**Question answered:** Will downstream consumers (runners' output parsers, the eShopOnWeb artifact tree,
the foundation/forward-engineering packages) still work after cutover?

Verdict legend: ✅ fully compatible · ⚠️ compatible with a noted contract change · 🔴 location/shape change requiring consumer repoint.

---

## 1. Output-capture mechanism compatibility

Each runner parses model output via either a marker scheme or direct file writes. The governed prompts
preserve these via `CMP-OUT` legacy-marker compatibility.

| Runner | Legacy capture | Governed prompt marker | Status |
|---|---|---|---|
| layer2 | JSON block / fallback markers | `BA-ANALYST-02` marker_name=JSON; same `layer2_output.json` | ✅ |
| layer3 | `===DOCUMENT_START:<file>===` split | `BA-ANALYST-03` marker_name=DOCUMENT (identical) | ✅ |
| da_agent1 | writes 13 files to `da-outputs/` | `DA-SCOUT-01`/`DA-ANALYST-01` marker_name=DA_FILE; same filenames | ✅ (+2 additive) |
| da_agent2 | updates in place + `review-summary.md` | `DA-REVIEW-01` marker_name=DA_FILE | ✅ |
| ta_agent1 | `===TA_FILE_START===` | `TA-SCOUT-01` marker_name=TA_FILE (identical) | ✅ |
| ta_agent2 | `===TA_FILE_START===` | `TA-ANALYST-01` marker_name=TA_FILE | ⚠️ markers same; **2 files relocate** (see C-09) |
| aa_runner | `===AA_FILE_START===` | governed AA marker_name=AA_FILE (identical) | ⚠️ markers same; **2 files relocate** (see C-17) |

---

## 2. Per-output compatibility cards

### C-04 · `layer2_output.json`
- **Top-level schema:** ✅ unchanged keys (`analysis_metadata, business_rules[], business_entities[], process_sequences[], user_roles[], capability_candidates[]`).
- **Element change:** `business_entities[]` now carry `da_entity_ref` + `confidence` rather than `relationship_type/target_entity/cardinality`.
- **Consumer impact:** `layer3_runner.py` consumes the JSON object and re-renders; it does not depend on entity-relationship internals → ✅. Any external consumer reading entity `relationships` must read `da_entity_ref` → ⚠️.
- **Status:** ⚠️ compatible with documented entity-shape contract change.

### C-05 · BA 10 documents
- Filenames `01_capability_map.md … 10_business_roadmap.md` and `===DOCUMENT_*===` delimiters preserved. `05_data_model.md` content is now a DA-sourced view (same file, sourced upstream). **Status: ✅** (filename/marker identical; provenance noted).

### C-06 · DA 13-file set
- All 13 legacy filenames preserved in `da-outputs/`. Two **additive** files arrive: `data-ownership-map.md` (from AA) and `datastore-transaction-consistency-assessment.md` (from TA). The da_agent1 retry/validation keyed on the 13-file set remains satisfied. **Status: ✅ (superset).**

### C-08 · TA 6 inventory files
- `technology-stack-inventory.md, component-service-map.md, data-store-registry.md, infrastructure-deployment-blueprint.md, integration-dependency-graph.md, security-configuration-snapshot.md` preserved. `data-store-registry.md` now name/engine/version only (data semantics → DA) — same file, narrower content. **Status: ✅.**

### C-09 · TA deep-analyst outputs 🔴
- **Kept:** `technology-stack-assessment.md, architecture-pattern-catalog.md, component-interaction-contract-map.md, nfr-registry.md, technical-debt-risk-register.md, operational-architecture-assessment.md`.
- **Relocated OUT of `ta-outputs/`:**
  - `data-architecture-assessment.md` → now produced by DA as `datastore-transaction-consistency-assessment.md` in `da-outputs/`.
  - `security-architecture-assessment.md` (app-level) → now `application-security-assessment.md` in AA outputs; TA keeps `infra-transport-security-assessment.md`.
- **Consumer impact:** anything globbing `ta-outputs/data-architecture-assessment.md` or the app-level `security-architecture-assessment.md` must repoint. The eShopOnWeb sample tree currently has `ta-outputs/data-architecture-assessment.md` and `ta-outputs/security-architecture-assessment.md` — these become the relocated files.
- **Net artifact set across the pipeline:** preserved (same information, new owning folder).
- **Status: 🔴 location change — consumer repoint required.**

### C-17 · AA Stage 05 outputs 🔴
- **Kept (AA-owned):** `module-consolidation-map.{json,md}, service-boundary-options.md, migration-wave-plan.md, preserve-redesign-retire-map.md, api-contract-preservation-map.json, test-runtime-evidence-map.{json,md}, confidence-report.md, architecture-decision-inputs.md, forward-engineering-backlog.md`.
- **Relocated:** `business-capability-map.{json,md}` → BA; `data-ownership-map.md` → DA. The eShopOnWeb sample has these under `aa-outputs/final/`; post-cutover they originate from BA/DA and AA references them by node ID.
- **Status: 🔴 location change — consumer repoint required.** (Foundation cross-links resolve provenance, so the canonical graph is unaffected.)

### C-18 · New AA security file
- `application-security-assessment.md` added under AA outputs. **Status: 🟡 additive.**

### C-22..24 · Foundation outputs
- `ENTERPRISE_KNOWLEDGE_GRAPH.json` (9 sections) + `CANONICAL_ENTERPRISE_MODEL.md`, `ARCHITECTURE_INVENTORY.md`, `TRACEABILITY_MATRIX.md`, `FORWARD_ENGINEERING_INPUT_MAP.md` + `reconciliation-report.md`. These mirror the existing foundation-package filenames, so the foundation/forward-engineering consumers remain compatible; difference is they are now produced **in-pipeline**. **Status: ✅ additive / parity.**

---

## 3. Generated artifact-structure preservation

| Tree | Preserved? |
|---|---|
| `output/eShopOnWeb/ba_documents/*` | ✅ same 10 files |
| `output/eShopOnWeb/da-outputs/*` | ✅ 13 + 2 additive |
| `output/eShopOnWeb/ta-outputs/*` | ⚠️ 2 files relocate (C-09) |
| `output/eShopOnWeb/aa-outputs/**` | ⚠️ 2 files relocate (C-17); 1 additive (C-18) |
| `output/eShopOnWeb/layer2_output.json` | ⚠️ entity element shape (C-04) |
| Foundation / forward-engineering packages | ✅ untouched; now also producible in-pipeline |

---

## 4. Summary

- **15 / 22 replacements:** ✅ fully output-compatible (filenames + markers identical).
- **5 / 22:** ⚠️ compatible with a documented contract change (entity shape, additive files, narrowed content).
- **2 / 22 (R-09, R-17):** 🔴 location change — two pairs of files move to their correct owning layer; **net information preserved**, consumer globs must be repointed.

No downstream consumer **breaks** on content; the 🔴 items are **path** changes driven by the four
ownership relocations, which were the entire point of the refactor. Mitigations and gates are in
`REGRESSION_RISK_REPORT.md`.
