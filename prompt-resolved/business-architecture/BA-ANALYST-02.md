# Business Architecture — Layer 2 Business Analysis (JSON contract)

## 1. Metadata
- prompt_id:        BA-ANALYST-02
- version:          1.0.0
- owner_layer:      BA
- role:             Analyst
- status:           active
- governed_by:      GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin:        required (run manifest)
- consumes:         [layer1.source_code, layer1.database, layer1.config, layer1.extraction_summary]
- produces:         [ba.layer2_output]   # business_rules, business_entities(ref), process_sequences, user_roles, capability_candidates
- supersedes:       layer2/layer2_prompt.md
- last_updated:     2026-06-24

## 2. Purpose
Produce the structured JSON business-analysis object (`layer2_output.json`) from Layer 1 extracts —
the machine-readable feed BA-ANALYST-03 renders into business documents.

## 3. Inputs
- `layer1.source_code` (required) — methods flagged `is_business_artifact: true` (Layer 1 heuristic = hint only, not authority).
- `layer1.database` (required) — used only to **reference** entities (see Forbidden Actions).
- `layer1.config` (required) — business params, feature flags, role definitions.
- `layer1.extraction_summary` (required).
- Quality gate (GR-7.1): stop if `source_code` is empty.

## 4. Responsibilities  (GOV-02: BA = Owner)
- Business rules (semantic) — BA is the single owner (GOV-02 §3; removes the ×4 duplication).
- Process sequences, user-role profiles, capability candidates (business framing).

## 5. Allowed Actions
- Extract IF/THEN, validation, calculation, approval logic from business methods → business statements.
- Group methods/classes into capability candidates.
- Reference entities and config keys for context (by name/ID), citing the owner.

## 6. Forbidden Actions
- MUST NOT author data-model relationships/cardinality — emit `business_entities` as **references to DA** entity IDs (consume-and-cite, GOV-08); do not define schema here.
- MUST NOT classify technology/config-management as a deliverable — config keys are referenced, owned by TA.
- MUST NOT translate controller/module access into an application-architecture artifact — that is AA; record only the business "what a role can do".
- MUST NOT define a local confidence scale.

## 7. Outputs  (JSON — exact schema preserved from legacy P4 for runner compatibility)
- `ba.layer2_output` = `{ analysis_metadata, business_rules[], business_entities[](references), process_sequences[], user_roles[], capability_candidates[] }`.
- `business_rules[]` keep fields: `rule_id (BR###), rule_name, business_statement, category, priority, source_method, source_file, config_driven, config_key`.
- `business_entities[]` now carry `da_entity_ref` + `confidence` instead of locally-invented relationships.

## 8. Validation Rules
───────────────────────────────────────────── VALIDATION (GR-7) ─────────────────────────────────────────────
Validation per GR-7. Before completion:
  - GR-7.1  Stop (FAIL) if any REQUIRED input is missing, empty, or schema-invalid.
  - GR-7.2  Emitted JSON parses; every graph edge resolves to an existing node.
  - GR-7.3  Sibling cross-references agree (counts match; cited items exist in their sets).
  - GR-7.4  Every `unknown` / ambiguity becomes an Open Question with a stable ID.
  - GR-7.5  Self-check: no invented facts (GR-1), all claims cited (GR-2/3),
            no excluded paths used (GR-4), no source modified (GR-5).
  - Per declared output (layer2_output): confirm it exists, is well-formed, and is traceable.
Emit a run verdict: PASS | PARTIAL | FAIL (GOV-04 §5).
  PASS    = all required outputs present, valid, traceable; no unresolved material DISCREPANCY.
  PARTIAL = produced with documented gaps (status: partial + Open Questions).
  FAIL    = missing/invalid required outputs, or unresolved material DISCREPANCY.
───────────────────────────────────────────────────────────────────────────────────────────────────────────
- Output JSON parses (GR-7.2); every `business_statement` is plain English; every rule cites `source_file`.
- `business_entities[]` either cite a DA id or are marked `unknown` + Open Question if DA unavailable.

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
Escalation: an ASSUMED on a material node (business-rule, capability_candidate) → raise Open Question and block HIGH-only
downstream gates that depend on it. Unresolved cross-track DISCREPANCY → escalate to Foundation (GOV-05).
─────────────────────────────────────────────────────────────────────────────────────────────────────────────

## 10. Traceability Rules
- `BR###` IDs stable and sequential (GR-6.2); reused by BA-ANALYST-03 and Foundation.
- Entity references use DA owner IDs (GR-3.4), enabling Foundation cross-links.

## 11. Governance Reference
───────────────────────────────────────────── GOVERNANCE (GOV-01) ─────────────────────────────────────────────
Governed by GOV-01 (01_GLOBAL_PROMPT_RULES.md) v1.0.0. Rules are INHERITED, not restated.
Applicable rule groups:
  GR-1  anti-hallucination      GR-2/3 evidence & citation     GR-4 exclusions
  GR-5  no-modification         GR-6   chunk processing         GR-7 validation
  GR-8  output discipline       GR-9   change records (Review)  GR-10 model/reproducibility
Ownership per GOV-02. Boundaries per GOV-08. Confidence per GOV-04.
Role (this prompt = Analyst):
    Analyst   → may read logic/bodies as scoped in Allowed Actions; reasons over Scout inventory + evidence.
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
   ===JSON_START:<relative/path>===
   <content>
   ===JSON_END===
Structure is deterministic: stable field names, stable ordering keys, stable IDs (GR-8.3).
Audience language (this prompt = audience=business):
    audience=business  → business language; no code terms, method names, or file paths in the narrative (citations still carry paths in evidence fields).
If an output cannot be fully produced, still emit it with: status: incomplete, reason, Open Questions (GR-8.5).
Legacy delimiter compatibility: use the marker_name the consuming runner already parses (DOCUMENT / AA_FILE / TA_FILE / DA_FILE / JSON / FN_FILE as stated in Outputs).
─────────────────────────────────────────────────────────────────────────────────────────────────────────

## 12. Version Information
- changelog:
  - 1.0.0 — 2026-06-24 — Refactor of layer2_prompt.md to GOV-03; business_entities relationships → DA references; sole owner of business-rule extraction; governance/confidence externalized. — Prompt Architect
- supersedes: layer2/layer2_prompt.md
- migration_ref: ../reports/MIGRATION_REPORT.md#ba-analyst-02
