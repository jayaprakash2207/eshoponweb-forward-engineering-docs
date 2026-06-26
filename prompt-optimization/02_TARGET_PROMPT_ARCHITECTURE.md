# 02 — Target Prompt Architecture

**Date:** 2026-06-24 · **Rule:** optimize, don't redesign. Same layers, same owners, fewer prompts.

---

## 1. Current architecture (21 executable prompts + 5 components)

```
BUSINESS (4 — two parallel lineages, one UNWIRED)
  ┌─ BA-SCOUT-01      (was BA_Agent1)  ✗ not wired to any runner ──┐  overlap
  ├─ BA-ANALYST-01    (was BA_Agent2)  ✗ not wired to any runner ──┘
  ├─ BA-ANALYST-02    (layer2_runner)  ✓ wired
  └─ BA-ANALYST-03    (layer3_runner)  ✓ wired

DATA (3 — extractor artificially split)
  ┌─ DA-SCOUT-01     ┐ both driven by ONE runner (da_agent1_runner)  ← split forces 2-phase load
  ├─ DA-ANALYST-01   ┘
  └─ DA-REVIEW-01      (da_agent2_runner)

APPLICATION (9 stages + orchestrator + master spec)
  AA-SCOUT-01 → AA-SCOUT-02 → AA-ANALYST-03 → AA-ANALYST-04 → AA-ANALYST-05 / AA-ANALYST-06
  → AA-REVIEW-06 → AA-REVIEW-07     (AGENTS.md orchestrates; AA-ANALYST-00 = spec)

TECHNOLOGY (2 — both wired)
  TA-SCOUT-01 (ta_agent1_runner) → TA-ANALYST-01 (ta_agent2_runner)

FOUNDATION (3)
  FN-SYNTH-01 → FN-SYNTH-02 → FN-REVIEW-01

COMPONENTS (5): CMP-GOV, CMP-CONF, CMP-VALID, CMP-EVID, CMP-OUT
```

## 2. Target architecture (18 executable prompts + 5 components)

```
BUSINESS (2)   ── eliminated the unwired parallel lineage
  BA-ANALYST-02  ✓  (now begins with an internal "structural scout" Phase-A; emits layer2_output.json)
        ↓
  BA-ANALYST-03  ✓  (renders 10 BA docs)

DATA (2)       ── re-unified the split extractor
  DA-EXTRACT-01  ✓  (= DA-SCOUT-01 + DA-ANALYST-01; one prompt, one runner, 13+2 file contract)
        ↓
  DA-REVIEW-01   ✓  (review/gate — unchanged)

APPLICATION (9 + orch + spec)  ── UNCHANGED (distinct parse-first stages)
  AA-SCOUT-01 → AA-SCOUT-02 → AA-ANALYST-03 → AA-ANALYST-04 → AA-ANALYST-05 / AA-ANALYST-06
  → AA-REVIEW-06 → AA-REVIEW-07     (AGENTS.md; AA-ANALYST-00 spec)

TECHNOLOGY (2)  ── UNCHANGED (both wired, real split)
  TA-SCOUT-01 → TA-ANALYST-01

FOUNDATION (3)  ── UNCHANGED
  FN-SYNTH-01 → FN-SYNTH-02 → FN-REVIEW-01

COMPONENTS (5): unchanged
```

## 3. Before → after delta

```
                 CURRENT        TARGET      Δ
  Business         4      →        2       −2   (eliminate unwired lineage)
  Data             3      →        2       −1   (re-merge split extractor)
  Application      9      →        9        0   (no safe merge)
  Technology       2      →        2        0
  Foundation       3      →        3        0
  ─────────────────────────────────────────────
  Executable      21      →       18       −3   (−14%)
  Components        5      →        5        0
  Unwired legacy    2      →        0       −2   (retired)
```

## 4. Layer-boundary preservation (visual)

```
   BA ───► DA ───► AA ───► TA ───► FN ───► (Forward Engineering, out of scope)
   │       │       │       │       │
   2       2       9       2       3      ← prompt counts; every arrow = consume-and-cite (GOV-07 C-1..C-6)
   owners unchanged: BA, DA, AA, TA, FN — no prompt crossed a layer in optimization.
```

## 5. What changed vs what is invariant

| Changed | Invariant |
|---|---|
| BA: 4→2 (drop unwired lineage; fold scout discipline into BA-ANALYST-02) | All BA outputs (layer2_output.json, 10 docs) |
| DA: 3→2 (DA-SCOUT-01 + DA-ANALYST-01 → DA-EXTRACT-01) | All 13+2 DA files, da-outputs/ contract, DA-REVIEW separate |
| Prompt count 21→18 | Layers (5), owners (5), markers, schemas, knowledge graph (274 nodes), governance, traceability |
