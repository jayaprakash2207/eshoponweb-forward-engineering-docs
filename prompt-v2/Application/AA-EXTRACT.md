# AA-EXTRACT — Application Architecture Extraction

## 1. Metadata
- prompt_id:        AA-EXTRACT
- version:          2.0.0
- owner_layer:      AA
- role:             EXTRACT (multi-phase, parse-first MANDATORY)
- status:           active
- governed_by:      GOV (V2-GOV) v2.0.0 → GOV-01
- confidence_model: CONFIDENCE (V2-CONF) v2.0.0 → GOV-04
- model_pin:        required (run manifest)
- consumes:         [repo.root, ba.capability-map?, da.data-ownership-map?, ta.stack?]
- produces:         [aa.inventory, aa.parsed, aa.evidence-packs, aa.final, aa.forward-eng-inputs, aa.application-security-assessment]
- merges:           [AA-SCOUT-01, AA-SCOUT-02, AA-ANALYST-03, AA-ANALYST-04, AA-ANALYST-05, AA-ANALYST-06]   (AA-ANALYST-00 = embedded spec)
- last_updated:     2026-06-24

## 2. Purpose
Discover modules/services/APIs/interfaces, analyze dependencies and application security, and produce the
Application Architecture — in one governed prompt that **internally preserves the mandatory parse-first
stage chain** (the AA pipeline the audit praised as the reference model).

## 3. Inputs
- `repo.root` (required). Consumes (cite-only): `ba.capability-map` (C-4), `da.data-ownership-map` (C-1), `ta.stack` (C-3).
- Quality gate (GR-7.1): stop if inventory cannot be produced.

## 4. Responsibilities  (GOV-02: AA = Owner)
- Inventory; symbols/components/services; interfaces/APIs/entry-points; dependency graph; call flows;
  architecture patterns/violations; application risk; forward-engineering inputs; app/data-level security.

## 5. Allowed Actions — MANDATORY PHASE CHAIN (GR-6.1 — NON-NEGOTIABLE)
> ⚠ This is a 6-stage chain authored as one prompt. **Each phase emits its artifacts before the next reads
> them.** Skipping a phase (raw→final) is a GR-6.1 violation and a hard FAIL. The embedded **AA-ANALYST-00
> spec** governs the contract.
- **P1 Inventory** (Scout) → `inventory/*.json` (4): files, projects, languages, ignored-files.
- **P2 Parse/Symbol** (Scout) → `parsed/*.json` (4): symbols, routes, dependency-candidates, entry-points.
- **P3 Evidence Packs** (Analyst) → `evidence-packs/*.json` (9). *Technology-agnostic IR.*
- **P4 Final Architecture** (Analyst, from evidence packs only — no repo rescan) → `final/*` (14): system-inventory, module-boundary, component-registry, dependency-graph, interface-catalogue, call-flow-map, pattern-report, violation/risk registers, diagrams, etc.
- **P5 Forward-Engineering Inputs** (Analyst) → module-consolidation, service-boundary-options, migration-wave-plan, preserve/redesign/retire, api-contract-preservation, test-runtime-evidence, confidence-report, decision-inputs, backlog. *Consumes BA capabilities + DA ownership by citation.*
- **P6 Application Security** (Analyst) → `application-security-assessment.md` (app/data-level only).

## 6. Forbidden Actions
- MUST NOT skip or reorder P1→P4 (parse-first; raw→final = FAIL).
- MUST NOT author business-capability-map (BA) or data-ownership-map (DA) — consume + cite (GOV-08).
- MUST NOT produce tech-stack/infra blueprint or infra/transport security (TA) — P6 is app/data-level only.
- MUST NOT rescan the repo in P4 (evidence-packs only).

## 7. Outputs  (AA_FILE marker; all legacy AA filenames preserved)
- `inventory/` 4 · `parsed/` 4 · `evidence-packs/` 9 · `final/` 14 · forward-eng 9 · `application-security-assessment.md`.
- Every intermediate artifact is emitted (parse-first); not just the final set.

## 8. Validation Rules
Per `Shared/VALIDATION.md`. Parse-first integrity check: inventory→parsed→packs→final all present and in
order; final JSON valid; graph edges resolve; risks cite components.

## 9. Confidence Rules
Per `Shared/CONFIDENCE.md`. Material nodes: component, interface, dependency, architecture-pattern, violation, security-control.

## 10. Traceability Rules
Stable IDs `FILE-/PROJ-/SYM-/RT-/EP-/CMP-/IF-/DEP-/FLOW-/PAT-/VIO-/RISK-/SECA-`; cited; feed FN cross-links. BA/DA references use owner IDs.

## 11. Governance Reference
Per `Shared/GOV.md` (role=EXTRACT, live_source=false, marker=AA_FILE, audience=technical). Embedded contract = former AA-ANALYST-00.

## 12. Version Information
- 2.0.0 — 2026-06-24 — Merge of AA-SCOUT-01/02 + AA-ANALYST-03/04/05/06 into one phased EXTRACT; AA-ANALYST-00 embedded as the spec. Parse-first chain preserved internally. — Prompt Architect
- supersedes: AA-SCOUT-01, AA-SCOUT-02, AA-ANALYST-03, AA-ANALYST-04, AA-ANALYST-05, AA-ANALYST-06, AA-ANALYST-00
- migration_ref: ../01_PROMPT_MERGE_REPORT.md#aa
