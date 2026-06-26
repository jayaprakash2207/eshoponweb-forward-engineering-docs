# Ownership Validation Report (GOV-02 / GOV-08)

**Package:** `prompt-refactored/`
**Date:** 2026-06-24
**Basis:** GOV-02 Ownership Matrix · GOV-08 Layer Boundary Specification.

---

## 1. Single-owner validation (GOV-02)

Each responsibility must have exactly one producing prompt; all others consume-and-cite.

| Responsibility | Sole Owner (produces) | Consumers (cite) | Pre-migration owners (removed) | Status |
|---|---|---|---|---|
| Business capability map | BA-ANALYST-01 / -03 | AA-ANALYST-05, FN | AA Stage 05 | ✅ resolved |
| Business rules (semantic) | BA-ANALYST-01 / -02 | DA (tags), AA, FN | layer2, DA hidden-rules, layer1 heuristic | ✅ resolved |
| Schema / entity / data-store | DA-SCOUT-01 / DA-ANALYST-01 | BA, AA, TA, FN | layer2, BA P1, TA P8, layer1 db | ✅ resolved |
| Data-ownership map | DA-ANALYST-01 | AA-ANALYST-05, FN | AA Stage 05 | ✅ resolved |
| Data-store transaction/consistency | DA-ANALYST-01 | TA, FN | TA P9 OUTPUT 4 | ✅ resolved |
| Components / interfaces / call flows | AA-SCOUT-02 / AA-ANALYST-03 / -04 | TA, FN | TA component map | ✅ resolved |
| App/data-level security | AA-ANALYST-06 | TA, FN | TA P9 OUTPUT 5 | ✅ resolved |
| Technology stack inventory | TA-SCOUT-01 | AA, FN | layer1 config, AA inventory framework-detect | ✅ resolved |
| Infra/transport security | TA-ANALYST-01 | AA, FN | (was bundled in TA OUT5) | ✅ split clean |
| NFRs / tech debt / patterns | TA-ANALYST-01 | AA, FN | — | ✅ clean |
| Cross-track reconciliation / graph | FN-SYNTH-01 / -02 | forward-engineering | (was external to pipeline) | ✅ in-pipeline now |

**Duplicate-extraction count:** data-store 5→1, business-rules 4→1, tech-stack 3→1, components 3→1,
capability-map 2→1. **All collapsed to a single owner.**

---

## 2. Boundary validation (GOV-08 Must-Not lists)

For each prompt: does any `produces` entry fall in its layer's **Must Not Produce/Own**?

| prompt_id | Produces something it Must Not? | Consumes others' facts by citation? | Verdict |
|---|---|---|---|
| BA-SCOUT-01 | No (entity-rel removed) | Yes (DA refs) | ✅ |
| BA-ANALYST-01 | No | Yes (DA/AA) | ✅ |
| BA-ANALYST-02 | No (entities = DA refs) | Yes (DA/TA) | ✅ |
| BA-ANALYST-03 | No (data-model = DA view) | Yes (DA) | ✅ |
| DA-SCOUT-01 | No | n/a (owner) | ✅ |
| DA-ANALYST-01 | No (received items are DA-owned) | Yes (AA components) | ✅ |
| DA-REVIEW-01 | No | n/a | ✅ |
| TA-SCOUT-01 | No | n/a (owner) | ✅ |
| TA-ANALYST-01 | **No** — OUT4/OUT5 removed; infra/transport security only | Yes (DA/AA) | ✅ violation closed |
| AA-ANALYST-00 | No (tech-stack → consume TA) | Yes (TA) | ✅ |
| AA-SCOUT-01/02 | No | n/a | ✅ |
| AA-ANALYST-03/04 | No | n/a | ✅ |
| AA-ANALYST-05 | **No** — capability-map & data-ownership removed | Yes (BA/DA) | ✅ violation closed |
| AA-ANALYST-06 | No (app-level security only) | Yes (DA, TA infra-sec) | ✅ relocation landed |
| AA-REVIEW-06/07 | No | n/a | ✅ |
| FN-SYNTH-01/02 | No (no primary extraction) | Yes (all owners) | ✅ |
| FN-REVIEW-01 | No | n/a | ✅ |

**No prompt produces an artifact outside its layer's May-Produce set.**

---

## 3. The four confirmed violations — closure evidence

| Audit violation | Pre | Post | Evidence in package |
|---|---|---|---|
| TA→Data (P9 OUTPUT 4) | TA produced Data Architecture Assessment | DA-ANALYST-01 produces `datastore-transaction-consistency-assessment.md`; TA-ANALYST-01 §6 forbids it | TA-ANALYST-01, DA-ANALYST-01 |
| TA→App/Security (P9 OUTPUT 5) | TA produced app-level security | AA-ANALYST-06 produces `application-security-assessment.md`; TA keeps infra/transport only | AA-ANALYST-06, TA-ANALYST-01 |
| AA→Business (Stage 05) | AA produced business-capability-map | BA owns; AA-ANALYST-05 consumes + cites | AA-ANALYST-05 §6, BA-ANALYST-01/03 |
| AA→Data (Stage 05) | AA produced data-ownership-map | DA owns; AA-ANALYST-05 consumes + cites | AA-ANALYST-05 §6, DA-ANALYST-01 |

---

## 4. Consume-and-cite enforcement

Every cross-layer fact now flows via a GOV-07 contract with owner-ID citation (GR-3.4):

| Contract | Producer → Consumer(s) | Verified in |
|---|---|---|
| C-1 Data facts | DA → BA, AA, TA, FN | BA-ANALYST-02 (entity refs), AA-ANALYST-05/06, TA-ANALYST-01 |
| C-2 Component facts | AA → TA, FN, DA | TA-ANALYST-01, DA-ANALYST-01 |
| C-3 Stack/infra facts | TA → AA, FN | AA-ANALYST-00 |
| C-4 Business facts | BA → AA, FN | AA-ANALYST-05 |
| C-5 Reconciliation | all → FN | FN-SYNTH-01 |
| C-6 Canonical | FN → forward-engineering | FN-SYNTH-02 (out of this package's scope to consume) |

---

## 5. Verdict

**Ownership & boundary validation: PASS.** Single ownership holds for every responsibility; all four
cross-layer violations are closed; no May-Not-Produce breach remains; cross-layer needs are met by
consume-and-cite under named contracts. The Foundation layer is the sole cross-track merge point.
