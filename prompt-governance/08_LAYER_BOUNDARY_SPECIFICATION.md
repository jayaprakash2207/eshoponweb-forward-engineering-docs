# 08 — Layer Boundary Specification (TOGAF-Aligned)

**Document ID:** GOV-08
**Version:** 1.0.0
**Status:** Canonical
**Created:** 2026-06-24
**Authority:** [`../PROMPT_AUDIT_REPORT.md`](../PROMPT_AUDIT_REPORT.md) §4, §6 (boundary validation)
**Depends on:** `02_PROMPT_OWNERSHIP_MATRIX.md`, `07_PROMPT_DEPENDENCY_MODEL.md`
**TOGAF mapping:** BA→Business Architecture · DA→Data (Information Systems) · AA→Application (Information Systems) · TA→Technology Architecture · FN→Architecture Repository / synthesis.

---

## 0. How to read this

For each layer, four formal lists:

- **May Extract** — what it may derive *directly from raw evidence*.
- **May Consume** — what it may take from another **owner** layer (cite, don't re-extract).
- **May Produce** — the artifacts it owns and publishes.
- **Must Not Produce / Must Not Own** — hard prohibitions (boundary violations if breached).

Breaching a "Must Not" is a 🔴 release blocker. These lists operationalize the audit's four confirmed
violations so they cannot recur.

---

## 1. Business Architecture (BA)

| | |
|---|---|
| **May Extract** | Business capabilities, business processes & value streams, semantic business rules, stakeholders/roles, motivation/operating model — from code/docs **as evidence**. |
| **May Consume** | DA entities/data model (C-1); AA interfaces & call flows for process steps (C-2). |
| **May Produce** | Capability map; process/value-stream models; business-rules catalog (semantic); stakeholder & operating model. |
| **Must Not Produce** | Schema/ERD/data dictionary; component/dependency graphs; tech-stack/NFR registries; infrastructure. |
| **Must Not Own** | Data-model cardinality, DDL, stored procedures (DA); call-flow topology (AA); technology versions/infra (TA). *(Closes audit P3/P4 BA→DA leak.)* |

## 2. Data Architecture (DA)

| | |
|---|---|
| **May Extract** | Schema, tables/collections, entities, keys, indexes, ERD, PII, data quality, data flows/lineage, data-ownership, data-store transaction/consistency characteristics, data-layer-embedded rules — incl. **live system queries** (rank 1 evidence). |
| **May Consume** | AA persistence components (where data is accessed, C-2); BA business rules to tag data-layer enforcement (C-4). |
| **May Produce** | Schema catalogue; conceptual data model; ERD; data dictionary; PII inventory; data-quality report; data-flow map; **data-ownership map**; **data-store transaction/consistency assessment**. |
| **Must Not Produce** | Business capability maps; application component/pattern reports; tech-stack/infra blueprints. |
| **Must Not Own** | Semantic business capabilities (BA); application architecture patterns (AA); technology stack (TA). |
| **Now also owns (relocated in)** | **Data-store transaction/consistency assessment** (from TA P9 OUTPUT 4); **data-ownership map** (from AA Stage 05). |

## 3. Application Architecture (AA)

| | |
|---|---|
| **May Extract** | File/project/language inventory; symbols/components/services; interfaces/APIs/entry points; dependency graph; call flows; architecture patterns & violations; application risks; modernization/strangler inputs; **application- & data-level security posture**. |
| **May Consume** | DA data-ownership (C-1) for module↔data mapping; TA stack (C-3) for tech context; BA capabilities (C-4) for strangler framing. |
| **May Produce** | System inventory; component registry; interface catalogue; dependency graph; call-flow map; pattern/violation registers; application risk register; strangler/forward-eng **inputs**; application/data-level security assessment. |
| **Must Not Produce** | Business capability maps; data-ownership maps; schema/ERD; tech-stack/infra/NFR registries. |
| **Must Not Own** | Business capabilities (BA); data ownership & schema (DA); technology stack/infra/NFRs (TA). *(Closes audit P17 Stage 05 AA→BA/DA leak.)* |
| **Now also owns (relocated in)** | **Application/data-level security posture** (from TA P9 OUTPUT 5). |

## 4. Technology Architecture (TA)

| | |
|---|---|
| **May Extract** | Technology-stack inventory; infrastructure & deployment; CI/CD inventory & maturity; NFRs; technical debt/risk; **infrastructure/transport security config** (TLS, network policy, secrets-management mechanism by name). |
| **May Consume** | AA components (C-2) to attach NFRs/infra; DA data stores (C-1) for the **infrastructure dimension only** (e.g., engine/version/hosting — not transaction semantics). |
| **May Produce** | Technology-stack assessment; infrastructure/deployment blueprint; CI/CD maturity; NFR registry; tech-debt/risk register; architecture-pattern catalog (system/infra patterns); infra/transport security assessment. |
| **Must Not Produce** | Data Architecture Assessment (transaction scope, consistency model, migration state); application-level security posture (authZ completeness, CORS semantics); business capabilities. |
| **Must Not Own** | Data semantics/transaction/consistency (DA); application security & components (AA); business capabilities (BA). *(Closes audit P9 OUTPUT 4 & 5 TA→DA / TA→App leaks.)* |

## 5. Foundation / Synthesis (FN)

| | |
|---|---|
| **May Extract** | **Nothing** from raw source as primary fact (FN-1). May read Layer 1 only as a tie-breaker. |
| **May Consume** | All BA/DA/AA/TA **owner** artifacts (C-5). |
| **May Produce** | Enterprise knowledge graph; canonical model; architecture inventory; traceability matrix; forward-engineering input map; reconciliation report; normalization log; open questions. |
| **Must Not Produce** | Any new fact, code, or target design (GR-1, GR-5). |
| **Must Not Own** | Any extraction responsibility (it reconciles, it does not extract). |

---

## 6. Boundary violation catalog (from audit → prevention)

| Audit violation | Boundary rule that now forbids it |
|---|---|
| TA P9 OUTPUT 4 does Data work | TA §4 "Must Not Produce: Data Architecture Assessment" |
| TA P9 OUTPUT 5 does App/Security work | TA §4 "Must Not Produce: application-level security" |
| AA Stage 05 emits business-capability-map | AA §3 "Must Not Produce: Business capability maps" |
| AA Stage 05 emits data-ownership-map | AA §3 "Must Not Produce: data-ownership maps" |
| BA P3/P4 extracts DDL / relationships | BA §1 "Must Not Own: DDL, data-model cardinality" |
| AA P11 invents cloud/k8s terms | AA §3 "Must Not Produce: infra blueprints" (consume TA) |
| Data-store discovery ×5 | Single owner = DA (§2); others consume via C-1 |
| Business-rule extraction ×4 | Single owner = BA (§1, semantic); DA tags only data-layer enforcement |

---

## 7. Boundary conformance checks

For each prompt at review time:
- [ ] Its `produces` ⊆ its layer's **May Produce**.
- [ ] Nothing in `produces` appears in the layer's **Must Not Produce**.
- [ ] Every external fact is in **May Consume** and cited to the owner (GR-3.4).
- [ ] No raw-source extraction of another layer's **Must Not Own** items.
- [ ] FN-only: zero primary extraction; all inputs are owner artifacts.
