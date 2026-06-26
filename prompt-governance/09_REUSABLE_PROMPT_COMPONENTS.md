# 09 — Reusable Prompt Components

**Document ID:** GOV-09
**Version:** 1.0.0
**Status:** Canonical — component library
**Created:** 2026-06-24
**Authority:** [`../PROMPT_AUDIT_REPORT.md`](../PROMPT_AUDIT_REPORT.md) §8 (reusability), §3.1 (duplication)
**Depends on:** GOV-01, GOV-03, GOV-04

---

## 0. Purpose

The audit found reusability "structurally low despite high conceptual repetition — the building blocks
exist but are duplicated rather than referenced." This document defines five **reusable prompt
components** as named, versioned, include-by-reference blocks. Prompts **reference** a component by ID;
they never copy its text. This is how GOV-01/04 stop being re-stated.

**Include syntax (convention):** a prompt cites a component as:

> `{{include: CMP-GOV v1.0.0}}`

The build/runtime assembler (orchestration, code deferred per refactoring rules) resolves includes into
the final prompt sent to the model. Until assembly exists, prompts cite the component ID in prose and the
review gate verifies no inline duplication remains.

---

## 1. CMP-GOV — Governance Block

**Resolves to:** the GOV-01 reference header + the rule IDs relevant to the prompt's role.

```
{{include: CMP-GOV}}
─────────────────────────────────────────────
Governed by GOV-01 (01_GLOBAL_PROMPT_RULES.md) v1.0.0. Rules are inherited, not restated.
Applicable rule groups: GR-1 (anti-hallucination), GR-2/3 (evidence & citation),
GR-4 (exclusions), GR-5 (no-modification), GR-6 (chunking), GR-7 (validation),
GR-8 (output), GR-10 (model/reproducibility).
Ownership per GOV-02. Boundaries per GOV-08.
─────────────────────────────────────────────
```

**Parameters:** `role` (Scout|Analyst|Review|Synthesis) selects emphasis (e.g., Scout adds "declaration-level only").
**Replaces:** AA golden rules (×3), exclusion list (×5), anti-hallucination (×~20).

## 2. CMP-CONF — Confidence Block

**Resolves to:** the GOV-04 reference + label decision rules.

```
{{include: CMP-CONF}}
─────────────────────────────────────────────
Confidence per GOV-04 (04_CONFIDENCE_STANDARD.md) v1.0.0.
Emit exactly one label per finding: HIGH | MEDIUM | LOW | ASSUMED | DISCREPANCY.
Decision order: DISCREPANCY → HIGH → MEDIUM → LOW → ASSUMED (GOV-04 §2).
LOW/ASSUMED/DISCREPANCY require a reason. HIGH requires a direct citation (GR-2.5).
Never define a local numeric or categorical scale.
─────────────────────────────────────────────
```

**Parameters:** `material_nodes` (list) → escalation triggers for ASSUMED on those nodes.
**Replaces:** 3 incompatible confidence schemes (numeric / categorical / gate verdict).

## 3. CMP-VALID — Validation Block

**Resolves to:** GR-7 quality gates + the prompt's output→check mapping.

```
{{include: CMP-VALID}}
─────────────────────────────────────────────
Validation per GR-7. Before completion:
- Stop if any required input is missing/empty/invalid (GR-7.1).
- Emitted JSON parses; graph edges resolve to nodes (GR-7.2).
- Sibling cross-references agree (GR-7.3).
- Every `unknown`/ambiguity → Open Question with ID (GR-7.4).
- Self-check: no invented facts, all cited, no excluded paths, no source modified (GR-7.5).
- Emit run verdict: PASS | PARTIAL | FAIL (GOV-04 §5).
─────────────────────────────────────────────
```

**Parameters:** `outputs[]` → auto-expands to one check row per declared output.
**Replaces:** ad-hoc per-prompt quality gates; AA's separate verdict vocabulary.

## 4. CMP-EVID — Evidence Block

**Resolves to:** the canonical evidence hierarchy + citation format.

```
{{include: CMP-EVID}}
─────────────────────────────────────────────
Evidence per GR-2. Strength hierarchy (high→low):
1 live system/DB · 2 migration/IaC/manifest · 3 entity/ORM/source decl ·
4 tests · 5 source logic/usage · 6 naming · 7 docs/comments/git.
Higher rank wins on conflict; never average (GR-2.4). Docs (7) win only if citing a hard constraint.
Citations are machine-resolvable: path/to/file.ext:line (GR-3.1).
Every emitted node carries ≥1 citation or `unknown` + Open Question.
─────────────────────────────────────────────
```

**Parameters:** `live_source` (bool) → toggles rank-1 instructions (e.g., DA live-DB attempt).
**Replaces:** Evidence Strength Hierarchy duplicated verbatim in DA P6/P7; scattered citation rules.

## 5. CMP-OUT — Output Block

**Resolves to:** GR-8 output discipline + the marker/delimiter convention.

```
{{include: CMP-OUT}}
─────────────────────────────────────────────
Output per GR-8. Produce exactly the artifacts in `Outputs` — no more, no fewer (GR-8.1).
Use the project's standard multi-file markers (do not invent new schemes, GR-8.2):
   ===FILE_START:<relative/path>===
   <content>
   ===FILE_END===
Structure is deterministic: stable field names, ordering keys, IDs (GR-8.3).
If an output cannot be completed, emit it with status: incomplete + reason + Open Questions (GR-8.5).
Never write secret values; reference by key name only (GR-5.4).
─────────────────────────────────────────────
```

**Parameters:** `marker_name` (default `FILE`), `audience` (business|technical) for GR-8.4 language.
**Replaces:** 6 near-identical marker schemes + divergent output-capture instructions across runners.

---

## 6. Component → prompt assembly

A conformant prompt's lower half is just includes:

```
## 11. Governance Reference
{{include: CMP-GOV role=<role>}}

## 8. Validation Rules
{{include: CMP-VALID outputs=<this prompt's outputs>}}

## 9. Confidence Rules
{{include: CMP-CONF material_nodes=<...>}}

(evidence + output discipline woven into Allowed Actions / Outputs:)
{{include: CMP-EVID live_source=<bool>}}
{{include: CMP-OUT marker_name=<...> audience=<...>}}
```

Only sections **2–7 and 10** (Purpose, Inputs, Responsibilities, Allowed/Forbidden Actions, Outputs,
Traceability) carry prompt-specific text. Everything governance-shaped is an include.

---

## 7. Component registry

| ID | Name | Version | Sources collapsed | Owner |
|---|---|---|---|---|
| CMP-GOV | Governance | 1.0.0 | GOV-01; ×3 golden rules; ×5 exclusion list; ×20 anti-hallucination | Governance |
| CMP-CONF | Confidence | 1.0.0 | GOV-04; 3 confidence schemes | Governance |
| CMP-VALID | Validation | 1.0.0 | GR-7; ad-hoc gates | Governance |
| CMP-EVID | Evidence | 1.0.0 | GR-2/3; ×2 evidence hierarchy | Governance |
| CMP-OUT | Output | 1.0.0 | GR-8; 6 marker schemes | Governance |

**Versioning:** components use semver; a prompt pins the component version it includes (e.g.,
`CMP-GOV v1.0.0`). Bumping a component is a governed change recorded in its registry row and propagated
via `06`-style waves.

---

## 8. Token impact

Per audit §9, collapsing these into includes removes the ~15–25% redundant instructional tokens. With an
assembler that supports prompt caching, the shared component prefix can be cached once per run rather than
re-sent per call (addresses the `--no-session-persistence` re-send tax noted in the audit).
