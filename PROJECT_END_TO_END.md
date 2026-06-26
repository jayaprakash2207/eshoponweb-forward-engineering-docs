# Project End-to-End — What It Is, What It Has, How It Works

**Project:** Forward-/Reverse-Engineering Enterprise Platform (`frwd engg - op's`)
**Document type:** Whole-project deep-dive (system + prompt-architecture journey)
**Date:** 2026-06-24
**Grounding:** Every claim below is drawn from files in this repository. Where the source records `unknown`/`inferred`/`LOW`, this document preserves that — it does not invent.

---

## 0. TL;DR — what this project is in three sentences

1. It is an **AI-assisted reverse-engineering platform** that takes a legacy codebase (worked example: **eShopOnWeb**, a .NET 8 reference app) and reconstructs a complete, evidence-anchored **enterprise architecture** across five TOGAF-style layers — Business, Data, Application, Technology, and a Foundation/synthesis layer.
2. The Foundation layer merges all four layers into a single **Enterprise Knowledge Graph (274 evidence-anchored nodes)**, which is then projected into a **17/18-document Forward-Engineering Package** an AI agent can use to *regenerate* the application on a modern stack **without the legacy source**.
3. Wrapped around the pipeline is a full **prompt-governance program** — the prompts that drive the LLM stages were audited, governed, refactored, conformance-checked, optimized, cutover-validated, and redesigned into a clean two-prompt-per-layer model.

So the repo contains **two intertwined things**: (A) the *engine* that does reverse→forward engineering, and (B) the *governance system* that makes the engine's prompts enterprise-grade.

---

## 1. The big picture (one diagram)

```
                          ┌─────────────────────────────────────────────────────────┐
   LEGACY CODEBASE  ─────▶│  LAYER 1 — Deterministic Python extraction (no LLM)      │
   (e.g. eShopOnWeb)      │  source_code.json · database.json · config.json · logs   │
                          └───────────────────────────┬─────────────────────────────┘
                                                       │  (raw, non-authoritative feed)
        ┌──────────────────────┬───────────────────────┼───────────────────────┬───────────────────────┐
        ▼                      ▼                        ▼                       ▼
  BUSINESS (BA)           DATA (DA)              APPLICATION (AA)         TECHNOLOGY (TA)
  layer2 → layer3         DA-extract → review    inventory→…→final→sec     stack-scout → deep-analyst
  rules, capabilities,    schema, ERD, PII,      modules, services,        tech stack, infra, CI/CD,
  processes, 10 BA docs   ownership, data flows  APIs, dependencies        NFRs, security posture
        └──────────────────────┴───────────────────────┬───────────────────────┴───────────────────────┘
                                                        │  (owner-cited facts)
                          ┌─────────────────────────────▼───────────────────────────┐
                          │  FOUNDATION / SYNTHESIS                                  │
                          │  reconcile → ENTERPRISE_KNOWLEDGE_GRAPH.json (274 nodes) │
                          │  → canonical model · inventory · traceability matrix     │
                          └─────────────────────────────┬───────────────────────────┘
                                                        │
                          ┌─────────────────────────────▼───────────────────────────┐
                          │  FORWARD-ENGINEERING PACKAGE (17 docs + #18 deployment)  │
                          │  BRD → capabilities → domain → data → APIs → tech →      │
                          │  security → NFR → generation manifest → readiness        │
                          └─────────────────────────────┬───────────────────────────┘
                                                        ▼
                              [ AI agent regenerates the app on a modern stack ]
```

**Governance wrapper (around the LLM stages):**
```
prompt-governance/  →  prompt-refactored/  →  prompt-resolved/  →  prompt-v2/
(rules & standards)    (GOV-03 prompts)       (runnable, includes     (2-prompt-per-layer
                       w/ {{include}})         expanded)               redesign)
audited by: PROMPT_AUDIT_REPORT · FINAL_PROMPT_CONFORMANCE_REPORT · pilot-cutover-validation
```

---

## 2. What's in the repository (top-level map)

| Directory | What it is | Role |
|---|---|---|
| `bussiness-architecture 1/bussiness-architecture/` | **The engine.** The Python pipeline + runners + legacy prompts + the eShopOnWeb worked output. | Reverse-engineering platform |
| `enterprise-foundation-package/` | The **canonical truth**: `ENTERPRISE_KNOWLEDGE_GRAPH.json` (274 nodes) + 4 read-only views. | Foundation output |
| `forward-engineering-package/` | The **17-doc FE spec** (BRD→readiness) + `16_GENERATION_MANIFEST.json` + the new `18_DEPLOYMENT_ARCHITECTURE.md`. | Forward-engineering input |
| `eshoponweb-forward-engineering/` | A packaged copy (foundation + FE package together) with a README. | Distributable bundle |
| `prompt-governance/` | 10 governance docs (GOV-01…GOV-10): rules, ownership, standard, confidence, foundation spec, dependency model, boundaries, components, target state. | Prompt rules |
| `prompt-refactored/` | 22 governed prompts (GOV-03 structure) + 5 `_shared/` components + reports. | Governed source prompts |
| `prompt-resolved/` | The same 22 prompts with every `{{include}}` expanded → runner-executable. | Production prompts |
| `prompt-v2/` | The redesign: 10 prompts (EXTRACT + VALIDATE per layer) + 3 shared components + reports. | Simplified prompt model |
| `prompt-optimization/` | Analysis that took 21→18 prompts (removed unwired duplicates). | Optimization study |
| `migration-output/` | Cutover plan: replacement matrix, compatibility, regression risk, validation. | Cutover artifacts |
| `pilot-cutover-validation/` | Evidence-grounded pilot validation (6 reports). | Cutover sign-off |
| `PROMPT_AUDIT_REPORT.md` · `FINAL_PROMPT_CONFORMANCE_REPORT.md` | Root-level audit + final conformance. | Top-level audits |

---

## 3. PART A — The Reverse-Engineering Engine

### 3.1 Orchestration — how a run is driven

The entry point is **`run_pipeline.py`** (in `bussiness-architecture 1/bussiness-architecture/`). It:

- Takes `--source` (a Git URL, local path, or `.zip`), `--output`, and `--full-run` / `--repo-root`.
- Always runs **Layer 1** (deterministic extraction) via `Layer1Pipeline`.
- With `--full-run`, then drives every LLM layer **sequentially as subprocesses** (verified in `run_pipeline.py` lines 103–126):

```
layer2/layer2_runner.py            →  layer3/layer3_runner.py            (Business)
data-architecture/da_agent1_runner →  data-architecture/da_agent2_runner (Data)
technology-architecture/ta_agent1  →  technology-architecture/ta_agent2  (Technology, needs --repo-root)
application-architecture/aa_runner                                       (Application, needs --repo-root)
```

Each runner is the same shape: **load prior JSON → build a trimmed context → concatenate `prompt.md` + data → call the `claude` CLI → parse/save outputs.** The pipeline is fully sequential and fault-tolerant (each stage checks prior outputs, retries on missing files, and saves raw output for debugging).

### 3.2 Layer 1 — Deterministic extraction (NO LLM)

`layer1/pipeline.py` runs an 8-step, regex/AST-based extraction — **no model calls**, so it's the reliable factual floor:

| Step | Component | Produces |
|---|---|---|
| 0 | `InputResolver` | clone/unzip/validate source |
| 1 | `LanguageDetector` | primary language → which extractors to use |
| 2 | `FileFilter` | whitelist extensions, blacklist junk dirs |
| 3 | language extractors (dotnet/java/python/javascript) | methods, classes, interfaces, enums + `is_business_artifact` flag |
| 4 | `DatabaseExtractor` | tables, stored procs, triggers, EF entities |
| 5 | `ConfigExtractor` | params, feature flags, connection strings, roles |
| 6 | `LogExtractor` | event/process sequences from logs |
| 7 | `Cleaner` | dedupe (MD5), normalize, drop low-quality |
| 8 | `OutputSaver` | `source_code.json`, `database.json`, `config.json`, `logs.json`, `extraction_summary.json` |

These four JSON files are the **input contract** every downstream LLM layer reads.

### 3.3 The four extraction layers (LLM-driven)

Each layer follows a **Scout → Analyst** rhythm (broad inventory first, deep analysis second — "parse-first, reason-second"):

| Layer | Prompts (legacy) | What it extracts | Key outputs |
|---|---|---|---|
| **Business (BA)** | `layer2_prompt` → `layer3_prompt` | business rules, capabilities, processes, roles, value streams | `layer2_output.json` + **10 BA docs** (capability map, value stream, process models, rules, data model, stakeholders, KPIs, motivation/operating/roadmap) |
| **Data (DA)** | `DA_REVERSE_ENGINEERING` → `DA_REVIEW` | schema, entities, PII, data flows, data quality, ownership | **13 DA files** (schema-catalogue, erd, data-dictionary, pii-inventory, data-flow-map, …) + `review-summary.md` |
| **Application (AA)** | 7-stage chain (`01-inventory` … `07-workflow-audit`) + master spec + `00-global-rules` | modules, components, services, APIs, interfaces, dependencies, call flows, patterns, violations, risks | inventory → parsed → evidence-packs → final (component-registry, dependency-graph, interface-catalogue, call-flow-map, risk register, diagrams) |
| **Technology (TA)** | `TA_STACKSCOUT` → `TA_DEEPANALYST` | tech stack, infrastructure, CI/CD, NFRs, tech debt, security posture | 6 inventory files + assessments (stack, patterns, NFR registry, tech-debt register, security) |

The AA layer is special: it has a **deterministic Python analyzer** (`tools/application_architecture_analyzer/`) that produces inventory→parsed→evidence-packs **before** the LLM stages synthesize the final architecture — the strictest "parse-first" discipline in the system.

### 3.4 The Foundation / Synthesis layer

This is where the four parallel layers **converge**. It reconciles cross-track facts (the same entity described by BA, DA, AA, TA becomes **one canonical node**), resolves conflicts by an evidence hierarchy, and emits:

- **`ENTERPRISE_KNOWLEDGE_GRAPH.json`** — the single source of truth, 9 sections:
  `metadata · business · data · application · technology · cross_links · assumptions · normalization_log · open_questions`
- **4 read-only views**: `CANONICAL_ENTERPRISE_MODEL.md`, `ARCHITECTURE_INVENTORY.md`, `TRACEABILITY_MATRIX.md`, `FORWARD_ENGINEERING_INPUT_MAP.md`

**The eShopOnWeb graph by the numbers (real, parsed):**

| Section | Contents | Count |
|---|---|---|
| business | capabilities 39, actors 5, processes 10 | 54 |
| data | entities 15, relationships 12, aggregates 4, repositories 4 | 35 |
| application | services 47, interfaces 13, apis 55, dependencies 19 | 134 |
| technology | current_stack 26, infrastructure 8, security 17 | 51 |
| cross_links | capability→process 17, process→entity 29, entity→service 16, service→api 55 | 117 |
| assumptions / open_questions | — | 7 / 9 |
| **Total nodes** | | **274** |

Every node carries `id`, `type`, `owner_layer`, `confidence` (HIGH/MEDIUM/LOW/ASSUMED/DISCREPANCY), and `evidence` (file:line citations).

### 3.5 The Forward-Engineering Package — the deliverable

The graph is projected into a **17-document specification** (+ the deployment doc #18 added later) that an AI agent consumes to regenerate the app on a new stack:

| # | Document | Layer |
|---|---|---|
| 01 | Business Requirements (BRD) | Business |
| 02 | Business Capability Model | Business |
| 03 | Use Case Specification | Business |
| 04 | Business Process Model | Business |
| 05 | Domain Model (DDD) | Data/Domain |
| 06 | Data Dictionary | Data |
| 07 | Data Model Specification | Data |
| 08 | ERD | Data |
| 09 | Data Flow Diagram (L0/L1/L2) | Data |
| 10 | Service Catalog | Application |
| 11 | API Contract Specification | Application |
| 12 | Technology Blueprint | Technology |
| 13 | Security Architecture | Technology |
| 14 | NFR Specification | Technology |
| 15 | Forward Engineering Specification (master, GATE rules) | Cross-cutting |
| 16 | Generation Manifest (`.json`, machine-readable) | Cross-cutting |
| 17 | Forward Engineering Readiness Report | Assessment |
| **18** | **Deployment Architecture** (supplementary, added 2026-06-24) | Technology |

**Readiness verdict (from doc 17): CONDITIONAL — 79/100.** The package is structurally strong (full capability→process→entity→service→API traceability, status flags honored, machine-consumable manifest), but not unconditionally ready because: the legacy module dependency **cycle** (`APP-DEP-001`) must be re-architected not reproduced; the supply chain is version-unknown; the **target stack is empty by design** (a human must pick one); and several environment/security open questions remain.

### 3.6 The golden rule that makes it trustworthy — anti-hallucination

The entire engine is built on **"never invent a fact."** Empty target stack stays empty. Unknown versions stay `unknown`. Inferred capabilities are labeled `inferred/LOW`. Security gaps are recorded as findings, not silently fixed. This is why the readiness report is *conservative* (79, not 95) — it treats every `aspirational`/`inferred`/`LOW`/`unknown` flag as a gap, not a feature.

---

## 4. PART B — The Prompt-Governance Journey

The prompts that drive the LLM layers were themselves put through an enterprise-grade lifecycle. This is the second half of what the repo *is*.

### 4.1 The seven stages (each produced a package)

| Stage | Package | What happened | Headline result |
|---|---|---|---|
| 1. **Audit** | `PROMPT_AUDIT_REPORT.md` | Audited all 20 legacy prompts for uniformity, duplication, layer violations | Uniformity 46/100; **3 paradigms, 3 confidence schemes, 4 cross-layer violations, data extraction duplicated ×5** |
| 2. **Govern** | `prompt-governance/` (GOV-01…10) | Wrote the single-source rules, ownership matrix, prompt standard, confidence model, foundation spec, dependency model, boundaries, reusable components | One rulebook every prompt must follow |
| 3. **Refactor** | `prompt-refactored/` | Rewrote 20 legacy → 22 governed prompts (GOV-03 12-section structure); governance pulled into 5 `_shared/` components; created the Foundation prompts | All inline duplication removed |
| 4. **Conformance** | `FINAL_PROMPT_CONFORMANCE_REPORT.md` | Independently re-checked all 22 prompts | **98/100, all PASS**; 4 violations confirmed closed |
| 5. **Optimize** | `prompt-optimization/` | Removed unwired duplicate prompts, re-merged an artificially-split one | **21 → 18 prompts** (−14%) |
| 6. **Cutover** | `migration-output/` + `prompt-resolved/` + `pilot-cutover-validation/` | Resolved every `{{include}}`; validated replacement against real eShopOnWeb outputs | **PASS WITH CONDITIONS** — 98.7% output-compat, 100% governance, 274-node graph preserved |
| 7. **Redesign** | `prompt-v2/` | Collapsed to a uniform **2-prompt-per-layer** model (EXTRACT + VALIDATE) | **18 → 10 prompts**, 100% compatible |

### 4.2 The four cross-layer violations that were fixed (the heart of the cleanup)

| Violation (audit found) | Fix |
|---|---|
| Technology layer doing **Data** work (transaction/consistency assessment) | Relocated to Data layer |
| Technology layer doing **Application** security work | Relocated to Application layer |
| Application layer authoring the **business capability map** | Relocated to Business layer; AA now *consumes* it |
| Application layer authoring the **data-ownership map** | Relocated to Data layer; AA now *consumes* it |

The principle: **each fact has exactly one owning layer; everyone else consumes-and-cites.** This is what stops the same entity being extracted five different ways with five different answers.

### 4.3 The governance rulebook (GOV-01…GOV-10) in one line each

| Doc | Rule |
|---|---|
| GOV-01 | Global rules (GR-1…GR-10): anti-hallucination, evidence hierarchy, exclusions, no-modification, parse-first, validation, output, model pinning |
| GOV-02 | Ownership matrix — who owns what, who may only consume |
| GOV-03 | The canonical 12-section prompt template |
| GOV-04 | One confidence model: HIGH/MEDIUM/LOW/ASSUMED/DISCREPANCY (replaced 3 schemes) |
| GOV-05 | Foundation/synthesis layer spec + reconciliation algorithm |
| GOV-06 | Per-prompt refactoring/migration plan |
| GOV-07 | Dependency model — DAG that terminates at Foundation |
| GOV-08 | Layer boundaries — May Extract / Consume / Produce / Must-Not |
| GOV-09 | 5 reusable components (CMP-GOV/CONF/VALID/EVID/OUT) |
| GOV-10 | Target end-state + readiness scoring |

### 4.4 The three prompt "generations" that now coexist

1. **Legacy** (`bussiness-architecture 1/.../*.md`) — what the runners load *today*; the originals.
2. **Refactored + Resolved** (`prompt-refactored/` is editable source with `{{include}}`; `prompt-resolved/` is the runnable, includes-expanded version) — the governed 18-prompt set.
3. **prompt-v2** (`prompt-v2/`) — the simplified 10-prompt redesign (2 per layer).

Editing rule: change `prompt-refactored/` (or a `_shared/` component), then re-resolve into `prompt-resolved/`. Never hand-edit resolved files.

---

## 5. End-to-end walkthrough (one concrete run, eShopOnWeb)

```
1. INPUT      python run_pipeline.py --source <eShopOnWeb> --output out/ --full-run --repo-root <src>

2. LAYER 1    Python extractors scan the repo (no LLM)
              → source_code.json (203 methods, 131 business), database.json (7 tables),
                config.json (298 params), logs.json, extraction_summary.json

3. BUSINESS   layer2_runner: methods+config → layer2_output.json (rules, entities, capabilities)
              layer3_runner: that JSON → 10 BA documents (capability map … roadmap)

4. DATA       da_agent1: code + live DB attempt → 13 DA files (schema, ERD, PII, ownership)
              da_agent2: validates/enriches → review-summary.md + Gate verdict

5. TECHNOLOGY ta_agent1: manifests/Docker/CI-CD → 6 inventory files
              ta_agent2: deep analysis → stack/pattern/NFR/debt/security assessments

6. APPLICATION aa_runner: Python analyzer (inventory→parsed→evidence-packs)
              → LLM stages (final architecture, forward-eng inputs, security, reviews)

7. FOUNDATION reconcile all four layers → ENTERPRISE_KNOWLEDGE_GRAPH.json (274 nodes)
              → canonical model + inventory + traceability matrix

8. FORWARD-ENG project the graph → 17-doc package + generation manifest + readiness (79/100)
              → 18_DEPLOYMENT_ARCHITECTURE.md (deployment view)

9. OUTPUT     an AI agent can now regenerate eShopOnWeb on Java/Node/Python/.NET +
              React/Angular/Vue + Postgres/SQLServer/MySQL — WITHOUT the legacy source,
              once a human picks the target stack (it is empty by design).
```

---

## 6. What the project *has* (capability inventory)

- ✅ **Deterministic extraction floor** (Layer 1, no LLM) — reliable facts before any reasoning.
- ✅ **Four TOGAF-aligned reverse-engineering layers** (BA/DA/AA/TA), each Scout→Analyst.
- ✅ **A canonical knowledge graph** (274 nodes, 9 sections, full citations).
- ✅ **A traceability matrix** — capability → process → entity → service → API, end to end.
- ✅ **A forward-engineering package** (17 docs + deployment) that is technology-neutral.
- ✅ **A machine-consumable generation manifest** (strict-valid JSON, graph-grounded).
- ✅ **An enterprise prompt-governance system** (10 governance docs, 5 reusable components).
- ✅ **Three prompt generations** with a validated migration path between them.
- ✅ **Full audit trail**: audit → conformance → optimization → cutover → pilot validation.
- ✅ **Anti-hallucination discipline** enforced everywhere (the reason it's trustworthy).

## 7. What it deliberately does NOT do / known limits

- ❌ **Does not pick a target stack** — empty by design; a human decides (the #1 pre-generation gate).
- ❌ **Does not reproduce legacy defects** — the module dependency cycle and endpoint→repo violations are flagged to be re-architected, not regenerated.
- ⚠️ **Deployment is local-dev-ready, production-incomplete** — containers + compose exist; no production orchestration/IaC/release automation in evidence (see doc 18).
- ⚠️ **Version-unknown supply chain** — most dependency versions are `LOW`/undeclared.
- ⚠️ **prompt-v2 + Foundation runners are partly deferred** — they are *authored and validated* but the executable runner wiring is the remaining code step (out of scope of the prompt work).
- ⚠️ **Two copies of the FE package exist** (`forward-engineering-package/` and `eshoponweb-forward-engineering/...`) — doc 18 + its registrations currently live only in the first.

---

## 8. How to navigate the repo (reading order)

1. **Want the engine?** Start at `bussiness-architecture 1/bussiness-architecture/run_pipeline.py` and `PIPELINE_OVERVIEW.md`.
2. **Want the result?** Read `enterprise-foundation-package/CANONICAL_ENTERPRISE_MODEL.md`, then `forward-engineering-package/` docs 01→17, then `18_DEPLOYMENT_ARCHITECTURE.md`.
3. **Want the governance story?** `PROMPT_AUDIT_REPORT.md` → `prompt-governance/00_README.md` → `FINAL_PROMPT_CONFORMANCE_REPORT.md` → `pilot-cutover-validation/06_EXECUTIVE_SUMMARY.md`.
4. **Want the final prompt design?** `prompt-v2/` (start with `02_EXECUTION_PIPELINE.md`).

---

*This document is a navigational/explanatory overview. The authoritative source of truth for all
architecture facts is `enterprise-foundation-package/ENTERPRISE_KNOWLEDGE_GRAPH.json`; the authoritative
governance source is `prompt-governance/`. Where they and this summary differ, they win.*
