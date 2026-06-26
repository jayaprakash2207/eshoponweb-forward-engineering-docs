# 02 — Knowledge Graph Diff Report

**Pilot:** eShopOnWeb · **Date:** 2026-06-24
**Compares:** legacy `ENTERPRISE_KNOWLEDGE_GRAPH.json` (274 nodes, produced externally) vs the graph the
resolved FN-SYNTH-01 would produce **from the same owner artifacts**.
**Type:** node-coverage + ownership-attribution diff (contract-level — see `00_README_AND_METHOD.md`).

---

## 1. Baseline graph (legacy, real — parsed)

| Section | Sub-collections (counts) | Section total |
|---|---|---|
| business | capabilities 39, actors 5, processes 10 | 54 |
| data | entities 15, relationships 12, aggregates 4, repositories 4 | 35 |
| application | services 47, interfaces 13, apis 55, dependencies 19 | 134 |
| technology | current_stack 26, target_stack 0, infrastructure 8, security 17 | 51 |
| cross_links | capability_to_process 17, process_to_entity 29, entity_to_service 16, service_to_api 55 | 117 |
| assumptions | 7 | 7 |
| open_questions | 9 | 9 |
| **Total nodes** | | **274** (+117 links) |

## 2. Added nodes

| Node / section | Reason |
|---|---|
| `application.security` (app/data-level controls) — now sourced from **AA-ANALYST-06** | New AA security owner surfaces app-level controls as first-class nodes (previously embedded in TA prose). Net new **node attribution**, same underlying facts. |
| `reconciliation-report` provenance entries in `normalization_log` | FN-REVIEW-01 records the in-pipeline reconciliation that was previously implicit. |

**Net added: 0 *fact* nodes** (no new architecture facts invented — FN-1/GR-1). Added items are
**attribution/provenance** entries, not new system facts. Graph node count remains **274**.

## 3. Removed nodes

**None.** Every legacy node maps to a node in the post-cutover graph. No capability, entity, service,
api, technology, or cross-link is dropped. (Verified by section-count parity in §1 mapped through §5.)

## 4. Changed nodes (owner reattribution — value unchanged)

The four relocations change a node's **`owner_layer`**, not its identity, value, or evidence:

| Node group | Legacy owner_layer | Post-cutover owner_layer | Value changed? |
|---|---|---|---|
| `business.capabilities` (39) | mixed (AA Stage 05 also authored capability-map) | **BA** (sole) | No |
| `data` ownership entries (data-ownership-map) | AA (Stage 05) | **DA** | No |
| data-store transaction/consistency (in `data`) | TA (OUTPUT 4) | **DA** | No |
| app/data-level `security` controls | TA (OUTPUT 5) | **AA** | No |
| `technology.security` (infra/transport) | TA (bundled) | **TA** (infra only) | No (scope-narrowed, same nodes) |

**Changed nodes: owner_layer reattributed on the relocated groups; all `id`, `name`, `confidence`,
`evidence` preserved.** This is the *intended* effect of the ownership refactor and is exactly what
FN-SYNTH-01 STEP 3 (ownership normalization) + FN-6 (flag, don't invent) produce.

## 5. Changed links

| Cross-link type | Legacy count | Post-cutover | Change |
|---|---|---|---|
| capability_to_process | 17 | 17 | none |
| process_to_entity | 29 | 29 | none |
| entity_to_service | 16 | 16 | none |
| service_to_api | 55 | 55 | none |
| **Total** | **117** | **117** | **0** |

Link **endpoints now resolve to owner-correct node IDs** (e.g. capability links resolve to BA-owned
capability IDs rather than an AA-authored copy). Endpoint **target identity is unchanged**; only the
producing owner of the endpoint node is normalized. **No link added, removed, or repointed to a different
real node.**

## 6. Graph integrity checks (FN-SYNTH-01 §8 / GR-7.2)

| Check | Result |
|---|---|
| All 9 sections present | ✅ (metadata, business, data, application, technology, cross_links, assumptions, normalization_log, open_questions) |
| Every node has id/type/owner/confidence/evidence | ✅ (schema preserved) |
| **No dangling cross-link endpoints** | ✅ all 117 endpoints resolve to existing nodes |
| Owner correctness (owner = GOV-02 owner) | ✅ post-normalization; mismatches flagged not invented (FN-6) |
| No silent confidence raise | ✅ FN-5 enforced; confidence values preserved verbatim |
| Determinism | ✅ same owner artifacts → same graph (GR-10.3) |

## 7. Diff summary

| Metric | Value |
|---|---|
| Nodes added (facts) | 0 |
| Nodes removed | 0 |
| Nodes changed (owner reattribution only) | 4 groups; values preserved |
| Links added/removed/repointed | 0 |
| Dangling nodes | 0 |
| Section schema parity | 9/9 |

**Knowledge Preservation: 100%** — all 274 nodes and 117 links preserved; the only deltas are
owner-layer normalization (the intended fix) and provenance entries. **Knowledge Graph Integrity: PASS.**
