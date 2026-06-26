# Dependency Validation Report (GOV-07)

**Package:** `prompt-refactored/`
**Date:** 2026-06-24
**Basis:** GOV-07 Prompt Dependency Model.

---

## 1. Target dependency graph (as built)

```
layer1 (raw feed, non-authoritative)
   │
   ├─► BA-SCOUT-01 ─► BA-ANALYST-01
   │                 BA-ANALYST-02 ─► BA-ANALYST-03
   │
   ├─► DA-SCOUT-01 ─► DA-ANALYST-01 ─► DA-REVIEW-01
   │
   ├─► AA-SCOUT-01 ─► AA-SCOUT-02 ─► AA-ANALYST-03 ─► AA-ANALYST-04 ─┬─► AA-ANALYST-05
   │                                                                 ├─► AA-ANALYST-06
   │                                                                 ├─► AA-REVIEW-06
   │                                                                 └─► AA-REVIEW-07
   │
   └─► TA-SCOUT-01 ─► TA-ANALYST-01
                │
   (owner-cited facts via C-1..C-4)
                ▼
   FN-SYNTH-01 ─► FN-SYNTH-02 ─► FN-REVIEW-01 ─► [Forward-Engineering]
```

Cross-track consume edges (citation only, no re-extraction):
- BA-ANALYST-01/02/03 → DA (C-1), AA (C-2).
- DA-ANALYST-01 → AA components (C-2).
- AA-ANALYST-00/05/06 → BA (C-4), DA (C-1), TA (C-3).
- TA-ANALYST-01 → AA (C-2), DA (C-1).
- FN-SYNTH-01 → all owner artifacts (C-5).

---

## 2. DAG validation

| Check | Result |
|---|---|
| Graph is acyclic | ✅ no cycles (intra-layer chains + fan-in to FN) |
| Terminates at FN → forward-engineering | ✅ FN-REVIEW-01 is the terminal gate |
| No extraction layer depends on FN output | ✅ FN consumed by nothing upstream |
| No mutual dependency between two extraction layers | ✅ cross edges are consume-only, one-directional per fact owner |
| Forward-engineering consumes only FN | ✅ (FE stage out of scope; contract C-6 enforces) |

**Cycle scan:** BA/DA/AA/TA consume *published owner facts* (not each other's reasoning loops). The
consume edges form a fan-in to FN; no path returns to its origin. Graph is a valid DAG.

---

## 3. Forbidden-dependency scan (GOV-07 §3)

| Forbidden pattern | Found? | Notes |
|---|---|---|
| Extraction layer depends on FN | No | FN is terminal |
| Re-extracting another layer's owned artifact | No | resolved in Ownership report §1 |
| Consuming a raw intermediate instead of published owner artifact | No | all consumes cite owner node IDs (GR-3.4) |
| FE depending directly on BA/DA/AA/TA (skipping FN) | No | FE consumes FN only (C-6) |
| Any layer resolving a cross-track DISCREPANCY | No | only FN-SYNTH-01 resolves (FN-3) |
| Extraction-layer cycle | No | — |
| Hardcoded model in any prompt | No | all `model_pin: required` (GR-10.1) |

---

## 4. Contract conformance (C-1 … C-6)

| Contract | Invariant: consumer cites owner ID | Invariant: confidence travels, not raised | Invariant: missing producer → quality gate | Status |
|---|:--:|:--:|:--:|:--:|
| C-1 Data | ✅ | ✅ | ✅ (GR-7.1 in consumers) | ✅ |
| C-2 Component | ✅ | ✅ | ✅ | ✅ |
| C-3 Stack/infra | ✅ | ✅ | ✅ | ✅ |
| C-4 Business | ✅ | ✅ | ✅ | ✅ |
| C-5 Reconciliation | ✅ (FN preserves contributor IDs) | ✅ (FN-5) | ✅ (FAIL if track missing) | ✅ |
| C-6 Canonical | ✅ | ✅ | ✅ | ✅ |

---

## 5. Consume-vs-extract direction check

For each cross-layer edge, the consumer's prompt was checked to ensure it **references** rather than
**re-derives** the fact:

| Edge | Consumer behavior | Verified |
|---|---|---|
| BA → DA entities | BA-ANALYST-02 emits `da_entity_ref` (not local schema) | ✅ |
| BA → AA interfaces | BA-ANALYST-01 cites AA interface IDs for process steps | ✅ |
| AA → BA capability | AA-ANALYST-05 cites BA capability IDs (does not author map) | ✅ |
| AA → DA data-ownership | AA-ANALYST-05 cites DA ownership IDs | ✅ |
| TA → DA data-store | TA-ANALYST-01 cites DA data-store; only infra dimension added | ✅ |
| AA → TA stack | AA-ANALYST-00 cites TA stack (no invented cloud/k8s) | ✅ |

---

## 6. Verdict

**Dependency validation: PASS.** The refactored prompt set forms a valid DAG terminating at the
Foundation layer; no forbidden edges exist; all cross-layer reads are consume-and-cite under named
contracts C-1…C-6; no model is hardcoded. The architecture is dependency-conformant to GOV-07.
