# Missing Output / Workflow Fixes

> Derived from `architecture-output/final/architecture-workflow-audit.md`. Each item below is a candidate fix, not a committed change. Per global rules, no legacy source or analyzer tooling has been modified by this stage — these are recommendations for a future implementation pass.

## WF-FIX-001 — Reconcile or supersede duplicate `final/` outputs (Priority: Critical)

**Problem:** At least 8 files exist in two conflicting generations under the same path:
`api-contract-preservation-map.json`, `architecture-decision-inputs.md`, `architecture-pattern-report.md`, `architecture-violation-register.json`, `business-capability-map.json`, `business-capability-map.md`, `confidence-report.md`, `data-ownership-map.md`.

**Impact:** Architects reading `final/architecture-pattern-report.md` cannot tell whether the system is a "Clean Architecture/Onion Modular Monolith (0.7)" or a "Layered Monolith (0.78)" — both claims currently coexist with no indication either is superseded.

**Fix:**
1. Add a `generated_at` (ISO-8601 UTC) timestamp field to **every** stage output file's schema (currently only present in Generation-B-style files), enforced by stage 04/05 output writers.
2. On each run of `run_architecture_extraction.py`, write outputs to `architecture-output/final/` as the **current** copy, and additionally copy the **previous** `architecture-output/final/` (if present) into `architecture-output/runs/<previous-generated_at>/final/` before overwriting. This preserves history without ambiguity in the "live" directory.
3. As a one-time manual reconciliation for the current repository state: move the older-looking (no `generated_at`, `schema_version: "1.0"`/`stage: "05-..."` style) copies of the 8 affected files into `architecture-output/runs/pre-2026-06-15T07-27-16Z/final/`, leaving only the Generation-B (`generated_at: 2026-06-15T07:27:16+00:00`) versions in `architecture-output/final/`.
4. Re-run stage 06 (quality review) against the reconciled `final/` directory (see WF-FIX-005).

## WF-FIX-002 — Unify JSON schema per output file (Priority: High)

**Problem:** The same filename is written with two structurally incompatible top-level shapes (e.g. `{schema_version, stage, generated_from, ...}` vs `{generated_at, source_final_artifacts, summary, ...}`).

**Fix:**
1. Define one canonical JSON shape per stage-05/stage-04 output file, including a required envelope: `{ "generated_at": <ISO-8601>, "stage": <string>, "source_artifacts": [<paths>], "schema_version": <string>, ... payload }`.
2. Add `architecture-output/schemas/<filename>.schema.json` for each `final/*.json` output (JSON Schema draft 2020-12 is sufficient).
3. Add a schema-validation step to stage 04/05 output writers (and optionally as part of stage 06) that validates each written JSON file against its schema before the run is considered complete. Fail the run (or flag in `confidence-report.md`) on schema mismatch rather than writing a non-conforming file.

## WF-FIX-003 — Enforce stage input completeness before writing outputs (Priority: High)

**Problem:** `final/confidence-report.md` (Gen A) documents that stage 05 ran with only 2 of the ~16+ expected `final/` input files available, yet still produced a full set of 14 "final" stage-05 outputs.

**Fix:**
1. At the start of stage 05 (and similarly stage 06), enumerate the expected input file list from the stage's own prompt (`05-enterprise-forward-engineering-agent.md` → "Input: `architecture-output/final/`"). Compare against what is actually present/readable.
2. If fewer than N% of expected inputs are present (e.g., < 50%), either:
   - (a) abort the stage and record a clear `STAGE-BLOCKED` marker file (e.g., `final/.stage05-blocked.json`) instead of writing degraded "final" outputs, or
   - (b) write outputs to a clearly-named degraded path (e.g., `architecture-output/final/degraded-run-<timestamp>/`) rather than `architecture-output/final/` directly, so they cannot be mistaken for the canonical output.
3. Either option avoids the current situation where a degraded run's outputs are indistinguishable, by path, from a complete run's outputs.

## WF-FIX-004 — Add explicit run manifest (Priority: Medium)

**Problem:** No single file lists which stages have run, when, against what input file-set, and with what completeness.

**Fix:**
1. Add `architecture-output/run-manifest.json` written/updated by `run_architecture_extraction.py` after each stage, containing per-stage: `stage_id`, `started_at`, `completed_at`, `input_files_expected`, `input_files_found`, `output_files_written`, `status` (`complete`/`degraded`/`blocked`).
2. Stage 06 (quality review) and stage 07 (this audit) should read this manifest first, before reading individual output files, to immediately detect situations like WF-FIX-001/003 without needing to diff file contents.

## WF-FIX-005 — Re-run quality review against current `final/` (Priority: High)

**Problem:** `final/quality-review.md`, `final/executive-summary-for-review.md`, and `final/final-sanity-check.md` were produced against the Generation-A (2-file-input, degraded) version of stage 05 outputs only. Generation B (full-input, 310-component run) has never been quality-reviewed.

**Fix:** Once WF-FIX-001 reconciliation is complete, re-run stage 06 against the reconciled `architecture-output/final/` directory and overwrite `quality-review.md`, `executive-summary-for-review.md`, and `final-sanity-check.md`. Apply WF-FIX-001's archival step to the old stage-06 outputs as well.

## WF-FIX-006 — Confirm/verify project-layout assumptions are configurable (Priority: Medium)

**Problem:** Both output generations consistently reference 6 specific project names (`ApplicationCore`, `Infrastructure`, `Web`, `PublicApi`, `BlazorAdmin`, `BlazorShared`). It is `unknown` whether these are discovered dynamically (e.g., from `.csproj`/`.sln` files, per `01-inventory-agent.md`'s "project files" extraction) or hardcoded into `tools/application_architecture_analyzer/`.

**Fix:** During the next workflow audit, include `tools/application_architecture_analyzer/` source in the audit's input context (it was declared as an input to stage 07 but not provided to this run) and grep for literal occurrences of `"ApplicationCore"`, `"PublicApi"`, `"BlazorAdmin"`, etc. outside of test fixtures. If found in non-discovery code paths, parameterize them via the inventory stage's output instead.

## WF-FIX-007 — Verify regex/heuristic fallback path for non-C# artifacts (Priority: Low)

**Problem:** `02-parser-symbol-agent.md` specifies "language/framework-specific parsing where available, then conservative regex/path heuristics as fallback," but no evidence of the fallback path firing was observed (all evidence is Roslyn-semantic or ASP.NET-specific).

**Fix:** Either (a) confirm via `tools/application_architecture_analyzer/` source that a non-Roslyn fallback exists and is exercised for non-`.cs` files (`.razor`, `.cshtml`, `.json` config, `wwwroot/**`), and add at least one example to a future evidence pack, or (b) if no such fallback exists yet, add it as a backlog item — this repo is currently all-.NET so the gap is latent, not yet impactful.

## WF-FIX-008 — Reconcile stale open-question cross-references (Priority: Medium)

**Problem:** Generation A's `architecture-decision-inputs.md` (AD-005) asks whether Identity/Authentication/Payment capabilities exist, explicitly noting they were not found in its (2-file) input. Generation B's `application-interface-catalogue.json` and `business-capability-map.json` already answer this (CAP-002 Identity, `POST /api/authenticate`, 25 components, 29 interfaces).

**Fix:** As part of WF-FIX-001 reconciliation, re-derive `architecture-decision-inputs.md`, `business-capability-map.md`'s "Open Questions" section, and `forward-engineering-backlog.md` (item FEB-008) from the reconciled Generation-B evidence, removing questions that Generation B already answers.

---

## Summary Table

| ID | Title | Priority | Depends on |
|---|---|---|---|
| WF-FIX-001 | Reconcile/supersede duplicate `final/` outputs | Critical | — |
| WF-FIX-002 | Unify JSON schema per output file | High | — |
| WF-FIX-003 | Enforce stage input completeness before writing | High | — |
| WF-FIX-004 | Add run manifest | Medium | — |
| WF-FIX-005 | Re-run quality review against current `final/` | High | WF-FIX-001 |
| WF-FIX-006 | Verify project-layout assumptions are configurable | Medium | — |
| WF-FIX-007 | Verify regex fallback for non-C# artifacts | Low | — |
| WF-FIX-008 | Reconcile stale open-question cross-references | Medium | WF-FIX-001 |