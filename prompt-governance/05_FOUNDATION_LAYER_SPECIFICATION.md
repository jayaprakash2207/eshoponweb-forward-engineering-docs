# 05 — Foundation / Synthesis Layer Specification

**Document ID:** GOV-05
**Version:** 1.0.0
**Status:** Canonical — defines a NEW first-class layer
**Created:** 2026-06-24
**Authority:** [`../PROMPT_AUDIT_REPORT.md`](../PROMPT_AUDIT_REPORT.md) finding 10 (no first-class synthesis layer), R1 (inconsistent truth), §4.2
**Depends on:** `01_GLOBAL_PROMPT_RULES.md`, `02_PROMPT_OWNERSHIP_MATRIX.md`, `04_CONFIDENCE_STANDARD.md`
**Reference outputs:** `ENTERPRISE_KNOWLEDGE_GRAPH.json`, `CANONICAL_ENTERPRISE_MODEL.md`, `TRACEABILITY_MATRIX.md`, `ARCHITECTURE_INVENTORY.md`, `FORWARD_ENGINEERING_INPUT_MAP.md`

---

## 0. Why this layer exists

Today BA/DA/AA/TA all fan out from Layer 1 and **never reconcile** inside the pipeline. The same entity
can be described 4–5 times with divergent answers (audit §4.2, R1). The reconciliation that *does* exist
(the `ENTERPRISE_KNOWLEDGE_GRAPH.json` foundation package) is produced **outside** the prompt pipeline.

The **Foundation / Synthesis Layer (FN)** makes reconciliation a first-class, governed pipeline stage. It
is the **only** layer permitted to merge facts across tracks and resolve conflicts. It converts four
parallel, independently-confident layer outputs into one canonical, traceable enterprise model.

---

## 1. Position in the pipeline

FN runs **after** all four extraction layers complete and **before** the forward-engineering package:

```
BA ─┐
DA ─┤
AA ─┼──►  FOUNDATION / SYNTHESIS (FN)  ──►  Forward-Engineering Package
TA ─┘            (reconcile · resolve · graph · validate)
```

FN owns no extraction. It **consumes** every owning layer's published artifacts and **produces** the
canonical layer.

---

## 2. Responsibilities (from GOV-02)

1. **Cross-track reconciliation** — unify the same real-world concept described by multiple layers into one canonical node.
2. **Conflict resolution** — resolve contradictions using the enterprise evidence hierarchy (GR-2.2).
3. **Knowledge graph generation** — emit `ENTERPRISE_KNOWLEDGE_GRAPH.json`.
4. **Traceability validation** — verify Capability→Process→Entity→Service→API chains and owner correctness.
5. **Canonical model generation** — emit human-readable canonical model + inventory + FE input map.

No other layer may perform 1–5 (GOV-02 prohibits it).

---

## 3. Inputs

| Input | Source layer | Required | Notes |
|---|---|---|---|
| Business capability map, processes, rules, stakeholders | BA | Yes | Owner artifacts only |
| Schema/ERD, data dictionary, PII, data flows, data ownership | DA | Yes | Owner artifacts only |
| Inventory, components, interfaces, dependency graph, call flows, patterns, app-security | AA | Yes | Owner artifacts only |
| Stack inventory, infra/CI-CD, NFRs, tech debt, infra-security | TA | Yes | Owner artifacts only |
| `layer1` deterministic feeds | Layer 1 | Optional | Tie-breaker evidence only; never authoritative |
| Confidence labels + citations | all | Yes | Per GOV-04; preserved verbatim (GR-1.6) |

**Quality gate (GR-7.1):** FN stops if any of the four owner tracks is missing or its required artifacts
are schema-invalid.

---

## 4. Outputs

| Output | Format | Mirrors reference artifact |
|---|---|---|
| `ENTERPRISE_KNOWLEDGE_GRAPH.json` | JSON | canonical source of truth |
| `CANONICAL_ENTERPRISE_MODEL.md` | Markdown view | human-readable model |
| `ARCHITECTURE_INVENTORY.md` | Markdown | flat node inventory |
| `TRACEABILITY_MATRIX.md` | Markdown | capability→…→API chains |
| `FORWARD_ENGINEERING_INPUT_MAP.md` | Markdown | FE input catalog |
| `normalization_log` (in graph) | JSON section | every merge/rename recorded |
| `open_questions` (in graph) | JSON section | unresolved gaps |
| `reconciliation-report.md` | Markdown | conflicts + resolutions + residual DISCREPANCYs |

### 4.1 Knowledge graph schema (authoritative top-level sections)

Confirmed from `FORWARD_ENGINEERING_INPUT_MAP.md`:

```
metadata · business · data · application · technology ·
cross_links · assumptions · normalization_log · open_questions
```

Each node MUST carry: `id`, `type`, `name`, `owner_layer`, `confidence` (GOV-04), `evidence[]`
(citations, GR-3), and `status`. Cross-track identity is expressed via `cross_links`.

---

## 5. Rules (FN-specific, narrower than GOV-01)

| ID | Rule |
|---|---|
| FN-1 | FN never extracts from raw source as a primary fact; it only consumes owner artifacts (+ optional layer1 tie-breakers). |
| FN-2 | When two layers describe the same concept, FN creates **one** canonical node and links the contributors via `cross_links`; it never deletes a contributor's evidence. |
| FN-3 | Conflicts are resolved by GR-2.2 rank; ties escalate as `DISCREPANCY` into `open_questions` (never resolved arbitrarily). |
| FN-4 | Every merge/rename is appended to `normalization_log` with before/after IDs and rationale (mirrors GR-9 change records). |
| FN-5 | FN preserves upstream confidence; it may only *lower* an effective confidence on conflict, never silently raise it. |
| FN-6 | The producing layer of every node must equal its GOV-02 owner; mismatches are flagged, not "fixed" by reassignment. |
| FN-7 | No code, no target design, no new facts (GR-1, GR-5). FN is a projection + reconciliation, not a generator. |

---

## 6. Reconciliation algorithm (specification, not code)

```
INPUT:  artifacts from BA, DA, AA, TA  (each = set of nodes with id, type, name, owner, confidence, evidence)
OUTPUT: ENTERPRISE_KNOWLEDGE_GRAPH.json + views

STEP 1 — INGEST & VALIDATE
  - Load each owner track; run GR-7 structural validation.
  - Reject (FAIL) if a required track is missing/invalid.

STEP 2 — IDENTITY RESOLUTION (entity resolution across tracks)
  - Bucket nodes by (type, normalized-name, key signals: file path, table name, route, capability label).
  - Candidate matches when buckets overlap on ≥1 strong signal (path/table/route) OR ≥2 weak signals (name+domain).
  - Each match group → one CANONICAL node; contributors recorded.

STEP 3 — OWNERSHIP NORMALIZATION
  - For each canonical node, set owner_layer = GOV-02 owner for that responsibility.
  - If a contributor came from a NON-owner layer (e.g., capability authored by AA), keep its evidence
    but mark the contribution "non-owner" and emit a traceability flag (relocation candidate, not auto-fix).

STEP 4 — CONFLICT RESOLUTION
  - For conflicting attribute values across contributors:
      rank each by GR-2.2; highest rank wins.
      if equal rank and different value → label node attribute DISCREPANCY; add to open_questions.
  - Confidence of canonical node = min(contributing confidences) unless higher-rank evidence justifies higher (with change record).

STEP 5 — CROSS-LINKING
  - Build cross_links: capability→process→entity→service→api using owner-layer IDs only.
  - Any dangling link (target missing) → unknown + open_question (GR-1.2, GR-7.4).

STEP 6 — TRACEABILITY VALIDATION
  - Assert every capability traces to ≥1 process; process→entity; service→api; etc. (TRACEABILITY_MATRIX).
  - Report coverage gaps as PARTIAL with open_questions.

STEP 7 — EMIT
  - Write graph (all 9 sections) + 4 markdown views + reconciliation-report.md.
  - normalization_log captures every STEP 2–4 decision.
```

---

## 7. Validation

| Check | Rule |
|---|---|
| Schema completeness | All 9 graph sections present; every node has id/type/owner/confidence/evidence. |
| Owner correctness | Each node's owner = GOV-02 owner (FN-6); mismatches flagged. |
| No orphan links | Every `cross_links` endpoint resolves to a node (GR-7.2). |
| Confidence integrity | No silent raises (FN-5); raises have change records. |
| Traceability coverage | Capability→…→API chains complete or gaps in open_questions. |
| Determinism | Same inputs + pinned model → identical graph (GR-10.3). |
| Verdict | Emit PASS/PARTIAL/FAIL per GOV-04 §5. |

---

## 8. Foundation prompt(s)

Implemented as Analyst/Synthesis-role prompt(s) per `03_PROMPT_STANDARD.md`:

| prompt_id | role | consumes | produces |
|---|---|---|---|
| `FN-SYNTH-01` | Synthesis | BA/DA/AA/TA owner artifacts | knowledge graph + normalization_log |
| `FN-SYNTH-02` | Synthesis | knowledge graph | canonical model, inventory, traceability matrix, FE input map |
| `FN-REVIEW-01` | Review | knowledge graph + views | reconciliation-report.md, verdict |

These are specified here and scheduled in `06_PROMPT_REFACTORING_PLAN.md`; no code is generated now.
