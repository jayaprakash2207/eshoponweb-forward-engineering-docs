# 03 — Prompt Standard (Canonical Template)

**Document ID:** GOV-03
**Version:** 1.0.0
**Status:** Canonical — mandatory structure for every prompt
**Created:** 2026-06-24
**Authority:** [`../PROMPT_AUDIT_REPORT.md`](../PROMPT_AUDIT_REPORT.md) §3.3 (3 paradigms), §7, finding 9 (no metadata/versioning)
**Depends on:** `01_GLOBAL_PROMPT_RULES.md`, `02_PROMPT_OWNERSHIP_MATRIX.md`, `04_CONFIDENCE_STANDARD.md`

---

## 0. Why one standard

The audit found **three incompatible prompt paradigms** (Scout+DeepAnalyst markdown · AA staged
global-rules · SLM/LLM JSON contract). This standard defines **one** structure all prompts adopt. It
generalizes the strongest paradigm the audit identified — the AA staged model (centralized governance,
parse-first, one concern per stage, cite-don't-copy) — to every layer.

Every prompt is one of two **roles**, but both use the **same 12 sections**:

- **Scout role** — broad, declaration-level inventory; no interpretation.
- **Analyst role** — deep reasoning over a Scout's inventory + evidence; produces final artifacts.

(Review/Gate and Synthesis prompts are Analyst-role variants.)

---

## 1. Mandatory section order

Every prompt MUST contain these 12 sections, in this order, with these headings:

```
1. Metadata
2. Purpose
3. Inputs
4. Responsibilities
5. Allowed Actions
6. Forbidden Actions
7. Outputs
8. Validation Rules
9. Confidence Rules
10. Traceability Rules
11. Governance Reference
12. Version Information
```

A prompt missing any section, or adding governance prose that duplicates GOV-01, is **non-conformant**
(release blocker per `06`).

---

## 2. Canonical template

> Copy this skeleton for every new/migrated prompt. Replace `<…>`. Do not restate GOV-01 rules — cite them.

```markdown
# <PROMPT TITLE>

## 1. Metadata
- prompt_id:        <LAYER>-<ROLE>-<NN>        # e.g. DA-SCOUT-01, AA-ANALYST-04
- version:          <semver>                   # e.g. 1.0.0
- owner_layer:      <BA | DA | AA | TA | FN>
- role:             <Scout | Analyst | Review | Synthesis>
- status:           <draft | active | superseded>
- governed_by:      GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin:        <required: set by run manifest, not hardcoded>   # GR-10
- consumes:         [<artifact ids / upstream prompt ids>]
- produces:         [<artifact ids>]
- last_updated:     <YYYY-MM-DD>

## 2. Purpose
<1–3 sentences. What this prompt is for and where it sits in the pipeline.>

## 3. Inputs
<Each input: name, source artifact/path, schema/shape, and whether required.
 Quality gate: per GR-7.1, stop if a required input is missing/empty/invalid.>

## 4. Responsibilities
<The exact responsibilities this prompt OWNS, copied from GOV-02 (Ownership Matrix).
 Must match the matrix. List nothing this layer does not own.>

## 5. Allowed Actions
<Concrete actions permitted: which files/dirs to read, what depth, what to extract.
 Scout role: declaration-level only. Analyst role: may read logic/bodies as scoped here.>

## 6. Forbidden Actions
<Only NARROWER constraints than GOV-01 (GR-x) — never looser. Explicitly list any
 cross-layer work this prompt must NOT do, referencing GOV-02 prohibited-owner entries.>

## 7. Outputs
<Each output: artifact id, filename, format, delimiter/marker scheme (per GR-8.2),
 and the owning-matrix responsibility it satisfies. Exactly these — no more (GR-8.1).>

## 8. Validation Rules
<Map each output to the GR-7 checks it must pass + any output-specific checks.
 State the stop conditions (quality gates).>

## 9. Confidence Rules
<Reference GOV-04. State which evidence yields HIGH/MEDIUM/LOW/ASSUMED here, and
 how DISCREPANCY is logged for this prompt's domain. Do not invent a local scale.>

## 10. Traceability Rules
<How every emitted node gets a stable ID + citation (GR-3), and which upstream
 node IDs are carried forward for the Foundation layer (GOV-05).>

## 11. Governance Reference
> Governed by GOV-01 (01_GLOBAL_PROMPT_RULES.md) v1.0.0. Rules are inherited, not restated.
> Ownership per GOV-02. Confidence per GOV-04. Boundaries per GOV-08.

## 12. Version Information
- changelog:
  - <semver> — <date> — <change> — <author/role>
- supersedes: <prior prompt file(s), if any>
- migration_ref: 06_PROMPT_REFACTORING_PLAN.md#<anchor>
```

---

## 3. Field rules

| Field | Rule |
|---|---|
| `prompt_id` | `<LAYER>-<ROLE>-<NN>`, globally unique, stable across versions. |
| `version` | Semantic versioning. Behavioral change → major; clarification → minor; typo → patch. |
| `owner_layer` | Exactly one of BA/DA/AA/TA/FN. Must equal the layer that owns every item in `produces` (GOV-02). |
| `model_pin` | Never hardcode a model in the prompt; the run manifest pins it (GR-10.1). The field asserts a pin is *required*. |
| `consumes` / `produces` | Artifact IDs only; drive the dependency model (`07`) and Foundation traceability (`05`). |
| `Responsibilities` | Must be a subset of this layer's **O** entries in GOV-02. Listing a non-owned responsibility = blocker. |
| `Forbidden Actions` | Must include the layer's **prohibited** items from GOV-02 relevant to this prompt's blast radius. |

---

## 4. Conformance checklist (used by review gate)

- [ ] All 12 sections present, in order.
- [ ] Metadata complete; `owner_layer` matches every `produces` owner in GOV-02.
- [ ] No GOV-01 rule restated/paraphrased/forked; only narrower constraints added.
- [ ] Confidence section references GOV-04 only (no third scheme).
- [ ] Every output mapped to ≥1 validation rule and a traceability rule.
- [ ] `model_pin` declared as run-manifest-controlled.
- [ ] Version + changelog present.
- [ ] No source-modification or secret-writing capability granted (GR-5).

---

## 5. Worked stub (illustrative — DA Scout)

```markdown
# Data Architecture — Schema & Entity Scout

## 1. Metadata
- prompt_id: DA-SCOUT-01
- version: 1.0.0
- owner_layer: DA
- role: Scout
- status: draft
- governed_by: GOV-01 v1.0.0
- confidence_model: GOV-04 v1.0.0
- model_pin: required (run manifest)
- consumes: [layer1.database, layer1.config, layer1.source_code]
- produces: [da.schema-catalogue, da.data-source-inventory]
- last_updated: 2026-06-24

## 4. Responsibilities
- Schema / ERD inputs, data-source inventory  (GOV-02: DA = Owner)

## 6. Forbidden Actions
- MUST NOT extract business capabilities or semantic business rules (GOV-02: BA owns).
- MUST NOT assess application components or call flows (GOV-02: AA owns).
- MUST NOT read method bodies for logic (Scout role; Analyst-only).
…
```

(Full prompt rewrites are deferred — see `06`. This stub shows the standard applied.)
