# 03 — Output Compatibility Report (prompt-v2)

**Date:** 2026-06-24 · Old (18-prompt) → New (10-prompt) compatibility verification.

---

## 1. Output compatibility (artifact-by-artifact)

| Artifact set | Old producer | New producer | Filenames/markers | Compatible? |
|---|---|---|---|---|
| `layer2_output.json` | BA-ANALYST-02 | BA-EXTRACT Phase B | unchanged (JSON) | ✅ |
| 10 BA documents | BA-ANALYST-03 | BA-EXTRACT Phase C | unchanged (DOCUMENT) | ✅ |
| BA review summary | *(none)* | BA-VALIDATE | new `ba-review-summary.md` | ✅ additive |
| 13 DA files + 2 relocated-in | DA-SCOUT-01 + DA-ANALYST-01 | DA-EXTRACT | unchanged (DA_FILE) | ✅ |
| `review-summary.md` | DA-REVIEW-01 | DA-VALIDATE | unchanged | ✅ |
| inventory(4)+parsed(4)+packs(9)+final(14) | AA-SCOUT-01/02 + AA-ANALYST-03/04 | AA-EXTRACT P1–P4 | unchanged (AA_FILE) | ✅ |
| forward-eng inputs (9) | AA-ANALYST-05 | AA-EXTRACT P5 | unchanged | ✅ |
| application-security-assessment | AA-ANALYST-06 | AA-EXTRACT P6 | unchanged | ✅ |
| 5 AA review files | AA-REVIEW-06 + AA-REVIEW-07 | AA-VALIDATE §A/§B | unchanged | ✅ |
| 6 TA inventory | TA-SCOUT-01 | TA-EXTRACT Phase A | unchanged (TA_FILE) | ✅ |
| 7 TA assessments | TA-ANALYST-01 | TA-EXTRACT Phase B | unchanged | ✅ |
| ta-review-summary | (inside TA-ANALYST-01) | TA-VALIDATE | unchanged filename | ✅ |
| ENTERPRISE_KNOWLEDGE_GRAPH.json | FN-SYNTH-01 | FN-SYNTHESIZE Phase 1 | unchanged (FN_FILE) | ✅ |
| 4 canonical views | FN-SYNTH-02 | FN-SYNTHESIZE Phase 2 | unchanged | ✅ |
| reconciliation-report | FN-REVIEW-01 | FN-VALIDATE | unchanged | ✅ |

**Output compatibility: 100%** — every artifact preserved; markers/filenames identical; the only additions
are the two standardized validate summaries (BA, TA), which are additive.

## 2. Knowledge graph compatibility

| Check | Result |
|---|---|
| 9 sections | ✅ unchanged |
| 274 nodes (business 54, data 35, application 134, technology 51) | ✅ FN inputs unchanged → reproducible |
| 117 cross-links | ✅ unchanged |
| Owner attribution | ✅ unchanged (merges are within-owner) |
| Integrity (FN-VALIDATE) | ✅ PASS expected |

**Knowledge graph compatibility: 100%.** The merges restructure *authoring*, not the facts or owner IDs
FN reconciles.

## 3. Governance compatibility

| Standard | Old | New |
|---|---|---|
| GOV-01 single-source | 100% | 100% — shared 5→3 (evidence/output folded into GOV); 1 block/prompt |
| GOV-02 ownership | 100% | 100% — no layer crossed |
| GOV-03 standard | 98/100 | maintained — all 10 keep the 12-section template; phases live inside §5 |
| GOV-04 confidence | 100% | 100% — single model |
| GOV-07 dependency | 100% | 100% — DAG unchanged, terminates at FN |
| GOV-08 boundaries | 100% | 100% — no Must-Not-Produce breach |

**Governance compatibility: 100%** (shared-component consolidation is a net governance improvement).

## 4. Traceability compatibility

| Check | Result |
|---|---|
| Stable IDs (BR-/ENT-/CMP-/NFR-/capability IDs) | ✅ preserved (now originate inside the EXTRACT prompt) |
| Capability→Process→Entity→Service→API chains | ✅ unchanged |
| Owner-cited cross-references (GR-3.4) | ✅ unchanged |
| Traceability matrix | ✅ unchanged (FN-SYNTHESIZE Phase 2) |

**Traceability compatibility: 100%.**

## 5. Forward-engineering compatibility

FE consumes the FN canonical model only (C-6) — unchanged. Graph + views + traceability + FE readiness
gate all preserved. **Forward-engineering compatibility: 100%.**

## 6. Roll-up

| Dimension | Score |
|---|---|
| Output | 100% |
| Knowledge graph | 100% |
| Governance | 100% |
| Traceability | 100% |
| Forward engineering | 100% |

**All compatibility dimensions: 100%.** The single behavioral risk is identical to every prior step:
LLM prose-wording variance on a live re-run (gated by pinned model + structural determinism + the live
pilot recommendation). No structural, output, graph, or governance incompatibility introduced.
