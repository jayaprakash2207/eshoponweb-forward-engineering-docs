# Application Architecture — Workflow Audit (Stage 7)

## 1. Metadata
- prompt_id: AA-REVIEW-07 | version: 1.0.0 | owner_layer: AA | role: Review | status: active
- governed_by: GOV-01 v1.0.0 | confidence_model: GOV-04 v1.0.0 | model_pin: required (run manifest)
- consumes: [AGENTS.md, aa.prompts.*, aa.outputs.*, run.manifest] | produces: [aa.architecture-workflow-audit, aa.missing-output-fixes]
- supersedes: architecture-prompts/07-workflow-audit-agent.md | last_updated: 2026-06-24

## 2. Purpose
Audit the AA extraction workflow itself for enterprise readiness and reuse — a meta-level gate over the pipeline.

## 3. Inputs
- `AGENTS.md`, all AA prompts, all AA outputs, and the run manifest (for model/version pins, GR-10).

## 4. Responsibilities (GOV-02: AA = Owner, Review role)
- Stage completeness, I/O contract validation, source-modification guard, schema validation, run-history verification, graph normalization, quality-gate verification, hardcoding detection, parser extensibility, hallucination-handling, forward-engineering usefulness.

## 5. Allowed Actions
- Inspect prompts/outputs/manifest; verify GOV-03 conformance and GOV-02 ownership were respected.

## 6. Forbidden Actions
- MUST NOT modify prompts or outputs (audit only, GR-5).
- MUST NOT use a local maturity vocabulary — map to GOV-04 §5 (ENTERPRISE READY=PASS, etc.).

## 7. Outputs (marker: AA_FILE)
- `architecture-workflow-audit.md, missing-output-fixes.md` (filenames preserved); include a score /100 and a GOV-04 verdict.

## 8. Validation Rules
───────────────────────────────────────────── VALIDATION (GR-7) ─────────────────────────────────────────────
Validation per GR-7. Before completion:
  - GR-7.1  Stop (FAIL) if any REQUIRED input is missing, empty, or schema-invalid.
  - GR-7.2  Emitted JSON parses; every graph edge resolves to an existing node.
  - GR-7.3  Sibling cross-references agree (counts match; cited items exist in their sets).
  - GR-7.4  Every `unknown` / ambiguity becomes an Open Question with a stable ID.
  - GR-7.5  Self-check: no invented facts (GR-1), all claims cited (GR-2/3),
            no excluded paths used (GR-4), no source modified (GR-5).
  - Per declared output (architecture-workflow-audit, missing-output-fixes): confirm it exists, is well-formed, and is traceable.
Emit a run verdict: PASS | PARTIAL | FAIL (GOV-04 §5).
  PASS    = all required outputs present, valid, traceable; no unresolved material DISCREPANCY.
  PARTIAL = produced with documented gaps (status: partial + Open Questions).
  FAIL    = missing/invalid required outputs, or unresolved material DISCREPANCY.
───────────────────────────────────────────────────────────────────────────────────────────────────────────
- Confirm model + prompt/component versions are recorded in the run manifest (GR-10.2/10.3).

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
Escalation: an ASSUMED on a material node (stage-contract, schema) → raise Open Question and block HIGH-only
downstream gates that depend on it. Unresolved cross-track DISCREPANCY → escalate to Foundation (GOV-05).
─────────────────────────────────────────────────────────────────────────────────────────────────────────────

## 10. Traceability Rules
- Each finding references the stage/prompt/output it concerns and the GOV rule applied.

## 11. Governance Reference
───────────────────────────────────────────── GOVERNANCE (GOV-01) ─────────────────────────────────────────────
Governed by GOV-01 (01_GLOBAL_PROMPT_RULES.md) v1.0.0. Rules are INHERITED, not restated.
Applicable rule groups:
  GR-1  anti-hallucination      GR-2/3 evidence & citation     GR-4 exclusions
  GR-5  no-modification         GR-6   chunk processing         GR-7 validation
  GR-8  output discipline       GR-9   change records (Review)  GR-10 model/reproducibility
Ownership per GOV-02. Boundaries per GOV-08. Confidence per GOV-04.
Role (this prompt = Review):
    Review    → validates/enriches upstream owner artifacts; emits change records (GR-9); verdict (GOV-04 §5).
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
   ===AA_FILE_START:<relative/path>===
   <content>
   ===AA_FILE_END===
Structure is deterministic: stable field names, stable ordering keys, stable IDs (GR-8.3).
Audience language (this prompt = audience=technical):
    audience=technical → technical terms permitted; still evidence-cited.
If an output cannot be fully produced, still emit it with: status: incomplete, reason, Open Questions (GR-8.5).
Legacy delimiter compatibility: use the marker_name the consuming runner already parses (AA_FILE).
─────────────────────────────────────────────────────────────────────────────────────────────────────────

## 12. Version Information
- 1.0.0 — 2026-06-24 — Refactor of 07-workflow-audit-agent.md to GOV-03; maturity verdict → GOV-04 §5; manifest/version checks added. — Prompt Architect
- supersedes: architecture-prompts/07-workflow-audit-agent.md | migration_ref: ../reports/MIGRATION_REPORT.md#aa-review-07
