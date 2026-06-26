# 04 — Responsibility Matrix (Optimized Set)

**Date:** 2026-06-24
**Scope:** the 18 executable prompts + orchestrator + master spec + 5 components after optimization.
**Ownership unchanged from GOV-02** — optimization removed/merged within layers only.

---

## Business Architecture (2)

| Prompt | Owner | Responsibilities | Outputs | Consumers |
|---|---|---|---|---|
| **BA-ANALYST-02** (absorbs scout Phase A) | BA | Phase A: structural scan (domains, services, states, roles). Phase B: business rules (sole owner), entities (as DA refs), processes, user roles, capability candidates | `layer2_output.json` | BA-ANALYST-03, FN-SYNTH-01 |
| **BA-ANALYST-03** | BA | Render 10 BA documents (capability map, value stream, processes, rules, data-model *view*, stakeholders, KPIs, motivation/operating/roadmap) | `ba_documents/01..10_*.md` | FN-SYNTH-01, stakeholders |

## Data Architecture (2)

| Prompt | Owner | Responsibilities | Outputs | Consumers |
|---|---|---|---|---|
| **DA-EXTRACT-01** (merged Scout+Analyst) | DA | Schema/source/PII/migration inventory; conceptual model, ERD, dictionary, data-flow, quality, storage, redundancy, access-control, hidden-rules; **data-ownership-map**; **datastore-transaction-consistency-assessment** | `da-outputs/` 13 files + 2 relocated-in | DA-REVIEW-01, BA, AA, TA, FN |
| **DA-REVIEW-01** | DA | Validate/enrich vs tests/docs/live DB; change records; Gate verdict | `da-outputs/*` (updated) + `review-summary.md` | FN-SYNTH-01 |

## Application Architecture (9 + orchestrator + spec)

| Prompt | Owner | Responsibilities | Outputs | Consumers |
|---|---|---|---|---|
| **AA-ANALYST-00** (spec) | AA | Authoritative AA contract (not a run stage) | — (governs stages) | AA stages |
| **AA-SCOUT-01** | AA | File/project/language inventory | `inventory/*.json` (4) | AA-SCOUT-02, AA-ANALYST-03 |
| **AA-SCOUT-02** | AA | Symbols/routes/deps/entry-points | `parsed/*.json` (4) | AA-ANALYST-03 |
| **AA-ANALYST-03** | AA | Evidence packs | `evidence-packs/*.json` (9) | AA-ANALYST-04 |
| **AA-ANALYST-04** | AA | Final architecture synthesis | `final/*` (14) | AA-ANALYST-05/06, AA-REVIEW-06, FN |
| **AA-ANALYST-05** | AA | Forward-eng inputs (consolidation, boundaries, waves, API-preservation, backlog) | 9 AA-owned files | FN, decision-makers |
| **AA-ANALYST-06** | AA | App/data-level security posture | `application-security-assessment.md` | FN, TA (context) |
| **AA-REVIEW-06** | AA | Artifact quality review | quality-review, exec-summary, sanity-check | gate |
| **AA-REVIEW-07** | AA | Workflow/process audit | workflow-audit, missing-output-fixes | gate, governance |
| **AGENTS.md** | AA | Stage orchestration manifest | — | runner |

## Technology Architecture (2)

| Prompt | Owner | Responsibilities | Outputs | Consumers |
|---|---|---|---|---|
| **TA-SCOUT-01** | TA | Stack/infra/CI-CD/security-config inventory | `ta_agent1/*` (6) | TA-ANALYST-01, FN |
| **TA-ANALYST-01** | TA | Patterns, NFRs, tech debt, component interactions, infra/transport security | 7 TA assessment files | FN, AA (context) |

## Foundation (3)

| Prompt | Owner | Responsibilities | Outputs | Consumers |
|---|---|---|---|---|
| **FN-SYNTH-01** | FN | Cross-track reconciliation → knowledge graph (9 sections, 274 nodes) | `ENTERPRISE_KNOWLEDGE_GRAPH.json` | FN-SYNTH-02, FN-REVIEW-01 |
| **FN-SYNTH-02** | FN | Canonical model + inventory + traceability matrix + FE input map | 4 markdown views | FN-REVIEW-01, FE (out of scope) |
| **FN-REVIEW-01** | FN | Reconciliation/owner/traceability validation + Gate verdict | `reconciliation-report.md` | FE gate |

## Reusable Components (5)

| Component | Resolves | Used by |
|---|---|---|
| CMP-GOV | GOV-01 governance | all prompts |
| CMP-CONF | GOV-04 confidence | all extracting/reviewing prompts |
| CMP-VALID | GR-7 validation | all prompts |
| CMP-EVID | GR-2/3 evidence | all prompts |
| CMP-OUT | GR-8 output discipline | all prompts |

---

## Ownership integrity check (post-optimization)

| Responsibility | Sole owner | Changed by optimization? |
|---|---|---|
| Business rules / capabilities / processes | BA | No |
| Schema / entities / data-ownership / data-store consistency | DA | No |
| Components / interfaces / app-security | AA | No |
| Tech stack / infra / NFR / infra-security | TA | No |
| Knowledge graph / canonical model | FN | No |

**Every responsibility still has exactly one owner. Optimization merged/eliminated only *within* a layer
and *within* an owner — GOV-02 fully preserved.**
