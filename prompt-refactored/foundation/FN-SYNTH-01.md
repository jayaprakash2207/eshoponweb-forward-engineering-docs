# Foundation / Synthesis — Knowledge Graph Synthesis

## 1. Metadata
- prompt_id:        FN-SYNTH-01
- version:          1.0.0
- owner_layer:      FN
- role:             Synthesis
- status:           active
- governed_by:      GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin:        required (run manifest)
- consumes:         [ba.*, da.*, aa.*, ta.*, layer1.* (tie-breaker only)]
- produces:         [fn.enterprise-knowledge-graph]
- supersedes:       (new — GOV-05)
- last_updated:     2026-06-24

## 2. Purpose
Reconcile the four extraction tracks (BA/DA/AA/TA) into one canonical, evidence-anchored knowledge graph.
The only layer permitted to merge facts across tracks and resolve cross-track conflicts.

## 3. Inputs
- `ba.*, da.*, aa.*, ta.*` owner artifacts (required) — consumed via contracts C-1…C-4 (GOV-07).
- `layer1.*` (optional) — tie-breaker evidence only; never an authoritative source (FN-1).
- Quality gate (GR-7.1): FAIL if any of the four owner tracks is missing or schema-invalid.

## 4. Responsibilities (GOV-02: FN = Owner)
- Cross-track identity resolution; ownership normalization; conflict resolution; cross-linking; emit the knowledge graph with all 9 sections.

## 5. Allowed Actions (algorithm — GOV-05 §6)
1. **Ingest & validate** each track (GR-7).
2. **Identity resolution** — bucket nodes by (type, normalized-name, strong signals: path/table/route); merge into one canonical node, recording contributors.
3. **Ownership normalization** — set `owner_layer` = GOV-02 owner; keep non-owner contributions as evidence but flag relocation candidates (do not auto-fix).
4. **Conflict resolution** — rank conflicting values by GR-2.2; higher rank wins; equal-rank disagreement → `DISCREPANCY` + Open Question (never arbitrary).
5. **Cross-linking** — build `cross_links` capability→process→entity→service→api using owner IDs only; dangling endpoint → `unknown` + Open Question.

## 6. Forbidden Actions
- MUST NOT extract primary facts from raw source (FN-1).
- MUST NOT delete a contributor's evidence (FN-2).
- MUST NOT raise confidence silently (FN-5 / GR-1.6); only lower on conflict, or raise with a change record.
- MUST NOT reassign a node's owner to "fix" a mismatch — flag it (FN-6).
- MUST NOT invent facts, code, or target design (GR-1, GR-5, FN-7).

## 7. Outputs (marker: FN_FILE)
- `fn.enterprise-knowledge-graph` → `ENTERPRISE_KNOWLEDGE_GRAPH.json` with sections:
  `metadata · business · data · application · technology · cross_links · assumptions · normalization_log · open_questions`.
- Every node: `id, type, name, owner_layer, confidence (GOV-04), evidence[], status`.
- `normalization_log` records every merge/rename (before/after IDs + rationale).

## 8. Validation Rules
{{include: CMP-VALID outputs=[enterprise-knowledge-graph]}}
- All 9 sections present; every node has id/type/owner/confidence/evidence (GR-7.2).
- Every `cross_links` endpoint resolves to a node (no orphans).
- Node `owner_layer` equals its GOV-02 owner; mismatches flagged (FN-6), not silently changed.
- Determinism: same inputs + pinned model → identical graph (GR-10.3).

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[capability, entity, component, technology, security-control]}}
- Canonical node confidence = min(contributors) unless higher-rank evidence justifies a raise (with change record).

## 10. Traceability Rules
- Preserve all contributor IDs inside each canonical node; `cross_links` use owner IDs only (GR-3.4).
- Unresolved DISCREPANCY and every `unknown` populate `open_questions`.

## 11. Governance Reference
{{include: CMP-GOV role=Synthesis}}
{{include: CMP-EVID live_source=false}}
{{include: CMP-OUT marker_name=FN_FILE audience=technical}}

## 12. Version Information
- changelog:
  - 1.0.0 — 2026-06-24 — New Foundation synthesis prompt per GOV-05; moves cross-track reconciliation inside the pipeline (audit finding 10, R1). — Prompt Architect
- supersedes: (none — new)
- migration_ref: ../reports/MIGRATION_REPORT.md#fn-synth-01
