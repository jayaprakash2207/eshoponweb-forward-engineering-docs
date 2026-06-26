# 04 — Migration Guide (Optimized 18-prompt → prompt-v2 10-prompt)

**Date:** 2026-06-24 · No code generated here; this is the plan + rollback. Prompt authoring only.

---

## 1. Change set

| # | Change | Type | Artifact |
|---|---|---|---|
| M1 | Author 3 consolidated shared files (GOV, CONFIDENCE, VALIDATION) | prompt source | `prompt-v2/Shared/` |
| M2 | Author 10 layer prompts (5 EXTRACT + 5 VALIDATE) | prompt source | `prompt-v2/<Layer>/` |
| M3 | Resolve shared references inline (like prompt-resolved) for runner use | generate | `prompt-v2-resolved/` (future) |
| M4 | Repoint runners to v2 prompts (multi-stage → single phased prompt) | code (deferred) | layer2/layer3/da/ta/aa/fn runners |
| M5 | Pilot re-validate on eShopOnWeb (artifact + graph diff) | run | — |

> AA/TA/FN runners currently invoke multiple prompt files per layer. v2 replaces those with one phased
> EXTRACT prompt + one VALIDATE prompt per layer. **M4 is the only executable change and is deferred**
> (no code in this task).

## 2. Runner repoint map (M4 — deferred code)

| Runner(s) | Old prompt load | v2 prompt load |
|---|---|---|
| layer2 + layer3 runners | layer2_prompt + layer3_prompt | **BA-EXTRACT** (single, phased) → **BA-VALIDATE** |
| da_agent1 + da_agent2 | DA scout/analyst + review | **DA-EXTRACT** → **DA-VALIDATE** |
| aa_runner (stages 01–07) | 8 stage prompts | **AA-EXTRACT** (6 internal phases) → **AA-VALIDATE** (2 sections) |
| ta_agent1 + ta_agent2 | StackScout + DeepAnalyst | **TA-EXTRACT** (2 phases) → **TA-VALIDATE** |
| FN runner (deferred) | FN-SYNTH-01/02 + REVIEW | **FN-SYNTHESIZE** (2 phases) → **FN-VALIDATE** |

**Critical M4 note:** because each EXTRACT is multi-phase, the runner must still drive the phases (loop or
sequential calls) and persist each phase's artifacts — OR the model must be instructed (as the prompt is)
to emit all phase artifacts in one structured response. Either way, **intermediate artifacts must be
written**, not skipped (GR-6.1). The runner change is mechanical but must preserve phase persistence.

## 3. Wave sequence

```
Wave 0  Snapshot prompt-resolved/ (18-prompt baseline) as rollback.                    [reversible]
Wave 1  M1+M2: author prompt-v2/ (done — this package).                                [source only]
Wave 2  M3: resolve shared refs into a runnable prompt-v2-resolved/.                    [generate]
Wave 3  M4: repoint ONE layer (start with DA — lowest risk, already 1:1) and pilot.     [code]
Wave 4  Validate DA layer parity (13+2 files); then repoint BA, TA, AA, FN in turn.     [code+run]
Wave 5  Full pilot on eShopOnWeb; diff artifacts + 274-node graph vs baseline.          [run]
Wave 6  Cut over; retain 18-prompt set read-only.                                       [ops]
```

Roll out **layer by layer** (DA first — it is a true 1:1+rename; AA last — highest phase complexity).

## 4. Validation gates (before retiring the 18-prompt set)

| Gate | Condition |
|---|---|
| V1 | Each EXTRACT emits all its phase artifacts (parse-first intact) |
| V2 | All legacy filenames/markers present (DOCUMENT/JSON/DA_FILE/AA_FILE/TA_FILE/FN_FILE) |
| V3 | Knowledge graph: 9 sections / 274 nodes / 117 links; FN-VALIDATE = PASS |
| V4 | Governance re-check: GOV-01/02/03/04/07/08 = 100% |
| V5 | Shared refs resolve; no `{{`/unresolved markers in prompt-v2-resolved/ |

## 5. Rollback

| Trigger | Action | Cost |
|---|---|---|
| Any EXTRACT skips a phase / drops an intermediate artifact | Revert that layer's runner to its multi-stage 18-prompt loads (Wave 0 snapshot) | minutes (per layer) |
| Graph/output diff fails in pilot | Revert affected layer(s); FN inputs restored → graph restored | minutes |
| Governance regression | Revert; the 18-prompt set is retained read-only | minutes |

- The 18-prompt set (`prompt-resolved/` + `prompt-refactored/`) is **retained, not deleted** — rollback is
  a per-layer runner-constant revert.
- Because migration is **layer-by-layer**, a failure in one layer rolls back only that layer; the others
  stay on v2. No big-bang risk.

## 6. Recommended order rationale

DA (1:1 + rename, lowest risk) → BA (3 phases, well-understood) → TA (2 phases, 2 runners→1) → FN
(2 phases, deferred runner anyway) → AA (6 phases, highest complexity, last). Each layer is independently
gated and independently reversible.
