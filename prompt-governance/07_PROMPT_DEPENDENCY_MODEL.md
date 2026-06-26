# 07 — Prompt Dependency Model (Target Architecture)

**Document ID:** GOV-07
**Version:** 1.0.0
**Status:** Canonical
**Created:** 2026-06-24
**Authority:** [`../PROMPT_AUDIT_REPORT.md`](../PROMPT_AUDIT_REPORT.md) §5 (dependency analysis)
**Depends on:** `02_PROMPT_OWNERSHIP_MATRIX.md`, `05_FOUNDATION_LAYER_SPECIFICATION.md`, `08_LAYER_BOUNDARY_SPECIFICATION.md`

---

## 0. Principles

1. **Layer 1 (deterministic extraction)** is a shared raw feed, not an authority.
2. Each architecture layer **fans out from raw evidence**, then publishes owner artifacts.
3. **Foundation (FN)** is the sole convergence point — it consumes all four tracks and reconciles.
4. Dependencies flow **toward Foundation**, never backward; no extraction layer depends on another
   extraction layer's *reasoning* (only on its **published, owner-cited facts**, via documented contracts).

> The requested ordering `Business ↓ Data ↓ Application ↓ Technology ↓ Foundation` is honored as the
> **canonical reference/consume order** (and the synthesis sequence into FN). At *runtime* the four
> extraction tracks may execute in parallel from Layer 1; the **dependency direction** for facts is the
> order below.

---

## 1. Target dependency graph

```
                    ┌─────────────────────────────┐
                    │  Layer 1 — Deterministic     │   (raw feeds: source_code, database,
                    │  Extraction (shared)         │    config, logs)  — NOT authoritative
                    └──────────────┬──────────────┘
                                   │ raw evidence (read-only)
        ┌───────────────┬──────────┼───────────┬─────────────────┐
        ▼               ▼          ▼            ▼                 
   ┌─────────┐    ┌─────────┐  ┌─────────┐  ┌─────────┐
   │   BA    │    │   DA    │  │   AA    │  │   TA    │
   │Business │    │  Data   │  │  App    │  │  Tech   │
   └────┬────┘    └────┬────┘  └────┬────┘  └────┬────┘
        │ capability   │ schema     │ components  │ stack
        │ rules        │ entities   │ interfaces  │ infra/NFR
        │              │ data-own.  │ app-security│ infra-security
        └──────────────┴─────┬──────┴─────────────┘
                             ▼   (owner-cited facts only)
                    ┌─────────────────────────────┐
                    │  FOUNDATION / SYNTHESIS (FN) │
                    │  reconcile · resolve · graph │
                    └──────────────┬──────────────┘
                                   ▼
                    ┌─────────────────────────────┐
                    │  Forward-Engineering Package │
                    └─────────────────────────────┘
```

### Canonical consume order (reference direction)

```
BA  →  DA  →  AA  →  TA  →  FN
```

Read as: when a layer needs a fact owned elsewhere, it consumes **leftward** owners by citation; FN
consumes all. This matches the requested vertical chain while keeping runtime parallelism.

---

## 2. Allowed dependencies

| Consumer | May depend on (consume + cite) | Contract |
|---|---|---|
| BA | Layer 1 raw; DA entities (for data-backed rules); AA interfaces (for process steps) | Cite owner node IDs; no re-extraction |
| DA | Layer 1 raw; AA persistence components (where data is accessed) | Cite owner node IDs |
| AA | Layer 1 raw; DA data-ownership (for module/data mapping); TA stack (for tech context) | Cite owner node IDs |
| TA | Layer 1 raw; AA components (to attach NFRs/infra); DA data stores (infra dimension only) | Cite owner node IDs |
| FN | **All** BA/DA/AA/TA owner artifacts; Layer 1 only as tie-breaker | Reconciliation contract (§4) |
| Forward-Engineering | FN canonical graph + views only | Read-only |

Within a layer, **Scout → Analyst → Review** is the standard intra-layer chain (GOV-03 roles).

---

## 3. Forbidden dependencies

| Forbidden | Why |
|---|---|
| Any extraction layer depending on **FN** output | FN is downstream; would create a cycle. |
| BA/DA/AA/TA re-extracting another layer's **owned** artifact | Violates GOV-02; recreates audit §4.2 duplication. |
| A layer consuming another layer's **raw intermediate** instead of its published owner artifact | Bypasses ownership + confidence; un-auditable. |
| Forward-Engineering package depending directly on BA/DA/AA/TA (skipping FN) | Skips reconciliation; reintroduces inconsistent-truth risk R1. |
| Any layer resolving a cross-track **DISCREPANCY** | Only FN resolves (FN-3). |
| Mutual dependency between two extraction layers (cycle) | Breaks DAG; non-deterministic order. |
| Hardcoding a model in any prompt | GR-10.1; pin in run manifest. |

The dependency set MUST be a **DAG** terminating at FN → Forward-Engineering.

---

## 4. Cross-layer contracts

Every cross-layer consume is governed by an explicit contract:

| Contract | Producer → Consumer | Payload | Guarantees |
|---|---|---|---|
| **C-1 Data facts** | DA → BA, AA, TA, FN | schema, entities, data-ownership, PII (node IDs + confidence + citations) | Owner-stable IDs; GOV-04 labels preserved |
| **C-2 Component facts** | AA → TA, FN, BA | components, interfaces, dependency graph, call flows | IDs resolve in graph; partial flows marked partial |
| **C-3 Stack/infra facts** | TA → AA, FN | stack inventory, infra, NFRs, infra-security | Verbatim versions/values (GR-1.4) |
| **C-4 Business facts** | BA → AA, FN, DA | capabilities, processes, rules, stakeholders | Business-language; cited to owner evidence |
| **C-5 Reconciliation** | (BA+DA+AA+TA) → FN | all owner artifacts | FN merges, never deletes contributor evidence (FN-2) |
| **C-6 Canonical** | FN → Forward-Engineering | knowledge graph + 4 views | Single source of truth; 9 sections complete |

**Contract invariants (all C-x):**
- Consumer cites the **producer's owner node ID** (GR-3.4); never a local copy.
- Confidence labels (GOV-04) travel with the fact and are never silently raised (GR-1.6).
- A missing/invalid producer artifact triggers the consumer's quality gate (GR-7.1).

---

## 5. Dependency conformance checks (for the review gate / FN validation)

- [ ] Graph is acyclic and terminates at FN → Forward-Engineering.
- [ ] No extraction layer references FN outputs.
- [ ] Every cross-layer fact carries an owner node ID (no re-extracted copies).
- [ ] No two extraction layers form a cycle.
- [ ] Forward-Engineering consumes only FN.
- [ ] All cross-layer reads map to a named contract C-1…C-6.
