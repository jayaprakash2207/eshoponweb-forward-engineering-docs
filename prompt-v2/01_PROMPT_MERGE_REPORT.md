# 01 — Prompt Merge Report (prompt-v2)

**Date:** 2026-06-24
**From:** 18-prompt optimized set · **To:** 10-prompt two-prompt-per-layer model.
**Authority:** `../prompt-governance/`, `../prompt-optimization/`, `../FINAL_PROMPT_CONFORMANCE_REPORT.md`.

---

## 0. Design principle (read first)

The two-prompt-per-layer target is delivered, but with a non-negotiable engineering invariant:

> **A merge is at the AUTHORING/maintenance surface — never a collapse of parse-first execution.**
> Each `*-EXTRACT` prompt is one governed document that **internally mandates the existing phase chain**
> and **emits every intermediate artifact** (inventory, parsed facts, evidence packs, layer JSON,
> knowledge graph). Skipping a phase (raw→final in one model pass) violates GR-6.1 and is a hard FAIL.

This is the only way to reduce prompt count while honoring the refactoring rule "outputs remain identical
/ knowledge-graph quality unchanged / do not violate governance." Where even a phased merge would reduce
quality, the merge was **declined** (see §6).

## 1. Merge map

| Layer | Old prompts | New prompt | Type |
|---|---|---|---|
| Business | BA-ANALYST-02, BA-ANALYST-03 (+ unwired BA-SCOUT-01/ANALYST-01) | **BA-EXTRACT** | phased merge (scout→analyze→produce) |
| Business | *(none — new validate slot)* | **BA-VALIDATE** | additive review/gate |
| Data | DA-SCOUT-01 + DA-ANALYST-01 | **DA-EXTRACT** | phased merge (restores original 1-prompt) |
| Data | DA-REVIEW-01 | **DA-VALIDATE** | 1:1 rename |
| Application | AA-SCOUT-01, AA-SCOUT-02, AA-ANALYST-03/04/05/06 (+ AA-ANALYST-00 spec) | **AA-EXTRACT** | 6-phase merge, spec embedded |
| Application | AA-REVIEW-06 + AA-REVIEW-07 | **AA-VALIDATE** | merge (2 labeled sections) |
| Technology | TA-SCOUT-01 + TA-ANALYST-01 | **TA-EXTRACT** | phased merge (2 runners→2 phases) |
| Technology | *(formalizes ta-review-summary)* | **TA-VALIDATE** | additive review/gate |
| Foundation | FN-SYNTH-01 + FN-SYNTH-02 | **FN-SYNTHESIZE** | phased merge (graph→views) |
| Foundation | FN-REVIEW-01 | **FN-VALIDATE** | 1:1 rename |

## 2. Why each merge is safe (per refactoring rules)

| New prompt | Outputs identical? | Ownership same? | Layer same? | Governance same? | KG/traceability same? | Safe? |
|---|---|---|---|---|---|---|
| BA-EXTRACT | ✅ layer2_output.json + 10 docs | ✅ BA | ✅ | ✅ | ✅ | ✅ |
| BA-VALIDATE | ✅ additive (review summary) | ✅ BA | ✅ | ✅ | ✅ | ✅ |
| DA-EXTRACT | ✅ 13+2 files | ✅ DA | ✅ | ✅ | ✅ | ✅ |
| DA-VALIDATE | ✅ review-summary.md | ✅ DA | ✅ | ✅ | ✅ | ✅ |
| AA-EXTRACT | ✅ inventory+parsed+packs+final+fwd+security | ✅ AA | ✅ | ✅ (parse-first preserved internally) | ✅ | ✅ |
| AA-VALIDATE | ✅ 5 review files | ✅ AA | ✅ | ✅ | ✅ | ✅ |
| TA-EXTRACT | ✅ 6 inventory + 7 assessments | ✅ TA | ✅ | ✅ | ✅ | ✅ |
| TA-VALIDATE | ✅ ta-review-summary.md | ✅ TA | ✅ | ✅ | ✅ | ✅ |
| FN-SYNTHESIZE | ✅ graph + 4 views | ✅ FN | ✅ | ✅ (graph-before-views gate) | ✅ 274 nodes | ✅ |
| FN-VALIDATE | ✅ reconciliation-report | ✅ FN | ✅ | ✅ | ✅ | ✅ |

## 3. Responsibilities retained

Every responsibility from the 18-prompt set maps into a v2 prompt with the **same owner**:

- BA rules/capabilities/processes/docs → BA-EXTRACT (+ BA-VALIDATE gate).
- DA schema/entities/PII/ownership/transaction-consistency → DA-EXTRACT (+ DA-VALIDATE gate).
- AA inventory→parsed→evidence→final→forward-eng→security → AA-EXTRACT phases (+ AA-VALIDATE product+process QA).
- TA stack/infra/CI-CD/NFR/debt/infra-security → TA-EXTRACT phases (+ TA-VALIDATE gate).
- FN reconciliation/graph/canonical/views → FN-SYNTHESIZE (+ FN-VALIDATE gate).

## 4. Responsibilities removed

**None.** No responsibility was dropped. Two things were *eliminated* in the prior optimization step
(unwired BA-SCOUT-01/BA-ANALYST-01) and are not reintroduced; their scout discipline lives as BA-EXTRACT
Phase A. The AA-ANALYST-00 master spec is not removed — it is **embedded** as the AA-EXTRACT contract.

## 5. Compatibility impact

| Dimension | Impact |
|---|---|
| Output filenames/markers | None — all preserved (DOCUMENT/JSON/DA_FILE/AA_FILE/TA_FILE/FN_FILE). |
| Intermediate artifacts | Preserved — EXTRACT prompts emit each phase's artifacts (GR-6.1). |
| Knowledge graph | None — FN inputs unchanged → 274 nodes / 9 sections / 117 links reproducible. |
| Governance | Improved — shared components 5→3 (evidence+output folded into GOV); 1 block per prompt. |
| Ownership/boundaries | None — every merge within one layer/owner. |
| Runner wiring | Changes (code, deferred): multi-stage runners now drive a single phased prompt; see migration guide. |

## 6. Merges DECLINED (correctness > count)

| Considered | Decision | Reason |
|---|---|---|
| Merge EXTRACT + VALIDATE within a layer (1 prompt/layer) | ❌ declined | Would put extraction and its own review in one pass — a reviewer cannot independently gate its own output; violates the validate/gate separation and GR-9 change-record integrity. **Two prompts per layer is the floor.** |
| Merge across layers (e.g. BA+DA) | ❌ declined | Forbidden — different owners/layers (refactoring rule). |
| Collapse AA phases into a true single pass | ❌ declined | Violates GR-6.1 parse-first; would drop intermediate evidence packs the graph depends on. AA-EXTRACT keeps phases internal. |
| Drop TA-VALIDATE / BA-VALIDATE (no legacy equivalent) | ❌ declined | The model standardizes two prompts per layer; the validate gate adds independent QA without changing outputs. Kept as additive. |

**Result: 18 → 10 prompts (−44%), shared components 5 → 3, with zero output/KG/governance/ownership loss.**
