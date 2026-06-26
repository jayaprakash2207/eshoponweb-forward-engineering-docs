# 03 — Prompt Merge Matrix

**Date:** 2026-06-24
**Rule check applied to every merge:** same layer ✓ · same ownership ✓ · outputs identical ✓ · consumers compatible ✓ · traceability preserved ✓ · governance intact ✓.

---

## MERGE 1 — Data extractor re-unification

```
DA-SCOUT-01  ┐
             ├──►  DA-EXTRACT-01   (owner: DA · role: Scout+Analyst · runner: da_agent1_runner.py)
DA-ANALYST-01┘
```

| Check | Detail |
|---|---|
| **Same layer / owner** | Both Data Architecture, owner=DA. ✅ |
| **Origin** | Legacy was a SINGLE prompt (`DA_REVERSE_ENGINEERING_PROMPT_GENERIC.md`) under a SINGLE runner. The refactor split it; the merge **restores the original contract**. |
| **Responsibilities retained** | (Scout) schema/source/PII/migration inventory + (Analyst) conceptual model, ERD, dictionary, data-flow, quality, storage, redundancy, access-control, hidden-rules, **data-ownership-map**, **datastore-transaction-consistency-assessment**. All retained. |
| **Outputs retained** | All 13 legacy `da-outputs/` files + the 2 relocated-in files. Identical filenames + DA_FILE markers. ✅ |
| **Consumers compatible** | DA-REVIEW-01, BA (entity refs), AA, TA, FN all consume `da-outputs/*` by path/ID — unchanged. ✅ |
| **Traceability** | DA IDs (`TBL-/ENT-/DS-/PII-/OWN-/DR-`) unchanged; carried into FN cross-links. ✅ |
| **Governance** | Single CMP-GOV/CMP-CONF/CMP-EVID(live_source=true) block instead of two copies — *reduces* duplication. ✅ |
| **Risks** | (a) Prompt length grows (scout+analyst in one) → mitigated by internal "Phase A / Phase B" headings. (b) loses the artificial scout/analyst file boundary — acceptable, it was never a runtime boundary (one runner). |
| **Risk level** | 🟢 Low (restores proven legacy contract). |

---

## MERGE 2 — Business Scout discipline folded into the wired extractor

```
BA-SCOUT-01 (unwired)  ──►  folded as "Phase A: structural scout" inside  BA-ANALYST-02
```

| Check | Detail |
|---|---|
| **Same layer / owner** | Both Business Architecture, owner=BA. ✅ |
| **Status** | BA-SCOUT-01 is **not wired to any runner** (verified). Its structural-inventory discipline is valuable but currently produces no pipeline-consumed artifact. |
| **Responsibilities retained** | Structural scan (domains, services, states, roles) becomes BA-ANALYST-02 **Phase A** before rule/entity/capability extraction — the same parse-first discipline (GR-6.1), now inside the wired prompt. |
| **Outputs retained** | BA-ANALYST-02 still emits `layer2_output.json` (unchanged schema). The 6 standalone scout tables were **never consumed** by a runner, so no consumed output is lost. ✅ |
| **Consumers compatible** | layer2_runner unchanged; layer3 consumes layer2_output.json — unchanged. ✅ |
| **Traceability** | Capability/role IDs now originate inside BA-ANALYST-02; still feed FN cross-links. ✅ |
| **Risks** | The standalone 6-table markdown view disappears (it had no consumer). If anyone used it for manual inspection, regenerate on demand from layer2_output.json. |
| **Risk level** | 🟢 Low (eliminating an unwired duplicate). |

---

## ELIMINATE — Business deep-analyst lineage (no merge target needed)

```
BA-ANALYST-01 (unwired)  ──►  RETIRE  (its 8 docs are a subset of BA-ANALYST-03's 10 docs)
```

| Check | Detail |
|---|---|
| **Why not merged** | Its outputs (business capability map, processes, rules, stakeholders, value streams, pain points, automation) are **already produced** by the wired `BA-ANALYST-03` (10 docs) + `BA-ANALYST-02` (rules/capabilities). Merging would duplicate, not consolidate. |
| **Outputs retained** | All 8 doc concerns exist in the layer2/layer3 lineage. ✅ no loss. |
| **Consumers** | None (unwired). ✅ |
| **Risk level** | 🟢 Low (retiring an unwired duplicate). |

---

## REJECTED MERGES (evaluated, not safe)

| Candidate | Verdict | Reason (MERGE RULE failed) |
|---|---|---|
| AA-SCOUT-01 + AA-SCOUT-02 | ❌ keep separate | Distinct parse stages (inventory vs symbols); collapsing risks raw→parsed conflation. |
| AA-ANALYST-03 + AA-ANALYST-04 | ❌ keep separate | Evidence-packs vs final synthesis — GR-6.1 parse-first forbids merging the intermediate representation into the final. |
| AA-REVIEW-06 + AA-REVIEW-07 | ❌ keep separate | Product QA (artifact quality) vs process QA (workflow/manifest audit) — different inputs & verdict scope. |
| TA-SCOUT-01 + TA-ANALYST-01 | ❌ keep separate | **Both wired to separate runners** (ta_agent1/ta_agent2); real load-bearing split (unlike BA). |
| FN-SYNTH-01 + FN-SYNTH-02 | ❌ keep separate | Graph build (canonical JSON) vs view render (markdown); FN-REVIEW gates between; FN-1/FN-7 keep them clean. |
| Any cross-layer review consolidation | ❌ forbidden | Different owners — MERGE RULES prohibit cross-owner/cross-layer merge. |

---

## Merge summary

| Action | Count | Net prompt change |
|---|---|---|
| MERGE (DA Scout+Analyst → DA-EXTRACT-01) | 2→1 | −1 |
| ELIMINATE/FOLD (BA-SCOUT-01 → BA-ANALYST-02 Phase A) | −1 | −1 |
| ELIMINATE (BA-ANALYST-01 unwired duplicate) | −1 | −1 |
| **Total** | | **−3 (21→18)** |

All merges pass every MERGE RULE. No layer crossed; no ownership changed; no consumed output dropped.
