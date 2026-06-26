# 03 — Regression Analysis

**Pilot:** eShopOnWeb · **Date:** 2026-06-24
**Definition:** a regression = a loss of capability, content, compatibility, or governance vs the legacy pipeline. Owner relocations that preserve information are **not** regressions (they are the refactor's intent).

---

## 1. Regression register

| ID | Area | Description | Severity | Root cause | Regression? |
|---|---|---|---|---|---|
| RG-1 | BA `layer2_output.json` | `business_entities[]` element shape: `relationships` → `da_entity_ref` | **Low** | BA→DA ownership fix (entities owned by DA; BA cites) | **No** (compat-managed) — content preserved as a citation; layer3 consumer unaffected |
| RG-2 | AA→BA/DA paths | `business-capability-map.*`, `data-ownership-map.md` move out of `aa-outputs/` | **Low** | GOV-02 ownership relocation | **No** — files exist at owner path; consumers must repoint (G2) |
| RG-3 | TA→DA path | `data-architecture-assessment.md` → DA `datastore-transaction-consistency-assessment.md` | **Low** | GOV-08 boundary fix | **No** — content preserved at DA path |
| RG-4 | TA→AA split | `security-architecture-assessment.md` split (infra→TA, app→AA) | **Low** | GOV-08 boundary fix | **No** — both halves preserved; consumer globbing old single file must repoint |
| RG-5 | Prose wording | Regenerated narrative wording may differ token-for-token from legacy prose | **Medium** | LLM non-determinism on a live re-run | **Potential** — cannot be excluded without a live run; mitigated by pinned model (GR-10) + structural validation |
| RG-6 | DA two-phase load | Single legacy DA prompt → DA-SCOUT + DA-ANALYST | **Low** | prompt split | **No** — same 13-file `da-outputs/` contract; runner-load change only (code, deferred) |

## 2. Severity distribution

| Severity | Count | Items |
|---|---|---|
| Critical | 0 | — |
| High | 0 | — |
| Medium | 1 | RG-5 (prose wording — live-run-only risk) |
| Low | 5 | RG-1, RG-2, RG-3, RG-4, RG-6 |

**Zero critical or high regressions.** The five Low items are all consequences of the intended ownership
relocations and are content-preserving. The one Medium is the inherent re-generation wording risk that
**only a live G3 run can fully close** — it is a *risk*, not an observed regression.

## 3. Root-cause analysis

- **Relocations (RG-2/3/4):** root cause = the four GOV-02/GOV-08 ownership fixes — the entire point of
  the refactor. Information is preserved; only the owning prompt/path changes. Not a defect.
- **Schema shape (RG-1):** root cause = entities are DA-owned; BA must cite, not re-author. Top-level
  `layer2_output.json` keys unchanged; only the entity element internal shape changes; the only consumer
  (`layer3_runner.py`) reads the object, not entity internals → no break.
- **Prose wording (RG-5):** root cause = model non-determinism across runs. The governed prompts pin the
  model (GR-10) and enforce structural determinism (stable IDs, ordering keys), bounding but not
  eliminating wording variance. This is independent of the refactor — the legacy prompts had the same
  property.
- **DA two-phase (RG-6):** root cause = Scout/Analyst split for boundary clarity. Output contract
  unchanged.

## 4. Recommendations

| Ref | Recommendation |
|---|---|
| RG-1 | Document the `business_entities[]` shape change in the layer2 schema notes; confirm no external consumer reads entity `relationships` (only `layer3_runner.py` consumes — verified unaffected). |
| RG-2/3/4 | Execute **G2**: repoint any consumer globbing the old `aa-outputs/`/`ta-outputs/` paths, or place transitional pointer files during a deprecation window. |
| RG-5 | Execute **G3**: one live pilot run on eShopOnWeb with the pinned model; diff structural artifacts (IDs, counts, graph) — expect parity; treat prose deltas as acceptable if structure matches. |
| RG-6 | Land the `da_agent1_runner.py` two-phase load (code, deferred) or concatenate the two resolved DA prompts for single-pass parity. |

## 5. Regression score

```
Regression score = 100 − Σ severity weights (Critical 25, High 10, Medium 4, Low 1)
                 = 100 − (0 + 0 + 4 + 5) = 91 / 100
```

**Regression Score: 91/100.** No critical/high regressions; all material findings are intended,
content-preserving relocations. The residual Medium (prose wording) is gated to the live G3 run.
