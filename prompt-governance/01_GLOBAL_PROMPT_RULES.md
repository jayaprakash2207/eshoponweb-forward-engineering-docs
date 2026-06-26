# 01 — Global Prompt Rules (Single Source of Truth)

**Document ID:** GOV-01
**Version:** 1.0.0
**Status:** Canonical — binding on all prompts
**Created:** 2026-06-24
**Authority:** [`../PROMPT_AUDIT_REPORT.md`](../PROMPT_AUDIT_REPORT.md) §3.1, §7, F4
**Supersedes:** All inline governance blocks in BA/DA/TA/AA prompts and the standalone
`application-architecture/architecture-prompts/00-global-rules.md` (which becomes a thin pointer to this file).

---

## 0. How this file is used

This is the **one** place enterprise prompt governance lives. The audit found governance rules
duplicated 2–7× (exclusion list ×5, AA golden rules ×3, anti-hallucination ×~20). This file ends that.

**Binding rule:** Every prompt MUST begin its rules section with:

> `Governed by GOV-01 (01_GLOBAL_PROMPT_RULES.md) v1.0.0. The rules below are inherited, not restated.`

No prompt may re-state, paraphrase, or fork any rule defined here. A prompt may only **add a narrower
constraint** in its own `Forbidden Actions` block (see `03_PROMPT_STANDARD.md`) — never a looser one.

Each rule has a stable ID (`GR-x`). Prompts and reviews cite the ID, not the prose.

---

## GR-1 — Anti-Hallucination

| ID | Rule |
|---|---|
| GR-1.1 | Never invent facts. Architecture facts (modules, owners, call flows, schemas, technologies, capabilities, rules) must come from observed evidence. |
| GR-1.2 | When evidence is missing or ambiguous, output the literal token `unknown` and route the gap to Open Questions (see GR-7.4). Never guess to fill a field. |
| GR-1.3 | Do not elevate inferred content to fact. Inference is marked per the confidence standard (`04_CONFIDENCE_STANDARD.md`) — `ASSUMED` at minimum. |
| GR-1.4 | Preserve verbatim values. Never paraphrase exact version numbers, thresholds, timeouts, enum/state values, or configuration values. Copy them exactly. |
| GR-1.5 | Do not merge distinct items to look tidy (two configs, two components, two rules stay two rows). |
| GR-1.6 | Confidence/status signals from upstream are preserved verbatim and never silently raised. |

## GR-2 — Evidence Requirements

| ID | Rule |
|---|---|
| GR-2.1 | Every material claim carries evidence: source file path, and line number where available. |
| GR-2.2 | Evidence Strength Hierarchy (enterprise canonical, replaces all per-track variants): **(1) live system/DB query → (2) migration/IaC/manifest declarations → (3) entity/ORM/source declarations → (4) tests → (5) source logic/usage → (6) naming conventions → (7) docs/comments/git history.** Higher rank wins on conflict. |
| GR-2.3 | Docs/comments/git history (rank 7) may only win a conflict if they cite a hard, named constraint; otherwise code/declared evidence wins. |
| GR-2.4 | On conflicting evidence, do **not** average. Rank both sources by GR-2.2; the higher-ranked wins; record both and the resolution (see GR-9, change records). |
| GR-2.5 | A finding may be marked `HIGH` confidence only with a direct code/config/declaration reference (see `04_CONFIDENCE_STANDARD.md`). |

## GR-3 — Citation Requirements

| ID | Rule |
|---|---|
| GR-3.1 | Citations are machine-resolvable: `path/to/file.ext:line` (line optional only when not derivable). |
| GR-3.2 | Every emitted node/row carries a stable ID and at least one citation, or `unknown` + Open Question. |
| GR-3.3 | When consuming an upstream artifact, cite the upstream node ID (cross-track traceability for the Foundation layer — `05_FOUNDATION_LAYER_SPECIFICATION.md`). |
| GR-3.4 | Cross-layer claims cite the **owning** layer's node ID, never a re-derived local copy (see Ownership Matrix `02`). |

## GR-4 — Exclusion Folders & Files (single canonical list)

> Replaces the 5 duplicated copies (`BA Scout`, `TA StackScout`, `AA master`, `AA 00-global-rules`, `layer1/file_filter.py`). All consumers reference `GR-4`.

**Never scan / never count as evidence:**

```
.git/            node_modules/     dist/            build/
bin/             obj/              target/          out/
coverage/        .vscode/          .idea/           vendor/
__pycache__/     .venv/  venv/     .next/  .nuxt/   logs/
generated/       tmp/  temp/       .cache/          packages/  (restored deps)
```

**File patterns to ignore:**
```
*.min.js   *.bundle.js   *.map   *.lock   *.dll   *.exe   *.pdb
*.class    *.pyc         large generated/compiled binaries
lockfiles used only as a version fallback (see GR-4.3)
```

| ID | Rule |
|---|---|
| GR-4.1 | Directories/patterns above are excluded from scanning and from evidence. |
| GR-4.2 | If the same component appears across allowed locations, read once, mark `SHARED`, and reference by path elsewhere (no re-reading, no duplicate rows). |
| GR-4.3 | Lock files are read **only** as a version fallback when a primary manifest omits a version; such findings are `LOW` confidence with reason `version sourced from lock file`. |

## GR-5 — No-Modification Rules

| ID | Rule |
|---|---|
| GR-5.1 | Never modify, refactor, rename, reformat, or delete legacy/source files. |
| GR-5.2 | Never generate production code into the analyzed repository. |
| GR-5.3 | All outputs go to the designated `OUTPUT_ROOT` only. |
| GR-5.4 | Never write secret values (API keys, passwords, connection-string credentials). Reference secrets by key name only. |
| GR-5.5 | This package itself modifies no source prompts (audit refactoring constraint). |

## GR-6 — Chunk Processing Rules

| ID | Rule |
|---|---|
| GR-6.1 | Parse first, reason second: `inventory → parsed facts → evidence packs → reasoned output`. Never jump from raw source to final reasoning. |
| GR-6.2 | Cumulative registries (IDs such as `BR-`, `NFR-`, `TD-`, entity/state registries) never reset between chunks; IDs are sequential across the whole run. |
| GR-6.3 | Never silently override an upstream-named artifact. Every divergence is logged as a `DISCREPANCY` (see `04`) with evidence. |
| GR-6.4 | Mark partial results as partial (e.g., partial call flows); never present a partial as complete. |
| GR-6.5 | Order chunks by information density / declared priority; record the chunk plan before processing. |
| GR-6.6 | Per-chunk continuity: flag `SHARED`, `VERSION CONFLICT`, and cross-chunk dependencies as they are found; carry them forward. |

## GR-7 — Validation Rules

| ID | Rule |
|---|---|
| GR-7.1 | Quality gate: stop and report if required inputs are missing, empty, or schema-invalid. |
| GR-7.2 | Structural validity: emitted JSON must parse; graph edges must resolve to existing nodes. |
| GR-7.3 | Consistency: counts and cross-references between sibling artifacts must agree (e.g., entities cited in a flow exist in the entity set). |
| GR-7.4 | Every `unknown` and every unresolved ambiguity becomes an Open Question item with an ID. |
| GR-7.5 | Self-check before completion: no invented facts (GR-1), all claims cited (GR-2/3), no excluded paths used (GR-4), no source modified (GR-5). |

## GR-8 — Output Rules

| ID | Rule |
|---|---|
| GR-8.1 | Produce exactly the artifacts named in the prompt's `Outputs` section — no more, no fewer. |
| GR-8.2 | Use the project's standard delimiters/markers for multi-file output as specified by the prompt; do not invent new marker schemes. |
| GR-8.3 | Outputs are deterministic in structure: stable field names, stable ordering keys, stable IDs. |
| GR-8.4 | Final-reader artifacts use the audience's language (e.g., business artifacts in business language; technical artifacts may use technical terms) but always remain evidence-cited. |
| GR-8.5 | If an output cannot be fully produced, still emit it with `status: incomplete`, a `reason`, and Open Questions. |

## GR-9 — Change & Review Records (enrichment/review stages)

| ID | Rule |
|---|---|
| GR-9.1 | Any correction or enrichment of an upstream artifact is recorded: `change_id, type (ADDED/CORRECTED/ENRICHED), target_id, what, evidence_source, evidence_detail, confidence_before, confidence_after`. |
| GR-9.2 | Never raise a confidence level without recording the new evidence that justifies it. |
| GR-9.3 | Do not escalate to a human gate a question answerable by reading more in-scope evidence. |

## GR-10 — Model & Reproducibility (AI governance)

| ID | Rule |
|---|---|
| GR-10.1 | The model is pinned per run and recorded in the run manifest (addresses audit F6 — no model pinning). |
| GR-10.2 | Each prompt declares a version (`03_PROMPT_STANDARD.md` metadata); the run manifest records prompt versions used. |
| GR-10.3 | Runs are reproducible: identical inputs + pinned model + pinned prompt versions must be diffable. |

---

## Conformance

A prompt is **GOV-01 conformant** when it (a) cites GOV-01 v1.0.0, (b) restates no rule herein,
(c) adds only narrower constraints, and (d) maps every output to a validation rule (GR-7) and a
confidence rule (`04`). Non-conformance is a release blocker (see `06`, Gate criteria).
