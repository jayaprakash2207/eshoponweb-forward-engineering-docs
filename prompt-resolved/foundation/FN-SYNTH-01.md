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
───────────────────────────────────────────── VALIDATION (GR-7) ─────────────────────────────────────────────
Validation per GR-7. Before completion:
  - GR-7.1  Stop (FAIL) if any REQUIRED input is missing, empty, or schema-invalid.
  - GR-7.2  Emitted JSON parses; every graph edge resolves to an existing node.
  - GR-7.3  Sibling cross-references agree (counts match; cited items exist in their sets).
  - GR-7.4  Every `unknown` / ambiguity becomes an Open Question with a stable ID.
  - GR-7.5  Self-check: no invented facts (GR-1), all claims cited (GR-2/3),
            no excluded paths used (GR-4), no source modified (GR-5).
  - Per declared output (enterprise-knowledge-graph): confirm it exists, is well-formed, and is traceable.
Emit a run verdict: PASS | PARTIAL | FAIL (GOV-04 §5).
  PASS    = all required outputs present, valid, traceable; no unresolved material DISCREPANCY.
  PARTIAL = produced with documented gaps (status: partial + Open Questions).
  FAIL    = missing/invalid required outputs, or unresolved material DISCREPANCY.
───────────────────────────────────────────────────────────────────────────────────────────────────────────
- All 9 sections present; every node has id/type/owner/confidence/evidence (GR-7.2).
- Every `cross_links` endpoint resolves to a node (no orphans).
- Node `owner_layer` equals its GOV-02 owner; mismatches flagged (FN-6), not silently changed.
- Determinism: same inputs + pinned model → identical graph (GR-10.3).

## 9. Confidence Rules
───────────────────────────────────────────── CONFIDENCE (GOV-04) ─────────────────────────────────────────────
Confidence per GOV-04 (04_CONFIDENCE_STANDARD.md) v1.0.0.
Emit exactly ONE label per finding: HIGH | MEDIUM | LOW | ASSUMED | DISCREPANCY.
Decision order (first match wins):
  1 contradicts another source / upstream owner   → DISCREPANCY  (resolve via GR-2.4, then relabel result)
  2 direct evidence, GR-2.2 rank 1–3, cited        → HIGH
  3 supported, rank 3–5, partial/indirect, cited   → MEDIUM
  4 only rank 6–7 / lock-file fallback / 1 weak    → LOW   (+reason)
  5 no qualifying evidence                          → ASSUMED (+rationale, +Open Question)
Rules:
  - HIGH requires a direct code/config/declaration citation (GR-2.5).
  - LOW / ASSUMED / DISCREPANCY require a short reason string.
  - Upstream confidence is preserved; never silently raised (GR-1.6); raising needs a change record (GR-9.2).
  - Numeric bands (HIGH .90–1.0, MEDIUM .70–.89, LOW .50–.69, ASSUMED <.50) are DERIVED for tooling only.
  - Do NOT define any local numeric or categorical scale.
Escalation: an ASSUMED on a material node (capability, entity, component, technology, security-control) → raise Open Question and block HIGH-only
downstream gates that depend on it. Unresolved cross-track DISCREPANCY → escalate to Foundation (GOV-05).
─────────────────────────────────────────────────────────────────────────────────────────────────────────────
- Canonical node confidence = min(contributors) unless higher-rank evidence justifies a raise (with change record).

## 10. Traceability Rules
- Preserve all contributor IDs inside each canonical node; `cross_links` use owner IDs only (GR-3.4).
- Unresolved DISCREPANCY and every `unknown` populate `open_questions`.

## 11. Governance Reference
───────────────────────────────────────────── GOVERNANCE (GOV-01) ─────────────────────────────────────────────
Governed by GOV-01 (01_GLOBAL_PROMPT_RULES.md) v1.0.0. Rules are INHERITED, not restated.
Applicable rule groups:
  GR-1  anti-hallucination      GR-2/3 evidence & citation     GR-4 exclusions
  GR-5  no-modification         GR-6   chunk processing         GR-7 validation
  GR-8  output discipline       GR-9   change records (Review)  GR-10 model/reproducibility
Ownership per GOV-02. Boundaries per GOV-08. Confidence per GOV-04.
Role (this prompt = Synthesis):
  Synthesis → consumes owner artifacts only; never extracts primary facts (FN-1); reconciles, never invents.
Prompts add only NARROWER constraints in Forbidden Actions — never looser.
─────────────────────────────────────────────────────────────────────────────────────────────────────────────
───────────────────────────────────────────── EVIDENCE (GR-2/GR-3) ─────────────────────────────────────────────
Evidence per GR-2. Strength hierarchy (high → low), used to RESOLVE conflicts:
  1 live system / DB query            (only if live_source=true)
  2 migration / IaC / manifest declarations
  3 entity / ORM / source declarations
  4 tests
  5 source logic / usage
  6 naming conventions
  7 docs / comments / git history
Higher rank wins on conflict; NEVER average (GR-2.4). Rank 7 wins only if it cites a hard, named constraint.
Citations are machine-resolvable: `path/to/file.ext:line` (line optional when not derivable) (GR-3.1).
Every emitted node carries ≥1 citation OR `unknown` + Open Question (GR-3.2, GR-1.2).
When consuming an upstream artifact, cite the OWNER's node ID, never a re-derived local copy (GR-3.3/3.4).
Preserve verbatim values — versions, thresholds, enum/state values, config (GR-1.4). Never write secrets (GR-5.4).
Live-source mode (this prompt = live_source=false):
    live_source=false → do not attempt live connections; rely on declared/source evidence only.
─────────────────────────────────────────────────────────────────────────────────────────────────────────────
───────────────────────────────────────────── OUTPUT (GR-8) ─────────────────────────────────────────────
Output per GR-8. Produce EXACTLY the artifacts listed in this prompt's `Outputs` — no more, no fewer (GR-8.1).
Multi-file output uses the project's standard markers; do NOT invent new schemes (GR-8.2):
   ===FN_FILE_START:<relative/path>===
   <content>
   ===FN_FILE_END===
Structure is deterministic: stable field names, stable ordering keys, stable IDs (GR-8.3).
Audience language (this prompt = audience=technical):
    audience=technical → technical terms permitted; still evidence-cited.
If an output cannot be fully produced, still emit it with: status: incomplete, reason, Open Questions (GR-8.5).
Legacy delimiter compatibility: use the marker_name the consuming runner already parses (FN_FILE).
─────────────────────────────────────────────────────────────────────────────────────────────────────────

## 12. Version Information
- changelog:
  - 1.0.0 — 2026-06-24 — New Foundation synthesis prompt per GOV-05; moves cross-track reconciliation inside the pipeline (audit finding 10, R1). — Prompt Architect
- supersedes: (none — new)
- migration_ref: ../reports/MIGRATION_REPORT.md#fn-synth-01
