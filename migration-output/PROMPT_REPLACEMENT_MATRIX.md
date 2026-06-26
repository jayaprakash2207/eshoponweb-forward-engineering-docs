# Prompt Replacement Matrix

**Date:** 2026-06-24
**Base (legacy):** `bussiness-architecture 1/bussiness-architecture/`
**Base (governed):** `prompt-refactored/`
**Risk legend:** 🟢 Low · 🟡 Medium · 🔴 High (see `REGRESSION_RISK_REPORT.md` for scoring).

---

## 1. Master matrix

| # | Original file (legacy) | Replacement (governed) | Loaded by runner | Functional difference | Output compatibility | Risk |
|---|---|---|---|---|---|---|
| 1 | `BA_Agent1_StructuralScout_v3.md` | `business-architecture/BA-SCOUT-01.md` | (BA scout — manual/legacy) | Entity-relationship extraction removed → consume-and-cite DA; governance/confidence externalized | Same 6 inventory tables, `===DOCUMENT_*===` markers | 🟡 |
| 2 | `BA_Agent2_DeepAnalyst_v3.md` | `business-architecture/BA-ANALYST-01.md` | (BA analyst — manual/legacy) | Data/app facts now consume DA/AA by citation; confidence→GOV-04 | Same 8 business docs | 🟡 |
| 3 | `BA_Pipeline_Execution_Plan.md` | **ARCHIVED** → `prompt-refactored/_archived/P3_*.POINTER.md` | none (never executed) | Superseded aspirational design | n/a (no output) | 🟢 |
| 4 | `layer2/layer2_prompt.md` | `business-architecture/BA-ANALYST-02.md` | `layer2/layer2_runner.py` | Sole owner of business rules; `business_entities[]` carry `da_entity_ref` instead of invented relationships | **`layer2_output.json` schema preserved** (entity field shape changed: relationships → da_entity_ref) | 🟡 |
| 5 | `layer3/layer3_prompt.md` | `business-architecture/BA-ANALYST-03.md` | `layer3/layer3_runner.py` | Data-model doc becomes DA-sourced view | **10 docs + `===DOCUMENT_START:<file>===` preserved** | 🟢 |
| 6 | `data-architecture/DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md` | `data-architecture/DA-SCOUT-01.md` + `data-architecture/DA-ANALYST-01.md` | `data-architecture/da_agent1_runner.py` | Split scout/analyst; numeric confidence→GOV-04 (band kept); **gains** data-ownership-map + datastore-transaction-consistency-assessment | **13 legacy DA filenames preserved + 2 received** | 🟡 |
| 7 | `data-architecture/DA_REVIEW_PROMPT.md` | `data-architecture/DA-REVIEW-01.md` | `data-architecture/da_agent2_runner.py` | Change records→GR-9 ref; Gate G1→PASS/PARTIAL/FAIL | **`review-summary.md` + change records preserved** | 🟢 |
| 8 | `technology-architecture/TA_STACKSCOUT_PROMPT.md` | `technology-architecture/TA-SCOUT-01.md` | `technology-architecture/ta_agent1_runner.py` | Sole owner tech-stack; governance externalized | **6 TA inventory filenames + `===TA_FILE_*===` preserved** | 🟢 |
| 9 | `technology-architecture/TA_DEEPANALYST_PROMPT.md` | `technology-architecture/TA-ANALYST-01.md` | `technology-architecture/ta_agent2_runner.py` | **OUTPUT 4 (data) → DA; OUTPUT 5 (app-sec) → AA;** keeps infra/transport security | 6 outputs kept; **2 outputs relocated (filenames move layer)** | 🔴 |
| 10 | `application-architecture/AGENTS.md` | `application-architecture/AGENTS.md` (governed) | `application-architecture/aa_runner.py` | Inline golden rules removed → CMP-GOV pointer; stage IDs assigned | Orchestration manifest (no artifact) | 🟢 |
| 11 | `application_architecture_extraction_agent_prompt.md` | `application-architecture/AA-ANALYST-00.md` | `aa_runner.py` (master ref) | Inline rules + junk list → CMP-GOV/GR-4; tech-stack→consume TA | AA package filenames preserved | 🟡 |
| 12 | `architecture-prompts/00-global-rules.md` | **DEMOTED** → `prompt-refactored/_archived/AA_00-global-rules.POINTER.md` + GOV-01 | `aa_runner.py` governance ref | Rules single-sourced to GOV-01 via CMP-GOV | n/a (governance only) | 🟡 |
| 13 | `architecture-prompts/01-inventory-agent.md` | `application-architecture/AA-SCOUT-01.md` | `aa_runner.py` (stage 01) | Global-rules ref → CMP-GOV; tiers→GOV-04 | **4 inventory JSONs preserved** | 🟢 |
| 14 | `architecture-prompts/02-parser-symbol-agent.md` | `application-architecture/AA-SCOUT-02.md` | `aa_runner.py` (stage 02) | → CMP-GOV; tiers→GOV-04 | **4 parsed JSONs preserved** | 🟢 |
| 15 | `architecture-prompts/03-evidence-pack-agent.md` | `application-architecture/AA-ANALYST-03.md` | `aa_runner.py` (stage 03) | → CMP-GOV | **9 evidence packs preserved** | 🟢 |
| 16 | `architecture-prompts/04-final-architecture-agent.md` | `application-architecture/AA-ANALYST-04.md` | `aa_runner.py` (stage 04) | → CMP-GOV | **AA final set preserved** | 🟢 |
| 17 | `architecture-prompts/05-enterprise-forward-engineering-agent.md` | `application-architecture/AA-ANALYST-05.md` | `aa_runner.py` (stage 05) | **capability-map → BA; data-ownership-map → DA** (consume-and-cite) | AA-owned FE inputs preserved; **2 outputs relocated (filenames move layer)** | 🔴 |
| 18 | *(new)* app/data-level security | `application-architecture/AA-ANALYST-06.md` | `aa_runner.py` (new stage 05b) | Receives app-level security from legacy TA OUTPUT 5 | New `application-security-assessment.md` (content previously in TA) | 🟡 |
| 19 | `architecture-prompts/06-quality-review-agent.md` | `application-architecture/AA-REVIEW-06.md` | `aa_runner.py` (stage 06) | Verdicts → GOV-04 §5 | **3 review docs preserved** | 🟢 |
| 20 | `architecture-prompts/07-workflow-audit-agent.md` | `application-architecture/AA-REVIEW-07.md` | `aa_runner.py` (stage 07) | + manifest/version checks | **audit docs preserved** | 🟢 |
| 21 | `tools/.../ARCHITECTURE_EXTRACTION_WORKFLOW.md` | (kept; safety → GR-5 ref) | doc only | Safety rules → GR-5 reference | Automation doc | 🟢 |
| 22 | *(new)* | `foundation/FN-SYNTH-01.md` | **new FN runner (deferred)** | Cross-track reconciliation (was external to pipeline) | **Additive** — `ENTERPRISE_KNOWLEDGE_GRAPH.json` (9 sections) | 🟡 |
| 23 | *(new)* | `foundation/FN-SYNTH-02.md` | new FN runner (deferred) | Canonical model + 3 views | Additive — canonical views | 🟢 |
| 24 | *(new)* | `foundation/FN-REVIEW-01.md` | new FN runner (deferred) | Reconciliation gate | Additive — `reconciliation-report.md` | 🟢 |

**Totals:** 20 legacy prompts replaced; 2 archived/demoted; 3 Foundation prompts added; 1 orchestrator + 1 doc updated.

---

## 2. Per-replacement detail cards

> Each card: original · replacement · functional differences · output compatibility status · risk level.

### R-01 · BA Scout
- **Original:** `BA_Agent1_StructuralScout_v3.md` · **Replacement:** `BA-SCOUT-01.md`
- **Functional diff:** removes entity-relationship/cardinality extraction (BA→DA boundary fix); records entity references as DA-consumed pointers; governance/confidence externalized to CMP-*.
- **Output compatibility:** ✅ Compatible — 6 inventory tables + DOCUMENT markers unchanged. Entity rows now carry DA references or `unknown` where DA not yet run.
- **Risk:** 🟡 (consume-and-cite changes a field's provenance, not the table shape).

### R-04 · Layer 2 (business analysis JSON)
- **Original:** `layer2/layer2_prompt.md` · **Replacement:** `BA-ANALYST-02.md`
- **Functional diff:** BA is now the single owner of business-rule extraction; `business_entities[]` emit `da_entity_ref` + `confidence` rather than locally-invented `relationship_type/target_entity/cardinality`.
- **Output compatibility:** ⚠️ Schema-compatible at the top level (`layer2_output.json` keys unchanged) but the **`business_entities[]` element shape changed**. `layer3_runner.py` splitter is unaffected (it consumes the JSON, not entity internals). Downstream that read entity `relationships` must read `da_entity_ref`. See COMPATIBILITY_REPORT C-04.
- **Risk:** 🟡.

### R-06 · DA reverse-engineering (split)
- **Original:** `DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md` · **Replacement:** `DA-SCOUT-01.md` + `DA-ANALYST-01.md`
- **Functional diff:** one prompt split into Scout (inventory) + Analyst (deliverables); numeric confidence mapped to GOV-04 labels (numeric band retained for tooling); **receives** `data-ownership-map.md` (from AA Stage 05) and `datastore-transaction-consistency-assessment.md` (from TA OUTPUT 4).
- **Output compatibility:** ✅ 13 legacy DA filenames preserved; +2 received files. `da_agent1_runner.py` retry logic keyed on the 13-file set still satisfied; the +2 are additive within `da-outputs/`.
- **Risk:** 🟡 (a single runner now drives two prompt phases — orchestration note in CHANGED_FILES).

### R-09 · TA deep analyst (OUTPUT 4 & 5 relocated)
- **Original:** `TA_DEEPANALYST_PROMPT.md` · **Replacement:** `TA-ANALYST-01.md`
- **Functional diff:** stops producing `data-architecture-assessment.md` (→ DA) and app-level `security-architecture-assessment.md` (→ AA); keeps `infra-transport-security-assessment.md`.
- **Output compatibility:** 🔴 **Behavioral relocation** — two files previously emitted under `ta-outputs/` now appear under `da-outputs/` and AA outputs respectively. Any consumer globbing `ta-outputs/*security*` or `ta-outputs/data-architecture-assessment.md` must be repointed. Net artifact set across the pipeline is preserved; **location changes**. See COMPATIBILITY_REPORT C-09.
- **Risk:** 🔴.

### R-17 · AA enterprise forward-engineering (Stage 05 relocations)
- **Original:** `05-enterprise-forward-engineering-agent.md` · **Replacement:** `AA-ANALYST-05.md`
- **Functional diff:** stops authoring `business-capability-map.{json,md}` (→ BA) and `data-ownership-map.md` (→ DA); consumes both by citation.
- **Output compatibility:** 🔴 **Behavioral relocation** — `business-capability-map.*` and `data-ownership-map.md` are now produced by BA / DA, not AA Stage 05. The eShopOnWeb sample output currently has these under `aa-outputs/final/`; post-cutover they originate upstream and AA references them. See COMPATIBILITY_REPORT C-17.
- **Risk:** 🔴.

### R-18 · AA application security (new)
- **Original:** *(portion of TA OUTPUT 5)* · **Replacement:** `AA-ANALYST-06.md`
- **Functional diff:** new AA stage owning application/data-level security (authZ completeness, CORS, app secrets usage); infra/transport security stays in TA.
- **Output compatibility:** 🟡 New file `application-security-assessment.md`; the application-layer content formerly inside TA's `security-architecture-assessment.md` now lives here. Additive within AA outputs.
- **Risk:** 🟡.

### R-22..24 · Foundation (new layer)
- **Original:** *(none — reconciliation was external)* · **Replacement:** `FN-SYNTH-01/02`, `FN-REVIEW-01`
- **Functional diff:** moves cross-track reconciliation + knowledge-graph generation into the pipeline.
- **Output compatibility:** ✅ Additive — `ENTERPRISE_KNOWLEDGE_GRAPH.json` (9 sections) + canonical views + reconciliation-report. No existing artifact changes.
- **Risk:** 🟡 (needs a new runner — deferred; see CHANGED_FILES "deferred").

*(Cards for the 🟢 1:1 swaps R-05, R-07, R-08, R-13..16, R-19, R-20 omitted for brevity — functional diff is governance/confidence externalization only; output filenames and markers unchanged.)*
