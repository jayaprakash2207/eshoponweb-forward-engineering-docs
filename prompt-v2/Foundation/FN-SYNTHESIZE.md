# FN-SYNTHESIZE — Foundation Synthesis

## 1. Metadata
- prompt_id:        FN-SYNTHESIZE
- version:          2.0.0
- owner_layer:      FN
- role:             SYNTHESIZE (internally phased: graph → views)
- status:           active
- governed_by:      GOV (V2-GOV) v2.0.0 → GOV-01
- confidence_model: CONFIDENCE (V2-CONF) v2.0.0 → GOV-04
- model_pin:        required (run manifest)
- consumes:         [ba.*, da.*, aa.*, ta.* (owner artifacts), layer1.* (tie-breaker only)]
- produces:         [fn.enterprise-knowledge-graph, fn.canonical-enterprise-model, fn.architecture-inventory, fn.traceability-matrix, fn.forward-engineering-input-map]
- merges:           [FN-SYNTH-01, FN-SYNTH-02]
- last_updated:     2026-06-24

## 2. Purpose
Merge Business + Data + Application + Technology owner artifacts into one canonical enterprise model:
reconcile cross-track facts, build the Enterprise Knowledge Graph, then project the canonical views —
in one prompt with a mandatory graph-before-views phasing.

> Merge note: FN-SYNTH-01 (graph) + FN-SYNTH-02 (views). Folded because views are a pure projection of the
> graph; **Phase 1 (graph) MUST be complete and well-formed before Phase 2 (views)** — the same ordering
> the two prompts enforced via FN-REVIEW between them is here an internal phase gate.

## 3. Inputs
- `ba.*, da.*, aa.*, ta.*` owner artifacts (required, contracts C-1…C-4); `layer1.*` tie-breaker only (FN-1).
- Quality gate (GR-7.1): FAIL if any of the four owner tracks is missing/invalid.

## 4. Responsibilities  (GOV-02: FN = Owner)
- Cross-track identity resolution; ownership normalization; conflict resolution; cross-linking;
  knowledge-graph generation (9 sections); canonical model + inventory + traceability matrix + FE input map.

## 5. Allowed Actions — MANDATORY PHASES
- **Phase 1 — Reconcile → Graph** (algorithm GOV-05 §6): ingest+validate; identity resolution (merge same concept → one canonical node, record contributors); ownership normalization (owner = GOV-02 owner; flag mismatch, don't reassign — FN-6); conflict resolution by GR-2.2 rank (ties → DISCREPANCY + Open Question); cross-link capability→process→entity→service→api using owner IDs → emit **`ENTERPRISE_KNOWLEDGE_GRAPH.json`** with all 9 sections (`metadata, business, data, application, technology, cross_links, assumptions, normalization_log, open_questions`).
- **Phase 2 — Project → Views** (graph must be well-formed first): render `CANONICAL_ENTERPRISE_MODEL.md`, `ARCHITECTURE_INVENTORY.md`, `TRACEABILITY_MATRIX.md`, `FORWARD_ENGINEERING_INPUT_MAP.md` as read-only projections; confidence/status copied verbatim (GR-1.6).

## 6. Forbidden Actions
- MUST NOT extract primary facts from raw source (FN-1); MUST NOT delete contributor evidence (FN-2).
- MUST NOT raise confidence silently (FN-5); MUST NOT reassign owner to "fix" mismatch (FN-6).
- MUST NOT add/infer/elevate any fact beyond the graph in Phase 2 (FN-7); MUST NOT propose target design/code.
- MUST NOT project views before the graph is complete and valid (phase gate).

## 7. Outputs  (FN_FILE marker; foundation-package filenames preserved)
- `ENTERPRISE_KNOWLEDGE_GRAPH.json` (9 sections; every node: id/type/name/owner/confidence/evidence/status; `normalization_log` records every merge).
- `CANONICAL_ENTERPRISE_MODEL.md`, `ARCHITECTURE_INVENTORY.md`, `TRACEABILITY_MATRIX.md`, `FORWARD_ENGINEERING_INPUT_MAP.md`.

## 8. Validation Rules
Per `Shared/VALIDATION.md`. All 9 sections present; no orphan cross-links; owner = GOV-02 owner (mismatch flagged); views carry graph node IDs; determinism (GR-10.3).

## 9. Confidence Rules
Per `Shared/CONFIDENCE.md`. Canonical node confidence = min(contributors) unless higher-rank evidence justifies a raise (with change record). Material nodes: capability, entity, component, technology, service, api, security-control.

## 10. Traceability Rules
Preserve all contributor IDs inside each canonical node; cross_links use owner IDs only; unresolved DISCREPANCY + every `unknown` → `open_questions`.

## 11. Governance Reference
Per `Shared/GOV.md` (role=SYNTHESIZE, live_source=false, marker=FN_FILE, audience=technical).

## 12. Version Information
- 2.0.0 — 2026-06-24 — Merge of FN-SYNTH-01 (graph) + FN-SYNTH-02 (views) with an internal graph-before-views phase gate. — Prompt Architect
- supersedes: FN-SYNTH-01, FN-SYNTH-02
- migration_ref: ../01_PROMPT_MERGE_REPORT.md#fn
