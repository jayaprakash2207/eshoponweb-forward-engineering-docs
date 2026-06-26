# PROMPT ARCHITECTURE AUDIT REPORT

**Project:** Forward-Engineering / Reverse-Engineering Enterprise Pipeline (`frwd engg - op's`)
**Audit type:** Enterprise Prompt Architecture & TOGAF Layer-Separation Review
**Audit date:** 2026-06-24
**Auditor role:** Enterprise Architect · Prompt Architect · AI Systems Architect · TOGAF Reviewer
**Mode:** Read-only. No files were modified.
**Scope:** All prompts, agents, skills, architecture layers, and orchestration files under
`bussiness-architecture 1/bussiness-architecture/`. The top-level `enterprise-foundation-package/`,
`forward-engineering-package/`, and `eshoponweb-forward-engineering/` trees are **generated output
artifacts** (read-only views over `ENTERPRISE_KNOWLEDGE_GRAPH.json`) and are treated as pipeline
output evidence, not as prompts.

---

## 0. Executive Summary

The system is a multi-layer reverse-engineering pipeline that extracts a TOGAF-style enterprise model
(Business → Data → Application → Technology) from a legacy codebase, then emits a forward-engineering
package. The **prompt engineering quality within each track is high** — strong anti-hallucination
discipline, evidence/citation mandates, confidence scoring, and cumulative-registry traceability are
present almost everywhere.

The **architectural problem is uniformity and ownership, not quality.** The repository contains **three
mutually incompatible prompt paradigms** that evolved independently, **three different confidence
schemes**, **five duplicated copies of the same exclusion/junk-folder rule**, and **at least four
confirmed cross-layer ownership violations** where one architecture layer performs another layer's
extraction. The same extraction task (data-store/entity discovery) is performed independently by **five
different components**.

### Headline Scores

| Score | Value | Direction | Verdict |
|---|---|---|---|
| **Uniformity Score** | **46 / 100** | higher = better | ⚠️ Poor — 3 divergent paradigms, 3 confidence schemes |
| **Duplication Score** | **63 / 100** | higher = *worse* (severity) | ⚠️ High — rules & orchestration copied 5–7× |
| **Layer Separation Score** | **54 / 100** | higher = better | ⚠️ Weak — 4 confirmed violations, 1 shared task ×5 |
| **Architecture Quality Score** | **64 / 100** | higher = better | 🟡 Moderate — strong craft, weak governance |

> **Bottom line:** The prompt architecture is **NOT uniform** and does **NOT** consistently follow
> layer separation. It is salvageable with consolidation rather than a rewrite: extract shared
> governance/boilerplate to single sources, standardize one paradigm, and relocate four
> mis-owned outputs.

---

## 1. Prompt Inventory

20 prompt / agent / governance / orchestration-doc files were identified across four architecture
layers plus the core pipeline. (Python runners are orchestration, not prompts; they are inventoried in
§5–§6.)

| # | File (relative to `bussiness-architecture 1/bussiness-architecture/`) | Layer | Kind | Lines (~) | Words (~) |
|---|---|---|---|---|---|
| P1 | `BA_Agent1_StructuralScout_v3.md` | Business | Scout prompt | 372 | 5,100 |
| P2 | `BA_Agent2_DeepAnalyst_v3.md` | Business | Deep-analyst prompt | 534 | 7,500 |
| P3 | `BA_Pipeline_Execution_Plan.md` | Business | Meta-guide (aspirational) | 1,566 | 11,200 |
| P4 | `layer2/layer2_prompt.md` | Core L2 (Business) | Working prompt (JSON) | 155 | 950 |
| P5 | `layer3/layer3_prompt.md` | Core L3 (Business) | Working prompt (MD docs) | 332 | 2,300 |
| P6 | `data-architecture/DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md` | Data | Scout/extraction prompt | 182 | 2,400 |
| P7 | `data-architecture/DA_REVIEW_PROMPT.md` | Data | Review/gate prompt | 165 | 2,200 |
| P8 | `technology-architecture/TA_STACKSCOUT_PROMPT.md` | Technology | Scout prompt | 546 | 6,100 |
| P9 | `technology-architecture/TA_DEEPANALYST_PROMPT.md` | Technology | Deep-analyst prompt | 745 | 7,600 |
| P10 | `application-architecture/AGENTS.md` | Application | Orchestrator | 90 | 600 |
| P11 | `application-architecture/application_architecture_extraction_agent_prompt.md` | Application | Master spec | 1,385 | 8,000 |
| P12 | `application-architecture/architecture-prompts/00-global-rules.md` | Application | Governance (source) | 59 | 350 |
| P13 | `…/architecture-prompts/01-inventory-agent.md` | Application S1 | Stage prompt | 49 | 280 |
| P14 | `…/architecture-prompts/02-parser-symbol-agent.md` | Application S2 | Stage prompt | 69 | 350 |
| P15 | `…/architecture-prompts/03-evidence-pack-agent.md` | Application S3 | Stage prompt | 51 | 300 |
| P16 | `…/architecture-prompts/04-final-architecture-agent.md` | Application S4 | Stage prompt | 55 | 310 |
| P17 | `…/architecture-prompts/05-enterprise-forward-engineering-agent.md` | Application S5 | Stage prompt | 55 | 310 |
| P18 | `…/architecture-prompts/06-quality-review-agent.md` | Application S6 | Stage prompt | 43 | 250 |
| P19 | `…/architecture-prompts/07-workflow-audit-agent.md` | Application S7 | Stage prompt | 48 | 280 |
| P20 | `…/tools/application_architecture_analyzer/ARCHITECTURE_EXTRACTION_WORKFLOW.md` | Application | Automation doc | 210 | 1,100 |

**Skills:** No project-defined Claude *skills* (`.claude/skills/`) exist in the pipeline. The "agents"
here are prompt-file + Python-runner pairs invoked through the `claude` CLI, not registered subagents.
The only `.claude/` content is `settings.local.json` and the AA `settings.json`. **Flag:** the task
asked to review "all skills" — there are none to review; the agent abstraction is implemented entirely
as prompt-files + CLI runners.

### Per-Prompt Detail (file · layer · purpose · inputs · outputs · duplicated logic · overlap %)

Overlap % below = estimated fraction of the prompt's *instructional content* that restates rules,
schemas, or extraction logic already defined in another prompt.

| Prompt | Purpose | Inputs | Outputs | Duplicated logic | Overlap % |
|---|---|---|---|---|---|
| **P1 BA Scout** | Structural inventory of codebase (no interpretation) | Source code, folder tree, entity/route/enum/role/service files | 6 MD inventory tables + scan summary + validation queue | Exclusion list; confidence (✅/⚠️); chunk-continuity; "never read method bodies" | ~25% |
| **P2 BA DeepAnalyst** | Turn P1 inventory into business docs (rules, processes, value streams) | P1's 6 files + source code | 8 business MD docs + discrepancy log | "Scout+DeepAnalyst" structure; confidence; never-reset registry; plain-English mandate | ~35% |
| **P3 BA Exec Plan** | Aspirational 3-layer (extract→SLM→LLM) design narrative | Legacy app, tool matrices | 10 BA docs (described, not executed) | Re-describes L1/L2/L3 tasks already in P4/P5; tool matrices; risk pattern | ~55% |
| **P4 Layer2 prompt** | Extract business rules/entities/roles/processes/capabilities as JSON | `source_code.json`, `database.json`, `config.json` | 1 JSON (`layer2_output.json`) | "5 tasks" duplicated in P3; anti-infra rule | ~40% |
| **P5 Layer3 prompt** | Render L2 JSON into 10 business MD docs | `layer2_output.json` | 10 MD docs (delimiter-wrapped) | Doc templates partially overlap P2 outputs; plain-English mandate | ~30% |
| **P6 DA Reverse-Eng** | Extract schema/PII/data-flows/data-layer rules + live DB | Source code, migrations, repos, connection strings, live DB | 13 DA artifacts | Evidence Strength Hierarchy (verbatim in P7); SHARED-component marking; confidence (numeric) | ~30% |
| **P7 DA Review** | Validate/enrich P6 outputs; Gate G1 readiness | P6's 13 files, tests, docs, live DB | 13 updated files + `review-summary.md` | Evidence Strength Hierarchy (verbatim from P6); confidence | ~30% |
| **P8 TA StackScout** | Tech-stack/infra/CI-CD inventory (declaration level) | Manifests, Dockerfiles, k8s, IaC, CI/CD, config | 6 TA inventory MD files | "Scout" structure mirrors P1; exclusion list; confidence; chunk-continuity | ~35% |
| **P9 TA DeepAnalyst** | Patterns, NFRs, tech debt, component interactions, CI/CD maturity | P8's 6 files + source bodies + pipelines | 8 TA assessment files | "DeepAnalyst" structure mirrors P2; NEVER rules; confidence; **+ Data & Security assessments (cross-layer)** | ~40% |
| **P10 AA AGENTS.md** | Orchestrate AA staged workflow | Stage prompts, global rules | Drives stages (no direct output) | Restates `00-global-rules.md` golden rules verbatim | ~50% |
| **P11 AA Master Spec** | Full 13-stage AA extraction spec | Legacy repo, output root | Full AA package (~15 artifacts) | Restates `00-global-rules.md` inline (evidence, junk-folder, no-modify) | ~30% |
| **P12 AA 00-global-rules** | **Source of truth** for AA governance | n/a | n/a (rules) | None — this is the canonical file | 0% (source) |
| **P13–P19 AA stages 01–07** | One concern per stage (inventory→audit) | Prior stage's JSON only | Stage-scoped JSON/MD | Minimal inline duplication; cite global rules well | ~5–10% |
| **P20 AA Workflow doc** | Document Python automation layer | Phase scripts, prompts | Run-summary docs | Restates safety rules in automation context | ~15% |

---

## 2. Prompt Ownership Matrix

Which layer **does** own each prompt today, and whether ownership is clean.

| Prompt(s) | Declared Owner | Actually Performs Work For | Clean? |
|---|---|---|---|
| P1, P2 | Business | Business (+ touches Data via entity relationships, App via call chains) | 🟡 mostly |
| P3 | Business | Business + Data (DDL/procs/triggers) + Technology (infra/cloud) | 🔴 leaks |
| P4 | Business (Core L2) | Business + Data (entity relationships) + Technology (config keys) + App (controller access) | 🟡 mild |
| P5 | Business (Core L3) | Business (+ Data model doc, Operating/HR model) | 🟡 mild |
| P6, P7 | Data | Data only | ✅ clean |
| P8 | Technology | Technology only | ✅ clean |
| P9 | Technology | Technology **+ Data (OUTPUT 4) + Security/App (OUTPUT 5)** | 🔴 leaks |
| P10, P11 | Application | Application (+ refs Business rules, Technology cloud terms) | 🟡 mild |
| P12–P16, P18, P19 | Application | Application only | ✅ clean |
| P17 (Stage 05) | Application | Application **+ Business (capability map) + Data (data-ownership map)** | 🔴 leaks |
| P20 | Application | Application only | ✅ clean |

---

## 3. Prompt Duplication Analysis

### 3.1 Duplicated governance / boilerplate (the costly kind)

| Duplicated block | Appears in | Copies | Should be |
|---|---|---|---|
| **Exclusion / junk-folder list** (`node_modules`, `.git`, `dist`, `build`, `bin`, `obj`, `*.min.js`, `*.map`…) | P1, P8, P11, P12, + `layer1/file_filter.py` | **5** | One shared constant + one prompt include |
| **"Do not invent / use `unknown` / every claim needs evidence"** | All ~20 prompts (varied wording) | ~20 | One canonical anti-hallucination block, referenced |
| **AA golden rules** (no-modify, evidence, parse-first) | P12 (source), restated verbatim in P10 & P11 | 3 | P10/P11 cite P12, not copy |
| **Evidence Strength Hierarchy** (live DB > migrations > entity code > …) | P6 and P7 **verbatim** | 2 | Shared DA fragment |
| **"Scout + DeepAnalyst" two-agent scaffolding** (orientation pass, chunk plan, validation queue, confidence legend) | P1/P2, P6(implied), P8/P9 | 3 tracks | One shared agent template, parameterized |
| **Confidence-marking instructions** | P1, P2, P6, P7, P8, P9 (3 different schemes) | 6 | One enterprise confidence standard |
| **Claude CLI command builder `_claude_cmd()`** | 7 runner files | **7** | One `common/_claude_cli.py` |
| **Marker-parsing regex** (`===X_START===…===X_END===`) | 6 runner files | **6** | One `common/parse_marked_files()` |
| **Task-file save + "manual run" instructions** | 7 runner files | **7** | One shared helper |

### 3.2 Confidence-scheme fragmentation (a uniformity defect)

Three incompatible schemes are in use simultaneously:

1. **Numeric** (P6/P7 Data): `1.0` confirmed-by-DB … `0.70` naming … `<0.70 → UNKNOWN`.
2. **Categorical** (P1/P2/P8/P9 BA & TA): `HIGH / LOW-reason / ASSUMED / DISCREPANCY`.
3. **Gate verdicts** (P18/P19 AA): `PASS / PARTIAL / FAIL` and `ENTERPRISE READY / … / NOT READY`.

A downstream consumer cannot compare confidence across layers without a translation table — none exists.

### 3.3 Paradigm fragmentation (the root uniformity defect)

| Paradigm | Used by | Output mechanism | Governance |
|---|---|---|---|
| **A — Scout + DeepAnalyst** | BA (P1/P2), DA (P6/P7), TA (P8/P9) | Markdown tables, agent writes to disk | Inline NEVER rules per file |
| **B — Staged global-rules pipeline** | AA (P10–P20) | JSON evidence packs → markers | Centralized `00-global-rules.md` |
| **C — SLM/LLM JSON contract** | Core layer2/layer3 (P4/P5) | Strict JSON schema → MD split | Inline "important rules" |

Paradigm **B** is the most mature (centralized governance, parse-first discipline, one concern per
stage). Paradigms A and C reinvent governance inline. **This split is the single biggest driver of the
low Uniformity Score.**

---

## 4. Cross-Layer Responsibility Analysis

### 4.1 Confirmed flagged violations

> **🔴 Business Architecture performs Data Architecture work**
> - **P3 `BA_Pipeline_Execution_Plan.md`** — Layer 1 extracts table DDL, stored procedures, triggers,
>   views, and foreign-key relationships. These are Data Architecture primitives. Mitigated only by
>   downstream translation to business entities.
> - **P4 `layer2_prompt.md`** — emits entity `relationships` (`"belongs to Customer"`, `"contains
>   OrderItems"`) → data-model/cardinality is Data Architecture.

> **🔴 Data Architecture performs Application Architecture work**
> - **None found.** P6/P7 stay cleanly within the data layer. ✅ This is the best-isolated layer.

> **🔴 Application Architecture performs Technology Architecture work**
> - **P11 master spec** references inventing "Kubernetes / cloud provider / API gateway" in its
>   hallucination warnings and detects framework/technology in Stage 3 — technology-stack concerns
>   belong to TA (P8). Borderline (frameworks inform app structure) but explicitly tech-layer language.
> - **Inverse also true (flagged for completeness): 🔴 Technology Architecture performs Data &
>   Application work** — **P9 `TA_DEEPANALYST_PROMPT.md` OUTPUT 4 (Data Architecture Assessment:
>   transaction scope, consistency model, connection-pool config, migration state)** is Data
>   Architecture work; **OUTPUT 5 (Security Architecture Assessment: auth completeness, CORS, secrets)**
>   is Security/Application work. Confirmed by output files `ta-outputs/data-architecture-assessment.md`
>   and `ta-outputs/security-architecture-assessment.md`.

> **🔴 Application Architecture performs Business & Data work**
> - **P17 Stage 05** produces `business-capability-map.json/.md` (Business Architecture artifact) and
>   `data-ownership-map.md` (Data Architecture artifact). Confirmed present in
>   `output/.../aa-outputs/final/`.

### 4.2 Multiple layers performing the SAME extraction task

| Extraction task | Performed independently by | # of owners |
|---|---|---|
| **Data-store / entity / schema discovery** | `layer1/database_extractor.py`; P4 layer2; **P6 DA**; P8 TA (Data Store Registry); P1 BA (Entity Inventory) | **5** |
| **Technology-stack detection** | `layer1/config_extractor.py`; P8 TA StackScout; AA `01-inventory` (framework detection) | 3 |
| **Business-capability mapping** | P2/P4/P5 BA; **P17 AA Stage 05** | 2 |
| **Component / service extraction** | AA P14/P15; TA P8 (Component & Service Map); P9 | 3 |
| **Business-rule extraction** | P2 BA; P4 layer2; P6 DA ("hidden business rules"); `layer1` keyword heuristic | 4 |

This is the most material finding: **data-store and business-rule extraction each happen in 4–5 places
with no shared canonical extractor and no reconciliation step.** Each layer re-derives the same facts
from raw source with its own heuristics, risking divergent answers for the same entity.

---

## 5. Prompt Dependency Analysis

Execution is **fully sequential**; each stage reads the prior stage's on-disk artifacts.

```
[Layer 1: Python extraction]  → source_code.json, database.json, config.json, logs.json
        │
        ├──► [Layer 2 prompt P4] → layer2_output.json
        │          └──► [Layer 3 prompt P5] → 10 BA docs
        │
        ├──► [DA Agent 1 P6] → 13 DA files
        │          └──► [DA Agent 2 P7] → reviewed 13 + review-summary.md
        │
        ├──► [TA Agent 1 P8] → 6 TA inventory files      (needs --repo-root)
        │          └──► [TA Agent 2 P9] → 8 TA assessments
        │
        └──► [AA Python stages 0–3] → evidence packs
                   └──► [AA LLM P16→P17→P18→P19] → final + forward-eng + review + audit
```

**Dependency contracts:**
- **Within a track:** explicit and clean (P2 refuses to start without P1's 6 files; P7 consumes P6's 13;
  AA stage N reads only stage N−1).
- **Across tracks:** **no dependency** — BA, DA, TA, AA all fan out from Layer 1 independently and never
  reconcile. This is why the same entity can be described four times (see §4.2). There is **no
  cross-track join** until the (separately generated) `ENTERPRISE_KNOWLEDGE_GRAPH.json` foundation
  package — which is produced outside these prompts.
- **Dead/aspirational dependency:** P3 describes an SLM+vector-DB architecture (Phi-3/Mistral, embeddings)
  that the actual runners do **not** implement (runners call the `claude` CLI directly). P3 is
  documentation drift and should be marked superseded by P4/P5.

**Input contract uniformity:** Every runner follows the same shape — load prior JSON → build trimmed
context → concat `prompt.md` + JSON block → run CLI → parse/save. But **context-trimming caps differ
per runner** (80 methods / adaptive 20–80 / 3000-char files / 4000-char files / 40 files) with no shared
config, and **output capture diverges** (Layer 3 & AA parse stdout markers; DA/TA write to disk via
`--permission-mode acceptEdits`). **No model is pinned anywhere** — all rely on the CLI default, so audit
reproducibility is not guaranteed.

---

## 6. Layer Boundary Validation

| Boundary | Expected separation | Observed | Verdict |
|---|---|---|---|
| Business ↔ Data | BA names business entities; DA owns schema/PII/keys | P3/P4 extract DDL & relationships | ⚠️ leak |
| Data ↔ Application | DA owns data; AA owns components/flows | Clean both directions | ✅ |
| Application ↔ Technology | AA owns app structure; TA owns stack/infra | P11 references cloud/k8s & frameworks | 🟡 minor |
| Technology ↔ Data | TA owns stack; DA owns data semantics | **P9 OUTPUT 4 does data deep-dive** | 🔴 violation |
| Technology ↔ Security/App | TA may own infra-security only | **P9 OUTPUT 5 does app-level security** | 🔴 violation |
| Application ↔ Business/Data | AA forward-eng inputs only | **P17 emits capability + data-ownership maps** | 🔴 violation |
| Governance centralization | One rules source per track | Only AA centralizes (P12); BA/DA/TA inline | ⚠️ inconsistent |

**Parse-first-reason-second discipline:** Enforced and excellent in AA (P12 + staged packs). BA/DA/TA
push raw source straight into a deep-analysis prompt with no intermediate evidence-pack contract —
weaker boundary between "facts" and "interpretation."

---

## 7. Architecture Consistency Review

**Consistent (strengths):**
- Anti-hallucination / evidence-citation present in **every** prompt.
- Sequential, fault-tolerant orchestration with retry + raw-output capture per stage.
- Cumulative-ID traceability (BR-IDs, NFR-IDs, TD-IDs never reset within a track).
- The two-agent Scout→DeepAnalyst rhythm is *conceptually* uniform across BA/DA/TA.

**Inconsistent (defects):**
1. **3 paradigms** (A/B/C) — §3.3.
2. **3 confidence schemes** — §3.2.
3. **Output capture** divergence (markers vs disk writes).
4. **Context caps** ad-hoc per runner.
5. **Governance**: centralized in AA, inline everywhere else.
6. **No model pinning / no prompt version headers** — prompts carry `v2`/`v3` in filenames but no
   machine-readable version metadata; changing a prompt silently changes all future runs.
7. **Doc drift** — P3 describes an unimplemented architecture.

---

## 8. Reusability Review

| Asset | Reusable today? | Issue |
|---|---|---|
| `00-global-rules.md` (P12) | ✅ Yes, and reused by AA stages | Not reused by BA/DA/TA — they inline equivalents |
| Exclusion/junk list | ❌ Copy-pasted ×5 | No shared include or constant |
| Confidence legend | ❌ 3 variants | No canonical standard |
| Scout/DeepAnalyst scaffold | ❌ Re-authored ×3 | No parameterized template |
| Evidence Strength Hierarchy | ❌ Verbatim ×2 | No shared fragment |
| Orchestration boilerplate (CLI/markers/task-save) | ❌ Copy-pasted 6–7× | No `common/` module |
| AA staged-prompt pattern (one concern/stage, cite-don't-copy) | ✅ Strong reuse model | Should be the template for *all* layers |

**Reusability is structurally low** despite high conceptual repetition — the building blocks exist but
are duplicated rather than referenced. The AA stage pattern (P12–P19) is the proven template to
generalize.

---

## 9. Token Efficiency Review

| Finding | Impact |
|---|---|
| **P3 (1,566 lines / ~11k words)** is an aspirational guide loaded by nothing executable | Pure dead weight; remove or archive |
| **P11 (1,385 lines)** restates global rules + junk lists inline | ~30% redundant tokens per AA stage-04 prompt that also injects P12 |
| Exclusion list shipped 5× | Each prompt that embeds it pays for ~20 lines that could be one reference |
| Confidence/anti-hallucination re-explained in every prompt | Cumulative tax across 20 prompts |
| Per-file truncation caps (3000–4000 chars) **are** good practice | ✅ keeps deep-analysis prompts bounded |
| AA stages are lean (43–69 lines, cite global rules) | ✅ token-efficient model |
| No prompt caching strategy (CLI `--no-session-persistence`) | Shared preambles re-sent every call; a cached system-prompt block would cut repeated governance tokens |

**Estimated waste:** consolidating duplicated governance + removing P3 + slimming P11 would cut roughly
**15–25%** of instructional tokens across the LLM stages with **zero** loss of behavior.

---

## 10. Refactoring Opportunities (Recommended)

Priority ordered. None require changing extraction *behavior* — they consolidate and relocate.

### P0 — Stop multi-layer duplicate extraction (correctness risk)
1. **Designate single owners** (see Ownership Matrix below). Data-store/entity discovery → **Data layer
   only**; all other layers *consume* DA output instead of re-extracting. Add a cross-track
   reconciliation step (the knowledge-graph join) as a first-class pipeline stage, not an external
   package.

### P0 — Relocate mis-owned outputs (layer separation)
2. Move **P9 OUTPUT 4 (data assessment)** → consumed from DA; keep only infra-level data config in TA.
3. Move **P9 OUTPUT 5 (app/security assessment)** → a dedicated Security concern or App layer.
4. Move **P17 Stage 05 `business-capability-map.*`** → Business layer; `data-ownership-map.md` → Data
   layer. AA should *reference* these, not author them.

### P1 — Unify governance (uniformity + tokens)
5. Promote `00-global-rules.md` to a **repo-wide** `GLOBAL_PROMPT_RULES.md`; have **every** prompt
   (BA/DA/TA/AA) cite it instead of inlining anti-hallucination, junk-folder, and no-modify rules.
6. Adopt **one confidence standard** (recommend the categorical `HIGH/LOW/ASSUMED/DISCREPANCY` plus an
   optional numeric score) and delete the other two schemes.
7. Extract a single **Scout/DeepAnalyst template** parameterized per layer; BA/DA/TA become
   instantiations, matching AA's staged model.

### P1 — Consolidate orchestration boilerplate
8. Create `common/` with `_claude_cli.py` (one command builder + `--model` support),
   `markers.py` (one parser), `task_io.py` (task-save + manual-run print), and `context_caps.py`
   (one config for all trimming caps).

### P2 — Governance metadata & hygiene
9. Add a version header to every prompt (`version`, `updated`, `owner_layer`, `consumes`, `produces`).
10. **Pin the model** per run via `run_pipeline.py --model` propagated to all runners (audit
    reproducibility).
11. Archive **P3** (`BA_Pipeline_Execution_Plan.md`) as superseded; it describes an unimplemented
    SLM/vector-DB design.

---

## Recommended Ownership Matrix (target state)

Which layer **should** own each responsibility:

| Responsibility | Should be owned by | Currently also done by (to remove) |
|---|---|---|
| File/project/language inventory | **Application (AA Stage 01)** / Layer 1 | — |
| Symbol / component / call-flow extraction | **Application** | TA (component map), partial |
| Architecture pattern / violation / risk (app) | **Application** | — |
| Business capability mapping | **Business** | AA Stage 05 (relocate) |
| Business rules / processes / value streams | **Business** | DA "hidden rules", layer1 heuristic (keep as hints only) |
| Stakeholder / role / operating model | **Business** | — |
| Schema / ERD / data dictionary | **Data** | BA P3 (DDL), layer1 db_extractor (keep as raw feed) |
| PII / data quality / data flows | **Data** | — |
| Data-store transaction/consistency assessment | **Data** | **TA P9 OUTPUT 4 (relocate)** |
| Data-ownership map | **Data** | AA Stage 05 (relocate) |
| Technology-stack inventory | **Technology** | layer1 config_extractor (raw feed), AA framework detect |
| Infrastructure / deployment / CI-CD | **Technology** | — |
| NFRs / tech debt / patterns (system) | **Technology** | — |
| Application & data-level security posture | **Security (new) or Application** | **TA P9 OUTPUT 5 (relocate)** |
| Infra/transport security config | **Technology** | — |
| Cross-track reconciliation → knowledge graph | **New "Foundation/Synthesis" stage** | done outside pipeline today |
| Prompt governance rules | **Global (single source)** | inlined in BA/DA/TA prompts |

---

## Findings (consolidated)

- **F1 (High):** Three incompatible prompt paradigms; no shared template → Uniformity 46/100.
- **F2 (High):** Same extraction (data-store/entities ×5, business rules ×4) performed by independent
  layers with no reconciliation → divergence risk.
- **F3 (High):** Four confirmed cross-layer ownership violations (TA→Data, TA→Security/App, AA→Business,
  AA→Data).
- **F4 (Medium):** Governance/exclusion/confidence rules duplicated 2–7×; only AA centralizes.
- **F5 (Medium):** Orchestration boilerplate copied across 6–7 runners.
- **F6 (Medium):** No model pinning, no prompt version metadata → non-reproducible audits.
- **F7 (Low):** P3 is unimplemented documentation drift (~11k words dead weight).
- **F8 (Positive):** Anti-hallucination, evidence-citation, traceability, and AA's staged
  parse-first model are genuinely strong and should be the template.

## Risks

- **R1 — Inconsistent truth:** Four-way duplicate extraction can yield contradictory entity/rule sets
  per layer; without reconciliation the consuming forward-engineering package may inherit conflicts.
- **R2 — Maintenance fragility:** A rule change (e.g., new junk folder) must be edited in ≥5 places;
  drift is near-certain.
- **R3 — Audit non-reproducibility:** Unpinned model + unversioned prompts mean two runs may differ and
  cannot be diffed against a known prompt version.
- **R4 — Boundary erosion:** TA emitting data/security assessments and AA emitting business/data maps
  blurs TOGAF accountability; stakeholders cannot tell which layer is authoritative for a fact.
- **R5 — Token cost:** ~15–25% of instructional tokens are redundant governance restatement.

## Recommended Refactoring (summary)

1. Single global rules file cited by all prompts; delete inline copies.
2. One Scout/DeepAnalyst template + one staged-pipeline model (adopt AA's pattern everywhere).
3. One confidence standard.
4. `common/` module for CLI, markers, task-IO, context caps.
5. Relocate the four mis-owned outputs to their correct layers.
6. Add a first-class cross-track reconciliation/knowledge-graph stage; make non-owning layers consume,
   not re-extract.
7. Pin model + add prompt version headers.
8. Archive the aspirational P3 plan.

---

## Scoring Methodology (transparency)

- **Uniformity (46):** −30 for 3 paradigms; −15 for 3 confidence schemes; −9 for divergent output
  capture & context caps; +0 base 100. Within-track consistency is high, which keeps it from being
  lower.
- **Duplication severity (63, higher=worse):** exclusion list ×5, golden rules ×3, evidence hierarchy
  ×2, orchestration boilerplate ×6–7, anti-hallucination ×~20. Weighted by blast radius of edits.
- **Layer Separation (54):** 4 confirmed violations (−8 each) + 1 task duplicated across 5 layers (−14);
  Data layer cleanliness and AA stage discipline add back credit.
- **Architecture Quality (64):** strong craft (anti-hallucination, traceability, fault tolerance,
  parse-first in AA) offset by governance fragmentation, no model pinning, and doc drift.

*End of report. No files were modified — audit only.*
