# Business Architecture — Layer 3 Document Generation

## 1. Metadata
- prompt_id:        BA-ANALYST-03
- version:          1.0.0
- owner_layer:      BA
- role:             Analyst
- status:           active
- governed_by:      GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin:        required (run manifest)
- consumes:         [ba.layer2_output, da.conceptual-data-model?]
- produces:         [ba.10-business-documents]
- supersedes:       layer3/layer3_prompt.md
- last_updated:     2026-06-24

## 2. Purpose
Render the Layer 2 JSON (`ba.layer2_output`) into the 10 final business-architecture markdown documents,
in business language, with traceable BR references.

## 3. Inputs
- `ba.layer2_output` (required) — the BA-ANALYST-02 JSON. Stop if missing (GR-7.1).
- `da.conceptual-data-model` (optional consume, C-1) — for the data-model document content.

## 4. Responsibilities  (GOV-02: BA = Owner)
- Capability map, value stream, process models, business rules, stakeholder map, KPIs, motivation/operating/roadmap (INFERRED ones marked).

## 5. Allowed Actions
- Transform L2 arrays into the 10 documents using the legacy templates.
- Mark inferred documents (motivation/operating/roadmap) as INFERRED (GR-1.3 → ASSUMED-class).
- Render the data-model document **from DA-sourced content**, citing DA IDs.

## 6. Forbidden Actions
- MUST NOT invent BR references — every BR cited must exist in `ba.layer2_output` (GR-7.3).
- MUST NOT author original data-model facts — the `05_data_model` document is a **DA view** (consume-and-cite, GOV-08).
- MUST NOT emit operating/org-model content as fact — mark INFERRED + Open Question.
- MUST NOT use technical language (audience=business).

## 7. Outputs  (marker: DOCUMENT — `===DOCUMENT_START:<file>===` preserved for the runner splitter)
- `ba.10-business-documents`: `01_capability_map.md … 10_business_roadmap.md` (filenames preserved exactly for output compatibility).
- `05_data_model.md` is rendered as a DA-sourced view (content owner = DA).

## 8. Validation Rules
───────────────────────────────────────────── VALIDATION (GR-7) ─────────────────────────────────────────────
Validation per GR-7. Before completion:
  - GR-7.1  Stop (FAIL) if any REQUIRED input is missing, empty, or schema-invalid.
  - GR-7.2  Emitted JSON parses; every graph edge resolves to an existing node.
  - GR-7.3  Sibling cross-references agree (counts match; cited items exist in their sets).
  - GR-7.4  Every `unknown` / ambiguity becomes an Open Question with a stable ID.
  - GR-7.5  Self-check: no invented facts (GR-1), all claims cited (GR-2/3),
            no excluded paths used (GR-4), no source modified (GR-5).
  - Per declared output (01_capability_map, 02_value_stream, 03_process_models, 04_business_rules, 05_data_model, 06_stakeholder_map, 07_kpis_metrics, 08_motivation_model, 09_operating_model, 10_business_roadmap): confirm it exists, is well-formed, and is traceable.
Emit a run verdict: PASS | PARTIAL | FAIL (GOV-04 §5).
  PASS    = all required outputs present, valid, traceable; no unresolved material DISCREPANCY.
  PARTIAL = produced with documented gaps (status: partial + Open Questions).
  FAIL    = missing/invalid required outputs, or unresolved material DISCREPANCY.
───────────────────────────────────────────────────────────────────────────────────────────────────────────
- Exactly 10 documents, each delimiter-wrapped (GR-8.1/8.2).
- Every BR reference resolves to `ba.layer2_output.business_rules` (GR-7.3).

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
Escalation: an ASSUMED on a material node (capability, business-rule) → raise Open Question and block HIGH-only
downstream gates that depend on it. Unresolved cross-track DISCREPANCY → escalate to Foundation (GOV-05).
─────────────────────────────────────────────────────────────────────────────────────────────────────────────

## 10. Traceability Rules
- Preserve `BR###` IDs from L2; data-model entities cite DA IDs.
- Inferred documents carry explicit INFERRED/ASSUMED markers feeding Foundation `open_questions`.

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
   ===DOCUMENT_START:<relative/path>===
   <content>
   ===DOCUMENT_END===
Structure is deterministic: stable field names, stable ordering keys, stable IDs (GR-8.3).
Audience language (this prompt = audience=business):
    audience=business  → business language; no code terms, method names, or file paths in the narrative (citations still carry paths in evidence fields).
If an output cannot be fully produced, still emit it with: status: incomplete, reason, Open Questions (GR-8.5).
Legacy delimiter compatibility: use the marker_name the consuming runner already parses (DOCUMENT / AA_FILE / TA_FILE / DA_FILE / JSON / FN_FILE as stated in Outputs).
─────────────────────────────────────────────────────────────────────────────────────────────────────────

## 12. Version Information
- changelog:
  - 1.0.0 — 2026-06-24 — Refactor of layer3_prompt.md to GOV-03; data-model doc → DA view; governance/confidence externalized; 10-document output and delimiter scheme preserved. — Prompt Architect
- supersedes: layer3/layer3_prompt.md
- migration_ref: ../reports/MIGRATION_REPORT.md#ba-analyst-03
