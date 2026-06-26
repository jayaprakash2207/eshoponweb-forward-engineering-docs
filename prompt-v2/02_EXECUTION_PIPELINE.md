# 02 — Execution Pipeline (prompt-v2)

**Date:** 2026-06-24 · 10 prompts, 2 per layer. Layer order and consume-direction unchanged (GOV-07).

---

## 1. End-to-end flow

```
LAYER 1 (deterministic Python extraction — not a prompt)
   source_code.json · database.json · config.json · logs.json
        │
        ├─► BUSINESS      BA-EXTRACT ──► BA-VALIDATE
        │                  (PhaseA scout → PhaseB layer2_output.json → PhaseC 10 docs)   (gate)
        │
        ├─► DATA          DA-EXTRACT ──► DA-VALIDATE
        │                  (PhaseA discovery → PhaseB analysis; 13+2 files)              (gate)
        │
        ├─► APPLICATION   AA-EXTRACT ──► AA-VALIDATE
        │                  (P1 inventory→P2 parsed→P3 packs→P4 final→P5 fwd→P6 security)  (§A product + §B process QA)
        │
        └─► TECHNOLOGY    TA-EXTRACT ──► TA-VALIDATE
                           (PhaseA inventory → PhaseB analysis; 6+7 files)               (gate)
                                          │
   (all four layers' validated owner artifacts) ─┘
                                          ▼
        FOUNDATION        FN-SYNTHESIZE ──► FN-VALIDATE
                           (Phase1 reconcile→graph → Phase2 views)   (graph+traceability+integrity gate)
                                          ▼
                          [ Forward Engineering — out of scope ]
```

## 2. The 10 prompts in order

```
BUSINESS     1. BA-EXTRACT      2. BA-VALIDATE
DATA         3. DA-EXTRACT      4. DA-VALIDATE
APPLICATION  5. AA-EXTRACT      6. AA-VALIDATE
TECHNOLOGY   7. TA-EXTRACT      8. TA-VALIDATE
FOUNDATION   9. FN-SYNTHESIZE  10. FN-VALIDATE
SHARED       GOV · CONFIDENCE · VALIDATION (referenced by all 10)
```

## 3. Within-prompt phase chains (parse-first preserved)

| Prompt | Internal phases (each emits its artifact before the next) |
|---|---|
| BA-EXTRACT | A scout → B `layer2_output.json` → C 10 docs |
| DA-EXTRACT | A discovery (schema/PII/source) → B analysis (model/ERD/ownership/consistency) |
| AA-EXTRACT | P1 inventory → P2 parsed → P3 evidence-packs → P4 final → P5 forward-eng → P6 app-security |
| TA-EXTRACT | A inventory (6) → B assessment (7) |
| FN-SYNTHESIZE | 1 reconcile→graph (9 sections) → 2 project views (4) |

## 4. Dependencies (GOV-07, unchanged)

- BA/DA/AA/TA EXTRACT fan out from Layer 1; each VALIDATE gates its own layer.
- Cross-layer reads = consume-and-cite (C-1…C-4): AA-EXTRACT cites BA capabilities + DA ownership; TA-EXTRACT cites AA components + DA data-stores.
- FN-SYNTHESIZE consumes all four validated owner sets (C-5); FN-VALIDATE gates Forward Engineering (C-6).
- DAG still acyclic, terminates at Foundation. No extraction layer consumes FN output.

## 5. Execution-order invariants

| Invariant | Status |
|---|---|
| Layer order BA→DA→AA→TA→FN (consume direction) | unchanged |
| Parse-first within each EXTRACT | preserved as internal phases |
| Validate after Extract within each layer | enforced (2-prompt model) |
| Foundation last | unchanged |
