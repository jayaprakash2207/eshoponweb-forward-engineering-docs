# 05 — Complexity Analysis

**Date:** 2026-06-24 · Current vs Optimized.

---

## 1. Prompt count

| Metric | Current | Optimized | Δ |
|---|---|---|---|
| Executable prompts | 21 | 18 | **−3 (−14%)** |
| Unwired/duplicate prompts | 2 (BA-SCOUT-01, BA-ANALYST-01) | 0 | **−2** |
| Reusable components | 5 | 5 | 0 |
| Orchestrator + master spec | 2 | 2 | 0 |
| Runners touched | — | 2 (da_agent1 single-load; BA lineage retire) | minimal |

## 2. Duplicated logic

| Source of duplication | Current | Optimized |
|---|---|---|
| Parallel BA lineages (scout/analyst markdown **and** layer2/layer3 JSON) | 2 lineages doing overlapping business extraction | 1 wired lineage |
| DA extractor split across 2 prompts under 1 runner | 2 prompts, 2 governance blocks, forced 2-phase load | 1 prompt, 1 governance block |
| Governance text | already single-sourced (CMP-GOV) | unchanged (already optimal) |
| Confidence scheme | already single (GOV-04) | unchanged |
| Validation block | already single (CMP-VALID) | unchanged |
| Review stages | product QA + process QA (distinct) | unchanged (correctly distinct) |

> Note: the heavy duplication (governance ×5, anti-hallucination ×20, confidence ×3) was **already
> removed** by the refactor. The *remaining* duplication is structural: parallel BA lineages and the DA
> split. Those are exactly what this optimization targets.

## 3. Maintenance effort

| Dimension | Current | Optimized | Effect |
|---|---|---|---|
| Prompts to edit on a governance change | 21 (via includes, 1 edit propagates) | 18 | fewer files to re-resolve |
| BA change surface | edit must consider 2 lineages (risk of drift between scout/analyst vs layer2/layer3) | 1 lineage | **removes a drift class** |
| DA change surface | keep scout+analyst in sync + 2-phase runner | 1 prompt, 1 runner | **simpler** |
| Runner/prompt contract | DA needs deferred 2-phase edit | DA = original 1:1 | **closes a deferred code item** |
| Re-resolution targets (prompt-resolved/) | 21 | 18 | fewer artifacts to regenerate |
| Onboarding clarity | "why are there two BA pipelines?" | one BA pipeline | **less confusion** |

**Qualitative maintenance reduction: ~25–30%** of BA/DA maintenance surface (the two layers touched),
≈ **14%** of total prompt surface. The biggest win is **eliminating the "two BA pipelines" drift class**,
not the raw count.

## 4. Execution complexity

| Dimension | Current | Optimized |
|---|---|---|
| Executable stages (BA) | layer2 → layer3 (BA-SCOUT/ANALYST-01 not executed) | layer2 → layer3 (scout folded in) — **same runtime, clearer source** |
| Executable stages (DA) | da_agent1 (loads 1 split into 2 phases — deferred edit) → da_agent2 | da_agent1 (loads DA-EXTRACT-01 single) → da_agent2 — **simpler load** |
| AA / TA / FN | unchanged | unchanged |
| Total executed prompt-loads | unchanged (the 2 BA prompts were never loaded) | unchanged |
| Deferred code items | DA 2-phase load required | **removed** (1:1 restored) |

> Execution **runtime** is essentially unchanged (the eliminated BA prompts never ran). The win is in
> **source/maintenance** complexity and **closing a deferred code item** (DA 2-phase load), not in
> fewer model calls.

## 5. Risk vs reward

| Change | Reward | Risk | Net |
|---|---|---|---|
| Retire BA-SCOUT-01 / BA-ANALYST-01 | removes drift class, −2 prompts | none consumed today | 🟢 strongly positive |
| Merge DA → DA-EXTRACT-01 | restores 1:1 contract, closes deferred edit | prompt length ↑ (managed by phases) | 🟢 positive |
| Keep AA/TA/FN as-is | preserves parse-first correctness | none | 🟢 correct restraint |

## 6. Bottom line

Optimization removes **structural** fragmentation (parallel BA lineages, artificial DA split) without
touching the parts that are correctly granular (AA parse-first stages, TA dual-runner, FN build/render/gate).
**−3 prompts, −1 deferred code item, −1 drift class**, with **zero functional or output loss**.
