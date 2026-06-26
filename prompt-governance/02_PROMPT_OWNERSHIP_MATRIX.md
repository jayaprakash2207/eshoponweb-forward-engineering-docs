# 02 — Prompt Ownership Matrix (Enterprise Responsibility Model)

**Document ID:** GOV-02
**Version:** 1.0.0
**Status:** Canonical
**Created:** 2026-06-24
**Authority:** [`../PROMPT_AUDIT_REPORT.md`](../PROMPT_AUDIT_REPORT.md) §2, §4, §6, "Recommended Ownership Matrix"
**Addresses:** Audit findings F2, F3; finding 7 (no centralized ownership model)

---

## 0. Ownership model

Every responsibility has exactly **one owning layer**. Other layers may **consume** the owner's output
but must never **re-extract** or **re-author** it. Layers explicitly listed as **prohibited** must not
produce the artifact even as a side effect.

Five layers participate:

| Layer | Code | Mandate |
|---|---|---|
| Business Architecture | **BA** | What the business does — capabilities, processes, rules, stakeholders, value streams. |
| Data Architecture | **DA** | How data is structured, governed, and flows — schema, entities, PII, quality, ownership. |
| Application Architecture | **AA** | How the software is structured — components, interfaces, dependencies, call flows, patterns, violations. |
| Technology Architecture | **TA** | What the system runs on — stack, infrastructure, deployment, CI/CD, NFRs, tech debt, transport/infra security. |
| Foundation / Synthesis | **FN** | Cross-track reconciliation, conflict resolution, knowledge graph, canonical model, traceability (`05`). |

> A sixth concern, **Security (SEC)**, is currently split between TA and AA. This matrix assigns
> *application/data-level security posture* to **AA** and *infrastructure/transport security* to **TA**,
> pending a future dedicated SEC layer (noted in `10`).

---

## 1. Master Responsibility Matrix

Legend: **O** = Owner (sole producer) · **C** = Consumer · **✗** = Prohibited owner.

| Responsibility | BA | DA | AA | TA | FN |
|---|:--:|:--:|:--:|:--:|:--:|
| Business capability map | **O** | ✗ | ✗ (was AA Stage 05) | ✗ | C |
| Business processes / value streams | **O** | C | C | ✗ | C |
| Business rules (semantic) | **O** | C | C | ✗ | C |
| Stakeholder / role / operating model | **O** | ✗ | C | ✗ | C |
| Conceptual data model | C | **O** | ✗ | ✗ | C |
| Schema / ERD / data dictionary | ✗ (was BA P3) | **O** | C | C | C |
| PII inventory / data quality | ✗ | **O** | C | C | C |
| Data flows / lineage | C | **O** | C | C | C |
| Data ownership map | ✗ | **O** (was AA Stage 05) | C | C | C |
| Data-store transaction/consistency assessment | ✗ | **O** | C | ✗ (was TA P9 OUT4) | C |
| File/project/language inventory | ✗ | ✗ | **O** | C | C |
| Symbol / component / service extraction | ✗ | ✗ | **O** | ✗ (was TA component map) | C |
| Interface / API / entry-point catalog | C | C | **O** | C | C |
| Dependency graph / call flows | ✗ | ✗ | **O** | C | C |
| Architecture patterns / violations (app) | ✗ | ✗ | **O** | C | C |
| Application risk / strangler / modernization inputs | C | C | **O** | C | C |
| App/data-level security posture | ✗ | C | **O** (was TA P9 OUT5) | C | C |
| Technology-stack inventory | ✗ | ✗ | C | **O** | C |
| Infrastructure / deployment blueprint | ✗ | ✗ | C | **O** | C |
| CI/CD inventory & maturity | ✗ | ✗ | C | **O** | C |
| NFR registry | C | C | C | **O** | C |
| Technical debt / risk register (system) | C | C | C | **O** | C |
| Infra/transport security config | ✗ | ✗ | C | **O** | C |
| Cross-track reconciliation | ✗ | ✗ | ✗ | ✗ | **O** |
| Conflict resolution across layers | ✗ | ✗ | ✗ | ✗ | **O** |
| Enterprise knowledge graph | ✗ | ✗ | ✗ | ✗ | **O** |
| Canonical enterprise model | ✗ | ✗ | ✗ | ✗ | **O** |
| Traceability matrix validation | C | C | C | C | **O** |
| Forward-engineering input map | C | C | C | C | **O** |
| Prompt governance rules (GOV-01) | C | C | C | C | C (owned by Governance, not a pipeline layer) |

---

## 2. Per-responsibility ownership cards (owner · consumers · prohibited)

### Business Architecture (BA)

| Responsibility | Owner | Consumers | Prohibited owners |
|---|---|---|---|
| Business capability map | BA | FN, AA (strangler input), DA | AA, TA, DA |
| Business processes / value streams | BA | FN, AA | TA |
| Business rules (semantic meaning) | BA | FN, DA, AA | TA |
| Stakeholder / operating model | BA | FN | DA, TA |

**Note:** BA may *read* code/schema as evidence but must **cite DA/AA node IDs** for any data or
component fact rather than re-deriving it (closes audit §4 BA→DA leak in P3/P4).

### Data Architecture (DA)

| Responsibility | Owner | Consumers | Prohibited owners |
|---|---|---|---|
| Schema / ERD / data dictionary | DA | FN, BA, AA, TA | BA, AA, TA |
| Conceptual data model | DA | FN, BA | — |
| PII / data quality | DA | FN, TA (security), AA | BA, TA |
| Data flows / lineage | DA | FN, AA | — |
| Data ownership map | DA | FN, AA | **AA** (relocate AA Stage 05 output) |
| Data-store transaction/consistency | DA | FN, TA | **TA** (relocate TA P9 OUTPUT 4) |

### Application Architecture (AA)

| Responsibility | Owner | Consumers | Prohibited owners |
|---|---|---|---|
| Inventory / symbols / components | AA | FN, TA, DA | TA (component map) |
| Interfaces / API catalog | AA | FN, TA, BA | — |
| Dependency graph / call flows | AA | FN, TA | — |
| Patterns / violations / risks (app) | AA | FN | TA |
| App/data-level security posture | AA | FN, TA | **TA** (relocate TA P9 OUTPUT 5) |
| Modernization / strangler inputs | AA | FN | — |

**Note:** AA must **not** author `business-capability-map` or `data-ownership-map` (relocate AA Stage 05
outputs to BA and DA respectively). AA *consumes* them as forward-engineering inputs.

### Technology Architecture (TA)

| Responsibility | Owner | Consumers | Prohibited owners |
|---|---|---|---|
| Technology-stack inventory | TA | FN, AA | AA framework-detect (raw hint only), layer1 (raw feed only) |
| Infrastructure / deployment / CI-CD | TA | FN, AA | — |
| NFR registry / tech debt | TA | FN, AA | — |
| Infra/transport security config | TA | FN, AA | — |

**Note:** TA must **not** produce a Data Architecture Assessment (transaction scope, consistency,
migration state) or an application-level Security Assessment — these are DA and AA respectively.
TA *consumes* DA/AA outputs and adds only the infrastructure/transport dimension.

### Foundation / Synthesis (FN)

| Responsibility | Owner | Consumers | Prohibited owners |
|---|---|---|---|
| Cross-track reconciliation | FN | all downstream (forward-engineering) | BA, DA, AA, TA |
| Conflict resolution | FN | all | BA, DA, AA, TA |
| Enterprise knowledge graph | FN | forward-engineering package | BA, DA, AA, TA |
| Canonical model / inventory | FN | forward-engineering package | — |
| Traceability validation | FN | governance | — |

**Note:** FN is the **only** layer permitted to merge facts across tracks. No extraction layer may
reconcile another layer's output (closes audit finding 10 + the duplicate-extraction risk).

---

## 3. Duplicate-extraction resolution (audit §4.2)

| Task duplicated | Audit owners (to remove) | Single owner (target) | Others become |
|---|---|---|---|
| Data-store / entity / schema discovery (×5) | layer1 db_extractor, layer2 P4, DA P6, TA P8, BA P1 | **DA** | layer1 = raw deterministic feed to DA; all others **consume** DA, cite DA node IDs |
| Business-rule extraction (×4) | BA P2, layer2 P4, DA "hidden rules", layer1 heuristic | **BA** (semantic) | DA keeps only data-layer-embedded rules and tags them for BA; layer1 heuristic = hint only |
| Technology-stack detection (×3) | layer1 config, TA P8, AA inventory | **TA** | layer1 = raw feed; AA framework-detect = hint, cite TA |
| Component / service extraction (×3) | AA, TA, P9 | **AA** | TA consumes AA component set |
| Business-capability mapping (×2) | BA, AA Stage 05 | **BA** | AA consumes BA capability map |

**Rule:** "Raw deterministic feed" (e.g., `layer1` extractors) is permitted as *input evidence* to the
owning layer but is **not** an authoritative artifact. Only the owning layer publishes the canonical
artifact; the Foundation layer reconciles if feeds disagree.

---

## 4. Enforcement

- Each prompt's `Responsibilities` and `Forbidden Actions` (per `03_PROMPT_STANDARD.md`) MUST match this
  matrix exactly.
- A prompt producing an artifact it does not **O**wn here is a **release blocker**.
- The Foundation layer's traceability validation (`05`) checks that every node's producing layer equals
  its matrix owner; mismatches are flagged `DISCREPANCY`.
