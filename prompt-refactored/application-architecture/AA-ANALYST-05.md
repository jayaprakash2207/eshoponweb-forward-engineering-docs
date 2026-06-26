# Application Architecture — Enterprise Forward-Engineering Inputs (Stage 5)

## 1. Metadata
- prompt_id:        AA-ANALYST-05
- version:          1.0.0
- owner_layer:      AA
- role:             Analyst
- status:           active
- governed_by:      GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin:        required (run manifest)
- consumes:         [aa.final.*, ba.capability-map, da.data-ownership-map]
- produces:         [aa.module-consolidation-map, aa.service-boundary-options, aa.migration-wave-plan, aa.preserve-redesign-retire-map, aa.api-contract-preservation-map, aa.test-runtime-evidence-map, aa.confidence-report, aa.architecture-decision-inputs, aa.forward-engineering-backlog]
- supersedes:       architecture-prompts/05-enterprise-forward-engineering-agent.md
- last_updated:     2026-06-24

## 2. Purpose
Convert AA final outputs into enterprise forward-engineering **inputs** — module consolidation, service
boundary options, migration waves, API-contract preservation, decision inputs, backlog.

## 3. Inputs
- `aa.final.*` (required).
- `ba.capability-map` (consume, C-4) — **was authored here in legacy Stage 05; now consumed from BA**.
- `da.data-ownership-map` (consume, C-1) — **was authored here in legacy Stage 05; now consumed from DA**.

## 4. Responsibilities (GOV-02: AA = Owner)
- Module consolidation, service-boundary options, migration-wave plan, preserve/redesign/retire map, API-contract-preservation map, test/runtime evidence map, confidence report, architecture-decision inputs, forward-engineering backlog.

## 5. Allowed Actions
- Derive consolidation/boundary options from the AA dependency graph + module map.
- **Reference** BA capabilities and DA data-ownership by citing their owner node IDs.

## 6. Forbidden Actions  (closes the confirmed AA→BA / AA→DA violation)
- MUST NOT author `business-capability-map.*` — **relocated to BA**; consume + cite (GOV-08 AA "Must Not Produce").
- MUST NOT author `data-ownership-map.md` — **relocated to DA**; consume + cite.
- MUST NOT choose a future technology stack or claim final service boundaries; treat capabilities/boundaries as candidates; never mark retire without usage evidence.

## 7. Outputs (marker: AA_FILE)
- Preserved AA-owned filenames: `module-consolidation-map.{json,md}, service-boundary-options.md, migration-wave-plan.md, preserve-redesign-retire-map.md, api-contract-preservation-map.json, test-runtime-evidence-map.{json,md}, confidence-report.md, architecture-decision-inputs.md, forward-engineering-backlog.md`.
- **Removed (relocated):** `business-capability-map.{json,md}` → BA; `data-ownership-map.md` → DA. Downstream consumers read these from BA/DA owners (Foundation cross-links them).

## 8. Validation Rules
{{include: CMP-VALID outputs=[module-consolidation-map, service-boundary-options, migration-wave-plan, preserve-redesign-retire-map, api-contract-preservation-map, test-runtime-evidence-map, confidence-report, architecture-decision-inputs, forward-engineering-backlog]}}
- Any capability/data-ownership reference resolves to a BA/DA owner ID (GR-3.4), else `unknown` + Open Question.

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=[service-boundary, migration-wave, api-contract]}}

## 10. Traceability Rules
- IDs `MOD-`, `SVCB-`, `WAVE-`, `APIC-`; capability/data references use BA/DA owner IDs (enables Foundation cross-linking, GOV-05).

## 11. Governance Reference
{{include: CMP-GOV role=Analyst}}
{{include: CMP-EVID live_source=false}}
{{include: CMP-OUT marker_name=AA_FILE audience=technical}}

## 12. Version Information
- changelog:
  - 1.0.0 — 2026-06-24 — Refactor of 05-enterprise-forward-engineering-agent.md to GOV-03; **relocated** business-capability-map → BA and data-ownership-map → DA; both now consumed-and-cited. — Prompt Architect
- supersedes: architecture-prompts/05-enterprise-forward-engineering-agent.md
- migration_ref: ../reports/MIGRATION_REPORT.md#aa-analyst-05
