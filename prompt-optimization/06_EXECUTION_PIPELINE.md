# 06 — Execution Pipeline (Optimized)

**Date:** 2026-06-24 · Every prompt shown. Order = canonical consume direction (GOV-07); BA/DA/AA/TA fan out from Layer 1, converge at Foundation.

---

## 1. End-to-end flow

```
LAYER 1 (deterministic Python extraction — not a prompt)
   source_code.json · database.json · config.json · logs.json
        │
        ├──────────────► BUSINESS ARCHITECTURE
        │                  BA-ANALYST-02   (Phase A: structural scout → Phase B: rules/entities/capabilities)
        │                       │  layer2_output.json
        │                       ▼
        │                  BA-ANALYST-03   → 10 BA documents
        │
        ├──────────────► DATA ARCHITECTURE
        │                  DA-EXTRACT-01    (schema/PII/flows/ownership/transaction-consistency; 13+2 files)
        │                       │
        │                       ▼
        │                  DA-REVIEW-01     (validate/enrich + Gate verdict)
        │
        ├──────────────► APPLICATION ARCHITECTURE   (AGENTS.md orchestrates; AA-ANALYST-00 = spec)
        │                  AA-SCOUT-01 → AA-SCOUT-02 → AA-ANALYST-03 → AA-ANALYST-04
        │                       ├──► AA-ANALYST-05  (forward-eng inputs; consumes BA caps + DA ownership)
        │                       ├──► AA-ANALYST-06  (app/data-level security)
        │                       ├──► AA-REVIEW-06   (artifact quality)
        │                       └──► AA-REVIEW-07   (workflow audit)
        │
        └──────────────► TECHNOLOGY ARCHITECTURE
                           TA-SCOUT-01 → TA-ANALYST-01
                                          │
   (all four tracks' owner artifacts) ────┘
                                          ▼
                        FOUNDATION / SYNTHESIS
                           FN-SYNTH-01  (reconcile → ENTERPRISE_KNOWLEDGE_GRAPH.json, 9 sections / 274 nodes)
                                ▼
                           FN-SYNTH-02  (canonical model + inventory + traceability + FE input map)
                                ▼
                           FN-REVIEW-01 (reconciliation gate → PASS/PARTIAL/FAIL)
                                ▼
                        [ Forward Engineering — out of scope ]
```

## 2. Layer-ordered prompt list (18 executable)

```
BUSINESS (2)
  1. BA-ANALYST-02     ✓ wired (layer2_runner)   [absorbs former BA-SCOUT-01 as Phase A]
  2. BA-ANALYST-03     ✓ wired (layer3_runner)

DATA (2)
  3. DA-EXTRACT-01     ✓ wired (da_agent1_runner) [= former DA-SCOUT-01 + DA-ANALYST-01]
  4. DA-REVIEW-01      ✓ wired (da_agent2_runner)

APPLICATION (9)        ✓ wired (aa_runner; AGENTS.md orchestrates, AA-ANALYST-00 spec)
  5. AA-SCOUT-01
  6. AA-SCOUT-02
  7. AA-ANALYST-03
  8. AA-ANALYST-04
  9. AA-ANALYST-05
 10. AA-ANALYST-06
 11. AA-REVIEW-06
 12. AA-REVIEW-07

TECHNOLOGY (2)
 13. TA-SCOUT-01       ✓ wired (ta_agent1_runner)
 14. TA-ANALYST-01     ✓ wired (ta_agent2_runner)

FOUNDATION (3)
 15. FN-SYNTH-01       (new FN runner — deferred wiring)
 16. FN-SYNTH-02
 17. FN-REVIEW-01

(+ AGENTS.md orchestrator, AA-ANALYST-00 master spec, 5 CMP-* components)
```

## 3. Execution order & dependency (unchanged from GOV-07)

| Stage group | Reads | Writes | Gate |
|---|---|---|---|
| BA-ANALYST-02 → 03 | Layer 1 JSON | layer2_output.json → 10 docs | — |
| DA-EXTRACT-01 → DA-REVIEW-01 | Layer 1 + live DB | 13+2 files → review-summary + verdict | Gate G1 (DA) |
| AA stages 01→07 | repo / prior AA stage | inventory→parsed→packs→final→fwd/sec→reviews | quality + workflow verdicts |
| TA-SCOUT → TA-ANALYST | repo | 6 inventory → 7 assessments | — |
| FN-SYNTH-01 → 02 → REVIEW-01 | all owner artifacts | graph → views → reconciliation gate | FE gate |

## 4. What is identical to the current pipeline

- **Runtime call sequence is unchanged** — the 2 eliminated BA prompts were never executed; the DA merge restores the original single-prompt load.
- **All markers/filenames/schemas** preserved (DOCUMENT / DA_FILE / AA_FILE / TA_FILE / FN_FILE).
- **DAG still terminates at Foundation**; no forbidden edges; consume-and-cite contracts C-1…C-6 intact.
