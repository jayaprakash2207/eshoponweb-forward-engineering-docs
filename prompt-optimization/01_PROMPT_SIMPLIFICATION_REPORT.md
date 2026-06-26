# 01 — Prompt Simplification Report

**Package:** `prompt-optimization/`
**Date:** 2026-06-24
**Authority:** `../prompt-governance/` (GOV-01…GOV-10), `../prompt-refactored/`, `../prompt-resolved/`, `../FINAL_PROMPT_CONFORMANCE_REPORT.md`, `../pilot-cutover-validation/`
**Constraint:** Optimize, do not redesign. Correctness > prompt count. No code, no FE artifacts.

---

## 1. Scope of optimization

The architecture already passed audit → governance → refactor → conformance → cutover. We therefore look
**only** for *safe* reductions — fragmentation that adds maintenance cost without adding capability —
under the MERGE RULES (same layer, same ownership, identical outputs, compatible consumers, preserved
traceability/governance).

**Decisive runtime fact (verified):** the executable BA pipeline is `layer2_runner.py` → `layer3_runner.py`
only. `BA_Agent1_StructuralScout` / `BA_Agent2_DeepAnalyst` (→ governed `BA-SCOUT-01` / `BA-ANALYST-01`)
are **not referenced by any runner** (`grep` of all `*.py` = 0 hits). They are a **parallel, unwired BA
lineage** that overlaps the layer2/layer3 lineage. This is the single largest, safest simplification.

## 2. Per-prompt disposition

Legend: **KEEP** · **MERGE** · **ELIMINATE** (fold/retire) · **COMPONENT** (already a reusable fragment).

### Business Architecture

| Prompt | Purpose | Wired? | Disposition | Why |
|---|---|---|---|---|
| `BA-SCOUT-01` (was BA_Agent1) | Structural inventory (markdown tables) | ❌ not in any runner | **ELIMINATE → fold into BA-ANALYST-02 as a Phase-A "structural scout" pass** | Duplicate of the layer2 extraction lineage; unwired; same owner (BA); folding preserves the scout discipline as a phase, removing a parallel artifact set. |
| `BA-ANALYST-01` (was BA_Agent2) | Deep business docs (8 markdown) | ❌ not in any runner | **ELIMINATE → its 8 doc concerns already covered by BA-ANALYST-03's 10 docs** | The layer3 lineage already produces capability map, processes, rules, stakeholders, value streams. Keeping a second BA doc-generator is pure duplication. |
| `BA-ANALYST-02` (layer2) | Business analysis → `layer2_output.json` | ✅ layer2_runner | **KEEP (absorbs Scout phase)** | The wired BA extractor; sole owner of business rules. |
| `BA-ANALYST-03` (layer3) | Render JSON → 10 BA docs | ✅ layer3_runner | **KEEP** | The wired BA document generator. |

> **BA net: 4 → 2.** Eliminate the two unwired, overlapping lineage prompts; their *capabilities* (scout
> discipline, business docs) are retained inside the wired pair. **Zero functional loss** because the
> eliminated prompts produce nothing the pipeline consumes today.

### Data Architecture

| Prompt | Purpose | Disposition | Why |
|---|---|---|---|
| `DA-SCOUT-01` | Schema/source inventory | **MERGE → DA-EXTRACT-01** | Legacy was ONE prompt (`DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md`) driven by ONE runner (`da_agent1_runner.py`). The refactor split it into Scout+Analyst; the split forced a deferred two-phase runner edit. Re-merging restores the original 1-prompt/1-runner contract. |
| `DA-ANALYST-01` | Data deliverables (incl. relocated ownership + transaction/consistency) | **MERGE → DA-EXTRACT-01** | Same owner (DA), same runner, sequential phases, single `da-outputs/` 13+2 file contract. Merge is the *original* design. |
| `DA-REVIEW-01` | Review/gate (`da_agent2_runner.py`) | **KEEP** | Distinct runner, distinct Review role, change records + Gate verdict. Different execution stage — do not merge. |

> **DA net: 3 → 2.** Re-unify the artificially-split extractor; keep the review gate separate.

### Application Architecture

| Prompt | Purpose | Disposition | Why |
|---|---|---|---|
| `AA-ANALYST-00` (master spec) | Authoritative AA contract | **KEEP (as spec, not a run stage)** | It is the contract the staged prompts implement; not a duplicate run stage. Mark explicitly "reference spec, not executed". |
| `AA-SCOUT-01` (inventory) | Stage 1 | **KEEP** | Distinct parse-first stage; deterministic contract. |
| `AA-SCOUT-02` (parser/symbol) | Stage 2 | **KEEP** | Distinct stage; consumed by evidence packs. |
| `AA-ANALYST-03` (evidence packs) | Stage 3 | **KEEP** | Intermediate representation; parse-first discipline (GR-6.1) forbids collapsing into final. |
| `AA-ANALYST-04` (final architecture) | Stage 4 | **KEEP** | The synthesis stage. |
| `AA-ANALYST-05` (forward-eng inputs) | Stage 5 | **KEEP** | Distinct outputs; consumes BA/DA. |
| `AA-ANALYST-06` (app security) | Stage 5b | **KEEP** | Owns relocated app/data security; distinct concern. |
| `AA-REVIEW-06` (quality review) | Stage 6 | **KEEP** | Reviews artifacts. |
| `AA-REVIEW-07` (workflow audit) | Stage 7 | **MERGE-CANDIDATE → fold into AA-REVIEW-06? NO → KEEP** | *Evaluated and rejected:* 06 reviews **artifact** quality; 07 audits the **workflow/process** (manifest, hardcoding, parser extensibility). Different inputs, different verdict scope. Merging would conflate product vs process QA. **Keep separate.** |
| `AGENTS.md` (orchestrator) | Stage wiring | **KEEP** | Orchestration manifest, not a prompt stage. |

> **AA net: unchanged (9 stages + orchestrator + spec).** The AA staged pipeline is the *reference* model
> the audit praised; its stages are genuinely distinct concerns (parse-first → evidence → final → review).
> Collapsing them would re-introduce the "raw→final" anti-pattern GR-6.1 forbids. **No safe merge.**

### Technology Architecture

| Prompt | Purpose | Disposition | Why |
|---|---|---|---|
| `TA-SCOUT-01` | Stack/infra inventory (`ta_agent1_runner.py`) | **KEEP** | Distinct runner + Scout discipline; feeds TA-ANALYST. |
| `TA-ANALYST-01` | Patterns/NFR/debt/security (`ta_agent2_runner.py`) | **KEEP** | Distinct runner + deep analysis. Two-runner Scout→Analyst is the *wired* TA design (unlike BA, both TA prompts ARE wired). |

> **TA net: unchanged (2).** Unlike BA, both TA prompts are wired to separate runners; the split is real
> and load-bearing. Do not merge.

### Foundation

| Prompt | Purpose | Disposition | Why |
|---|---|---|---|
| `FN-SYNTH-01` | Knowledge graph synthesis | **KEEP** | Owns the 274-node graph; reconciliation algorithm. |
| `FN-SYNTH-02` | Canonical model + 3 views | **KEEP** | Distinct: produces human views from the graph. |
| `FN-REVIEW-01` | Reconciliation gate | **KEEP** | Distinct Review role + gate verdict. |

> **FN net: unchanged (3).** Evaluated merging SYNTH-01+02: rejected — graph generation vs view projection
> are different concerns (one builds the canonical JSON, the other renders read-only markdown); FN-1/FN-7
> keep them cleanly separable and the graph must validate (FN-REVIEW-01) *between* build and views in a
> robust flow. Keep separate.

### Reusable components (already optimal)

| Component | Disposition |
|---|---|
| `CMP-GOV, CMP-CONF, CMP-VALID, CMP-EVID, CMP-OUT` | **KEEP (COMPONENT)** | Already the de-duplication mechanism; nothing to merge. |

## 3. Disposition roll-up

| Layer | Current governed prompts | Disposition | Optimized |
|---|---|---|---|
| Business | 4 (BA-SCOUT-01, BA-ANALYST-01/02/03) | eliminate 2 unwired; keep 2 wired | **2** |
| Data | 3 (DA-SCOUT-01, DA-ANALYST-01, DA-REVIEW-01) | merge 2 → 1; keep review | **2** |
| Application | 9 stages (+orch +spec) | keep all | **9** (+orch +spec) |
| Technology | 2 | keep both | **2** |
| Foundation | 3 | keep all | **3** |
| Components | 5 | keep all | **5** |
| **Prompt total (exec stages)** | **21** | | **18** |

**Net: 21 → 18 executable prompts (−3 / −14%).** Plus retiring 2 unwired legacy lineage prompts entirely.
No layer crossed, no ownership changed, no output dropped (see `03`, `08`).

## 4. What was deliberately NOT merged (and why)

- **AA stages** — distinct parse-first concerns; merging violates GR-6.1.
- **AA-REVIEW-06 vs 07** — product QA vs process QA; different inputs/verdict.
- **TA Scout vs Analyst** — both wired to separate runners; real split.
- **FN-SYNTH-01 vs 02** — graph build vs view render; FN-REVIEW gates between.
- **Review prompts across layers** — different owners; MERGE RULES forbid cross-owner merge.

These were evaluated and rejected to honor "correctness > count."
