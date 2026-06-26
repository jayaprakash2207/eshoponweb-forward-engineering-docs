# Data Architecture — Schema & Source Scout

## 1. Metadata
- prompt_id:        DA-SCOUT-01
- version:          1.0.0
- owner_layer:      DA
- role:             Scout
- status:           active
- governed_by:      GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin:        required (run manifest)
- consumes:         [layer1.database, layer1.config, layer1.source_code, live.database?]
- produces:         [da.schema-catalogue, da.data-source-inventory, da.pii-inventory(raw), da.migration-complexity(raw)]
- supersedes:       DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md (extraction half)
- last_updated:     2026-06-24

## 2. Purpose
Extract the factual data inventory — schema/tables/entities/keys, data sources, PII candidates, migration
state — from code, migrations, and (when reachable) the live database. Declaration/extraction level.

## 3. Inputs
- `layer1.database`, `layer1.config`, `layer1.source_code` (required).
- `live.database` (optional) — connection strings parsed from config; live query is rank-1 evidence.
- Quality gate: stop if no entity/migration/schema evidence exists (GR-7.1).

## 4. Responsibilities  (GOV-02: DA = Owner; single owner of data-store/entity/schema discovery — removes ×5 duplication)
- Schema catalogue (tables/collections, columns, keys, indexes).
- Data-source inventory (every persistence technology declared/used).
- PII candidate inventory; migration-complexity raw signals.

## 5. Allowed Actions
- Read entity/model classes, migration files (chronologically), repository/query classes for declared schema.
- Attempt live DB connection (live_source=true); on success run row counts / schema / FK / index checks.
- Mark `SHARED` files read once (GR-4.2). Apply the evidence hierarchy to rank conflicting schema evidence.

## 6. Forbidden Actions
- MUST NOT assign business meaning to entities (BA owns semantic capabilities/rules) — surface facts only.
- MUST NOT assess application components/call flows (AA owns).
- MUST NOT inventory technology stack/infra as a deliverable (TA owns) — engine/version recorded only as data-source attribute.
- MUST NOT capture secret values from connection strings (GR-5.4).

## 7. Outputs
- `da.schema-catalogue` (JSON), `da.data-source-inventory` (JSON), `da.pii-inventory` (raw JSON), `da.migration-complexity` (raw JSON).
- Filenames preserved from legacy DA output set (`schema-catalogue.json`, `data-source-inventory.json`, `pii-inventory.json`, `migration-complexity.json`).

## 8. Validation Rules
───────────────────────────────────────────── VALIDATION (GR-7) ─────────────────────────────────────────────
Validation per GR-7. Before completion:
  - GR-7.1  Stop (FAIL) if any REQUIRED input is missing, empty, or schema-invalid.
  - GR-7.2  Emitted JSON parses; every graph edge resolves to an existing node.
  - GR-7.3  Sibling cross-references agree (counts match; cited items exist in their sets).
  - GR-7.4  Every `unknown` / ambiguity becomes an Open Question with a stable ID.
  - GR-7.5  Self-check: no invented facts (GR-1), all claims cited (GR-2/3),
            no excluded paths used (GR-4), no source modified (GR-5).
  - Per declared output (schema-catalogue, data-source-inventory, pii-inventory, migration-complexity): confirm it exists, is well-formed, and is traceable.
Emit a run verdict: PASS | PARTIAL | FAIL (GOV-04 §5).
  PASS    = all required outputs present, valid, traceable; no unresolved material DISCREPANCY.
  PARTIAL = produced with documented gaps (status: partial + Open Questions).
  FAIL    = missing/invalid required outputs, or unresolved material DISCREPANCY.
───────────────────────────────────────────────────────────────────────────────────────────────────────────
- Each table/entity carries evidence + confidence; live-confirmed = HIGH, naming-only = LOW (GOV-04).
- If live DB unreachable, record exact error + command; downstream confidence adjusted accordingly.

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
Escalation: an ASSUMED on a material node (entity, data-store, pii-field) → raise Open Question and block HIGH-only
downstream gates that depend on it. Unresolved cross-track DISCREPANCY → escalate to Foundation (GOV-05).
─────────────────────────────────────────────────────────────────────────────────────────────────────────────

## 10. Traceability Rules
- Stable IDs: `TBL-`, `ENT-`, `DS-`, `PII-`; each cites file:line or live-query (GR-3).
- These IDs are the canonical entity references consumed by BA/AA/TA and Foundation (no other layer re-extracts them).

## 11. Governance Reference
───────────────────────────────────────────── GOVERNANCE (GOV-01) ─────────────────────────────────────────────
Governed by GOV-01 (01_GLOBAL_PROMPT_RULES.md) v1.0.0. Rules are INHERITED, not restated.
Applicable rule groups:
  GR-1  anti-hallucination      GR-2/3 evidence & citation     GR-4 exclusions
  GR-5  no-modification         GR-6   chunk processing         GR-7 validation
  GR-8  output discipline       GR-9   change records (Review)  GR-10 model/reproducibility
Ownership per GOV-02. Boundaries per GOV-08. Confidence per GOV-04.
Role (this prompt = Scout):
    Scout     → declaration/inventory level only; never read logic/method bodies; no interpretation.
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
Live-source mode (this prompt = live_source=true):
    live_source=true  → attempt the live query first; on failure, fall back to code evidence and record the exact error + command; mark affected findings with the appropriate confidence (GOV-04).
─────────────────────────────────────────────────────────────────────────────────────────────────────────────
───────────────────────────────────────────── OUTPUT (GR-8) ─────────────────────────────────────────────
Output per GR-8. Produce EXACTLY the artifacts listed in this prompt's `Outputs` — no more, no fewer (GR-8.1).
Multi-file output uses the project's standard markers; do NOT invent new schemes (GR-8.2):
   ===DA_FILE_START:<relative/path>===
   <content>
   ===DA_FILE_END===
Structure is deterministic: stable field names, stable ordering keys, stable IDs (GR-8.3).
Audience language (this prompt = audience=technical):
    audience=technical → technical terms permitted; still evidence-cited.
If an output cannot be fully produced, still emit it with: status: incomplete, reason, Open Questions (GR-8.5).
Legacy delimiter compatibility: use the marker_name the consuming runner already parses (DOCUMENT / AA_FILE / TA_FILE / DA_FILE / JSON / FN_FILE as stated in Outputs).
─────────────────────────────────────────────────────────────────────────────────────────────────────────

## 12. Version Information
- changelog:
  - 1.0.0 — 2026-06-24 — Split extraction half of DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md into GOV-03 Scout; local Evidence Strength Hierarchy → CMP-EVID; numeric confidence → GOV-04 (band preserved). — Prompt Architect
- supersedes: DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md (extraction half)
- migration_ref: ../reports/MIGRATION_REPORT.md#da-scout-01
